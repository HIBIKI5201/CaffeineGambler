using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardViewer : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _cardTexts;

    public void SetCards(IReadOnlyList<Card> cards)
    {
        for (var i = 0; i < _cardTexts.Count; i++)
        {
            if (cards != null && i < cards.Count)
            {
                _cardTexts[i].text = FormatCard(cards[i]);
            }
            else
            {
                _cardTexts[i].text = string.Empty;
            }
        }
    }

    public static string FormatCard(Card card)
    {
        if (card.IsJoker)
        {
            return "Joker";
        }

        var rankLabel = card.Rank switch
        {
            11 => "J",
            12 => "Q",
            13 => "K",
            14 => "A",
            _ => card.Rank.ToString()
        };

        return $"{card.Suit} {rankLabel}";
    }
}