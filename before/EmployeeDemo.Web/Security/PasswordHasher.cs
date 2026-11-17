using System.Security.Cryptography;

namespace EmployeeDemo.Web.Security
{
    // PBKDF2-SHA256, 100,000 iterations, 32-byte hash.
    // .NET 8 produces exactly the same bytes with Rfc2898DeriveBytes.Pbkdf2(...),
    // so the same Users table works for both the old and the new app.
    public static class PasswordHasher
    {
        private const int Iterations = 100000;

        public static bool Verify(string password, byte[] salt, byte[] expectedHash)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var actualHash = pbkdf2.GetBytes(expectedHash.Length);
                return FixedTimeEquals(actualHash, expectedHash);
            }
        }

        // Compares every byte, so the time taken does not reveal how many bytes matched.
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            var diff = 0;
            for (var i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }
    }
}
