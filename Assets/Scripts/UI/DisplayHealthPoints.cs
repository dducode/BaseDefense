using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BaseDefense.Extensions;

namespace BaseDefense.UI
{
    ///<summary>Реализует отображение полосы здоровья</summary>
    public class DisplayHealthPoints : MonoBehaviour
    {
        [SerializeField] Slider healthPointsBar;
        TextMeshProUGUI healthPointsView;

        void Awake()
        {
            healthPointsView = healthPointsBar.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void UpdateView(int currentHealthPoints)
        {
            healthPointsBar.value = currentHealthPoints;
            if (healthPointsView != null)
                healthPointsView.text = currentHealthPoints.ToStringWithSeparator();
        }

        public void SetMaxValue(int maxHealthPoints)
        {
            healthPointsBar.maxValue = maxHealthPoints;
        }
    }
}


