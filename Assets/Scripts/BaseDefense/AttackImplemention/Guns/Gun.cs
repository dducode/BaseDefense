using UnityEngine;
using BaseDefense.AttackImplemention.Projectiles;

namespace BaseDefense.AttackImplemention.Guns
{
    ///<summary>Базовый класс для всех видов оружия</summary>
    public abstract class Gun : Object
    {
        ///<summary>Префаб патрона. Каждому оружию соответствует свой патрон</summary>
        [Header("Общие характеристики оружия")]
        [Tooltip("Префаб патрона. Каждому оружию соответствует свой патрон")]
        [SerializeField] protected Projectile projectilePrefab;

        ///<summary>Временной интервал между выстрелами</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Временной интервал между выстрелами. [0, infinity]")]
        [SerializeField, Min(0)] protected float intervalOfShots;
        protected float timeOfLastShot;

        ///<summary>Дула оружия, преобразования, из которого вылетают патроны</summary>
        [Tooltip("Дула оружия, преобразования, из которого вылетают патроны")]
        [SerializeField] protected Transform[] muzzles;

        ///<summary>Производит выстрел из оружия</summary>
        ///<param name="target">Позиция цели, в которую необходимо выстрелить</param>
        public abstract void Shot();

        protected override void Awake()
        {
            base.Awake();
            timeOfLastShot = Time.time;
        }
    }
}

