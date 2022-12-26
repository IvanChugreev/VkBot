namespace TypesUsedByBot
{
    // T - тип данных для Id чата 
    public interface IMessengerApi<T>
    {
        void SendTextMessage(T recipientId, string text);

        List<MessageParams<T>> GetNewMessages();
    }

    // T - тип данных для Id чата 
    public interface IRepositoryApi<T>
    {
        Workday GetWorkdayForDate(T chatId, DateTime date);

        Lesson GetNextLesson(T chatId);

        (Workday, DateTime) GetStartTimeOfNextWokrday(T chatId);

        bool NewTimetable(T chatId, Timetable timetable);

        bool DeleteLesson(T chatId, bool numerator, DayOfWeek dayOfWeek, TimeSpan startTimeOfLesson);

        bool AddLesson(T chatId, bool numerator, DayOfWeek dayOfWeek, Lesson lesson);
    }
}
