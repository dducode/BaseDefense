using UnityEngine;
using UnityEngine.UI;
using TMPro;

///<summary>Реализует отображение полосы здоровья игрока</summary>
public class DisplayHealthPoints : MonoBehaviour
{
    [SerializeField] Slider HealthPointsBar;
    TextMeshProUGUI HealthPointsView;

    void Awake()
    {
        HealthPointsView = HealthPointsBar.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateView(int currentHealthPoints)
    {
        HealthPointsBar.value = currentHealthPoints;
        HealthPointsView.text = currentHealthPoints.ToStringWithSeparator();
    }

    public void SetMaxValue(int maxHealthPoints)
    {
        HealthPointsBar.maxValue = maxHealthPoints;
    }
}
