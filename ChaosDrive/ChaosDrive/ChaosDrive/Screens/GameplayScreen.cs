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
#endregion

namespace ChaosDrive
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
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
        Player player;
        BulletController bulletController;
        EnemyController enemyController;
        ParticleController particleController;
        BackgroundController backgroundController;

        float pauseAlpha;

        InputAction pauseAction;

        Rectangle bounds = new Rectangle(0, 0, 1280, 720);

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
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

                if (particleController == null) particleController = new ParticleController();
                if (bulletController == null) bulletController = new BulletController();
                if (enemyController == null) enemyController = new TestEnemyController(bounds, bulletController, particleController);
                if (backgroundController == null) backgroundController = new TestBackgroundController(bounds);
                
                if (player == null)
                {
                    var playerSprites = content.Load<List<Sprite>>(@"Sprites\Player\PlayerSprites");
                    player = new Player(bounds.Center.ToVector2(), bounds, playerSprites);
                }

                Enemy.hitEffect = content.Load<Effect>(@"Sprite Effects\HitEffect");
                PlayerBullet.baseSprite = content.Load<Sprite>(@"Sprites\Bullets\PlayerBullet");
                BasicEnemy.baseSprite = content.Load<Sprite>(@"Sprites\Enemies\BasicEnemy");
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
                if (player != null) player.Update(elapsedTime);
                if (!timeFactors.ContainsKey(player.UniqueName))
                    timeFactors.Add(player.UniqueName, player.TimeAdjustment);
                else
                    timeFactors[player.UniqueName] = player.TimeAdjustment;

                bulletController.AddBullets(player.BulletsFired);
                player.BulletsFired.Clear();
                player.Collide(enemyController.Enemies);
                player.Collide(bulletController.Bullets);
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
                if (player != null) player.HandleInput(input, ControllingPlayer.Value);
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
            player.Draw(spriteBatch);
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
            Color healthColor = Color.Green;
            Color chaosColor = Color.Green;

            if (timeFactors.ContainsKey(player.UniqueName))
                if (timeFactors[player.UniqueName] > 1.0f)
                    timeColor = Color.Green;
                else if (timeFactors[player.UniqueName] < 1.0f)
                    timeColor = Color.Red;

            if (player.Health < 33.0f)
                healthColor = Color.Red;
            else if (player.Health < 66.0f)
                healthColor = Color.Yellow;

            if (player.ChaosDriveRecharging)
                chaosColor = Color.DarkGray;
            else if (player.ChaosFuel < 33.0f)
                chaosColor = Color.Red;
            else if (player.ChaosFuel < 66.0f)
                chaosColor = Color.Yellow;

            string timeString = TimeString();
            string healthString = HealthString();
            string chaosString = ChaosDriveString();

            Vector2 timePos = new Vector2(8, 8);
            Vector2 healthPos = timePos;
            healthPos.Y += hudFont.MeasureString(timeString).Y + 4;
            Vector2 chaosPos = healthPos;
            chaosPos.Y += hudFont.MeasureString(healthString).Y + 4;

            spriteBatch.Begin();

            spriteBatch.DrawString(hudFont, timeString, timePos, timeColor);
            spriteBatch.DrawString(hudFont, healthString, healthPos, healthColor);
            spriteBatch.DrawString(hudFont, chaosString, chaosPos, chaosColor);
            spriteBatch.DrawString(hudFont, "Bullets: " + bulletController.Bullets.Count, new Vector2(8, chaosPos.Y + hudFont.MeasureString(chaosString).Y + 4), Color.Yellow);

            spriteBatch.End();
        }

        #endregion

        string HUDString()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(affectedGameTime);
            object[] stringObjects = new object[] { t.Minutes, t.Seconds, t.Milliseconds, player.Health, player.ChaosFuel};
            return String.Format(HUD_STRING, stringObjects);
        }
        string TimeString()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(affectedGameTime);
            object[] stringObjects = new object[] { t.Minutes, t.Seconds, t.Milliseconds };
            return String.Format(TIME_STRING, stringObjects);
        }
        string HealthString()
        {
            object[] stringObjects = new object[] {  player.Health };
            return String.Format(HEALTH_STRING, stringObjects);
        }
        string ChaosDriveString()
        {
            object[] stringObjects = new object[] {  player.ChaosFuel };
            return String.Format(CHAOS_STRING, stringObjects);
        }
    }
}
