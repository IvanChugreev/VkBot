using System;
using System.Timers;
using TypesUsedByBot;

namespace VkBot
{
    // T - тип данных для Id чата получаемого из мессенджера
    class UserTimers<T>
    {
        private readonly Timer tmpTimerLesson;
        private readonly Timer tmpTimerWorkday;
        private readonly Bot<T> bot;
        private readonly T chatId;

        public string TextOfMessageAboutNextLesson { get; set; }

        public double HeadStartTimerLesson { get; private set; }

        public Timer TimerLesson { get; private set; }

        public string TextOfMessageAboutNextWorkday { get; set; }

        public double HeadStartTimerWorkday { get; private set; }

        public Timer TimerWorkday { get; private set; }

        // значение времени предупреждения об уроке по умолчанию 15 минут(в миллисекундах 900000)
        // значение времени предупреждения о дне по умолчанию 12 часов(в миллисекундах 43200000)
        public UserTimers(Bot<T> bot, T chatId, double headStartTimerLesson = 900000, double headStartWorkday = 43200000)
        {
            this.bot = bot;

            this.chatId = chatId;

            HeadStartTimerLesson = headStartTimerLesson;

            TimerLesson = new Timer() { AutoReset = false };

            TimerLesson.Elapsed += TimerLesson_Elapsed;

            HeadStartTimerWorkday = headStartWorkday;

            TimerWorkday = new Timer() { AutoReset = false };

            TimerWorkday.Elapsed += TimerWorkday_Elapsed;

            tmpTimerLesson = new Timer() { AutoReset = false };

            tmpTimerLesson.Elapsed += (sender, e) => UpdateTimerLesson();

            tmpTimerWorkday = new Timer() { AutoReset = false };

            tmpTimerWorkday.Elapsed += (sender, e) => UpdateTimerWokrday();
        }

        public void StartTimerLesson()
        {
            if (UpdateTimerLesson())
                TimerLesson.Start();
        }

        private void TimerLesson_Elapsed(object sender, ElapsedEventArgs e)
        {
            bot.MessangerApi.SendTextMessage(chatId, TextOfMessageAboutNextLesson);

            StartTimerLesson();
        }

        private bool UpdateTimerLesson()
        {
            Lesson lesson = bot.RepositoryApi.GetNextLesson(chatId);

            if (lesson == null)
                return false;

            if ((lesson.StartTime - DateTime.Now.TimeOfDay).Milliseconds < HeadStartTimerLesson + 100)
            {
                bot.MessangerApi.SendTextMessage(chatId, lesson.ToString());

                tmpTimerLesson.Interval = (lesson.StartTime - DateTime.Now.TimeOfDay).TotalMilliseconds;

                tmpTimerLesson.Start();
            }
            else
            {
                TextOfMessageAboutNextLesson = lesson.ToString();

                TimerLesson.Interval = (lesson.StartTime - DateTime.Now.TimeOfDay).TotalMilliseconds - HeadStartTimerLesson;
            }

            return true;
        }

        public void StartTimerWorkday()
        {
            UpdateTimerWokrday();

            TimerWorkday.Start();
        }

        private void TimerWorkday_Elapsed(object sender, ElapsedEventArgs e)
        {
            bot.MessangerApi.SendTextMessage(chatId, TextOfMessageAboutNextWorkday);

            StartTimerWorkday();
        }

        private void UpdateTimerWokrday()
        {
            (Workday workday, DateTime startTimeOfWorkday) = bot.RepositoryApi.GetStartTimeOfNextWokrday(chatId);

            if ((startTimeOfWorkday - DateTime.Now).Milliseconds < HeadStartTimerWorkday + 100)
            {
                bot.MessangerApi.SendTextMessage(chatId, workday.ToString());

                StartTimerLesson();

                tmpTimerWorkday.Interval = (startTimeOfWorkday - DateTime.Now).TotalMilliseconds;

                tmpTimerWorkday.Start();
            }
            else
            {
                TextOfMessageAboutNextWorkday = workday.ToString();

                TimerWorkday.Interval = (startTimeOfWorkday - DateTime.Now).TotalMilliseconds - HeadStartTimerWorkday;

                StartTimerLesson();
            }
        }

        public void Stop()
        {
            TimerLesson.Stop();

            TimerWorkday.Stop();
        }
    }
}
