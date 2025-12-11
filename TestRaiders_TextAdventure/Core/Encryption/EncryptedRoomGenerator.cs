using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestRaiders_TextAdventure.Core.Encryption
{
    public static class EncryptedRoomGenerator
    {
        private const string Keyshare = "ABC-EFG-HIJK";
        private const string Passphrase = "coolpasswoord";

        // Your input plaintext files
        private static readonly string ThroneTxt = "throne.txt";
        private static readonly string SealTxt = "seal.txt";

        // Output encrypted files
        private static readonly string ThroneEnc = "throne.enc";
        private static readonly string SealEnc = "seal.enc";


        /// Automatically generates .enc files if they do not exist yet.
        public static void EnsureEncryptedRoomsExist()
        {
            if (!File.Exists(ThroneEnc))
            {
                Console.WriteLine("throne.enc missing → generating...");
                GenerateEncryptedFile(ThroneTxt, ThroneEnc);
            }

            if (!File.Exists(SealEnc))
            {
                Console.WriteLine("seal.enc missing → generating...");
                GenerateEncryptedFile(SealTxt, SealEnc);
            }
        }

        /// Encrypts a plaintext file using AES(CBC) + SHA256(keyshare:passphrase) derived key.
        private static void GenerateEncryptedFile(string inputPath, string outputPath)
        {
            if (!File.Exists(inputPath))
            {
                Console.WriteLine($"ERROR: Missing plaintext file: {inputPath}");
                return;
            }

            string plaintext = File.ReadAllText(inputPath);

            // Derive encryption key from keyshare + passphrase
            byte[] key = DeriveKey(Keyshare, Passphrase);

            // Random IV for AES-CBC
            byte[] iv = RandomNumberGenerator.GetBytes(16);

            byte[] encryptedBytes = EncryptAES(plaintext, key, iv);

            // FORMAT: [IV][CIPHERTEXT]
            using var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            fs.Write(iv, 0, iv.Length);
            fs.Write(encryptedBytes, 0, encryptedBytes.Length);

            Console.WriteLine($"{outputPath} generated successfully.");
        }

        /// SHA256(keyshare + ":" + passphrase)
        private static byte[] DeriveKey(string keyshare, string passphrase)
        {
            string combined = keyshare + ":" + passphrase;
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
        }

        /// AES-CBC encryption with PKCS7 padding.
        private static byte[] EncryptAES(string plaintext, byte[] key, byte[] iv)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] inputBytes = Encoding.UTF8.GetBytes(plaintext);
            using var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
        }
    }
}
