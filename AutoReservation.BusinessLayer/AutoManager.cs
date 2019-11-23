using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoReservation.BusinessLayer
{
    public class AutoManager
        : ManagerBase
    {
        public async Task<List<Auto>> GetAll()
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                return await context.Autos.ToListAsync();
            }
        }

        public Auto GetForKey(int autoKey)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                return context.Autos.Single(auto => auto.Id == autoKey);
            }
        }

        public int insert(Auto auto)
        {
            using (AutoReservationContext autoReservationContext = new AutoReservationContext())
            {
                autoReservationContext.Entry(auto).State = EntityState.Added;
                autoReservationContext.SaveChanges();
                return auto.Id;
            }
        }

        public int update(Auto auto)
        {
            using (AutoReservationContext autoReservationContext = new AutoReservationContext())
            {
                autoReservationContext.Entry(auto).State = EntityState.Modified;

                try
                {
                    autoReservationContext.SaveChanges();
                    return auto.Id;
                } catch (DbUpdateConcurrencyException concurrencyException)
                {
                    var originalValues = concurrencyException.Entries.First().OriginalValues;
                    throw new OptimisticConcurrencyException<PropertyValues>("Can not update because of a concurrency exception. Maybe the the object was updated in the meantime?", originalValues);
                }
            }
        }

        public void delete(Auto auto)
        {
            using (AutoReservationContext autoReservationContext = new AutoReservationContext())
            {
                autoReservationContext.Entry(auto).State = EntityState.Deleted;

                try
                {
                    autoReservationContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException concurrencyException)
                {
                    var originalValues = concurrencyException.Entries.First().OriginalValues;
                    throw new OptimisticConcurrencyException<PropertyValues>("Can not delete because of a concurrency exception. Maybe the the object was updated in the meantime?", originalValues);
                }
            }
        }

    }
}