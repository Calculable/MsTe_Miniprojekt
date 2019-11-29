using System;

namespace AutoReservation.BusinessLayer.Exceptions
{
    public class ServiceException<T>
        : Exception
    {
        public ServiceException(OptimisticConcurrencyException<T> innerException) : base("Bei der Kommunikation mit der Datenbank ist ein Fehler aufgetreten. Bitte versuchen Sie es nochmals", innerException) { }
        public ServiceException(InvalidDateRangeException innerException) : base("Das ausgewählte Startdatum oder Enddatum der Reservation sind ungültig", innerException) { }
        public ServiceException(AutoUnavailableException innerException) : base("Das Fahrzeug ist zum gewählten Zeitpunkt nicht verfügbar", innerException) { }

    }
}