using Develop.Player;
using Develop.Poker;
using UnityEngine;

namespace Runtime
{
    public class PokerInit : MonoBehaviour
    {
        [SerializeField] private PlayerPresenter _playerPresenter;
        [SerializeField] private PlayerViewer _playerViewer;
        [SerializeField] private PokerBattlePresenter _battlePresenter;

        public void Init(PlayerData playerData)
        {
            _playerPresenter?.Init(playerData);
            _battlePresenter?.Initialize(playerData);
        }
    }
}
