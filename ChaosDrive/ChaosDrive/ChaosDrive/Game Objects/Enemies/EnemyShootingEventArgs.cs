using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChaosDrive.Game_Objects.Bullets;

namespace ChaosDrive.Game_Objects.Enemies
{
    public class EnemyShootingEventArgs
    {
        #region Properties
        public List<Bullet> Bullets
        {
            get;
            protected set;
        }
        #endregion

        #region Constructors
        public EnemyShootingEventArgs(List<Bullet> bullets)
            : base()
        {
            Bullets = new List<Bullet>();
            Bullets.AddRange(bullets);
        }
        #endregion
    }
}
