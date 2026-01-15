using Cysharp.Threading.Tasks;
using Develop.Player;
using UniRx;
using UnityEngine;

namespace Develop.Save
{
    /// <summary> セーブお試し用クラス。 </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private PlayerDataHandler _playerDataHandler;

        public PlayerData PlayerData => _playerDataHandler?.PlayerData;

        [SerializeField] private PlayerViewer _playerViewer;

        private CompositeDisposable _disposables;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _disposables = new CompositeDisposable();
            _playerDataHandler = new PlayerDataHandler();
            _playerDataHandler.Load();
            PlayerData.Money.Subscribe(money => _playerViewer.SetCount(money)).AddTo(_disposables);
        }

        private void OnApplicationQuit()
        {
            _playerDataHandler?.Save();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                _playerDataHandler?.OnDestroy();
            }
        }

        private void Update()
        {
            PlayerData?.AddMoney(1);
        }
    }
}
