using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Zenject;
using BroadcastMessages;

public class PlayerCharacter : BaseCharacter
{
    ///<summary>Transform руки, в которой игрок держит оружие</summary>
    [Header("Связанные объекты")]
    [SerializeField, Tooltip("Transform руки, в которой игрок держит оружие")]
    Transform gunSlot;

    ///<summary>Точка респавна игрока</summary>
    [SerializeField, Tooltip("Точка респавна игрока")]
    Transform recoveryPoint;

    ///<inheritdoc cref="BaseCharacter.maxHealthPoints"/>
    public float MaxHealthPoints => maxHealthPoints;

    ///<summary>Направление взгляда в сторону врага</summary>
    Vector3 lookToEnemy;

    JoystickController joystick;
    Shop shop;
    Gun gun;
    Vector3 move;
    bool inBase;
    bool inEnemyBase;
    IEnumerator awaitAnimation;

    bool invulnerability;

    [Inject]
    public void Construct(Shop shop, JoystickController joystick)
    {
        this.shop = shop;
        this.joystick = joystick;
    }

    public override void Awake()
    {
        base.Awake();
        gun = gunSlot.GetChild(0).GetComponent<Gun>();
        gun.gameObject.SetActive(false);
    }

    void Update()
    {
        if (lookToEnemy != Vector3.zero)
            LookToTarget(lookToEnemy, 15);
        else if (move != Vector3.zero)
            LookToTarget(move, 15);

        if (inEnemyBase && !Animator.IsInTransition(0)) // Ожидание перехода для предотвращения преждевременной стрельбы
        // поиск ближайшего врага на вражеской базе и атака
        {
            GameObject enemy = GetNearestEnemy();
            if (enemy is null)
                lookToEnemy = Vector3.zero;
            else
            {
                lookToEnemy = enemy.transform.position - transform.position;
                if (Vector3.Dot(lookToEnemy.normalized, transform.forward) > .9f)
                {
                    if (gun is GrenadeLauncher grenade)
                    {
                        if (lookToEnemy.magnitude > grenade.DamageRadius)
                            gun.Shot(enemy.transform.position + Vector3.up);
                    }
                    else
                        gun.Shot(enemy.transform.position + Vector3.up);
                }
            }
        }
        else if (inBase) // восстановление здоровья на своей базе
            CurrentHealthPoints += Time.smoothDeltaTime * 5f;

        // Перемещение игрока с помощью джойстика
        move = joystick.GetInput();
        Animator.SetFloat("speed", move.magnitude * maxSpeed);
        move = move * maxSpeed;
        Controller.Move(move * Time.smoothDeltaTime);

        //Реализует плавный поворот к цели с определённой скоростью
        void LookToTarget(Vector3 target, float speed)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Quaternion.LookRotation(target), Time.smoothDeltaTime * speed
            );
        }

        if (Input.GetKeyDown(KeyCode.I))
            invulnerability = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBase"))
            SetParams(true);
        if (other.CompareTag("PlayerBase"))
            inBase = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyBase"))
        {
            SetParams(false);
            lookToEnemy = Vector3.zero;
        }
        if (other.CompareTag("PlayerBase")) 
            inBase = false;
    }

    ///<summary>Вызывается при выборе оружия из магазина</summary>
    ///<param name="gunName">Выбранное оружие</param>
    public void SelectGun(GunName gunName)
    {
        gun = shop.TakeGun(gunName, gun);
        gun.transform.parent = gunSlot;
        gun.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        gun.transform.localScale = Vector3.one;
    }

    public override void Hit(float damage)
    {
        if (invulnerability)
            return;
        CurrentHealthPoints -= damage;
        if (CurrentHealthPoints == 0)
        {
            Animator.SetBool("alive", false);
            gun.gameObject.SetActive(false);
            this.enabled = false;
            Controller.enabled = false;
            Messenger.SendMessage(MessageType.DEATH_PLAYER);
        }
    }

    [Listener(MessageType.RESTART)]
    public void Resurrection()
    {
        Animator.SetBool("alive", true);
        Animator.SetBool("inEnemyBase", false);
        lookToEnemy = Vector3.zero;
        transform.SetPositionAndRotation(recoveryPoint.position, Quaternion.identity);
        this.enabled = true;
        Controller.enabled = true;
        CurrentHealthPoints = maxHealthPoints;
        inEnemyBase = false;
        inBase = true;
    }

    ///<returns>Возвращает ближайшего к игроку врага. Если рядом врагов нет - возвращает null</returns>
    GameObject GetNearestEnemy()
    {
        GameObject target = null;
        Collider[] enemies = Physics.OverlapSphere(
            transform.position, attackDistance, 1 << 3
        ); // 1 << 3 - маска слоя врага (3: EnemyLayer)
        float dist = Mathf.Infinity;
        foreach (Collider enemy in enemies)
        {
            Vector3 distance = enemy.transform.position - transform.position;
            if (distance.magnitude < dist)
            {
                target = enemy.gameObject;
                dist = distance.magnitude;
            }
        }

        return target;
    }

    ///<summary>Устанавливает параметры, связанные с нахождением на вражеской базе</summary>
    ///<param name="value">Значение, устанавливаемое необходимым параметрам</param>
    void SetParams(bool value)
    {
        inEnemyBase = value;
        gun.gameObject.SetActive(value);
        Animator.SetBool("inEnemyBase", value);
    }
}
