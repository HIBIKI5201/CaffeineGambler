using Develop.Player;
using UnityEngine;

public class PlayerInitilizer : MonoBehaviour
{
    [SerializeField] private PlayerPresenter _playerPresenter;
    [SerializeField] private PlayerViewer _playerViewer;
    private PlayerData _playerData;
    public void Init(PlayerData playerdata)
    {
        _playerPresenter.Init(_playerData, _playerViewer);
    }

    private void OnDestroy()
    {
        _playerData?.OnDestroy();
    }
}
