﻿using System;

namespace AutoReservation.BusinessLayer.Exceptions
{
    public class OptimisticConcurrencyException<T> 
        : Exception
    {
        public OptimisticConcurrencyException(string message) : base(message) { }
        public OptimisticConcurrencyException(string message, T mergedEntity) : base(message)
        {
            MergedEntity = mergedEntity;
        }
        public T MergedEntity { get; set; }
    }
}