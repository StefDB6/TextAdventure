using System.Text;
using System.Security.Cryptography;

namespace TextAdventureApi.Security
{
    public class Sha256Hasher
    {
        public static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes); // bv. "A1B2C3..."
        }
    }
}
