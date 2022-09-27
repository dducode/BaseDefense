using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory instance;

    public static Inventory getInstance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Inventory>();
            return instance;
        }
    }

    void Awake()
    {
        money = PlayerPrefs.GetInt("Money", 0);
        gems = PlayerPrefs.GetInt("Gem", 0);
        ui = FindObjectOfType<DisplayingUI>();
    }

    int money;
    int gems;
    public int getMoney { get { return money; } }
    public int getGems { get { return gems; } }

    DisplayingUI ui;

    public void CollectItem(GameObject item)
    {
        if (item.CompareTag("Money")) money += 5;
        if (item.CompareTag("Gem")) gems++;

        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.SetInt("Gem", gems);
        ui.UpdateUI();
    }
}
