using NUnit.Framework;
using UnityEngine;
namespace Develop.Upgrade.Festival
{
    public class TestDI : MonoBehaviour
    {
        
        public void Event()
        {
            var clock = new FakeClock();
            var random = new FakeRandom(30);
            var timeEvent = new TimedEvent(clock,random);

            clock.Advance(29);
            timeEvent.Update();

            Assert.IsFalse(timeEvent.IsActiv);

            clock.Advance(1);
            timeEvent.Update();

            Assert.IsTrue( timeEvent.IsActiv);

        }

    }

}