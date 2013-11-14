using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ChaosDrive.Game_Objects.Bullets;
using ChaosDrive.Game_Objects.Effects;
using ChaosDrive.Extensions;
using ChaosDrive.Game_Objects.Player;

namespace ChaosDrive.Game_Objects.Enemies
{
    public class EnemyController
    {
        #region Variables
        List<Enemy> enemies;
        protected Rectangle bounds;
        protected Random random;
        #endregion

        #region Properties
        public bool LevelFinished
        {
            get;
            set;
        }
        public List<Enemy> Enemies
        {
            get { return enemies; }
        }
        BulletController BulletController
        {
            get;
            set;
        }
        ParticleController ParticleController
        {
            get;
            set;
        }
        PlayerController PlayerController
        {
            get;
            set;
        }
        public Vector2 PlayerPosition
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public EnemyController(Rectangle bounds, BulletController bulletController, ParticleController particleController)
        {
            enemies = new List<Enemy>();
            random = new Random();
            BulletController = bulletController;
            ParticleController = particleController;
            PlayerPosition = new Vector2(0, 0);
            LevelFinished = false;
            this.bounds = bounds;
        }
        #endregion

        #region Methods
        public virtual void Update(float elapsedTime)
        {
            foreach(Enemy enemy in enemies.FindAll(e => e.ShouldRemove))
            {
                if (enemy.Health <= 0)
                {
                    //Dictionary<Color, int> pieces = enemy.ActiveSprite.GetPixelDictionary();
                    //int highNumber = 0;
                    //foreach (Color key in pieces.Keys)
                        //highNumber += pieces[key];

                    var center = enemy.ActiveSprite.Position;

                    for (int i = 0; i < 30; i++)
                    {
                        double rot = random.NextDouble() * Math.PI * 2.0;
                        var partVel = new Vector2((float)(Math.Cos(rot) * 6.0), (float)(Math.Sin(rot) * 4.0 - 2.0));
                        ParticleController.Particles.Add(new Particle(center, partVel, Color.Red));
                    }
                }

                enemy.ShotsFired -= Enemy_ShotsFired;
                //enemy.DisposeObjects();
            }

            enemies.RemoveAll(e => e.ShouldRemove);

            foreach (Enemy enemy in enemies)
            {
                enemy.Update(elapsedTime);
            }

            CollideWithBullets();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }
        public virtual void CollideWithBullets()
        {
            foreach (Bullet bullet in BulletController.Bullets)
            {
                foreach (Enemy enemy in enemies)
                {
                    enemy.Collide(bullet);
                }
            }
        }
        public virtual void Enemy_ShotsFired(object sender, EnemyShootingEventArgs e)
        {
            BulletController.AddBullets(e.Bullets);
        }
        #endregion
    }
}
