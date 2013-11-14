using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpriteLibrary;
using Microsoft.Xna.Framework;
using ChaosDrive.Extensions;
using ChaosDrive.Game_Objects.Player;

namespace ChaosDrive.Game_Objects.Bullets
{
    public class EnemyBullet : Bullet
    {
        public static Sprite baseSprite;
        Sprite bulletSprite;
        Vector2 velocity;

        public override Sprite ActiveSprite
        {
            get { return bulletSprite; }
        }

        public EnemyBullet(Vector2 pos, Vector2 playerPos, Rectangle bounds)
            : base(pos, 20.0f, false, bounds)
        {
            bulletSprite = baseSprite.Copy();
            velocity = (pos - playerPos);
            velocity.Normalize();
            velocity = velocity.Multiply(200);
        }

        public override bool Collide(ICollidable other)
        {
            if (other is Player.PlayerObject)
            {
                if (ActiveSprite.Collide(other.ActiveSprite))
                {
                    shouldRemove = true;
                    return true;
                }
            }

            return false;
        }

        public override void Update(float elapsedTime)
        {
            if (!shouldRemove)
            {
                position += velocity.Multiply(elapsedTime / 1000f);
                ActiveSprite.Position = position;
                shouldRemove = !ActiveSprite.Bounds.Intersects(bounds);
            }
        }
    }
}
