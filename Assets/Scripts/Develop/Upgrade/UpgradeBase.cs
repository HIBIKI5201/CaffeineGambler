using Develop.Upgrade;
using UnityEngine;

public abstract class UpgradeBase : IUpgrade
{
    public string Name => throw new System.NotImplementedException();

    public int Level => throw new System.NotImplementedException();

    public int MaxLevel => throw new System.NotImplementedException();

    public void ApplyUpgrade()
    {
        throw new System.NotImplementedException();
    }

    public int GetCost()
    {
        throw new System.NotImplementedException();
    }
}
