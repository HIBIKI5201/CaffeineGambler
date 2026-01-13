using UnityEngine;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックのゲームバランス調整用パラメータを保持する ScriptableObject。
    /// インスペクターから値を変更できるようにし、コードを書き換えずに調整を可能にする。
    /// </summary>
    [CreateAssetMenu(fileName = "BlackJackSettings", menuName = "Gambling/BlackJackSettings")]
    public class BlackJackSettings : ScriptableObject
    {
        public int TargetScore => _targetScore;
        public int DealerStandThreshold => _dealerStandThreshold;
        public int CardAce => _cardAce;
        public int CardJack => _cardJack;
        public int RandomRangeMin => _randomRangeMin;
        public int RandomRangeMaxExclusive => _randomRangeMaxExclusive;
        public int AceBonusScore => _aceBonusScore;
        public int FaceCardScore => _faceCardScore;
        public int FaceCardThreshold => _faceCardThreshold;
        public int InitialHandCount => _initialHandCount;

        [Header("Game Rules")]
        [SerializeField, Tooltip("目標とする点数（通常21）")]
        private int _targetScore = 21;

        [SerializeField, Tooltip("ディーラーがスタンドする閾値（通常17）")]
        private int _dealerStandThreshold = 17;

        [SerializeField, Tooltip("初期手札の枚数")]
        private int _initialHandCount = 2;

        [Header("Card Values")]
        [SerializeField, Tooltip("エースのカード数値")]
        private int _cardAce = 1;

        [SerializeField, Tooltip("ジャックのカード数値")]
        private int _cardJack = 11;

        [SerializeField, Tooltip("点数計算時にJ,Q,Kを何点として扱うか")]
        private int _faceCardScore = 10;

        [SerializeField, Tooltip("この数値以上のカードを絵札（10点）として扱う閾値")]
        private int _faceCardThreshold = 11;

        [SerializeField, Tooltip("エースを11点として扱う際に追加するスコア(1+10)")]
        private int _aceBonusScore = 10;

        [Header("Deck Settings")]
        [SerializeField, Tooltip("ランダム生成するカードの最小値")]
        private int _randomRangeMin = 1;

        [SerializeField, Tooltip("ランダム生成するカードの最大値（この値自体は含まない）")]
        private int _randomRangeMaxExclusive = 14;
    }
}
