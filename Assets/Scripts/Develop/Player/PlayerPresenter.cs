using Develop.Upgrade;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Develop.Player
{
    /// <summary>
    /// 単発クリックとチャージアップグレードによる長押し採取をまとめて処理するプレゼンター。
    /// </summary>
    public class PlayerPresenter : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private PlayerViewer _playerViewer;     // 所持金表示を担当するビュー。
        [SerializeField] private float _baseAmount;              // アップグレード適用前の基本獲得量。

        private CompositeDisposable _disposables;               // 購読の一括破棄用。
        private PlayerData _playerData;                         // 所持金などプレイヤー状態の実体。
        private CollectionCalculation _collectionCalculationPresenter; // 各種修飾子を合算する計算器。
        private ChargeCollectUpgrade _chargeUpgrade;            // 長押し倍率を提供するアップグレード。

        private bool _isPointerHeld;                            // 長押し中かどうか。
        private float _holdBuffer;                              // 長押し中に累積した小数分の報酬。

        /// <summary>
        /// PlayerData とアップグレード群を受け取り、UI バインドと計算器を準備する。
        /// </summary>
        public void Init(PlayerData playerData, List<IUpgrade> upgrades)
        {
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();

            _playerData = playerData;
            _chargeUpgrade = upgrades.OfType<ChargeCollectUpgrade>().FirstOrDefault();

            _playerData.Money
                .Subscribe(money => _playerViewer.SetCount(money))
                .AddTo(_disposables);

            var modifiers = upgrades.OfType<IModifier>().ToList();
            _collectionCalculationPresenter = new CollectionCalculation(modifiers);
        }

        /// <summary>
        /// 通常クリックで基本額を獲得。
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            var amount = _collectionCalculationPresenter.ApplyModifiers(_baseAmount);
            _playerData.AddMoney(Mathf.FloorToInt(amount));
        }

        /// <summary>
        /// 長押し開始。チャージ倍率が 1以下 なら何もしない。
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (GetChargeMultiplier() < 1)
            {
                return;
            }

            _isPointerHeld = true;
        }

        /// <summary>
        /// 長押し終了。溜まっていた端数を精算してリセット。
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            _isPointerHeld = false;
            FlushHoldBuffer();
            _holdBuffer = 0f;
        }

        /// <summary>
        /// 長押ししている間は毎フレーム報酬を積み上げ、整数分だけ即時付与する。
        /// </summary>
        private void Update()
        {
            if (!_isPointerHeld)
            {
                return;
            }

            var multiplier = GetChargeMultiplier();
            if (multiplier <= 0)
            {
                return;
            }

            var perSecond = _collectionCalculationPresenter.ApplyModifiers(_baseAmount * multiplier);
            if (perSecond <= 0f)
            {
                return;
            }

            _holdBuffer += perSecond * Time.deltaTime;

            var payout = Mathf.FloorToInt(_holdBuffer);
            if (payout > 0)
            {
                _holdBuffer -= payout;
                _playerData.AddMoney(payout);
            }
        }

        /// <summary>
        /// 長押しで貯めた端数をまとめて支払う。
        /// </summary>
        private void FlushHoldBuffer()
        {
            var payout = Mathf.FloorToInt(_holdBuffer);
            if (payout > 0)
            {
                _playerData.AddMoney(payout);
            }

            _holdBuffer = 0f;
        }

        /// <summary>
        /// 現在のチャージ倍率（CollctPerSecond）を取得。未取得なら 0。
        /// </summary>
        private int GetChargeMultiplier() =>
            _chargeUpgrade?.CollctPerSecond ?? 0;

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}
