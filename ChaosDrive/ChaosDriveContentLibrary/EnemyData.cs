using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ChaosDriveContentLibrary
{
    [Serializable]
    public class EnemyData
    {
        public string enemyType;
        public int appearanceTime;
        public string data;
    }

    public class EnemyDataContentReader : ContentTypeReader<EnemyData>
    {
        protected override EnemyData Read(ContentReader input, EnemyData existingInstance)
        {
            EnemyData enemy = new EnemyData();

            enemy.enemyType = input.ReadString();
            enemy.appearanceTime = input.ReadInt32();
            enemy.data = input.ReadString();

            return enemy;
        }
    }
}
