using Develop.Gambling.Develop;
using UnityEngine;
using TMPro;

public class UiPresenter : MonoBehaviour
{

    [SerializeField] private TMP_Text _moneyText;




    public void UpdateMoneyDisplay(int money)
    {
        if (_moneyText != null)
        {
            _moneyText.text = $"Money: {money}";
        }
    }
}
