using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static UnityEngine.Object;

public static class Game
{
    public static Scene ItemsScene { get; private set; }
    public static Scene EnemiesScene { get; private set; }
    public static Scene ProjectilesScene { get; private set; }

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        ItemsScene = SceneManager.CreateScene("Items");
        EnemiesScene = SceneManager.CreateScene("Enemies");
        ProjectilesScene = SceneManager.CreateScene("Projectiles");
    }
}
