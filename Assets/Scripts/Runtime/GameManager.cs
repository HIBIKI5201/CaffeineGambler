using Develop.Player;
using Runtime.Gambling;
using Runtime.Save;
using UnityEngine;
namespace Runtime
{
    /// <summary>
    /// ゲーム全体の管理を担当するマネージャークラス。
    /// ゲームの初期化、状態管理、主要なシステムの調整を行う。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GamblingInitilizer _gamblingInitializer;
        [SerializeField] private PokerInit _pokerInitializer;
        [SerializeField] private PlayerInitilizer _playerInitializer;
        [SerializeField] private int _initialMoney = 1000;
        private PlayerData _playerData;
        private SaveInitializer _saveInitializer;
        private void Awake()
        {
            _playerData = new PlayerData(_initialMoney); // 初期所持金1000でプレイヤーデータを作成
            // ギャンブルシステムの初期化を実行
            _saveInitializer = new SaveInitializer();
            _saveInitializer.Init(_playerData);
            _gamblingInitializer.GamblingInit(_playerData);
            _pokerInitializer?.Init(_playerData);
            _playerInitializer.Init(_playerData);
            
        }
        private void OnApplicationQuit()
        {
           _saveInitializer.Save(_playerData);
        }
    }
}

