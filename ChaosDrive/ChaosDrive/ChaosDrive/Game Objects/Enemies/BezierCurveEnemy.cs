using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpriteLibrary;
using Microsoft.Xna.Framework;
using ChaosDrive.Extensions;
using ChaosDrive.Utility;

namespace ChaosDrive.Game_Objects.Enemies
{
    public class BezierCurveEnemy : Enemy
    {
        public static Sprite baseSprite;
        Sprite sprite;
        Vector2 velocity;
        Vector2[] controlPoints;
        float runTime;
        float currentTime;

        public override Sprite ActiveSprite
        {
            get { return sprite; }
        }

        public BezierCurveEnemy(Rectangle bounds, Vector2[] bezPoints, float time)
            : base (bounds, bezPoints[0], 100)
        {
            velocity = new Vector2();
            sprite = baseSprite.Copy();
            controlPoints = bezPoints;
            runTime = time;
            currentTime = 0;
        }

        public override void Update(float elapsedTime)
        {
            currentTime += elapsedTime;

            float t = currentTime / runTime;

            if (t <= 1.0f)
            {
                //Do Bezier logic here
                var nextPos = ChaosDriveMath.CalculateBezierCurveLocation(controlPoints, t);

                velocity = nextPos - position;
                position = nextPos;
            }
            else
            {
                position += velocity;
            }

            UpdateHitEffect(elapsedTime);

            ActiveSprite.Update(elapsedTime);
            ActiveSprite.Position = position;

            if (health <= 0) shouldRemove = true;
            if (currentTime >= runTime && !ActiveSprite.Bounds.Intersects(bounds)) shouldRemove = true;
        }
        public override void DisposeObjects()
        {
            sprite.Dispose();

            base.DisposeObjects();
        }
    }
}
