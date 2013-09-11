using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpriteLibrary;

namespace ChaosDrive.Game_Objects
{
    public interface ICollidable
    {
        Sprite ActiveSprite
        {
            get;
        }

        bool Collide(ICollidable other);
    }
}
