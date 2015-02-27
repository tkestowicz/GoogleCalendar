using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCalendar.Domain
{
    public interface IStorage
    {
        ICollection<Author> Authors { get; }

        ICollection<Event> Events { get; }

        ICollection<EventSerie> EventSeries { get; }

        void Store();
    }

    public interface ISchedulerFor<TEvent>
        where TEvent : CalendarEvent
    {
        void Schedule(TEvent @event);

        void UpdateScheduled(UpdateScheduledEvent<TEvent> update);
    }

    public interface ISchedulingService
        : ISchedulerFor<WeeklyEvent>
    {
    }
}
