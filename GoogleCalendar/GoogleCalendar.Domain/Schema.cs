using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCalendar.Domain
{
    public class Event
    {
        public string Id { get; set; }

        public string AuthorId { get; set; }

        public bool IsFullDay { get; set; }

        public string Name { get; set; }

        public OneTimeEventRange Range { get; set; }

        public bool Repeatable { get; set; }

        public string EventSerieId { get; set; }
    }

    public class OneTimeEventRange
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int NumberOfDays { get; set; }
        public int NumberOfHours { get; set; }
        public string TimeZone { get; set; }
        public string Culture { get; set; }
    }

    public class EventSerieRange
    {
        public DateTime StartsAt { get; set; }
        public EndsAt EndsAt { get; set; }
        public string TimeZone { get; set; }
        public string Culture { get; set; }
    }

    public class EndsAt
    {
        public bool? Never { get; set; }
        public int? AfterTimes { get; set; }
        public DateTime? ParticularDate { get; set; }
    }

    public class EventSerie
    {
        public string Id { get; set; }
        public int Frequency { get; set; }
        public EventSerieRange Range { get; set; }
        public WeeklyParams WeeklyParams { get; set; }
        public DailyParams DailyParams { get; set; }
        public MonthlyParams MonthlyParams { get; set; }
        public YearlyParams YearlyParams { get; set; }
        public Changes Changes { get; set; }
    }

    public class Changes
    {
        public ICollection<Event> ParticularEvents { get; set; }

        public ICollection<EventSerie> SubSeries { get; set; }
    }

    public class WeeklyParams
    {
        public int Interval { get; set; }
        public int Occurences { get; set; }
    }

    public class DailyParams
    {
        public int Interval { get; set; }
    }

    public class MonthlyParams
    {
        public int Interval { get; set; }
        public int RepeatedBy { get; set; }
    }

    public class YearlyParams
    {
        public int Interval { get; set; }
    }
}
