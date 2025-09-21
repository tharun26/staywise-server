import fs from "fs";
import csv from "csv-parser";
import pkg from "pg";
import crypto from "crypto";

const { Pool } = pkg;

const pool = new Pool({
  user: "postgres",
  host: "localhost",
  database: "StayWiseDb",
  password: "admin",
  port: 5432,
});

// Helpers
const uuid = () => crypto.randomUUID();

// Track inserted data
const hosts = new Map();    // host_id → userId
const listings = new Map(); // airbnb_id → listingId
const bookings = new Map(); // (listingId, date) → bookingId

async function insertUser(host_id, host_name) {
  if (hosts.has(host_id)) return hosts.get(host_id);

  const userId = uuid();

  // 👇 Ignore if email already exists
  await pool.query(
    `INSERT INTO "Users" ("Id","Name","Email","PasswordHash")
     VALUES ($1,$2,$3,$4)
     ON CONFLICT ("Email") DO NOTHING`,
    [userId, host_name || "Unknown Host", `${host_id}@airbnb.test`, "dummyhash"]
  );

  // ✅ Retrieve existing user if conflict happened
  let finalUserId = userId;
  const result = await pool.query(`SELECT "Id" FROM "Users" WHERE "Email" = $1`, [
    `${host_id}@airbnb.test`,
  ]);
  if (result.rows.length > 0) {
    finalUserId = result.rows[0].Id || result.rows[0].id;
  }

  hosts.set(host_id, finalUserId);
  return finalUserId;
}


async function insertListing(row) {
  const listingId = uuid();
  const hostId = await insertUser(row.host_id, row.host_name);

  // ✅ Parse amenities field into JSON array
  let amenities = [];
  if (row.amenities) {
    try {
      amenities = row.amenities
        .replace(/[{}"]/g, "") // remove braces/quotes
        .split(",")
        .map((a) => a.trim())
        .filter((a) => a.length > 0);
    } catch {
      amenities = [];
    }
  }
  const amenitiesJson = JSON.stringify(amenities);

  // ✅ Collect all photo URLs
  const photoFields = [
    row.picture_url,
    row.thumbnail_url,
    row.xl_picture_url,
  ].filter((url) => url && url.trim() !== "");

  if (photoFields.length === 0) {
    photoFields.push("https://placehold.co/600x400?text=No+Photo");
  }

  const photosJson = JSON.stringify(photoFields);

  await pool.query(
    `INSERT INTO "Listings" (
       "Id","HostId","Title","Description","PricePerNight",
       "MaxGuests","BedRooms","BathRooms",
       "Address_Line1","Address_Line2","Address_City","Address_State",
       "Address_Country","Address_PostalCode","Address_Location_Type","Address_Location_Coordinates",
       "Amenities","Photos"
     )
     VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9,$10,$11,$12,$13,$14,$15,$16,$17,$18)`,
    [
      listingId,
      hostId,
      row.name || "Untitled",
      row.description.substr(0,40) || "",
      parseFloat((row.price || "0").toString().replace(/[^0-9.]/g, "")),
      parseInt(row.accommodates || "1"),
      parseInt(row.bedrooms || "0"),
      parseInt(row.bathrooms || "0"),
      row.street || "Unknown",
      "", // Line2 (CSV doesn’t have it)
      "Amsterdam",
      "Noord-Holland",
      "Netherlands",
      0, // PostalCode missing in CSV
      "Point",
      `{${row.longitude},${row.latitude}}`, // still Postgres array for Location
      amenitiesJson, // ✅ JSON
      photosJson,    // ✅ JSON
    ]
  );

  listings.set(row.id, listingId);
}

async function insertBooking(row) {
  const listingId = listings.get(row.listing_id);
  if (!listingId) return;

  const bookingId = uuid();
  const guestId = uuid();

  await pool.query(
    `INSERT INTO "Users" ("Id","Name","Email","PasswordHash")
     VALUES ($1,$2,$3,$4)
     ON CONFLICT ("Id") DO NOTHING`,
    [guestId, "Guest User", `${guestId}@guest.test`, "dummyhash"]
  );

  const checkIn = new Date(row.date);
  const checkOut = new Date(row.date);
  checkOut.setDate(checkOut.getDate() + 1);

  await pool.query(
    `INSERT INTO "Bookings" ("Id","ListingId","GuestId","CheckIn","CheckOut","TotalPrice","Status")
     VALUES ($1,$2,$3,$4,$5,$6,$7)`,
    [
      bookingId,
      listingId,
      guestId,
      checkIn,
      checkOut,
      parseFloat((row.price || "0").toString().replace(/[^0-9.]/g, "")),
      1, // Status.Confirmed
    ]
  );

  bookings.set(`${listingId}-${row.date}`, bookingId);
}

async function insertReview(row) {
  const listingId = listings.get(row.listing_id);
  if (!listingId) return;

  const bookingKey = `${listingId}-${row.date}`;
  const bookingId = bookings.get(bookingKey);

  const reviewId = uuid();
  const authorId = uuid();

  // Reviewer → fake user
  await pool.query(
    `INSERT INTO "Users" ("Id","Name","Email","PasswordHash")
     VALUES ($1,$2,$3,$4)
     ON CONFLICT ("Id") DO NOTHING`,
    [authorId, row.reviewer_name || "Anonymous", `${row.reviewer_id}@review.test`, "dummyhash"]
  );

  await pool.query(
    `INSERT INTO "Reviews" ("Id","ListingId","BookingId","AuthorId","Rating","Comment")
     VALUES ($1,$2,$3,$4,$5,$6)`,
    [
      reviewId,
      listingId,
      bookingId || null,
      authorId,
      Math.floor(Math.random() * 5) + 1, // random rating
      row.comments.substr(0,20) || "",
    ]
  );
}
async function main() {
  console.log("⏳ Loading listings...");
  await new Promise((resolve) => {
    fs.createReadStream("listings.csv")
      .pipe(csv())
      .on("data", (row) => insertListing(row))
      .on("end", resolve);
  });

  // console.log("⏳ Loading bookings...");
  await new Promise((resolve) => {
    fs.createReadStream("calendar.csv")
      .pipe(csv())
      .on("data", (row) => {
        if (row.available === "f") insertBooking(row);
      })
      .on("end", resolve);
  });

  console.log("⏳ Loading reviews...");
  await new Promise((resolve) => {
    fs.createReadStream("reviews.csv")
      .pipe(csv())
      .on("data", (row) => insertReview(row))
      .on("end", resolve);
  });

  console.log("✅ Import complete!");
  await pool.end();
}

main();
