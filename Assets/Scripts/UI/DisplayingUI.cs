using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using BroadcastMessages;

public class DisplayingUI : MonoBehaviour
{
    ///<summary>Отображает количество денег у игрока</summary>
    [SerializeField, Tooltip("Отображает количество денег у игрока")] 
    TextMeshProUGUI moneys;

    ///<summary>Отображает количество кристаллов у игрока</summary>
    [SerializeField, Tooltip("Отображает количество кристаллов у игрока")] 
    TextMeshProUGUI gems;

    ///<summary>Окно, выводимое при смерти игрока</summary>
    [SerializeField, Tooltip("Окно, выводимое при смерти игрока")] 
    Canvas deathWindow;

    ///<summary>Рамка для выбранного игроком оружия</summary>
    [SerializeField, Tooltip("Рамка для выбранного игроком оружия")] 
    RectTransform frame;

    [SerializeField] Canvas shopWindow;
    [SerializeField] Canvas playerUpgradesWindow;
    [SerializeField] UpgradeValues upgradeValues;
    [Inject] PlayerCharacter player;
    [Inject] Upgrades upgrades;

    void Start()
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

    [Listener(MessageType.DEATH_PLAYER)]
    public void DisplayDeathWindow() => StartCoroutine(Await());
    IEnumerator Await()
    {
        yield return new WaitForSeconds(2);
        deathWindow.enabled = true;
    }
    public void Restart()
    {
        Messenger.SendMessage(MessageType.RESTART);
        deathWindow.enabled = false;
    }
    public void SelectGun(GunSlot slot)
    {
        frame.localPosition = slot.transform.localPosition;
        player.SelectGun(slot.GunName);
    }
    public void OpenShop() => shopWindow.enabled = true;
    public void CloseShop() => shopWindow.enabled = false;

    public void UpgradePlayer(PlayerUpgradesUI playerUpgrades)
    {
        upgrades.Upgrade(playerUpgrades.UpgradeType);
        upgradeValues.SetValues(upgrades);
    }
    public void OpenUpgrades()
    {
        playerUpgradesWindow.enabled = true;
        upgradeValues.SetValues(upgrades);
    }
    public void CloseUpgrades() => playerUpgradesWindow.enabled = false;

    [System.Serializable]
    public struct UpgradeValues
    {
        public UpgradeableProperty speed;
        public UpgradeableProperty capacity;
        public UpgradeableProperty maxHealth;

        public void SetValues(Upgrades upgrades)
        {
            for (int i = 0; i < upgrades.Targets.Length; i++)
            {
                if (upgrades.Targets[i] is PlayerCharacter player)
                {
                    speed.textField.text = $"Speed: {player.MaxSpeed}";
                    speed.button.interactable = player.MaxSpeed < upgrades.Speed.maxValue;
                    maxHealth.textField.text = $"Max health: {player.MaxHealthPoints}";
                    maxHealth.button.interactable = player.MaxHealthPoints < upgrades.MaxHealth.maxValue;
                }
                else if (upgrades.Targets[i] is ItemCollecting itemCollecting)
                {
                    capacity.textField.text = $"Capacity: {itemCollecting.Capacity}";
                    capacity.button.interactable = itemCollecting.Capacity < upgrades.Capacity.maxValue;
                }
            }
        }
        [System.Serializable]
        public struct UpgradeableProperty
        {
            public TextMeshProUGUI textField;
            public Button button;
        }
    }
}

