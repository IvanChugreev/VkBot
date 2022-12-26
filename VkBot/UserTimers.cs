using System;
using System.Timers;
using TypesUsedByBot;

namespace VkBot
{
    // T - тип данных для Id чата получаемого из мессенджера
    class UserTimers<T>
    {
        private readonly Bot<T> bot;
        private readonly T chatId;
        private readonly Timer timerWorkday;
        private TimeSpan notificationTime;

        public TimeSpan NotificationTime 
        {
            get => notificationTime;
            set
            {
                notificationTime = value;

                if (timerWorkday.Enabled)
                    Start();
            }
        }

        public UserTimers(Bot<T> bot, T chatId)
        {
            this.bot = bot;

            this.chatId = chatId;

            timerWorkday = new Timer();

            NotificationTime = new TimeSpan(7, 30, 0);

            timerWorkday.Elapsed += TimerWorkday_Elapsed;
        }

        private void TimerWorkday_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerWorkday.Interval = (DateTime.Today.AddDays(1).Add(NotificationTime) - DateTime.Now).TotalMilliseconds;

            Workday workday = bot.RepositoryApi.GetWorkdayForDate(chatId, DateTime.Today);

            if (workday != null)
                bot.MessangerApi.SendTextMessage(chatId, workday.ToString());
        }

        public void Start()
        {
            if (DateTime.Now.TimeOfDay > NotificationTime)
                timerWorkday.Interval = (DateTime.Today.AddDays(1).Add(NotificationTime) - DateTime.Now).TotalMilliseconds;
            else
                timerWorkday.Interval = (NotificationTime - DateTime.Now.TimeOfDay).TotalMilliseconds + 100;

            timerWorkday.Start();
        }

        public void Stop() => timerWorkday.Stop();
    }
}
