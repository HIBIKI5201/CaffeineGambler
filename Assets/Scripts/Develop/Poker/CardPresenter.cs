using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using TMPro;
using UnityEngine;

namespace Develop.Poker
{
    /// <summary>
    /// 手札描画と操作を担当するプレゼンター。
    /// </summary>
    public class CardPresenter : MonoBehaviour
    {
        /// <summary>役判定や配札イベントを発行するゲームマネージャー。</summary>
        [SerializeField] private PokerGameManager _gameManager;

        /// <summary>このプレゼンターが担当する手札の所有者。</summary>
        [SerializeField] private PokerGameManager.HandOwner _handOwner = PokerGameManager.HandOwner.Player;

        /// <summary>カード一覧を表示するビュー。</summary>
        [SerializeField] private CardViewer _cardViewer;

        /// <summary>役名を表示するラベル。</summary>
        [SerializeField] private TextMeshProUGUI _rankLabel;

        /// <summary>初期状態で手札を公開するか。</summary>
        [SerializeField] private bool _revealCards = true;

        /// <summary>カードの選択操作を許可するか。</summary>
        [SerializeField] private bool _allowSelection = true;

        /// <summary>開始時に自動で配札するか。</summary>
        [SerializeField] private bool _autoDealOnStart = true;

        /// <summary>伏せ表示時に使用するプレースホルダー文字列。</summary>
        [SerializeField] private string _hiddenRankLabel = "???";
        [SerializeField] private bool _limitRedrawToOnce = true; // true の場合、1 ラウンドにつき 1 回だけ引き直し可能。

        private readonly HashSet<int> _selectedIndices = new();
        private readonly CompositeDisposable _disposables = new();

        private bool _initialRevealState;
        private bool _redrawConsumed;

        public event Action<CardPresenter> RedrawPerformed;

        private void Awake()
        {
            _initialRevealState = _revealCards;

            // HandUpdated を購読し、担当手札だけ UI を再描画する。
            if (_gameManager != null)
            {
                _gameManager.HandUpdated
                    .Where(owner => owner == _handOwner)
                    .Subscribe(_ => RefreshView())
                    .AddTo(_disposables);
            }
        }

        private void OnDestroy() => _disposables.Dispose();

        /// <summary>
        /// 手札の公開／非公開状態を切り替える。
        /// </summary>
        public void SetRevealState(bool revealCards, bool refreshImmediately = true)
        {
            if (_revealCards == revealCards)
            {
                if (refreshImmediately)
                {
                    RefreshView();
                }

                return;
            }

            _revealCards = revealCards;

            if (!_revealCards)
            {
                _selectedIndices.Clear();
            }

            if (refreshImmediately)
            {
                RefreshView();
            }
            else if (!_revealCards)
            {
                UpdateRankLabel(false);
            }
        }

        /// <summary>
        /// Awake 時点の公開状態へ戻すショートカット。
        /// </summary>
        public void ResetRevealState(bool refreshImmediately = true) =>
            SetRevealState(_initialRevealState, refreshImmediately);

        /// <summary>
        /// 現在の手札と役をコンソールへログ出力する。
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
        /// 単独のディールボタンから呼ばれる配札処理。
        /// </summary>
        public void OnDealButton()
        {
            ResetRoundState(refreshImmediately: false);
            _gameManager.DealInitialHand(_handOwner);
            RefreshView();
        }

        /// <summary>
        /// 役表示ボタン対応。伏せ表示なら何もしない。
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
        /// 選択されたカードを引き直す。
        /// </summary>
        public void OnRedrawButton()
        {
            if (!_allowSelection || _selectedIndices.Count == 0 || !TryEnsureHandReady())
            {
                return;
            }

            if (_limitRedrawToOnce && _redrawConsumed)
            {
                Debug.LogWarning($"[{_handOwner}] Redraw is limited to once per round.");
                return;
            }

            _gameManager.ReplaceCards(_handOwner, _selectedIndices);
            _redrawConsumed = true;
            _selectedIndices.Clear();

            RefreshView();
            RedrawPerformed?.Invoke(this);
        }

        /// <summary>
        /// UI 上のカード選択トグルを処理。
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
        /// CardViewer と役ラベルを最新の手札で描画する。
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
        /// マネージャー参照や手札の存在を検証する共通ヘルパー。
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
        /// 選択状態と公開状態に応じてビューへ反映する。
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
        /// 役ラベルを公開／非公開状態に合わせて更新する。
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

        /// <summary>
        /// ラウンド開始時の内部状態リセット。賭け直し可能フラグもここで復帰する。
        /// </summary>
        public void ResetRoundState(bool refreshImmediately = true)
        {
            _redrawConsumed = false;
            _selectedIndices.Clear();

            if (refreshImmediately)
            {
                RefreshView();
            }
        }
    }
}
