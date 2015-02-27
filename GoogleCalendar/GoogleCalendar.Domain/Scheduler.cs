using System;
using System.Linq;

namespace GoogleCalendar.Domain
{
    public class Scheduler : ISchedulingService
    {
        private readonly IStorage storage;
        
        public Scheduler(IStorage storage)
        {
            this.storage = storage;
        }

        public void Schedule(WeeklyEvent @event)
        {
            var preparedEventSerie = @event.ToEventSerieDocument();
            var preparedEvent = @event.ToEventDocument(preparedEventSerie.Id);

            storage.Events.Add(preparedEvent);
            storage.EventSeries.Add(preparedEventSerie);
            storage.Store();
        }

        public void UpdateScheduled(UpdateScheduledEvent<WeeklyEvent> update)
        {
            var @event = storage
                .Events
                .FirstOrDefault(e => e.Id == update.EventId && string.IsNullOrEmpty(e.EventSerieId) == false);

            if(@event == null)
                throw new ArgumentException("Series event with given id does not exist.");

            var eventSerie = storage.EventSeries.First(e => e.Id == @event.EventSerieId);

            switch (update.UpdateStrategy)
            {
                case UpdateScheduledEvent<WeeklyEvent>.ApplyTo.ParticularEvent:

                    UpdateParticularEvent(eventSerie, update.Changes);

                    break;
            }

            storage.Store();
        }

        private void UpdateParticularEvent(EventSerie serie, WeeklyEvent changes)
        {
            var @event = changes.ToEventDocument();

            serie.Changes.ParticularEvents.Add(@event);
        }
    }
}