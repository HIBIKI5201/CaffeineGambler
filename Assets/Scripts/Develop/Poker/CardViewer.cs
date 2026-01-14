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

        public void SetCards(IReadOnlyList<Card> cards, IReadOnlyCollection<int> selectedIndices = null)
        {
            ISet<int> selected = selectedIndices as ISet<int> ?? selectedIndices?.ToHashSet();

            for (var i = 0; i < _cardTexts.Count; i++)
            {
                var text = _cardTexts[i];

                if (cards != null && i < cards.Count)
                {
                    text.text = FormatCard(cards[i]);
                }
                else
                {
                    text.text = string.Empty;
                }

                var isSelected = selected != null && selected.Contains(i);

                if (_cardButtons != null && i < _cardButtons.Count && _cardButtons[i] != null)
                {
                    _cardButtons[i].image.color = isSelected ? _selectedColor : _normalColor;
                }
                else
                {
                    text.color = isSelected ? _selectedColor : _normalColor;
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