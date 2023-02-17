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
        [SerializeField, Min(0)] float walkingSpeed;

        ///<summary>Урон, наносимый врагом игроку</summary>
        ///<value>Диапазон значений на отрезке [0, 100]</value>
        [Tooltip("Урон, наносимый врагом игроку")]
        [SerializeField] MinMaxSliderFloat damage = new MinMaxSliderFloat(0, 100);

        ///<summary>Атакующая рука врага</summary>
        [Header("Связанные объекты")]
        [Tooltip("Атакующая рука врага")]
        [SerializeField] Punch hand;

        ///<inheritdoc cref="walkingSpeed"/>
        public float WalkingSpeed => walkingSpeed;

        ///<inheritdoc cref="BaseCharacter.maxSpeed"/>
        public float MaxSpeed => maxSpeed;

        ///<inheritdoc cref="BaseCharacter.attackDistance"/>
        public float AttackDistance => attackDistance;

        ///<summary>При включении поведения врага также включаются его аниматор и контроллёр</summary>
        bool _enabled;
        ///<inheritdoc cref="_enabled"/>
        public new bool enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                base.enabled = _enabled;
                Animator.enabled = _enabled;
            }
        }

        Transform[] targetPoints;
        PlayerCharacter player;
        State state;
        ItemDrop itemDrop;
        Ragdoll ragdoll;

        [Inject]
        public void Initialize(PlayerCharacter player)
        {
            itemDrop = GetComponent<ItemDrop>();
            ragdoll = GetComponent<Ragdoll>();
            this.player = player;
            hand.Damage = damage;
        }

        ///<summary>Вызывается как для порождения нового врага, так и для респавна умершего</summary>
        ///<param name="position">Точка спавна врага</param>
        ///<param name="rotation">Поворот, принимаемый во время спавна</param>
        public void Spawn(Transform[] targetPoints, Vector3 position, Quaternion rotation)
        {
            this.targetPoints = targetPoints;
            CurrentHealthPoints = maxHealthPoints;
            ragdoll.enabled = false;
            enabled = false;
            transform.SetLocalPositionAndRotation(position, rotation);
            enabled = true;
            state = new Walking(this, player.transform);
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
                state = state.Process();
                if (state is Attack)
                {
                    AnimatorStateInfo info = Animator.GetCurrentAnimatorStateInfo(0);
                    int cycle = (int)info.normalizedTime;
                    hand.enabled = info.IsName("Base.Attack") && 
                        info.normalizedTime - cycle > 0.5f &&
                        info.normalizedTime - cycle < 0.75f;
                }
                else
                    hand.enabled = false;

                if (state is Walking) // Восстановление здоровья при патруле
                    CurrentHealthPoints += Time.smoothDeltaTime * maxHealthPoints / 60;
            }
            else
                hand.enabled = false;
            
            return IsAlive;
        }

        ///<summary>Вызывается для получения случайной целевой точки патруля</summary>
        public Vector3 GetRandomPoint()
        {
            return targetPoints[Random.Range(0, targetPoints.Length)].position;
        }

        ///<summary>Устанавливает триггер для атаки на игрока</summary>
        ///<param name="value">Значение устанавливаемого триггера</param>
        public void SetTrigger(bool value)
        {
            state.SetTrigger(value);
        }

        public override void Hit(float damage)
        {
            CurrentHealthPoints -= damage;
            var emission = HitEffect.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, (int)damage * 100 / maxHealthPoints));
            HitEffect.Play();
        }

        protected override void DestroyCharacter()
        {
            enabled = false;
            ragdoll.enabled = true;
            ragdoll.AddImpulse((transform.forward * -25) + Vector3.up);
            itemDrop.DropItems();
            MeshRenderer.material.color = deathColor;
            StartCoroutine(Disappearance());
        }

        IEnumerator Disappearance()
        {
            yield return new WaitForSeconds(2);
            Color startColor = MeshRenderer.material.color;
            Color targetColor = startColor;
            targetColor.a = 0;
            float time = Time.time;
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


