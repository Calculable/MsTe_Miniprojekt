using System;

namespace AutoReservation.BusinessLayer.Exceptions
{
    public class AutoUnavailableException
        : Exception
    {
        public AutoUnavailableException(string message) : base(message) { }
    }
}