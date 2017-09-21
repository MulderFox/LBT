using LBT.Cache;
using LBT.Filters;
using LBT.Models;
using LBT.ModelViews;
using PagedList;
using LBT.Resources;
using System;
using System.Linq;
using System.Web.Mvc;

namespace LBT.Controllers
{
    [InitializeSimpleMembership]
    [Authorize]
    public class TeamController : BaseController
    {
        public ActionResult Index(string sortOrder, int? page, int pageSize = PageSize)
        {
            int pageNumber;
            ProcessPaging(page, out pageNumber);

            var sortingNames = new[] { "Title" };
            ProcessSorting(sortOrder, sortingNames);

            PopulatePageSize(pageSize);

            TeamIndex[] teams = TeamIndex.GetIndex(Db, sortOrder, UserId, IsAdmin);
            return View(teams.ToPagedList(pageNumber, PageSize));
        }

        [Authorize(Roles = LeadingRoles)]
        public ActionResult Create()
        {
            PopulateMemberIds();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeamEdit teamEdit)
        {
            ModelState.Merge(teamEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                int[] memberIds;
                Team team = teamEdit.GetModel(UserId, out memberIds);
                TeamCache.Insert(Db, UserId, team, memberIds);
                return RedirectToAction("Index");
            }

            PopulateMemberIds(teamEdit.MemberIds);

            return View(teamEdit);
        }

        public ActionResult Edit(int id = 0)
        {
            Team team = TeamCache.GetDetail(Db, id);
            if (!IsAccessForEdit(team))
            {
                return RedirectToAccessDenied();
            }

            TeamEdit teamEdit = TeamEdit.GetModelView(team);

            PopulateMemberIds(teamEdit.MemberIds);

            return View(teamEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeamEdit teamEdit)
        {
            ModelState.Merge(teamEdit.Validate(Db));

            if (ModelState.IsValid)
            {
                int[] memberIds;
                Team team = teamEdit.GetModel(UserId, out memberIds);
                bool success = TeamCache.Update(Db, memberIds, ref team);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("Index");
            }

            PopulateMemberIds(teamEdit.MemberIds);

            return View(teamEdit);
        }

        public ActionResult EditNote(int id = 0)
        {
            Team team = TeamCache.GetDetail(Db, id);
            if (!IsAccessForEditNote(team))
            {
                return RedirectToAccessDenied();
            }

            TeamEditNote teamEditNote = TeamEditNote.GetModelView(team);

            return View(teamEditNote);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNote(TeamEditNote teamEditNote)
        {
            ModelState.Merge(teamEditNote.Validate(Db));

            if (ModelState.IsValid)
            {
                Team team = teamEditNote.GetModel(UserId);
                bool success = TeamCache.UpdateNote(Db, ref team);
                if (!success)
                {
                    return RedirectToAccessDenied();
                }

                return RedirectToAction("Index");
            }

            return View(teamEditNote);
        }

        public ActionResult Details(int id = 0)
        {
            Team team = TeamCache.GetDetail(Db, id);
            if (!IsAccessForDetails(team))
            {
                return RedirectToAccessDenied();
            }

            TeamDetails teamDetails = TeamDetails.GetModelView(team, UserId, IsAdmin);

            return View(teamDetails);
        }

        public ActionResult Delete(int id = 0)
        {
            Team team = TeamCache.GetDetail(Db, id);
            if (!IsAccessForDelete(team))
            {
                return RedirectToAccessDenied();
            }

            TeamDelete teamDelete = TeamDelete.GetModelView(team);

            return View(teamDelete);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Team team;
            DeleteResult deleteResult = TeamCache.Delete(Db, id, out team);

            switch (deleteResult)
            {
                case DeleteResult.Ok:
                    return RedirectToAction("Index");
                    
                case DeleteResult.AuthorizationFailed:
                    return RedirectToAccessDenied();

                case DeleteResult.DbFailed:
                    ModelState.AddModelError("Title", ValidationResource.Global_DeleteRecord_ErrorMessage);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            TeamDelete teamDelete = TeamDelete.GetModelView(team);

            return View(teamDelete);
        }

        private bool IsAccessForEdit(Team team)
        {
            if (team == null)
                return false;

            bool isAccess = IsAdmin || team.OwnerId == UserId;
            return isAccess;
        }

        private bool IsAccessForEditNote(Team team)
        {
            if (team == null)
                return false;

            bool isAccess = IsAdmin || team.OwnerId == UserId || team.TeamMembers.Any(tm => tm.MemberId == UserId);
            return isAccess;
        }

        private bool IsAccessForDetails(Team team)
        {
            bool isAccess = IsAccessForEditNote(team);
            return isAccess;
        }

        private bool IsAccessForDelete(Team team)
        {
            bool isAccess = IsAccessForEdit(team);
            return isAccess;
        }
    }
}