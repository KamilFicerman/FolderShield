using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace FolderShieldLib
{
    //all the cryptography stuff
    public static class ShieldEncryptor
    {
        public static byte[] GenerateKeyToEncrypt(string filePath)
        {
            int counter = 0; //counter for recursive calls
            byte[] salt = new byte[8];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(salt);
            }
            try
            {
                SaveSaltToFile(salt, filePath);
            }
            catch (UnauthorizedAccessException)
            {
                //sometimes SaveSaltToFile throws UnauthorizedAccessException that's
                //why we try again 
                if (counter < 6)
                {
                    Thread.Sleep(200);
                    SaveSaltToFile(salt, filePath);
                    counter++;
                }

            }
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(Shield._password, salt, 100, HashAlgorithmName.SHA256);
            return key.GetBytes(16);
        }
        public static byte[] GenerateKeyToDecrypt(string filePath)
        {
            byte[] salt = ReadSaltFromFile(filePath);
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(Shield._password, salt, 100, HashAlgorithmName.SHA256);
            return key.GetBytes(16);
        }
        public static void SaveSaltToFile(byte[] salt, string filePath)
        {
            string fpath = filePath + ".salt";
            File.WriteAllBytes(fpath, salt);
            File.SetAttributes(fpath, File.GetAttributes(fpath) | FileAttributes.Hidden);
        }
        public static byte[] ReadSaltFromFile(string filePath)
        {
            string fpath = filePath + ".salt";
            return File.ReadAllBytes(fpath);
        }
        public static void DeleteSalt(string filePath)
        {
            string fpath = filePath + ".salt";
            File.Delete(fpath);
        }
        public static void SaveIVToFile(byte[] iv, string filePath)
        {
            string fpath = filePath + ".iv";
            File.WriteAllBytes(fpath, iv);
            File.SetAttributes(fpath, File.GetAttributes(fpath) | FileAttributes.Hidden);
        }

        public static byte[] ReadIVFromFile(string filePath)
        {
            string fpath = filePath + ".iv";
            return File.ReadAllBytes(fpath);
        }
        public static void DeleteIV(string filePath)
        {
            string fpath = filePath + ".iv";
            File.Delete(fpath);
        }
        public static void EncryptFile(string inputFile, string outputFile, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                SaveIVToFile(aesAlg.IV, inputFile);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (FileStream inputFileStream = new FileStream(inputFile, FileMode.Open))
                {
                    using (FileStream outputFileStream = new FileStream(outputFile, FileMode.Create))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write))
                        {
                            inputFileStream.CopyTo(cryptoStream);
                        }
                    }
                }
            }
        }
        public static void DecryptFile(string inputFile, string outputFile, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = ReadIVFromFile(outputFile);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (FileStream inputFileStream = new FileStream(inputFile, FileMode.Open))
                {
                    using (FileStream outputFileStream = new FileStream(outputFile, FileMode.Create))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(inputFileStream, decryptor, CryptoStreamMode.Read))
                        {
                            cryptoStream.CopyTo(outputFileStream);
                        }
                    }
                }
            }
        }
    }
}
