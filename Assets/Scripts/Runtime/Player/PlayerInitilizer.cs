using Develop.Player;
using UnityEngine;

public class PlayerInitilizer : MonoBehaviour
{
    [SerializeField] private PlayerPresenter _playerPresenter;
    private PlayerData _playerData;
    public void Init(PlayerData playerdata)
    {
        _playerData = playerdata;
        _playerPresenter.Init(_playerData);
    }

    private void OnDestroy()
    {
        _playerData?.OnDestroy();
    }
}
