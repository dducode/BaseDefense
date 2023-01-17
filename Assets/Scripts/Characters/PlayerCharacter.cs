using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Zenject;
using BroadcastMessages;

[RequireComponent(typeof(ItemCollecting), typeof(DisplayHealthPoints))]
public class PlayerCharacter : BaseCharacter
{
    ///<summary>Transform руки, в которой игрок держит оружие</summary>
    [Tooltip("Transform руки, в которой игрок держит оружие")]
    [Header("Связанные объекты")]
    [SerializeField] Transform gunSlot;

    ///<summary>Точка респавна игрока</summary>
    [Tooltip("Точка респавна игрока")]
    [SerializeField] Transform recoveryPoint;

    ///<inheritdoc cref="Upgrades"/>
    [Tooltip("Хранит в себе информацию о прокачиваемых характеристиках игрока")]
    [SerializeField] Upgrades upgrades = new Upgrades();

    ///<inheritdoc cref="DisplayHealthPoints"/>
    DisplayHealthPoints displayHealthPoints;

    ///<inheritdoc cref="ItemCollecting"/>
    ItemCollecting itemCollecting;

    ///<inheritdoc cref="BaseCharacter.maxHealthPoints"/>
    public float MaxHealthPoints => maxHealthPoints;

    ///<inheritdoc cref="BaseCharacter.maxSpeed"/>
    public float MaxSpeed => maxSpeed;

    ///<inheritdoc cref="ItemCollecting.Capacity"/>
    public int Capacity => itemCollecting.Capacity;

    public bool IsNotMaxForSpeed => maxSpeed < upgrades.Speed.maxValue;
    public bool IsNotMaxForMaxHealth => maxHealthPoints < upgrades.MaxHealth.maxValue;
    public bool IsNotMaxForCapacity => Capacity < upgrades.Capacity.maxValue;

    ///<summary>Направление взгляда в сторону атакуемой сущности</summary>
    Vector3 lookToAttackable;
    Vector3 move;

    JoystickController joystick;
    Shop shop;
    Gun gun;
    bool inEnemyBase;

    [Inject]
    public void Construct(Shop shop, JoystickController joystick)
    {
        this.shop = shop;
        this.joystick = joystick;
    }

    public override void Awake()
    {
        base.Awake();
        itemCollecting = GetComponent<ItemCollecting>();
        displayHealthPoints = GetComponent<DisplayHealthPoints>();
        displayHealthPoints.SetMaxValue((int)maxHealthPoints);
        displayHealthPoints.UpdateView((int)CurrentHealthPoints);
        gun = gunSlot.GetChild(0).GetComponent<Gun>();
        gun.gameObject.SetActive(false);
    }

