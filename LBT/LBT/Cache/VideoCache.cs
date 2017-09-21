using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace LBT.Cache
{
    public sealed class VideoCache : BaseCache
    {
        public static Video[] GetIndex(DefaultContext db, string sortOrder)
        {
            IQueryable<Video> videos = db.Videos.AsQueryable();

            switch (sortOrder)
            {
                default:
                    videos = videos.OrderBy(v => v.Title);
                    break;

                case TitleDescSortOrder:
                    videos = videos.OrderByDescending(v => v.Title);
                    break;
            }

            return videos.ToArray();
        }

        public static void Insert(DefaultContext db, Video video, int[] userIds)
        {
            db.Videos.Add(video);

            VideoUserCache.InsertWithoutSave(db, video.VideoId, userIds);

            db.SaveChanges();
        }

        public static Video GetDetail(DefaultContext db, int id)
        {
            Video video = db.Videos.Find(id);
            return video;
        }

        public static bool Update(DefaultContext db, Video video, int[] userIds)
        {
            int videoId = video.VideoId;
            Video dbVideo = GetDetail(db, videoId);
            if (dbVideo == null)
                return false;

            dbVideo.CopyFrom(video);

            VideoUserCache.UpdateWithoutSave(db, dbVideo, userIds);

            db.SaveChanges();

            return true;
        }

        public static DeleteResult Delete(DefaultContext db, Video video)
        {
            if (video == null)
                return DeleteResult.AuthorizationFailed;

            try
            {
                var parameter = new SqlParameter(VideoIdSqlparameter, video.VideoId);
                db.Database.ExecuteSqlCommand(CascadeRemoveVideoProcedureTemplate, parameter);
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