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
    internal class AutoService : Grpc.AutoService.AutoServiceBase
    {
        private readonly ILogger<AutoService> _logger;

        public AutoService(ILogger<AutoService> logger)
        {
            _logger = logger;
        }

        public override Task<Empty> DeleteAuto(AutoDTO autoDTO, ServerCallContext context)
        {
            Auto autoEntity = DtoConverter.ConvertToEntity(autoDTO);
            AutoManager autoManager = new AutoManager();

            try
            {
                autoManager.delete(autoEntity);
                return Task.FromResult(new Empty());
            } catch (OptimisticConcurrencyException<Auto> exception)
            {
                throw new ServiceException<Auto>(exception);
            }
            
        } 

        public override Task<Empty> InsertAuto(AutoDTO autoDTO, ServerCallContext context)
        {
            Auto autoEntity = DtoConverter.ConvertToEntity(autoDTO);
            AutoManager autoManager = new AutoManager();
            autoManager.insert(autoEntity);
            return Task.FromResult(new Empty());
        }

        public override Task<AutoDTOList> ReadAllAutos(Empty request, ServerCallContext context)
        {
            AutoManager autoManager = new AutoManager();
            Task<List<Auto>> allAuto = autoManager.GetAll();
            return DtoConverter.ConvertToDtos(allAuto);   
        }

        public override Task<AutoDTO> ReadAutoForId(AutoIdentifier autoDTOIdentifier, ServerCallContext context)
        {
            AutoManager autoManager = new AutoManager();
            Auto autoEntity = autoManager.GetForKey(autoDTOIdentifier.Id);
            return Task.FromResult(DtoConverter.ConvertToDto(autoEntity));
        }

        public override Task<Empty> UpdateAuto(AutoDTO autoDTO, ServerCallContext context)
        {
            Auto autoEntity = DtoConverter.ConvertToEntity(autoDTO);
            AutoManager autoManager = new AutoManager();
            try
            {
                autoManager.update(autoEntity);
            }
            catch (OptimisticConcurrencyException<Auto> exception)
            {
                throw new ServiceException<Auto>(exception);
            }

            return Task.FromResult(new Empty());
        }
    }
}
