using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpriteLibrary;
using Microsoft.Xna.Framework.Graphics;
using ChaosDrive.Game_Objects.Bullets;

namespace ChaosDrive.Game_Objects.Enemies
{
    public abstract class Enemy : ICollidable
    {
        protected float health;
        protected Vector2 position;
        protected bool shouldRemove;
        protected Rectangle bounds;

        public abstract Sprite ActiveSprite
        {
            get;
        }
        public virtual bool ShouldRemove
        {
            get { return shouldRemove; }
            set { shouldRemove = value; }
        }
        public virtual float Health
        {
            get { return health; }
        }

        public Enemy(Rectangle bounds, Vector2 pos, float health)
        {
            this.bounds = bounds;
            position = pos;
            this.health = health;
        }

        public virtual bool Collide(ICollidable other)
        {
            if (ActiveSprite.Collide(other.ActiveSprite))
            {
                if (other is Player.Player)
                {
                    return true;
                }

                if (other is Bullet)
                {
                    if ((other as Bullet).IsPlayerBullet)
                    {
                        health -= (other as Bullet).Damage;
                        shouldRemove = health <= 0;
                        (other as Bullet).ShouldRemove = true;
                        return true;
                    }
                }
            }

            return false;
        }
        public virtual void TakeDamage(float damage)
        {
            health -= damage;
        }
        public virtual void Update(float elapsedTime)
        {
            ActiveSprite.Update(elapsedTime);
            ActiveSprite.Position = position;

            if (health < 0 || !ActiveSprite.Bounds.Intersects(bounds)) shouldRemove = true;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            ActiveSprite.Draw(spriteBatch);
        }
    }
}
