using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayingUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneys;
    [SerializeField] TextMeshProUGUI gems;
    [SerializeField] Canvas shop;
    [SerializeField] Canvas deathWindow;
    [SerializeField] RectTransform frame;

    void OnEnable() => BroadcastMessages.AddListener(MessageType.DEATH_PLAYER, DisplayDeathWindow);
    void OnDisable() => BroadcastMessages.RemoveListener(MessageType.DEATH_PLAYER, DisplayDeathWindow);

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
        Game.Player.SelectGun(slot);
    }
    public void OpenShop() => shop.enabled = true;
    public void CloseShop() => shop.enabled = false;
}
