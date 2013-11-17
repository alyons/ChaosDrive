using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpriteLibrary;
using ChaosDrive.Utility;
using ChaosDriveContentLibrary;
using ChaosDrive.Game_Objects.Bullets;

namespace ChaosDrive.Game_Objects.Enemies
{
    public class StandardEnemy : Enemy
    {
        #region Variables
        public static Sprite baseSprite;
        Sprite sprite;
        Vector2 velocity;
        List<Vector2[]> bezierCurves;
        List<float> runTimes;
        List<EnemyBulletData> bullets;
        float currentTime;
        float activeRunTime;
        int currentCurve;
        #endregion

        #region Properties
        public override Sprite ActiveSprite
        {
            get { return sprite; }
        }
        EnemyController EnemyController
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public StandardEnemy(Rectangle bounds, List<Vector2[]> bezierCurves, List<float> runTimes, List<EnemyBulletData> bullets)
            : base (bounds, bezierCurves[0][0], 100)
        {
            velocity = new Vector2();
            sprite = baseSprite.Copy();
            currentCurve = 0;
            currentTime = 0f;
            activeRunTime = 0f;
            this.bezierCurves = new List<Vector2[]>();
            this.bezierCurves.AddRange(bezierCurves);
            this.runTimes = new List<float>();
            this.runTimes.AddRange(runTimes);
            this.bullets = new List<EnemyBulletData>();
            this.bullets.AddRange(bullets);
        }
        #endregion

        #region Methods
        public override void Update(float elapsedTime)
        {
            activeRunTime += elapsedTime;
            currentTime += elapsedTime;

            if (currentCurve < bezierCurves.Count)
            {
                float t = activeRunTime / runTimes[currentCurve];
                if (t <= 1.0)
                {
                    var nextPos = ChaosDriveMath.CalculateBezierCurveLocation(bezierCurves[currentCurve], t);

                    velocity = nextPos - position;
                    position = nextPos;
                }
                else
                {
                    activeRunTime -= runTimes[currentCurve];
                    currentCurve++;
                    if (currentCurve < bezierCurves.Count)
                    {
                        t = activeRunTime / runTimes[currentCurve];
                        var nextPos = ChaosDriveMath.CalculateBezierCurveLocation(bezierCurves[currentCurve], t);

                        velocity = nextPos - position;
                        position = nextPos;
                    }
                }
            }
            else
            {
                position += velocity;
            }

            if (bullets.Count > 0)
            {
                if (bullets.Any(b => b.LaunchTime < currentTime))
                {
                    OnShotsFired();
                }
            }

            UpdateHitEffect(elapsedTime);

            ActiveSprite.Update(elapsedTime);
            ActiveSprite.Position = position;

            if (health <= 0) shouldRemove = true;
            if (currentCurve >= bezierCurves.Count && activeRunTime >= runTimes.Last() && !ActiveSprite.Bounds.Intersects(bounds)) shouldRemove = true;
        }
        protected override void OnShotsFired()
        {
            var bulletList = new List<Bullet>();
            foreach(EnemyBulletData data in bullets.FindAll(b => b.LaunchTime < currentTime))
                bulletList.Add(BulletFactory.GenerateEnemyBulletFromData(position, data));

            bullets.RemoveAll(b => b.LaunchTime < currentTime);

            OnShotsFired(new EnemyShootingEventArgs(bulletList));
        }
        #endregion
    }
}
