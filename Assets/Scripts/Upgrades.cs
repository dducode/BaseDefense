using UnityEngine;

public class Upgrades : MonoBehaviour
{
    [SerializeField] FloatUpgradeInfo speed = new FloatUpgradeInfo(10, 0.25f);
    [SerializeField] FloatUpgradeInfo maxHealth = new FloatUpgradeInfo(1000, 25);
    [SerializeField] IntUpgradeInfo capacity = new IntUpgradeInfo(10, 1);
    public IUpgradeable[] Targets { get; private set; }
    public FloatUpgradeInfo Speed => speed;
    public FloatUpgradeInfo MaxHealth => maxHealth;
    public IntUpgradeInfo Capacity => capacity;

    void Awake()
    {
        Targets = GetComponents<IUpgradeable>();
    }

    ///<summary>Реализует прокачку свойств компонентов</summary>
    ///<param name="upgradeType">Определяет прокачиваемое свойство</param>
    public void Upgrade(UpgradeTypes upgradeType)
    {
        if (upgradeType == UpgradeTypes.Upgrade_Speed)
        {
            for (int i = 0; i < Targets.Length; i++)
                if (Targets[i] is PlayerCharacter player && player.MaxSpeed < speed.maxValue)
                    player.Upgrade(upgradeType, speed.step);
        }
        else if (upgradeType == UpgradeTypes.Upgrade_Capacity)
        {
            for (int i = 0; i < Targets.Length; i++)
                if (Targets[i] is ItemCollecting itemCollecting && itemCollecting.Capacity < capacity.maxValue)
                    itemCollecting.Upgrade(upgradeType, capacity.step);
        }
        else if (upgradeType == UpgradeTypes.Upgrade_Max_Health)
        {
            for (int i = 0; i < Targets.Length; i++)
                if (Targets[i] is PlayerCharacter player && player.MaxHealthPoints < maxHealth.maxValue)
                    player.Upgrade(upgradeType, maxHealth.step);
        }
    }
    [System.Serializable]
    public struct FloatUpgradeInfo
    {
        [Min(0.001f)] public float maxValue;

        ///<summary>Размер шага определяет, на сколько увеличится прокачиваемое свойство за один раз</summary>
        [Tooltip("Размер шага определяет, на сколько увеличится прокачиваемое свойство за один раз")]
        [Min(0.001f)] public float step;

        public FloatUpgradeInfo(float maxValue, float step)
        {
            this.maxValue = maxValue;
            this.step = step;
        }
    }
    [System.Serializable]
    public struct IntUpgradeInfo
    {
        [Min(1)] public int maxValue;

        ///<summary>Размер шага определяет, на сколько увеличится прокачиваемое свойство за один раз</summary>
        [Tooltip("Размер шага определяет, на сколько увеличится прокачиваемое свойство за один раз")]
        [Min(1)] public int step;

        public IntUpgradeInfo(int maxValue, int step)
        {
            this.maxValue = maxValue;
            this.step = step;
        }
    }
}

///<summary>Реализуется в компонентах MonoBehaviour, свойства которых необходимо прокачивать в процессе игры</summary>
public interface IUpgradeable
{
    ///<summary>Реализует прокачку определённого свойства</summary>
    ///<param name="upgradeType">Определяет прокачиваемое свойство</param>
    ///<param name="step">Размер шага определяет, на сколько увеличится прокачиваемое свойство за один раз</param>
    public void Upgrade(UpgradeTypes upgradeType, float step);
}

///<summary>Список прокачиваемых свойств</summary>
public enum UpgradeTypes
{
    Upgrade_Speed, Upgrade_Capacity, Upgrade_Max_Health
}
