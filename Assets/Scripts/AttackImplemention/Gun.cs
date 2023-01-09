using UnityEngine;

///<summary>Базовый класс для всех видов оружия</summary>
public abstract class Gun : MonoBehaviour
{
    [Header("Общие характеристики оружия")]
    [SerializeField] protected GunName gunName;
    public GunName GunName => gunName;

    [SerializeField, Tooltip("Префаб патрона. Каждому оружию соответствует свой префаб патрона")] 
    protected Projectile projectilePrefab;

    ///<summary>Временной интервал между выстрелами. Не может быть меньше 0</summary>
    [SerializeField, Min(0), 
    Tooltip("Временной интервал между выстрелами. Не может быть меньше 0")] 
    protected float intervalOfShots;

    ///<summary>Производит выстрел из оружия</summary>
    ///<param name="target">Позиция цели, в которую необходимо выстрелить</param>
    public abstract void Shot(Vector3 target);
}
