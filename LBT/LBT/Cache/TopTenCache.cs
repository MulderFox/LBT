// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 06-04-2014
//
// Last Modified By : zmikeska
// Last Modified On : 06-05-2014
// ***********************************************************************
// <copyright file="TopTenCache.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace LBT.Cache
{
    /// <summary>
    /// Class TopTenCache
    /// </summary>
    public class TopTenCache : BaseCache
    {
        public static TopTen[] GetIndex(DefaultContext db, int fromUserId, string searchStringAccording, string searchString, string sortOrder)
        {
            IQueryable<TopTen> topTens = db.TopTens.Where(tt => tt.FromUser.UserId == fromUserId);

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchStringAccording)
                {
                    case FirstNameField:
                        topTens = topTens.Where(tt => tt.ToUser.FirstName.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case LastNameField:
                        topTens = topTens.Where(tt => tt.ToUser.LastName.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case CityField:
                        topTens = topTens.Where(tt => tt.ToUser.City.ToUpper().Contains(searchString.ToUpper()));
                        break;

                    case LyonessIdField:
                        topTens = topTens.Where(tt => tt.ToUser.LyonessId.ToUpper().Contains(searchString.ToUpper()));
                        break;
                }
            }

            switch (sortOrder)
            {
                case LastNameDescSortOrder:
                    topTens = topTens.OrderByDescending(tt => tt.ToUser.LastName);
                    break;

                case FirstNameAscSortOrder:
                    topTens = topTens.OrderBy(tt => tt.ToUser.FirstName);
                    break;

                case FirstNameDescSortOrder:
                    topTens = topTens.OrderByDescending(tt => tt.ToUser.FirstName);
                    break;

                case StatusAscSortOrder:
                    topTens = topTens.OrderBy(tt => tt.Status);
                    break;

                case StatusDescSortOrder:
                    topTens = topTens.OrderByDescending(tt => tt.Status);
                    break;

                case LastContactAscSortOrder:
                    topTens = topTens.OrderBy(tt => tt.LastContact);
                    break;

                case LastContactDescSortOrder:
                    topTens = topTens.OrderByDescending(tt => tt.LastContact);
                    break;

                case ActualCareerStageAscSortOrder:
                    topTens = topTens.OrderBy(tt => tt.ActualCareerStage);
                    break;

                case ActualCareerStageDescSortOrder:
                    topTens = topTens.OrderByDescending(tt => tt.ActualCareerStage);
                    break;

                case RoleAscSortOrder:
                    topTens = topTens.OrderBy(tt => tt.Role);
                    break;

                case RoleDescSortOrder:
                    topTens = topTens.OrderByDescending(tt => tt.Role);
                    break;

                case LyonessIdAscSortOrder:
                    topTens = topTens.OrderBy(tt => tt.ToUser.LyonessId);
                    break;

                case LyonessIdDescSortOrder:
                    topTens = topTens.OrderByDescending(tt => tt.ToUser.LyonessId);
                    break;

                default:
                    topTens = topTens.OrderBy(tt => tt.ToUser.LastName);
                    break;
            }

            return topTens.ToArray();
        }

        /// <summary>
        /// Gets the top ten with to user.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="topTenId">The top ten id.</param>
        /// <param name="fromUserId">From user id.</param>
        /// <returns>TopTen.</returns>
        public static TopTen GetDetail(DefaultContext db, int topTenId, int fromUserId)
        {
            TopTen topTen = db.TopTens.Include(p => p.ToUser).SingleOrDefault(tt => tt.TopTenId == topTenId && tt.FromUserId == fromUserId);
            return topTen;
        }

        /// <summary>
        /// Inserts the specified db.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="fromUserId">From user id.</param>
        /// <param name="topTen">The top ten.</param>
        public static void Insert(DefaultContext db, int fromUserId, ref TopTen topTen)
        {
            topTen.FromUserId = fromUserId;
            db.TopTens.Add(topTen);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified db.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="fromUserId">From user id.</param>
        /// <param name="topTen">The top ten.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public static bool Update(DefaultContext db, int fromUserId, ref TopTen topTen)
        {
            TopTen dbTopTen = GetDetail(db, topTen.TopTenId, fromUserId);
            if (dbTopTen == null)
                return false;

            dbTopTen.CopyFrom(topTen);
            db.SaveChanges();

            topTen = dbTopTen;
            return true;
        }

        /// <summary>
        /// Deletes the specified db.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="topTenId">The top ten id.</param>
        /// <param name="fromUserId">From user id.</param>
        /// <param name="topTen">The top ten.</param>
        /// <returns>DeleteResult.</returns>
        public static DeleteResult Delete(DefaultContext db, int topTenId, int fromUserId, out TopTen topTen)
        {
            topTen = GetDetail(db, topTenId, fromUserId);
            if (topTen == null)
                return DeleteResult.AuthorizationFailed;

            try
            {
                db.TopTens.Remove(topTen);
                db.SaveChanges();
                return DeleteResult.Ok;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                return DeleteResult.DbFailed;
            }
        }

        public static void RefreshTopTenRoles(DefaultContext db)
        {
            db.Database.ExecuteSqlCommand(RefreshTopTenRolesProcedureTemplate);
        }
    }
}