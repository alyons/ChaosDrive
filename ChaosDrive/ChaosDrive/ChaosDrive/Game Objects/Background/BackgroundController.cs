using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ChaosDrive.Game_Objects.Background
{
    public abstract class BackgroundController
    {
        public abstract void Update(float elapsedTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
