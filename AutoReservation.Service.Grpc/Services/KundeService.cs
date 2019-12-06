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
    internal class KundeService : Grpc.KundeService.KundeServiceBase
    {
        private readonly ILogger<KundeService> _logger;

        public KundeService(ILogger<KundeService> logger)
        {
            _logger = logger;
        }

        public override Task<Empty> DeleteKunde(KundeDTO kundeDTO, ServerCallContext context)
        {
            Kunde kundeEntity = DtoConverter.ConvertToEntity(kundeDTO);
            KundeManager kundeManager = new KundeManager();

            try
            {
                kundeManager.delete(kundeEntity);
                return Task.FromResult(new Empty());
            }
            catch (OptimisticConcurrencyException<Kunde> exception)
            {
                throw new RpcException(new Status(StatusCode.Aborted, "Kunde could not be deleted because of a concurrency exception"));
            }
            catch (DbUpdateException)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "An exception occured while deleting Kunde"));
            }
        }

        public override Task<KundeIdentifier> InsertKunde(KundeDTO kundeDTO, ServerCallContext context)
        {
            Kunde KundeEntity = DtoConverter.ConvertToEntity(kundeDTO);
            KundeManager KundeManager = new KundeManager();

            int newKundeId;
            try
            {
                 newKundeId = KundeManager.insert(KundeEntity);
            }
            catch (DbUpdateException)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "An exception occured while inserting Kunde"));
            }


            KundeIdentifier newKundeIdentifier = new KundeIdentifier();
            newKundeIdentifier.Id = newKundeId;

            return Task.FromResult(newKundeIdentifier);
        }

        public override Task<KundeDTOList> ReadAllKunden(Empty request, ServerCallContext context)
        {
            KundeManager KundeManager = new KundeManager();

            Task<List<Kunde>> allKunden;

            allKunden = KundeManager.GetAll();

            
            return DtoConverter.ConvertToDtos(allKunden);
        }

        public override Task<KundeDTO> ReadKundeForId(KundeIdentifier KundeDTOIdentifier, ServerCallContext context)
        {
            KundeManager KundeManager = new KundeManager();

            Kunde KundeEntity;

            try
            {
                KundeEntity = KundeManager.GetForKey(KundeDTOIdentifier.Id);
            }
            catch (InvalidOperationException)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Kunde with the given id could not be found"));
            }

            return Task.FromResult(DtoConverter.ConvertToDto(KundeEntity));
        }

        public override Task<KundeIdentifier> UpdateKunde(KundeDTO kundeDTO, ServerCallContext context)
        {
            Kunde KundeEntity = DtoConverter.ConvertToEntity(kundeDTO);
            KundeManager KundeManager = new KundeManager();

            int newKundeId;
            try
            {
                newKundeId = KundeManager.update(KundeEntity);
            }
            catch (OptimisticConcurrencyException<Kunde> exception)
            {
                throw new ServiceException(exception);
            }
            catch (DbUpdateException exception)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "An exception occured while updating Kunde"));
            }

            KundeIdentifier newKundeIdentifier = new KundeIdentifier();
            newKundeIdentifier.Id = newKundeId;

            return Task.FromResult(newKundeIdentifier);
        }

    }
}
