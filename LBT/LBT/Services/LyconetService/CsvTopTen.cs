using System;

namespace LBT.Services.LyconetService
{
    public class CsvTopTen
    {
        private const string FirstNameColumnName = "First Name";
        private const string LastNameColumnName = "Last Name";
        private const string EmailColumnName = "E-mail Address";
        private const string PhoneNumberColumnName = "Primary Phone";
        private const string CategoriesColumnName = "Categories";
        private const string TeamTemplate = "{0} team";

        public static string[] GetColumns()
        {
            var requiredColumns = new[] { FirstNameColumnName, LastNameColumnName, EmailColumnName, PhoneNumberColumnName, CategoriesColumnName };
            return requiredColumns;
        }

        public static string[] GetRow(CsvTopTenData csvTopTenData)
        {
            if (String.IsNullOrEmpty(csvTopTenData.FirstName) || String.IsNullOrEmpty(csvTopTenData.LastName) || String.IsNullOrEmpty(csvTopTenData.Email1))
                return null;

            string categories = String.Format(TeamTemplate, csvTopTenData.Status);
            var row = new[] { csvTopTenData.FirstName, csvTopTenData.LastName, csvTopTenData.Email1.ToLower(), String.Empty, categories };
            if (csvTopTenData.PhoneNumberPrefix1 != null && csvTopTenData.PhoneNumber1 != null)
            {
                row[3] = String.Format("{0}{1}", csvTopTenData.PhoneNumberPrefix1.ExportablePrefix, csvTopTenData.PhoneNumber1);
            }

            return row;
        }
    }
}