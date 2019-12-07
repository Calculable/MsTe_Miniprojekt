using System;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal.Entities;
using AutoReservation.TestEnvironment;
using Xunit;

namespace AutoReservation.BusinessLayer.Testing
{
    public class ReservationDateRangeTest
        : TestBase
    {
        private readonly ReservationManager _target;

        public ReservationDateRangeTest()
        {
            _target = new ReservationManager();
        }

        [Fact]
        public void ScenarioOkay01Exactly24HoursTest()
        {
            Reservation firstReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 04));
            _target.insert(firstReservation);
        }

        [Fact]
        public void ScenarioOkay02VeryLongreservationTest()
        {
            Reservation firstReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2021, 08, 07));
            _target.insert(firstReservation);
        }

        [Fact]
        public void ScenarioNotOkay01LessThan24HoursTest()
        {
            DateTime dateTime = new DateTime(2020, 03, 02);

            Reservation firstReservation = createNewExampleReservation(dateTime, dateTime.AddHours(22));
            var ex = Assert.Throws<InvalidDateRangeException>(() => _target.insert(firstReservation));
        }

        [Fact]
        public void ScenarioNotOkay02VonIsLargerThanBisTest()
        {
            DateTime dateTime = new DateTime(2020, 03, 02);

            Reservation firstReservation = createNewExampleReservation(dateTime.AddDays(2), dateTime);
            var ex = Assert.Throws<InvalidDateRangeException>(() => _target.insert(firstReservation));
        }

        [Fact]
        public void ScenarioNotOkay03VonAndBisAreEqualTest()
        {
            DateTime dateTime = new DateTime(2020, 03, 02);
            Reservation firstReservation = createNewExampleReservation(dateTime, dateTime);
            var ex = Assert.Throws<InvalidDateRangeException>(() => _target.insert(firstReservation));
        }

        private Reservation createNewExampleReservation(DateTime von, DateTime bis)
        {
            Kunde kunde = new Kunde { Name = "Müller", Vorname = "Judith", Geburtsdatum = new DateTime(1980, 02, 13) };
            return new Reservation { AutoId = 1, Kunde = kunde, Von = von, Bis = bis };
        }
    }
}
