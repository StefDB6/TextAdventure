using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TestRaiders_TextAdventure.Core.Encryption
{
    public static class EncryptedRoomReader
    {
        public static string? TryDecrypt(string path, string keyshare, string passphrase)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            byte[] fileBytes = File.ReadAllBytes(path);

            // First 16 bytes = IV
            byte[] iv = fileBytes[..16];
            byte[] ciphertext = fileBytes[16..];

            byte[] key = DeriveKey(keyshare, passphrase);

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            try
            {
                using var decryptor = aes.CreateDecryptor();
                byte[] plaintextBytes = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
                return Encoding.UTF8.GetString(plaintextBytes);
            }
            catch
            {
                return null; // Wrong key or passphrase
            }
        }

        private static byte[] DeriveKey(string ks, string pass)
        {
            using var sha = SHA256.Create();
            string combined = ks + ":" + pass;
            return sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
        }
    }
}
