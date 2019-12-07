using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoReservation.Dal.Entities
{
    public class Kunde
    {
        [Key, Column("Id")]
        public int Id { get; set; }

        [Column(TypeName = "DATETIME"), Required]
        public DateTime? Geburtsdatum { get; set; }

        [Column("Nachname"), MaxLength(20), Required]
        public string Name { get; set; }

        [Column("Vorname"), MaxLength(20), Required]
        public string Vorname { get; set; }

        public ICollection<Reservation> Reservationen { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


    }
}
