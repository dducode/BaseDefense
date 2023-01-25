using UnityEngine;

namespace BaseDefense
{
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

        ///<summary>Кладёт предмет в инвентарь и сохраняет значение в PlayerPrefs</summary>
        ///<remarks>
        ///Если тип предмета неизвестен - выводится сообщение в консоль с ошибкой. Сохранение значения не происходит
        ///</remarks>
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
            item.Destroy();
            PlayerPrefs.Save();
            UI.UpdateUI(moneys, gems);
        }
    }
}


