using LBT.Cache;
using LBT.DAL;
using LBT.ModelViews;
using LBT.Models;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace LBT.Tests.ModelViews
{
    [TestFixture]
    public class BankAccountCommonTests
    {
        [Test]
        public void GetInvoiceTest()
        {
            using(var db = new DefaultContext())
            {
                var userProfile = db.UserProfiles.SingleOrDefault(u => u.UserName.Equals("dzccele", StringComparison.InvariantCultureIgnoreCase));
                if (userProfile == null)
                    throw new Exception("UserProfile cannot be null.");

                var bankAccountForClaAccess = BankAccountCache.GetDetail(db, BankAccountType.ApplicationAccess, userProfile.ClaAccessCurrency);
                var currentDate = DateTime.Now.Date;
                string invoiceNumber;
                string invoiceHtml = BankAccountCommon.GetInvoice(db, userProfile, bankAccountForClaAccess, currentDate, out invoiceNumber);

                File.WriteAllText("Invoice.html", invoiceHtml);
            }
        }
    }
}
