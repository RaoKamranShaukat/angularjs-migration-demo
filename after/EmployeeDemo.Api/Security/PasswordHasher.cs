using System.Security.Cryptography;

namespace EmployeeDemo.Api.Security;

// PBKDF2-SHA256, 100,000 iterations, 32-byte hash. Byte-for-byte the same as the .NET Framework
// version, so users created by the old app can sign in to the new one (same Users table).
//   BEFORE: new Rfc2898DeriveBytes(...).GetBytes(32) + a hand-written constant-time compare
//   AFTER:  one static call + the built-in CryptographicOperations.FixedTimeEquals
public static class PasswordHasher
{
    private const int Iterations = 100_000;

    public static bool Verify(string password, byte[] salt, byte[] expectedHash)
    {
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, expectedHash.Length);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
