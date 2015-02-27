using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCalendar.Domain
{
    public class UpdateScheduledEvent<TEvent>
        where TEvent : CalendarEvent
    {
        public UpdateScheduledEvent(ApplyTo updateStrategy, string eventId, TEvent changes)
        {
            Changes = changes;
            EventId = eventId;
            UpdateStrategy = updateStrategy;
        }

        public enum ApplyTo
        {
            ParticularEvent,
            FutureEvents,
            AllEvents
        }

        public ApplyTo UpdateStrategy { get; private set; }

        public string EventId { get; private set; }

        public TEvent Changes { get; private set; }
    }
}
