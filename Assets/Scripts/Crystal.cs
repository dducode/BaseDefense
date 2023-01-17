using UnityEngine;

[RequireComponent(typeof(ItemDrop))]
public class Crystal : MonoBehaviour, IAttackable
{
    ///<summary>Максимально возможное количество очков "здоровья" для кристалла</summary>
    ///<value>[1, infinity]</value>
    [Tooltip("Максимально возможное количество очков 'здоровья' для кристалла. [1, infinity]")]
    [SerializeField, Min(1)] float maxHealthPoints = 100;

    ///<summary>Текущее количество очков "здоровья" кристалла</summary>
    ///<value>[0, maxHealthPoints]</value>
    float currentHealthPoints;
    ///<inheritdoc cref="currentHealthPoints"/>
    public float CurrentHealthPoints
    {
        get => currentHealthPoints;
        private set
        {
            currentHealthPoints = value;
            currentHealthPoints = Mathf.Clamp(currentHealthPoints, 0, maxHealthPoints);
            if (currentHealthPoints == 0)
                DestroyCrystal();
        }
    }

    ItemDrop itemDrop;

    void Awake()
    {
        CurrentHealthPoints = maxHealthPoints;
        itemDrop = GetComponent<ItemDrop>();
    }

    ///<summary>Вызывается для нанесения повреждений кристаллу</summary>
    ///<param name="damage">Количество нанесённых повреждений</param>
    public void Hit(float damage)
    {
        CurrentHealthPoints -= damage;
    }

    void DestroyCrystal()
    {
        itemDrop.DropItems();
        Destroy(gameObject);
    }
}