using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ChaosDrive.Game_Objects.Effects
{
    public class Particle
    {
        #region Variables
        public static Texture2D baseTexture;
        Texture2D texture;
        Rectangle rectangle;
        Vector2 position, velocity, midpoint, scale;
        double rotation;
        bool shouldRemove;
        float age;
        Color color;
        #endregion

        #region Properties
        public double Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        Color AgedColor
        {
            get
            {
                return new Color(color.R * (age / 255.0f), color.G * (age / 255.0f), color.B * (age / 255.0f), age);
            }
        }
        Effect Effect
        {
            get;
            set;
        }
        public bool ShouldRemove
        {
            get { return shouldRemove; }
        }
        #endregion

        #region Constructors
        public Particle(Vector2 pos, Vector2 vel, Color col)
        {
            age = 255;
            position = pos;
            velocity = vel;
            color = col;
            texture = baseTexture;
            rectangle = new Rectangle((int)pos.X, (int)pos.Y, texture.Width / 4, texture.Width / 4);
            midpoint = new Vector2(rectangle.Width / 2.0f, rectangle.Height / 2.0f);
            rotation = 0;
            scale = new Vector2(0.1f, 0.1f);
            shouldRemove = false;
        }
        #endregion

        #region Methods
        public void Update(float elapsedTime)
        {
            //Age the particle
            age -= (255.0f / 3.0f) * (elapsedTime / 1000.0f);

            //Move the Particle
            rotation = Math.Atan2(velocity.Y, velocity.X);
            scale.X = Math.Abs(0.1f * velocity.Length());
            position += velocity * (elapsedTime / 1000.0f);

            //Test to see if the particle is alive
            if (age <= 0)
            {
                shouldRemove = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(0, null, null, null, null, Effect);
            spriteBatch.Draw(texture, position, null, AgedColor, (float)rotation, midpoint, scale, SpriteEffects.None, 1.0f);
            spriteBatch.End();
        }
        #endregion
    }
}
