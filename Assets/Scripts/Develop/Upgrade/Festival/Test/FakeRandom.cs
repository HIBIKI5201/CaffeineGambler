using UnityEngine;

namespace Develop.Upgrade.Festival
{
    public class  FakeRandom : IRandom
    {
        public FakeRandom(int value)
        {
            _value = value;
        }
        public int Range(int min, int max)
        {
           return _value;
        }


        private readonly int _value;
    }
}