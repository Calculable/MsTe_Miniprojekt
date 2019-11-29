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
                throw new ServiceException<Kunde>(exception);
            }
        }

        public override Task<Empty> InsertKunde(KundeDTO kundeDTO, ServerCallContext context)
        {
            Kunde KundeEntity = DtoConverter.ConvertToEntity(kundeDTO);
            KundeManager KundeManager = new KundeManager();
            KundeManager.insert(KundeEntity);
            return Task.FromResult(new Empty());
        }

        public override Task<KundeDTOList> ReadAllKunden(Empty request, ServerCallContext context)
        {
            KundeManager KundeManager = new KundeManager();
            Task<List<Kunde>> allKunden = KundeManager.GetAll();
            return DtoConverter.ConvertToDtos(allKunden);
        }

        public override Task<KundeDTO> ReadKundeForId(KundeIdentifier KundeDTOIdentifier, ServerCallContext context)
        {
            KundeManager KundeManager = new KundeManager();
            Kunde KundeEntity = KundeManager.GetForKey(KundeDTOIdentifier.Id);
            return Task.FromResult(DtoConverter.ConvertToDto(KundeEntity));
        }

        public override Task<Empty> UpdateKunde(KundeDTO kundeDTO, ServerCallContext context)
        {
            Kunde KundeEntity = DtoConverter.ConvertToEntity(kundeDTO);
            KundeManager KundeManager = new KundeManager();
            try
            {
                KundeManager.update(KundeEntity);
            }
            catch (OptimisticConcurrencyException<Kunde> exception)
            {
                throw new ServiceException<Kunde>(exception);
            }

            return Task.FromResult(new Empty());
        }

    }
}
