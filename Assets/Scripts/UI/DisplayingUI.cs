using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class DisplayingUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneys;
    [SerializeField] TextMeshProUGUI gems;
    [SerializeField] Canvas shop;
    [SerializeField] Canvas deathWindow;
    [SerializeField] RectTransform frame;
    [Inject] PlayerCharacter player;

    void Start()
    {
        deathWindow.enabled = false;
        shop.enabled = false;
    }
    
    public void UpdateUI(int moneys, int gems)
    {
        this.moneys.text = moneys.ToString();
        this.gems.text = gems.ToString();
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
        BroadcastMessages.SendMessage(MessageType.RESTART);
        deathWindow.enabled = false;
    }
    public void SelectGun(GunSlot slot)
    {
        frame.localPosition = slot.transform.localPosition;
        player.SelectGun(slot);
    }
    public void OpenShop() => shop.enabled = true;
    public void CloseShop() => shop.enabled = false;
}
