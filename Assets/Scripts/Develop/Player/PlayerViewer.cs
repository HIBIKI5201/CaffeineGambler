using TMPro;
using UnityEngine;

public class PlayerViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moneyText;

    public void SetCount(int count)
    {
        _moneyText.text = count.ToString();
    }
}
