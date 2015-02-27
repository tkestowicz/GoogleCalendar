using System;
using System.Globalization;
using System.Linq;

namespace GoogleCalendar.Domain
{
    public class Author
    {
        public string Id { get; private set; }
        public string Culture { get; private set; }
        public string Timezone { get; private set; }

        public Author(string id)
        {
            Id = id;

            ChangeCulture(CultureInfo.CurrentCulture.Name);
            ChangeTimezone(TimeZoneInfo.Local.Id);
        }

        public void ChangeCulture(string culture)
        {
            var isSupported = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Any(c => c.Name == culture);

            if(!isSupported)
                throw new ArgumentException(string.Format("Given culture '{0}' is invalid.", culture), "culture");

            Culture = culture;
        }

        public void ChangeTimezone(string timezone)
        {
            var isSupported = TimeZoneInfo
                .GetSystemTimeZones()
                .Any(t => t.Id == timezone);

            if (!isSupported)
                throw new ArgumentException(string.Format("Given timezone '{0}' is invalid.", timezone), "timezone");

            Timezone = timezone;
        }
    }

    public abstract class CalendarEvent
    {
        public Author CreatedBy { get; set; }

        public bool IsFullDay { get; set; }

        public string Name { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public abstract bool IsRepeable { get; }
    }

    public class OneTimeEvent : CalendarEvent
    {
        public sealed override bool IsRepeable { get { return false; } }
    }

    public abstract class RepeatableEvent : CalendarEvent
    {
        public enum EventFrequency
        {
            OneTime,
            Daily,
            Weekly,
            Monthly,
            Yearly
        }

        public bool IsForever { get; protected set; }

        public int? EndsAfterTimes { get; protected set; }

        public DateTime? EndsAtParticularDate { get; protected set; }

        public int Interval { get; protected set; }

        public void NeverEnds()
        {
            IsForever = true;

            EndsAfterTimes = null;
            EndsAtParticularDate = null;
        }

        public void EndsAfter(int times)
        {
            EndsAfterTimes = times;

            IsForever = false;
            EndsAtParticularDate = null;
        }

        public void EndsAt(DateTime particularDate)
        {
            EndsAtParticularDate = particularDate;

            IsForever = false;
            EndsAfterTimes = null;
        }

        public abstract EventFrequency Frequency { get; }

        public abstract void RepeatEvery(int interval);

        public sealed override bool IsRepeable { get { return true; } }
    }

    public class WeeklyEvent : RepeatableEvent
    {
        [Flags]
        public enum Day
        {
            None,
            Monday = 1,
            Tuesday = 2,
            Wednesday = 4,
            Thursday = 8,
            Friday = 16,
            Saturday = 32,
            Sunday = 64
        }

        private Day occurance = Day.None;

        public Day Occurance
        {
            get
            {
                if (occurance != Day.None) return occurance;
                
                var nameOfStartingWeekDay = Enum.GetName(typeof(DayOfWeek), From.DayOfWeek) ?? Day.None.ToString();

                var day = (Day)Enum.Parse(typeof(Day), nameOfStartingWeekDay, ignoreCase: true);

                OccursAt(day);

                return occurance;
            }
            
            private set { occurance = value; }
        }

        public void OccursAt(Day day)
        {
            if(day == Day.None)
                throw new ArgumentException("Incorrect value of the day param.", "day");

            Occurance = day;
        }

        public override EventFrequency Frequency
        {
            get { return EventFrequency.Weekly; }
        }

        public override void RepeatEvery(int interval)
        {
            if (interval < 0 || interval > 52)
                throw new ArgumentOutOfRangeException("interval", "Incorrect number of the week.");

            Interval = interval;
        }
    }

    public class DailyEvent : RepeatableEvent
    {
        public override EventFrequency Frequency
        {
            get { return EventFrequency.Daily; }
        }

        public override void RepeatEvery(int interval)
        {
            var daysInMonth = DateTime.DaysInMonth(From.Year, From.Month);

            if (interval < 1 || interval > daysInMonth)
                throw new ArgumentOutOfRangeException("interval", "Incorrect number of days.");

            Interval = interval;
        }
    }

    public class MonthlyEvent : RepeatableEvent
    {
        public enum OccuranceOption
        {
            None,
            DayOfTheMonth,
            DayOfTheWeek
        }

        public OccuranceOption Occurance { get; private set; }

        public void OccursAt(OccuranceOption occurance)
        {
            if (occurance == OccuranceOption.None)
                throw new ArgumentException("Incorrect occurance value.", "occurance");

            Occurance = occurance;
        }

        public override EventFrequency Frequency
        {
            get { return EventFrequency.Monthly; }
        }

        public override void RepeatEvery(int interval)
        {
            if(interval < 1 || interval > 12)
                throw new ArgumentOutOfRangeException("interval", "Incorrect number of months.");

            Interval = interval;
        }
    }

    public class YearlyEvent : RepeatableEvent
    {
        public override EventFrequency Frequency
        {
            get { return EventFrequency.Daily; }
        }

        public override void RepeatEvery(int interval)
        {
            if(interval < 1)
                throw new ArgumentOutOfRangeException("interval", "Incorrect number of years.");
        }
    }

    public class BusinessDaysEvent : WeeklyEvent
    {
        public enum RepeatsAt
        {
            None,
            AllBusinessDays,
            EvenBusinessDays,
            OddBusinessDays
        }

        public override void RepeatEvery(int interval)
        {
            RepeatsAt result;
            var valueExists = !Enum.TryParse(interval.ToString(NumberFormatInfo.InvariantInfo), true, out result);

            if(!valueExists || result == RepeatsAt.None)
                    throw new ArgumentException("Incorrect interval given.", "interval");

            switch (result)
            {
                case RepeatsAt.AllBusinessDays:

                    OccursAt(Day.Monday | Day.Tuesday | Day.Wednesday | Day.Thursday | Day.Friday);

                    break;
                case RepeatsAt.EvenBusinessDays:

                    OccursAt(Day.Tuesday | Day.Thursday);

                    break;
                case RepeatsAt.OddBusinessDays:

                    OccursAt(Day.Monday | Day.Wednesday | Day.Friday);

                    break;
            }

            Interval = interval;
        }
    }

    public class ReminderInfo
    {
        public enum ReminderMethod
        {
            NotSet,
            Email,
            PopUp
        }

        public enum Unit
        {
            NotSet,
            Minutes,
            Hours,
            Weeks,
        }

        public ReminderMethod Method { get; private set; }

        public Unit WhenUnit { get; private set; }

        public int WhenValue { get; private set; }

        public void RemindVia(ReminderMethod method)
        {
            if (method == ReminderMethod.NotSet)
                throw new ArgumentException("Incorrect method selected.", "method");

            Method = method;
        }

        public void RemindInAdvance(Unit unit, int value)
        {
            if (unit == Unit.NotSet)
                throw new ArgumentException("Incorrect unit selected.", "unit");

            if (value < 1)
                throw new ArgumentException("Value has to be grater than or equal to 1.", "value");

            WhenUnit = unit;
            WhenValue = value;
        }

    }
}
