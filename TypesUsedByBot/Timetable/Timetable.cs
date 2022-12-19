namespace TypesUsedByBot
{
    public class Timetable
    {
        public string GroupName { get; set; }

        public List<Workday> Numerator { get; set; }                    //числитель

        public List<Workday> Denominator { get; set; }                  //знаменатель

        public Timetable(string groupName, List<Workday> numerator, List<Workday> denominator)
        {
            GroupName = groupName;
            Numerator = numerator;
            Denominator = denominator;
        }

        public override string ToString() => $"{GroupName}\r\n" + 
            "Числитель\r\n" + string.Join("\r\n", Numerator) + 
            "\r\nЗнаменатель\r\n" + string.Join("\r\n", Denominator);
    }
}
