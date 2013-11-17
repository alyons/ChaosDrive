using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ChaosDriveContentLibrary;
using System.Web.Script.Serialization;

namespace ChaosDrive.Game_Objects.Bullets
{
    public class BulletFactory
    {
        #region Variables
        static BulletFactory _me;
        static Vector2 playerPosition;
        static Rectangle bounds;
        #endregion

        #region Properties
        public static Vector2 PlayerPosition
        {
            protected get { return playerPosition; }
            set { playerPosition = value; }
        }
        public static Rectangle Bounds
        {
            set { bounds = value; }
        }
        #endregion

        #region Constructors
        static BulletFactory()
        {
            _me = new BulletFactory();
        }
        protected BulletFactory()
        {
        }
        #endregion

        #region Methods
        public static Bullet GenerateEnemyBulletFromData(Vector2 position, EnemyBulletData data)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            switch (data.Target)
            {
                default:
                    return new DirectedEnemyBullet(position, playerPosition, bounds);
            }
        }
        #endregion
    }
}
