using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpriteLibrary;
using Microsoft.Xna.Framework;
using ChaosDrive.Extensions;

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
            float T = 1.0f - t;

            if (t <= 1.0f)
            {
                //Do Bezier logic here
                var nextPos = controlPoints[0].Multiply(Math.Pow(T, 3))
                                + controlPoints[1].Multiply(3 * Math.Pow(T, 2) * t)
                                + controlPoints[2].Multiply(3 * T * Math.Pow(t, 2))
                                + controlPoints[3].Multiply(Math.Pow(t, 3));

                velocity = nextPos - position;
                position = nextPos;
            }
            else
            {
                position += velocity;
            }

            ActiveSprite.Update(elapsedTime);
            ActiveSprite.Position = position;

            if (health <= 0) shouldRemove = true;
            if (currentTime >= runTime && !ActiveSprite.Bounds.Intersects(bounds)) shouldRemove = true;
        }
    }
}
