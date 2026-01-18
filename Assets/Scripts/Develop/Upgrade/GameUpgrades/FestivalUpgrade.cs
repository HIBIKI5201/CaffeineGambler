using Develop.Upgrade;
using UnityEngine;

public class FestivalUpgrade : UpgradeBase
{
    public override string Name => "収穫祭";

    public override int MaxLevel => 3;

    private FestivalInfra _festivalInfra;

    public FestivalUpgrade(FestivalInfra festivalInfra)
    {
        _festivalInfra = festivalInfra;
    }

    public override int GetCost()
    {
        throw new System.NotImplementedException();
    }

    public override void ApplyUpgrade()
    {
        if (Level >= MaxLevel) return;
        base.ApplyUpgrade();
        _festivalInfra.SetFestivalLevel(Level);
    }
}
