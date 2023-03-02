using UnityEngine;

namespace BaseDefense
{
    public class BaseTemplate : Object
    {
        [SerializeField] private EnemyStation[] stations;
        [SerializeField] private Crystal[] crystals;

        public EnemyStation[] EnemyStations => stations;
        public Crystal[] Crystals => crystals;
    }
}