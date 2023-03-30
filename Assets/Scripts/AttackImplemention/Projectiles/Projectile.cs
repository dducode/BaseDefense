using UnityEngine;

namespace BaseDefense.AttackImplemention.Projectiles {

    ///<summary>Базовый класс для всех видов патронов</summary>
    [Icon("Assets/EditorUI/ammo.png")]
    [RequireComponent(typeof(Rigidbody), typeof(TrailRenderer))]
    public abstract class Projectile : Object {

        protected Rigidbody rb;
        protected TrailRenderer trailRenderer;


        ///<summary>Добавляет импульс во время выстрела из оружия</summary>
        ///<param name="force">Вектор направления силы выстрела</param>
        public abstract void AddImpulse (Vector3 force);


        protected abstract void OnCollisionEnter (Collision collision);


        protected override void Awake () {
            base.Awake();
            rb = GetComponent<Rigidbody>();
            trailRenderer = GetComponent<TrailRenderer>();
        }

    }

}