    void Update()
    {
        if (lookToAttackable != Vector3.zero)
            LookToTarget(lookToAttackable, 15);
        else if (move != Vector3.zero)
            LookToTarget(move, 15);

        if (inEnemyBase && !Animator.IsInTransition(0)) // Ожидание перехода для предотвращения преждевременной стрельбы
        // Поиск ближайшего врага на вражеской базе и атака
        {
            GameObject attackable = GetNearestAttackable();
            if (attackable is null)
                lookToAttackable = Vector3.zero;
            else
            {
                Vector3 attackablePosition = attackable.transform.position;
                Vector3 playerPosition = transform.position;
                attackablePosition.y = playerPosition.y = 0;
                lookToAttackable = attackablePosition - playerPosition;
                if (Vector3.Dot(lookToAttackable.normalized, transform.forward) > .95f) // Прицеливание
                {
                    if (gun is GrenadeLauncher grenade)
                    // Стреляет из гранатомёта только на безопасном расстоянии
                    {
                        if (lookToAttackable.magnitude > grenade.DamageRadius + 1)
                            grenade.Shot(attackable.transform.position + Vector3.up);
                    }
                    else
                        gun.Shot(attackable.transform.position + Vector3.up);
                }
            }
        }
        else 
        // Восстановление здоровья на своей базе
        {
            CurrentHealthPoints += Time.smoothDeltaTime * maxHealthPoints / 20;
            displayHealthPoints.UpdateView((int)CurrentHealthPoints);
        }

        // Перемещение игрока с помощью джойстика
        move = joystick.GetInput();
        Animator.SetFloat("speed", move.magnitude * maxSpeed);
        move = move * maxSpeed;
        Controller.Move(move * Time.smoothDeltaTime);

        // Реализует плавный поворот к цели с определённой скоростью
        void LookToTarget(Vector3 target, float speed)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Quaternion.LookRotation(target), Time.smoothDeltaTime * speed
            );
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBase"))
            SetParams(true);
        if (other.GetComponent<Gem>() is Gem gem)
            itemCollecting.PutGem(gem);
        if (other.GetComponent<Money>() is Money money)
            itemCollecting.StackMoney(money);
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyBase"))
        {
            SetParams(false);
            lookToAttackable = Vector3.zero;
            if (!itemCollecting.DropIsInProcess)
                StartCoroutine(itemCollecting.DropMoney());
        }
    }

    ///<summary>Вызывается при выборе оружия из магазина</summary>
    ///<param name="gunName">Выбранное оружие</param>
    public void SelectGun(GunName gunName)
    {
        gun = shop.TakeGun(gunName, gun);
        gun.transform.parent = gunSlot;
        gun.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    ///<summary>Прокачивает характеристики игрока</summary>
    ///<param name="upgradeType">Определяет прокачиваемую характеристику</param>
    public void Upgrade(UpgradeTypes upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeTypes.Upgrade_Speed:
                if (maxSpeed < upgrades.Speed.maxValue)
                {
                    maxSpeed += upgrades.Speed.step;
                    if (maxSpeed > upgrades.Speed.maxValue)
                        maxSpeed = upgrades.Speed.maxValue;
                }
                break;
            case UpgradeTypes.Upgrade_Max_Health:
                if (maxHealthPoints < upgrades.MaxHealth.maxValue)
                {
                    maxHealthPoints += upgrades.MaxHealth.step;
                    if (maxHealthPoints > upgrades.MaxHealth.maxValue)
                        maxHealthPoints = upgrades.MaxHealth.maxValue;
                    CurrentHealthPoints = maxHealthPoints;
                    displayHealthPoints.UpdateView((int)CurrentHealthPoints);
                    displayHealthPoints.SetMaxValue((int)maxHealthPoints);
                }
                break;
            case UpgradeTypes.Upgrade_Capacity:
                itemCollecting.UpgradeCapacity(upgrades.Capacity.step, upgrades);
                break;
        }
    }

    public override void Hit(float damage)
    {
        CurrentHealthPoints -= damage;
        displayHealthPoints.UpdateView((int)CurrentHealthPoints);
        var emission = HitEffect.emission;
        emission.SetBurst(0, new ParticleSystem.Burst(0, (int)damage * 100 / maxHealthPoints));
        HitEffect.Play();
    }

    protected override void DestroyCharacter()
    {
        Animator.SetBool("alive", false);
        gun.gameObject.SetActive(false);
        enabled = false;
        Messenger.SendMessage(MessageType.DEATH_PLAYER);
    }

    [Listener(MessageType.RESTART)]
    public void Resurrection()
    {
        Animator.SetBool("alive", true);
        Animator.SetBool("inEnemyBase", false);
        lookToAttackable = Vector3.zero;
        transform.SetPositionAndRotation(recoveryPoint.position, Quaternion.identity);
        enabled = true;
        CurrentHealthPoints = maxHealthPoints;
        inEnemyBase = false;
    }

    ///<returns>Возвращает ближайшую к игроку атакуемую сущность. Если рядом таких нет - возвращает null</returns>
    GameObject GetNearestAttackable()
    {
        GameObject target = null;
        Collider[] attackables = Physics.OverlapSphere(
            transform.position, attackDistance, 1 << 3
        ); // 1 << 3 - маска слоя атакуемой сущности (3: AttackableLayer)
        float dist = Mathf.Infinity;
        foreach (Collider attackable in attackables)
        {
            Vector3 distance = attackable.transform.position - transform.position;
            if (distance.magnitude < dist)
            {
                target = attackable.gameObject;
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
