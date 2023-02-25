using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.Serialization;

namespace BaseDefense.Items
{
    ///<summary>Базовый класс для всех видов выпадаемых предметов</summary>
    [Icon("Assets/EditorUI/item.png")]
    [RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
    public abstract class Item : Object
    {
        ///<summary>Скорость анимации исчезания предмета</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Скорость анимации исчезания предмета. [0, infinity]")]
        [SerializeField, Min(0)] private float collapseSpeed = 2;
        [SerializeField] protected Collider trigger;
        [SerializeField] protected Collider meshCollider;

        ///<summary>При включении предмета также включается его триггер, физика жёсткого тела и коллайдер</summary>
        bool m_enabled;
        ///<inheritdoc cref="m_enabled"/>
        public new bool enabled
        {
            get => m_enabled;
            set
            {
                m_enabled = value;
                base.enabled = m_enabled;
                trigger.enabled = m_enabled;
                meshCollider.enabled = m_enabled;
                Rigidbody.isKinematic = !m_enabled;
            }
        }

        protected Rigidbody Rigidbody;

        ///<summary>Вызывается для выброса предмета</summary>
        ///<param name="force">Направление силы, в котором нужно выбросить предмет</param>
        ///<param name="torque">Направление вращения предмета во время выброса</param>
        public abstract void Drop(Vector3 force, Vector3 torque = default);

        ///<summary>Уничтожает предмет</summary>
        ///<remarks>
        ///Рекомендуется вместо вызова метода Object.Destroy() в данном методе использовать ObjectsPool.Push()
        ///</remarks>
        public abstract void DestroyItem();

        protected override void Awake()
        {
            base.Awake();
            Rigidbody = GetComponent<Rigidbody>();
            enabled = true;
        }

        protected Sequence Collapse()
        {
            const float punchScaleScalar = 0.5f;
            const float smallScaleScalar = 0.001f;
            var punchScale = new Vector3(punchScaleScalar, punchScaleScalar, punchScaleScalar);
            var smallScale = new Vector3(smallScaleScalar, smallScaleScalar, smallScaleScalar);
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOPunchScale(punchScale, 1 / collapseSpeed, 1, 0));
            sequence.Append(transform.DOScale(smallScale, 1 / collapseSpeed));
            sequence.OnComplete(() => sequence.Kill());

            return sequence;
        }
    }
}


