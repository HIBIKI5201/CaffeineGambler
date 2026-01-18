using UniRx;
using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    /// 「採取が起きたこと」を通知する場所
    /// </summary>
    public class HarvestBus
    {
        public Subject<int> OnHarvested { get; } = new();
    }
}
