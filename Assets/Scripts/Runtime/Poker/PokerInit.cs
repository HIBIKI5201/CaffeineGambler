using Develop.Player;
using Develop.Poker;
using UnityEngine;

namespace Runtime
{
    public class PokerInit : MonoBehaviour
    {
        [SerializeField] private PlayerViewer _playerViewer;
        [SerializeField] private PokerBattlePresenter _battlePresenter;

        public void Init(PlayerData playerData)
        {
            _battlePresenter?.Initialize(playerData);
        }
    }
}
