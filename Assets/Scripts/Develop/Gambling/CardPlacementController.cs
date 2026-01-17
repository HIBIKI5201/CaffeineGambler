using UnityEngine;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックにおけるカードの3D配置点を管理するクラス。
    /// シーンに配置し、各 Transform をインスペクターから設定する。
    /// </summary>
    public class CardPlacementController : MonoBehaviour
    {
        [Tooltip("ディーラーがカードを生成する（持つ）位置")]
        [SerializeField] private RectTransform _dealerCardOrigin;
        public RectTransform DealerCardOrigin => _dealerCardOrigin;

        [Tooltip("プレイヤーの手札を整列させる親RectTransform")]
        [SerializeField] private RectTransform _playerHandContainer;
        public RectTransform PlayerHandContainer => _playerHandContainer;
        
        [Tooltip("ディーラーの手札を整列させる親RectTransform")]
        [SerializeField] private RectTransform _dealerHandContainer;
        public RectTransform DealerHandContainer => _dealerHandContainer;
    }
}
