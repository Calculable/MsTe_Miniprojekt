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
            }
            catch (OptimisticConcurrencyException<Auto>)
            {
                throw new RpcException(new Status(StatusCode.Aborted, "Auto could not be deleted because of a concurrency exception"));
            }
            catch (DbUpdateException)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "An exception occured while deleting Auto"));

            }
        } 

        public override Task<AutoIdentifier> InsertAuto(AutoDTO autoDTO, ServerCallContext context)
        {
            Auto autoEntity = DtoConverter.ConvertToEntity(autoDTO);
            AutoManager autoManager = new AutoManager();

            int newId;
            try
            {
                newId = autoManager.insert(autoEntity);
            } 
            catch (DbUpdateException)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "An exception occured while inserting Auto"));
            }

            AutoIdentifier newAutoIdentifier = new AutoIdentifier();
            newAutoIdentifier.Id = newId;

            return Task.FromResult(new AutoIdentifier(newAutoIdentifier));
        }

        public override Task<AutoDTOList> ReadAllAutos(Empty request, ServerCallContext context)
        {
            AutoManager autoManager = new AutoManager();
            Task<List<Auto>> allAuto;
             allAuto = autoManager.GetAll();
            return DtoConverter.ConvertToDtos(allAuto);   
        }

        public override Task<AutoDTO> ReadAutoForId(AutoIdentifier autoDTOIdentifier, ServerCallContext context)
        {
            AutoManager autoManager = new AutoManager();

            Auto autoEntity;
            try
            {
                autoEntity = autoManager.GetForKey(autoDTOIdentifier.Id);
            }
            catch (InvalidOperationException)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Auto with the given id could not be found"));
            }

            return Task.FromResult(DtoConverter.ConvertToDto(autoEntity));
        }

        public override Task<AutoIdentifier> UpdateAuto(AutoDTO autoDTO, ServerCallContext context)
        {
            Auto autoEntity = DtoConverter.ConvertToEntity(autoDTO);
            AutoManager autoManager = new AutoManager();
            int newId;
            try
            {
                newId= autoManager.update(autoEntity);
            }
            catch (OptimisticConcurrencyException<Auto>)
            {
                throw new RpcException(new Status(StatusCode.Aborted, "Auto could not be updated because of a concurrency exception"));
            }
            catch (DbUpdateException)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "An exception occured while updating Auto"));
            }

            AutoIdentifier newAutoIdentifier = new AutoIdentifier();
            newAutoIdentifier.Id = newId;

            return Task.FromResult(newAutoIdentifier);
        }

    }
}
