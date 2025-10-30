using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    private static readonly string encryptionKey = "YourSecretKey123"; // must be 16/24/32 chars for AES

    public static string Encrypt(string plainText)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cs))
        {
            writer.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string cipherText)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherText);
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

        using var aes = Aes.Create();
        byte[] iv = new byte[aes.BlockSize / 8];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);

        using var decryptor = aes.CreateDecryptor(keyBytes, iv);
        using var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);

        return reader.ReadToEnd();
    }
}
