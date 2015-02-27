using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCalendar.Domain
{
    public interface IMessage { }

    public class RepetableEventPreparedMessage : IMessage
    {
        public Event Event { get; private set; }
        public EventSerie EventSerie { get; private set; }

        public RepetableEventPreparedMessage(EventSerie eventSerie, Event @event)
        {
            Event = @event;
            EventSerie = eventSerie;
        }
    }
}
