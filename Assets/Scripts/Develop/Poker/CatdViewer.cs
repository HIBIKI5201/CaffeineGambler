using TMPro;
using UnityEngine;

public class CatdViewer : MonoBehaviour
{
    [SerializeField] private TextMeshPro _cardType;

    public void SetCardType(string cardType)
    {
        _cardType.text = cardType;
    }
}
