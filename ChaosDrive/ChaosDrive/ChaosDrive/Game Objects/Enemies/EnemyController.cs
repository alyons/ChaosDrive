using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChaosDrive.Game_Objects.Enemies
{
    public class EnemyController
    {
        #region Variables
        List<Enemy> enemies;
        protected Rectangle bounds;
        #endregion

        #region Properties
        public List<Enemy> Enemies
        {
            get { return enemies; }
        }
        #endregion

        #region Constructors
        public EnemyController(Rectangle bounds)
        {
            enemies = new List<Enemy>();
            this.bounds = bounds;
        }
        #endregion

        #region Methods
        public virtual void Update(float elapsedTime)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(elapsedTime);
            }

            enemies.RemoveAll(e => e.ShouldRemove);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }
        #endregion
    }
}
