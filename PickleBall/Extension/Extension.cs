
using System.Security.Cryptography;
using System.Text;

namespace PickleBall.Extension
{
    public static class Extension
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> values, int page, int pageSize)
        {
            return values.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IEnumerable<T> Paging<T>(this IEnumerable<T> values, int page, int pageSize)
        {
            return values.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static string HashRefreshToken(this string token)
        {
            using var sha256 = SHA256.Create();
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var hashToken = Convert.ToBase64String(sha256.ComputeHash(tokenBytes));

            return hashToken;
        }
    }
}
