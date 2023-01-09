using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PlayerCharacter))]
public class DisplayHealthPoints : MonoBehaviour
{
    [SerializeField] Slider HealthPointsBar;
    [SerializeField] TextMeshProUGUI HealthPointsView;
    PlayerCharacter playerCharacter;

    void Start()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
        HealthPointsBar.maxValue = playerCharacter.MaxHealthPoints;
    }

    void Update()
    {
        int HPValue = (int)playerCharacter.CurrentHealthPoints;
        HealthPointsBar.value = HPValue;
        HealthPointsView.text = HPValue.ToStringWithSeparator();
    }
}
