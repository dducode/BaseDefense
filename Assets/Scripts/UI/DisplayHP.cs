using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayHP : MonoBehaviour
{
    [SerializeField] Slider HPBar;
    [SerializeField] TextMeshProUGUI HP;
    PlayerCharacter playerCharacter;

    void Start()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
        HPBar.maxValue = playerCharacter.MaxHealthPoint;
    }

    void Update()
    {
        int HPValue = (int)playerCharacter.CurrentHP;
        HPBar.value = HPValue;
        HP.text = HPValue.ToString();
    }
}
