using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayingUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI money;
    [SerializeField] TextMeshProUGUI gems;
    [SerializeField] Canvas deathWindow;
    JoystickController joystick;

    void OnEnable() => BroadcastMessages.AddListener(MessageType.DEATH_PLAYER, DisplayWindow);
    void OnDisable() => BroadcastMessages.RemoveListener(MessageType.DEATH_PLAYER, DisplayWindow);

    void Start()
    {
        joystick = GetComponent<JoystickController>();
        money.text = Inventory.getInstance.getMoney.ToString();
        gems.text = Inventory.getInstance.getGems.ToString();
        deathWindow.enabled = false;
    }
    
    public void UpdateUI()
    {
        money.text = Inventory.getInstance.getMoney.ToString();
        gems.text = Inventory.getInstance.getGems.ToString();
    }

    public void DisplayWindow() => StartCoroutine(Await());
    IEnumerator Await()
    {
        joystick.enabled = false;
        yield return new WaitForSeconds(2);
        deathWindow.enabled = true;
    }

    public void Restart()
    {
        PlayerCharacter.getInstance.Resurrection();
        joystick.enabled = true;
        deathWindow.enabled = false;
    }
}
