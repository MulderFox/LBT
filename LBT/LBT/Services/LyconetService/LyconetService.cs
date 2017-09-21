using LBT.DAL;
using LBT.Models;
using Microsoft.VisualBasic.FileIO;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;

namespace LBT.Services.LyconetService
{
    public class LyconetService : IDisposable
    {
        public string SummaryMessage
        {
            get { return _summaryMesage.ToString(); }
        }

        private readonly DataTable _csvData;
        private readonly StringBuilder _summaryMesage;
        private readonly DefaultContext _db;
        private readonly int _userId;

        public LyconetService(DefaultContext db, int userId)
        {
            _csvData = new DataTable();
            _summaryMesage = new StringBuilder();
            _db = db;
            _userId = userId;
        }

        ~LyconetService()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _csvData.Dispose();
        }

        public void ReadDataFromCsvFile(Stream fileStream)
        {
            using (var csvReader = new TextFieldParser(fileStream))
            {
                csvReader.SetDelimiters(new[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;
                string[] colFields = csvReader.ReadFields();
                if (colFields == null)
                    throw new Exception(ValidationResource.PeopleContact_CannotReadCsvHeader_ErrorMessage);

                foreach (var datecolumn in colFields.Select(column => new DataColumn(column) { AllowDBNull = true }))
                {
                    _csvData.Columns.Add(datecolumn);
                }

                while (!csvReader.EndOfData)
                {
                    string[] fieldData = csvReader.ReadFields();
                    if (fieldData == null)
                        throw new Exception(String.Format(ValidationResource.PeopleContact_CannorReadCsvRow_ErrorMessage, csvReader.LineNumber));

                    var fieldObjectData = new object[colFields.Length];
                    for (int i = 0; i < Math.Min(colFields.Length, fieldData.Length); i++)
                    {
                        if (fieldData[i] == String.Empty)
                            continue;

                        fieldObjectData[i] = fieldData[i];
                    }
                    // ReSharper disable CoVariantArrayConversion
                    _csvData.Rows.Add(fieldObjectData);
                    // ReSharper restore CoVariantArrayConversion
                }
            }
        }

        public void UpdatePeopleContacts()
        {
            ValidateHeaderDataForPeopleContacts();

            IEnumerable<string> emails = GetEmails();
            PeopleContact[] peopleContacts = _db.PeopleContacts.Where(pc => pc.RegistrarId == _userId && emails.Contains(pc.Email1.ToLower())).ToArray();

            int updatedContacts = 0;
            int addedContacts = 0;
            int notProcessedContacts = 0;
            var notProcessedMessage = new StringBuilder();
            PhoneNumberPrefix[] phoneNumberPrefixes = _db.PhoneNumberPrefixes.ToArray();
            DateTime createdSnapshot = DateTime.Now;
            for (int i = 0; i < _csvData.Rows.Count; i++)
            {
                var csvPeopleContact = new CsvPeopleContact(_csvData.Rows[i], phoneNumberPrefixes);
                if (!csvPeopleContact.IsValid)
                {
                    notProcessedContacts++;
                    notProcessedMessage.AppendLine(csvPeopleContact.ErrorMessage);
                    continue;
                }

                PeopleContact peopleContact = peopleContacts.FirstOrDefault(pc => pc.Email1.ToLower() == csvPeopleContact.Data.Email1);
                if (peopleContact == null)
                {
                    peopleContact = new PeopleContact
                                        {
                                            FirstName = csvPeopleContact.Data.FirstName,
                                            LastName = csvPeopleContact.Data.LastName,
                                            Email1 = csvPeopleContact.Data.Email1,
                                            PhoneNumberPrefix1Id = csvPeopleContact.Data.PhoneNumberPrefix1Id,
                                            PhoneNumber1 = csvPeopleContact.Data.PhoneNumber1,
                                            Created = createdSnapshot,
                                            RegistrarId = _userId
                                        };
                    _db.PeopleContacts.Add(peopleContact);
                    addedContacts++;
                }
                else if (peopleContact.FirstName != csvPeopleContact.Data.FirstName || peopleContact.LastName != csvPeopleContact.Data.LastName || peopleContact.PhoneNumberPrefix1Id != csvPeopleContact.Data.PhoneNumberPrefix1Id || peopleContact.PhoneNumber1 != csvPeopleContact.Data.PhoneNumber1)
                {
                    peopleContact.FirstName = csvPeopleContact.Data.FirstName;
                    peopleContact.LastName = csvPeopleContact.Data.LastName;
                    peopleContact.PhoneNumberPrefix1Id = csvPeopleContact.Data.PhoneNumberPrefix1Id;
                    peopleContact.PhoneNumber1 = csvPeopleContact.Data.PhoneNumber1;
                    _db.Entry(peopleContact).State = EntityState.Modified;
                    updatedContacts++;
                }
            }

            _db.SaveChanges();

            _summaryMesage.AppendLine(String.Format(ViewResource.PeopleContact_ImportSummary_Text,
                                                    _csvData.Rows.Count, addedContacts, updatedContacts,
                                                    notProcessedContacts));

            if (notProcessedContacts == 0)
                return;

            _summaryMesage.AppendLine(String.Format("{0}{1}", Environment.NewLine,
                                                    String.Format(
                                                        ViewResource.PeopleContact_NotProccessedSummary_Text,
                                                        notProcessedMessage)));
        }

        public byte[] GetPeopleContacts()
        {
            PeopleContact[] peopleContacts = _db.PeopleContacts.Include(p => p.PhoneNumberPrefix1).Where(pc => pc.RegistrarId == _userId).ToArray();

            var csvPeopleContacts = new StringBuilder();

            string[] header = CsvPeopleContact.GetColumns();
            csvPeopleContacts.AppendLine(String.Join(",", header));
            foreach (PeopleContact peopleContact in peopleContacts)
            {
                var csvPeopleContactData = new CsvPeopleContactData
                                               {
                                                   FirstName = peopleContact.FirstName,
                                                   LastName = peopleContact.LastName,
                                                   Email1 = peopleContact.Email1,
                                                   PhoneNumberPrefix1 = peopleContact.PhoneNumberPrefix1,
                                                   PhoneNumber1 = peopleContact.PhoneNumber1
                                               };
                string[] row = CsvPeopleContact.GetRow(csvPeopleContactData);
                if (row == null)
                    continue;

                csvPeopleContacts.AppendLine(String.Join(",", row));
            }

            byte[] csvBytes = new UTF8Encoding().GetBytes(csvPeopleContacts.ToString());
            return csvBytes;
        }

        public byte[] GetTopTen()
        {
            TopTen[] topTens = _db.TopTens.Include(tt => tt.ToUser).Where(tt => tt.FromUserId == _userId).ToArray();

            var csvTopTens = new StringBuilder();

            string[] header = CsvTopTen.GetColumns();
            csvTopTens.AppendLine(String.Join(",", header));
            foreach (TopTen topTen in topTens)
            {
                var csvTopTenData = new CsvTopTenData
                                        {
                                            FirstName = topTen.ToUser.FirstName,
                                            LastName = topTen.ToUser.LastName,
                                            Email1 = topTen.ToUser.Email1,
                                            PhoneNumberPrefix1 = topTen.ToUser.PhoneNumberPrefix1,
                                            PhoneNumber1 = topTen.ToUser.PhoneNumber1,
                                            Status = topTen.Status
                                        };
                string[] row = CsvTopTen.GetRow(csvTopTenData);
                if (row == null)
                    continue;

                csvTopTens.AppendLine(String.Join(",", row));
            }

            byte[] csvBytes = new UTF8Encoding().GetBytes(csvTopTens.ToString());
            return csvBytes;
        }

        private void ValidateHeaderDataForPeopleContacts()
        {
            string[] requiredColumns = CsvPeopleContact.GetColumns();
            foreach (string requiredColumn in requiredColumns.Where(rc => !_csvData.Columns.Contains(rc)))
            {
                throw new Exception(String.Format(ValidationResource.PeopleContact_MissingCsvHeader_ErrorMessage, requiredColumn));
            }
        }

        private IEnumerable<string> GetEmails()
        {
            var emails = new List<string>();
            for (int i = 0; i < _csvData.Rows.Count; i++)
            {
                object email = _csvData.Rows[i][CsvPeopleContact.EmailColumnName];
                if (email == null)
                    continue;

                emails.Add(email.ToString().ToLower());
            }
            return emails.Distinct().ToArray();
        }
    }
}