using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteLibrary;

namespace ChaosDrive.Game_Objects.Bullets
{
    public abstract class Bullet : ICollidable
    {
        protected Vector2 position;
        protected bool isPlayerBullet;
        protected float damage;
        protected bool shouldRemove;
        protected Rectangle bounds;

        public virtual float Damage
        {
            get { return damage; }
        }
        public virtual bool ShouldRemove
        {
            get { return shouldRemove; }
            set { shouldRemove = value; }
        }
        public virtual bool IsPlayerBullet
        {
            get { return isPlayerBullet; }
        }
        public abstract Sprite ActiveSprite
        {
            get;
        }

        public Bullet(Vector2 pos, float dmg, bool ipb, Rectangle bounds)
        {
            position = pos;
            damage = dmg;
            isPlayerBullet = ipb;
            this.bounds = bounds;
        }

        public abstract void Update(float elapsedTime);
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            ActiveSprite.Draw(spriteBatch);
        }
        public abstract bool Collide(ICollidable other);
    }
}
