using Develop.Player;
using UnityEngine;

namespace Develop.Poker
{
    public class PokerInit : MonoBehaviour
    {
        [SerializeField] private int _initialMoney = 1000;
        private PlayerData _playerData;

        private void Awake()
        {
            _playerData = new PlayerData(_initialMoney);
        }
    }
}

