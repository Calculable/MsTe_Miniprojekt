using System;

namespace AutoReservation.BusinessLayer.Exceptions
{
    public class InvalidDateRangeException 
        : Exception
    {
        public InvalidDateRangeException(string message, DateTime firstDate, DateTime secondDate) : base(message)
        {
            this.firstDate = firstDate;
            this.secondDate = secondDate;
              
        }

        public DateTime firstDate { get; set; }
        public DateTime secondDate { get; set; }

    }
}