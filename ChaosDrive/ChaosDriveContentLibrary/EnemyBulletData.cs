using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChaosDriveContentLibrary
{
    public class EnemyBulletData
    {
        #region Properties
        public float Damage { get; set; }
        public float LaunchTime { get; set; }
        public string Target { get; set; }
        public object Data { get; set; }
        #endregion
    }
}
