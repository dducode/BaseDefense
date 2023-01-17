using UnityEngine;
using System.Collections;

///<summary>Базовый класс для всех видов выпадаемых предметов</summary>
[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public abstract class Item : MonoBehaviour
{
    ///<summary>Скорость анимации исчезания предмета</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Скорость анимации исчезания предмета. [0, infinity]")]
    [SerializeField, Min(0)] float collapseSpeed = 2;

    protected SphereCollider trigger;
    protected Rigidbody rb;

    ///<summary>Вызывается для выброса предмета</summary>
    ///<param name="force">Направление силы, в котором нужно выбросить предмет</param>
    ///<param name="torque">Направление вращения предмета во время выброса</param>
    public abstract void Drop(Vector3 force, Vector3 torque = default);

    ///<summary>Уничтожает предмет</summary>
    ///<remarks>
    ///Рекомендуется вместо вызова метода Destroy в данном методе использовать ObjectsPool
    ///</remarks>
    public abstract void DestroyItem();

    public virtual void Awake()
    {
        trigger = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        trigger.isTrigger = true;
    }

    ///<summary>
    ///Вспомогательная сопрограмма для реализации анимации исчезания предмета в производных классах
    ///</summary>
    protected IEnumerator Collapse()
    {
        Vector3 scale = transform.localScale;
        float step = 0;
        while (transform.localScale != Vector3.zero)
        {
            scale = Vector3.Lerp(Vector3.one, Vector3.zero, step);
            step += Time.smoothDeltaTime * collapseSpeed;
            transform.localScale = scale;
            yield return null;
        }
    }
}
