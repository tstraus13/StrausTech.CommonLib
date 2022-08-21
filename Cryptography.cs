using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CommonLib;

/// <summary>
/// Class to be used for the general encryption and
/// decryption of text/strings
/// </summary>
public static class Cryptography
{
    /// <summary>
    /// Encrypt a string using AES 256-bit encryption
    /// </summary>
    /// <param name="text">The text you want to encrypt</param>
    /// <returns>The details of the encryption. The Encrypted text in a byte array, 
    /// the Key string used during encryption, and the IV used during encryption string</returns>
    public static EncryptionByteDetails EncryptToBytesAes256(string text)
    {
        if (text == null || text.Length <= 0)
            throw new ArgumentNullException("text");

        byte[] encrypted;
        string key;
        string iv;

        using (Aes aesAlg = Aes.Create())
        {
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            key = Encoding.UTF8.GetString(aesAlg.Key);
            iv = Encoding.UTF8.GetString(aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        EncryptionByteDetails details = new EncryptionByteDetails() { EncryptedBytes = encrypted, IV = iv, Key = key};

        return details;
    }

    /// <summary>
    /// Encrypt a string using AES 256-bit encryption with predetermined details (key and iv)
    /// </summary>
    /// <param name="text">The text to encrypt</param>
    /// <param name="key">The key to use during encryption</param>
    /// <param name="iv">The iv to use during encryption</param>
    /// <returns>The encrypted data in a byte array</returns>
    public static byte[] EncryptToBytesAes256(string text, string key, string iv)
    {
        if (text == null || text.Length <= 0)
            throw new ArgumentNullException("text");
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException("key");
        if (iv == null || iv.Length <= 0)
            throw new ArgumentNullException("iv");

        byte[] encrypted;

        using (Aes aesAlg = Aes.Create())
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            Array.Resize(ref keyBytes, 32);

            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            Array.Resize(ref ivBytes, 16);

            aesAlg.Key = keyBytes;
            aesAlg.IV = ivBytes;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return encrypted;
    }

    /// <summary>
    /// Encrypt a string using AES 256-bit encryption
    /// </summary>
    /// <param name="text">The text you want to encrypt</param>
    /// <returns>The details of the encryption. The Encrypted text as a string, 
    /// the Key string used during encryption, and the IV used during encryption string</returns>
    public static EncrytionStringDetails EncryptToStringAes256(string text)
    {
        EncryptionByteDetails encryptedByteDetails = EncryptToBytesAes256(text);
        EncrytionStringDetails encryptedStringDetails = new EncrytionStringDetails() { EncryptedText = Convert.ToBase64String(encryptedByteDetails.EncryptedBytes), IV = encryptedByteDetails.IV, Key = encryptedByteDetails.Key };

        return encryptedStringDetails;
    }

    /// <summary>
    /// Encrypt a string using AES 256-bit encryption with predetermined details (key and iv)
    /// </summary>
    /// <param name="text">The text to encrypt</param>
    /// <param name="key">The key to use during encryption</param>
    /// <param name="iv">The iv to use during encryption</param>
    /// <returns>The encrypted data as a string</returns>
    public static string EncryptToStringAes256(string text, string key, string iv)
    {
        byte[] encryptedBytes = EncryptToBytesAes256(text, key, iv);
        string encryptedString = Convert.ToBase64String(encryptedBytes);

        return encryptedString;
    }

    /// <summary>
    /// Decrypt an AES 256-bit encrypted byte array
    /// </summary>
    /// <param name="encryptedBytes">The encrypted text as a byte array</param>
    /// <param name="key">The key used during encryption</param>
    /// <param name="iv">The iv used during encryption</param>
    /// <returns>The unencrytped text as a string</returns>
    public static string DecryptFromBytesAes256(byte[] encryptedBytes, string key, string iv)
    {
        if (encryptedBytes == null || encryptedBytes.Length <= 0)
            throw new ArgumentNullException("encryptedBytes");
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException("key");
        if (iv == null || iv.Length <= 0)
            throw new ArgumentNullException("iv");

        string plaintext = "";

        using (Aes aesAlg = Aes.Create())
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            Array.Resize(ref keyBytes, 32);

            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            Array.Resize(ref ivBytes, 16);

            aesAlg.Key = keyBytes;
            aesAlg.IV = ivBytes;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
                
            }
        }

        return plaintext;
    }

    /// <summary>
    /// Decrypt an AES 256-bit encrypted string
    /// </summary>
    /// <param name="encryptedString">The encrypted text as a string</param>
    /// <param name="key">The key used during encryption</param>
    /// <param name="iv">The iv used during encryption</param>
    /// <returns>The unencrytped text as a string</returns>
    public static string DecryptFromStringAes256(string encryptedString, string key, string iv)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedString);

        string plaintext = DecryptFromBytesAes256(encryptedBytes, key, iv);

        return plaintext;
    }

    /// <summary>
    /// The struct containing the details of an encryption
    /// with the encrypted text as a byte array
    /// </summary>
    public struct EncryptionByteDetails
    {
        public byte[] EncryptedBytes;
        public string Key;
        public string IV;

        public override string ToString()
        {
            return $"Encrypted Byte Array: {System.Text.Encoding.UTF8.GetString(EncryptedBytes)}\nKey: {Key}\nIV: {IV}";
        }
    }

    /// <summary>
    /// The struct containing the details of an encryption
    /// with the encrypted text as a string
    /// </summary>
    public struct EncrytionStringDetails
    {
        public string EncryptedText;
        public string Key;
        public string IV;

        public override string ToString()
        {
            return $"Encrypted Text: {EncryptedText}\nKey: {Key}\nIV: {IV}";
        }
    }
}