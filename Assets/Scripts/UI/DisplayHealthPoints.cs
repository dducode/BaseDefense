using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PlayerCharacter))]
public class DisplayHealthPoints : MonoBehaviour
{
    [SerializeField] Slider HealthPointsBar;
    [SerializeField] TextMeshProUGUI HealthPointsView;
    PlayerCharacter playerCharacter;

    void Awake()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
    }

    void Update()
    {
        HealthPointsBar.maxValue = playerCharacter.MaxHealthPoints;
        int HPValue = (int)playerCharacter.CurrentHealthPoints;
        HealthPointsBar.value = HPValue;
        HealthPointsView.text = HPValue.ToStringWithSeparator();
    }
}
