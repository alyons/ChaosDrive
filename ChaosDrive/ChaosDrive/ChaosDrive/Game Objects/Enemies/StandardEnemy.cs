using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpriteLibrary;
using ChaosDrive.Utility;

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
        List<float> shotTimes;
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
        public StandardEnemy(Rectangle bounds, List<Vector2[]> bezierCurves, List<float> runTimes, List<float> shotTimes)
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
            this.shotTimes = new List<float>();
            this.shotTimes.AddRange(shotTimes);
        }
        #endregion

        #region Methods
        public override void Update(float elapsedTime)
        {
            activeRunTime += elapsedTime;
            currentTime += elapsedTime;

            float t = activeRunTime / runTimes[currentCurve];
            if (t <= 1.0)
            {
                var nextPos = ChaosDriveMath.CalculateBezierCurveLocation(bezierCurves[currentCurve], t);

                velocity = nextPos - position;
                position = nextPos;
            }
            else
            {
                currentCurve++;
                if (currentCurve < bezierCurves.Count)
                {
                    t -= 1.0f;
                    var nextPos = ChaosDriveMath.CalculateBezierCurveLocation(bezierCurves[currentCurve], t);

                    velocity = nextPos - position;
                    position = nextPos;
                }
                else
                {
                    position += velocity;
                }
            }

            if (shotTimes.Count > 0)
            {
                if (currentTime > shotTimes.First())
                {
                    OnShotsFired();
                    shotTimes.RemoveAt(0);
                }
            }

            UpdateHitEffect(elapsedTime);

            ActiveSprite.Update(elapsedTime);
            ActiveSprite.Position = position;

            if (health <= 0) shouldRemove = true;
            if (currentCurve >= bezierCurves.Count && activeRunTime >= runTimes.Last() && !ActiveSprite.Bounds.Intersects(bounds)) shouldRemove = true;
        }
        public override void DisposeObjects()
        {
            EnemyController = null;
            sprite.Dispose();

            base.DisposeObjects();
        }
        #endregion
    }
}
