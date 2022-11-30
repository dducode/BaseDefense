using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayingUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneys;
    [SerializeField] TextMeshProUGUI gems;
    [SerializeField] Canvas deathWindow;
    [SerializeField] Canvas shop;
    JoystickController joystick;

    void OnEnable() => BroadcastMessages.AddListener(MessageType.DEATH_PLAYER, DisplayDeathWindow);
    void OnDisable() => BroadcastMessages.RemoveListener(MessageType.DEATH_PLAYER, DisplayDeathWindow);

    void Start()
    {
        joystick = GetComponent<JoystickController>();
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
        joystick.enabled = false;
        yield return new WaitForSeconds(2);
        deathWindow.enabled = true;
    }
    public void Restart()
    {
        PlayerCharacter.Instance.Resurrection();
        joystick.enabled = true;
        deathWindow.enabled = false;
    }
    public void OpenShop()
    {
        shop.enabled = true;
    }
    public void CloseShop()
    {
        shop.enabled = false;
    }
}
