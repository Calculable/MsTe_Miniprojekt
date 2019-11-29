using System.Collections.Generic;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace AutoReservation.Service.Grpc.Services
{
    internal class ReservationService : Grpc.ReservationService.ReservationServiceBase
    {
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(ILogger<ReservationService> logger)
        {
            _logger = logger;
        }

        public override Task<Empty> DeleteReservation(ReservationDTO reservationDTO, ServerCallContext context)
        {
            Reservation ReservationEntity = DtoConverter.ConvertToEntity(reservationDTO);
            ReservationManager reservationManager = new ReservationManager();

            try
            {
                reservationManager.delete(ReservationEntity);
                return Task.FromResult(new Empty());
            }
            catch (OptimisticConcurrencyException<Reservation> exception)
            {
                throw new ServiceException<Reservation>(exception);
            }
            catch (InvalidDateRangeException exception)
            {
                throw new ServiceException<Reservation>(exception);
            }
            catch (AutoUnavailableException exception)
            {
                throw new ServiceException<Reservation>(exception);
            }

        }

        public override Task<Empty> InsertReservation(ReservationDTO reservationDTO, ServerCallContext context)
        {
            Reservation ReservationEntity = DtoConverter.ConvertToEntity(reservationDTO);
            ReservationManager ReservationManager = new ReservationManager();
            ReservationManager.insert(ReservationEntity);
            return Task.FromResult(new Empty());
        }

        public override Task<ReservationDTOList> ReadAllReservationen(Empty request, ServerCallContext context)
        {
            ReservationManager ReservationManager = new ReservationManager();
            Task<List<Reservation>> allReservationen = ReservationManager.GetAll();
            return DtoConverter.ConvertToDtos(allReservationen);
        }

        public override Task<ReservationDTO> ReadReservationForId(ReservationIdentifier reservationDTOIdentifier, ServerCallContext context)
        {
            ReservationManager ReservationManager = new ReservationManager();
            Reservation ReservationEntity = ReservationManager.GetForKey(reservationDTOIdentifier.ReservationsNr);
            return Task.FromResult(DtoConverter.ConvertToDto(ReservationEntity));
        }

        public override Task<Empty> UpdateReservation(ReservationDTO reservationDTO, ServerCallContext context)
        {
            Reservation ReservationEntity = DtoConverter.ConvertToEntity(reservationDTO);
            ReservationManager ReservationManager = new ReservationManager();
            try
            {
                ReservationManager.update(ReservationEntity);
            }
            catch (OptimisticConcurrencyException<Reservation> exception)
            {
                throw new ServiceException<Reservation>(exception);
            }
            catch (InvalidDateRangeException exception)
            {
                throw new ServiceException<Reservation>(exception);
            }
            catch (AutoUnavailableException exception)
            {
                throw new ServiceException<Reservation>(exception);
            }

            return Task.FromResult(new Empty());
        }
    }    
}
