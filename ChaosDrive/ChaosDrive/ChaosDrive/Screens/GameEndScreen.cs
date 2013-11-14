using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ChaosDrive.Screens
{
    class GameEndScreen : MenuScreen
    {
        #region Variables
        SpriteFont entryFont;
        ContentManager content;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public GameEndScreen(String endGameMessage)
            :base(endGameMessage)
        {

        }
        #endregion

        #region Methods
        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                entryFont = content.Load<SpriteFont>("TitleFont");

                // Create our menu entries.
                MenuEntry playGameMenuEntry = new MenuEntry("Play Again", entryFont);
                MenuEntry optionsMenuEntry = new MenuEntry("Main Menu", entryFont);
                MenuEntry exitMenuEntry = new MenuEntry("Exit", entryFont);

                // Hook up menu event handlers.
                playGameMenuEntry.Selected += PlayAgainEntrySelected;
                optionsMenuEntry.Selected += MainMenuEntrySelected;
                exitMenuEntry.Selected += ExitGameEntrySelected;

                // Add entries to the menu.
                MenuEntries.Add(playGameMenuEntry);
                MenuEntries.Add(optionsMenuEntry);
                MenuEntries.Add(exitMenuEntry);
            }
        }
        #region Event Handlers
        void PlayAgainEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }
        void MainMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }
        void ExitGameEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, e.PlayerIndex);
        }
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
        #endregion
        #endregion
    }
}
