using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ChaosDrive.Game_Objects.Enemies
{
    public class TestEnemyController : EnemyController
    {
        const int enemySpawnChance = 5000;
        int buildUp = 0;
        Random random;

        public TestEnemyController(Rectangle bounds)
            : base(bounds)
        {
            random = new Random();
        }

        public override void Update(float elapsedTime)
        {
            base.Update(elapsedTime);

            buildUp += (int)elapsedTime;
            if (random.Next(buildUp) > enemySpawnChance)
            {
                Enemies.Add(new BasicEnemy(bounds, new Vector2(random.Next(700) + 50, 1)));
                buildUp -= enemySpawnChance;
                Console.WriteLine("Ding!");
            }
        }
    }
}
