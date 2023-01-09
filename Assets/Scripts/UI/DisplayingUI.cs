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
    [Inject] PlayerCharacter player;

    void Start()
    {
        deathWindow.enabled = false;
        shopWindow.enabled = false;
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
}
