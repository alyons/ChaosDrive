using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteLibrary;
using ChaosDrive.Game_Objects.Bullets;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using ChaosDrive.Game_Objects.Enemies;

namespace ChaosDrive.Game_Objects.Player
{
    public class Player : ITimeAdjuster, INotifyPropertyChanged, ICollidable
    {
        #region Enumertaions
        #region State Numbers
        const int UPLEFT = 0;
        const int UP = 1;
        const int UPRIGHT = 2;
        const int LEFT = 3;
        const int IDLE = 4;
        const int RIGHT = 5;
        const int DOWNLEFT = 6;
        const int DOWN = 7;
        const int DOWNRIGHT = 8;
        #endregion
        #endregion

        #region Variables
        float timeAdjustment;
        List<Sprite> sprites;
        List<Bullet> bulletsFired;
        Vector2 position;
        Vector2 velocity;
        Rectangle bounds;
        float gunReloadTime;
        float driveReloadTime;
        float chaosDriveFuel;
        bool invulnerable;
        int activeSprite;
        int state = 0;
        float health;
        float incomingDamage;
        float incomingHealing;
        const float HEALTH_CHANGE = 25;
        const float VERTICAL_SPEED = 375.0f;
        const float HORIZONTAL_SPEED = 500.0f;

        InputAction upActions;
        InputAction downActions;
        InputAction leftActions;
        InputAction rightActions;
        InputAction fireActions;
        InputAction slowTimeActions;
        InputAction accelTimeActions;
        #endregion

