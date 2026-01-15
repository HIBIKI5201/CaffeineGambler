using Develop.Gambling;
using Develop.Gambling.Develop;
using Develop.Player;
using UnityEngine;
using UniRx;
using Runtime.Gambling;
namespace Runtime
{
    /// <summary>
    /// ゲーム全体の管理を担当するマネージャークラス。
    /// ゲームの初期化、状態管理、主要なシステムの調整を行う。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GamblingInitilizer _gamblingInitializer;
        [SerializeField] private PlayerInitilizer _playerInitializer;
        private PlayerData _playerData;
        private void Awake()
        {
            _playerData = new PlayerData(1000); // 初期所持金1000でプレイヤーデータを作成
            // ギャンブルシステムの初期化を実行
            _gamblingInitializer.GamblingInit(_playerData);
            _playerInitializer.Init(_playerData);
        }
    }
}

