using System;
using System.Collections.Generic;

namespace GoogleCalendar.Domain
{
    public interface IEventsProjector<out TResult, in TQuery>
        where TQuery : EventsQuery
    {
        TResult ProjectEvents(TQuery query);
    }

    public interface IProjector
        : IEventsProjector<YearlyEventsProjection, YearlyEventsQuery>
        , IEventsProjector<MonthlyEventsProjection, MonthlyEventsQuery>
        , IEventsProjector<WeeklyEventsProjection, WeaklyEventsQuery>
        , IEventsProjector<DailyEventsProjection, DailyEventsQuery>
    {
    }

    public class YearlyEventsProjection
    {
        public struct EventsMonthlySummary
        {
            public int Month { get; internal set; }

            public int NumberOfEvents { get; internal set; }
        }

        public int AllEvents { get; internal set; }

        public int PassedEvents { get; internal set; }

        public int UpcomingEvents { get; internal set; }

        public IReadOnlyCollection<EventsMonthlySummary> MonthlyResults { get; internal set; }
    }

    public struct EventProjection
    {
        public string Name { get; internal set; }
        public DateTime From { get; internal set; }
        public DateTime To { get; internal set; }
        public bool IsFullDayEvent { get; internal set; }
    }

    public class MonthlyEventsProjection
    {
        public struct EventsPerDay
        {
            public int DayInMonth { get; internal set; }

            public IReadOnlyCollection<EventProjection> Events { get; internal set; }
        }

        public IReadOnlyCollection<EventsPerDay> DailyResults { get; internal set; }
    }

    public class WeeklyEventsProjection
    {
        public struct EventsPerDay
        {
            public int DayInWeek { get; internal set; }

            public IReadOnlyCollection<EventProjection> Events { get; internal set; }
        }

        public IReadOnlyCollection<EventsPerDay> DailyResults { get; internal set; }
    }

    public class DailyEventsProjection
    {
        public IReadOnlyCollection<EventProjection> DailyResults { get; internal set; }
    }

    public abstract class EventsQuery
    {
        public string CalendarOwnerId { get; set; }
    }

    public class YearlyEventsQuery : EventsQuery
    {
        public int Year { get; set; }
    }

    public class MonthlyEventsQuery : EventsQuery
    {
        public int Year { get; set; }

        public int Month { get; set; }
    }

    public class WeaklyEventsQuery : EventsQuery
    {
        public int Year { get; set; }

        public int Week { get; set; }
    }

    public class DailyEventsQuery : EventsQuery
    {
        public DateTime Day { get; set; }
    }

    public class CalendarItemsProjectionApiProofOfConcept
    {
        readonly IProjector projector = null;

        public void YearlyView()
        {
            YearlyEventsProjection projection = projector.ProjectEvents(new YearlyEventsQuery
            {
                CalendarOwnerId = null,
                Year = 2014,
            });
        }

        public void MonthlyView()
        {

            MonthlyEventsProjection projection = projector.ProjectEvents(new MonthlyEventsQuery
            {
                CalendarOwnerId = null,
                Year = 2014,
                Month = 1
            });
        }

        public void WeeklyView()
        {

            WeeklyEventsProjection projection = projector.ProjectEvents(new WeaklyEventsQuery
            {
                CalendarOwnerId = null,
                Year = 2014,
                Week = 31
            });
        }

        public void DailyView()
        {
            DailyEventsProjection projection = projector.ProjectEvents(new DailyEventsQuery
            {
                CalendarOwnerId = null,
                Day = new DateTime(2014, 4, 12)
            });
        }
    }
}
