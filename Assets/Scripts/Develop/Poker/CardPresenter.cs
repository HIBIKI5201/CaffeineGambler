using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Develop.Poker
{
    /// <summary>
    /// 指定ハンド（プレイヤー／敵）の操作と表示更新を司るプレゼンター。
    /// </summary>
    public class CardPresenter : MonoBehaviour
    {
        /// <summary>ゲーム全体の状態を提供するマネージャー。</summary>
        [SerializeField] private PokerGameManager _gameManager;

        /// <summary>このプレゼンターが扱う手札の所有者。</summary>
        [SerializeField] private PokerGameManager.HandOwner _handOwner = PokerGameManager.HandOwner.Player;

        /// <summary>カード一覧を表示するビュー。</summary>
        [SerializeField] private CardViewer _cardViewer;

        /// <summary>役名を表示する TextMeshPro ラベル。</summary>
        [SerializeField] private TextMeshProUGUI _rankLabel;

        /// <summary>カード内容を公開するか（敵側なら false）。</summary>
        [SerializeField] private bool _revealCards = true;

        /// <summary>カード選択および引き直し操作を許可するか。</summary>
        [SerializeField] private bool _allowSelection = true;

        /// <summary>開始時に自動で配札するか。</summary>
        [SerializeField] private bool _autoDealOnStart = true;

        /// <summary>未公開時に役ラベルへ表示する文字列。</summary>
        [SerializeField] private string _hiddenRankLabel = "???";

        /// <summary>UI 上で選択状態にあるカードのインデックス集合。</summary>
        private readonly HashSet<int> _selectedIndices = new();

        /// <summary>
        /// 現在の役と手札をデバッグログへ出力する（非公開モードでは警告のみ）。
        /// </summary>
        public void LogCurrentHandRank()
        {
            if (!_revealCards)
            {
                Debug.LogWarning($"[{_handOwner}] Hand is hidden. Enable reveal to log details.");
                return;
            }

            if (!TryEnsureHandReady())
            {
                return;
            }

            var rank = _gameManager.EvaluateHand(_handOwner);
            var cardsText = string.Join(", ", _gameManager.GetHand(_handOwner).Select(CardViewer.FormatCard));
            Debug.Log($"[Poker][{_handOwner}] Rank: {rank} | Cards: {cardsText}");
        }

        /// <summary>
        /// ディールボタン経由で呼び出され、対象手札を配り直す。
        /// </summary>
        public void OnDealButton()
        {
            _selectedIndices.Clear();
            _gameManager.DealInitialHand(_handOwner);
            RefreshView();
        }

        /// <summary>
        /// 役判定ボタンで呼び出され、役表示ラベルを更新する。
        /// </summary>
        public void OnEvaluateButton()
        {
            if (!_revealCards)
            {
                UpdateRankLabel(false);
                return;
            }

            if (!TryEnsureHandReady())
            {
                return;
            }

            UpdateRankLabel(true);
        }

        /// <summary>
        /// 選択済みカードのみを置き換える（選択不可モードでは無視）。
        /// </summary>
        public void OnRedrawButton()
        {
            if (!_allowSelection || _selectedIndices.Count == 0 || !TryEnsureHandReady())
            {
                return;
            }

            _gameManager.ReplaceCards(_handOwner, _selectedIndices);
            _selectedIndices.Clear();
            RefreshView();
        }

        /// <summary>
        /// 個別カード UI から呼び出し、選択状態をトグルする。
        /// </summary>
        public void ToggleCardSelection(int index)
        {
            if (!_allowSelection || !_revealCards || !TryEnsureHandReady())
            {
                return;
            }

            var hand = _gameManager.GetHand(_handOwner);
            if (index < 0 || index >= hand.Count)
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
        /// 手札と役表示をビューへ反映する（敵側は伏せ札＆役ラベル非公開）。
        /// </summary>
        public void RefreshView()
        {
            if (!TryEnsureHandReady(allowEmpty: true))
            {
                _cardViewer?.SetCards(null);
                UpdateRankLabel(false);
                _selectedIndices.Clear();
                return;
            }

            var hand = _gameManager.GetHand(_handOwner);
            if (hand == null || hand.Count == 0)
            {
                _cardViewer?.SetCards(null);
                UpdateRankLabel(false);
                _selectedIndices.Clear();
                return;
            }

            if (!_allowSelection)
            {
                _selectedIndices.Clear();
            }

            var selection = (_allowSelection && _revealCards) ? _selectedIndices : null;
            _cardViewer?.SetCards(hand, selection, _revealCards);
            UpdateRankLabel(_revealCards);
        }

        private void Start()
        {
            if (_autoDealOnStart && _gameManager != null)
            {
                _gameManager.DealInitialHand(_handOwner);
            }

            RefreshView();
        }

        /// <summary>
        /// 手札参照が有効か、枚数が0ではないかを検証する。
        /// </summary>
        private bool TryEnsureHandReady(bool allowEmpty = false)
        {
            if (_gameManager == null)
            {
                Debug.LogWarning("GameManager reference is missing.");
                return false;
            }

            var hand = _gameManager.GetHand(_handOwner);
            if (hand == null)
            {
                Debug.LogWarning($"[{_handOwner}] Hand reference is missing.");
                return false;
            }

            if (!allowEmpty && hand.Count == 0)
            {
                Debug.LogWarning($"[{_handOwner}] Hand is empty. Deal cards first.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 選択状態に合わせて CardViewer を再描画する。
        /// </summary>
        private void UpdateSelectionVisuals()
        {
            if (!TryEnsureHandReady())
            {
                return;
            }

            var selection = (_allowSelection && _revealCards) ? _selectedIndices : null;
            _cardViewer?.SetCards(_gameManager.GetHand(_handOwner), selection, _revealCards);
        }

        /// <summary>
        /// 役表示ラベルを公開／非公開の設定に応じて更新する。
        /// </summary>
        private void UpdateRankLabel(bool revealed)
        {
            if (_rankLabel == null)
            {
                return;
            }

            if (!revealed)
            {
                _rankLabel.SetText(_hiddenRankLabel);
                return;
            }

            var rank = _gameManager.EvaluateHand(_handOwner);
            _rankLabel.SetText(rank.ToString());
        }
    }
}
