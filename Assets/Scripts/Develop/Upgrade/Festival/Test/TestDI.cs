using NUnit.Framework;
using UnityEngine;
namespace Develop.Upgrade.Festival.Test
{
    public class TestDI : MonoBehaviour
    {
        int s = 10;
      

        public void EventEnd()
        {
            var threshold = 3;
            var multiplier = 3;
            var eventDomain = new TimedEvent(threshold, multiplier);

            eventDomain.StartEvent();
            eventDomain.OnHarvest();
            eventDomain.OnHarvest();

            eventDomain.AddCoffeeBeans(10);
            eventDomain.EndEvent();

            Assert.AreEqual(30, eventDomain.TotalCoffeeBeans);
        }

    }

}