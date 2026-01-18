using Cysharp.Threading.Tasks;
using Develop.Player;
using Develop.Upgrade;
using Develop.Upgrade.Festival;
using Domain;
using UniRx;
using UnityEngine;

/// <summary>
/// フェスティバル機能のUnityインフラ層。
/// </summary>
public class FestivalInfra : MonoBehaviour
{
    [Header("Event Settings")]
    [SerializeField] private int _eventDuration = 5;

    [Header("Level Settings")]
    [SerializeField] private RewardRankSO _levelData;
    [SerializeField] private int _currentLevel = 1;

    private TimedEvent _timeEvent;
    private EventReward _eventReward;
    private HarvestBus _hervestBus;
    private PlayerData _playerData;

    public void Init(HarvestBus harvestBus,PlayerData playerData)
    {
        _hervestBus = harvestBus;
        _playerData = playerData;

        IClock clock = new SystemClock();
        IRandom random = new SystemRandom();

        // オブジェクトの構築
        _timeEvent = new TimedEvent(clock, random, _eventDuration);
        _eventReward = new EventReward(_timeEvent,_playerData);

        // イベント購読によるライフサイクル管理
        _timeEvent.OnStarted += _eventReward.OnEventStarted;
        _timeEvent.OnEnded += HandleEventEnded;

        // 採取入力の登録
        _hervestBus.OnHarvested.Subscribe(onNext: amount =>
        {
            _eventReward.OnHarvest();
            _eventReward.AddCoffeeBeans(amount);
            Debug.Log("押された");
        }).AddTo(this);
        Debug.Log("Festival System Init Complete");
    }

    /// <summary>
    /// Upgradeからレベルを変更できるように
    /// </summary>
    /// <param name="level"></param>
    public void SetFestivalLevel(int level)
    {
        _currentLevel = level;
    }

    private void Update()
    {
        // 時間の監視
        _timeEvent.Update();

        Debug.Log(_timeEvent.IsActive);
    }

    /// <summary>
    /// イベント開始時の設定およびリセット処理。
    /// </summary>
    private void HandleEventEnded()
    {
        Debug.Log(_eventReward.TotalCoffeeBeans);
        _eventReward.ConfigureRanks(_currentLevel, _levelData);
        _eventReward.OnEventEnded();
    }
}