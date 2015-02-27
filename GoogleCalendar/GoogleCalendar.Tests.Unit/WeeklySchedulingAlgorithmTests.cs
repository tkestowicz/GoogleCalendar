using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleCalendar.Domain;
using GoogleCalendar.Tests.Unit.Fixtures;
using NSubstitute;
using Ploeh.AutoFixture;
using Should;
using Xunit;

namespace GoogleCalendar.Tests.Unit
{
    public class WeeklySchedulingAlgorithmTests : IUseFixture<FakeStorageFixture>
    {
        private readonly ISchedulingService scheduler;

        private readonly IFixture fixture = new Fixture();
        private IStorage storage;
        private readonly IMessageBus bus = Substitute.For<IMessageBus>();

        public WeeklySchedulingAlgorithmTests()
        {
            bus.When(a => a.Publish(Arg.Any<RepetableEventPreparedMessage>()))
               .Do(a => storage.Handle(a.Arg<RepetableEventPreparedMessage>()));

            scheduler = new Scheduler(bus);
        }

        [Fact]
        public void schedule_weekly_forever_event___every_week_starting_from_today___event_serie_is_created()
        {
            const int week = 1;
            var weeklyEvent = MakeWeeklyEvent();

            weeklyEvent.NeverEnds();
            weeklyEvent.From = DateTime.Today;
            weeklyEvent.RepeatEvery(week);

            scheduler.Schedule(weeklyEvent);

            var actualEvent = storage.Events.First(e => e.AuthorId == weeklyEvent.CreatedBy.Id);
            var actualSerie = storage.EventSeries.First(e => e.Id == actualEvent.EventSerieId);

            storage.Received().Handle(Arg.Any<RepetableEventPreparedMessage>());
            storage.Received().Store();
            
            actualEvent.IsFullDay.ShouldBeFalse();
            actualEvent.Repeatable.ShouldBeTrue();
            actualEvent.Range.TimeZone.ShouldEqual(weeklyEvent.CreatedBy.Timezone);
            actualEvent.Range.Culture.ShouldEqual(weeklyEvent.CreatedBy.Culture);

            actualSerie.Frequency.ShouldEqual((int) RepeatableEvent.EventFrequency.Weekly);
            actualSerie.Range.StartsAt.ShouldEqual(weeklyEvent.From);
            actualSerie.Range.EndsAt.Never.Value.ShouldBeTrue();
            actualSerie.Range.EndsAt.AfterTimes.ShouldBeNull();
            actualSerie.Range.EndsAt.ParticularDate.ShouldBeNull();
            actualSerie.Range.TimeZone.ShouldEqual(weeklyEvent.CreatedBy.Timezone);
            actualSerie.Range.Culture.ShouldEqual(weeklyEvent.CreatedBy.Culture);
            actualSerie.WeeklyParams.Interval.ShouldEqual(week);
            actualSerie.DailyParams.ShouldBeNull();
            actualSerie.MonthlyParams.ShouldBeNull();
            actualSerie.YearlyParams.ShouldBeNull();

            var currentDayOfTheWeek = Enum.GetName(typeof(DayOfWeek), DateTime.Today.DayOfWeek);

            var expectedDayOfTheWeek = (WeeklyEvent.Day) Enum.Parse(typeof (WeeklyEvent.Day), currentDayOfTheWeek, true);

            actualSerie.WeeklyParams.Occurences.ShouldEqual((int) expectedDayOfTheWeek);
        }

        private WeeklyEvent MakeWeeklyEvent()
        {
            return new WeeklyEvent()
            {
                CreatedBy = new Author(fixture.Create<string>()),
                Name = fixture.Create<string>()
            };
        }

        public void SetFixture(FakeStorageFixture data)
        {
            storage = data.Storage;
        }
    }
}
