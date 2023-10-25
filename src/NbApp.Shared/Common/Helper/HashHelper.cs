using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Common
{
    public interface IHashHelper
    {
        HashAlgorithm GetMd5();
        HashAlgorithm GetSHA256();
    }

    public class HashHelper : IHashHelper
    {
        public static IHashHelper Instance = new HashHelper();

        internal MD5 Md5 { get; set; } = MD5.Create();
        public HashAlgorithm GetMd5()
        {
            return Md5;
        }

        internal SHA256 SHA256 { get; set; } = SHA256.Create();
        public HashAlgorithm GetSHA256()
        {
            return SHA256;
        }
    }

    public static class HashExtensions
    {
        public static string ComputeFileHash(this HashAlgorithm hashAlgorithm, string filePath, string notExistsReturn = null)
        {
            if (!File.Exists(filePath))
            {
                return notExistsReturn;
            }
            var buffer = File.ReadAllBytes(filePath);
            return ComputeHashString(hashAlgorithm, buffer, 0, buffer.Length);
        }

        public static string ComputeHashString(this HashAlgorithm hashAlgorithm, string input)
        {
            var buffer = Encoding.UTF8.GetBytes(input);
            return ComputeHashString(hashAlgorithm, buffer, 0, buffer.Length);
        }

        public static string ComputeHashString(this HashAlgorithm hashAlgorithm, byte[] buffer, int offset, int count)
        {
            byte[] data = hashAlgorithm.ComputeHash(buffer, offset, count);
            var sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

    public class HashFileInfo
    {
        public string File { get; set; }
        public string Hash { get; set; }
        public static HashFileInfo TryCreate(string filePath)
        {
            var info = new HashFileInfo();
            try
            {
                info.Hash = HashHelper.Instance.GetMd5().ComputeFileHash(filePath, "");
                info.File = filePath;
                return info;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}