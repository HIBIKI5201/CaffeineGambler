using UnityEngine;

public interface IModifier
{
    /// <summary>
    ///     計算処理を行う。
    /// </summary>
    /// <param name="value">計算する値</param>
    /// <returns>計算結果</returns>
    public float Modify(float value);
}
