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
                } catch (DbUpdateConcurrencyException)
                {
                    throw CreateOptimisticConcurrencyException(autoReservationContext, auto);
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
                catch (DbUpdateConcurrencyException)
                {
                    throw CreateOptimisticConcurrencyException(autoReservationContext, auto);
                }
            }
        }

      
    }
}