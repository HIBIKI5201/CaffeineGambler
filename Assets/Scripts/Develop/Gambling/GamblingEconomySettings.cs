using UnityEngine;

namespace Develop.Gambling
{
    /// <summary>
    /// ギャンブルの経済バランス（配当倍率など）を保持する ScriptableObject。
    /// クラスごとに設定を分離することで、プログラマー以外でもバランス調整を容易にするために必要。
    /// </summary>
    [CreateAssetMenu(fileName = "GamblingEconomySettings", menuName = "Gambling/EconomySettings")]
    public class GamblingEconomySettings : ScriptableObject
    {
        public int WinMultiplier => _winMultiplier;
        public int BlackJackMultiplier => _blackJackMultiplier;
        public int DrawMultiplier => _drawMultiplier;

        [Header("Payout Multipliers")]
        [SerializeField, Tooltip("通常勝利時の配当倍率（賭け金に対する倍率）")]
        private int _winMultiplier = 2;

        [SerializeField, Tooltip("ブラックジャック勝利時の配当倍率")]
        private int _blackJackMultiplier = 3;

        [SerializeField, Tooltip("引き分け時の返金倍率（通常は1で全額返金）")]
        private int _drawMultiplier = 1;
    }
}
