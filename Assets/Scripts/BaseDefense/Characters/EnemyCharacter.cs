using System;
using System.Collections;
using UnityEngine;
using Zenject;
using BaseDefense.AttackImplemention;
using BaseDefense.StateMachine;
using BaseDefense.Items;
using BaseDefense.Properties;
using Random = UnityEngine.Random;

namespace BaseDefense.Characters {

    [RequireComponent(typeof(ItemDrop), typeof(Ragdoll))]
    public sealed class EnemyCharacter : BaseCharacter {

        ///<summary>Скорость, развиваемая врагом при патруле</summary>
        ///<value>[0, infinity]</value>
        [Header("Характеристики врага")]
        [Tooltip("Скорость, развиваемая врагом при патруле. [0, infinity]")]
        [SerializeField, Min(0)]
        private float walkingSpeed;

        ///<summary>Урон, наносимый врагом игроку</summary>
        ///<value>Диапазон значений на отрезке [0, 100]</value>
        [Tooltip("Урон, наносимый врагом игроку")]
        [SerializeField]
        private MinMaxSliderFloat damage = new MinMaxSliderFloat(0, 100);

        ///<summary>Атакующая рука врага</summary>
        [Header("Связанные объекты")]
        [Tooltip("Атакующая рука врага")]
        [SerializeField]
        private Punch hand;


        [SerializeField]
        private Material opaqueMaterial;

        [SerializeField]
        private Material transparentMaterial;

        ///<inheritdoc cref="walkingSpeed"/>
        public float WalkingSpeed => walkingSpeed;

        ///<inheritdoc cref="BaseCharacter.maxSpeed"/>
        public float MaxSpeed => maxSpeed;

        ///<inheritdoc cref="BaseCharacter.attackDistance"/>
        public float AttackDistance => attackDistance;

        ///<summary>При включении поведения врага также включаются его аниматор и контроллёр</summary>
        private bool m_enabled;

        ///<inheritdoc cref="m_enabled"/>
        private new bool Enabled {
            set {
                m_enabled = value;
                base.Enabled = m_enabled;
                Animator.enabled = m_enabled;
            }
        }

        private Transform[] m_targetPoints;
        private PlayerCharacter m_player;
        private State m_state;
        private ItemDrop m_itemDrop;
        private Ragdoll m_ragdoll;
        private static readonly int ColorID = Shader.PropertyToID("_Color");


        [Inject]
        public void Constructor (PlayerCharacter player) {
            m_itemDrop = GetComponent<ItemDrop>();
            m_ragdoll = GetComponent<Ragdoll>();
            m_player = player;
            hand.Damage = damage;
        }


        /// <summary>Вызывается как для порождения нового врага, так и для респавна умершего</summary>
        /// <param name="targetPoints">Целевые точки для патруля</param>
        /// <param name="position">Точка спавна врага</param>
        /// <param name="rotation">Поворот, принимаемый во время спавна</param>
        public void Initialize (Transform[] targetPoints, Vector3 position, Quaternion rotation) {
            m_targetPoints = targetPoints;
            CurrentHealthPoints = maxHealthPoints;
            transform.SetLocalPositionAndRotation(position, rotation);
            Enabled = true;
            m_state = new Walking(this, m_player.transform);
        }


        ///<summary>Заменяет обычный Update метод</summary>
        ///<remarks>
        ///Должен вызываться из другого сценария. Обычно это сценарий, который порождает персонажей данного типа
        ///</remarks>
        ///<returns>Возвращает false, если персонаж мёртв</returns>
        public bool EnemyUpdate () {
            if (IsAlive) {
                m_state = m_state.Process();

                if (m_state is Attack) {
                    var info = Animator.GetCurrentAnimatorStateInfo(0);
                    var cycle = (int) info.normalizedTime;
                    hand.Enabled = info.IsName("Base.Attack") &&
                                   info.normalizedTime - cycle > 0.5f &&
                                   info.normalizedTime - cycle < 0.75f;
                }
                else
                    hand.Enabled = false;

                const float recoveryTime = 60;
                if (m_state is Walking) // Восстановление здоровья при патруле
                    CurrentHealthPoints += Time.smoothDeltaTime * maxHealthPoints / recoveryTime;
            }
            else
                hand.Enabled = false;

            return IsAlive;
        }


        ///<summary>Вызывается для получения случайной целевой точки патруля</summary>
        public Vector3 GetRandomPoint () {
            return m_targetPoints[Random.Range(0, m_targetPoints.Length)].position;
        }


        ///<summary>Вызывается для атаки на игрока</summary>
        public void AttackPlayer () {
            m_state.SetTrigger(true);
        }


        /// <summary>Вызывается для патруля</summary>
        public void Patrol () {
            m_state.SetTrigger(false);
        }


        public override void Hit (float damage) {
            CurrentHealthPoints -= damage;
            var emission = HitEffect.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, (int) damage * 100 / maxHealthPoints));
            HitEffect.Play();
        }


        protected override void OnDeath () {
            Enabled = false;
            m_ragdoll.Enabled = true;
            var punchDirection = transform.forward * -25 + Vector3.up;
            m_ragdoll.AddImpulse(punchDirection);
            m_itemDrop.DropItems();
            MeshRenderer.material.color = deathColor;
            Destroy(Disappearance(() => {
                MeshRenderer.material.color = DefaultColor;
                m_ragdoll.Enabled = false;
            }));
        }


        private IEnumerator Disappearance (Action onComplete) {
            yield return new WaitForSeconds(2);
            MeshRenderer.material = transparentMaterial;
            var startColor = deathColor;
            var targetColor = startColor;
            targetColor.a = 0;
            var time = Time.time;

            while (MeshRenderer.material.color.a > 0) {
                MeshRenderer.material.color = Color.Lerp(startColor, targetColor, (Time.time - time) * 2);
                yield return null;
            }

            MeshRenderer.material = opaqueMaterial;
            onComplete();
        }


        public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyCharacter> { }

    }

}