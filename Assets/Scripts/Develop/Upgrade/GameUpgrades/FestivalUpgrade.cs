using Develop.Upgrade;
using UnityEngine;

public class FestivalUpgrade : UpgradeBase
{
    public override string Name => "収穫祭";

    public override int MaxLevel => 3;

    public override int GetCost()
    {
        throw new System.NotImplementedException();
    }
}
