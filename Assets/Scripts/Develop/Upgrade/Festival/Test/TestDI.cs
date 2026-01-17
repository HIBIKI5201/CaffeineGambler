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
        public void Event()
        {
            var clock = new FakeClock();
            var random = new FakeRandom(30);
            var timeEvent = new TimedEvent(clock,random,s);


            //30秒たったらイベントが起動する
            clock.Advance(29);　
            timeEvent.Update();
            Assert.IsFalse(timeEvent.IsActive);

            //ここでイベントがtrue確認
            clock.Advance(1);
            timeEvent.Update();
            Assert.IsTrue(timeEvent.IsActive);

            //新しくタイマーがセットされるかの確認
            //フィーバータイムは10秒なので
            //falseならOK
            clock.Advance(10);
            timeEvent.Update();
            Assert.IsFalse(timeEvent.IsActive);

            //イベントが起動したか
            clock.Advance(30);
            timeEvent.Update();
            Assert.IsTrue(timeEvent.IsActive);

        }

    }

}