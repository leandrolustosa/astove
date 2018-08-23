using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AInBox.Astove.Core.Extensions
{
    public static class StringExtension
    {
        public static string ToCamelCase(this string source)
        {
            // If there are 0 or 1 characters, just return the string.
            if (source == null || source.Length < 2)
                return source;

            if (source.Equals(source.ToUpper()))
                return source.ToLower();

            // Split the string into words.
            string[] words = Regex.Split(source, @"(?<!^)(?=[A-Z])");

            // Combine the words.
            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }

            return result;
        }

        public static string ToPascalCase(this string source)
        {
            // If there are 0 or 1 characters, just return the string.
            if (source == null) return source;
            if (source.Length < 2) return source.ToUpper();

            // Split the string into words.
            string[] words = Regex.Split(source, @"(?<!^)(?=[A-Z])");

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1);
            }

            return result;
        }

        // Capitalize the first character and add a space before
        // each capitalized letter (except the first character).
        public static string ToProperCase(this string source)
        {
            // If there are 0 or 1 characters, just return the string.
            if (source == null) return source;
            if (source.Length < 2) return source.ToUpper();

            // Split the string into words.
            string[] words = source.ToLower().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1) + " ";
            }

            return result.Trim();
        }

        public static string ToSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            //str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim it   
            str = str.Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   

            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        private static readonly string cryptoKey = "@InB0x6!35";
        private static readonly byte[] IV = new byte[8] { 240, 3, 45, 29, 0, 76, 173, 59 };

        public static string Encrypt(this string s)
        {
            return Encrypt(s, cryptoKey);
        }

        public static string Decrypt(this string s)
        {
            return Decrypt(s, cryptoKey);
        }

        public static string Encrypt(this string s, string key)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(s);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
            des.IV = IV;
            return Convert.ToBase64String(
                des.CreateEncryptor().TransformFinalBlock(
                    buffer,
                    0,
                    buffer.Length
                )
            );
        }

        public static string Decrypt(this string s, string key)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            byte[] buffer = Convert.FromBase64String(s);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
            des.IV = IV;
            return Encoding.ASCII.GetString(
                des.CreateDecryptor().TransformFinalBlock(
                    buffer,
                    0,
                    buffer.Length
                )
            );
        }

        public static int ToInt32(this string s)
        {
            int i;
            int.TryParse(s, out i);
            return i;
        }
    }
}
