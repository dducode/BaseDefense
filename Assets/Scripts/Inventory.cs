using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Object;

public class Inventory
{
    int moneys;
    int gems;
    DisplayingUI UI;

    public Inventory(DisplayingUI UI)
    {
        this.UI = UI;
        moneys = PlayerPrefs.GetInt("Money", 0);
        gems = PlayerPrefs.GetInt("Gem", 0);
        UI.UpdateUI(moneys, gems);
    }

    public void PutItem(Item item)
    {
        if (item is Money)
        {
            moneys += 5;
            PlayerPrefs.SetInt("Money", moneys);
        }
        else if (item is Gem)
        {
            gems++;
            PlayerPrefs.SetInt("Gem", gems);
        }
        else
        {
            Debug.LogError($"Unknow item {item}");
            return;
        }
        item.DestroyItem();
        PlayerPrefs.Save();
        UI.UpdateUI(moneys, gems);
    }
}
