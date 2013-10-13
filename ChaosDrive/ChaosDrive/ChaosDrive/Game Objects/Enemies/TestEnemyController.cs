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
                Enemies.Add(new BasicEnemy(bounds, new Vector2(random.Next(700) + 50, 1)));
                buildUp -= enemySpawnChance;
            }
        }
    }
}
