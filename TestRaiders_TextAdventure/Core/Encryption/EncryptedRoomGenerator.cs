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
            // Try to resolve the file from several likely locations
            string? resolved = FindPlaintextFile(inputPath);
            if (resolved == null)
            {
                Console.WriteLine($"ERROR: Missing plaintext file: {inputPath}");
                Console.WriteLine("Searched working directory and output folders. To fix:");
                Console.WriteLine("- In Visual Studio set the file's __Build Action__ = __Content__");
                Console.WriteLine("  and __Copy to Output Directory__ = __Copy if newer__.");
                return;
            }

            string plaintext = File.ReadAllText(resolved);

            // Derive encryption key from keyshare + passphrase
            byte[] key = DeriveKey(Keyshare, Passphrase);

            // Random IV for AES-CBC
            byte[] iv = RandomNumberGenerator.GetBytes(16);

            byte[] encryptedBytes = EncryptAES(plaintext, key, iv);

            // FORMAT: [IV][CIPHERTEXT]
            using var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            fs.Write(iv, 0, iv.Length);
            fs.Write(encryptedBytes, 0, encryptedBytes.Length);

            Console.WriteLine($"{outputPath} generated successfully from {resolved}.");
        }

        // Attempts to find the plaintext file from several locations:
        // 1) as given, 2) in AppContext.BaseDirectory, 3) recursive search under base directory,
        // 4) walk up parents searching for the file in project tree.
        private static string? FindPlaintextFile(string inputPath)
        {
            // 1) as given
            if (File.Exists(inputPath))
                return Path.GetFullPath(inputPath);

            // 2) in the runtime base directory (bin/**)
            string baseDir = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
            string candidate = Path.Combine(baseDir, inputPath);
            if (File.Exists(candidate))
                return candidate;

            // 3) recursive search under base directory (may be expensive but limited to a single pattern)
            try
            {
                var found = Directory.EnumerateFiles(baseDir, Path.GetFileName(inputPath), SearchOption.AllDirectories).FirstOrDefault();
                if (found != null)
                    return found;
            }
            catch { /* ignore IO exceptions during search */ }

            // 4) walk up a few parent directories (project root scenarios)
            var dir = new DirectoryInfo(baseDir);
            for (int i = 0; i < 6 && dir.Parent != null; i++)
            {
                dir = dir.Parent;
                string check = Path.Combine(dir.FullName, inputPath);
                if (File.Exists(check))
                    return check;

                // also check common project relative path
                string alt = Path.Combine(dir.FullName, "TestRaiders_TextAdventure", "Core", "Encryption", inputPath);
                if (File.Exists(alt))
                    return alt;
            }

            // Not found
            return null;
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
