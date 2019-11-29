using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoReservation.Dal.Entities;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace AutoReservation.Service.Grpc
{
    internal static class DtoConverter
    {
        #region Auto
        private static Auto GetAutoInstance(AutoDTO dto)
        {
            if (dto.Klasse == AutoKlasse.Standard) { return new StandardAuto(); }
            if (dto.Klasse == AutoKlasse.Mittelklasse) { return new MittelklasseAuto(); }
            if (dto.Klasse == AutoKlasse.Luxusklasse) { return new LuxusklasseAuto(); }
            throw new ArgumentException("Unknown AutoDto implementation.", nameof(dto));
        }
        public static Auto ConvertToEntity(this AutoDTO dto)
        {
            if (dto == null) { return null; }

            Auto auto = GetAutoInstance(dto);
            auto.Id = dto.Id;
            auto.Marke = dto.Marke;
            auto.Tagestarif = dto.Tagestarif;
            auto.RowVersion = dto.RowVersion.Length == 0
                ? null
                : dto.RowVersion.ToByteArray();

            if (auto is LuxusklasseAuto luxusklasseAuto)
            {
                luxusklasseAuto.Basistarif = dto.Basistarif;
            }
            return auto;
        }
        public static async Task<AutoDTO> ConvertToDto(this Task<Auto> entityTask) => (await entityTask).ConvertToDto();
        public static AutoDTO ConvertToDto(this Auto entity)
        {
            if (entity == null) { return null; }

            AutoDTO dto = new AutoDTO
            {
                Id = entity.Id,
                Marke = entity.Marke,
                Tagestarif = entity.Tagestarif,
                RowVersion = ByteString.CopyFrom(entity.RowVersion ?? new byte[0]),
            };

            if (entity is StandardAuto) { dto.Klasse = AutoKlasse.Standard; }
            if (entity is MittelklasseAuto) { dto.Klasse = AutoKlasse.Mittelklasse; }
            if (entity is LuxusklasseAuto auto)
            {
                dto.Klasse = AutoKlasse.Luxusklasse;
                dto.Basistarif = auto.Basistarif;
            }

            return dto;
        }
        public static List<Auto> ConvertToEntities(this IEnumerable<AutoDTO> dtos)
        {
            return ConvertGenericList(dtos, ConvertToEntity);
        }
        public static async Task<List<AutoDTO>> ConvertToDtos(this Task<List<Auto>> entitiesTask) => (await entitiesTask).ConvertToDtos();
        public static List<AutoDTO> ConvertToDtos(this IEnumerable<Auto> entities)
        {
            return ConvertGenericList(entities, ConvertToDto);
        }
        #endregion
        #region Kunde
        public static Kunde ConvertToEntity(this KundeDTO dto)
        {
            if (dto == null) { return null; }

            return new Kunde
            {
                Id = dto.Id,
                Name = dto.Nachname,
                Vorname = dto.Vorname,
                Geburtsdatum = dto.Geburtsdatum.ToDateTime(),
                RowVersion = dto.RowVersion.Length == 0
                    ? null
                    : dto.RowVersion.ToByteArray()
            };
        }
        public static async Task<KundeDTO> ConvertToDto(this Task<Kunde> entityTask) => (await entityTask).ConvertToDto();
        public static KundeDTO ConvertToDto(this Kunde entity)
        {
            if (entity == null) { return null; }

            return new KundeDTO
            {
                Id = entity.Id,
                Nachname = entity.Name,
                Vorname = entity.Vorname,
                Geburtsdatum = entity.Geburtsdatum.HasValue?entity.Geburtsdatum.Value.ToTimestampUtcFaked():null,
                RowVersion = ByteString.CopyFrom(entity.RowVersion ?? new byte[0]),
            };
        }
        public static List<Kunde> ConvertToEntities(this IEnumerable<KundeDTO> dtos)
        {
            return ConvertGenericList(dtos, ConvertToEntity);
        }
        public static async Task<List<KundeDTO>> ConvertToDtos(this Task<List<Kunde>> entitiesTask) => (await entitiesTask).ConvertToDtos();
        public static List<KundeDTO> ConvertToDtos(this IEnumerable<Kunde> entities)
        {
            return ConvertGenericList(entities, ConvertToDto);
        }
        #endregion
        #region Reservation
        public static Reservation ConvertToEntity(this ReservationDTO dto)
        {
            if (dto == null) { return null; }

            Reservation reservation = new Reservation
            {
                ReservationsNr = dto.ReservationsNr,
                Von = dto.Von.ToDateTime(),
                Bis = dto.Bis.ToDateTime(),
                AutoId = dto.Auto.Id,
                KundeId = dto.Kunde.Id,
                RowVersion = dto.RowVersion.Length == 0
                    ? null
                    : dto.RowVersion.ToByteArray()
            };

            return reservation;
        }
        public static async Task<ReservationDTO> ConvertToDto(this Task<Reservation> entityTask) => (await entityTask).ConvertToDto();
        public static ReservationDTO ConvertToDto(this Reservation entity)
        {
            if (entity == null) { return null; }

            return new ReservationDTO
            {
                ReservationsNr = entity.ReservationsNr,
                Von = entity.Von.ToTimestampUtcFaked(),
                Bis = entity.Bis.ToTimestampUtcFaked(),
                RowVersion = ByteString.CopyFrom(entity.RowVersion ?? new byte[0]),
                Auto = ConvertToDto(entity.Auto),
                Kunde = ConvertToDto(entity.Kunde)
            };
        }
        public static List<Reservation> ConvertToEntities(this IEnumerable<ReservationDTO> dtos)
        {
            return ConvertGenericList(dtos, ConvertToEntity);
        }
        public static async Task<List<ReservationDTO>> ConvertToDtos(this Task<List<Reservation>> entitiesTask) => (await entitiesTask).ConvertToDtos();
        public static List<ReservationDTO> ConvertToDtos(this IEnumerable<Reservation> entities)
        {
            return ConvertGenericList(entities, ConvertToDto);
        }
        #endregion

        private static List<TTarget> ConvertGenericList<TSource, TTarget>(this IEnumerable<TSource> source, Func<TSource, TTarget> converter)
        {
            if (source == null) { return null; }
            if (converter == null) { return null; }

            return source.Select(converter).ToList();
        }
        /// <summary>
        /// Don't try this at home!
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Timestamp ToTimestampUtcFaked(this DateTime source)
            => new DateTime(source.Ticks, DateTimeKind.Utc).ToTimestamp();
    }
}
