using System;

namespace GoogleCalendar.Domain
{
    public class Scheduler : ISchedulingService
    {
        private readonly IMessageBus bus;


        public Scheduler(IMessageBus bus)
        {
            this.bus = bus;
        }

        public void Schedule(WeeklyEvent @event)
        {
            var preparedEventSerie = new EventSerie()
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
                        ParticularDate = @event.EndsAtParticularDate
                    }
                },
                WeeklyParams = new WeeklyParams()
                {
                    Interval = @event.Interval,
                    Occurences = (int) @event.Occurance
                }
            };

            var preparedEvent = new Event
            {
                AuthorId = @event.CreatedBy.Id,
                Id = Guid.NewGuid().ToString(),
                EventSerieId = preparedEventSerie.Id,
                Name = @event.Name,
                Repeatable = @event.IsRepeable,
                Range = new OneTimeEventRange()
                {
                    Culture = @event.CreatedBy.Culture,
                    TimeZone = @event.CreatedBy.Timezone
                },
                IsFullDay = @event.IsFullDay
            };

            bus.Publish(new RepetableEventPreparedMessage(preparedEventSerie, preparedEvent));
        }
    }
}