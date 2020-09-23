using FinancialPortal.Extensions;
using FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Helpers
{
    public class NotificationHelper
    {
        //trackNotifications for
        // change in transaction
        //bank account or budget/budget items reach warning/target amount
        private ApplicationDbContext db = new ApplicationDbContext();
        UserHelper userHelper = new UserHelper();
        
        public int NotificationCount()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var count = 0;
            foreach (var notification in db.Notifications.Where(n => n.RecipientId == userId))
            {
                if (notification.IsRead != true)
                {
                    count++;
                }
            }
            return count;
        }
        public List<Notification> GetNotifications()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            List<Notification> validNotifcations = new List<Notification>();
            foreach (var notification in db.Notifications.Where(n => n.RecipientId == userId))
            {
                if (notification.IsRead != true)
                {
                    validNotifcations.Add(notification);
                }
            }
            return validNotifcations;
        }

        public List<Notification> ListUsersNotifications(string userId)
        {
            var currentUserId = HttpContext.Current.User.Identity.GetUserId();
            return db.Notifications.Where(n => n.RecipientId == currentUserId && !n.IsRead).OrderByDescending(n => n.Created).ToList();
        }
    }
}