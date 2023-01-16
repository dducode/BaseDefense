using UnityEngine;

[RequireComponent(typeof(ItemDrop), typeof(DisplayHealthPoints))]
public class Crystal : MonoBehaviour, IAttackable
{
    ///<summary>Максимально возможное количество очков "здоровья" для кристалла</summary>
    ///<value>[1, infinity]</value>
    [Tooltip("Максимально возможное количество очков 'здоровья' для кристалла. [1, infinity]")]
    [SerializeField, Min(1)] float maxHealthPoints = 100;

    ///<summary>Текущее количество очков "здоровья"</summary>
    ///<value>[0, maxHealthPoints]</value>
    float currentHealthPoints;
    ///<inheritdoc cref="currentHealthPoints"/>
    public float CurrentHealthPoints
    {
        get { return currentHealthPoints; }
        set
        {
            currentHealthPoints = value;
            currentHealthPoints = Mathf.Clamp(currentHealthPoints, 0, maxHealthPoints);
        }
    }

    ///<returns>Возвращает true, если текущий показатель здоровья равен 0, иначе false</returns>
    public bool IsDestroyed => currentHealthPoints == 0;

    ItemDrop itemDrop;
    DisplayHealthPoints displayHealthPoints;

    void Awake()
    {
        itemDrop = GetComponent<ItemDrop>();
        displayHealthPoints = GetComponent<DisplayHealthPoints>();
        displayHealthPoints.SetMaxValue((int)maxHealthPoints);
        CurrentHealthPoints = maxHealthPoints;
    }

    public void Hit(float damage)
    {
        CurrentHealthPoints -= damage;
        displayHealthPoints.UpdateView((int)CurrentHealthPoints);
        if (IsDestroyed)
        {
            itemDrop.DropItems();
            Destroy(gameObject);
        }
    }
}