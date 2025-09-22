using System.Security.Cryptography.X509Certificates;

namespace Staywise.Dtos;

public class ReviewResponseDto<T> where T : class
{
    public T? Data { get; set; } = null;

    public int Limit { get; set; }
    public int Page { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
    
}