using Develop.Player;
using System;

namespace Develop.Gambling
{
    /// <summary>
    ///     ギャンブルにおける経済処理（賭け金の支払い、配当の受け取り）を担当するクラス。
    ///     経済反映層に位置する。
    /// </summary>
    public class GamblingEconomy
    {
        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        /// <param name="playerData">プレイヤーデータへの参照</param>
        /// <param name="settings">経済バランス設定</param>
        public GamblingEconomy(PlayerData playerData, GamblingEconomySettings settings)
        {
            // プレイヤーデータの操作権限を保持するため
            _playerData = playerData ?? throw new ArgumentNullException(nameof(playerData));

            // 配当ルールを保持するため
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        ///     賭け金を支払う。
        /// </summary>
        /// <param name="amount">賭ける金額</param>
        /// <returns>支払いに成功した場合は true、資金不足の場合は false</returns>
        public bool TryBet(int amount)
        {
            // 不正な金額でのベットを弾くため
            if (amount <= 0) return false;

            // プレイヤーの所持金を実際に消費させるため
            return _playerData.TrySpendMoney(amount);
        }

        /// <summary>
        ///     勝負結果に基づいて配当を計算し、プレイヤーに付与する。
        ///     設定された倍率に基づいて計算を行うことで、柔軟なバランス調整を可能にする。
        /// </summary>
        /// <param name="betAmount">賭けた金額</param>
        /// <param name="result">勝負の結果</param>
        public void Payout(int betAmount, GamblingResult result)
        {
            int returnAmount = 0;

            // ゲーム結果に応じた倍率を適用して返金・賞金額を計算するため
            switch (result)
            {
                case GamblingResult.Draw:
                    // 引き分け時に賭け金を戻すため
                    returnAmount = betAmount * _settings.DrawMultiplier; 
                    break;
                case GamblingResult.Win:
                    // 通常勝利の配当を与えるため
                    returnAmount = betAmount * _settings.WinMultiplier;
                    break;
                case GamblingResult.BlackJack:
                    // ブラックジャックによる特別配当を与えるため
                    returnAmount = betAmount * _settings.BlackJackMultiplier; 
                    break;
                case GamblingResult.Lose:
                default:
                    // 敗北時は配当を0にするため
                    returnAmount = 0;
                    break;
            }

            // 計算された金額が0より大きい場合のみ、プレイヤーの所持金を増やすため
            if (returnAmount > 0)
            {
                _playerData.AddMoney(returnAmount);
            }
        }

        private readonly PlayerData _playerData;
        private readonly GamblingEconomySettings _settings;
    }
}
