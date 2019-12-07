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
        public void GetReservationenTest()
        {
            ReservationDTOList result = _target.ReadAllReservationen(new Empty());
            Assert.Equal(4, result.Reservationen.Count);
        }

        [Fact]
        public void GetReservationByIdTest()
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
        public void GetReservationByIdWithIllegalIdTest()
        {
            int illegalId = 999;

            ReservationIdentifier reservationIdentifier = new ReservationIdentifier();
            reservationIdentifier.ReservationsNr = illegalId;

            var ex = Assert.Throws<RpcException>(() => _target.ReadReservationForId(reservationIdentifier));
            Assert.Equal(StatusCode.NotFound, ex.StatusCode);
        }

        [Fact]
        public void InsertReservationTest()
        {
            ReservationDTO newReservation = generateExampleReservation();
            _target.InsertReservation(newReservation);

            ReservationDTOList result = _target.ReadAllReservationen(new Empty());
            Assert.Equal(5, result.Reservationen.Count);
        }

        [Fact]
        public void DeleteReservationTest()
        {
            ReservationDTO newReservation = generateExampleReservation();
            ReservationIdentifier insertedId = _target.InsertReservation(newReservation);

            ReservationDTO inserted = _target.ReadReservationForId(insertedId);

            _target.DeleteReservation(inserted);

            ReservationDTOList result = _target.ReadAllReservationen(new Empty());
            Assert.Equal(4, result.Reservationen.Count);
        }

        [Fact]
        public void UpdateReservationTest()
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
        public void UpdateReservationWithOptimisticConcurrencyTest()
        {
            ReservationDTO newReservation = generateExampleReservation();
            ReservationIdentifier insertedID = _target.InsertReservation(newReservation);

            ReservationDTO inserted = _target.ReadReservationForId(insertedID);
            ReservationDTO inserted2 = _target.ReadReservationForId(insertedID);

            inserted.Von = Timestamp.FromDateTime(new DateTime(2020, 03, 02).ToUniversalTime());

            inserted2.Bis = Timestamp.FromDateTime(new DateTime(2020, 03, 04).ToUniversalTime());

            _target.UpdateReservation(inserted);

            var ex = Assert.Throws<RpcException>(() => _target.UpdateReservation(inserted2));
            Assert.Equal(StatusCode.Aborted, ex.StatusCode);
        }

        [Fact]
        public void InsertReservationWithInvalidDateRangeTest()
        {
            ReservationDTO newReservation = generateExampleReservation(new DateTime(2020, 03, 03), new DateTime(2020, 03, 01));

            var ex = Assert.Throws<RpcException>(() => _target.InsertReservation(newReservation));
            Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        }

        [Fact]
        public void InsertReservationWithAutoNotAvailableTest()
        {

            ReservationDTO newReservation = generateExampleReservation(new DateTime(2020, 03, 01), new DateTime(2020, 03, 03));
            _target.InsertReservation(newReservation);

            ReservationDTO reservationForUnavailableAuto = generateExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 02));

            var ex = Assert.Throws<RpcException>(() => _target.InsertReservation(reservationForUnavailableAuto));
            Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        }

        [Fact]
        public void UpdateReservationWithInvalidDateRangeTest()
        {
            ReservationDTO newReservation = generateExampleReservation(new DateTime(2020, 03, 01), new DateTime(2020, 03, 03));
            ReservationIdentifier insertedID = _target.InsertReservation(newReservation);
            ReservationDTO inserted = _target.ReadReservationForId(insertedID);

            ReservationDTO newReservation2 = generateExampleReservation(new DateTime(2020, 03, 04), new DateTime(2020, 03, 06));
            ReservationIdentifier insertedID2 = _target.InsertReservation(newReservation2);
            ReservationDTO inserted2 = _target.ReadReservationForId(insertedID2);

            inserted2.Von = Timestamp.FromDateTime(new DateTime(2020, 03, 02).ToUniversalTime());


            var ex = Assert.Throws<RpcException>(() => _target.UpdateReservation(inserted2));
            Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        }

        [Fact]
        public void UpdateReservationWithAutoNotAvailableTest()
        {
            ReservationDTO newReservation = generateExampleReservation();
            ReservationIdentifier insertedID = _target.InsertReservation(newReservation);

            ReservationDTO result = _target.ReadReservationForId(insertedID);

            result.Bis = Timestamp.FromDateTime(new DateTime(2020, 03, 01).ToUniversalTime());

            var ex = Assert.Throws<RpcException>(() => _target.UpdateReservation(result));
            Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        }

        [Fact]
        public void CheckAvailabilityIsTrueTest()
        {
            ReservationDTO newReservation = generateExampleReservation(new DateTime(2020, 03, 01), new DateTime(2020, 03, 03));
            _target.InsertReservation(newReservation);

            Assert.True(_target.IsCarAvailable(generateExampleReservation(new DateTime(2020, 03, 04), new DateTime(2020, 03, 04))).CarAvailable);
        }

        [Fact]
        public void CheckAvailabilityIsFalseTest()
        {
            ReservationDTO newReservation = generateExampleReservation(new DateTime(2020, 03, 01), new DateTime(2020, 03, 03));
            _target.InsertReservation(newReservation);

            Assert.True(_target.IsCarAvailable(generateExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 02))).CarAvailable);
        }

        private ReservationDTO generateExampleReservation()
        {
            return generateExampleReservation(new DateTime(2020, 03, 01), new DateTime(2020, 03, 03));
        }
        private ReservationDTO generateExampleReservation(DateTime from, DateTime to)
        {

            KundeDTO kunde = new KundeDTO();
            kunde.Id = 1;

            AutoDTO auto = new AutoDTO();
            auto.Id = 1;

            ReservationDTO reservationDTO = new ReservationDTO();
            reservationDTO.Auto = auto;
            reservationDTO.Kunde = kunde;
            reservationDTO.Von = Timestamp.FromDateTime(from.ToUniversalTime());
            reservationDTO.Bis = Timestamp.FromDateTime(to.ToUniversalTime());

            return reservationDTO;

        }
    }
}