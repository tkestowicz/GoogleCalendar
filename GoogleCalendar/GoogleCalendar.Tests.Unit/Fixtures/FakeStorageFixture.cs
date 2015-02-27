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
        }

        public IStorage Storage { get; private set; }
    }
}