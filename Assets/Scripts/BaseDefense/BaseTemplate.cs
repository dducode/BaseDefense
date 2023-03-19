using System.Collections.Generic;
using UnityEngine;

namespace BaseDefense {

    public class BaseTemplate : Object {

        [SerializeField]
        private EnemyStation[] stations;

        [SerializeField]
        private Crystal[] crystals;

        public IEnumerable<EnemyStation> EnemyStations => stations;
        public IEnumerable<Crystal> Crystals => crystals;

    }

}