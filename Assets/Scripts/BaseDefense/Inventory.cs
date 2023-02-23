using System;
using UnityEngine;
using BaseDefense.Items;
using BaseDefense.UI;

namespace BaseDefense
{
    public class Inventory
    {
        private int m_moneys;
        private int m_gems;
        private readonly DisplayingUI m_ui;

        public Inventory(DisplayingUI ui)
        {
            m_ui = ui;
            m_moneys = PlayerPrefs.GetInt("Money", 0);
            m_gems = PlayerPrefs.GetInt("Gem", 0);
            ui.UpdateUI(m_moneys, m_gems);
        }

        ///<summary>Кладёт предмет в инвентарь и сохраняет значение в PlayerPrefs</summary>
        public void PutItem(Item item)
        {
            switch (item)
            {
                case Money:
                    m_moneys += 5;
                    PlayerPrefs.SetInt("Money", m_moneys);
                    break;
                case Gem:
                    m_gems++;
                    PlayerPrefs.SetInt("Gem", m_gems);
                    break;
                default:
                    throw new NotImplementedException($"Предмет {item} не реализован");
            }
            item.DestroyItem();
            PlayerPrefs.Save();
            m_ui.UpdateUI(m_moneys, m_gems);
        }
    }
}


