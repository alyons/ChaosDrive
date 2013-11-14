using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpriteLibrary;
using Microsoft.Xna.Framework;
using ChaosDrive.Game_Objects.Bullets;
using ChaosDrive.Game_Objects.Player;

namespace ChaosDrive.Game_Objects.Enemies
{
    public class BasicEnemy : Enemy
    {
        public static Sprite baseSprite;
        Sprite sprite;

        public override Sprite ActiveSprite
        {
            get { return sprite; }
        }

        public BasicEnemy(Rectangle bounds, Vector2 pos)
            : base(bounds, pos, 100)
        {
            sprite = baseSprite.Copy();
        }
        public override void Update(float elapsedTime)
        {
            position.Y += 150 * elapsedTime / 1000.0f;

            base.Update(elapsedTime);
        }
        public override void DisposeObjects()
        {
            sprite.Dispose();

            base.DisposeObjects();
        }
    }
}
