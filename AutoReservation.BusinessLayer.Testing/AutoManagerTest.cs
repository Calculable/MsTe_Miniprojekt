using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal.Entities;
using AutoReservation.TestEnvironment;
using Microsoft.EntityFrameworkCore;
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
            _target.insert(auto);

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
            Assert.Throws<OptimisticConcurrencyException<Auto>>(() => _target.delete(auto));
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

            var ex = Assert.Throws<OptimisticConcurrencyException<Auto>>(() => _target.delete(inserted1));
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

            var ex = Assert.Throws<OptimisticConcurrencyException<Auto>>(() => _target.update(inserted2));
        }

    }
}
