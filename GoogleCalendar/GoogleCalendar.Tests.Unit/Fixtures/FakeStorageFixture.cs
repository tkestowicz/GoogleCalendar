using System.Collections.Generic;
using GoogleCalendar.Domain;
using NSubstitute;

namespace GoogleCalendar.Tests.Unit.Fixtures
{
    public class FakeStorageFixture
    {
        public FakeStorageFixture()
        {

            Storage = Substitute.For<IStorage>();

            var authors = new List<Author>();
            var events = new List<Event>();
            var eventSeries = new List<EventSerie>();

            Storage.Authors.Returns(authors);
            Storage.Events.Returns(events);
            Storage.EventSeries.Returns(eventSeries);

            Storage.When(a => a.Handle(Arg.Any<RepetableEventPreparedMessage>())).Do(a =>
            {
                var message = a.Arg<RepetableEventPreparedMessage>();

                Storage.Events.Add(message.Event);
                Storage.EventSeries.Add(message.EventSerie);
                Storage.Store();
            });
        }

        public IStorage Storage { get; private set; }
    }
}