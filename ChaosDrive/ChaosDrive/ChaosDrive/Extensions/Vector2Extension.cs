using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ChaosDrive.Extensions
{
    public static class Vector2Extension
    {
        public static Vector2 Multiply(this Vector2 vector, double value)
        {
            return new Vector2((float)(vector.X * value), (float)(vector.Y * value));
        }
    }
}
