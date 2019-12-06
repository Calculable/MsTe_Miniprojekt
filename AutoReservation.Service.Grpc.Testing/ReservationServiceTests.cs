using System;
using System.Threading.Tasks;
using AutoReservation.Service.Grpc.Testing.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Xunit;

namespace AutoReservation.Service.Grpc.Testing
{
    public class ReservationServiceTests
        : ServiceTestBase
    {
        private readonly ReservationService.ReservationServiceClient _target;
        private readonly AutoService.AutoServiceClient _autoClient;
        private readonly KundeService.KundeServiceClient _kundeClient;

        public ReservationServiceTests(ServiceTestFixture serviceTestFixture)
            : base(serviceTestFixture)
        {
            _target = new ReservationService.ReservationServiceClient(Channel);
            _autoClient = new AutoService.AutoServiceClient(Channel);
            _kundeClient = new KundeService.KundeServiceClient(Channel);
        }

        [Fact]
        public async Task GetReservationenTest()
        {
            ReservationDTOList result = _target.ReadAllReservationen(new Empty());
            Assert.Equal(4, result.Reservationen.Count);
        }

        [Fact]
        public async Task GetReservationByIdTest()
        {
            ReservationDTO newReservation = generateExampleReservation();
            ReservationIdentifier insertedID = _target.InsertReservation(newReservation);

            ReservationDTO result = _target.ReadReservationForId(insertedID);
            Assert.Equal(newReservation.Auto.Id, result.Auto.Id);
            Assert.Equal(newReservation.Kunde.Id, result.Kunde.Id);
            Assert.Equal(newReservation.Bis, result.Bis);
            Assert.Equal(newReservation.Von, result.Von);
        }

        [Fact]
        public async Task GetReservationByIdWithIllegalIdTest()
        {
            throw new NotImplementedException("Test not implemented.");
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task InsertReservationTest()
        {
            ReservationDTO newReservation = generateExampleReservation();
            _target.InsertReservation(newReservation);

            ReservationDTOList result = _target.ReadAllReservationen(new Empty());
            Assert.Equal(5, result.Reservationen.Count);
        }

        [Fact]
        public async Task DeleteReservationTest()
        {
            ReservationDTO newReservation = generateExampleReservation();
            ReservationIdentifier insertedId = _target.InsertReservation(newReservation);

            ReservationDTO inserted = _target.ReadReservationForId(insertedId);

            _target.DeleteReservation(inserted);

            ReservationDTOList result = _target.ReadAllReservationen(new Empty());
            Assert.Equal(4, result.Reservationen.Count);
        }

        [Fact]
        public async Task UpdateReservationTest()
        {
            ReservationDTO newReservation = generateExampleReservation();
            ReservationIdentifier insertedID = _target.InsertReservation(newReservation);

            ReservationDTO result = _target.ReadReservationForId(insertedID);

            result.Bis = Timestamp.FromDateTime(new DateTime(2020, 03, 04).ToUniversalTime());

            ReservationIdentifier updatedID = _target.UpdateReservation(result);

            ReservationDTO updatedResult = _target.ReadReservationForId(updatedID);

            Assert.Equal(result.Bis, updatedResult.Bis);
        }

        [Fact]
        public async Task UpdateReservationWithOptimisticConcurrencyTest()
        {
            throw new NotImplementedException("Test not implemented.");
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task InsertReservationWithInvalidDateRangeTest()
        {
            throw new NotImplementedException("Test not implemented.");
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task InsertReservationWithAutoNotAvailableTest()
        {
            throw new NotImplementedException("Test not implemented.");
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task UpdateReservationWithInvalidDateRangeTest()
        {
            throw new NotImplementedException("Test not implemented.");
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task UpdateReservationWithAutoNotAvailableTest()
        {
            throw new NotImplementedException("Test not implemented.");
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task CheckAvailabilityIsTrueTest()
        {
            throw new NotImplementedException("Test not implemented.");
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task CheckAvailabilityIsFalseTest()
        {
            throw new NotImplementedException("Test not implemented.");
            // arrange
            // act
            // assert
        }

        private ReservationDTO generateExampleReservation()
        {
            return generateExampleReservation(new DateTime(2020, 03, 01), new DateTime(2020, 03, 03));
        }
            private ReservationDTO generateExampleReservation(DateTime from, DateTime to)
        {

            DateTime datetime = new DateTime(1990, 05, 02);

            KundeDTO kunde = new KundeDTO();
            /* Timestamp geburtsdatum = Timestamp.FromDateTime(new DateTime(1990, 05, 02).ToUniversalTime());
             kunde.Geburtsdatum = geburtsdatum;
             kunde.Nachname = "Mustermann";
             kunde.Vorname = "Max";*/
            kunde.Id = 1;

            AutoDTO auto = new AutoDTO();
            auto.Id = 1;
           /* auto.Basistarif = 40;
            auto.Klasse = AutoKlasse.Mittelklasse;
            auto.Marke = "Testmarke";
            auto.Tagestarif = 60;*/

            ReservationDTO reservationDTO = new ReservationDTO();
            reservationDTO.Auto = auto;
            reservationDTO.Kunde = kunde;
            reservationDTO.Von = Timestamp.FromDateTime(from.ToUniversalTime());
            reservationDTO.Bis = Timestamp.FromDateTime(to.ToUniversalTime());

            return reservationDTO;


        }
    }
}