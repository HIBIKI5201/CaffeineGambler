using TMPro;
using UnityEngine;

namespace Develop.Player
{
    public class PlayerViewer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _moneyText;

        public void SetCount(int count)
        {
            _moneyText.text = count.ToString();
        }
    }
}
