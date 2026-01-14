using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CatdViewer : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _cardTexts;

    public void SetCardType(string cardType)
    {
        for(int i=0; i < _cardTexts.Count; i++)
        {
            if (i < cardType.Length)
            {
                _cardTexts[i].text = cardType[i].ToString();
            }
            else
            {
                _cardTexts[i].text = string.Empty;
            }
        }
    }
}
