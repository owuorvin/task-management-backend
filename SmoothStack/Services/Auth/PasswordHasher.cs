using System.Security.Cryptography;

namespace SmoothStack.Services.Auth
{
    public class PasswordHasher: IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt;
            using (var rng = RandomNumberGenerator.Create())
            {
                salt = new byte[SaltSize];
                rng.GetBytes(salt);
            }

            // Hash the password with the salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            // Combine salt and hash
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                var hashBytes = Convert.FromBase64String(hash);

                // Extract salt from the stored hash
                var salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                // Compute hash of the provided password
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
                var computedHash = pbkdf2.GetBytes(HashSize);

                // Compare the computed hash with stored hash
                for (int i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i + SaltSize] != computedHash[i])
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
