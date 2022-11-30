using UnityEngine;
using System.Collections.Generic;

public static class Pools
{
    static Stack<EnemyCharacter> enemiesPool;
    static Stack<Money> moneysPool;
    static Stack<Gem> gemsPool;
    static Stack<Bullet> bulletsPool;

    public static int EnemiesCount => enemiesPool.Count;
    public static int MoneysCount => moneysPool.Count;
    public static int GemsCount => gemsPool.Count;
    public static int BulletsCount => bulletsPool.Count;

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        enemiesPool = new Stack<EnemyCharacter>();
        moneysPool = new Stack<Money>();
        gemsPool = new Stack<Gem>();
        bulletsPool = new Stack<Bullet>();
    }

    public static void Push(EnemyCharacter value)
    {
        enemiesPool.Push(value);
    }
    public static void Push(Money value)
    {
        moneysPool.Push(value);
    }
    public static void Push(Gem value)
    {
        gemsPool.Push(value);
    }
    public static void Push(Bullet value)
    {
        bulletsPool.Push(value);
    }

    public static EnemyCharacter PopEnemy()
    {
        return enemiesPool.Pop();
    }
    public static Money PopMoney()
    {
        return moneysPool.Pop();
    }
    public static Gem PopGem()
    {
        return gemsPool.Pop();
    }
    public static Bullet PopBullet()
    {
        return bulletsPool.Pop();
    }
}
