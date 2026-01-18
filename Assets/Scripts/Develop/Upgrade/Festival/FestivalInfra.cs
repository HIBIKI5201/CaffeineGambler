using Develop.Upgrade.Festival;
using Domain;
using UnityEngine;
using System.Collections.Generic;

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

    private void Start()
    {
        IClock clock = new SystemClock();
        IRandom random = new SystemRandom();

        // オブジェクトの構築
        _timeEvent = new TimedEvent(clock, random, _eventDuration);
        _eventReward = new EventReward(_timeEvent);

        // イベント購読によるライフサイクル管理
        _timeEvent.OnStarted += _eventReward.OnEventStarted;
        _timeEvent.OnEnded += HandleEventEnded;

        Debug.Log("Festival System Init Complete");
    }

    private void Update()
    {
        // 時間の監視
        _timeEvent.Update();

        // 採取入力の監視
        if (Input.GetMouseButtonDown(0))
        {
            _eventReward.OnHarvest();
            _eventReward.AddCoffeeBeans(1);
        }
    }

    /// <summary>
    /// イベント開始時の設定およびリセット処理。
    /// </summary>
    private void HandleEventEnded()
    {
        _eventReward.ConfigureRanks(_currentLevel, _levelData);
        _eventReward.OnEventEnded();   
    }
}