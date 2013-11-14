using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ChaosDriveContentLibrary
{
    class BezierCurveShootingEnemyData
    {
        #region Properties
        public List<Vector2> Points { get; set; }
        public float FlightTime { get; set; }
        public float ShotTime { get; set; }
        #endregion
    }
}
