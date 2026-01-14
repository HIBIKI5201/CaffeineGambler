using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Develop.Poker
{
    /// <summary>
    /// 指定された手札をテキストおよびボタンで表示するビュークラス。
    /// 非公開表示や選択ハイライトの切り替えにも対応する。
    /// </summary>
    public class CardViewer : MonoBehaviour
    {
        /// <summary>各カードの内容を表示する TextMeshPro ラベル群。</summary>
        [SerializeField] private List<TextMeshProUGUI> _cardTexts;

        /// <summary>カードごとのボタン（任意）。存在する場合は色・操作可否を制御する。</summary>
        [SerializeField] private List<Button> _cardButtons;

        /// <summary>未選択時の表示色。</summary>
        [SerializeField] private Color _normalColor = Color.white;

        /// <summary>選択状態の表示色。</summary>
        [SerializeField] private Color _selectedColor = Color.yellow;

        /// <summary>非公開表示時の色。</summary>
        [SerializeField] private Color _hiddenColor = Color.gray;

        /// <summary>カードを伏せて表示する際のテキスト。</summary>
        [SerializeField] private string _hiddenCardLabel = "???";

        /// <summary>カードを隠している間、ボタン操作を無効化するか。</summary>
        [SerializeField] private bool _disableButtonsWhenHidden = true;

        /// <summary>
        /// 手札情報を受け取り、公開／非公開・選択状態に応じた表示へ反映する。
        /// </summary>
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

        /// <summary>
        /// カード情報を「スート ランク」形式の文字列へ変換する。
        /// </summary>
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