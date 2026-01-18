using System.Collections.Generic;
using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    /// 採取の倍率等の計算をするクラス。
    /// </summary>
    public class CollectionCalculation
    {
        private readonly List<IModifier> _modifiers;

        public CollectionCalculation(List<IModifier> modifiers)
        {
            _modifiers = new List<IModifier>(modifiers);
        }

        public float ApplyModifiers(float baseValue)
        {
            float modifiedValue = baseValue;
            foreach (var modifier in _modifiers)
            {
                modifiedValue = modifier.Modify(modifiedValue);
            }
            return modifiedValue;
        }
    }
}
