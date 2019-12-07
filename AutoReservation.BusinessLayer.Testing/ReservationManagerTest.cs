using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal.Entities;
using AutoReservation.TestEnvironment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;

namespace AutoReservation.BusinessLayer.Testing
{
    public class ReservationManagerTest
        : TestBase
    {
        private readonly ReservationManager _target;

        public ReservationManagerTest()
        {
            _target = new ReservationManager();
        }

        [Fact]
        public async Task GetAllReservationenTest()
        {
            List<Reservation> reservations = await _target.GetAll();
            Assert.Equal(4, reservations.Count);
        }

        [Fact]
        public async Task InsertReservationTest()
        {
            Reservation reservation = createNewExampleReservation();

            _target.insert(reservation);

            List<Reservation> reservations = await _target.GetAll();
            Assert.Equal(5, reservations.Count);
        }

        [Fact]
        public void GetReservationForKeyTest()
        {
            Reservation reservationToInsert = createNewExampleReservation();
            int newId = _target.insert(reservationToInsert);

            Reservation inserted = _target.GetForKey(newId);
            Assert.Equal(reservationToInsert.Bis, inserted.Bis);
            Assert.Equal(reservationToInsert.Von, inserted.Von);

            //check that kunde und auto are loaded too
            Assert.Equal(reservationToInsert.Kunde.Name, inserted.Kunde.Name);
            Assert.Equal(reservationToInsert.Auto.Marke, inserted.Auto.Marke);
        }

        [Fact]
        public void GetReservationForNonExistingKeyThrowsExceptionTest()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _target.GetForKey(999));
            Assert.Equal("Sequence contains no elements", ex.Message);
        }

        [Fact]
        public void InsertAlreadyExistingReservationThrowsExceptionTest()
        {
            Reservation toInsert = createNewExampleReservation();
            int newId = _target.insert(toInsert);

            Reservation inserted = _target.GetForKey(newId);

            Assert.Throws<DbUpdateException>(() => _target.insert(inserted));
        }

        [Fact]
        public async Task DeleteReservationTest()
        {
            Reservation reservation = createNewExampleReservation();
            int newId = _target.insert(reservation);

            List<Reservation> reservationen = await _target.GetAll();
            Assert.Equal(5, reservationen.Count);

            Reservation inserted = _target.GetForKey(newId);
            _target.delete(inserted);

            reservationen = await _target.GetAll();
            Assert.Equal(4, reservationen.Count);
        }

        [Fact]
        public void DeleteNotInsertedReservationThrowsExceptionTest()
        {
            Reservation reservation = createNewExampleReservation();

            Assert.Throws<InvalidOperationException>(() => _target.delete(reservation));
        }

        [Fact]
        public void DeleteUpdatedReservationThrowsExceptionTest()
        {
            Reservation reservation = createNewExampleReservation();
            int newId = _target.insert(reservation);

            Reservation inserted1 = _target.GetForKey(newId);
            Reservation inserted2 = _target.GetForKey(newId);
            inserted2.Bis = new DateTime(2021, 03, 10);
            _target.update(inserted2);

            var ex = Assert.Throws<OptimisticConcurrencyException<Reservation>>(() => _target.delete(inserted1));
        }

        [Fact]
        public void UpdateReservationTest()
        {
            Reservation reservation = createNewExampleReservation();
            int newId = _target.insert(reservation);

            reservation.Bis = new DateTime(2021, 03, 10);
            _target.update(reservation);

            Reservation inserted = _target.GetForKey(newId);

            Assert.Equal(new DateTime(2021, 03, 10), inserted.Bis);
        }

        [Fact]
        public void UpdateKundeViaReservationTest()
        {
            Reservation reservation = createNewExampleReservation();
            int newId = _target.insert(reservation);

            reservation.Kunde.Name = "changed";
            _target.update(reservation);

            Reservation inserted = _target.GetForKey(newId);

            Assert.Equal("changed", inserted.Kunde.Name);
        }

        [Fact]
        public void UpdateChangedReservationThrowsExceptionTest()
        {
            Reservation reservation = createNewExampleReservation();
            int newId = _target.insert(reservation);

            Reservation inserted1 = _target.GetForKey(newId);
            Reservation inserted2 = _target.GetForKey(newId);

            reservation.Bis = new DateTime(2021, 03, 10);
            _target.update(inserted1);

            reservation.Bis = new DateTime(2021, 04, 10);

            var ex = Assert.Throws<OptimisticConcurrencyException<Reservation>>(() => _target.update(inserted2));
        }

        private Reservation createNewExampleReservation()
        {
            Auto auto = new LuxusklasseAuto { Marke = "Example Marke", Tagestarif = 100, Basistarif = 200 };
            Kunde kunde = new Kunde { Name = "Müller", Vorname = "Judith", Geburtsdatum = new DateTime(1980, 02, 13) };
            return new Reservation { Auto = auto, Kunde = kunde, Von = new DateTime(2020, 01, 10), Bis = new DateTime(2020, 01, 20) };
        }
    }
}
