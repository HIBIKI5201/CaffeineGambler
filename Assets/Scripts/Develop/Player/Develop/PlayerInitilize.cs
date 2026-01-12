using Develop.Player;
using UnityEngine;

public class PlayerInitilize : MonoBehaviour
{

    private void Start()
    {
        PlayerData playerData = new PlayerData(1000);
        PlayerData playerData1 = new PlayerData(-1);
    }
}
