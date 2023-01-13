using UnityEngine;

public class PlayerUpgradesUI : MonoBehaviour
{
    [SerializeField] UpgradeTypes upgradeType;
    public UpgradeTypes UpgradeType => upgradeType;
}
