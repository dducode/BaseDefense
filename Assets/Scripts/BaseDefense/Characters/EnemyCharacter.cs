using System.Collections;
using UnityEngine;
using Zenject;
using BaseDefense.AttackImplemention;
using BaseDefense.StateMachine;
using BaseDefense.Items;

namespace BaseDefense.Characters
{
    [RequireComponent(typeof(ItemDrop), typeof(Ragdoll))]
    public sealed class EnemyCharacter : BaseCharacter
    {
        ///<summary>Скорость, развиваемая врагом при патруле</summary>
        ///<value>[0, infinity]</value>
        [Header("Характеристики врага")]
        [Tooltip("Скорость, развиваемая врагом при патруле. [0, infinity]")]
        [SerializeField, Min(0)] private float walkingSpeed;

        ///<summary>Урон, наносимый врагом игроку</summary>
        ///<value>Диапазон значений на отрезке [0, 100]</value>
        [Tooltip("Урон, наносимый врагом игроку")]
        [SerializeField] private MinMaxSliderFloat damage = new MinMaxSliderFloat(0, 100);

        ///<summary>Атакующая рука врага</summary>
        [Header("Связанные объекты")]
        [Tooltip("Атакующая рука врага")]
        [SerializeField] private Punch hand;

        ///<inheritdoc cref="walkingSpeed"/>
        public float WalkingSpeed => walkingSpeed;

        ///<inheritdoc cref="BaseCharacter.maxSpeed"/>
        public float MaxSpeed => maxSpeed;

        ///<inheritdoc cref="BaseCharacter.attackDistance"/>
        public float AttackDistance => attackDistance;

        ///<summary>При включении поведения врага также включаются его аниматор и контроллёр</summary>
        private bool m_enabled;
        ///<inheritdoc cref="m_enabled"/>
        public new bool enabled
        {
            get => m_enabled;
            set
            {
                m_enabled = value;
                base.enabled = m_enabled;
                Animator.enabled = m_enabled;
            }
        }

        private Transform[] m_targetPoints;
        private PlayerCharacter m_player;
        private State m_state;
        private ItemDrop m_itemDrop;
        private Ragdoll m_ragdoll;

        [Inject]
        public void Initialize(PlayerCharacter player)
        {
            m_itemDrop = GetComponent<ItemDrop>();
            m_ragdoll = GetComponent<Ragdoll>();
            m_player = player;
            hand.Damage = damage;
        }

        /// <summary>Вызывается как для порождения нового врага, так и для респавна умершего</summary>
        /// <param name="targetPoints">Целевые точки для патруля</param>
        /// <param name="position">Точка спавна врага</param>
        /// <param name="rotation">Поворот, принимаемый во время спавна</param>
        public void Spawn(Transform[] targetPoints, Vector3 position, Quaternion rotation)
        {
            m_targetPoints = targetPoints;
            CurrentHealthPoints = maxHealthPoints;
            m_ragdoll.enabled = false;
            enabled = false;
            transform.SetLocalPositionAndRotation(position, rotation);
            enabled = true;
            m_state = new Walking(this, m_player.transform);
            MeshRenderer.material.color = DefaultColor;
        }

        ///<summary>Заменяет обычный Update метод</summary>
        ///<remarks>
        ///Должен вызываться из другого сценария. Обычно это сценарий, который порождает персонажей данного типа
        ///</remarks>
        ///<returns>Возвращает false, если персонаж мёртв</returns>
        public bool EnemyUpdate()
        {
            if (IsAlive)
            {
                m_state = m_state.Process();
                if (m_state is Attack)
                {
                    var info = Animator.GetCurrentAnimatorStateInfo(0);
                    int cycle = (int)info.normalizedTime;
                    hand.enabled = info.IsName("Base.Attack") && 
                        info.normalizedTime - cycle > 0.5f &&
                        info.normalizedTime - cycle < 0.75f;
                }
                else
                    hand.enabled = false;

                const float recoveryTime = 60;
                if (m_state is Walking) // Восстановление здоровья при патруле
                    CurrentHealthPoints += Time.smoothDeltaTime * maxHealthPoints / recoveryTime;
            }
            else
                hand.enabled = false;
            
            return IsAlive;
        }

        ///<summary>Вызывается для получения случайной целевой точки патруля</summary>
        public Vector3 GetRandomPoint()
        {
            return m_targetPoints[Random.Range(0, m_targetPoints.Length)].position;
        }

        ///<summary>Устанавливает триггер для атаки на игрока</summary>
        ///<param name="value">Значение устанавливаемого триггера</param>
        public void SetTrigger(bool value)
        {
            m_state.SetTrigger(value);
        }

        public override void Hit(float damage)
        {
            CurrentHealthPoints -= damage;
            var emission = HitEffect.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, (int)damage * 100 / maxHealthPoints));
            HitEffect.Play();
        }

        protected override void Death()
        {
            enabled = false;
            m_ragdoll.enabled = true;
            var punchDirection = (transform.forward * -25) + Vector3.up;
            m_ragdoll.AddImpulse(punchDirection);
            m_itemDrop.DropItems();
            MeshRenderer.material.color = deathColor;
            StartCoroutine(Disappearance());
        }

        private IEnumerator Disappearance()
        {
            yield return new WaitForSeconds(2);
            var startColor = MeshRenderer.material.color;
            var targetColor = startColor;
            targetColor.a = 0;
            var time = Time.time;
            while (MeshRenderer.material.color.a > 0)
            {
                MeshRenderer.material.color = Color.Lerp(startColor, targetColor, (Time.time - time) * 2);
                yield return null;
            }
            ObjectsPool<EnemyCharacter>.Push(this);
        }

        public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyCharacter> {}
    }
}


