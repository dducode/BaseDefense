using UnityEngine;
using System.Collections;

namespace BaseDefense.Items
{
    ///<summary>Базовый класс для всех видов выпадаемых предметов</summary>
    [RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
    public abstract class Item : MonoBehaviour
    {
        ///<summary>Скорость анимации исчезания предмета</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Скорость анимации исчезания предмета. [0, infinity]")]
        [SerializeField, Min(0)] float collapseSpeed = 2;

        ///<summary>При включении предмета также включается его триггер, физика жёсткого тела и коллайдер</summary>
        bool _enabled;
        ///<inheritdoc cref="_enabled"/>
        public new bool enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                base.enabled = _enabled;
                trigger.enabled = _enabled;
                meshCollider.enabled = _enabled;
                rb.isKinematic = !_enabled;
            }
        }

        protected Collider trigger;
        protected Collider meshCollider;
        protected Rigidbody rb;

        ///<summary>Вызывается для выброса предмета</summary>
        ///<param name="force">Направление силы, в котором нужно выбросить предмет</param>
        ///<param name="torque">Направление вращения предмета во время выброса</param>
        public abstract void Drop(Vector3 force, Vector3 torque = default);

        ///<summary>Уничтожает предмет</summary>
        ///<remarks>
        ///Рекомендуется вместо вызова метода Object.Destroy() в данном методе использовать ObjectsPool.Push()
        ///</remarks>
        public abstract void Destroy();

        public virtual void Awake()
        {
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider collider in colliders)
                if (collider.isTrigger)
                    trigger = collider;
                else
                    meshCollider = collider;
            rb = GetComponent<Rigidbody>();
            enabled = true;
        }

        ///<summary>
        ///Вспомогательная сопрограмма для реализации анимации исчезания предмета в производных классах
        ///</summary>
        protected IEnumerator Collapse()
        {
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = Vector3.one * 0.001f;
            float time = Time.time;
            while (transform.localScale != targetScale)
            {
                transform.localScale = Vector3.Lerp(startScale, targetScale, (Time.time - time) * collapseSpeed);
                yield return null;
            }
        }
    }
}


