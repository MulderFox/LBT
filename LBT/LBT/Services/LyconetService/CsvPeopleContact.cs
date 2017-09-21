using LBT.Models;
using LBT.Resources;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace LBT.Services.LyconetService
{
    public class CsvPeopleContact
    {
        public const string EmailColumnName = "E-mail Address";

        private const string FirstNameColumnName = "First Name";
        private const string LastNameColumnName = "Last Name";
        private const string PhoneNumberColumnName = "Primary Phone";

        public CsvPeopleContactData Data;

        public bool IsValid;
        public string ErrorMessage;

        private readonly DataRow _dataRow;
        private readonly PhoneNumberPrefix[] _phoneNumberPrefixes;

        public CsvPeopleContact(DataRow dataRow, PhoneNumberPrefix[] phoneNumberPrefixes)
        {
            _dataRow = dataRow;
            _phoneNumberPrefixes = phoneNumberPrefixes;
            Data = new CsvPeopleContactData();

            IsValid = true;

            string additionalErrorMessage = ValidateRequiredFields();

            if (!IsValid)
            {
                ErrorMessage = String.Format(GetErrorTemplate(), additionalErrorMessage);
                return;
            }

            Data.FirstName = _dataRow[FirstNameColumnName].ToString();
            Data.LastName = _dataRow[LastNameColumnName].ToString();
            Data.Email1 = _dataRow[EmailColumnName].ToString().ToLower();

            additionalErrorMessage = ParseMobilePhone();

            if (!IsValid)
            {
                ErrorMessage = String.Format(GetErrorTemplate(), additionalErrorMessage);
            }
        }

        public static string[] GetColumns()
        {
            var requiredColumns = new[] { FirstNameColumnName, LastNameColumnName, EmailColumnName, PhoneNumberColumnName };
            return requiredColumns;
        }

        public static string[] GetRow(CsvPeopleContactData csvPeopleContactData)
        {
            if (String.IsNullOrEmpty(csvPeopleContactData.FirstName) || String.IsNullOrEmpty(csvPeopleContactData.LastName) || String.IsNullOrEmpty(csvPeopleContactData.Email1))
                return null;

            var row = new[] { csvPeopleContactData.FirstName, csvPeopleContactData.LastName, csvPeopleContactData.Email1.ToLower(), String.Empty };
            if (csvPeopleContactData.PhoneNumberPrefix1 != null && csvPeopleContactData.PhoneNumber1 != null)
            {
                row[3] = String.Format("{0}{1}", csvPeopleContactData.PhoneNumberPrefix1.ExportablePrefix, csvPeopleContactData.PhoneNumber1);
            }

            return row;
        }

        private string ValidateRequiredFields()
        {
            string additionalErrorMessage = null;
            if (String.IsNullOrEmpty(_dataRow[EmailColumnName].ToString()))
            {
                additionalErrorMessage = String.Format(ValidationResource.Global_RequiredValue_ErrorMessage, EmailColumnName);
            }
            else if (_dataRow[EmailColumnName].ToString().Length > 100)
            {
                additionalErrorMessage = String.Format(ValidationResource.Global_TooLongValue_ErrorMessage, EmailColumnName);
            }

            if (String.IsNullOrEmpty(_dataRow[LastNameColumnName].ToString()))
            {
                additionalErrorMessage = String.Format(ValidationResource.Global_RequiredValue_ErrorMessage, LastNameColumnName);
            }
            else if (_dataRow[LastNameColumnName].ToString().Length > 40)
            {
                additionalErrorMessage = String.Format(ValidationResource.Global_TooLongValue_ErrorMessage, LastNameColumnName);
            }

            if (String.IsNullOrEmpty(_dataRow[FirstNameColumnName].ToString()))
            {
                additionalErrorMessage = String.Format(ValidationResource.Global_RequiredValue_ErrorMessage, FirstNameColumnName);
            }
            else if (_dataRow[FirstNameColumnName].ToString().Length > 40)
            {
                additionalErrorMessage = String.Format(ValidationResource.Global_TooLongValue_ErrorMessage, FirstNameColumnName);
            }

            if (!String.IsNullOrEmpty(additionalErrorMessage))
                IsValid = false;

            return additionalErrorMessage;
        }

        private string GetErrorTemplate()
        {
            string errorTemplate = String.Format(ValidationResource.PeopleContact_BadImportedRow_ErrorMessage, _dataRow[FirstNameColumnName], _dataRow[LastNameColumnName], _dataRow[EmailColumnName], _dataRow[PhoneNumberColumnName]);
            return errorTemplate;
        }

        private string ParseMobilePhone()
        {
            if (_dataRow[PhoneNumberColumnName] == null || String.IsNullOrEmpty(_dataRow[PhoneNumberColumnName].ToString()))
                return String.Empty;

            string cleanedPhoneNumber = Regex.Replace(_dataRow[PhoneNumberColumnName].ToString(), @"[^\d]", String.Empty);
            if (String.IsNullOrEmpty(cleanedPhoneNumber))
            {
                IsValid = false;
                return ValidationResource.Global_PhoneNumberNotContainNumbers_ErrorMessage;
            }

            PhoneNumberPrefix phoneNumberPrefix = _phoneNumberPrefixes.FirstOrDefault(pnp => !String.IsNullOrEmpty(pnp.MatchRegex) && !String.IsNullOrEmpty(pnp.ReplaceRegex) && Regex.IsMatch(cleanedPhoneNumber, pnp.MatchRegex));
            if (phoneNumberPrefix == null && cleanedPhoneNumber.Length != 9)
            {
                IsValid = false;
                return ValidationResource.Global_BadPhoneNumberPrefix_ErrorMessage;
            }

            if (phoneNumberPrefix == null)
            {
                Data.PhoneNumberPrefix1Id = 1;
                Data.PhoneNumber1 = cleanedPhoneNumber;
            }
            else
            {
                Data.PhoneNumberPrefix1Id = phoneNumberPrefix.PhoneNumberPrefixId;
                Data.PhoneNumber1 = Regex.Replace(cleanedPhoneNumber, phoneNumberPrefix.ReplaceRegex, String.Empty);
            }

            if (Data.PhoneNumber1.Length > 40)
            {
                IsValid = false;
                return ValidationResource.Global_PhoneNumberIsTooLong_ErrorMessage;
            }

            return String.Empty;
        }
    }
}