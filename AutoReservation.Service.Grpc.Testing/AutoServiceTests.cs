using System;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal.Entities;
using AutoReservation.Service.Grpc.Testing.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Xunit;

namespace AutoReservation.Service.Grpc.Testing
{
    public class AutoServiceTests
        : ServiceTestBase
    {
        private readonly AutoService.AutoServiceClient _target;

        public AutoServiceTests(ServiceTestFixture serviceTestFixture)
            : base(serviceTestFixture)
        {
            _target = new AutoService.AutoServiceClient(Channel);
        }


        [Fact]
        public async Task GetAutosTest()
        {
            AutoDTOList result = _target.ReadAllAutos(new Empty());
            Assert.Equal(4, result.Autos.Count);

        }

        [Fact]
        public async Task GetAutoByIdTest()
        {
            AutoDTO newAuto = generateExampleAuto();
            AutoIdentifier insertedID = _target.InsertAuto(newAuto);

            AutoDTO result = _target.ReadAutoForId(insertedID);
            Assert.Equal(newAuto.Klasse, result.Klasse);
            Assert.Equal(newAuto.Marke, result.Marke);

            Assert.Equal(newAuto.Tagestarif, result.Tagestarif);
            Assert.Equal(newAuto.Basistarif, result.Basistarif);

        }

        [Fact]
        public void GetAutoByIdWithIllegalIdTest()
        {
            /* int illegalId = 999;

             AutoIdentifier autoIdentifier = new AutoIdentifier();
             autoIdentifier.Id = illegalId;


             var ex = Assert.Throws<ServiceException>(() => _target.ReadAutoForId(autoIdentifier));
             Assert.True(ex.InnerException is InvalidOperationException);*/

            throw new NotImplementedException("Test not implemented.");

        }

        [Fact]
        public async Task InsertAutoTest()
        {
            AutoDTO newAuto = generateExampleAuto();
            _target.InsertAuto(newAuto);

            AutoDTOList result = _target.ReadAllAutos(new Empty());
            Assert.Equal(5, result.Autos.Count);
        }

        [Fact]
        public async Task DeleteAutoTest()
        {
            AutoDTO newAuto = generateExampleAuto();
            AutoIdentifier insertedId = _target.InsertAuto(newAuto);

            AutoDTO inserted = _target.ReadAutoForId(insertedId);

            _target.DeleteAuto(inserted);

            AutoDTOList result = _target.ReadAllAutos(new Empty());
            Assert.Equal(4, result.Autos.Count);
        }

        [Fact]
        public async Task UpdateAutoTest()
        {
            AutoDTO newAuto = generateExampleAuto();
            AutoIdentifier insertedID = _target.InsertAuto(newAuto);

            AutoDTO result = _target.ReadAutoForId(insertedID);

            result.Marke = "Andere Testmarke";

            _target.UpdateAuto(result);

            AutoDTO updatedResult = _target.ReadAutoForId(insertedID);


            Assert.Equal(result.Marke, updatedResult.Marke);
        }

        [Fact]
        public async Task UpdateAutoWithOptimisticConcurrencyTest()
        {
            throw new NotImplementedException("Test not implemented.");
            // arrange
            // act
            // assert
        }

        private AutoDTO generateExampleAuto()
        {
            AutoDTO result = new AutoDTO();
            result.Basistarif = 40;
            result.Klasse = AutoKlasse.Luxusklasse;
            result.Marke = "Testmarke";
            result.Tagestarif = 60;
            return result;
        }
    }
}