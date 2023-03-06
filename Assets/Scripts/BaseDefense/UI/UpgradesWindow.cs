using System;
using BaseDefense.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BaseDefense.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UpgradesWindow : MonoBehaviour
    {
        [SerializeField] private UpgradeValues upgradeValues;
        [Inject] private PlayerCharacter m_player;

        public void UpgradePlayer(PlayerUpgradesUI playerUpgrades)
        {
            m_player.Upgrade(playerUpgrades.UpgradableProperty);
            upgradeValues.SetValues(m_player);
        }
        
        [Serializable]
        public struct UpgradeValues
        {
            public UpgradeableProperty speed;
            public UpgradeableProperty capacity;
            public UpgradeableProperty maxHealth;

            public void SetValues(PlayerCharacter player)
            {
                speed.textField.text = $"Speed: {player.MaxSpeed}";
                speed.button.interactable = player.IsNotMaxForSpeed;

                maxHealth.textField.text = $"Max health: {player.MaxHealthPoints}";
                maxHealth.button.interactable = player.IsNotMaxForMaxHealth;

                capacity.textField.text = $"Capacity: {player.Capacity}";
                capacity.button.interactable = player.IsNotMaxForCapacity;
            }
            [Serializable]
            public struct UpgradeableProperty
            {
                public TextMeshProUGUI textField;
                public Button button;
            }
        }
    }
}