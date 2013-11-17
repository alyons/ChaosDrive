using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using ChaosDrive.Extensions;
using ChaosDrive.Game_Objects.Effects;
using SpriteLibrary;
using ChaosDrive.Game_Objects.Bullets;
using ChaosDrive.Game_Objects.Enemies;

namespace ChaosDrive.Game_Objects.Player
{
    public class PlayerController
    {
        #region Variables
        int lives;
        PlayerObject player;
        Vector2 lastPlayerPosition;
        SpriteFont font;
        Texture2D hudTexture;
        Texture2D barTexture;
        Random random;
        #endregion

        #region Properties
        public Vector2 PlayerPosition
        {
            get { return lastPlayerPosition; }
        }
        public float PlayerTimeAdjustment
        {
            get
            {
                if (player == null)
                    return 1.0f;
                else
                    return player.TimeAdjustment;
            }
        }
        public PlayerObject Player
        {
            get { return player; }
        }
        public bool GameOver
        {
            get;
            set;
        }
        ParticleController ParticleController
        {
            get;
            set;
        }
        BulletController BulletController
        {
            get;
            set;
        }
        EnemyController EnemyController
        {
            get;
            set;
        }
        GameplayScreen GameplayScreen
        {
            get;
            set;
        }
        List<Sprite> PlayerSprites
        {
            get;
            set;
        }
        Rectangle Bounds
        {
            get;
            set;
        }
        #endregion

        #region Events
        #endregion

        #region Constructors
        private PlayerController(int lives, IEnumerable<Sprite> playerSprites, Rectangle bounds, Texture2D hudTexture, Texture2D barTexture, SpriteFont font, EnemyController ec, BulletController bc, ParticleController pc, GameplayScreen gameScreen)
        {
            this.lives = lives;
            PlayerSprites = new List<Sprite>();
            PlayerSprites.AddRange(playerSprites);
            Bounds = bounds;
            this.hudTexture = hudTexture;
            this.barTexture = barTexture;
            this.font = font;
            EnemyController = ec;
            BulletController = bc;
            ParticleController = pc;
            GameplayScreen = gameScreen;
            random = new Random();
        }
        public PlayerController(IEnumerable<Sprite> playerSprites, Rectangle bounds, Texture2D hudTexture, Texture2D barTexture, SpriteFont font, EnemyController ec, BulletController bc, ParticleController pc, GameplayScreen gameScreen)
            : this(1, playerSprites, bounds, hudTexture, barTexture, font, ec, bc, pc, gameScreen)
        {
            GeneratePlayer();
        }
        #endregion

        #region Methods
        protected void GeneratePlayer(float invulnerableTime)
        {
            player = new PlayerObject(Bounds.Center.ToVector2(), Bounds, PlayerSprites, invulnerableTime);
        }
        protected void GeneratePlayer()
        {
            GeneratePlayer(0f);
        }
        public void Update(float elapsedTime)
        {
            if (player != null)
            {
                player.Update(elapsedTime);
                lastPlayerPosition = player.Position;


                EnemyController.PlayerPosition = player.Position;
                if (!GameplayScreen.TimeFactors.ContainsKey(Player.UniqueName))
                    GameplayScreen.TimeFactors.Add(Player.UniqueName, Player.TimeAdjustment);
                else
                    GameplayScreen.TimeFactors[Player.UniqueName] = Player.TimeAdjustment;

                BulletController.AddBullets(Player.BulletsFired);
                Player.BulletsFired.Clear();

                Player.Collide(BulletController.Bullets);
                Player.Collide(EnemyController.Enemies);


                if (player.Health <= 0)
                {
                    if (ParticleController != null)
                    {
                        var center = player.Position;

                        for (int i = 0; i < 30; i++)
                        {
                            double rot = random.NextDouble() * Math.PI * 2.0;
                            var partVel = new Vector2((float)(Math.Cos(rot) * 6.0), (float)(Math.Sin(rot) * 4.0 - 2.0));
                            ParticleController.Particles.Add(new Particle(center, partVel, Color.Red));
                        }
                    }

                    GeneratePlayer(3000f);
                    lives--;
                }
            }

            BulletFactory.PlayerPosition = lastPlayerPosition;

            GameOver = lives <= 0;
        }
        public void HandleInput(InputState input, PlayerIndex controllingPlayer)
        {
            if (player != null)
            {
                player.HandleInput(input, controllingPlayer);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (player != null)
            {
                player.Draw(spriteBatch);
            }

            spriteBatch.Begin();
            spriteBatch.Draw(hudTexture, new Vector2(0, Bounds.Height - hudTexture.Bounds.Height), Color.White);

            #region Draw Health Bar
            var healthRectangle = new Rectangle(8, Bounds.Height - hudTexture.Bounds.Height + 1, (int)((player != null) ? player.Health : 0f), 14);
            var healthColor = Color.Green;
            if (player.Health <= 66.6f) healthColor = Color.Yellow;
            if (player.Health <= 33.3f) healthColor = Color.Red;
            spriteBatch.Draw(barTexture, healthRectangle, healthColor);
            #endregion

            #region Draw Chaos Drive Bar
            var chaosColor = Color.Blue;
            
            if (player.ChaosFuel < 66.0f) chaosColor = Color.Yellow;
            if (player.ChaosFuel < 33.0f) chaosColor = Color.Red;
            if (player.ChaosDriveRecharging) chaosColor = Color.DarkGray;
            
            var chaosRectangle = new Rectangle(8, Bounds.Height - hudTexture.Bounds.Height + 17, (int)((player != null) ? player.ChaosFuel : 0f), 14);

            spriteBatch.Draw(barTexture, chaosRectangle, chaosColor);
            #endregion
            spriteBatch.End();
        }
        public void Collide(IEnumerable<ICollidable> others)
        {
            if (player != null)
            {
                player.Collide(others);
            }
        }
        #endregion
    }
}
