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
const listings = new Map(); // airbnb_listing_id → our internal ListingId

async function insertUser(host_id, host_name) {
  if (hosts.has(host_id)) return hosts.get(host_id);

  const userId = uuid();

  await pool.query(
    `INSERT INTO "Users" ("Id","Name","Email","PasswordHash")
     VALUES ($1,$2,$3,$4)
     ON CONFLICT ("Email") DO NOTHING`,
    [userId, host_name || "Unknown Host", `${host_id}@airbnb.test`, "dummyhash"]
  );

  let finalUserId = userId;
  const result = await pool.query(
    `SELECT "Id" FROM "Users" WHERE "Email" = $1 LIMIT 1`,
    [`${host_id}@airbnb.test`]
  );
  if (result.rows.length > 0) {
    finalUserId = result.rows[0].id ?? result.rows[0].Id;
  }

  hosts.set(host_id, finalUserId);
  return finalUserId;
}

async function insertListing(row) {
  const listingId = uuid();
  const hostId = await insertUser(row.host_id, row.host_name);

  let amenities = [];
  if (row.amenities) {
    try {
      amenities = row.amenities
        .replace(/[{}"]/g, "")
        .split(",")
        .map((a) => a.trim())
        .filter((a) => a.length > 0);
    } catch {
      amenities = [];
    }
  }
  const amenitiesJson = JSON.stringify(amenities);

  const photoFields = [
    row.picture_url,
    row.thumbnail_url,
    row.xl_picture_url,
  ].filter((url) => url && url.trim() !== "");
  if (photoFields.length === 0) {
    photoFields.push("https://placehold.co/600x400?text=No+Photo");
  }
  const photosJson = JSON.stringify(photoFields);

  const addressId = uuid();

  try {
    await pool.query(
      `INSERT INTO "Addresses" (
         "Id","Line1","Line2","City","State","Country","PostalCode","Location_Type","Location_Coordinates"
       ) VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9)`,
      [
        addressId,
        row.street || "Unknown",
        "",
        "Amsterdam",
        "Noord-Holland",
        "Netherlands",
        0,
        "Point",
        `{${row.longitude},${row.latitude}}`,
      ]
    );

    await pool.query(
      `INSERT INTO "Listings" (
         "Id","HostId","AddressId","Title","Description","PricePerNight",
         "MaxGuests","BedRooms","BathRooms","Amenities","Photos"
       )
       VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9,$10,$11)`,
      [
        listingId,
        hostId,
        addressId,
        row.name || "Untitled",
        row.description?.slice(0, 40) || "",
        parseFloat((row.price || "0").toString().replace(/[^0-9.]/g, "")),
        parseInt(row.accommodates || "1"),
        parseInt(row.bedrooms || "0"),
        parseInt(row.bathrooms || "0"),
        amenitiesJson,
        photosJson,
      ]
    );

    // 👇 IMPORTANT: Use the same key as reviews.csv (usually "id")
    const externalId = row.listing_id || row.id;
    listings.set(externalId, listingId);
  } catch (err) {
    console.error("❌ Failed to insert listing:", err.message, row.id || row.listing_id);
  }
}

async function insertReview(row) {
  const externalId = row.listing_id || row.id;
  const listingId = listings.get(externalId);

  if (!listingId) {
    console.warn("⚠️ Skipping review: no listing for", externalId);
    return;
  }

  const reviewId = uuid();
  let authorId = uuid();
  const email = `${row.reviewer_id || reviewId}@review.test`;

  try {
    await pool.query(
      `INSERT INTO "Users" ("Id","Name","Email","PasswordHash")
       VALUES ($1,$2,$3,$4)
       ON CONFLICT ("Email") DO NOTHING`,
      [authorId, row.reviewer_name || "Anonymous", email, "dummyhash"]
    );

    const result = await pool.query(
      `SELECT "Id" FROM "Users" WHERE "Email" = $1 LIMIT 1`,
      [email]
    );
    if (result.rows.length > 0) {
      authorId = result.rows[0].id ?? result.rows[0].Id;
    }

    await pool.query(
      `INSERT INTO "Reviews" ("Id","ListingId","BookingId","AuthorId","Rating","Comment")
       VALUES ($1,$2,$3,$4,$5,$6)`,
      [
        reviewId,
        listingId,
        null, // no booking link for now
        authorId,
        Math.floor(Math.random() * 5) + 1,
        row.comments ? row.comments.slice(0, 200) : "",
      ]
    );
  } catch (err) {
    console.error("❌ Failed to insert review:", err.message, externalId);
  }
}

async function loadCsv(file, handler) {
  const tasks = [];
  return new Promise((resolve, reject) => {
    fs.createReadStream(file)
      .pipe(csv())
      .on("data", (row) => tasks.push(handler(row)))
      .on("end", async () => {
        try {
          await Promise.all(tasks);
          resolve();
        } catch (err) {
          reject(err);
        }
      });
  });
}

async function main() {
  console.log("⏳ Loading listings...");
  await loadCsv("listings.csv", insertListing);
  console.log("✅ Total listings:", listings.size);

  console.log("⏳ Loading reviews...");
  await loadCsv("reviews.csv", insertReview);

  console.log("✅ Import complete!");
  await pool.end();
}

main();
