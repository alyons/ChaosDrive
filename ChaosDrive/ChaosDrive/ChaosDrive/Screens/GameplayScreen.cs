#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using System.Collections.Generic;
using ChaosDrive.Game_Objects.Player;
using SpriteLibrary;
using ChaosDrive.Game_Objects.Bullets;
using ChaosDrive.Game_Objects.Enemies;
using ChaosDrive.Game_Objects.Effects;
using ChaosDrive.Extensions;
using ChaosDrive.Game_Objects.Background;
using ChaosDriveContentLibrary;
using ChaosDrive.Screens;
#endregion

namespace ChaosDrive
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        const string HUD_STRING = "Time: {0:D2}:{1:D2}:{2:D3}\nHealth: {3}\nChaos Drive: {4}";
        const string TIME_STRING = "Time: {0:D2}:{1:D2}:{2:D3}";
        const string HEALTH_STRING = "Health: {0:F3}";
        const string CHAOS_STRING = "Chaos Drive: {0:F2}";

        Random random = new Random();

        GameTime myGameTime;
        float affectedGameTime;
        Dictionary<string, float> timeFactors;
        SpriteFont hudFont;
        PlayerController playerController;
        BulletController bulletController;
        EnemyController enemyController;
        ParticleController particleController;
        BackgroundController backgroundController;
        string levelAsset;

        float pauseAlpha;

        InputAction pauseAction;

        Rectangle bounds = new Rectangle(0, 0, 1280, 720);

        #endregion

        #region Properties
        public Dictionary<string, float> TimeFactors
        {
            get { return timeFactors; }
        }
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
            : this("XML\\EnemyList\\EnemiesLevel01")
        {
            
        }
        public GameplayScreen(string levelAsset)
        {
            this.levelAsset = levelAsset;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                if (timeFactors == null)
                    timeFactors = new Dictionary<string, float>();

                if (myGameTime == null)
                    myGameTime = new GameTime();

                gameFont = content.Load<SpriteFont>("TitleFont");
                hudFont = content.Load<SpriteFont>(@"Fonts\hudFont");

                EnemyFactory.Bounds = bounds;
                BulletFactory.Bounds = bounds;

                var enemyCues = new List<EnemyData>();
                if (!String.IsNullOrWhiteSpace(levelAsset)) enemyCues.AddRange(content.Load<EnemyData[]>(levelAsset));

                if (particleController == null) particleController = new ParticleController();
                if (bulletController == null) bulletController = new BulletController();
                if (enemyController == null)
                {
                    if (enemyCues.Count > 0)
                        enemyController = new QueuedEnemyController(bounds, bulletController, particleController, enemyCues);
                    else
                        enemyController = new TestEnemyController(bounds, bulletController, particleController);
                }
                if (backgroundController == null) backgroundController = new TestBackgroundController(bounds);
                
                if (playerController == null)
                {
                    var playerSprites = content.Load<List<Sprite>>(@"Sprites\Player\PlayerSprites");
                    var hudTexture = content.Load<Texture2D>(@"Images\Player\HUDBars");
                    var barTexture = content.Load<Texture2D>(@"Images\Player\statusBar");
                    playerController = new PlayerController(playerSprites, bounds, hudTexture, barTexture, hudFont, enemyController, bulletController, particleController, this);
                }

                Enemy.hitEffect = content.Load<Effect>(@"Sprite Effects\HitEffect");
                PlayerBullet.baseSprite = content.Load<Sprite>(@"Sprites\Bullets\PlayerBullet");
                DirectedEnemyBullet.baseSprite = content.Load<Sprite>(@"Sprites\Bullets\EnemyBullet");
                BasicEnemy.baseSprite = content.Load<Sprite>(@"Sprites\Enemies\BasicEnemy");
                StandardEnemy.baseSprite = content.Load<Sprite>(@"Sprites\Enemies\BasicEnemy");
                BezierCurveEnemy.baseSprite = content.Load<Sprite>(@"Sprites\Enemies\BasicEnemy");
                Particle.baseTexture = content.Load<Texture2D>(@"Images\Effects\fire_particle");
                SimpleBoss.baseSprite = content.Load<Sprite>(@"Sprites\Enemies\SimpleBoss");
                Star.texture = content.Load<Texture2D>(@"Images\Background\Star");

                affectedGameTime = 0.0f;
                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(1000);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

#if WINDOWS_PHONE
            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];
            }
#endif
        }


        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }
        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                if (playerController.GameOver)
                {
                    HandleGameOver();
                }

                if (enemyController.LevelFinished)
                {
                    HandleLevelComplete();
                }

                var timeWarpFactor = 1.0f;
                foreach (float value in timeFactors.Values)
                {
                    timeWarpFactor *= value;
                }

                var elapsedTime = gameTime.ElapsedGameTime.Milliseconds * timeWarpFactor;
                var timeSpan = new TimeSpan(0, 0, 0, 0, (int)elapsedTime);
                myGameTime = new GameTime(myGameTime.TotalGameTime, timeSpan);
                affectedGameTime += elapsedTime;

                #region Update Objects
                #region Update Player
                playerController.Update(elapsedTime);
                #endregion

                #region Update Bullets
                bulletController.Update(elapsedTime);
                #endregion

                #region Update Enemies
                enemyController.Update(elapsedTime);
                #endregion

                #region Update Particles
                particleController.Update(elapsedTime);
                #endregion

                #region Update Background
                backgroundController.Update(elapsedTime);
                #endregion
                #endregion
            }
        }

        public virtual void HandleGameOver()
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new GameEndScreen("Game Over"));
        }

        public virtual void HandleLevelComplete()
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new GameEndScreen("Mission Complete!"));
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex refForPlayerIndex;
            if (pauseAction.Evaluate(input, ControllingPlayer, out refForPlayerIndex) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
#endif
            }
            else
            {
                playerController.HandleInput(input, ControllingPlayer.Value);
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            backgroundController.Draw(spriteBatch);
            particleController.Draw(spriteBatch);
            bulletController.Draw(spriteBatch);
            enemyController.Draw(spriteBatch);
            playerController.Draw(spriteBatch);
            DrawHUD(spriteBatch);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        void DrawHUD(SpriteBatch spriteBatch)
        {
            Color timeColor = Color.Yellow;

            if (timeFactors.ContainsKey(playerController.Player.UniqueName))
                if (timeFactors[playerController.Player.UniqueName] > 1.0f)
                    timeColor = Color.Green;
                else if (timeFactors[playerController.Player.UniqueName] < 1.0f)
                    timeColor = Color.Red;

            string timeString = TimeString();

            Vector2 timePos = new Vector2(8, 8);

            spriteBatch.Begin();

            spriteBatch.DrawString(hudFont, timeString, timePos, timeColor);
            spriteBatch.DrawString(hudFont, "Bullets: " + bulletController.Bullets.Count, new Vector2(8, timePos.Y + hudFont.MeasureString(timeString).Y + 4), Color.Yellow);

            spriteBatch.End();
        }
        #endregion

        string HUDString()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(affectedGameTime);
            object[] stringObjects = new object[] { t.Minutes, t.Seconds, t.Milliseconds, playerController.Player.Health, playerController.Player.ChaosFuel };
            return String.Format(HUD_STRING, stringObjects);
        }
        string TimeString()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(affectedGameTime);
            object[] stringObjects = new object[] { t.Minutes, t.Seconds, t.Milliseconds };
            return String.Format(TIME_STRING, stringObjects);
        }
    }
}
