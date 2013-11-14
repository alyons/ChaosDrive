using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpriteLibrary;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ChaosDrive.Utility;

namespace ChaosDrive.Game_Objects.Enemies
{
    public class SimpleBoss : Enemy
    {
        #region Variables
        public static Sprite baseSprite;
        Sprite sprite;
        List<Vector2[]> paths;
        List<float> pathTimes;
        int activePath;
        float workingTime;
        #endregion

        #region Properties
        public override Sprite ActiveSprite
        {
            get { return sprite; }
        } 
        #endregion

        #region Constructors
        public SimpleBoss(Rectangle bounds)
            : base(bounds, new Vector2(-100, -100), 1000)
        {
            Initialize();
        }
        #endregion

        #region Methods
        void Initialize()
        {
            sprite = baseSprite.Copy();
            paths = new List<Vector2[]>();
            pathTimes = new List<float>();
            workingTime = 0.0f;

            #region Create all paths
            var a = new Vector2(sprite.Bounds.Width / 2f, bounds.Height / 2f);
            var b = new Vector2(0.25f * (bounds.Width - (sprite.Bounds.Width / 2f)), -(bounds.Height / 2f));
            var c = new Vector2(0.25f * (bounds.Width - (sprite.Bounds.Width / 2f)), 3 * (bounds.Height / 2f));
            var d = new Vector2(0.75f * (bounds.Width - (sprite.Bounds.Width / 2f)), -(bounds.Height / 2f));
            var e = new Vector2(0.75f * (bounds.Width - (sprite.Bounds.Width / 2f)), 3 * (bounds.Height / 2f));
            var f = new Vector2(bounds.Width - (sprite.Bounds.Width / 2f), bounds.Height / 2f);
            paths.Add(new Vector2[] { position, a });
            pathTimes.Add(2500);
            paths.Add(new Vector2[] { a, b, e, f });
            pathTimes.Add(5000);
            paths.Add(new Vector2[] { f, d, c, a });
            pathTimes.Add(5000);
            #endregion

            activePath = 0;
        }

        public override void Update(float elapsedTime)
        {
            if (!shouldRemove)
            {
                workingTime += elapsedTime;
            }

            if (workingTime > pathTimes[activePath])
            {
                workingTime -= pathTimes[activePath];
                if (activePath == 0 || activePath == 2)
                {
                    activePath = 1;
                }
                else
                {
                    activePath = 2;
                }
            }

            float t = workingTime / pathTimes[activePath];
            position = ChaosDriveMath.CalculateBezierCurveLocation(paths[activePath], t);

            UpdateHitEffect(elapsedTime);

            ActiveSprite.Update(elapsedTime);
            ActiveSprite.Position = position;

            if (health <= 0) shouldRemove = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public override void DisposeObjects()
        {
            sprite.Dispose();

            base.DisposeObjects();
        }
        #endregion
    }
}
