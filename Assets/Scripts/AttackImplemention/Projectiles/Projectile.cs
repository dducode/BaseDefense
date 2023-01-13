using UnityEngine;

///<summary>Базовый класс для всех видов патронов</summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TrailRenderer))]
public abstract class Projectile : MonoBehaviour
{
    protected Rigidbody rb;
    protected TrailRenderer trailRenderer;

    ///<summary>Эффект, который необходимо воспроизвести после попадания снаряда</summary>
    [Tooltip("Эффект, который необходимо воспроизвести после попадания снаряда")]
    [SerializeField] protected ParticleSystem effect;

    ///<summary>Добавляет импульс во время выстрела из оружия</summary>
    ///<param name="force">Вектор направления силы выстрела</param>
    public abstract void AddImpulse(Vector3 force);

    public abstract void OnCollisionEnter(Collision collision);

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }
}
