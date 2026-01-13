using Develop.Player;
using UnityEngine;

public class PlayerInitilize : MonoBehaviour
{
    private PlayerData _playerData;
    private void Start()
    {
        _playerData = new PlayerData(0);
    }

    private void OnDestroy()
    {
        _playerData.OnDestroy();
    }
}
