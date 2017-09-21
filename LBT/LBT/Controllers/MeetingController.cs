using LBT.Cache;
using LBT.Filters;
using LBT.Helpers;
using LBT.Models;
using LBT.ModelViews;
using PagedList;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static System.String;

namespace LBT.Controllers
{
    [InitializeSimpleMembership]
    [Authorize]
    public class MeetingController : BaseController
    {
        public ActionResult Dashboard()
        {
            var dashboardIndex = new DashboardIndex
                                     {
                                         MeetingBusinessInfoIndex = MeetingBusinessInfoIndex.GetNearestMeeting(Db, IsAdmin, UserId),
                                         MeetingWebinarIndex = MeetingWebinarIndex.GetNearestMeeting(Db, IsAdmin, UserId),
                                         MeetingMspEveningIndex = MeetingMspEveningIndex.GetNearestMeeting(Db, IsAdmin, UserId),
                                         MeetingSetkaniTymuIndex = MeetingSetkaniTymuIndex.GetNearestMeeting(Db, IsAdmin, UserId),
                                         MeetingSkoleniDavidaKotaskaIndex = MeetingSkoleniDavidaKotaskaIndex.GetNearestMeeting(Db, IsAdmin, UserId),
                                         MeetingOstatniIndex = MeetingOstatniIndex.GetNearestMeeting(Db, IsAdmin, UserId)
                                     };

            return View(dashboardIndex);
        }

