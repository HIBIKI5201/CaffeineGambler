using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Develop.Poker
{
    /// <summary>
    /// PokerGameManager のプレイヤー手札を対象に、UI からの操作とビュー更新を仲介するプレゼンター。
    /// </summary>
    public class CardPresenter : MonoBehaviour
    {
        /// <summary>ゲーム全体の状態を管理するコンポーネント。</summary>
        [SerializeField] private PokerGameManager _gameManager;

        /// <summary>カード一覧を描画するビュー。</summary>
        [SerializeField] private CardViewer _cardViewer;

        /// <summary>役の名称を表示するラベル。</summary>
        [SerializeField] private TextMeshProUGUI _rankLabel;

        /// <summary>現在選択中のカードインデックス。</summary>
        private readonly HashSet<int> _selectedIndices = new();

        /// <summary>
        /// UI ボタンから呼び出し、現在の役と手札をデバッグログへ出力する。
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
        /// ディールボタン用ハンドラー。手札を配り直し、ビューを更新する。
        /// </summary>
        public void OnDealButton()
        {
            _selectedIndices.Clear();
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
        /// 選択済みカードのみを引き直す。
        /// </summary>
        public void OnRedrawButton()
        {
            if (!TryEnsureHandReady() || _selectedIndices.Count == 0)
            {
                return;
            }

            _gameManager.ReplaceCards(_selectedIndices);
            _selectedIndices.Clear();
            RefreshView();
        }

        /// <summary>
        /// カード個別ボタンから呼び出し、指定インデックスの選択状態をトグルする。
        /// </summary>
        public void ToggleCardSelection(int index)
        {
            if (!TryEnsureHandReady() || index < 0 || index >= _gameManager.CurrentHand.Count)
            {
                return;
            }

            if (_selectedIndices.Contains(index))
            {
                _selectedIndices.Remove(index);
            }
            else
            {
                _selectedIndices.Add(index);
            }

            UpdateSelectionVisuals();
        }

        /// <summary>
        /// 手札と役をビューへ反映する。
        /// </summary>
        public void RefreshView()
        {
            if (!TryEnsureHandReady())
            {
                _cardViewer?.SetCards(null);
                _rankLabel?.SetText("-");
                _selectedIndices.Clear();
                return;
            }

            var currentHand = _gameManager.CurrentHand;
            _cardViewer?.SetCards(currentHand, _selectedIndices);
            _rankLabel?.SetText(_gameManager.EvaluateCurrentHand().ToString());
        }

        private void Start()
        {
            _gameManager.DealInitialHand();
            RefreshView();
        }

        /// <summary>
        /// 手札が表示・操作可能かどうかを検証する。
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

        /// <summary>
        /// 選択状態に応じてビューを再描画する。
        /// </summary>
        private void UpdateSelectionVisuals()
        {
            if (!TryEnsureHandReady())
            {
                return;
            }

            _cardViewer?.SetCards(_gameManager.CurrentHand, _selectedIndices);
        }
    }
}
