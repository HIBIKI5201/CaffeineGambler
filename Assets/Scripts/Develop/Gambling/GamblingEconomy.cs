using Develop.Player;
using System;

namespace Develop.Gambling
{
    /// <summary>
    /// ギャンブルにおける経済処理（賭け金の支払い、配当の受け取り）を担当するクラス。
    /// 経済反映層に位置する。
    /// </summary>
    public class GamblingEconomy
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="playerData">プレイヤーデータへの参照</param>
        /// <param name="settings">経済バランス設定</param>
        public GamblingEconomy(PlayerData playerData, GamblingEconomySettings settings)
        {
            _playerData = playerData ?? throw new ArgumentNullException(nameof(playerData));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// 賭け金を支払う。
        /// </summary>
        /// <param name="amount">賭ける金額</param>
        /// <returns>支払いに成功した場合は true、資金不足の場合は false</returns>
        public bool TryBet(int amount)
        {
            if (amount <= 0) return false;
            return _playerData.TrySpendMoney(amount);
        }

        /// <summary>
        /// 勝負結果に基づいて配当を計算し、プレイヤーに付与する。
        /// 設定された倍率に基づいて計算を行うことで、柔軟なバランス調整を可能にする。
        /// </summary>
        /// <param name="betAmount">賭けた金額</param>
        /// <param name="result">勝負の結果</param>
        public void Payout(int betAmount, GamblingResult result)
        {
            int returnAmount = 0;

            // ScriptableObjectから倍率を取得して計算
            switch (result)
            {
                case GamblingResult.Draw:
                    returnAmount = betAmount * _settings.DrawMultiplier; 
                    break;
                case GamblingResult.Win:
                    returnAmount = betAmount * _settings.WinMultiplier;
                    break;
                case GamblingResult.BlackJack:
                    returnAmount = betAmount * _settings.BlackJackMultiplier; 
                    break;
                case GamblingResult.Lose:
                default:
                    returnAmount = 0;
                    break;
            }

            if (returnAmount > 0)
            {
                _playerData.AddMoney(returnAmount);
            }
        }

        private readonly PlayerData _playerData;
        private readonly GamblingEconomySettings _settings;
    }
}