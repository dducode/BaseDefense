using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyCharacter : BaseCharacter
{
    ///<summary>Скорость, развиваемая врагом при патруле</summary>
    [Header("Характеристики врага")]
    [SerializeField, Tooltip("Скорость, развиваемая врагом при патруле")] 
    float walkingSpeed;

    ///<summary>Коллайдер атакующей руки врага</summary>
    [Header("Связанные объекты")]
    [SerializeField, Tooltip("Коллайдер атакующей руки врага")] 
    Collider hand;

    ///<inheritdoc cref="walkingSpeed"/>
    public float WalkingSpeed => walkingSpeed;

    ///<inheritdoc cref="BaseCharacter.maxSpeed"/>
    public float MaxSpeed => maxSpeed;

    ///<inheritdoc cref="BaseCharacter.attackDistance"/>
    public float AttackDistance => attackDistance;

    EnemyBaseContainer enemyBase;
    Transform[] targetPoints;
    PlayerCharacter player;
    State State;

    [Inject]
    public void Initialize(EnemyBaseContainer enemyBase, Transform[] targetPoints, PlayerCharacter player)
    {
        Controller = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        this.enemyBase = enemyBase;
        this.targetPoints = targetPoints;
        this.player = player;
        State = new Walking(this, player.transform);
    }

    ///<summary>Вызывается как для порождения нового врага, так и для респавна умершего</summary>
    ///<param name="position">Точка спавна врага</param>
    ///<param name="rotation">Поворот, принимаемый во время спавна</param>
    public void Spawn(Vector3 position, Quaternion rotation)
    {
        CurrentHealthPoints = maxHealthPoints;
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
            hand.enabled = State is Attack;

            if (State is Walking) // Восстановление здоровья при патруле
                CurrentHealthPoints += Time.smoothDeltaTime * 5f;
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
        if (!IsAlive)
        {
            Controller.enabled = false;
            Animator.SetBool("alive", false);
            StartCoroutine(Await());
        }
    }

    IEnumerator Await()
    {
        yield return new WaitWhile(() => IsDeath());
        GetComponent<ItemDrop>().DropItems();
        ObjectsPool<EnemyCharacter>.Push(this);
    }
    bool IsDeath()
    {
        AnimatorStateInfo info = Animator.GetCurrentAnimatorStateInfo(0);
        return !(info.IsName("Death") && info.normalizedTime >= 0.9f);
    }

    public class Factory : PlaceholderFactory<Transform[], EnemyCharacter> {}
}
