using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ChaosDriveContentLibrary
{
    public class StandardEnemyData
    {
        #region Properties
        public List<Vector2[]> BezierCurves { get; set; }
        public List<float> RunTimes { get; set; }
        public List<EnemyBulletData> Bullets { get; set; }
        #endregion
    }
}
