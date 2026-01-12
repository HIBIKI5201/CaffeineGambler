
namespace Develop.Save
{
    /// <summary>
    /// ゲームのセーブデータを管理するクラス。
    /// </summary>
    public class PlayerStatusSaveData 
    {
        /// <summary>
        /// セーブデータを生成する。
        /// </summary>
        /// <param name="money"> 所持金 </param>   
        public PlayerStatusSaveData(int money)
        {
            Money = money;
        }

        public int Money { get; private set; }
    }
}
