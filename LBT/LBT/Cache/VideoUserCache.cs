using LBT.DAL;
using LBT.Models;
using System.Collections.Generic;
using System.Linq;

namespace LBT.Cache
{
    public sealed class VideoUserCache : BaseCache
    {
        public static void InsertWithoutSave(DefaultContext db, int videoId, int[] userIds)
        {
            IEnumerable<VideoUser> videoUsers = userIds == null
                                                    ? new VideoUser[0]
                                                    : userIds.Select(ui => new VideoUser
                                                                               {
                                                                                   VideoId = videoId,
                                                                                   UserProfileId = ui
                                                                               });

            foreach (VideoUser videoUser in videoUsers)
            {
                db.VideoUsers.Add(videoUser);
            }
        }

        public static void UpdateWithoutSave(DefaultContext db, Video dbVideo, int[] userIds)
        {
            int[] dbUserProfileIds = dbVideo.VideoUsers.Select(vu => vu.UserProfileId).ToArray();
            int[] deletedUserProfileIds = dbUserProfileIds.Except(userIds).ToArray();
            int[] newUserProfileIds = userIds.Except(dbUserProfileIds).ToArray();

            VideoUser[] deletedVideoUsers =
                deletedUserProfileIds.Select(dupi => dbVideo.VideoUsers.Where(vu => vu.UserProfileId == dupi))
                .SelectMany(vu => vu).ToArray();
            foreach (VideoUser videoUser in deletedVideoUsers)
            {
                db.VideoUsers.Remove(videoUser);
            }

            List<VideoUser> newVideoUsers = newUserProfileIds.Select(nupi => new VideoUser
                                                                                 {
                                                                                     VideoId = dbVideo.VideoId,
                                                                                     UserProfileId = nupi
                                                                                 }).ToList();
            foreach (VideoUser videoUser in newVideoUsers)
            {
                db.VideoUsers.Add(videoUser);
            }
        }
    }
}