using Develop.Player;
using UnityEngine;

namespace Develop.Poker
{
    public class PokerInit : MonoBehaviour
    {
        [SerializeField] private int _initialMoney = 1000;
        [SerializeField] private PlayerPresenter _playerPresenter;
        [SerializeField] private PlayerViewer _playerViewer;
        [SerializeField] private PokerBattlePresenter _battlePresenter;

        private PlayerData _playerData;

        private void Awake()
        {
            _playerData = new PlayerData(Mathf.Max(0, _initialMoney));
            _playerPresenter?.Init(_playerData);
            _battlePresenter?.Initialize(_playerData);
        }

        private void OnDestroy()
        {
            _playerData?.OnDestroy();
        }
    }
}

