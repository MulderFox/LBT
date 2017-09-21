using LBT.DAL;
using LBT.Models;
using System;

namespace LBT.Cache
{
    public sealed class VideoTokenCache : BaseCache
    {
        public static VideoToken Insert(DefaultContext db, int videoId, int senderId, int recipientId)
        {
            var videoToken = new VideoToken
                                 {
                                     Expired = DateTime.Now.AddHours(Properties.Settings.Default.VideoTokenExpirationHours),
                                     VideoId = videoId,
                                     SenderId = senderId,
                                     RecipientId = recipientId
                                 };

            db.VideoTokens.Add(videoToken);

            db.SaveChanges();

            return videoToken;
        }

        public static VideoToken GetDetail(DefaultContext db, int id)
        {
            VideoToken videoToken = db.VideoTokens.Find(id);
            return videoToken;
        }

        public static void ExpireVideoTokens(DefaultContext db)
        {
            db.Database.ExecuteSqlCommand(ExpireVideoTokensTemplate);
        }

        public static bool Update(DefaultContext db, VideoToken videoToken)
        {
            int videoTokenId = videoToken.VideoTokenId;
            VideoToken dbVideoToken = GetDetail(db, videoTokenId);
            if (dbVideoToken == null)
                return false;

            dbVideoToken.CopyFrom(videoToken);

            db.SaveChanges();

            return true;
        }
    }
}