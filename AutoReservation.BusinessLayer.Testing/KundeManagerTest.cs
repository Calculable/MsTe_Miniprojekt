using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal.Entities;
using AutoReservation.TestEnvironment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;

namespace AutoReservation.BusinessLayer.Testing
{
    public class KundeManagerTest
        : TestBase
    {
        private readonly KundeManager _target;

        public KundeManagerTest()
        {
            _target = new KundeManager();
        }

  

        [Fact]
        public async Task GetAllKundeTest()
        {
            List<Kunde> kunden = await _target.GetAll();
            Assert.Equal(4, kunden.Count);

        }

        [Fact]
        public async Task InsertKundeTest()
        {
            Kunde kunde = new Kunde { Name = "Mustermann", Vorname = "Max", Geburtsdatum = new DateTime(1990, 05, 02) };

            int newId = _target.insert(kunde);

            List<Kunde> kunden = await _target.GetAll();
            Assert.Equal(5, kunden.Count);
        }

        [Fact]
        public void GetKundeForKeyTest()
        {
            Kunde kundeToInsert = new Kunde { Name = "Mustermann", Vorname = "Max", Geburtsdatum = new DateTime(1990, 05, 02) };
            int newId = _target.insert(kundeToInsert);

            Kunde inserted = _target.GetForKey(newId);
            Assert.Equal(kundeToInsert.Name, inserted.Name);
            Assert.Equal(kundeToInsert.Vorname, inserted.Vorname);
            Assert.Equal(kundeToInsert.Geburtsdatum, inserted.Geburtsdatum);
        }

        [Fact]
        public void GetKundeForNonExistingKeyThrowsExceptionTest()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _target.GetForKey(999));
            Assert.Equal("Sequence contains no elements", ex.Message);
        }

        [Fact]
        public void InsertAlreadyExistingKundeThrowsExceptionTest()
        {
            Kunde toInsert = new Kunde { Name = "Mustermann", Vorname = "Max", Geburtsdatum = new DateTime(1990, 05, 02) };
            int newId = _target.insert(toInsert);


            Kunde inserted = _target.GetForKey(newId);

            Assert.Throws<DbUpdateException>(() => _target.insert(inserted));


        }

        [Fact]
        public async Task DeleteKundeTest()
        {
            Kunde kunde = new Kunde { Name = "Mustermann", Vorname = "Max", Geburtsdatum = new DateTime(1990, 05, 02) };
            int newId = _target.insert(kunde);

            List<Kunde> kunden = await _target.GetAll();
            Assert.Equal(5, kunden.Count);

            Kunde inserted = _target.GetForKey(newId);
            _target.delete(inserted);

            kunden = await _target.GetAll();
            Assert.Equal(4, kunden.Count);
        }

        [Fact]
        public void DeleteNotInsertedKundeThrowsExceptionTest()
        {
            Kunde kunde = new Kunde { Name = "Mustermann", Vorname = "Max", Geburtsdatum = new DateTime(1990, 05, 02) };

            Assert.Throws<OptimisticConcurrencyException<PropertyValues>>(() => _target.delete(kunde));

        }

        [Fact]
        public void DeleteUpdatedKundeThrowsExceptionTest()
        {
            Kunde kunde = new Kunde { Name = "Mustermann", Vorname = "Max", Geburtsdatum = new DateTime(1990, 05, 02) };
            int newId = _target.insert(kunde);


            Kunde inserted1 = _target.GetForKey(newId);
            Kunde inserted2 = _target.GetForKey(newId);
            inserted2.Name = "another";
            _target.update(inserted2);

            var ex = Assert.Throws<OptimisticConcurrencyException<PropertyValues>>(() => _target.delete(inserted1));
        }

        [Fact]
        public void UpdateKundeTest()
        {
            Kunde kunde = new Kunde { Name = "Mustermann", Vorname = "Max", Geburtsdatum = new DateTime(1990, 05, 02) };
            int newId = _target.insert(kunde);

            kunde.Name = "another";
            _target.update(kunde);

            Kunde inserted = _target.GetForKey(newId);

            Assert.Equal("another", inserted.Name);
        }

        [Fact]
        public void UpdateChangedKundeThrowsExceptionTest()
        {
            Kunde kunde = new Kunde { Name = "Mustermann", Vorname = "Max", Geburtsdatum = new DateTime(1990, 05, 02) };
            int newId = _target.insert(kunde);


            Kunde inserted1 = _target.GetForKey(newId);
            Kunde inserted2 = _target.GetForKey(newId);

            inserted1.Name = "one";
            _target.update(inserted1);

            inserted2.Name = "two";

            var ex = Assert.Throws<OptimisticConcurrencyException<PropertyValues>>(() => _target.update(inserted2));
        }


        /*            Auto[] autos = {
                new StandardAuto {Marke = "Fiat Punto", Tagestarif = 50},
                new MittelklasseAuto {Marke = "VW Golf", Tagestarif = 120},
                new LuxusklasseAuto {Marke = "Audi S6", Tagestarif = 180, Basistarif = 50},
                new StandardAuto {Marke = "Fiat 500", Tagestarif = 75},
            };

            Kunde[] kunden = {
                new Kunde {Name = "Nass", Vorname = "Anna", Geburtsdatum = new DateTime(1981, 05, 05)},
                new Kunde {Name = "Beil", Vorname = "Timo", Geburtsdatum = new DateTime(1980, 09, 09)},
                new Kunde {Name = "Pfahl", Vorname = "Martha", Geburtsdatum = new DateTime(1990, 07, 03)},
                new Kunde {Name = "Zufall", Vorname = "Rainer", Geburtsdatum = new DateTime(1954, 11, 11)},
            };

            int year = DateTime.Now.Year + 1;
            Reservation[] reservationen = {
                new Reservation {Auto = autos[0], Kunde = kunden[0], Von = new DateTime(year, 01, 10), Bis = new DateTime(year, 01, 20)},
                new Reservation {Auto = autos[1], Kunde = kunden[1], Von = new DateTime(year, 01, 10), Bis = new DateTime(year, 01, 20)},
                new Reservation {Auto = autos[2], Kunde = kunden[2], Von = new DateTime(year, 01, 10), Bis = new DateTime(year, 01, 20)},
                new Reservation {Auto = autos[1], Kunde = kunden[0], Von = new DateTime(year, 05, 19), Bis = new DateTime(year, 06, 19)},
            };*/
    }
}
