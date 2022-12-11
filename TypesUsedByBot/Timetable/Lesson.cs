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

        public Lesson(string info)
        {
            string[] parameters = info.Split(',');

            StartTime = TimeSpan.Parse(parameters[0]);

            Name = parameters[1];

            CabinetNumber = parameters[2];

            TeacherName = parameters[3];
        }
    }
}
