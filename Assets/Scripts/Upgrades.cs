using UnityEngine;

///<summary>Хранит в себе информацию о прокачиваемых характеристиках игрока</summary>
///<remarks>Каждая характеристика имеет свои максимальные значения при прокачке и "шаг" прокачки</remarks>
[System.Serializable]
public class Upgrades
{
    [SerializeField] FloatUpgradeInfo speed = new FloatUpgradeInfo(10, 0.25f);
    [SerializeField] FloatUpgradeInfo maxHealth = new FloatUpgradeInfo(1000, 25);
    [SerializeField] IntUpgradeInfo capacity = new IntUpgradeInfo(10, 1);

    public FloatUpgradeInfo Speed => speed;
    public FloatUpgradeInfo MaxHealth => maxHealth;
    public IntUpgradeInfo Capacity => capacity;

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

///<summary>Список прокачиваемых свойств</summary>
public enum UpgradeTypes
{
    Upgrade_Speed, Upgrade_Capacity, Upgrade_Max_Health
}
