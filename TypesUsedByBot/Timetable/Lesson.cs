namespace TypesUsedByBot
{
    public class Lesson
    {
        public string Name { get; set; }

        public string TeacherName { get; set; }

        public string CabinetNumber { get; set; }

        public TimeSpan StartTime { get; set; }

        public Lesson(string name, string teacherName, string cabinetNumber, TimeSpan startTime)
        {
            Name = name;
            TeacherName = teacherName;
            CabinetNumber = cabinetNumber;
            StartTime = startTime;
        }

        public override string ToString() => $"{StartTime:hh\\:mm} -- {Name} -- {CabinetNumber} -- {TeacherName}";
    }
}
