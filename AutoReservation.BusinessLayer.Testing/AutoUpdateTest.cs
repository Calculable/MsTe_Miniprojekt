using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using AutoReservation.TestEnvironment;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoReservation.BusinessLayer.Testing
{
    public class AutoUpdateTests
        : TestBase
    {
        private readonly AutoManager _target;

        public AutoUpdateTests()
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
    }
}
