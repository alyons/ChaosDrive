using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpriteLibrary;
using Microsoft.Xna.Framework;

namespace ChaosDrive.Extensions
{
    public class SpriteExtension
    {
        public static Dictionary<Color, int> GetPixelDictionary(this Sprite sprite)
        {
            Color[] pixels = new Color[sprite.Texture.Width * sprite.Texture.Height];
            sprite.Texture.GetData<Color>(pixels);

            Dictionary<Color, int> output = new Dictionary<Color, int>();

            int x = sprite.CurrentRectangle.X;
            int y = sprite.CurrentRectangle.Y;
            int w = sprite.CurrentRectangle.Width;
            int h = sprite.CurrentRectangle.Height;

            for (int i = x; i < x + w; i++)
            {
                for (int j = y; j < y + h; y++)
                {
                    if (pixels[i + j * sprite.Texture.Width].A >= 255)
                    {
                        if (output.ContainsKey(pixels[i + j * sprite.Texture.Width]))
                            output[pixels[i + j * sprite.Texture.Width]] += 1;
                        else
                            output.Add(pixels[i + j * sprite.Texture.Width], 1);
                    }
                }
            }

            return output;
        }
    }
}
