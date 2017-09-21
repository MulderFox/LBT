using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LBT.Models
{
    public class Video
    {
        public int VideoId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string RelativeFilePath { get; set; }

        [Required]
        public string EmailSubject { get; set; }

        [Required]
        public string EmailBody { get; set; }

        [Required]
        public string EmailSenderBody { get; set; }

        public bool AllUsers { get; set; }

        // Video duration in seconds
        public int Duration { get; set; }

        public virtual ICollection<VideoUser> VideoUsers { get; set; }

        public virtual ICollection<VideoToken> VideoTokens { get; set; }

        public void CopyFrom(Video video)
        {
            Title = video.Title;

            if (!String.IsNullOrEmpty(video.RelativeFilePath))
            {
                RelativeFilePath = video.RelativeFilePath;                
            }

            EmailSubject = video.EmailSubject;
            EmailBody = video.EmailBody;
            EmailSenderBody = video.EmailSenderBody;
            AllUsers = video.AllUsers;
            if (video.Duration > 0)
            {
                Duration = video.Duration;                
            }
        }
    }
}