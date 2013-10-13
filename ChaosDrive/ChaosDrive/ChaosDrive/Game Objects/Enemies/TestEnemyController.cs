using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ChaosDrive.Game_Objects.Bullets;
using ChaosDrive.Game_Objects.Effects;

namespace ChaosDrive.Game_Objects.Enemies
{
    public class TestEnemyController : EnemyController
    {
        const int enemySpawnChance = 5000;
        int buildUp = 0;

        public TestEnemyController(Rectangle bounds, BulletController bulletController, ParticleController particleController)
            : base(bounds, bulletController, particleController)
        {
        }

        public override void Update(float elapsedTime)
        {
            base.Update(elapsedTime);

            buildUp += (int)elapsedTime;
            if (random.Next(buildUp) > enemySpawnChance)
            {
                switch (random.Next(3))
                {
                    case 0:
                        CreateBezierEnemy();
                        break;
                    case 1:
                        CreateBezierEnemyEX();
                        break;
                    default:
                        Enemies.Add(new BasicEnemy(bounds, new Vector2(random.Next(700) + 50, 1)));
                        break;
                }
                
                buildUp -= enemySpawnChance;
            }
        }

        void CreateBezierEnemy()
        {
            var controlPoints = new Vector2[] { new Vector2(0, 0), new Vector2(0, bounds.Bottom), new Vector2(bounds.Right, 0), new Vector2(bounds.Right, bounds.Bottom) };
            Enemies.Add(new BezierCurveEnemy(bounds, controlPoints, 5000));
        }

        void CreateBezierEnemyEX()
        {
            var controlPoints = new Vector2[] { new Vector2(bounds.Right, 0), new Vector2(bounds.Right / 2f, bounds.Bottom), new Vector2(bounds.Right / 2f, bounds.Bottom), new Vector2(0, 0) };
            Enemies.Add(new BezierCurveEnemy(bounds, controlPoints, 5000));
        }
    }
}
