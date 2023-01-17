using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(ItemDrop), typeof(Ragdoll))]
public class EnemyCharacter : BaseCharacter
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

    EnemyFactory enemyFactory;
    Transform[] targetPoints;
    PlayerCharacter player;
    State State;
    ItemDrop itemDrop;
    Ragdoll ragdoll;

    [Inject]
    public void Initialize(EnemyFactory enemyFactory, Transform[] targetPoints, PlayerCharacter player)
    {
        itemDrop = GetComponent<ItemDrop>();
        ragdoll = GetComponent<Ragdoll>();
        this.enemyFactory = enemyFactory;
        this.targetPoints = targetPoints;
        this.player = player;
        hand.Damage = damage;
        State = new Walking(this, player.transform);
    }

    ///<summary>Вызывается как для порождения нового врага, так и для респавна умершего</summary>
    ///<param name="position">Точка спавна врага</param>
    ///<param name="rotation">Поворот, принимаемый во время спавна</param>
    public void Spawn(Vector3 position, Quaternion rotation)
    {
        CurrentHealthPoints = maxHealthPoints;
        Animator.enabled = true;
        ragdoll.enabled = false;
        Controller.enabled = false;
        transform.SetLocalPositionAndRotation(position, rotation);
        Controller.enabled = true;
        State = new Walking(this, player.transform);
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
            State = State.Process();
            if (State is Attack)
            {
                AnimatorStateInfo info = Animator.GetCurrentAnimatorStateInfo(0);
                int cycle = (int)info.normalizedTime;
                hand.enabled = info.IsName("Base.Attack") && 
                    info.normalizedTime - cycle > 0.5f &&
                    info.normalizedTime - cycle < 0.75f;
            }
            else
                hand.enabled = false;

            if (State is Walking) // Восстановление здоровья при патруле
                CurrentHealthPoints += Time.smoothDeltaTime * maxHealthPoints / 20;
        }
        
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
        State.SetTrigger(value);
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
        Controller.enabled = false;
        Animator.enabled = false;
        ragdoll.enabled = true;
        ragdoll.AddImpulse((transform.forward * -25) + Vector3.up);
        StartCoroutine(AwaitRagdoll());
    }

    IEnumerator AwaitRagdoll()
    {
        yield return new WaitForSeconds(2);
        itemDrop.DropItems();
        ObjectsPool<EnemyCharacter>.Push(this);
    }

    public class Factory : PlaceholderFactory<Transform[], EnemyCharacter> {}
}
