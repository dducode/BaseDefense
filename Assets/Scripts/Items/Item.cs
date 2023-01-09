using UnityEngine;

///<summary>Базовый класс для всех видов выпадаемых предметов</summary>
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Item : MonoBehaviour
{
    [SerializeField, Tooltip("Вид силы, прикладываемый к предмету")] 
    protected ForceMode forceMode = ForceMode.Impulse;

    protected SphereCollider trigger;
    protected Rigidbody rb;

    ///<summary>Вызывается для выброса предмета</summary>
    ///<param name="force">Направление силы, в котором нужно выбросить предмет</param>
    ///<param name="torque">Направление вращения предмета во время выброса</param>
    public abstract void Drop(Vector3 force, Vector3 torque = default);

    ///<summary>Уничтожает предмет</summary>
    ///<remarks>
    ///Рекомендуется вместо вызова метода Destroy в данном методе использовать пул объектов
    ///</remarks>
    public abstract void DestroyItem();

    public virtual void Awake()
    {
        trigger = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        trigger.isTrigger = true;
    }
}
