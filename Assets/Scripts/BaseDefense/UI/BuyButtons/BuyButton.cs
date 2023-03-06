using BaseDefense.Currencies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaseDefense.UI.BuyButtons
{
    [RequireComponent(typeof(Button))]
    public abstract class BuyButton : MonoBehaviour
    {
        [SerializeField] private int price;
        [SerializeField] private TextMeshProUGUI priceView;
        private Button m_thisButton;

        protected virtual void Awake()
        {
            m_thisButton = GetComponent<Button>();
        }

        private void Start()
        {
            priceView.text = price.ToString();
        }

        protected void Check(Currency currency)
        {
            m_thisButton.interactable = currency.Value > price;
        }
    }
}