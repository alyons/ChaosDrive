using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ChaosDrive.Game_Objects.Bullets;
using ChaosDrive.Game_Objects.Effects;
using ChaosDriveContentLibrary;

namespace ChaosDrive.Game_Objects.Enemies
{
    public class QueuedEnemyController : EnemyController
    {
        #region Variables
        float totalTime;
        List<EnemyData> queue;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public QueuedEnemyController(Rectangle bounds, BulletController bulletController, ParticleController particleController, List<EnemyData> enemyData)
            : base(bounds, bulletController, particleController)
        {
            queue = enemyData.OrderBy(e => e.appearanceTime).ToList();
        }
        #endregion

        #region Methods
        public override void Update(float elapsedTime)
        {
            base.Update(elapsedTime);

            totalTime += elapsedTime;

            if (queue.Count > 0)
            {
                while (totalTime >= queue[0].appearanceTime)
                {
                    var enemy = EnemyFactory.ParseEnemyData(queue[0]);
                    enemy.ShotsFired += Enemy_ShotsFired;
                    Enemies.Add(enemy);
                    queue.RemoveAt(0);
                    if (queue.Count == 0) break;
                }
            }

            LevelFinished = queue.Count == 0 && Enemies.Count == 0;
        }
        #endregion
    }
}
