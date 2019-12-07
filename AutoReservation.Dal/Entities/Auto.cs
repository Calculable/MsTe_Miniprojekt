using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoReservation.Dal.Entities
{
    public abstract class Auto
    {
        [Key, Column("Id")]
        public int Id { get; set; }


        [MaxLength(20), Column("Marke"), Required]
        public string Marke { get; set; }


        [Column("Tagestarif"), Required]
        public int Tagestarif { get; set; }

        public ICollection<Reservation> Reservationen { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

    public class StandardAuto: Auto
    {
    }

    public class LuxusklasseAuto : Auto
    {
        [Column("Basistarif"), Required] //Achtung : Im Datenmodell als nicht Required angegeben - das liegt daran, dass in der gleichen Tabelle auch nicht-Luxuswagen gespeichert werden
        public int Basistarif { get; set; }
    }

    public class MittelklasseAuto : Auto
    {
    }
}
