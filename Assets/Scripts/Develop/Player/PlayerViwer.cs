using TMPro;
using UnityEngine;

public class PlayerViwer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moneyText;

    public void SetCount(int count)
    {
        _moneyText.text = count.ToString();
    }
}