        public ActionResult BusinessInfoIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingBusinessInfoIndex>)ProcessIndex(showAll, MeetingType.Lgs, false, sortOrder,
                                                                                currentFilter, currentFilterAccording,
                                                                                searchString, searchStringAccording,
                                                                                page,
                                                                                pageSize);
            return View(pagedRows);
        }

        public ActionResult WebinarIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingWebinarIndex>)ProcessIndex(showAll, MeetingType.Webinar, false, sortOrder,
                                                                                currentFilter, currentFilterAccording,
                                                                                searchString, searchStringAccording,
                                                                                page,
                                                                                pageSize);
            return View(pagedRows);
        }

        public ActionResult MspEveningIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingMspEveningIndex>)ProcessIndex(showAll, MeetingType.MspEvening, false, sortOrder,
                                                                              currentFilter, currentFilterAccording,
                                                                              searchString, searchStringAccording,
                                                                              page,
                                                                              pageSize);
            return View(pagedRows);
        }

        public ActionResult SetkaniTymuIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingSetkaniTymuIndex>)ProcessIndex(showAll, MeetingType.SetkaniTymu, false, sortOrder,
                                                                              currentFilter, currentFilterAccording,
                                                                              searchString, searchStringAccording,
                                                                              page,
                                                                              pageSize);
            return View(pagedRows);
        }

        public ActionResult SkoleniDavidaKotaskaIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingSkoleniDavidaKotaskaIndex>)ProcessIndex(showAll, MeetingType.SkoleniDavidaKotaska, false, sortOrder,
                                                                            currentFilter, currentFilterAccording,
                                                                            searchString, searchStringAccording,
                                                                            page,
                                                                            pageSize);
            return View(pagedRows);
        }

        public ActionResult OstatniIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingOstatniIndex>)ProcessIndex(showAll, MeetingType.Ostatni, false, sortOrder,
                                                                           currentFilter, currentFilterAccording,
                                                                           searchString, searchStringAccording,
                                                                           page,
                                                                           pageSize);
            return View(pagedRows);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult BusinessInfoCreate()
        {
            PopulateMainLeaderId(UserId);
            PopulateSecondaryLeaderId();

            return View(new MeetingBusinessInfoEdit());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult BusinessInfoCreate(MeetingBusinessInfoEdit meetingBusinessInfoEdit)
        {
            ModelState.Merge(meetingBusinessInfoEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                MeetingCache.Insert(Db, MeetingType.Lgs, UserId, meetingBusinessInfoEdit);
                return RedirectToAction("BusinessInfoIndex");
            }

            PopulateMainLeaderId(meetingBusinessInfoEdit.OrganizerId.GetValueOrDefault(UserId));
            PopulateSecondaryLeaderId(meetingBusinessInfoEdit.SecondaryLeaderId);

            return View(meetingBusinessInfoEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult WebinarCreate()
        {
            PopulateMainLeaderId(UserId);
            PopulateSecondaryLeaderId();

            return View(new MeetingWebinarEdit());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult WebinarCreate(MeetingWebinarEdit meetingWebinarEdit)
        {
            ModelState.Merge(meetingWebinarEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                MeetingCache.Insert(Db, MeetingType.Webinar, UserId, meetingWebinarEdit);
                return RedirectToAction("WebinarIndex");
            }

            PopulateMainLeaderId(meetingWebinarEdit.OrganizerId.GetValueOrDefault(UserId));
            PopulateSecondaryLeaderId(meetingWebinarEdit.SecondaryLeaderId);

            return View(meetingWebinarEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult MspEveningCreate()
        {
            PopulateMainLeaderId(UserId);
            PopulateSecondaryLeaderId();

            return View(new MeetingMspEveningEdit());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult MspEveningCreate(MeetingMspEveningEdit meetingMspEveningEdit)
        {
            ModelState.Merge(meetingMspEveningEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                MeetingCache.Insert(Db, MeetingType.MspEvening, UserId, meetingMspEveningEdit);
                return RedirectToAction("MspEveningIndex");
            }

            PopulateMainLeaderId(meetingMspEveningEdit.OrganizerId.GetValueOrDefault(UserId));
            PopulateSecondaryLeaderId(meetingMspEveningEdit.SecondaryLeaderId);

            return View(meetingMspEveningEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult SetkaniTymuCreate()
        {
            PopulateMeetingTitleTypeId(MeetingType.SetkaniTymu);
            PopulateMainLeaderId(UserId);
            PopulateSecondaryLeaderId();
            PopulateBankAccountId(BankAccountType.TeamMeeting, UserId);
            PopulateSecondBankAccountId(BankAccountType.TeamMeeting, UserId);

            return View(new MeetingSetkaniTymuEdit());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult SetkaniTymuCreate(MeetingSetkaniTymuEdit meetingSetkaniTymuEdit)
        {
            ModelState.Merge(meetingSetkaniTymuEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                MeetingCache.Insert(Db, MeetingType.SetkaniTymu, UserId, meetingSetkaniTymuEdit);
                return RedirectToAction("SetkaniTymuIndex");
            }

            PopulateMeetingTitleTypeId(MeetingType.SetkaniTymu, meetingSetkaniTymuEdit.MeetingTitleTypeId);
            PopulateMainLeaderId(meetingSetkaniTymuEdit.OrganizerId.GetValueOrDefault(UserId));
            PopulateSecondaryLeaderId(meetingSetkaniTymuEdit.SecondaryLeaderId);
            PopulateBankAccountId(BankAccountType.TeamMeeting, UserId, meetingSetkaniTymuEdit.BankAccountId);
            PopulateSecondBankAccountId(BankAccountType.TeamMeeting, UserId, meetingSetkaniTymuEdit.SecondBankAccountId);

            return View(meetingSetkaniTymuEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult SkoleniDavidaKotaskaCreate()
        {
            PopulateMainLeaderId(UserId);
            PopulateSecondaryLeaderId();
            PopulateBankAccountId(BankAccountType.DavidKotasekTraining, UserId);
            PopulateSecondBankAccountId(BankAccountType.DavidKotasekTraining, UserId);

            return View(new MeetingSkoleniDavidaKotaskaEdit());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult SkoleniDavidaKotaskaCreate(MeetingSkoleniDavidaKotaskaEdit meetingSkoleniDavidaKotaskaEdit)
        {
            ModelState.Merge(meetingSkoleniDavidaKotaskaEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                MeetingCache.Insert(Db, MeetingType.SkoleniDavidaKotaska, UserId, meetingSkoleniDavidaKotaskaEdit);
                return RedirectToAction("SkoleniDavidaKotaskaIndex");
            }

            PopulateMainLeaderId(meetingSkoleniDavidaKotaskaEdit.OrganizerId.GetValueOrDefault(UserId));
            PopulateSecondaryLeaderId(meetingSkoleniDavidaKotaskaEdit.SecondaryLeaderId);
            PopulateBankAccountId(BankAccountType.DavidKotasekTraining, UserId, meetingSkoleniDavidaKotaskaEdit.BankAccountId);
            PopulateSecondBankAccountId(BankAccountType.DavidKotasekTraining, UserId, meetingSkoleniDavidaKotaskaEdit.SecondBankAccountId);

            return View(meetingSkoleniDavidaKotaskaEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult OstatniCreate()
        {
            PopulateMeetingTitleTypeId(MeetingType.Ostatni);
            PopulateMainLeaderId(UserId);
            PopulateSecondaryLeaderId();
            PopulateBankAccountId(BankAccountType.Others, UserId);
            PopulateSecondBankAccountId(BankAccountType.Others, UserId);

            return View(new MeetingOstatniEdit());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult OstatniCreate(MeetingOstatniEdit meetingOstatniEdit)
        {
            ModelState.Merge(meetingOstatniEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                MeetingCache.Insert(Db, MeetingType.Ostatni, UserId, meetingOstatniEdit);
                return RedirectToAction("OstatniIndex");
            }

            PopulateMeetingTitleTypeId(MeetingType.Ostatni, meetingOstatniEdit.MeetingTitleTypeId);
            PopulateMainLeaderId(meetingOstatniEdit.OrganizerId.GetValueOrDefault(UserId));
            PopulateSecondaryLeaderId(meetingOstatniEdit.SecondaryLeaderId);
            PopulateBankAccountId(BankAccountType.Others, UserId, meetingOstatniEdit.BankAccountId);
            PopulateSecondBankAccountId(BankAccountType.Others, UserId, meetingOstatniEdit.SecondBankAccountId);

            return View(meetingOstatniEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult BusinessInfoEdit(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingBusinessInfoEdit meetingBusinessInfoEdit = MeetingBusinessInfoEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingBusinessInfoEdit))
            {
                return RedirectToAccessDenied();
            }

            PopulateMainLeaderId(meetingBusinessInfoEdit.OrganizerId.GetValueOrDefault(UserId), meetingBusinessInfoEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingBusinessInfoEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingBusinessInfoEdit.MainLeaderId, meetingBusinessInfoEdit.SecondaryLeaderId, meetingBusinessInfoEdit.SecondaryOrganizerId);

            return View(meetingBusinessInfoEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult BusinessInfoEdit(MeetingBusinessInfoEdit meetingBusinessInfoEdit)
        {
            ModelState.Merge(meetingBusinessInfoEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                Meeting meeting = meetingBusinessInfoEdit.GetModel(UserId);
                bool success = MeetingCache.Update(Db, ref meeting);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("BusinessInfoIndex");
            }

            PopulateMainLeaderId(UserId, meetingBusinessInfoEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingBusinessInfoEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingBusinessInfoEdit.MainLeaderId, meetingBusinessInfoEdit.SecondaryLeaderId, meetingBusinessInfoEdit.SecondaryOrganizerId);

            return View(meetingBusinessInfoEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult WebinarEdit(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingWebinarEdit meetingWebinarEdit = MeetingWebinarEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingWebinarEdit))
            {
                return RedirectToAccessDenied();
            }

            PopulateMainLeaderId(meetingWebinarEdit.OrganizerId.GetValueOrDefault(UserId), meetingWebinarEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingWebinarEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingWebinarEdit.MainLeaderId, meetingWebinarEdit.SecondaryLeaderId, meetingWebinarEdit.SecondaryOrganizerId);

            return View(meetingWebinarEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult WebinarEdit(MeetingWebinarEdit meetingWebinarEdit)
        {
            ModelState.Merge(meetingWebinarEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                Meeting meeting = meetingWebinarEdit.GetModel(UserId);
                bool success = MeetingCache.Update(Db, ref meeting);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("WebinarIndex");
            }

            PopulateMainLeaderId(UserId, meetingWebinarEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingWebinarEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingWebinarEdit.MainLeaderId, meetingWebinarEdit.SecondaryLeaderId, meetingWebinarEdit.SecondaryOrganizerId);

            return View(meetingWebinarEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult MspEveningEdit(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingMspEveningEdit meetingMspEveningEdit = MeetingMspEveningEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingMspEveningEdit))
            {
                return RedirectToAccessDenied();
            }

            PopulateMainLeaderId(meetingMspEveningEdit.OrganizerId.GetValueOrDefault(UserId), meetingMspEveningEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingMspEveningEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingMspEveningEdit.MainLeaderId, meetingMspEveningEdit.SecondaryLeaderId, meetingMspEveningEdit.SecondaryOrganizerId);

            return View(meetingMspEveningEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult MspEveningEdit(MeetingMspEveningEdit meetingMspEveningEdit)
        {
            ModelState.Merge(meetingMspEveningEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                Meeting meeting = meetingMspEveningEdit.GetModel(UserId);
                bool success = MeetingCache.Update(Db, ref meeting);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("MspEveningIndex");
            }

            PopulateMainLeaderId(UserId, meetingMspEveningEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingMspEveningEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingMspEveningEdit.MainLeaderId, meetingMspEveningEdit.SecondaryLeaderId, meetingMspEveningEdit.SecondaryOrganizerId);

            return View(meetingMspEveningEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult SetkaniTymuEdit(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingSetkaniTymuEdit meetingSetkaniTymuEdit = MeetingSetkaniTymuEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingSetkaniTymuEdit))
            {
                return RedirectToAccessDenied();
            }

            PopulateMeetingTitleTypeId(MeetingType.SetkaniTymu, meetingSetkaniTymuEdit.MeetingTitleTypeId);
            PopulateMainLeaderId(meetingSetkaniTymuEdit.OrganizerId.GetValueOrDefault(UserId), meetingSetkaniTymuEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingSetkaniTymuEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingSetkaniTymuEdit.MainLeaderId, meetingSetkaniTymuEdit.SecondaryLeaderId, meetingSetkaniTymuEdit.SecondaryOrganizerId);
            PopulateBankAccountId(BankAccountType.TeamMeeting, UserId, meetingSetkaniTymuEdit.BankAccountId);
            PopulateSecondBankAccountId(BankAccountType.TeamMeeting, UserId, meetingSetkaniTymuEdit.SecondBankAccountId);

            return View(meetingSetkaniTymuEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult SetkaniTymuEdit(MeetingSetkaniTymuEdit meetingSetkaniTymuEdit)
        {
            ModelState.Merge(meetingSetkaniTymuEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                Meeting meeting = meetingSetkaniTymuEdit.GetModel(UserId);
                bool success = MeetingCache.Update(Db, ref meeting);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("SetkaniTymuIndex");
            }

            PopulateMeetingTitleTypeId(MeetingType.SetkaniTymu, meetingSetkaniTymuEdit.MeetingTitleTypeId);
            PopulateMainLeaderId(UserId, meetingSetkaniTymuEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingSetkaniTymuEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingSetkaniTymuEdit.MainLeaderId, meetingSetkaniTymuEdit.SecondaryLeaderId, meetingSetkaniTymuEdit.SecondaryOrganizerId);
            PopulateBankAccountId(BankAccountType.TeamMeeting, UserId, meetingSetkaniTymuEdit.BankAccountId);
            PopulateSecondBankAccountId(BankAccountType.TeamMeeting, UserId, meetingSetkaniTymuEdit.SecondBankAccountId);

            return View(meetingSetkaniTymuEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult SkoleniDavidaKotaskaEdit(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingSkoleniDavidaKotaskaEdit meetingSkoleniDavidaKotaskaEdit = MeetingSkoleniDavidaKotaskaEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingSkoleniDavidaKotaskaEdit))
            {
                return RedirectToAccessDenied();
            }

            PopulateMainLeaderId(meetingSkoleniDavidaKotaskaEdit.OrganizerId.GetValueOrDefault(UserId), meetingSkoleniDavidaKotaskaEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingSkoleniDavidaKotaskaEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingSkoleniDavidaKotaskaEdit.MainLeaderId, meetingSkoleniDavidaKotaskaEdit.SecondaryLeaderId, meetingSkoleniDavidaKotaskaEdit.SecondaryOrganizerId);
            PopulateBankAccountId(BankAccountType.DavidKotasekTraining, UserId, meetingSkoleniDavidaKotaskaEdit.BankAccountId);
            PopulateSecondBankAccountId(BankAccountType.DavidKotasekTraining, UserId, meetingSkoleniDavidaKotaskaEdit.SecondBankAccountId);

            return View(meetingSkoleniDavidaKotaskaEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult SkoleniDavidaKotaskaEdit(MeetingSkoleniDavidaKotaskaEdit meetingSkoleniDavidaKotaskaEdit)
        {
            ModelState.Merge(meetingSkoleniDavidaKotaskaEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                Meeting meeting = meetingSkoleniDavidaKotaskaEdit.GetModel(UserId);
                bool success = MeetingCache.Update(Db, ref meeting);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("SkoleniDavidaKotaskaIndex");
            }

            PopulateMainLeaderId(UserId, meetingSkoleniDavidaKotaskaEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingSkoleniDavidaKotaskaEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingSkoleniDavidaKotaskaEdit.MainLeaderId, meetingSkoleniDavidaKotaskaEdit.SecondaryLeaderId, meetingSkoleniDavidaKotaskaEdit.SecondaryOrganizerId);
            PopulateBankAccountId(BankAccountType.DavidKotasekTraining, UserId, meetingSkoleniDavidaKotaskaEdit.BankAccountId);
            PopulateSecondBankAccountId(BankAccountType.DavidKotasekTraining, UserId, meetingSkoleniDavidaKotaskaEdit.SecondBankAccountId);

            return View(meetingSkoleniDavidaKotaskaEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult OstatniEdit(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingOstatniEdit meetingOstatniEdit = MeetingOstatniEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingOstatniEdit))
            {
                return RedirectToAccessDenied();
            }

            PopulateMeetingTitleTypeId(MeetingType.Ostatni, meetingOstatniEdit.MeetingTitleTypeId);
            PopulateMainLeaderId(meetingOstatniEdit.OrganizerId.GetValueOrDefault(UserId), meetingOstatniEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingOstatniEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingOstatniEdit.MainLeaderId, meetingOstatniEdit.SecondaryLeaderId, meetingOstatniEdit.SecondaryOrganizerId);
            PopulateBankAccountId(BankAccountType.Others, UserId, meetingOstatniEdit.BankAccountId);
            PopulateSecondBankAccountId(BankAccountType.Others, UserId, meeting.SecondBankAccountId);

            return View(meetingOstatniEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult OstatniEdit(MeetingOstatniEdit meetingOstatniEdit)
        {
            ModelState.Merge(meetingOstatniEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                Meeting meeting = meetingOstatniEdit.GetModel(Db, UserId);
                bool success = MeetingCache.Update(Db, ref meeting);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("OstatniIndex");
            }

            PopulateMeetingTitleTypeId(MeetingType.Ostatni, meetingOstatniEdit.MeetingTitleTypeId);
            PopulateMainLeaderId(UserId, meetingOstatniEdit.MainLeaderId);
            PopulateSecondaryLeaderId(meetingOstatniEdit.SecondaryLeaderId);
            PopulateSecondaryOrganizerId(meetingOstatniEdit.MainLeaderId, meetingOstatniEdit.SecondaryLeaderId, meetingOstatniEdit.SecondaryOrganizerId);
            PopulateBankAccountId(BankAccountType.Others, UserId, meetingOstatniEdit.BankAccountId);
            PopulateSecondBankAccountId(BankAccountType.Others, UserId, meetingOstatniEdit.SecondBankAccountId);

            return View(meetingOstatniEdit);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult BusinessInfoDelete(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingBusinessInfoEdit meetingBusinessInfoEdit = MeetingBusinessInfoEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingBusinessInfoEdit))
            {
                return RedirectToAccessDenied();
            }

            return View(meetingBusinessInfoEdit);
        }

        [HttpPost, ActionName("BusinessInfoDelete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult BusinessInfoDeleteConfirmed(int id)
        {
            Meeting meeting;
            DeleteResult deleteResult = MeetingCache.Delete(Db, id, out meeting);
            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("BusinessInfoIndex");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.CityField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                    MeetingBusinessInfoEdit meetingBusinessInfoEdit = MeetingBusinessInfoEdit.GetModelView(meeting);
                    return View(meetingBusinessInfoEdit);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult WebinarDelete(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingWebinarEdit meetingWebinarEdit = MeetingWebinarEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingWebinarEdit))
            {
                return RedirectToAccessDenied();
            }

            return View(meetingWebinarEdit);
        }

        [HttpPost, ActionName("WebinarDelete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult WebinarDeleteConfirmed(int id)
        {
            Meeting meeting;
            DeleteResult deleteResult = MeetingCache.Delete(Db, id, out meeting);
            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("WebinarIndex");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.WebinarUrlField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                    MeetingWebinarEdit meetingWebinarEdit = MeetingWebinarEdit.GetModelView(meeting);
                    return View(meetingWebinarEdit);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult MspEveningDelete(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingMspEveningEdit meetingMspEveningEdit = MeetingMspEveningEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingMspEveningEdit))
            {
                return RedirectToAccessDenied();
            }

            return View(meetingMspEveningEdit);
        }

        [HttpPost, ActionName("MspEveningDelete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult MspEveningDeleteConfirmed(int id)
        {
            Meeting meeting;
            DeleteResult deleteResult = MeetingCache.Delete(Db, id, out meeting);
            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("MspEveningIndex");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.CityField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                    MeetingMspEveningEdit meetingMspEveningEdit = MeetingMspEveningEdit.GetModelView(meeting);
                    return View(meetingMspEveningEdit);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult SetkaniTymuDelete(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingSetkaniTymuEdit meetingSetkaniTymuEdit = MeetingSetkaniTymuEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingSetkaniTymuEdit))
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSetkaniTymuEdit);
        }

        [HttpPost, ActionName("SetkaniTymuDelete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult SetkaniTymuDeleteConfirmed(int id)
        {
            Meeting meeting;
            DeleteResult deleteResult = MeetingCache.Delete(Db, id, out meeting);
            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("SetkaniTymuIndex");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                    MeetingSetkaniTymuEdit meetingSetkaniTymuEdit = MeetingSetkaniTymuEdit.GetModelView(meeting);
                    return View(meetingSetkaniTymuEdit);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult SkoleniDavidaKotaskaDelete(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingSkoleniDavidaKotaskaEdit meetingSkoleniDavidaKotaskaEdit = MeetingSkoleniDavidaKotaskaEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingSkoleniDavidaKotaskaEdit))
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSkoleniDavidaKotaskaEdit);
        }

        [HttpPost, ActionName("SkoleniDavidaKotaskaDelete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult SkoleniDavidaKotaskaDeleteConfirmed(int id)
        {
            Meeting meeting;
            DeleteResult deleteResult = MeetingCache.Delete(Db, id, out meeting);
            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("SkoleniDavidaKotaskaIndex");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.TitleField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                    MeetingSkoleniDavidaKotaskaEdit meetingSkoleniDavidaKotaskaEdit = MeetingSkoleniDavidaKotaskaEdit.GetModelView(meeting);
                    return View(meetingSkoleniDavidaKotaskaEdit);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult OstatniDelete(int id = 0)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            MeetingOstatniEdit meetingOstatniEdit = MeetingOstatniEdit.GetModelView(meeting);

            if (!IsAccess(meeting, meetingOstatniEdit))
            {
                return RedirectToAccessDenied();
            }

            return View(meetingOstatniEdit);
        }

        [HttpPost, ActionName("OstatniDelete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = LeadingRoles)]
        public ActionResult OstatniDeleteConfirmed(int id)
        {
            Meeting meeting;
            DeleteResult deleteResult = MeetingCache.Delete(Db, id, out meeting);
            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("OstatniIndex");

                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError(BaseCache.CityField, ValidationResource.Global_DeleteRecord_ErrorMessage);
                    MeetingOstatniEdit meetingOstatniEdit = MeetingOstatniEdit.GetModelView(meeting);
                    return View(meetingOstatniEdit);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ActionResult BusinessInfoDetails(string sortOrder, int id = 0)
        {
            var meetingBusinessInfoDetail = (MeetingBusinessInfoDetail)ProcessDetails(MeetingType.Lgs, id, sortOrder);
            if (!meetingBusinessInfoDetail.IsValid || meetingBusinessInfoDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingBusinessInfoDetail);
        }

        public ActionResult WebinarDetails(string sortOrder, int id = 0)
        {
            var meetingWebinarDetail = (MeetingWebinarDetail)ProcessDetails(MeetingType.Webinar, id, sortOrder);
            if (!meetingWebinarDetail.IsValid || meetingWebinarDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingWebinarDetail);
        }

        public ActionResult MspEveningDetails(string sortOrder, int id)
        {
            var meetingMspEveningDetail = (MeetingMspEveningDetail)ProcessDetails(MeetingType.MspEvening, id, sortOrder);
            if (!meetingMspEveningDetail.IsValid || meetingMspEveningDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingMspEveningDetail);
        }

        public ActionResult SetkaniTymuDetails(string sortOrder, int id)
        {
            var meetingSetkaniTymuDetail = (MeetingSetkaniTymuDetail)ProcessDetails(MeetingType.SetkaniTymu, id, sortOrder);
            if (!meetingSetkaniTymuDetail.IsValid || meetingSetkaniTymuDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSetkaniTymuDetail);
        }

        public ActionResult SkoleniDavidaKotaskaDetails(string sortOrder, int id)
        {
            var meetingSkoleniDavidaKotaskaDetail = (MeetingSkoleniDavidaKotaskaDetail)ProcessDetails(MeetingType.SkoleniDavidaKotaska, id, sortOrder);
            if (!meetingSkoleniDavidaKotaskaDetail.IsValid && meetingSkoleniDavidaKotaskaDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSkoleniDavidaKotaskaDetail);
        }

        public ActionResult OstatniDetails(string sortOrder, int id)
        {
            var meetingOstatniDetail = (MeetingOstatniDetail)ProcessDetails(MeetingType.Ostatni, id, sortOrder);
            if (!meetingOstatniDetail.IsValid || meetingOstatniDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingOstatniDetail);
        }

        public ActionResult BusinessInfoPrintIndex(bool showAll, string sortOrder, string currentFilter, string currentFilterAccording)
        {
            MeetingBusinessInfoIndex[] index = MeetingBusinessInfoIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, false, currentFilter, currentFilterAccording, sortOrder).ToArray();
            if (index.Length > 1000)
            {
                index = index.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.CurrentFilterAccording = currentFilterAccording;

            return View("BusinessInfoPrintIndex", "_PrintLayout", index);
        }

        public ActionResult WebinarPrintIndex(bool showAll, string sortOrder, string currentFilter, string currentFilterAccording)
        {
            MeetingWebinarIndex[] index = MeetingWebinarIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, false, currentFilter, currentFilterAccording, sortOrder).ToArray();
            if (index.Length > 1000)
            {
                index = index.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.CurrentFilterAccording = currentFilterAccording;

            return View("WebinarPrintIndex", "_PrintLayout", index);
        }

        public ActionResult MspEveningPrintIndex(bool showAll, string sortOrder, string currentFilter, string currentFilterAccording)
        {
            MeetingMspEveningIndex[] index = MeetingMspEveningIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, false, currentFilter, currentFilterAccording, sortOrder).ToArray();
            if (index.Length > 1000)
            {
                index = index.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.CurrentFilterAccording = currentFilterAccording;

            return View("MspEveningPrintIndex", "_PrintLayout", index);
        }

        public ActionResult SetkaniTymuPrintIndex(bool showAll, string sortOrder, string currentFilter, string currentFilterAccording)
        {
            MeetingSetkaniTymuIndex[] index = MeetingSetkaniTymuIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, false, currentFilter, currentFilterAccording, sortOrder).ToArray();
            if (index.Length > 1000)
            {
                index = index.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.CurrentFilterAccording = currentFilterAccording;

            return View("SetkaniTymuPrintIndex", "_PrintLayout", index);
        }

        public ActionResult SkoleniDavidaKotaskaPrintIndex(bool showAll, string sortOrder, string currentFilter, string currentFilterAccording)
        {
            MeetingSkoleniDavidaKotaskaIndex[] index = MeetingSkoleniDavidaKotaskaIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, false, currentFilter, currentFilterAccording, sortOrder).ToArray();
            if (index.Length > 1000)
            {
                index = index.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.CurrentFilterAccording = currentFilterAccording;

            return View("SkoleniDavidaKotaskaPrintIndex", "_PrintLayout", index);
        }

        public ActionResult OstatniPrintIndex(bool showAll, string sortOrder, string currentFilter, string currentFilterAccording)
        {
            MeetingOstatniIndex[] index = MeetingOstatniIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, false, currentFilter, currentFilterAccording, sortOrder).ToArray();
            if (index.Length > 1000)
            {
                index = index.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentFilter = currentFilter;
            ViewBag.CurrentFilterAccording = currentFilterAccording;

            return View("OstatniPrintIndex", "_PrintLayout", index);
        }

        public ActionResult BusinessInfoSignUnsign(
            string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize = PageSize, int filteredUserId = 0, int id = 0)
        {
            var meetingSignUnsign =
                (MeetingBusinessInfoSignUnsign)
                ProcessSignUnsign(MeetingType.Lgs, sortOrder, currentFilter, currentFilterAccording,
                                  searchString, searchStringAccording, page, signedSortOrder, signedPage, pageSize,
                                  filteredUserId, id);

            if (meetingSignUnsign == null)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSignUnsign);
        }

        public ActionResult BusinessInfoUserSignUnsign(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize = PageSize, int id = 0)
        {
            var meetingUserSignUsign =
                (MeetingBusinessInfoUserSignUnsign)
                ProcessUserSignUnsign(MeetingType.Lgs, sortOrder, currentFilter, currentFilterAccording,
                                      searchString, searchStringAccording, page, signedSortOrder, signedPage, pageSize,
                                      id);

            if (meetingUserSignUsign == null)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingUserSignUsign);
        }

        public ActionResult WebinarSignUnsign(
            string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize = PageSize, int filteredUserId = 0, int id = 0)
        {
            var meetingSignUnsign =
                (MeetingWebinarSignUnsign)
                ProcessSignUnsign(MeetingType.Webinar, sortOrder, currentFilter, currentFilterAccording,
                                  searchString, searchStringAccording, page, signedSortOrder, signedPage, pageSize,
                                  filteredUserId, id);

            if (meetingSignUnsign == null)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSignUnsign);
        }

        public ActionResult MspEveningSignUnsign(
            string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize = PageSize, int filteredUserId = 0, int id = 0)
        {
            var meetingSignUnsign =
                (MeetingMspEveningSignUnsign)
                ProcessSignUnsign(MeetingType.MspEvening, sortOrder, currentFilter, currentFilterAccording,
                                  searchString, searchStringAccording, page, signedSortOrder, signedPage, pageSize,
                                  filteredUserId, id);

            if (meetingSignUnsign == null)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSignUnsign);
        }

        public ActionResult SetkaniTymuSignUnsign(
            string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize = PageSize, int filteredUserId = 0, int id = 0)
        {
            var meetingSignUnsign =
                (MeetingSetkaniTymuSignUnsign)
                ProcessSignUnsign(MeetingType.SetkaniTymu, sortOrder, currentFilter, currentFilterAccording,
                                  searchString, searchStringAccording, page, signedSortOrder, signedPage, pageSize,
                                  filteredUserId, id);

            if (meetingSignUnsign == null)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSignUnsign);
        }

        public ActionResult SetkaniTymuUserSignUnsign(
            string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize = PageSize, int id = 0)
        {
            var meetingUserSignUnsign =
                (MeetingSetkaniTymuUserSignUnsign)
                ProcessUserSignUnsign(MeetingType.SetkaniTymu, sortOrder, currentFilter, currentFilterAccording,
                                      searchString, searchStringAccording, page, signedSortOrder, signedPage, pageSize,
                                      id);

            if (meetingUserSignUnsign == null)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingUserSignUnsign);
        }

        public ActionResult SkoleniDavidaKotaskaSignUnsign(
            string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize = PageSize, int filteredUserId = 0, int id = 0)
        {
            var meetingSignUnsign =
                (MeetingSkoleniDavidaKotaskaSignUnsign)
                ProcessSignUnsign(MeetingType.SkoleniDavidaKotaska, sortOrder, currentFilter, currentFilterAccording,
                                  searchString, searchStringAccording, page, signedSortOrder, signedPage, pageSize,
                                  filteredUserId, id);

            if (meetingSignUnsign == null)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSignUnsign);
        }

        public ActionResult SkoleniDavidaKotaskaUserSignUnsign(
            string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize = PageSize, int id = 0)
        {
            var meetingUserSignUnsign =
                (MeetingSkoleniDavidaKotaskaUserSignUnsign)
                ProcessUserSignUnsign(MeetingType.SkoleniDavidaKotaska, sortOrder, currentFilter, currentFilterAccording,
                                      searchString, searchStringAccording, page, signedSortOrder, signedPage, pageSize,
                                      id);

            if (meetingUserSignUnsign == null)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingUserSignUnsign);
        }

        public ActionResult OstatniSignUnsign(
            string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize = PageSize, int filteredUserId = 0, int id = 0)
        {
            var meetingSignUnsign =
                (MeetingOstatniSignUnsign)
                ProcessSignUnsign(MeetingType.Ostatni, sortOrder, currentFilter, currentFilterAccording,
                                  searchString, searchStringAccording, page, signedSortOrder, signedPage, pageSize,
                                  filteredUserId, id);

            if (meetingSignUnsign == null)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSignUnsign);
        }

        public ActionResult OstatniUserSignUnsign(
            string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize = PageSize, int id = 0)
        {
            var meetingUserSignUsign =
                (MeetingOstatniUserSignUnsign)
                ProcessUserSignUnsign(MeetingType.Ostatni, sortOrder, currentFilter, currentFilterAccording,
                                      searchString, searchStringAccording, page, signedSortOrder, signedPage, pageSize,
                                      id);

            if (meetingUserSignUsign == null)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingUserSignUsign);
        }

        public ActionResult BusinessInfoSign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("BusinessInfoSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult BusinessInfoUserSign(int id, int attendeeId, string currentFilter, string currentFilterAccording)
        {
            if (!CheckVisibleUserId(ref attendeeId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveUserAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("BusinessInfoUserSignUnsign", new { id, currentFilter, currentFilterAccording });
        }

        public ActionResult WebinarSign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("WebinarSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult MspEveningSign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("MspEveningSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult SetkaniTymuSign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!IsReservationPossibility(id) || !IsValidPeopleContact(attendeeId) || !CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SetkaniTymuSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult SetkaniTymuUserSign(int id, int attendeeId, string currentFilter, string currentFilterAccording)
        {
            if (!CheckVisibleUserId(ref attendeeId) || !IsReservationPossibility(id))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveUserAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SetkaniTymuUserSignUnsign", new { id, currentFilter, currentFilterAccording });
        }

        public ActionResult SkoleniDavidaKotaskaSign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!IsReservationPossibility(id) || !IsValidPeopleContact(attendeeId) || !CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SkoleniDavidaKotaskaSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult SkoleniDavidaKotaskaUserSign(int id, int attendeeId, string currentFilter, string currentFilterAccording)
        {
            if (!CheckVisibleUserId(ref attendeeId) || !IsReservationPossibility(id))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveUserAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SkoleniDavidaKotaskaUserSignUnsign", new { id, currentFilter, currentFilterAccording });
        }

        public ActionResult OstatniSign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("OstatniSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult OstatniUserSign(int id, int attendeeId, string currentFilter, string currentFilterAccording)
        {
            if (!CheckVisibleUserId(ref attendeeId) || !IsReservationPossibility(id))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveUserAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("OstatniUserSignUnsign", new { id, currentFilter, currentFilterAccording });
        }

        public ActionResult BusinessInfoUnsign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignAttendee(Db, id, attendeeId, UserId, MeetingType.Lgs, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("BusinessInfoSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult BusinessInfoUserUnsign(int id, int attendeeId, string currentFilter, string currentFilterAccording)
        {
            try
            {
                MeetingAttendeeCache.UnsignUserAttendee(Db, id, attendeeId, UserId, MeetingType.Lgs, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("BusinessInfoUserSignUnsign", new { id, currentFilter, currentFilterAccording });
        }

        public ActionResult WebinarUnsign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignAttendee(Db, id, attendeeId, UserId, MeetingType.Webinar, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("WebinarSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult MspEveningUnsign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignAttendee(Db, id, attendeeId, UserId, MeetingType.MspEvening, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("MspEveningSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult SetkaniTymuUnsign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignAttendee(Db, id, attendeeId, UserId, MeetingType.SetkaniTymu, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SetkaniTymuSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult SetkaniTymuUserUnsign(int id, int attendeeId, string currentFilter, string currentFilterAccording)
        {
            try
            {
                MeetingAttendeeCache.UnsignUserAttendee(Db, id, attendeeId, UserId, MeetingType.SetkaniTymu, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SetkaniTymuUserSignUnsign", new { id, currentFilter, currentFilterAccording });
        }

        public ActionResult SkoleniDavidaKotaskaUnsign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignAttendee(Db, id, attendeeId, UserId, MeetingType.SkoleniDavidaKotaska, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SkoleniDavidaKotaskaSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult SkoleniDavidaKotaskaUserUnsign(int id, int attendeeId, string currentFilter, string currentFilterAccording)
        {
            try
            {
                MeetingAttendeeCache.UnsignUserAttendee(Db, id, attendeeId, UserId, MeetingType.SkoleniDavidaKotaska, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SkoleniDavidaKotaskaUserSignUnsign", new { id, currentFilter, currentFilterAccording });
        }

        public ActionResult OstatniUnsign(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignAttendee(Db, id, attendeeId, UserId, MeetingType.Ostatni, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("OstatniSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult OstatniUserUnsign(int id, int attendeeId, string currentFilter, string currentFilterAccording)
        {
            try
            {
                MeetingAttendeeCache.UnsignUserAttendee(Db, id, attendeeId, UserId, MeetingType.Ostatni, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("OstatniUserSignUnsign", new { id, currentFilter, currentFilterAccording });
        }

        public ActionResult BusinessInfoSignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveUserAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("BusinessInfoSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult WebinarSignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveUserAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("WebinarSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult MspEveningSignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveUserAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("MspEveningSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult SetkaniTymuSignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!IsReservationPossibility(id) || !CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveUserAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SetkaniTymuSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult SkoleniDavidaKotaskaSignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!IsReservationPossibility(id) || !CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveUserAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SkoleniDavidaKotaskaSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult OstatniSignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.SignOrReserveUserAttendee(Db, id, attendeeId, UserId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("OstatniSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult BusinessInfoUnsignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignUserAttendee(Db, id, attendeeId, UserId, MeetingType.Lgs, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("BusinessInfoSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult WebinarUnsignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignUserAttendee(Db, id, attendeeId, UserId, MeetingType.Webinar, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("WebinarSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult MspEveningUnsignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignUserAttendee(Db, id, attendeeId, UserId, MeetingType.MspEvening, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("MspEveningSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult SetkaniTymuUnsignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignUserAttendee(Db, id, attendeeId, UserId, MeetingType.SetkaniTymu, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SetkaniTymuSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult SkoleniDavidaKotaskaUnsignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignUserAttendee(Db, id, attendeeId, UserId, MeetingType.SkoleniDavidaKotaska, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("SkoleniDavidaKotaskaSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult OstatniUnsignUser(int id, int attendeeId, string currentFilter, string currentFilterAccording, int filteredUserId = 0)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            try
            {
                MeetingAttendeeCache.UnsignUserAttendee(Db, id, attendeeId, UserId, MeetingType.Ostatni, IsAdmin);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            return RedirectToAction("OstatniSignUnsign", new { id, currentFilter, currentFilterAccording, filteredUserId });
        }

        public ActionResult BusinessInfoPrintDetail(int id, string sortOrder)
        {
            var meetingBusinessInfoDetail = (MeetingBusinessInfoDetail)ProcessDetails(MeetingType.Lgs, id, sortOrder);
            if (!meetingBusinessInfoDetail.IsValid)
            {
                return RedirectToAccessDenied();
            }

            if (meetingBusinessInfoDetail.MeetingAttendees.Length > 1000)
            {
                meetingBusinessInfoDetail.MeetingAttendees = meetingBusinessInfoDetail.MeetingAttendees.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            return View("BusinessInfoPrintDetail", "_PrintLayout", meetingBusinessInfoDetail);
        }

        public ActionResult WebinarPrintDetail(int id, string sortOrder)
        {
            var meetingWebinarDetail = (MeetingWebinarDetail)ProcessDetails(MeetingType.Webinar, id, sortOrder);
            if (!meetingWebinarDetail.IsValid)
            {
                return RedirectToAccessDenied();
            }

            if (meetingWebinarDetail.MeetingAttendees.Length > 1000)
            {
                meetingWebinarDetail.MeetingAttendees = meetingWebinarDetail.MeetingAttendees.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            return View("WebinarPrintDetail", "_PrintLayout", meetingWebinarDetail);
        }

        public ActionResult MspEveningPrintDetail(int id, string sortOrder)
        {
            var meetingMspEveningDetail = (MeetingMspEveningDetail)ProcessDetails(MeetingType.MspEvening, id, sortOrder);
            if (!meetingMspEveningDetail.IsValid)
            {
                return RedirectToAccessDenied();
            }

            if (meetingMspEveningDetail.MeetingAttendees.Length > 1000)
            {
                meetingMspEveningDetail.MeetingAttendees = meetingMspEveningDetail.MeetingAttendees.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            return View("MspEveningPrintDetail", "_PrintLayout", meetingMspEveningDetail);
        }

        public ActionResult SetkaniTymuPrintDetail(int id, string sortOrder)
        {
            var meetingSetkaniTymuDetail = (MeetingSetkaniTymuDetail)ProcessDetails(MeetingType.SetkaniTymu, id, sortOrder);
            if (!meetingSetkaniTymuDetail.IsValid)
            {
                return RedirectToAccessDenied();
            }

            if (meetingSetkaniTymuDetail.MeetingAttendees.Length > 1000)
            {
                meetingSetkaniTymuDetail.MeetingAttendees = meetingSetkaniTymuDetail.MeetingAttendees.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            return View("SetkaniTymuPrintDetail", "_PrintLayout", meetingSetkaniTymuDetail);
        }

        public ActionResult SkoleniDavidaKotaskaPrintDetail(int id, string sortOrder)
        {
            var meetingSkoleniDavidaKotaskaDetail = (MeetingSkoleniDavidaKotaskaDetail)ProcessDetails(MeetingType.SkoleniDavidaKotaska, id, sortOrder);
            if (!meetingSkoleniDavidaKotaskaDetail.IsValid)
            {
                return RedirectToAccessDenied();
            }

            if (meetingSkoleniDavidaKotaskaDetail.MeetingAttendees.Length > 1000)
            {
                meetingSkoleniDavidaKotaskaDetail.MeetingAttendees = meetingSkoleniDavidaKotaskaDetail.MeetingAttendees.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            return View("SkoleniDavidaKotaskaPrintDetail", "_PrintLayout", meetingSkoleniDavidaKotaskaDetail);
        }

        public ActionResult OstatniPrintDetail(int id, string sortOrder)
        {
            var meetingOstatniDetail = (MeetingOstatniDetail)ProcessDetails(MeetingType.Ostatni, id, sortOrder);
            if (!meetingOstatniDetail.IsValid)
            {
                return RedirectToAccessDenied();
            }

            if (meetingOstatniDetail.MeetingAttendees.Length > 1000)
            {
                meetingOstatniDetail.MeetingAttendees = meetingOstatniDetail.MeetingAttendees.Take(1000).ToArray();
                ViewBag.IsListCuttedMessage = ViewResource.Global_IsListCutted_Text;
            }

            return View("OstatniPrintDetail", "_PrintLayout", meetingOstatniDetail);
        }

        public ActionResult BusinessInfoDeleteAttendee(int id)
        {
            int meetingId = 0;
            try
            {
                MeetingAttendeeCache.UnsignMeetingAttendee(Db, id, UserId, MeetingType.Lgs, IsAdmin, ref meetingId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            if (meetingId == 0)
                return RedirectToAccessDenied();

            return RedirectToAction("BusinessInfoDetails", new { id = meetingId });
        }

        public ActionResult WebinarDeleteAttendee(int id)
        {
            int meetingId = 0;
            try
            {
                MeetingAttendeeCache.UnsignMeetingAttendee(Db, id, UserId, MeetingType.Webinar, IsAdmin, ref meetingId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            if (meetingId == 0)
                return RedirectToAccessDenied();

            return RedirectToAction("WebinarDetails", new { id = meetingId });
        }

        public ActionResult MspEveningDeleteAttendee(int id)
        {
            int meetingId = 0;
            try
            {
                MeetingAttendeeCache.UnsignMeetingAttendee(Db, id, UserId, MeetingType.MspEvening, IsAdmin, ref meetingId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            if (meetingId == 0)
                return RedirectToAccessDenied();

            return RedirectToAction("MspEveningDetails", new { id = meetingId });
        }

        public ActionResult SetkaniTymuDeleteAttendee(int id)
        {
            int meetingId = 0;
            try
            {
                MeetingAttendeeCache.UnsignMeetingAttendee(Db, id, UserId, MeetingType.SetkaniTymu, IsAdmin, ref meetingId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            if (meetingId == 0)
                return RedirectToAccessDenied();

            return RedirectToAction("SetkaniTymuDetails", new { id = meetingId });
        }

        public ActionResult SkoleniDavidaKotaskaDeleteAttendee(int id)
        {
            int meetingId = 0;
            try
            {
                MeetingAttendeeCache.UnsignMeetingAttendee(Db, id, UserId, MeetingType.SkoleniDavidaKotaska, IsAdmin, ref meetingId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            if (meetingId == 0)
                return RedirectToAccessDenied();

            return RedirectToAction("SkoleniDavidaKotaskaDetails", new { id = meetingId });
        }

        public ActionResult OstatniDeleteAttendee(int id)
        {
            int meetingId = 0;
            try
            {
                MeetingAttendeeCache.UnsignMeetingAttendee(Db, id, UserId, MeetingType.Ostatni, IsAdmin, ref meetingId);
            }
            catch (Exception e)
            {
                Logger.SetLog(e); TempData[StatusMessageTempKey] = e.Message;
            }

            if (meetingId == 0)
                return RedirectToAccessDenied();

            return RedirectToAction("OstatniDetails", new { id = meetingId });
        }

        public ActionResult OstatniLockAttendee(int id)
        {
            int? meetingId = ProcessLockAttendee(id);
            if (meetingId == null)
            {
                return RedirectToAccessDenied();
            }

            return RedirectToAction("OstatniDetails", new { id = meetingId });
        }

        public ActionResult OstatniUnlockAttendee(int id)
        {
            int? meetingId = ProcessUnlockAttendee(id);
            if (meetingId == null)
            {
                return RedirectToAccessDenied();
            }

            return RedirectToAction("OstatniDetails", new { id = meetingId });
        }

        public ActionResult SetkaniTymuLockAttendee(int id)
        {
            int? meetingId = ProcessLockAttendee(id);
            if (meetingId == null)
            {
                return RedirectToAccessDenied();
            }

            return RedirectToAction("SetkaniTymuDetails", new { id = meetingId });
        }

        public ActionResult SetkaniTymuUnlockAttendee(int id)
        {
            int? meetingId = ProcessUnlockAttendee(id);
            if (meetingId == null)
            {
                return RedirectToAccessDenied();
            }

            return RedirectToAction("SetkaniTymuDetails", new { id = meetingId });
        }

        public ActionResult SkoleniDavidaKotaskaLockAttendee(int id)
        {
            int? meetingId = ProcessLockAttendee(id);
            if (meetingId == null)
            {
                return RedirectToAccessDenied();
            }

            return RedirectToAction("SkoleniDavidaKotaskaDetails", new { id = meetingId });
        }

        public ActionResult SkoleniDavidaKotaskaUnlockAttendee(int id)
        {
            int? meetingId = ProcessUnlockAttendee(id);
            if (meetingId == null)
            {
                return RedirectToAccessDenied();
            }

            return RedirectToAction("SkoleniDavidaKotaskaDetails", new { id = meetingId });
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult BusinessInfoArchiveIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingBusinessInfoIndex>)ProcessIndex(showAll, MeetingType.Lgs, true, sortOrder,
                                                                              currentFilter, currentFilterAccording,
                                                                              searchString, searchStringAccording,
                                                                              page,
                                                                              pageSize);
            return View(pagedRows);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult WebinarArchiveIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingWebinarIndex>)ProcessIndex(showAll, MeetingType.Webinar, true, sortOrder,
                                                                              currentFilter, currentFilterAccording,
                                                                              searchString, searchStringAccording,
                                                                              page,
                                                                              pageSize);
            return View(pagedRows);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult MspEveningArchiveIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingMspEveningIndex>)ProcessIndex(showAll, MeetingType.MspEvening, true, sortOrder,
                                                                              currentFilter, currentFilterAccording,
                                                                              searchString, searchStringAccording,
                                                                              page,
                                                                              pageSize);
            return View(pagedRows);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult SetkaniTymuArchiveIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingSetkaniTymuIndex>)ProcessIndex(showAll, MeetingType.SetkaniTymu, true, sortOrder,
                                                                              currentFilter, currentFilterAccording,
                                                                              searchString, searchStringAccording,
                                                                              page,
                                                                              pageSize);
            return View(pagedRows);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult SkoleniDavidaKotaskaArchiveIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingSkoleniDavidaKotaskaIndex>)ProcessIndex(showAll, MeetingType.SkoleniDavidaKotaska, true, sortOrder,
                                                                              currentFilter, currentFilterAccording,
                                                                              searchString, searchStringAccording,
                                                                              page,
                                                                              pageSize);
            return View(pagedRows);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult OstatniArchiveIndex(string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, bool showAll = false, int pageSize = PageSize)
        {
            var pagedRows = (IPagedList<MeetingOstatniIndex>)ProcessIndex(showAll, MeetingType.Ostatni, true, sortOrder,
                                                                              currentFilter, currentFilterAccording,
                                                                              searchString, searchStringAccording,
                                                                              page,
                                                                              pageSize);
            return View(pagedRows);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult BusinessInfoArchiveDetails(string sortOrder, int id = 0)
        {
            var meetingBusinessInfoDetail = (MeetingBusinessInfoDetail)ProcessDetails(MeetingType.Lgs, id, sortOrder);
            if (!meetingBusinessInfoDetail.IsValid || !meetingBusinessInfoDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingBusinessInfoDetail);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult WebinarArchiveDetails(string sortOrder, int id = 0)
        {
            var meetingWebinarDetail = (MeetingWebinarDetail)ProcessDetails(MeetingType.Webinar, id, sortOrder);
            if (!meetingWebinarDetail.IsValid || !meetingWebinarDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingWebinarDetail);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult MspEveningArchiveDetails(string sortOrder, int id = 0)
        {
            var meetingMspEveningDetail = (MeetingMspEveningDetail)ProcessDetails(MeetingType.MspEvening, id, sortOrder);
            if (!meetingMspEveningDetail.IsValid || !meetingMspEveningDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingMspEveningDetail);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult SetkaniTymuArchiveDetails(string sortOrder, int id = 0)
        {
            var meetingSetkaniTymuDetail = (MeetingSetkaniTymuDetail)ProcessDetails(MeetingType.SetkaniTymu, id, sortOrder);
            if (!meetingSetkaniTymuDetail.IsValid || !meetingSetkaniTymuDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSetkaniTymuDetail);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult SkoleniDavidaKotaskaArchiveDetails(string sortOrder, int id)
        {
            var meetingSkoleniDavidaKotaskaDetail = (MeetingSkoleniDavidaKotaskaDetail)ProcessDetails(MeetingType.SkoleniDavidaKotaska, id, sortOrder);
            if (!meetingSkoleniDavidaKotaskaDetail.IsValid || !meetingSkoleniDavidaKotaskaDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingSkoleniDavidaKotaskaDetail);
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult OstatniArchiveDetails(string sortOrder, int id = 0)
        {
            var meetingOstatniDetail = (MeetingOstatniDetail)ProcessDetails(MeetingType.Ostatni, id, sortOrder);
            if (!meetingOstatniDetail.IsValid || !meetingOstatniDetail.IsArchive)
            {
                return RedirectToAccessDenied();
            }

            return View(meetingOstatniDetail);
        }

        private object ProcessIndex(bool showAll, MeetingType meetingType, bool getArchive, string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page, int pageSize)
        {
            int pageNumber;
            ProcessSearchFilterAndPaging(currentFilter, currentFilterAccording, ref searchString, ref searchStringAccording, ref page, out pageNumber);

            string[] sortingNames;
            switch (meetingType)
            {
                case MeetingType.Lgs:
                case MeetingType.SkoleniDavidaKotaska:
                case MeetingType.MspEvening:
                    sortingNames = new[] { BaseCache.DateField, BaseCache.CityField, BaseCache.AddressLine1Field, BaseCache.OrganizerField };
                    ProcessSorting(sortOrder, sortingNames);
                    break;

                case MeetingType.Webinar:
                    sortingNames = new[] { BaseCache.DateField, BaseCache.OrganizerField };
                    ProcessSorting(sortOrder, sortingNames);
                    break;

                case MeetingType.SetkaniTymu:
                case MeetingType.Ostatni:
                    sortingNames = new[] { BaseCache.DateField, BaseCache.MeetingTitleTypeField, BaseCache.CityField, BaseCache.AddressLine1Field, BaseCache.OrganizerField };
                    ProcessSorting(sortOrder, sortingNames);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(meetingType));
            }

            PopulateMeetingSearchStringAccording(meetingType, searchString, searchStringAccording);
            PopulatePageSize(pageSize);

            ViewBag.ShowShowAll = !IsAdmin && !showAll;
            ViewBag.ShowAvailable = !IsAdmin && showAll;
            ViewBag.IsShowAll = showAll;

            object pagedRows;
            switch (meetingType)
            {
                case MeetingType.Lgs:
                    IEnumerable<MeetingBusinessInfoIndex> meetingBusinessInfoIndices = MeetingBusinessInfoIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, getArchive, searchString, searchStringAccording, sortOrder);
                    pagedRows = meetingBusinessInfoIndices.ToPagedList(pageNumber, pageSize);
                    break;

                case MeetingType.Webinar:
                    IEnumerable<MeetingWebinarIndex> meetingWebinarIndices = MeetingWebinarIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, getArchive, searchString, searchStringAccording, sortOrder);
                    pagedRows = meetingWebinarIndices.ToPagedList(pageNumber, pageSize);
                    break;

                case MeetingType.SetkaniTymu:
                    IEnumerable<MeetingSetkaniTymuIndex> meetingSetkaniTymuIndices = MeetingSetkaniTymuIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, getArchive, searchString, searchStringAccording, sortOrder);
                    pagedRows = meetingSetkaniTymuIndices.ToPagedList(pageNumber, pageSize);
                    break;

                case MeetingType.SkoleniDavidaKotaska:
                    IEnumerable<MeetingSkoleniDavidaKotaskaIndex> meetingSkoleniDavidaKotaskaIndex = MeetingSkoleniDavidaKotaskaIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, getArchive, searchString, searchStringAccording, sortOrder);
                    pagedRows = meetingSkoleniDavidaKotaskaIndex.ToPagedList(pageNumber, pageSize);
                    break;

                case MeetingType.Ostatni:
                    IEnumerable<MeetingOstatniIndex> meetingOstatniIndex = MeetingOstatniIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, getArchive, searchString, searchStringAccording, sortOrder);
                    pagedRows = meetingOstatniIndex.ToPagedList(pageNumber, pageSize);
                    break;

                case MeetingType.MspEvening:
                    IEnumerable<MeetingMspEveningIndex> meetingMspEveningIndices = MeetingMspEveningIndex.GetIndexRows(Db, UserId, IsAdmin, showAll, getArchive, searchString, searchStringAccording, sortOrder);
                    pagedRows = meetingMspEveningIndices.ToPagedList(pageNumber, pageSize);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(meetingType));
            }

            return pagedRows;
        }

        private object ProcessDetails(MeetingType meetingType, int id, string sortOrder)
        {
            var sortingNames = new[] { BaseCache.LastNameField, BaseCache.LyonessIdField };
            ProcessSorting(sortOrder, sortingNames);

            Meeting meeting = MeetingCache.GetDetail(Db, id);
            object meetingDetail;
            switch (meetingType)
            {
                case MeetingType.Lgs:
                    meetingDetail = new MeetingBusinessInfoDetail(meeting, Db, UserId, IsAdmin, sortOrder);
                    break;

                case MeetingType.Webinar:
                    meetingDetail = new MeetingWebinarDetail(meeting, Db, UserId, IsAdmin, sortOrder);
                    break;

                case MeetingType.MspEvening:
                    meetingDetail = new MeetingMspEveningDetail(meeting, Db, UserId, IsAdmin, sortOrder);
                    break;

                case MeetingType.SetkaniTymu:
                    meetingDetail = new MeetingSetkaniTymuDetail(meeting, Db, UserId, IsAdmin, sortOrder);
                    break;

                case MeetingType.SkoleniDavidaKotaska:
                    meetingDetail = new MeetingSkoleniDavidaKotaskaDetail(meeting, Db, UserId, IsAdmin, sortOrder);
                    break;

                case MeetingType.Ostatni:
                    meetingDetail = new MeetingOstatniDetail(meeting, Db, UserId, IsAdmin, sortOrder);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(meetingType));
            }

            return meetingDetail;
        }

        private object ProcessSignUnsign(MeetingType meetingType, string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize, int filteredUserId, int id)
        {
            if (!CheckFilteredUserId(ref filteredUserId))
            {
                return RedirectToAccessDenied();
            }

            int pageNumber;
            ProcessSearchFilterAndPaging(currentFilter, currentFilterAccording, ref searchString, ref searchStringAccording, ref page, out pageNumber);

            int signedPageNumber;
            ProcessPaging(signedPage, out signedPageNumber);

            Meeting meeting = MeetingCache.GetDetail(Db, id);
            object meetingSignUnsign;
            switch (meetingType)
            {
                case MeetingType.Lgs:
                    var meetingBusinessInfoSignUnsign = new MeetingBusinessInfoSignUnsign(meeting, Db, id, IsAdmin, filteredUserId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
                    if (!meetingBusinessInfoSignUnsign.IsValid || !IsSignUnsignAccess(meetingBusinessInfoSignUnsign.MainLeaderId, meetingBusinessInfoSignUnsign.SecondaryLeaderId, meetingBusinessInfoSignUnsign.Finished))
                    {
                        return null;
                    }

                    meetingSignUnsign = meetingBusinessInfoSignUnsign;
                    break;

                case MeetingType.Webinar:
                    var meetingWebinarSignUnsign = new MeetingWebinarSignUnsign(meeting, Db, id, IsAdmin, filteredUserId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
                    if (!meetingWebinarSignUnsign.IsValid || !IsSignUnsignAccess(meetingWebinarSignUnsign.MainLeaderId, meetingWebinarSignUnsign.SecondaryLeaderId, meetingWebinarSignUnsign.Finished))
                    {
                        return null;
                    }

                    meetingSignUnsign = meetingWebinarSignUnsign;
                    break;

                case MeetingType.SetkaniTymu:
                    var meetingSetkaniTymuSignUnsign = new MeetingSetkaniTymuSignUnsign(meeting, Db, id, IsAdmin, filteredUserId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
                    if (!meetingSetkaniTymuSignUnsign.IsValid || !IsSignUnsignAccess(meetingSetkaniTymuSignUnsign.MainLeaderId, meetingSetkaniTymuSignUnsign.SecondaryLeaderId, meetingSetkaniTymuSignUnsign.Finished))
                    {
                        return null;
                    }

                    meetingSignUnsign = meetingSetkaniTymuSignUnsign;
                    break;

                case MeetingType.SkoleniDavidaKotaska:
                    var meetingSkoleniDavidaKotaskaSignUnsign = new MeetingSkoleniDavidaKotaskaSignUnsign(meeting, Db, id, IsAdmin, filteredUserId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
                    if (!meetingSkoleniDavidaKotaskaSignUnsign.IsValid || !IsSignUnsignAccess(meetingSkoleniDavidaKotaskaSignUnsign.MainLeaderId, meetingSkoleniDavidaKotaskaSignUnsign.SecondaryLeaderId, meetingSkoleniDavidaKotaskaSignUnsign.Finished))
                    {
                        return null;
                    }

                    meetingSignUnsign = meetingSkoleniDavidaKotaskaSignUnsign;
                    break;

                case MeetingType.Ostatni:
                    var meetingOstatniSignUnsign = new MeetingOstatniSignUnsign(meeting, Db, id, IsAdmin, filteredUserId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
                    if (!meetingOstatniSignUnsign.IsValid || !IsSignUnsignAccess(meetingOstatniSignUnsign.MainLeaderId, meetingOstatniSignUnsign.SecondaryLeaderId, meetingOstatniSignUnsign.Finished))
                    {
                        return null;
                    }

                    meetingSignUnsign = meetingOstatniSignUnsign;
                    break;

                case MeetingType.MspEvening:
                    var meetingMspEveningSignUnsign = new MeetingMspEveningSignUnsign(meeting, Db, id, IsAdmin, filteredUserId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
                    if (!meetingMspEveningSignUnsign.IsValid || !IsSignUnsignAccess(meetingMspEveningSignUnsign.MainLeaderId, meetingMspEveningSignUnsign.SecondaryLeaderId, meetingMspEveningSignUnsign.Finished))
                    {
                        return null;
                    }

                    meetingSignUnsign = meetingMspEveningSignUnsign;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(meetingType));
            }

            SetSignUnsignPageAndSortViewBag(sortOrder, signedSortOrder, pageNumber, signedPageNumber);

            PopulatePageSize(pageSize);
            PopulateFilteredUserId(filteredUserId);
            PopulateSignUnsignSearchStringAccording(searchString, searchStringAccording);

            return meetingSignUnsign;
        }

        private object ProcessUserSignUnsign(MeetingType meetingType, string sortOrder, string currentFilter, string currentFilterAccording, string searchString, string searchStringAccording, int? page,
            string signedSortOrder, int? signedPage,
            int pageSize, int id)
        {
            int pageNumber;
            ProcessSearchFilterAndPaging(currentFilter, currentFilterAccording, ref searchString, ref searchStringAccording, ref page, out pageNumber);

            int signedPageNumber;
            ProcessPaging(signedPage, out signedPageNumber);

            Meeting meeting = MeetingCache.GetDetail(Db, id);
            object meetingUserSignUnsign;
            switch (meetingType)
            {
                case MeetingType.Lgs:
                    var meetingBusinessInfoUserSignUnsign = new MeetingBusinessInfoUserSignUnsign(meeting, Db, id, IsAdmin, UserId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
                    if (!meetingBusinessInfoUserSignUnsign.IsValid || !IsSignUnsignAccess(meetingBusinessInfoUserSignUnsign.MainLeaderId, meetingBusinessInfoUserSignUnsign.SecondaryLeaderId, meetingBusinessInfoUserSignUnsign.Finished))
                    {
                        return null;
                    }

                    meetingUserSignUnsign = meetingBusinessInfoUserSignUnsign;
                    break;

                case MeetingType.SetkaniTymu:
                    var meetingSetkaniTymuSignUnsign = new MeetingSetkaniTymuUserSignUnsign(meeting, Db, id, IsAdmin, UserId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
                    if (!meetingSetkaniTymuSignUnsign.IsValid || !IsSignUnsignAccess(meetingSetkaniTymuSignUnsign.MainLeaderId, meetingSetkaniTymuSignUnsign.SecondaryLeaderId, meetingSetkaniTymuSignUnsign.Finished))
                    {
                        return null;
                    }

                    meetingUserSignUnsign = meetingSetkaniTymuSignUnsign;
                    break;

                case MeetingType.SkoleniDavidaKotaska:
                    var meetingSkoleniDavidaKotaskaUserSignUnsign = new MeetingSkoleniDavidaKotaskaUserSignUnsign(meeting, Db, id, IsAdmin, UserId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
                    if (!meetingSkoleniDavidaKotaskaUserSignUnsign.IsValid || !IsSignUnsignAccess(meetingSkoleniDavidaKotaskaUserSignUnsign.MainLeaderId, meetingSkoleniDavidaKotaskaUserSignUnsign.SecondaryLeaderId, meetingSkoleniDavidaKotaskaUserSignUnsign.Finished))
                    {
                        return null;
                    }

                    meetingUserSignUnsign = meetingSkoleniDavidaKotaskaUserSignUnsign;
                    break;

                case MeetingType.Ostatni:
                    var meetingOstatniUserSignUnsign = new MeetingOstatniUserSignUnsign(meeting, Db, id, IsAdmin, UserId, searchString, searchStringAccording, sortOrder, signedSortOrder, pageNumber, pageSize, signedPageNumber);
                    if (!meetingOstatniUserSignUnsign.IsValid || !IsSignUnsignAccess(meetingOstatniUserSignUnsign.MainLeaderId, meetingOstatniUserSignUnsign.SecondaryLeaderId, meetingOstatniUserSignUnsign.Finished))
                    {
                        return null;
                    }

                    meetingUserSignUnsign = meetingOstatniUserSignUnsign;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(meetingType));
            }

            SetSignUnsignPageAndSortViewBag(sortOrder, signedSortOrder, page, signedPage);

            PopulatePageSize(pageSize);
            PopulateSignUnsignSearchStringAccording(searchString, searchStringAccording);

            return meetingUserSignUnsign;
        }

        private void SetSignUnsignPageAndSortViewBag(string sortOrder, string signedSortOrder, int? page, int? signedPage)
        {
            var sortingNames = new[] { BaseCache.LastNameField, BaseCache.FirstNameField, BaseCache.CityField };
            ProcessSorting(sortOrder, sortingNames);
            ProcessSorting(signedSortOrder, sortingNames, "Signed");

            ViewBag.CurrentPage = page;
            ViewBag.CurrentSignedPage = signedPage;
        }

        private int? ProcessLockAttendee(int id)
        {
            return ProcessLockUnlockAttendee(id, true);
        }

        private int? ProcessUnlockAttendee(int id)
        {
            return ProcessLockUnlockAttendee(id, false);
        }

        private int? ProcessLockUnlockAttendee(int id, bool lockAttendee)
        {
            MeetingAttendee meetingAttendee = MeetingAttendeeCache.GetDetail(Db, id);
            if (meetingAttendee == null || (!IsAdmin && meetingAttendee.Meeting.OrganizerId != UserId && meetingAttendee.Meeting.SecondaryOrganizerId != UserId && meetingAttendee.Registered.HasValue))
            {
                return null;
            }

            meetingAttendee.Reserved = lockAttendee ? DateTime.MaxValue : DateTime.Now;
            Db.SaveChanges();

            return meetingAttendee.MeetingId;
        }

        private bool IsSignUnsignAccess(int mainLeaderId, int? secondaryLeaderId, DateTime finished)
        {
            int[] validUserIds = UserProfileCache.GetDownlineUserIdsWithoutAdmins(Db, mainLeaderId, secondaryLeaderId);
            return validUserIds != null && (IsAdmin || validUserIds.Contains(UserId)) && finished > DateTime.Now;
        }

        private bool IsAccess(Meeting meeting, IMeetingEdit meetingEdit)
        {
            if (meeting == null || (!IsAdmin && meeting.OrganizerId != UserId && meeting.SecondaryOrganizerId != UserId) || meeting.Finished <= DateTime.Now)
            {
                return false;
            }

            return meetingEdit != null;
        }

        private bool IsReservationPossibility(int id)
        {
            Meeting meeting = MeetingCache.GetDetail(Db, id);
            return meeting != null && !(meeting.RegisterDeadline < DateTime.Now);
        }

        private bool IsValidPeopleContact(int id)
        {
            PeopleContact peopleContact = PeopleContactCache.GetDetail(Db, id);
            return !IsNullOrEmpty(peopleContact?.LyonessId) && !IsNullOrEmpty(peopleContact.PhoneNumber1);
        }

        private bool CheckVisibleUserId(ref int attendeeId)
        {
            if (attendeeId == UserId)
                return true;

            IEnumerable<UserProfileIndex> userProfiles = IsAdmin
                                                             ? UserProfileIndex.GetUserProfileIndexForAdmin(Db, UserId)
                                                             : UserProfileIndex.GetUserProfileIndex(Db, UserId);

            int userId = attendeeId;
            bool success = userProfiles.Any(up => up.UserId == userId);
            return success;
        }
    }
}