using System;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal.Entities;
using AutoReservation.TestEnvironment;
using Xunit;

namespace AutoReservation.BusinessLayer.Testing
{
    public class ReservationAvailabilityTest
        : TestBase
    {
        private readonly ReservationManager _target;
        public ReservationAvailabilityTest()
        {
            _target = new ReservationManager();
        }

        [Fact]
        public void ScenarioOkay01Test()
        {
            //| ---Date 1--- |
            //               | ---Date 2--- |

            Reservation firstReservation = createNewExampleReservation(new DateTime(2020, 03, 01), new DateTime(2020, 03, 02));
            Reservation secondReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 03));

            _target.insert(firstReservation); 
            _target.insert(secondReservation);
        }

        [Fact]
        public void ScenarioOkay02Test()
        {
            //| ---Date 1--- |
            //                 | ---Date 2--- |

            Reservation firstReservation = createNewExampleReservation(new DateTime(2020, 03, 01), new DateTime(2020, 03, 02));
            Reservation secondReservation = createNewExampleReservation(new DateTime(2020, 03, 03), new DateTime(2020, 03, 04));

            _target.insert(firstReservation);
            _target.insert(secondReservation);
        }

        [Fact]
        public void ScenarioOkay03Test()
        {
            //                | ---Date 1--- |
            //| ---Date 2-- - |

            Reservation firstReservation = createNewExampleReservation(new DateTime(2020, 03, 03), new DateTime(2020, 03, 04));
            Reservation secondReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 03));

            _target.insert(firstReservation);
            _target.insert(secondReservation);
        }

        [Fact]
        public void ScenarioOkay04Test()
        {
            //                | ---Date 1--- |
            //| ---Date 2--- |

            Reservation firstReservation = createNewExampleReservation(new DateTime(2020, 03, 03), new DateTime(2020, 03, 04));
            Reservation secondReservation = createNewExampleReservation(new DateTime(2020, 03, 01), new DateTime(2020, 03, 02));

            _target.insert(firstReservation);
            _target.insert(secondReservation);
        }

        [Fact]
        public void ScenarioNotOkay01Test()
        {
            //| ---Date 1--- |
            //    | ---Date 2--- |

            Reservation firstReservation = createNewExampleReservation(new DateTime(2020, 03, 01), new DateTime(2020, 03, 03));
            Reservation secondReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 04));

            int firstResult = _target.insert(firstReservation);

            var ex = Assert.Throws<AutoUnavailableException>(() => _target.insert(secondReservation));
        }

        [Fact]
        public void ScenarioNotOkay02Test()
        {
            //    | ---Date 1--- |
            //| ---Date 2--- |

            Reservation firstReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 04));
            Reservation secondReservation = createNewExampleReservation(new DateTime(2020, 03, 01), new DateTime(2020, 03, 03));

            int firstResult = _target.insert(firstReservation);

            var ex = Assert.Throws<AutoUnavailableException>(() => _target.insert(secondReservation));
        }

        [Fact]
        public void ScenarioNotOkay03Test()
        {
            //| ---Date 1--- |
            //| --------Date 2-------- |

            Reservation firstReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 04));
            Reservation secondReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 06));

            int firstResult = _target.insert(firstReservation);

            var ex = Assert.Throws<AutoUnavailableException>(() => _target.insert(secondReservation));
        }

        [Fact]
        public void ScenarioNotOkay04Test()
        {
            //| --------Date 1-------- |
            //| ---Date 2--- |

            Reservation firstReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 04));
            Reservation secondReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 03));

            int firstResult = _target.insert(firstReservation);

            var ex = Assert.Throws<AutoUnavailableException>(() => _target.insert(secondReservation));
        }

        [Fact]
        public void ScenarioNotOkay05Test()
        {

            //| ---Date 1--- |
            //| ---Date 2--- |

            Reservation firstReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 04));
            Reservation secondReservation = createNewExampleReservation(new DateTime(2020, 03, 02), new DateTime(2020, 03, 04));

            int firstResult = _target.insert(firstReservation);

            var ex = Assert.Throws<AutoUnavailableException>(() => _target.insert(secondReservation));
        }

        private Reservation createNewExampleReservation(DateTime von, DateTime bis)
        {
            Kunde kunde = new Kunde { Name = "Müller", Vorname = "Judith", Geburtsdatum = new DateTime(1980, 02, 13) };
            return new Reservation { AutoId = 1, Kunde = kunde, Von = von, Bis = bis };
        }


    }
}
