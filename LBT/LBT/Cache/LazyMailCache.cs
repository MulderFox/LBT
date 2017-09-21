using System;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System.Linq;
using LBT.Resources;
using LBT.Services;

namespace LBT.Cache
{
    public sealed class LazyMailCache : BaseCache
    {
        public static void InsertWithoutSave(DefaultContext db, LazyMail lazyMail)
        {
            db.LazyMails.Add(lazyMail);
        }

        public static void SendLazyMails(DefaultContext db)
        {
            DateTime currentTime = DateTime.Now;
            LazyMail[] lazyMails = db.LazyMails.Where(lm => lm.TimeToSend <= currentTime).ToArray();
            foreach (LazyMail lazyMail in lazyMails)
            {
                Mail.SendEmail(lazyMail.Address, MailResource.TaskSchedulerController_SendLazyMails_Subject, lazyMail.TextBody, true, true);

                db.LazyMails.Remove(lazyMail);
                db.SaveChanges();
            }
        }
    }
}