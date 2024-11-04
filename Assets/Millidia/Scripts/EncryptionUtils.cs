using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class EncryptionUtils
{
    public static void CreateKeyAndIV()
    {
        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();
        Debug.LogError(Convert.ToBase64String(aes.Key));
        Debug.LogError(Convert.ToBase64String(aes.IV));
    }

    public static void DecryptFile(string inputFile, string outputFile, string configKeys, string configIv)
    {
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = Convert.FromBase64String(configKeys);
            aes.IV = Convert.FromBase64String(configIv);
            
            using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
            {
                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (CryptoStream cs = new CryptoStream(fsOutput, decryptor, CryptoStreamMode.Write))
                    {
                        using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                        {
                            fsInput.CopyTo(cs);
                        }
                    }
                }
            }
        }
    }
    
    /*
    public static MemoryStream DecryptMemory(string inputFile, string configKeys, string configIv)
    {
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = Convert.FromBase64String(configKeys);
            aes.IV = Convert.FromBase64String(configIv);
            MemoryStream fsOutput = new MemoryStream();
            using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                using (CryptoStream cs = new CryptoStream(fsOutput, decryptor, CryptoStreamMode.Write))
                {
                    using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                    {
                        fsInput.CopyTo(cs);
                    }
                }
            }
        }
        
        
    }
    */

    public static void EncryptFile(string inputFile, string outputFile, string configKeys, string configIv)
    {
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = Convert.FromBase64String(configKeys);
            aes.IV = Convert.FromBase64String(configIv);
            
            using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
            {
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (CryptoStream cs = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                    {
                        using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                        {
                            fsInput.CopyTo(cs);
                        }
                    }
                }
            }
        }
    }
}