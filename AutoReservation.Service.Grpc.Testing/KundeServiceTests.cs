﻿using System;
using System.Threading.Tasks;
using AutoReservation.Service.Grpc.Testing.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Xunit;

namespace AutoReservation.Service.Grpc.Testing
{
    public class KundeServiceTests
        : ServiceTestBase
    {
        private readonly KundeService.KundeServiceClient _target;

        public KundeServiceTests(ServiceTestFixture serviceTestFixture)
            : base(serviceTestFixture)
        {
            _target = new KundeService.KundeServiceClient(Channel);
        }

        [Fact]
        public void GetKundenTest()
        {
            KundeDTOList result = _target.ReadAllKunden(new Empty());
            Assert.Equal(4, result.Kunden.Count);
        }

        [Fact]
        public void GetKundeByIdTest()
        { 
            KundeDTO newKunde = generateExampleKunde();
            KundeIdentifier insertedID = _target.InsertKunde(newKunde);

            KundeDTO result = _target.ReadKundeForId(insertedID);
            Assert.Equal(newKunde.Geburtsdatum, result.Geburtsdatum);
            Assert.Equal(newKunde.Nachname, result.Nachname);
            Assert.Equal(newKunde.Vorname, result.Vorname);
        }

        [Fact]
        public void GetKundeByIdWithIllegalIdTest()
        {
            int illegalId = 999;

            KundeIdentifier kundeIdentifier = new KundeIdentifier();
            kundeIdentifier.Id = illegalId;

            var ex = Assert.Throws<RpcException>(() => _target.ReadKundeForId(kundeIdentifier));
            Assert.Equal(StatusCode.NotFound, ex.StatusCode);
        }

        [Fact]
        public void InsertKundeTest()
        {
            KundeDTO newKunde = generateExampleKunde();
            _target.InsertKunde(newKunde);

            KundeDTOList result = _target.ReadAllKunden(new Empty());
            Assert.Equal(5, result.Kunden.Count);
        }

        [Fact]
        public void DeleteKundeTest()
        {
            KundeDTO newKunde = generateExampleKunde();
            KundeIdentifier insertedId = _target.InsertKunde(newKunde);

            KundeDTO inserted = _target.ReadKundeForId(insertedId);

            _target.DeleteKunde(inserted);

            KundeDTOList result = _target.ReadAllKunden(new Empty());
            Assert.Equal(4, result.Kunden.Count);
        }

        [Fact]
        public void UpdateKundeTest()
        {
            KundeDTO newKunde = generateExampleKunde();
            KundeIdentifier insertedID = _target.InsertKunde(newKunde);

            KundeDTO result = _target.ReadKundeForId(insertedID);

            result.Vorname = "Anderer Vorname";

            _target.UpdateKunde(result);

            KundeDTO updatedResult = _target.ReadKundeForId(insertedID);

            Assert.Equal(result.Vorname, updatedResult.Vorname);
        }

        [Fact]
        public void UpdateKundeWithOptimisticConcurrencyTest()
        {
            KundeDTO newKunde = generateExampleKunde();
            KundeIdentifier insertedID = _target.InsertKunde(newKunde);

            KundeDTO inserted = _target.ReadKundeForId(insertedID);
            KundeDTO inserted2 = _target.ReadKundeForId(insertedID);

            inserted.Nachname = "Anderer Nachname";
            inserted2.Vorname = "Anderer Vorname";

            _target.UpdateKunde(inserted);

            var ex = Assert.Throws<RpcException>(() => _target.UpdateKunde(inserted2));
            Assert.Equal(StatusCode.Aborted, ex.StatusCode);
        }

        private KundeDTO generateExampleKunde()
        {
            KundeDTO result = new KundeDTO();

            Timestamp geburtsdatum = Timestamp.FromDateTime(new DateTime(1990, 05, 02).ToUniversalTime());
            result.Geburtsdatum = geburtsdatum;

            result.Nachname = "Mustermann";
            result.Vorname = "Max";
            return result;
        }
    }
}