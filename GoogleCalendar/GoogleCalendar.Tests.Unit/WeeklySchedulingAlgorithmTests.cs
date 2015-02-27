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
        private ISchedulingService scheduler;

        private readonly IFixture fixture = new Fixture();
        private IStorage storage;
        
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

            storage.Received().Store();
            
            actualEvent.IsFullDay.ShouldBeFalse();
            actualEvent.Repeatable.ShouldBeTrue();
            actualEvent.Range.TimeZone.ShouldEqual(weeklyEvent.CreatedBy.Timezone);
            actualEvent.Range.Culture.ShouldEqual(weeklyEvent.CreatedBy.Culture);

            actualSerie.Frequency.ShouldEqual((int) RepeatableEvent.EventFrequency.Weekly);
            actualSerie.Range.StartsAt.ShouldEqual(weeklyEvent.From);
            actualSerie.Range.EndsAt.Never.ShouldBeTrue();
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

        [Fact]
        public void schedule_weekend_fullday_event___which_ends_after_particular_date___event_serie_is_created()
        {
            const int week = 1;
            const WeeklyEvent.Day weekend = WeeklyEvent.Day.Saturday | WeeklyEvent.Day.Sunday;
            var weeklyEvent = MakeWeeklyEvent();

            weeklyEvent.EndsAt(DateTime.Today.AddDays(FakeData.NumberData.GetNumber(1, 50)));
            weeklyEvent.From = DateTime.Today;
            weeklyEvent.RepeatEvery(week);
            weeklyEvent.OccursAt(weekend);
            weeklyEvent.IsFullDay = true;

            scheduler.Schedule(weeklyEvent);

            var actualEvent = storage.Events.First(e => e.AuthorId == weeklyEvent.CreatedBy.Id);
            var actualSerie = storage.EventSeries.First(e => e.Id == actualEvent.EventSerieId);

            storage.Received().Store();

            actualEvent.IsFullDay.ShouldBeTrue();
            actualEvent.Repeatable.ShouldBeTrue();
            actualEvent.Range.TimeZone.ShouldEqual(weeklyEvent.CreatedBy.Timezone);
            actualEvent.Range.Culture.ShouldEqual(weeklyEvent.CreatedBy.Culture);

            actualSerie.Frequency.ShouldEqual((int)RepeatableEvent.EventFrequency.Weekly);
            actualSerie.Range.StartsAt.ShouldEqual(weeklyEvent.From);
            actualSerie.Range.EndsAt.Never.ShouldBeFalse();
            actualSerie.Range.EndsAt.AfterTimes.ShouldBeNull();
            actualSerie.Range.EndsAt.ParticularDate.Value.ShouldEqual(weeklyEvent.EndsAtParticularDate.Value);
            actualSerie.Range.TimeZone.ShouldEqual(weeklyEvent.CreatedBy.Timezone);
            actualSerie.Range.Culture.ShouldEqual(weeklyEvent.CreatedBy.Culture);
            actualSerie.WeeklyParams.Interval.ShouldEqual(week);
            actualSerie.DailyParams.ShouldBeNull();
            actualSerie.MonthlyParams.ShouldBeNull();
            actualSerie.YearlyParams.ShouldBeNull();

            actualSerie.WeeklyParams.Occurences.ShouldEqual((int)weekend);
        }

        [Fact]
        public void schedule_weekdays_event_every_n_weeks__which_ends_after_few_occurances__event_serie_is_created()
        {
            var weeksInterval = FakeData.NumberData.GetNumber(1, 10);
            const WeeklyEvent.Day weekdays = WeeklyEvent.Day.Monday | WeeklyEvent.Day.Tuesday
                | WeeklyEvent.Day.Wednesday | WeeklyEvent.Day.Thursday
                | WeeklyEvent.Day.Friday;
            var weeklyEvent = MakeWeeklyEvent();

            weeklyEvent.EndsAfter(FakeData.NumberData.GetNumber(1, 10));
            weeklyEvent.From = DateTime.Today;
            weeklyEvent.RepeatEvery(weeksInterval);
            weeklyEvent.OccursAt(weekdays);

            scheduler.Schedule(weeklyEvent);

            var actualEvent = storage.Events.First(e => e.AuthorId == weeklyEvent.CreatedBy.Id);
            var actualSerie = storage.EventSeries.First(e => e.Id == actualEvent.EventSerieId);

            storage.Received().Store();

            actualEvent.IsFullDay.ShouldBeFalse();
            actualEvent.Repeatable.ShouldBeTrue();
            actualEvent.Range.TimeZone.ShouldEqual(weeklyEvent.CreatedBy.Timezone);
            actualEvent.Range.Culture.ShouldEqual(weeklyEvent.CreatedBy.Culture);

            actualSerie.Frequency.ShouldEqual((int)RepeatableEvent.EventFrequency.Weekly);
            actualSerie.Range.StartsAt.ShouldEqual(weeklyEvent.From);
            actualSerie.Range.EndsAt.Never.ShouldBeFalse();
            actualSerie.Range.EndsAt.AfterTimes.ShouldEqual(weeklyEvent.EndsAfterTimes);
            actualSerie.Range.EndsAt.ParticularDate.ShouldBeNull();
            actualSerie.Range.TimeZone.ShouldEqual(weeklyEvent.CreatedBy.Timezone);
            actualSerie.Range.Culture.ShouldEqual(weeklyEvent.CreatedBy.Culture);
            actualSerie.WeeklyParams.Interval.ShouldEqual(weeksInterval);
            actualSerie.DailyParams.ShouldBeNull();
            actualSerie.MonthlyParams.ShouldBeNull();
            actualSerie.YearlyParams.ShouldBeNull();

            actualSerie.WeeklyParams.Occurences.ShouldEqual((int)weekdays);
        }

        [Fact]
        public void update_scheduled_event_serie__set_fullday_for_particular_event__changes_applied()
        {
            var weeklyEvent = MakeWeeklyEvent();

            weeklyEvent.IsFullDay = false;

            var serie = weeklyEvent.ToEventSerieDocument();
            var @event = weeklyEvent.ToEventDocument(serie.Id);

            storage.Events.Add(@event);
            storage.EventSeries.Add(serie);

            var eventId = storage.Events.First().Id;
            var updateStrategy = UpdateScheduledEvent<WeeklyEvent>.ApplyTo.ParticularEvent;

            weeklyEvent.IsFullDay = true;

            scheduler.UpdateScheduled(new UpdateScheduledEvent<WeeklyEvent>(updateStrategy, eventId, weeklyEvent));

            var updateEvent = storage
                .EventSeries.First()
                .Changes.ParticularEvents.First();

            updateEvent.IsFullDay.ShouldBeTrue();
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
            scheduler = new Scheduler(storage);
        }
    }
}
