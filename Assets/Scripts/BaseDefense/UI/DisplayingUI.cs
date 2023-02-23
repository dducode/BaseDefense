using System;
using System.Collections;
using BaseDefense.Broadcast_messages;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using BroadcastMessages;
using BaseDefense.Characters;
using BaseDefense.Extensions;

namespace BaseDefense.UI
{
    public class DisplayingUI : MonoBehaviour
    {
        ///<summary>Отображает количество денег у игрока</summary>
        [SerializeField, Tooltip("Отображает количество денег у игрока")]
        private TextMeshProUGUI moneys;

        ///<summary>Отображает количество кристаллов у игрока</summary>
        [SerializeField, Tooltip("Отображает количество кристаллов у игрока")]
        private TextMeshProUGUI gems;

        ///<summary>Окно, выводимое при смерти игрока</summary>
        [SerializeField, Tooltip("Окно, выводимое при смерти игрока")]
        private Canvas deathWindow;

        ///<summary>Рамка для выбранного игроком оружия</summary>
        [SerializeField, Tooltip("Рамка для выбранного игроком оружия")]
        private RectTransform frame;

        [SerializeField] private Canvas shopWindow;
        [SerializeField] private Canvas playerUpgradesWindow;
        [SerializeField] private UpgradeValues upgradeValues;

        private void Start()
        {
            deathWindow.enabled = false;
            shopWindow.enabled = false;
            playerUpgradesWindow.enabled = false;
        }
        
        public void UpdateUI(int moneys, int gems)
        {
            this.moneys.text = moneys.ToStringWithSeparator();
            this.gems.text = gems.ToStringWithSeparator();
        }
        
        public void Restart()
        {
            Messenger.SendMessage(MessageType.RESTART);
            deathWindow.enabled = false;
        }
        
        [Inject] private PlayerCharacter m_player;
        
        public void SelectGun(GunSlot slot)
        {
            frame.localPosition = slot.transform.localPosition;
            m_player.SelectGun(slot.GunName);
        }
        public void OpenShop() => shopWindow.enabled = true;
        public void CloseShop() => shopWindow.enabled = false;

        public void UpgradePlayer(PlayerUpgradesUI playerUpgrades)
        {
            m_player.Upgrade(playerUpgrades.UpgradableProperty);
            upgradeValues.SetValues(m_player);
        }
        public void OpenUpgrades()
        {
            playerUpgradesWindow.enabled = true;
            upgradeValues.SetValues(m_player);
        }
        public void CloseUpgrades() => playerUpgradesWindow.enabled = false;

        private void OnEnable() => Messenger.AddListener(MessageType.DEATH_PLAYER, DisplayDeathWindow);
        private void OnDisable() => Messenger.RemoveListener(MessageType.DEATH_PLAYER, DisplayDeathWindow);

        private void DisplayDeathWindow() => StartCoroutine(Await());
        private IEnumerator Await()
        {
            yield return new WaitForSeconds(2);
            deathWindow.enabled = true;
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



