using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteLibrary;
using ChaosDrive.Extensions;

namespace ChaosDrive.Game_Objects.Background
{
    public class TestBackgroundController : BackgroundController
    {
        #region Variables
        Rectangle bounds;
        List<Star> stars;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public TestBackgroundController(Rectangle bounds)
        {
            this.bounds = bounds;
            stars = new List<Star>();
            Random random = new Random();
            for (int i = 0; i < 150; i++)
            {
                stars.Add(new Star(new Vector2(random.Next(bounds.Width), random.Next(bounds.Height)), new Vector2(0, random.Next(Star.MIN_VELOCITY, Star.MAX_VELOCITY)), bounds));
            }
        }
        #endregion

        #region Methods
        public override void Update(float elapsedTime)
        {
            foreach (Star star in stars) star.Update(elapsedTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Star star in stars) star.Draw(spriteBatch);
        }
        #endregion
    }

    public class Star
    {
        public static Texture2D texture;
        static Random random = new Random();
        Rectangle bounds;
        Vector2 position;
        Vector2 velocity;
        Color color;
        public const int MAX_VELOCITY = 200;
        public const int MIN_VELOCITY = 50;

        public Star(Vector2 pos, Vector2 vel, Rectangle bnds)
        {
            position = pos;
            velocity = vel;
            bounds = bnds;
            color = new Color(1.0f, 1.0f, 1.0f, vel.Y / MAX_VELOCITY);
        }

        public void Update(float elapsedTime)
        {
            position += velocity.Multiply(elapsedTime / 1000.0f);
            if (position.Y > bounds.Height)
            {
                position.Y = -texture.Height;
                position.X = random.Next(bounds.Width);
                velocity.Y = random.Next(MIN_VELOCITY, MAX_VELOCITY);
                color = new Color(1.0f, 1.0f, 1.0f, velocity.Y / MAX_VELOCITY);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(texture, position, color);

            spriteBatch.End();
        }
    }
}
