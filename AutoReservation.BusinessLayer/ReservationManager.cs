using System;
using System.Linq;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoReservation.BusinessLayer
{
    public class ReservationManager
        : ManagerBase
    {
        public async Task<List<Reservation>> GetAll()
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                return await context
                    .Reservationen
                    .Include(reservation => reservation.Kunde)
                    .Include(reservation => reservation.Auto)
                    .ToListAsync();
            }
        }

        public Reservation GetForKey(int reservationsNummer)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                return context.Reservationen
                    .Include(reservation => reservation.Kunde)
                    .Include(reservation => reservation.Auto)
                    .Single(reservation => reservation.ReservationsNr == reservationsNummer);
            }
        }

        public int insert(Reservation reservation)
        {
            assertDateRangeIsValid(reservation);
            assertCarIsAvaliable(reservation);

            using (AutoReservationContext autoReservationContext = new AutoReservationContext())
            {
                autoReservationContext.Reservationen.Add(reservation);

                autoReservationContext.SaveChanges();
                return reservation.ReservationsNr;
            }
        }

        public int update(Reservation reservation)
        {
            assertDateRangeIsValid(reservation);
            assertCarIsAvaliable(reservation);
            using (AutoReservationContext autoReservationContext = new AutoReservationContext())
            {
                autoReservationContext.Reservationen.Update(reservation);

                try
                {
                    autoReservationContext.SaveChanges();
                    return reservation.ReservationsNr;
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw CreateOptimisticConcurrencyException(autoReservationContext, reservation);
                }
            }
        }

        public void delete(Reservation reservation)
        {
            using (AutoReservationContext autoReservationContext = new AutoReservationContext())
            {
                autoReservationContext.Reservationen.Remove(reservation);
                try
                {
                    autoReservationContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw CreateOptimisticConcurrencyException(autoReservationContext, reservation);
                }
            }
        }

        private void assertDateRangeIsValid(Reservation reservation)
        {
            DateTime von = reservation.Von;
            DateTime bis = reservation.Bis;

            if (von >= bis)
            {
                throw new InvalidDateRangeException("Das von-Datum darf nicht grösser sein als das bis-Datum", von, bis);
            }

            if (von.AddDays(1) > bis)
            {
                throw new InvalidDateRangeException("Zwischen von-Datum uns bis-Datum müssen mindestens 24 Stunden liegen", von, bis);
            }
        }

        private void assertCarIsAvaliable(Reservation currentReservation)
        {
            if (!IsCarAvailable(currentReservation))
            {
                throw new AutoUnavailableException("Die Reservierung ist ungültig, da das reservierte Fahrzeug in diesem Zeitrahmen bereits reserviert ist.");
            }
        }

        public bool IsCarAvailable(Reservation currentReservation)
        {
            using (AutoReservationContext autoReservationContext = new AutoReservationContext())
            {
                int amountOfOverlappingReservations = autoReservationContext
                .Reservationen
                .Include(reservation => reservation.Auto)
                .Where(reservation => reservation.AutoId == currentReservation.AutoId)
                .Where(reservation => reservation.ReservationsNr != currentReservation.ReservationsNr)
                .Where(reservation => (reservation.Von > currentReservation.Von && reservation.Von < currentReservation.Bis) || (reservation.Bis > currentReservation.Von && reservation.Bis < currentReservation.Bis) || (reservation.Von == currentReservation.Von))
                .Count();

                if (amountOfOverlappingReservations > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}