        #region Properties
        public float TimeAdjustment
        {
            get { return timeAdjustment; }
            private set
            {
                if (value != timeAdjustment)
                {
                    timeAdjustment = value;
                    OnPropertyChanged("TimeAdjustment");
                }
            }
        }
        public string UniqueName
        {
            get { return "Player"; }
        }
        public bool Invulnerable
        {
            get { return invulnerable; }
        }
        public Sprite ActiveSprite
        {
            get { return sprites[activeSprite]; }
        }
        public List<Bullet> BulletsFired
        {
            get { return bulletsFired; }
        }
        public float Health
        {
            get { return health; }
        }
        public float ChaosFuel
        {
            get { return chaosDriveFuel; }
        }
        public bool ChaosDriveRecharging
        {
            get { return driveReloadTime > 0; }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public Player(Vector2 pos, Rectangle bounds, IEnumerable<Sprite> sprites)
        {
            position = pos;
            activeSprite = 0;
            this.bounds = bounds;
            this.sprites = new List<Sprite>();
            this.sprites.AddRange(sprites);
            this.bulletsFired = new List<Bullet>();
            timeAdjustment = 1.0f;
            health = 100;
            chaosDriveFuel = 100;

            upActions = new InputAction(new Buttons[] { Buttons.LeftThumbstickUp, Buttons.DPadUp }, new Keys[] { Keys.W, Keys.Up }, false);
            downActions = new InputAction(new Buttons[] { Buttons.LeftThumbstickDown, Buttons.DPadDown }, new Keys[] { Keys.S, Keys.Down }, false);
            leftActions = new InputAction(new Buttons[] { Buttons.LeftThumbstickLeft, Buttons.DPadLeft }, new Keys[] { Keys.A, Keys.Left }, false);
            rightActions = new InputAction(new Buttons[] { Buttons.LeftThumbstickRight, Buttons.DPadRight }, new Keys[] { Keys.D, Keys.Right }, false);
            fireActions = new InputAction(new Buttons[] { Buttons.A }, new Keys[] { Keys.Space }, false);
            slowTimeActions = new InputAction(new Buttons[] { Buttons.LeftTrigger, Buttons.LeftShoulder }, new Keys[] { Keys.J, Keys.Z }, false);
            accelTimeActions = new InputAction(new Buttons[] { Buttons.RightTrigger, Buttons.RightShoulder }, new Keys[] { Keys.K, Keys.X }, false);
        }
        #endregion

        #region Methods
        public void Update(float elapsedTime)
        {
            ActiveSprite.Update(elapsedTime);

            #region Move the player according to the state
            Move(elapsedTime);
            #endregion

            if (incomingDamage > 0)
            {
                var toRemove = 25.0f / (elapsedTime * 1000.0f);
                toRemove = Math.Min(incomingDamage, toRemove);

                health -= toRemove;
                incomingDamage -= toRemove;
            }

            if (gunReloadTime > 0) gunReloadTime -= elapsedTime;

            if (driveReloadTime > 0) driveReloadTime -= elapsedTime;

            if (chaosDriveFuel < 100 && timeAdjustment == 1.0f)
                chaosDriveFuel = Math.Min((chaosDriveFuel + (10.0f * elapsedTime / 1000.0f)), 100);

            if (timeAdjustment != 1.0f)
            {
                chaosDriveFuel = Math.Max(chaosDriveFuel - (10.0f * elapsedTime / 1000.0f / timeAdjustment), 0.0f);

                if (chaosDriveFuel == 0.0f)
                    driveReloadTime = 4000.0f;
            }
        }
        public void HandleInput(InputState input, PlayerIndex controllingPlayer)
        {
            #region Set up variables to use
            int playerIndex = (int)controllingPlayer;
            PlayerIndex player;

            int nextState = -1;
            #endregion

            #region Change Player State
            if (upActions.Evaluate(input, controllingPlayer, out player))
            {
                if (leftActions.Evaluate(input, controllingPlayer, out player))
                    nextState = UPLEFT;
                else if (rightActions.Evaluate(input, controllingPlayer, out player))
                    nextState = UPRIGHT;
                else
                    nextState = UP;
            }
            else if (downActions.Evaluate(input, controllingPlayer, out player))
            {
                if (leftActions.Evaluate(input, controllingPlayer, out player))
                    nextState = DOWNLEFT;
                else if (rightActions.Evaluate(input, controllingPlayer, out player))
                    nextState = DOWNRIGHT;
                else
                    nextState = DOWN;
            }
            else if (leftActions.Evaluate(input, controllingPlayer, out player))
            {
                nextState = LEFT;
            }
            else if (rightActions.Evaluate(input, controllingPlayer, out player))
            {
                nextState = RIGHT;
            }
            else
            {
                nextState = IDLE;
            }

            if (nextState != state)
            {
                state = nextState;
                switch (state)
                {
                    default:
                        activeSprite = 0;
                        break;
                }
                ResetInactiveSprites();
            }
            #endregion

            #region Try to fire bullets
            if (gunReloadTime <= 0 && fireActions.Evaluate(input, controllingPlayer, out player))
            {
                bulletsFired.Add(new PlayerBullet(position, bounds));
                gunReloadTime += 1000.0f;
            }
            #endregion

            #region Set player Velocity
            velocity = new Vector2();
            switch (state)
            {
                case UPLEFT:
                    velocity = new Vector2(-HORIZONTAL_SPEED / 2.0f, -VERTICAL_SPEED / 2.0f);
                    break;
                case UP:
                    velocity = new Vector2(0, -VERTICAL_SPEED);
                    break;
                case UPRIGHT:
                    velocity = new Vector2(HORIZONTAL_SPEED / 2.0f, -VERTICAL_SPEED / 2.0f);
                    break;
                case LEFT:
                    velocity = new Vector2(-HORIZONTAL_SPEED, 0);
                    break;
                case RIGHT:
                    velocity = new Vector2(HORIZONTAL_SPEED, 0);
                    break;
                case DOWNRIGHT:
                    velocity = new Vector2(HORIZONTAL_SPEED / 2.0f, VERTICAL_SPEED / 2.0f);
                    break;
                case DOWN:
                    velocity = new Vector2(0, VERTICAL_SPEED);
                    break;
                case DOWNLEFT:
                    velocity = new Vector2(-HORIZONTAL_SPEED / 2.0f, VERTICAL_SPEED / 2.0f);
                    break;
                default:
                    velocity = new Vector2(0, 0);
                    break;
            }

            
            #endregion

            #region Adjust Time
            if (slowTimeActions.Evaluate(input, controllingPlayer, out player) && chaosDriveFuel > 0 && driveReloadTime <= 0)
            {
                timeAdjustment = 0.5f;
            }
            else if (accelTimeActions.Evaluate(input, controllingPlayer, out player) && chaosDriveFuel > 0 && driveReloadTime <= 0)
            {
                timeAdjustment = 2.0f;
            }
            else
            {
                timeAdjustment = 1.0f;
            }
            #endregion
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            ActiveSprite.Draw(spriteBatch);   
        }
        public void RestoreChaosFuel(float fuel, float timeWarpFactor)
        {
            chaosDriveFuel += (fuel * timeWarpFactor);
        }
        public bool Collide(ICollidable other)
        {
            if (!Invulnerable)
            {
                if (other is Bullet)
                {
                    if (!(other as Bullet).IsPlayerBullet)
                    {
                        incomingDamage += (other as Bullet).Damage;
                        (other as Bullet).ShouldRemove = true;
                        invulnerable = true;
                        return true;
                    }
                }
                if (other is Enemy)
                {
                    incomingDamage += (other as Enemy).Health / 10.0f;
                    (other as Enemy).ShouldRemove = true;
                    invulnerable = true;
                    return true;
                }
            }

            return false;
        }
        void Move(float elapsedTime)
        {
            var adjustedVelocity = velocity * elapsedTime / 1000.0f;

            if ((ActiveSprite.Bounds.Top + adjustedVelocity.Y) < bounds.Top)
            {
                adjustedVelocity.Y = bounds.Top - ActiveSprite.Bounds.Top;
            }

            if ((ActiveSprite.Bounds.Left + adjustedVelocity.X) < bounds.Left)
            {
                adjustedVelocity.X = bounds.Left - ActiveSprite.Bounds.Left;
            }

            if ((ActiveSprite.Bounds.Bottom + adjustedVelocity.Y) > bounds.Bottom)
            {
                adjustedVelocity.Y = bounds.Bottom - ActiveSprite.Bounds.Bottom;
            }

            if ((ActiveSprite.Bounds.Right + adjustedVelocity.X) > bounds.Right)
            {
                adjustedVelocity.X = bounds.Right - ActiveSprite.Bounds.Right;
            }

            position += adjustedVelocity;
            ActiveSprite.Position = position;
        }
        void ResetInactiveSprites()
        {
            for (int i = 0; i < sprites.Count; i++)
                if (i != activeSprite)
                    sprites[i].Frame = 0;
        }
        #endregion

        #region Event Creators
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
