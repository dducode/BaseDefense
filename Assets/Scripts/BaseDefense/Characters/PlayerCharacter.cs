using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;
using BaseDefense.AttackImplemention.Guns;
using BaseDefense.BroadcastMessages;
using BaseDefense.Items;
using BaseDefense.UI;
using UnityEngine.Profiling;

namespace BaseDefense.Characters
{
    [RequireComponent(typeof(ItemCollecting), typeof(DisplayHealthPoints))]
    public sealed class PlayerCharacter : BaseCharacter
    {
        ///<summary>Transform руки, в которой игрок держит оружие</summary>
        [Header("Связанные объекты")]
        [Tooltip("Transform руки, в которой игрок держит оружие")]
        [SerializeField] private Transform gunSlot;
        
        ///<inheritdoc cref="Upgrades"/>
        [Tooltip("Хранит в себе информацию о прокачиваемых характеристиках игрока")]
        [SerializeField] private Upgrades upgrades;

        ///<inheritdoc cref="DisplayHealthPoints"/>
        private DisplayHealthPoints m_displayHealthPoints;

        ///<inheritdoc cref="ItemCollecting"/>
        private ItemCollecting m_itemCollecting;

        ///<inheritdoc cref="BaseCharacter.maxHealthPoints"/>
        public float MaxHealthPoints => maxHealthPoints;

        ///<inheritdoc cref="BaseCharacter.maxSpeed"/>
        public float MaxSpeed => maxSpeed;

        ///<inheritdoc cref="ItemCollecting.Capacity"/>
        public int Capacity => m_itemCollecting.Capacity;
        
        private bool m_inEnemyBase;

        [Inject]
        public void Constructor(JoystickController joystick, Shop shop)
        {
            m_joystick = joystick;
            m_shop = shop;
        }

