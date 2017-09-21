using System.Text;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using LBT.ModelViews;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace LBT.Cache
{
    public class PeopleContactCache : BaseCache
    {
        public static IQueryable<PeopleContact> GetIndex(DefaultContext db, int registrarId)
        {
            IQueryable<PeopleContact> peopleContacts = db.PeopleContacts.Where(pc => pc.RegistrarId == registrarId).OrderBy(pc => pc.LastName);
            return peopleContacts;
        }

        public static IQueryable<PeopleContact> GetIndex(DefaultContext db, string searchString, string searchStringAccording, string sortOrder, int filteredUserId, bool hideDeathContacts)
        {
            IQueryable<PeopleContact> peopleContacts =
                db.PeopleContacts.Include(p => p.Registrar).Include(p => p.District).Include(p => p.PhoneNumberPrefix1).
                    Include(p => p.PhoneNumberPrefix2).Where(
                        p => p.RegistrarId == filteredUserId);

            if (hideDeathContacts)
                peopleContacts = peopleContacts.Where(p => !p.ContactDead);

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchStringAccording)
                {
                    case LastNameField:
                        peopleContacts = peopleContacts.Where(up => up.LastName.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case FirstNameField:
                        peopleContacts = peopleContacts.Where(up => up.FirstName.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case CityField:
                        peopleContacts = peopleContacts.Where(up => up.City.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case PhoneNumber1Field:
                        peopleContacts = peopleContacts.Where(up => up.PhoneNumber1.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case Email1Field:
                        peopleContacts = peopleContacts.Where(up => up.Email1.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case SkypeField:
                        peopleContacts = peopleContacts.Where(up => up.Skype.ToUpper().Contains(searchString.ToUpper()));
                        break;
                }
            }

            switch (sortOrder)
            {
                case LastNameDescSortOrder:
                    peopleContacts = peopleContacts.OrderByDescending(up => up.LastName);
                    break;

                case FirstNameAscSortOrder:
                    peopleContacts = peopleContacts.OrderBy(up => up.FirstName);
                    break;

                case FirstNameDescSortOrder:
                    peopleContacts = peopleContacts.OrderByDescending(up => up.FirstName);
                    break;

                case CityAscSortOrder:
                    peopleContacts = peopleContacts.OrderBy(up => up.City);
                    break;

                case CityDescSortOrder:
                    peopleContacts = peopleContacts.OrderByDescending(up => up.City);
                    break;

                case PhoneNumber1AscSortOrder:
                    peopleContacts = peopleContacts.OrderBy(up => up.PhoneNumber1);
                    break;

                case PhoneNumber1DescSortOrder:
                    peopleContacts = peopleContacts.OrderByDescending(up => up.PhoneNumber1);
                    break;

                case Email1AscSortOrder:
                    peopleContacts = peopleContacts.OrderBy(up => up.Email1);
                    break;

                case Email1DescSortOrder:
                    peopleContacts = peopleContacts.OrderByDescending(up => up.Email1);
                    break;

                case SkypeAscSortOrder:
                    peopleContacts = peopleContacts.OrderBy(up => up.Skype);
                    break;

                case SkypeDescSortOrder:
                    peopleContacts = peopleContacts.OrderByDescending(up => up.Skype);
                    break;

                case PotentialAscSortOrder:
                    peopleContacts = peopleContacts.OrderBy(up => up.Potential);
                    break;

                case PotentialDescSortOrder:
                    peopleContacts = peopleContacts.OrderByDescending(up => up.Potential);
                    break;

                default:
                    peopleContacts = peopleContacts.OrderBy(up => up.LastName);
                    break;
            }

            return peopleContacts;
        }

        public static PeopleContact GetDetail(DefaultContext db, int id)
        {
            PeopleContact peopleContact = db.PeopleContacts.Find(id);
            peopleContact.ConfirmTermsAndConditions = true;
            peopleContact.ConfirmPersonalData = true;
            return peopleContact;
        }

        public static PeopleContact GetDetailWithRegister(DefaultContext db, int id)
        {
            PeopleContact peopleContact = db.PeopleContacts.Include(pc => pc.Registrar).Single(pc => pc.PeopleContactId == id);
            peopleContact.ConfirmTermsAndConditions = true;
            peopleContact.ConfirmPersonalData = true;
            return peopleContact;
        }

        public static void Insert(DefaultContext db, int userId, ref PeopleContact peopleContact)
        {
            peopleContact.Created = DateTime.Now;
            peopleContact.RegistrarId = userId;
            db.PeopleContacts.Add(peopleContact);
            db.SaveChanges();

            peopleContact.CheckDuplicity(db);
        }

        public static void Insert(DefaultContext db, int userId, ref MultiplePeopleContacts multiplePeopleContacts)
        {
            DateTime created = DateTime.Now;
            foreach (MultiplePeopleContact multiplePeopleContact in multiplePeopleContacts.MultiplePeopleContactsList)
            {
                var peopleContact = new PeopleContact
                                        {
                                            LastName = multiplePeopleContact.LastName,
                                            FirstName = multiplePeopleContact.FirstName,
                                            City = multiplePeopleContact.City,
                                            PhoneNumber1 = multiplePeopleContact.PhoneNumber1,
                                            Email1 = multiplePeopleContact.Email1,
                                            Skype = multiplePeopleContact.Skype,
                                            Created = created,
                                            RegistrarId = userId
                                        };
                if (!String.IsNullOrEmpty(peopleContact.PhoneNumber1))
                    peopleContact.PhoneNumberPrefix1Id = multiplePeopleContact.PhoneNumberPrefix1Id;

                db.PeopleContacts.Add(peopleContact);
            }

            db.SaveChanges();
        }

        public static bool Update(DefaultContext db, ref PeopleContact peopleContact)
        {
            int peopleContactId = peopleContact.PeopleContactId;
            PeopleContact dbPeopleContact = GetDetail(db, peopleContactId);
            if (dbPeopleContact == null)
                return false;

            dbPeopleContact.CopyFrom(peopleContact);
            db.SaveChanges();
            peopleContact = dbPeopleContact;
            return true;
        }

        public static DeleteResult Delete(DefaultContext db, int id, out PeopleContact peopleContact)
        {
            peopleContact = GetDetail(db, id);
            if (peopleContact == null)
                return DeleteResult.AuthorizationFailed;

            try
            {
                var parameter = new SqlParameter(PeopleContactIdSqlParameter, peopleContact.PeopleContactId);
                db.Database.ExecuteSqlCommand(CascadeRemovePeopleContactProcedureTemplate, parameter);
                return DeleteResult.Ok;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                return DeleteResult.DbFailed;
            }
        }

        public enum ValidationStatus
        {
            Ok,
            PhoneNumberPrefix1IsEmpty,
            PhoneNumber1OwnUniqueError,
            SkypeOwnUniqueError,
            Email1OwnUniqueError
        }

        public static ValidationStatus ValidateIsPhoneNumber1OrSkypeOrEmail1UniqueByUser(DefaultContext db, int userId, int? peopleContactId, int? phoneNumberPrefix1Id, string phoneNumber1, string skype, string email1, out object data)
        {
            data = true;

            if (!phoneNumberPrefix1Id.HasValue && !String.IsNullOrEmpty(phoneNumber1))
            {
                data = ValidationResource.PeopleContact_PhoneNumberPrefix1IsEmpty_ErrorMessage;
                return ValidationStatus.PhoneNumberPrefix1IsEmpty;
            }

            int fixPeopleContactId = peopleContactId.GetValueOrDefault();
            PeopleContact[] peopleContacts;
            if (phoneNumberPrefix1Id.HasValue && !String.IsNullOrEmpty(phoneNumber1))
            {
                peopleContacts = db.PeopleContacts.Include(p => p.Registrar).Include(p => p.PhoneNumberPrefix1)
                    .Where(
                        p =>
                        p.PeopleContactId != fixPeopleContactId &&
                        p.PhoneNumberPrefix1.PhoneNumberPrefixId == phoneNumberPrefix1Id.Value &&
                        p.PhoneNumber1 == phoneNumber1 &&
                        p.Registrar.UserId == userId).ToArray();

                if (!peopleContacts.Any())
                {
                    return ValidationStatus.Ok;
                }

                data = String.Format(ValidationResource.Global_OwnUnique_ErrorMessage, FieldResource.Global_PhoneNumber1_Name);
                return ValidationStatus.PhoneNumber1OwnUniqueError;
            }

            if (!String.IsNullOrEmpty(skype))
            {
                peopleContacts = db.PeopleContacts.Include(p => p.Registrar).Include(p => p.PhoneNumberPrefix1)
                    .Where(
                        p =>
                        p.PeopleContactId != fixPeopleContactId &&
                        p.Skype == skype &&
                        p.Registrar.UserId == userId).ToArray();

                if (!peopleContacts.Any())
                {
                    return ValidationStatus.Ok;
                }

                data = String.Format(ValidationResource.Global_OwnUnique_ErrorMessage, FieldResource.Global_Skype_Name);
                return ValidationStatus.SkypeOwnUniqueError;
            }

            if (!String.IsNullOrEmpty(email1))
            {
                peopleContacts = db.PeopleContacts.Include(p => p.Registrar).Include(p => p.PhoneNumberPrefix1)
                    .Where(
                        p =>
                        p.PeopleContactId != fixPeopleContactId &&
                        p.Email1 == email1 &&
                        p.Registrar.UserId == userId).ToArray();

                if (!peopleContacts.Any())
                {
                    return ValidationStatus.Ok;
                }

                data = String.Format(ValidationResource.Global_OwnUnique_ErrorMessage, FieldResource.Global_Email1_Name);
                return ValidationStatus.Email1OwnUniqueError;
            }

            return ValidationStatus.Ok;
        }

        public static PeopleContactTask[] GetPeopleContactTasks(DefaultContext db, int userProfileId)
        {
            IQueryable<PeopleContact> peopleContacts = db.PeopleContacts.Include(pc => pc.Registrar).Where(pc => pc.RegistrarId == userProfileId && pc.WorkflowState != WorkflowState.Idle && pc.WorkflowState != WorkflowState.Finished && pc.WorkflowState != WorkflowState.Stopped && !pc.ContactDead);

            var peopleContactTasks = new List<PeopleContactTask>();
            foreach (PeopleContact peopleContact in peopleContacts)
            {
                peopleContactTasks.AddRange(peopleContact.GetPeopleContactTasks());
            }

            return peopleContactTasks.ToArray();
        }

        /// <summary>
        /// Gets the people contact tasks.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <returns>PeopleContactTask[][].</returns>
        public static PeopleContactTask[] GetPeopleContactTasks(DefaultContext db)
        {
            IQueryable<PeopleContact> peopleContacts = db.PeopleContacts.Include(pc => pc.Registrar).Where(pc => pc.Registrar.Active && (pc.Registrar.UseMail || (!String.IsNullOrEmpty(pc.Registrar.SmsEmail) && pc.Registrar.UseSms)) && pc.WorkflowState != WorkflowState.Idle && pc.WorkflowState != WorkflowState.Finished && pc.WorkflowState != WorkflowState.Stopped && !pc.ContactDead);

            var peopleContactTasks = new List<PeopleContactTask>();
            foreach (PeopleContact peopleContact in peopleContacts)
            {
                peopleContactTasks.AddRange(peopleContact.GetPeopleContactTasks());
            }

            return peopleContactTasks.ToArray();
        }

        public static void CheckTasks(DefaultContext db)
        {
            PeopleContactTask[] peopleContactTasks = GetPeopleContactTasks(db);
            var groupedPeopleContactTasks = peopleContactTasks.GroupBy(p => p.RegistrarId, (key, g) => new { RegistrarId = key, PeopleContactTasks = g.ToArray() }).ToArray();

            foreach (var groupedPeopleContactTask in groupedPeopleContactTasks)
            {
                if (groupedPeopleContactTask.PeopleContactTasks[0].UseMail)
                {
                    var actualTasks = new StringBuilder();
                    var delayedTasks = new StringBuilder();
                    foreach (PeopleContactTask peopleContactTask in groupedPeopleContactTask.PeopleContactTasks)
                    {
                        switch (peopleContactTask.PeopleContactTaskType)
                        {
                            case PeopleContactTaskType.Actual:
                                actualTasks.AppendLine(peopleContactTask.Text);
                                break;

                            case PeopleContactTaskType.Delayed:
                                delayedTasks.AppendLine(peopleContactTask.Text);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    if (actualTasks.Length == 0)
                    {
                        actualTasks.AppendLine(MailResource.TaskSchedulerController_ExecuteCheckTasks_NoTasks);
                    }

                    if (delayedTasks.Length == 0)
                    {
                        delayedTasks.AppendLine(MailResource.TaskSchedulerController_ExecuteCheckTasks_NoTasks);
                    }

                    string textBody = String.Format(MailResource.TaskSchedulerController_ExecuteCheckTasks_TextBody,
                                                    actualTasks, delayedTasks, Url.GetWebRootUrl());

                    Mail.SendEmail(groupedPeopleContactTask.PeopleContactTasks[0].Email1,
                                        MailResource.TaskSchedulerController_ExecuteCheckTasks_Subject, textBody,
                                        true, true);
                }

                if (groupedPeopleContactTask.PeopleContactTasks[0].UseSms && !String.IsNullOrEmpty(groupedPeopleContactTask.PeopleContactTasks[0].SmsEmail))
                {
                    string textBody =
                        String.Format(SmsResource.TaskSchedulerController_ExecuteCheckTasks_TextBody,
                                      groupedPeopleContactTask.PeopleContactTasks.Count());

                    Mail.SendEmail(groupedPeopleContactTask.PeopleContactTasks[0].SmsEmail,
                                        SmsResource.TaskSchedulerController_ExecuteCheckTasks_Subject, textBody,
                                        true, true);
                }
            }
        }
    }
}