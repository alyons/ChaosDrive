using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ChaosDrive.Game_Objects.Effects
{
    public class ParticleController
    {
        #region Variables
        List<Particle> particles;
        #endregion

        #region Properties
        public List<Particle> Particles
        {
            get { return particles; }
        }
        #endregion

        #region Constructors
        public ParticleController()
        {
            particles = new List<Particle>();
        }
        #endregion

        #region Methods
        public void Update(float elapsedTime)
        {
            particles.RemoveAll(p => p.ShouldRemove);

            foreach (Particle particle in particles)
                particle.Update(elapsedTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in particles)
                particle.Draw(spriteBatch);
        }
        #endregion
    }
}
