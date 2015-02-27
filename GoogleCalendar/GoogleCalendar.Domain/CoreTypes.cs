using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCalendar.Domain
{
    public interface IHandler<in TMessage>
        where TMessage : IMessage
    {
        void Handle(TMessage message);
    }

    public interface IMessageBus
    {
        void Publish<TMessage>(TMessage message)
            where TMessage : IMessage;
    }

    public interface IStorage
        : IHandler<RepetableEventPreparedMessage>
    {
        ICollection<Author> Authors { get; }

        ICollection<Event> Events { get; }

        ICollection<EventSerie> EventSeries { get; }

        void Store();
    }

    public interface ISchedulerFor<in TEvent>
        where TEvent : CalendarEvent
    {
        void Schedule(TEvent @event);
    }

    public interface ISchedulingService
        : ISchedulerFor<WeeklyEvent>
    {
    }
}
