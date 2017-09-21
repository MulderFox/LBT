// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-25-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-25-2014
// ***********************************************************************
// <copyright file="CryptographyHelper.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LBT.Helpers
{
    /// <summary>
    /// Class CryptographyHelper
    /// </summary>
    public static class Cryptography
    {
        private const string Key = "aXbUxEirA0jdIehE";
        /// <summary>
        /// Encrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>System.String.</returns>
        public static string Encrypt(string data)
        {
            var des = new TripleDESCryptoServiceProvider
                          {
                              Mode = CipherMode.ECB,
                              Key = Encoding.ASCII.GetBytes(Key),
                              Padding = PaddingMode.PKCS7
                          };

            ICryptoTransform desEncrypt = des.CreateEncryptor();
            Byte[] buffer = Encoding.ASCII.GetBytes(data);

            return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }

        /// <summary>
        /// Decrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>System.String.</returns>
        public static string Decrypt(string data)
        {
            try
            {
                var des = new TripleDESCryptoServiceProvider
                {
                    Mode = CipherMode.ECB,
                    Key = Encoding.ASCII.GetBytes(Key),
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform desEncrypt = des.CreateDecryptor();
                Byte[] buffer = Convert.FromBase64String(data.Replace(" ", "+"));

                return Encoding.UTF8.GetString(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                return String.Empty;
            }
        }

        public static string GetRandomPassword()
        {
            var password = new char[7];
            var random = new Random();
            for (int i = 0; i < 3; i++)
            {
                // UpperCase chars
                password[i] = Convert.ToChar(random.Next(65, 91));

                // LowerCase chars
                password[i + 3] = Convert.ToChar(random.Next(97, 123));
            }

            // number
            password[6] = Convert.ToChar(random.Next(48, 58));

            return new String(password.OrderBy(s => (random.Next(2) % 2) == 0).ToArray());
        }
    }
}