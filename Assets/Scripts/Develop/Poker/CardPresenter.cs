using TMPro;
using UnityEngine;

public class CardPresenter : MonoBehaviour
{
    [SerializeField] private PokerGameManager _gameManager;
    [SerializeField] private CardViewer _cardViewer;
    [SerializeField] private TextMeshProUGUI _rankLabel;

    private void Start()
    {
        RefreshView();
    }

    public void OnDealButton()
    {
        _gameManager.DealInitialHand();
        RefreshView();
    }

    public void OnEvaluateButton()
    {
        var rank = _gameManager.EvaluateCurrentHand();
        _rankLabel?.SetText(rank.ToString());
    }

    public void RefreshView()
    {
        _cardViewer?.SetCards(_gameManager.CurrentHand);
        _rankLabel?.SetText(_gameManager.EvaluateCurrentHand().ToString());
    }
}
