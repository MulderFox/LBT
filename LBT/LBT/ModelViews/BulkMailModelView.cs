using LBT.Cache;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using LBT.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public sealed class BulkMailCreate : BaseModelView
    {
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_EmailSubject_Name", ResourceType = typeof(FieldResource))]
        public string Subject { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_EmailBody_Name", ResourceType = typeof(FieldResource))]
        public string Body { get; set; }

        public ModelStateDictionary Validate()
        {
            var modelStateDictionary = new ModelStateDictionary();

            CheckFields(ref modelStateDictionary);

            return modelStateDictionary;
        }

        public ModelStateDictionary SendEmails(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();
            UserProfile[] userProfiles = UserProfileCache.GetIndex(db);
            bool success = true;
            foreach (UserProfile userProfile in userProfiles.Where(up => !String.IsNullOrEmpty(up.Email1)))
            {
                bool partialSuccess = Mail.SendEmail(userProfile.Email1, Subject, Body, true, true);
                if (partialSuccess)
                {
                    string logMessage = String.Format("Email for user {0} with address {1} was successfully sent.", userProfile.FullName, userProfile.Email1);
                    Logger.SetLog(logMessage);
                }
                else
                {
                    string errorMessage = String.Format("Email for user {0} with address {1} was not sent.", userProfile.FullName, userProfile.Email1);
                    Logger.SetErrorLog(errorMessage);
                }

                success &= partialSuccess;
            }

            if (!success)
            {
                modelStateDictionary.AddModelError(BaseCache.EmptyField, ValidationResource.BulkMail_SomeEmailWasNotSent_ErrorMessage);
            }

            return modelStateDictionary;
        }

        private void CheckFields(ref ModelStateDictionary modelStateDictionary)
        {
            if (String.IsNullOrEmpty(Subject))
            {
                modelStateDictionary.AddModelError(BaseCache.SubjectField, String.Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.Global_EmailSubject_Name));
                return;
            }

            if (String.IsNullOrEmpty(Body))
            {
                modelStateDictionary.AddModelError(BaseCache.BodyField, String.Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.Global_EmailBody_Name));
            }
        }
    }
}