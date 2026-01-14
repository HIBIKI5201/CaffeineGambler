using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// PokerGameManager の状態を取得し、ビュー更新やログ出力を仲介するプレゼンター。
/// </summary>
public class CardPresenter : MonoBehaviour
{
    [SerializeField] private PokerGameManager _gameManager;
    [SerializeField] private CardViewer _cardViewer;
    /// <summary>役表示用ラベル。</summary>
    [SerializeField] private TextMeshProUGUI _rankLabel;

    /// <summary>
    /// UI ボタンから呼び出し、現在の役と手札をログに出力する。
    /// </summary>
    public void LogCurrentHandRank()
    {
        if (!TryEnsureHandReady())
        {
            return;
        }

        var rank = _gameManager.EvaluateCurrentHand();
        var cardsText = string.Join(", ", _gameManager.CurrentHand.Select(CardViewer.FormatCard));
        Debug.Log($"[Poker] Rank: {rank} | Cards: {cardsText}");
    }

    /// <summary>
    /// ディールボタン用ハンドラー。手札を配り直しビューを更新する。
    /// </summary>
    public void OnDealButton()
    {
        _gameManager.DealInitialHand();
        RefreshView();
    }

    /// <summary>
    /// 役判定ボタン用ハンドラー。最新の役を UI に表示する。
    /// </summary>
    public void OnEvaluateButton()
    {
        if (!TryEnsureHandReady())
        {
            return;
        }

        var rank = _gameManager.EvaluateCurrentHand();
        _rankLabel?.SetText(rank.ToString());
    }

    /// <summary>
    /// 現在の手札と役をビューへ反映する。
    /// </summary>
    public void RefreshView()
    {
        if (!TryEnsureHandReady())
        {
            _cardViewer?.SetCards(null);
            _rankLabel?.SetText("-");
            return;
        }

        var currentHand = _gameManager.CurrentHand;
        _cardViewer?.SetCards(currentHand);
        _rankLabel?.SetText(_gameManager.EvaluateCurrentHand().ToString());
    }

    private void Start()
    {
        RefreshView();
    }

    /// <summary>
    /// 手札が表示可能な状態にあるか検証する。
    /// </summary>
    private bool TryEnsureHandReady()
    {
        if (_gameManager == null)
        {
            Debug.LogWarning("GameManager reference is missing.");
            return false;
        }

        var hand = _gameManager.CurrentHand;
        if (hand == null || hand.Count == 0)
        {
            Debug.LogWarning("Hand is empty. Call DealInitialHand before logging.");
            return false;
        }

        return true;
    }
}
