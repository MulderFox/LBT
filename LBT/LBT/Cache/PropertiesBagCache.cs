using LBT.DAL;
using LBT.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LBT.Cache
{
    public class PropertiesBagCache : BaseCache
    {
        private const string ClaAccessInvoiceNumberKey = "ClaAccessInvoiceNumber";

        private static List<PropertiesBag> _propertiesBags;
        private static readonly object LockPropertiesBags = new object();

        private static readonly object LockClaAccessInvoiceNumber = new object();

        public static string GetClaAccessInvoiceNumber(DefaultContext db)
        {
            string newInvoiceNumberValue;
            lock (LockClaAccessInvoiceNumber)
            {
                string invoiceNumberValue = GetPropertiesBagItem(db, ClaAccessInvoiceNumberKey);
                var invoiceNumber = new InvoiceNumber(invoiceNumberValue);
                newInvoiceNumberValue = invoiceNumber.Next();
                SavePropertiesBagItem(db, ClaAccessInvoiceNumberKey, newInvoiceNumberValue, false);
            }

            return newInvoiceNumberValue;
        }

        public static string GetPropertiesBagItem(DefaultContext db, string key)
        {
            lock (LockPropertiesBags)
            {
                if (_propertiesBags == null)
                {
                    _propertiesBags = db.PropertiesBags.ToList();
                }

                PropertiesBag propertiesBag = _propertiesBags.SingleOrDefault(pbi => pbi.Key.Equals(key));
                return propertiesBag == null ? null : propertiesBag.Value;
            }
        }

        public static void SavePropertiesBagItem(DefaultContext db, string key, string value, bool saveChangesToDb)
        {
            lock (LockPropertiesBags)
            {
                PropertiesBag propertiesBag = _propertiesBags.SingleOrDefault(pb => pb.Key.Equals(key));
                if (propertiesBag == null)
                {
                    propertiesBag = new PropertiesBag { Key = key, Value = value };
                    _propertiesBags.Add(propertiesBag);
                }
                else
                {
                    propertiesBag.Value = value;
                }

                PropertiesBag dbPropertiesBag = db.PropertiesBags.Find(key);
                if (dbPropertiesBag == null)
                {
                    dbPropertiesBag = new PropertiesBag { Key = key, Value = value };
                    db.PropertiesBags.Add(dbPropertiesBag);
                }
                else
                {
                    dbPropertiesBag.Value = value;
                }

                if (saveChangesToDb)
                {
                    db.SaveChanges();                    
                }
            }
        }

        private class InvoiceNumber
        {
            private string Index { get { return _indexNumber.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0'); } }

            private readonly string _actualYear;
            private string _year;
            private int _indexNumber;
            private bool _isValid;

            private const string InvoiceFormat = @"{0}01{1}";

            public InvoiceNumber(string invoiceNumber)
            {
                _isValid = false;
                _actualYear = DateTime.Today.ToString("yy");
                Validate(invoiceNumber);
            }

            public string Next()
            {
                return _isValid ? GenerateNext() : CreateNew();
            }

            private void Validate(string invoiceNumber)
            {
                if (String.IsNullOrEmpty(invoiceNumber) || invoiceNumber.Length != 9)
                    return;

                _year = invoiceNumber.Substring(0, 2);
                if (!_year.Equals(_actualYear))
                    return;

                string index = invoiceNumber.Substring(4, 5);
                if (!Int32.TryParse(index, out _indexNumber))
                    return;

                _isValid = true;
            }

            private string GenerateNext()
            {
                _indexNumber++;
                string nextInvoiceNumber = String.Format(InvoiceFormat, _year, Index);
                return nextInvoiceNumber;
            }

            private string CreateNew()
            {
                _indexNumber = 1;
                _year = _actualYear;
                _isValid = true;
                string newInvoiceNumber = String.Format(InvoiceFormat, _year, Index);
                return newInvoiceNumber;
            }
        }
    }
}