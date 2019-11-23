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
    public class AutoManagerTests
        : TestBase
    {
        private readonly AutoManager _target;

        public AutoManagerTests()
        {
            _target = new AutoManager();
        }

      /*  [Fact]
        public async Task UpdateAutoTest()
        {
            // arrange
            // act
            // assert
            using(AutoReservationContext context = new AutoReservationContext())
            {
            }

        }*/

        [Fact]
        public async Task GetAllAutoTest()
        {
            List<Auto> autos = await _target.GetAll();
            Assert.Equal(4, autos.Count);

        }

        [Fact]
        public async Task InsertAutoTest()
        {
            Auto auto = new MittelklasseAuto { Marke = "Testmarke", Tagestarif = 40 };
            int newId = _target.insert(auto);

            List<Auto> autos = await _target.GetAll();
            Assert.Equal(5, autos.Count);
        }

        [Fact]
        public void GetAutoForKeyTest()
        {
            Auto toInsert = new MittelklasseAuto { Marke = "Testmarke", Tagestarif = 40 };
            int newId = _target.insert(toInsert);

            Auto inserted = _target.GetForKey(newId);
            Assert.Equal(toInsert.Marke, inserted.Marke);
            Assert.Equal(toInsert.Tagestarif, inserted.Tagestarif);

        }

        [Fact]
        public void GetAutoForNonExistingKeyThrowsExceptionTest()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _target.GetForKey(999));
            Assert.Equal("Sequence contains no elements", ex.Message);
        }

        [Fact]
        public void InsertAlreadyExistingAutoThrowsExceptionTest()
        {
            Auto toInsert = new MittelklasseAuto { Marke = "Testmarke", Tagestarif = 40 };
            int newId = _target.insert(toInsert);

            
            Auto inserted = _target.GetForKey(newId);

            Assert.Throws<DbUpdateException>(() => _target.insert(inserted));


        }

        [Fact]
        public async Task DeleteAutoTest()
        {
            Auto auto = new MittelklasseAuto { Marke = "Testmarke", Tagestarif = 40 };
            int newId = _target.insert(auto);

            List<Auto> autos = await _target.GetAll();
            Assert.Equal(5, autos.Count);

            Auto inserted = _target.GetForKey(newId);
            _target.delete(inserted);

            autos = await _target.GetAll();
            Assert.Equal(4, autos.Count);
        }

        [Fact]
        public void DeleteNotInsertedAutoThrowsExceptionTest()
        {
            Auto auto = new MittelklasseAuto { Marke = "Testmarke", Tagestarif = 40 };
           
            Assert.Throws<OptimisticConcurrencyException<PropertyValues>>(() => _target.delete(auto));

        }

        [Fact]
        public void DeleteUpdatedAutoThrowsExceptionTest()
        {
            Auto auto = new MittelklasseAuto { Marke = "Testmarke", Tagestarif = 40 };
            int newId = _target.insert(auto);


            Auto inserted1 = _target.GetForKey(newId);
            Auto inserted2 = _target.GetForKey(newId);
            inserted2.Marke = "another";
            _target.update(inserted2);

            var ex = Assert.Throws<OptimisticConcurrencyException<PropertyValues>>(() => _target.delete(inserted1));
        }

        [Fact]
        public void UpdateAutoTest()
        {
            Auto auto = new MittelklasseAuto { Marke = "Testmarke", Tagestarif = 40 };
            int newId = _target.insert(auto);

            auto.Marke = "another";
            _target.update(auto);

            Auto inserted = _target.GetForKey(newId);

            Assert.Equal("another", inserted.Marke);
        }

        [Fact]
        public void UpdateChangedAutoThrowsExceptionTest()
        {
            Auto auto = new MittelklasseAuto { Marke = "Testmarke", Tagestarif = 40 };
            int newId = _target.insert(auto);


            Auto inserted1 = _target.GetForKey(newId);
            Auto inserted2 = _target.GetForKey(newId);

            inserted1.Marke = "one";
            _target.update(inserted1);

            inserted2.Marke = "two";

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
