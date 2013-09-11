using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ChaosDrive.Game_Objects.Bullets
{
    public class BulletController
    {
        #region Variables
        List<Bullet> bullets;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public BulletController()
        {
            bullets = new List<Bullet>();
        }
        #endregion

        #region Methods
        public void Update(float elapsedTime)
        {
            foreach (Bullet bullet in bullets)
            {
                bullet.Update(elapsedTime);
            }

            bullets.RemoveAll(b => b.ShouldRemove);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }
        public void AddBullets(IEnumerable<Bullet> newBullets)
        {
            bullets.AddRange(newBullets);
        }
        #endregion
    }
}
