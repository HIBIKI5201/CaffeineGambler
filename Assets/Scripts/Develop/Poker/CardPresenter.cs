using System.Linq;
using TMPro;
using UnityEngine;

public class CardPresenter : MonoBehaviour
{
    [SerializeField] private PokerGameManager _gameManager;
    [SerializeField] private CardViewer _cardViewer;
    [SerializeField] private TextMeshProUGUI _rankLabel;

    /// <summary>
    /// UI ボタンから呼び出し、現在の役をログに出力する。
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

    public void OnDealButton()
    {
        _gameManager.DealInitialHand();
        RefreshView();
    }

    public void OnEvaluateButton()
    {
        if (!TryEnsureHandReady())
        {
            return;
        }

        var rank = _gameManager.EvaluateCurrentHand();
        _rankLabel?.SetText(rank.ToString());
    }

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
