namespace TypesUsedByBot
{
    public class Timetable
    {
        public string GroupName { get; set; }

        public List<Workday> Numerator { get; set; }                    //числитель

        public List<Workday> Denominator { get; set; }                  //знаменатель

        public Timetable(List<Workday> numerator, List<Workday> denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }
    }
}
