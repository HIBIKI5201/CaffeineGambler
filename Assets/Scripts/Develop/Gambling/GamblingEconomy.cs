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
        public GamblingEconomy(PlayerData playerData)
        {
            _playerData = playerData ?? throw new ArgumentNullException(nameof(playerData));
        }

        private readonly PlayerData _playerData;

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
        /// </summary>
        /// <param name="betAmount">賭けた金額</param>
        /// <param name="result">勝負の結果</param>
        public void Payout(int betAmount, GamblingResult result)
        {
            int returnAmount = 0;

            // 結果に応じた倍率設定
            // Loseは0倍なので処理なし
            switch (result)
            {
                case GamblingResult.Draw:
                    returnAmount = betAmount; // 返金
                    break;
                case GamblingResult.Win:
                    returnAmount = betAmount * 2; // 2倍
                    break;
                case GamblingResult.BlackJack:
                    // 通常2.5倍だが、整数計算のため3倍とする（ボーナス要素）
                    returnAmount = betAmount * 3; 
                    break;
            }

            if (returnAmount > 0)
            {
                _playerData.AddMoney(returnAmount);
            }
        }
    }
}
