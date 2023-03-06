using BaseDefense.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;

namespace BaseDefense.UI
{
    ///<summary>Реализует отображение полосы здоровья</summary>
    public class DisplayHealthPoints : MonoBehaviour
    {
        [SerializeField] private Slider healthPointsBar;
        private TextMeshProUGUI m_healthPointsView;

        public void UpdateView(int currentHealthPoints)
        {
            healthPointsBar.value = currentHealthPoints;
            Assert.IsNotNull(m_healthPointsView);
            m_healthPointsView.text = currentHealthPoints.ToStringWithSeparator();
        }

        public void SetMaxValue(int maxHealthPoints)
        {
            healthPointsBar.maxValue = maxHealthPoints;
        }

        private void Awake()
        {
            m_healthPointsView = healthPointsBar.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}


