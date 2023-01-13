using UnityEngine;
using Zenject;

public class PlayerUpgrades : MonoBehaviour
{
    [Inject] DisplayingUI UI;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            UI.OpenUpgrades();
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            UI.CloseUpgrades();
    }
}
