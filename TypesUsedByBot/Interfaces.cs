namespace TypesUsedByBot
{
    // T - тип данных для Id чата 
    public interface IMessengerApi<T>
    {
        void SendTextMessege(T recipientId, string text);

        List<MessageParams<T>> GetNewMesseges();
    }

    // T - тип данных для Id чата 
    public interface IRepositoryApi<T>
    {
        // TODO: добавить event для нового дня и недели
        (Lesson, DateTime) NextLesson(T chatId);

        (Workday, DateTime) NextWokrday(T chatId);

        bool NewTimetable(T chatId, Timetable timetable);

        bool DeleteLesson(T chatId, bool numerator, DayOfWeek dayOfWeek, TimeSpan startTimeOfLesson);

        bool AddLesson(T chatId, bool numerator, DayOfWeek dayOfWeek, Lesson lesson);
    }
}
