using System;
using System.Security.Cryptography;
using System.Text;

namespace PTMS.Utilities
{
    public static class PasswordHelper
    {
        // Hash password with salt
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Generate a random salt
                byte[] salt = new byte[16];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt);
                }

                // Combine password and salt
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                // Hash the salted password
                byte[] hashBytes = sha256Hash.ComputeHash(saltedPassword);

                // Combine salt and hash
                byte[] hashWithSalt = new byte[hashBytes.Length + salt.Length];
                Buffer.BlockCopy(salt, 0, hashWithSalt, 0, salt.Length);
                Buffer.BlockCopy(hashBytes, 0, hashWithSalt, salt.Length, hashBytes.Length);

                return Convert.ToBase64String(hashWithSalt);
            }
        }

        // Verify password
        public static bool VerifyPassword(string password, string hash)
        {
            try
            {
                byte[] hashWithSalt = Convert.FromBase64String(hash);
                
                // Extract salt (first 16 bytes)
                byte[] salt = new byte[16];
                Buffer.BlockCopy(hashWithSalt, 0, salt, 0, 16);

                // Extract stored hash (remaining bytes)
                byte[] storedHash = new byte[hashWithSalt.Length - 16];
                Buffer.BlockCopy(hashWithSalt, 16, storedHash, 0, storedHash.Length);

                // Compute hash of provided password with extracted salt
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                    byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];
                    Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                    Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                    byte[] computedHash = sha256Hash.ComputeHash(saltedPassword);

                    // Compare hashes
                    if (computedHash.Length != storedHash.Length)
                        return false;

                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != storedHash[i])
                            return false;
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

