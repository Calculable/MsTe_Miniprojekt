using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal;
using System;

namespace AutoReservation.BusinessLayer
{
    public abstract class ManagerBase
    {
        protected static OptimisticConcurrencyException<T> CreateOptimisticConcurrencyException<T>(AutoReservationContext context, T entity)
            where T : class
        {
            T dbEntity;

            try
            {
                dbEntity = (T)context.Entry(entity)
                .GetDatabaseValues()
                .ToObject();
            } catch (NullReferenceException)
            {
                //we can not load the existing dbEntry from the database because it does not exist
                //this occurs if the client tries to delete or update an entity that is not stored in the database
                dbEntity = null;
            }

            return new OptimisticConcurrencyException<T>($"Update {typeof(T).Name}: Concurrency-Fehler", dbEntity);
        }
    }
}