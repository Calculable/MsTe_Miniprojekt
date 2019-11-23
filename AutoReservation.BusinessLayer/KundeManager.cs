﻿using System;
using System.Linq;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AutoReservation.BusinessLayer
{
    public class KundeManager
        : ManagerBase
    {
        public async Task<List<Kunde>> GetAll()
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                return await context.Kunden.ToListAsync();
            }
        }

        public Kunde GetForKey(int kundeKey)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                return context.Kunden.Single(kunde => kunde.Id == kundeKey);
            }
        }

        public int insert(Kunde kunde)
        {
            using (AutoReservationContext autoReservationContext = new AutoReservationContext())
            {
                autoReservationContext.Entry(kunde).State = EntityState.Added;
                autoReservationContext.SaveChanges();
                return kunde.Id;
            }
        }

        public int update(Kunde kunde)
        {
            using (AutoReservationContext autoReservationContext = new AutoReservationContext())
            {
                autoReservationContext.Entry(kunde).State = EntityState.Modified;

                try
                {
                    autoReservationContext.SaveChanges();
                    return kunde.Id;
                }
                catch (DbUpdateConcurrencyException concurrencyException)
                {
                    var originalValues = concurrencyException.Entries.First().OriginalValues;
                    throw new OptimisticConcurrencyException<PropertyValues>("Can not update customer because of a concurrency exception. Maybe the the customer was updated in the meantime?", originalValues);
                }
            }
        }

        public void delete(Kunde kunde)
        {
            using (AutoReservationContext autoReservationContext = new AutoReservationContext())
            {
                autoReservationContext.Entry(kunde).State = EntityState.Deleted;

                try
                {
                    autoReservationContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException concurrencyException)
                {
                    var originalValues = concurrencyException.Entries.First().OriginalValues;
                    throw new OptimisticConcurrencyException<PropertyValues>("Can not delete customer because of a concurrency exception. Maybe the the customer was updated in the meantime?", originalValues);
                }
            }
        }
    }
}