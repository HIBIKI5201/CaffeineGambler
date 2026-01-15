using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Develop.Poker
{
    public class CardViewer : MonoBehaviour
    {
        [SerializeField] private List<TextMeshProUGUI> _cardTexts;
        [SerializeField] private List<Button> _cardButtons;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _selectedColor = Color.yellow;
        [SerializeField] private Color _hiddenColor= Color.gray;
        [SerializeField] private string _hiddenCardLabel = "???";
        [SerializeField] private bool _disableButtonsWhenHidden = true;

        public void SetCards(IReadOnlyList<Card> cards, IReadOnlyCollection<int> selectedIndices = null, bool revealCards = true)
        {
            ISet<int> selected = selectedIndices as ISet<int> ?? selectedIndices?.ToHashSet();

            for (var i = 0; i < _cardTexts.Count; i++)
            {
                var text = _cardTexts[i];
                var hasCard = cards != null && i < cards.Count;

                text.text = hasCard
                    ? (revealCards ? FormatCard(cards[i]) : _hiddenCardLabel)
                    : string.Empty;

                var isSelected = revealCards && selected != null && selected.Contains(i);
                var highlightColor = isSelected ? _selectedColor : _normalColor;
                var visualColor = hasCard
                    ? (revealCards ? highlightColor : _hiddenColor)
                    : _normalColor;

                text.color = visualColor;

                if (_cardButtons != null && i < _cardButtons.Count && _cardButtons[i] != null)
                {
                    var button = _cardButtons[i];
                    button.image.color = visualColor;
                    if (_disableButtonsWhenHidden)
                    {
                        button.interactable = revealCards && hasCard;
                    }
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
}