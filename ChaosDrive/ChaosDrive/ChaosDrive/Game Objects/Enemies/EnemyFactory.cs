using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChaosDriveContentLibrary;
using System.Web.Script.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ChaosDrive.Game_Objects.Enemies
{
    public class EnemyFactory
    {
        #region Variables
        protected static EnemyFactory _me;
        protected static Rectangle bounds;
        #endregion

        #region Properties
        public static Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }
        #endregion

        #region Constructors
        static EnemyFactory()
        {
            _me = new EnemyFactory();
        }
        protected EnemyFactory()
        {
            
        }
        #endregion

        #region Methods
        public static Enemy ParseEnemyData(EnemyData enemyData)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            switch (enemyData.enemyType)
            {
                case "StandardEnemy":
                    {
                        StandardEnemyData someData = jsonSerializer.Deserialize<StandardEnemyData>(enemyData.data);
                        return new StandardEnemy(bounds, someData.BezierCurves, someData.RunTimes, someData.Bullets);
                    }
                case "BezierCurveEnemy":
                    {
                        BezierCurveEnemyData someData = jsonSerializer.Deserialize<BezierCurveEnemyData>(enemyData.data);
                        return new BezierCurveEnemy(bounds, someData.Points, someData.Time);
                    }
                case "SimpleBoss":
                    {
                        return new SimpleBoss(bounds);
                    }
                default:
                    {
                        return new BasicEnemy(bounds, jsonSerializer.Deserialize<Vector2>(enemyData.data));
                    }
            }
        }
        #endregion
    }
}
