using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpriteLibrary;
using ChaosDrive.Game_Objects.Enemies;
using Microsoft.Xna.Framework.Graphics;

namespace ChaosDrive.Game_Objects.Bullets
{
    public class PlayerBullet : Bullet
    {
        public static Sprite baseSprite;
        Sprite bulletSprite;

        public override Sprite ActiveSprite
        {
            get { return bulletSprite; }
        }

        public PlayerBullet(Vector2 pos, Rectangle bounds)
            : base(pos, 50.0f, true, bounds)
        {
            bulletSprite = baseSprite.Copy();
        }

        public override bool Collide(ICollidable other)
        {
            if (other is Enemy)
            {
                (other as Enemy).TakeDamage(damage);
                shouldRemove = true;
                return true;
            }

            return false;
        }

        public override void Update(float elapsedTime)
        {
            position.Y -= 500.0f * elapsedTime / 1000.0f;
            ActiveSprite.Position = position;
            shouldRemove = !ActiveSprite.Bounds.Intersects(bounds);
        }
    }
}
