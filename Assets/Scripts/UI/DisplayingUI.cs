using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] PlayerUpgradeValues playerUpgradeValues;
    [Inject] PlayerCharacter player;
    [Inject] ItemCollecting itemCollecting;

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
        player.Upgrade(playerUpgrades.UpgradeType);
        playerUpgradeValues.SetValues(player.MaxSpeed, itemCollecting.StackSize, player.MaxHealthPoints);
    }
    public void OpenUpgrades()
    {
        playerUpgradesWindow.enabled = true;
        playerUpgradeValues.SetValues(player.MaxSpeed, itemCollecting.StackSize, player.MaxHealthPoints);
    }
    public void CloseUpgrades() => playerUpgradesWindow.enabled = false;

    [System.Serializable]
    public struct PlayerUpgradeValues
    {
        public TextMeshProUGUI speed;
        public TextMeshProUGUI stackSize;
        public TextMeshProUGUI maxHealth;

        public void SetValues(float speed, int stackSize, float maxHealth)
        {
            this.speed.text = $"Speed: {speed}";
            this.stackSize.text = $"Stack size: {stackSize}";
            this.maxHealth.text = $"Max health: {maxHealth}";
        }
    }
}

public enum UpgradeTypes
{
    Upgrade_Speed, Upgrade_Stack_Size, Upgrade_Max_Health
}