        public override void Hit(float damage)
        {
            CurrentHealthPoints -= damage;
            m_displayHealthPoints.UpdateView((int)CurrentHealthPoints);
            var emission = HitEffect.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, (int)damage * 100 / maxHealthPoints));
            HitEffect.Play();
        }
        
        ///<summary>Прокачивает характеристики игрока</summary>
        ///<param name="upgradeType">Определяет прокачиваемую характеристику</param>
        public void Upgrade(UpgradableProperties upgradeType)
        {
            switch (upgradeType)
            {
                case UpgradableProperties.Speed:
                    UpgradeSpeed();
                    break;
                case UpgradableProperties.Max_Health:
                    UpgradeMaxHealth();
                    break;
                case UpgradableProperties.Capacity:
                    m_itemCollecting.UpgradeCapacity(upgrades);
                    break;
                default:
                    throw new NotImplementedException($"Тип прокачки {upgradeType} не реализован");
            }
        }
        
        private Shop m_shop;
        private Gun m_gun;
        
        ///<summary>Вызывается при выборе оружия из магазина</summary>
        ///<param name="gunName">Выбранное оружие</param>
        public void SelectGun(string gunName)
        {
            m_gun = m_shop.TakeGun(gunName, m_gun);
            m_gun.transform.parent = gunSlot;
            m_gun.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        #region PlayerInitialization

        protected override void Awake()
        {
            base.Awake();
            m_itemCollecting = GetComponent<ItemCollecting>();
            m_displayHealthPoints = GetComponent<DisplayHealthPoints>();
            m_gun = gunSlot.GetChild(0).GetComponent<Gun>();
        }

        private void Start()
        {
            m_displayHealthPoints.SetMaxValue((int)maxHealthPoints);
            m_displayHealthPoints.UpdateView((int)CurrentHealthPoints);
            m_gun.gameObject.SetActive(false);
            m_gazeDirection = transform.forward;
        }

        #endregion

        #region PlayerUpdate

        private static readonly int SpeedId = Animator.StringToHash("speed");
        ///<summary>Направление взгляда в сторону атакуемой сущности</summary>
        private Vector3 m_gazeDirection;
        private Vector3 m_move;
        private JoystickController m_joystick;

        private void Update()
        {
            // Реализует плавный поворот к цели с определённой скоростью
            LookToTarget(m_gazeDirection, 15);

            Movement();
            
            if (m_inEnemyBase)
            {
                // Ожидание перехода анимации для предотвращения преждевременной атаки
                if (!Animator.IsInTransition(0))
                    Attack();
                // Если анимация ещё проигрывается - ничего не делаем
                else { }
            }
            else HealthRecovery();
            
            void LookToTarget(Vector3 target, float rotationSpeed)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, Quaternion.LookRotation(target), Time.smoothDeltaTime * rotationSpeed
                );
            }
        }
        
        private void Movement()
        {
            m_move = m_joystick.GetInput();
            Animator.SetFloat(SpeedId, m_move.magnitude * maxSpeed);
            m_move *= maxSpeed;
            Controller.Move(m_move * Time.smoothDeltaTime);
            m_gazeDirection = m_move == Vector3.zero ? transform.forward : m_move;
        }

        private void Attack()
        {
            Profiler.BeginSample("Attack");
            
            var attackable = GetNearestAttackable();
            if (attackable is null) return;
            
            var attackablePosition = attackable.transform.position;
            var playerPosition = transform.position;
            attackablePosition.y = playerPosition.y = 0;
            m_gazeDirection = attackablePosition - playerPosition;
            
            // Прицеливаемся
            const float aimingAccuracy = 0.95f;
            if (Vector3.Dot(m_gazeDirection.normalized, transform.forward) > aimingAccuracy) 
            {
                // Если стреляем из гранатомёта - определяем безопасное расстояние
                if (m_gun is GrenadeLauncher grenade)
                {
                    var playerTransform = transform;
                    var ray = new Ray(playerTransform.position + Vector3.up, playerTransform.forward);
                    
                    // Если расстояние до цели безопасно - стреляем
                    if (Physics.Raycast(ray, out var hit) && hit.distance > grenade.DamageRadius)
                        grenade.Shot();
                    // Иначе не стреляем
                    else { }
                }
                // Иначе стреляем сразу
                else m_gun.Shot();
            }
            // Если ещё не прицелились - ничего не делаем
            else { }
            
            Profiler.EndSample();
        }
        
        ///<returns>Возвращает ближайшую к игроку атакуемую сущность. Если рядом таких нет - возвращает null</returns>
        private GameObject GetNearestAttackable()
        {
            // ReSharper disable once Unity.PreferNonAllocApi
            var attackables = Physics.OverlapSphere(
                transform.position, attackDistance, 1 << 3
            ); // 1 << 3 - маска слоя атакуемой сущности (3: AttackableLayer)
            var enemies = attackables.Where((e) => e.GetComponent<EnemyCharacter>()).ToArray();
            
            // Приоритет отдаётся врагам. Если они не найдены - атакуем другие сущности
            return FindTarget(enemies.Any() ? enemies : attackables);

            GameObject FindTarget(IEnumerable targets)
            {
                GameObject target = null;
                
                float dist = Mathf.Infinity;
                foreach (Collider coll in targets)
                {
                    var distance = coll.transform.position - transform.position;
                    if (distance.magnitude < dist)
                    {
                        target = coll.gameObject;
                        dist = distance.magnitude;
                    }
                }
                
                return target;
            }
        }

        private void HealthRecovery()
        {
            const float recoveryTime = 30;
            CurrentHealthPoints += maxHealthPoints * Time.smoothDeltaTime / recoveryTime;
            m_displayHealthPoints.UpdateView((int)CurrentHealthPoints);
        }

        #endregion

        #region TriggerEvents
        
        private static readonly int InEnemyBase = Animator.StringToHash("inEnemyBase");
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("enemy field"))
                SetParams(true);

            if (other.GetComponent<Item>() is { } item)
                switch (item)
                {
                    case Gem gem:
                        m_itemCollecting.PutGem(gem);
                        break;
                    case Money money:
                        m_itemCollecting.StackMoney(money);
                        break;
                    default:
                        throw new NotImplementedException($"Обработка объекта {item} не реализована");
                }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("enemy field"))
            {
                SetParams(false);
                if (!m_itemCollecting.DropIsInProcess)
                    StartCoroutine(m_itemCollecting.DropMoney());
            }
        }
        
        ///<summary>Устанавливает параметры, связанные с нахождением на вражеской базе</summary>
        ///<param name="value">Значение, устанавливаемое необходимым параметрам</param>
        private void SetParams(bool value)
        {
            m_inEnemyBase = value;
            m_gun.gameObject.SetActive(value);
            Animator.SetBool(InEnemyBase, value);
        }
        
        #endregion

        #region PlayerUpgrades
        
        public bool IsNotMaxForSpeed => maxSpeed < upgrades.Speed.maxValue;
        public bool IsNotMaxForMaxHealth => maxHealthPoints < upgrades.MaxHealth.maxValue;
        public bool IsNotMaxForCapacity => Capacity < upgrades.Capacity.maxValue;

        private void UpgradeSpeed()
        {
            if (IsNotMaxForSpeed)
            {
                maxSpeed += upgrades.Speed.step;
                if (maxSpeed > upgrades.Speed.maxValue)
                    maxSpeed = upgrades.Speed.maxValue;
            }
            else
                Debug.LogWarning("Достигнут предел в прокачке максимальной скорости");
        }

        private void UpgradeMaxHealth()
        {
            if (IsNotMaxForMaxHealth)
            {
                maxHealthPoints += upgrades.MaxHealth.step;
                if (maxHealthPoints > upgrades.MaxHealth.maxValue)
                    maxHealthPoints = upgrades.MaxHealth.maxValue;
                CurrentHealthPoints = maxHealthPoints;
                m_displayHealthPoints.UpdateView((int)CurrentHealthPoints);
                m_displayHealthPoints.SetMaxValue((int)maxHealthPoints);
            }
            else
                Debug.LogWarning("Достигнут предел в прокачке максимального здоровья");
        }
        
        #endregion

        #region PlayerLifecycle
        
        private static readonly int Alive = Animator.StringToHash("alive");

        private void OnEnable() => Messenger.AddListener(MessageType.RESTART, Resurrection);
        private void OnDisable() => Messenger.RemoveListener(MessageType.RESTART, Resurrection);

        protected override void OnDeath()
        {
            Animator.SetBool(Alive, false);
            m_gun.gameObject.SetActive(false);
            Enabled = false;
            MeshRenderer.material.color = deathColor;
            Messenger.SendMessage(MessageType.DEATH_PLAYER);
        }
        
        private void Resurrection()
        {
            var respawn = GameObject.FindGameObjectWithTag("Respawn").transform;
            Animator.SetBool(Alive, true);
            Animator.SetBool(InEnemyBase, false);
            transform.SetPositionAndRotation(respawn.position, Quaternion.identity);
            Enabled = true;
            CurrentHealthPoints = maxHealthPoints;
            m_inEnemyBase = false;
            MeshRenderer.material.color = DefaultColor;
        }
        
        #endregion
    }
}


