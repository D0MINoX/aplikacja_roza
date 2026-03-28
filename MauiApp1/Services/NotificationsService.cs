using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Services
{
    public class NotificationsService
    {

        public async Task ScheduleWeeklyReminders()
        {

            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
            string savedTime = Preferences.Default.Get("ReminderTime", "20:00:00");
            if (!TimeSpan.TryParse(savedTime, out TimeSpan targetTime))
            {
                targetTime = new TimeSpan(20, 0, 0); 
            }
           
            for (int i = 0; i < 7; i++)
            {
                DateTime notifyDate = DateTime.Today.AddDays(i).Add(targetTime);

              
                if (notifyDate < DateTime.Now) continue;

                string dayKey = notifyDate.ToString("yyyy-MM-dd");
                if (Preferences.Default.Get($"Done_{dayKey}", false))
                {
                    LocalNotificationCenter.Current.Cancel(1000 + i);
                    continue;
                }

                var request = new NotificationRequest
                {
                    NotificationId = 1000 + i, 
                    Title = "Czas na Różaniec 📿",
                    Description = "Chwila wytchnienia z Maryją. Twoje dzisiejsze rozważanie już na Ciebie czeka.",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyDate,
                        RepeatType = NotificationRepeat.No
                    }
                };

                await LocalNotificationCenter.Current.Show(request);
            }
        }
    }
}
