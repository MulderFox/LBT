using System.IO;
using System.Web;
using LBT.Cache;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System;
using LBT.Services;

namespace LBT.ModelViews
{
    public class StreamVideoStream : BaseModelView
    {
        public const string NoVideoFilePath = @"Videos\VideoNotFound.mp4";

        public int Id { get; private set; }

        public string Token { get; private set; }

        public string VideoRelativeFilePath { get; private set; }

        public string VideoAbsoluteFilePath { get; private set; }

        private readonly bool _isAdmin;

        private readonly HttpServerUtilityBase _server;

        public StreamVideoStream(bool isAdmin, int id, string token, HttpServerUtilityBase server)
        {
            _isAdmin = isAdmin;
            Id = id;
            Token = token;
            _server = server;
            VideoRelativeFilePath = NoVideoFilePath;
            VideoAbsoluteFilePath = FileService.GetAbsoluteFilePath(_server, NoVideoFilePath);

            FindVideoFilePath();
        }

        protected override bool OnIsValid()
        {
            return _isAdmin && Id > 0 || !String.IsNullOrEmpty(Token) || _server != null;
        }

        private void FindVideoFilePath()
        {
            if (!IsValid)
                return;

            Video video = null;
            using (var db = new DefaultContext())
            {
                if (_isAdmin && Id > 0)
                {
                    video = VideoCache.GetDetail(db, Id);
                }
                else if (!String.IsNullOrEmpty(Token))
                {
                    string decryptedToken = Cryptography.Decrypt(Token);
                    if (String.IsNullOrEmpty(decryptedToken))
                        return;

                    int videoTokenId;
                    if (!Int32.TryParse(decryptedToken, out videoTokenId) || videoTokenId < 1)
                        return;

                    VideoToken videoToken = VideoTokenCache.GetDetail(db, videoTokenId);
                    if (videoToken == null)
                        return;

                    if (!videoToken.IsPlayedByRecipient && videoToken.Sender != null && videoToken.Recipient != null)
                    {
                        DateTime timeToSend = DateTime.Now.AddSeconds(videoToken.Video.Duration);
                        string textBody = String.Format(videoToken.Video.EmailSenderBody, videoToken.Recipient.FullName, videoToken.Video.Title, timeToSend.ToString("dd. MM. yyyy HH:mm:ss"));
                        var lazyMail = new LazyMail
                                           {
                                               Address = videoToken.Sender.Email1,
                                               TextBody = textBody,
                                               TimeToSend = timeToSend
                                           };
                        LazyMailCache.InsertWithoutSave(db, lazyMail);

                        videoToken.IsPlayedByRecipient = true;

                        VideoTokenCache.Update(db, videoToken);
                    }

                    video = videoToken.Video;
                }
            }

            if (video == null)
                return;

            string videoAbsoluteFilePath = FileService.GetAbsoluteFilePath(_server, video.RelativeFilePath);
            if (!File.Exists(videoAbsoluteFilePath))
                return;

            VideoRelativeFilePath = video.RelativeFilePath;
            VideoAbsoluteFilePath = videoAbsoluteFilePath;
        }
    }
}