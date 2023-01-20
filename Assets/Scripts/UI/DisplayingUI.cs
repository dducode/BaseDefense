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
    Shop currentShop;

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
        player.SelectGun(currentShop, slot.GunName);
    }
    public void OpenShop(Shop currentShop)
    {
        shopWindow.enabled = true;
        this.currentShop = currentShop;
    }
    public void CloseShop() => shopWindow.enabled = false;

    public void UpgradePlayer(PlayerUpgradesUI playerUpgrades)
    {
        player.Upgrade(playerUpgrades.UpgradeType);
        upgradeValues.SetValues(player);
    }
    public void OpenUpgrades()
    {
        playerUpgradesWindow.enabled = true;
        upgradeValues.SetValues(player);
    }
    public void CloseUpgrades() => playerUpgradesWindow.enabled = false;

    [System.Serializable]
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
        [System.Serializable]
        public struct UpgradeableProperty
        {
            public TextMeshProUGUI textField;
            public Button button;
        }
    }
}

