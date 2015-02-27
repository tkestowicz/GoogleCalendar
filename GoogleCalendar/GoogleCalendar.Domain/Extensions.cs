using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCalendar.Domain
{
    public static class EventsMapper
    {
        public static Event ToEventDocument(this WeeklyEvent @event, string eventSerieId = null)
        {
            return new Event
            {
                AuthorId = @event.CreatedBy.Id,
                Id = Guid.NewGuid().ToString(),
                Name = @event.Name,
                Repeatable = @event.IsRepeable,
                Range = new OneTimeEventRange()
                {
                    Culture = @event.CreatedBy.Culture,
                    TimeZone = @event.CreatedBy.Timezone
                },
                IsFullDay = @event.IsFullDay,
                EventSerieId = eventSerieId
            };
        }

        public static EventSerie ToEventSerieDocument(this WeeklyEvent @event)
        {
            return new EventSerie()
            {
                Id = Guid.NewGuid().ToString(),
                Frequency = (int)@event.Frequency,
                Range = new EventSerieRange()
                {
                    Culture = @event.CreatedBy.Culture,
                    TimeZone = @event.CreatedBy.Timezone,
                    StartsAt = @event.From,
                    EndsAt = new EndsAt()
                    {
                        Never = @event.IsForever,
                        ParticularDate = @event.EndsAtParticularDate,
                        AfterTimes = @event.EndsAfterTimes
                    }
                },
                WeeklyParams = new WeeklyParams()
                {
                    Interval = @event.Interval,
                    Occurences = (int)@event.Occurance
                },
                Changes = new Changes()
                {
                    ParticularEvents = new List<Event>(),
                    SubSeries = new List<EventSerie>()
                }
            };
        }
    }

}
