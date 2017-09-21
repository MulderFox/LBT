using LBT.Helpers;
using NUnit.Framework;
using System;

namespace LBT.Tests.Helpers
{
    [TestFixture]
    public class CryptographyTests
    {
        private const string DecryptedMailServer = "w8HYFVCV";
        private const string EncryptedMailServer = "6KB9ZvuyuY7xgSqLhJFhBQ==";

        [Test]
        public void EncryptTest()
        {
            string encrypted = Cryptography.Encrypt(DecryptedMailServer);
            Assert.IsTrue(encrypted.Equals(EncryptedMailServer, StringComparison.CurrentCulture));
        }

        [Test]
        public void DecryptTest()
        {
            string decrypted = Cryptography.Decrypt(EncryptedMailServer);
            Assert.IsTrue(decrypted.Equals(DecryptedMailServer, StringComparison.CurrentCulture));
        }

        [Test]
        public void GetRandomPasswordTest()
        {
            Assert.Fail();
        }
    }
}
