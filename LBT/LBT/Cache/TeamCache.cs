using System.Data.SqlClient;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LBT.Cache
{
    public class TeamCache : BaseCache
    {
        public static Team[] GetIndex(DefaultContext db, string sortOrder)
        {
            IQueryable<Team> teams = db.Teams.AsQueryable();

            switch (sortOrder)
            {
                default:
                    teams = teams.OrderBy(ba => ba.Title);
                    break;

                case TitleDescSortOrder:
                    teams = teams.OrderByDescending(ba => ba.Title);
                    break;
            }

            return teams.ToArray();
        }

        public static void Insert(DefaultContext db, int userId, Team team, int[] memberIds)
        {
            db.Teams.Add(team);

            IEnumerable<TeamMember> teamMembers = memberIds == null
                                                      ? new TeamMember[0]
                                                      : memberIds.Select(mi => new TeamMember
                                                                                    {
                                                                                        TeamId = team.TeamId,
                                                                                        MemberId = mi
                                                                                    });
            foreach (var teamMember in teamMembers)
            {
                db.TeamMembers.Add(teamMember);
            }

            db.SaveChanges();
        }

        public static Team GetDetail(DefaultContext db, int id)
        {
            Team team = db.Teams.Find(id);
            return team;
        }

        public static bool Update(DefaultContext db, int[] memberIds, ref Team team)
        {
            int teamId = team.TeamId;
            Team dbTeam = GetDetail(db, teamId);
            if (dbTeam == null)
                return false;

            dbTeam.CopyFrom(team);

            int[] dbMemberIds = dbTeam.TeamMembers.Select(tm => tm.MemberId).ToArray();
            int[] deletedMemberIds = dbMemberIds.Except(memberIds).ToArray();
            int[] newMemberIds = memberIds.Except(dbMemberIds).ToArray();

            TeamMember[] deletedTeamMembers =
                deletedMemberIds.Select(dmi => db.TeamMembers.Where(tm => tm.TeamId == teamId && tm.MemberId == dmi))
                .SelectMany(tm => tm).ToArray();
            foreach (TeamMember teamMember in deletedTeamMembers)
            {
                db.TeamMembers.Remove(teamMember);
            }

            List<TeamMember> newTeamMembers = newMemberIds.Select(nmi => new TeamMember
                                                                             {
                                                                                 TeamId = teamId,
                                                                                 MemberId = nmi
                                                                             }).ToList();
            foreach (TeamMember teamMember in newTeamMembers)
            {
                db.TeamMembers.Add(teamMember);
            }

            db.SaveChanges();

            team = dbTeam;
            return true;
        }

        public static bool UpdateNote(DefaultContext db, ref Team team)
        {
            Team dbTeam = GetDetail(db, team.TeamId);
            if (dbTeam == null)
                return false;

            dbTeam.CopyNoteFrom(team);

            db.SaveChanges();

            team = dbTeam;
            return true;
        }

        public static DeleteResult Delete(DefaultContext db, int id, out Team team)
        {
            team = GetDetail(db, id);
            if (team == null)
                return DeleteResult.AuthorizationFailed;

            try
            {
                var parameter = new SqlParameter(TeamIdSqlParameter, team.TeamId);
                db.Database.ExecuteSqlCommand(CascadeRemoveTeamProcedureTemplate, parameter);
                return DeleteResult.Ok;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                return DeleteResult.DbFailed;
            }
        }
    }
}