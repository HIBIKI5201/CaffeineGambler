using NUnit.Framework;
using UnityEngine;
namespace Develop.Upgrade.Festival
{
    public class TestDI : MonoBehaviour
    {
        int s = 10;
        private void Start()
        {
            Event();
        }
        [Test]
        public void Event()
        {
            var clock = new FakeClock();
            var random = new FakeRandom(30);
            var timeEvent = new TimedEvent(clock,random,s);

            timeEvent.Harvest();
            Assert.AreEqual(0, timeEvent.EventCounter);
        }

        public void EventActive()
        {
            var clock = new FakeClock();
            var random = new FakeRandom(30);
            var timeEvent = new TimedEvent(clock, random, s);

            clock.Advance(39);
            timeEvent.Update();
            Assert.IsTrue(timeEvent.IsActive);

            timeEvent.Harvest();
            Assert.AreEqual(1, timeEvent.EventCounter);
        }

    }

}