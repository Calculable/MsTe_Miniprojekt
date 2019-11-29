using System;

namespace AutoReservation.BusinessLayer.Exceptions
{
    public class ServiceException<T>
        : Exception
    {
        public ServiceException(OptimisticConcurrencyException<T> innerException) : base("Bei der Kommunikation mit der Datenbank ist ein Fehler aufgetreten. Bitte versuchen Sie es nochmals", innerException) { }
       
    }
}