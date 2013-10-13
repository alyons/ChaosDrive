using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ChaosDrive.Game_Objects.Enemies;

namespace ChaosDrive.Game_Objects.Bullets
{
    public class BulletController
    {
        #region Variables
        List<Bullet> bullets;
        #endregion

        #region Properties
        public List<Bullet> Bullets
        {
            get { return bullets; }
        }
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

            bullets.RemoveAll(b => b.ShouldRemove == true);

            if (bullets.Exists(b => b.ShouldRemove == true))
            {
                Console.WriteLine("There are bad bullets in here!");
            }
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
        public void CollideWithEnemies(IEnumerable<Enemy> enemies)
        {
            foreach (Bullet bullet in bullets)
                foreach (Enemy enemy in enemies)
                    if (bullet is PlayerBullet)
                        if (bullet.Collide(enemy))
                            bullet.ShouldRemove = true;
        }
        #endregion
    }
}
