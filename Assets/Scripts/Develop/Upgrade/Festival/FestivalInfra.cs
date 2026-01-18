using Develop.Upgrade.Festival;
using Domain;
using UnityEngine;

public class FestivalInfra
{
    [Header("Event Settings")]
    [SerializeField] private int _eventDuration = 5; // イベント時間（秒）
    [SerializeField] private int _threshold = 3;     // ボーナス閾値
    [SerializeField] private int _multiplier = 2;    // ボーナス倍率

    private TimedEvent _timeEvent;
    private EventReward _eventReward;


    private void Start()
    {
        IClock clock = new SystemClock();
        IRandom random = new SystemRandom();

        // ドメインオブジェクトの生成
        _timeEvent = new TimedEvent(clock, random, _eventDuration);
        _eventReward = new EventReward(_timeEvent, _threshold, _multiplier);

        _timeEvent.OnStarted += _eventReward.OnEventStarted;
        _timeEvent.OnEnded += _eventReward.OnEventEnded;
        Debug.Log("Festival System Init Complete");
    }
    private void Dispose()
    {
        _timeEvent.OnStarted -= _eventReward.OnEventStarted;
        _timeEvent.OnEnded -= _eventReward.OnEventEnded;
    }

    private void Update()
    {
        _timeEvent.Update();

        if (Input.GetMouseButtonDown(0))
        {
            _eventReward.OnHarvest();
            _eventReward.AddCoffeeBeans(1);

        }


    }
}
