using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace LBT.Cache
{
    public class BankAccountHistoryCache : BaseCache
    {
        public static BankAccountHistory[] GetIndex(DefaultContext db, int? bankAccountId, int? secondBankAccountId)
        {
            IQueryable<BankAccountHistory> bankAccountHistories = db.BankAccountHistories.Where(ba => ba.BankAccountId == bankAccountId || ba.BankAccountId == secondBankAccountId);
            return bankAccountHistories.ToArray();
        }

        public static BankAccountHistory[] GetIndex(DefaultContext db, BankAccountType bankAccountType)
        {
            BankAccountHistory[] bankAccountHistories = db.BankAccountHistories.Where(bah => bah.BankAccount.BankAccountType == bankAccountType).ToArray();
            return bankAccountHistories;
        }

        public static BankAccountHistory GetDetail(DefaultContext db, int id)
        {
            BankAccountHistory bankAccountHistory = db.BankAccountHistories.Find(id);
            return bankAccountHistory;
        }

        /// <summary>
        /// Processes the transaction request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="bankAccountId">The bank account id.</param>
        /// <param name="bankAccountHistories">The bank account histories.</param>
        /// <param name="lastDownloadId">The last download id.</param>
        /// <returns>HttpStatusCode.</returns>
        /// <exception cref="System.Exception"></exception>
        public static HttpStatusCode ProcessTransactionRequest(string url, int bankAccountId, out BankAccountHistory[] bankAccountHistories, out Int64? lastDownloadId)
        {
            bankAccountHistories = new BankAccountHistory[0];
            lastDownloadId = null;

            try
            {
                InternetResponse internetResponse = InternetRequest.SendRequest(url, InternetRequestType.ProcessXDocument);
                if (internetResponse.XDocument == null)
                    return internetResponse.HttpStatusCode;

                bankAccountHistories = internetResponse.XDocument
                    .Descendants("AccountStatement")
                    .Descendants("TransactionList")
                    .Descendants("Transaction")
                    .Select(t =>
                    {
                        XElement transactionId = t.Element("column_22");
                        XElement date = t.Element("column_0");
                        XElement ammount = t.Element("column_1");
                        XElement currency = t.Element("column_14");

                        if (transactionId == null || date == null || ammount == null || currency == null)
                            throw new Exception("Communication XML structure corrupted.");

                        var bankAccountHistory = new BankAccountHistory
                        {
                            BankAccountId = bankAccountId,
                            TransactionId = Convert.ToInt64(transactionId.Value),
                            Date = DateTime.ParseExact(date.Value, "yyyy-MM-ddzzz", CultureInfo.InvariantCulture),
                            Ammount = Convert.ToDecimal(ammount.Value, new CultureInfo("en-US")),
                            Currency = currency.Value
                        };

                        XElement xElementOptional = t.Element("column_2");
                        if (xElementOptional != null)
                            bankAccountHistory.Exchange = xElementOptional.Value;

                        xElementOptional = t.Element("column_3");
                        if (xElementOptional != null)
                            bankAccountHistory.BankCode = xElementOptional.Value;

                        xElementOptional = t.Element("column_12");
                        if (xElementOptional != null)
                            bankAccountHistory.BankName = xElementOptional.Value;

                        xElementOptional = t.Element("column_4");
                        int tempNumber;
                        if (xElementOptional != null && Int32.TryParse(xElementOptional.Value, out tempNumber))
                            bankAccountHistory.Ks = tempNumber;

                        xElementOptional = t.Element("column_5");
                        Int64 tempDwordNumber;
                        if (xElementOptional != null && Int64.TryParse(xElementOptional.Value, out tempDwordNumber))
                            bankAccountHistory.Vs = tempDwordNumber;

                        xElementOptional = t.Element("column_6");
                        if (xElementOptional != null && Int32.TryParse(xElementOptional.Value, out tempNumber))
                            bankAccountHistory.Ss = tempNumber;

                        xElementOptional = t.Element("column_16");
                        if (xElementOptional != null)
                            bankAccountHistory.Note = xElementOptional.Value;

                        return bankAccountHistory;
                    }).ToArray();

                long[] lastDownloadIds;
                if (bankAccountHistories.Length == 0)
                {
                    lastDownloadIds = internetResponse.XDocument
                    .Descendants("AccountStatement")
                    .Descendants("Info")
                    .Select(t =>
                    {
                        XElement idLastDownload = t.Element("idLastDownload");

                        if (idLastDownload == null)
                            throw new Exception("Communication XML structure corrupted.");

                        return Convert.ToInt64(idLastDownload.Value);
                    }).ToArray();
                }
                else
                {
                    lastDownloadIds = internetResponse.XDocument
                    .Descendants("AccountStatement")
                    .Descendants("Info")
                    .Select(t =>
                    {
                        XElement idTo = t.Element("idTo");

                        if (idTo == null)
                            throw new Exception("Communication XML structure corrupted.");

                        return Convert.ToInt64(idTo.Value);
                    }).ToArray();
                }

                if (lastDownloadIds.Length != 1)
                    return HttpStatusCode.ServiceUnavailable;

                lastDownloadId = lastDownloadIds[0];

                return HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                Logger.SetLog(e, new Logger.LogParameter { AdditionalMessage = String.Format("Url: {0}", url) });
                return HttpStatusCode.Conflict;
            }
        }
    }
}