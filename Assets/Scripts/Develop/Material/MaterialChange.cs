using UnityEngine;

/// <summary>
/// URP/Lit 系マテリアルを使って、元マテリアルは共有のまま
/// 個別に色を変えて表示できるコンポーネント。
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class MaterialChange : MonoBehaviour
{
    [Tooltip("元のマテリアル（Shared Material）")]
    [SerializeField] private Material baseMaterial;

    [Tooltip("このオブジェクトで適用する色")]
    [SerializeField] private Color variantColor = Color.white;

    private MeshRenderer _renderer;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _renderer.material = baseMaterial; // インスタンス化して個別設定可能
        _propertyBlock = new MaterialPropertyBlock();

        ApplyVariantColor();
    }

    /// <summary>
    /// 現在の variantColor を Renderer に適用する
    /// </summary>
    public void ApplyVariantColor()
    {
        _renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_BaseColor", variantColor);
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    /// <summary>
    /// ランタイムで色を変更する
    /// </summary>
    /// <param name="newColor">新しい色</param>
    public void SetVariantColor(Color newColor)
    {
        variantColor = newColor;
        ApplyVariantColor();
    }

    /// <summary>
    /// Inspector 右クリック用メニュー
    /// </summary>
    [ContextMenu("Apply Variant Color")]
    public void ApplyVariantColorInspector()
    {
        ApplyVariantColor();
    }
}
