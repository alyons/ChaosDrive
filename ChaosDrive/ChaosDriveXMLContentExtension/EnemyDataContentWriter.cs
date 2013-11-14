using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using ChaosDriveContentLibrary;

namespace ChaosDriveXMLContentExtension
{
    [ContentTypeWriter]
    public class EnemyDataContentWriter : ContentTypeWriter<EnemyData>
    {
        protected override void Write(ContentWriter output, EnemyData value)
        {
            output.Write(value.enemyType);
            output.Write(value.appearanceTime);
            output.Write(value.data);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(EnemyDataContentReader).AssemblyQualifiedName;
        }
    }
}