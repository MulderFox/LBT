using NUnit.Framework;

namespace LBT.Tests.Cache
{
    [TestFixture]
    public class BankAccountCacheTests
    {
        [Test]
        public void GetPaymentInfoRowTest()
        {
            string text = "Účet: {0}";
            const string arg = "12345/1234";
            string rowText = LBT.Cache.BankAccountCache.GetPaymentInfoRow(text, arg);
            string expectedRowText = "  Účet: 12345/1234";

            Assert.IsTrue(rowText == expectedRowText);

            text = "Cena: {0:N0} {1}";
            const decimal arg1 = 100;
            const string arg2 = "CZK";
            rowText = LBT.Cache.BankAccountCache.GetPaymentInfoRow(text, arg1, arg2);
            expectedRowText = "  Cena: 100 CZK";

            Assert.IsTrue(rowText == expectedRowText);
        }
    }
}
