using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoReservation.Dal.Entities
{
    public class Reservation
    {

        [Key, Column("ReservationsNr"), Required]
        public int ReservationsNr { get; set; }

        [Column("AutoId"), Required]
        public int AutoId { get; set; }

        [ForeignKey(nameof(AutoId))]
        public virtual Auto Auto { get; set; }

        [Column("KundeId"), Required]
        public int KundeId { get; set; }

        [ForeignKey(nameof(KundeId))]
        public virtual Kunde Kunde { get; set; }

        [Column("Bis"), Required]
        public DateTime Bis { get; set; }

        [Column("Von"), Required]
        public DateTime Von { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } 
    }

}