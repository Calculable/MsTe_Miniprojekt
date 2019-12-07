using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
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
            catch (OptimisticConcurrencyException<Reservation>)
            {
                throw new RpcException(new Status(StatusCode.Aborted, "Reservation could not be deleted because of a concurrency exception"));
            }
            catch (DbUpdateException)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "An exception occured while deleting Reservation"));
            }

        }

        public override Task<ReservationIdentifier> InsertReservation(ReservationDTO reservationDTO, ServerCallContext context)
        {
            Reservation ReservationEntity = DtoConverter.ConvertToEntity(reservationDTO);
            ReservationManager ReservationManager = new ReservationManager();

            int newId;
            try
            {
                newId = ReservationManager.insert(ReservationEntity);
            }
            catch (InvalidDateRangeException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "The provided date-range for the reservation is not valid"));
            }
            catch (AutoUnavailableException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "The requested Auto is no available at the given date-range"));
            }
            catch (DbUpdateException)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "An exception occured while inserting Auto"));
            }

            ReservationIdentifier reservationIdentifier = new ReservationIdentifier();
            reservationIdentifier.ReservationsNr = newId;

            return Task.FromResult(reservationIdentifier);
        }

        public override Task<ReservationDTOList> ReadAllReservationen(Empty request, ServerCallContext context)
        {
            ReservationManager ReservationManager = new ReservationManager();

            Task<List<Reservation>>  allReservationen = ReservationManager.GetAll();
            
            return DtoConverter.ConvertToDtos(allReservationen);
        }

        public override Task<ReservationDTO> ReadReservationForId(ReservationIdentifier reservationDTOIdentifier, ServerCallContext context)
        {
            ReservationManager ReservationManager = new ReservationManager();
            Reservation reservationEntity;
            try
            {
                reservationEntity = ReservationManager.GetForKey(reservationDTOIdentifier.ReservationsNr);

            } catch (InvalidOperationException)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Reservation with the given id could not be found"));

            }

            return Task.FromResult(DtoConverter.ConvertToDto(reservationEntity));
        }

        public override Task<ReservationIdentifier> UpdateReservation(ReservationDTO reservationDTO, ServerCallContext context)
        {
            Reservation ReservationEntity = DtoConverter.ConvertToEntity(reservationDTO);
            ReservationManager ReservationManager = new ReservationManager();
            int insertedID;
            try
            {
                insertedID = ReservationManager.update(ReservationEntity);
            }
            catch (OptimisticConcurrencyException<Reservation>)
            {
                throw new RpcException(new Status(StatusCode.Aborted, "Reservation could not be updated because of a concurrency exception"));
            }
            catch (InvalidDateRangeException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "The provided date-range for the reservation is not valid"));
            }
            catch (AutoUnavailableException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "The requested Auto is no available at the given date-range"));
            }
            catch (DbUpdateException)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "An exception occured while updating Reservation"));
            }

            ReservationIdentifier reservationIdentifier = new ReservationIdentifier();
            reservationIdentifier.ReservationsNr = insertedID;

            return Task.FromResult(reservationIdentifier);
        }

        public override Task<CarAvailableResult> IsCarAvailable(ReservationDTO reservationDTO, ServerCallContext context)
        {
            Reservation ReservationEntity = DtoConverter.ConvertToEntity(reservationDTO);
            ReservationManager ReservationManager = new ReservationManager();

            CarAvailableResult carAvailableResult = new CarAvailableResult();
            carAvailableResult.CarAvailable = ReservationManager.IsCarAvailable(ReservationEntity); 
            return Task.FromResult(carAvailableResult);
        }
    }    
}
