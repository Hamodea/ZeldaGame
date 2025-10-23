using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Zelda_game;

namespace Zeldagame
{
    public class GameState
    {
        public enum State { Menu, Playing, Options, GameOver, Win }

        public State CurrentState { get; private set; } = State.Menu;

        // signal till Game1 att avsluta spelet
        public bool RequestExit { get; private set; } = false;

        // expose simple HUD data that Game1 updates
        public int PlayerLives { get; set; } = 0;
        public int Score { get; set; } = 0;

        // Bakgrunder + musik hämtas från TextureManager
        private Texture2D background;
        private Song music;

        // Meny
        private readonly string[] menuItems = { "New Game", "Options", "Exit" };
        private int selectedIndex = 0;
        private SpriteFont uiFont;

        public Game1 game;
        // Input-edge detect
        private KeyboardState prevKb;

        public void LoadContent(ContentManager content)
        {
            uiFont = content.Load<SpriteFont>("UiFont");

            LoadStateAssets(); // bakgrund + musik första gången
        }

        public void ChangeState(State newState)
        {
            if (MediaPlayer.State == MediaState.Playing) MediaPlayer.Stop();
            CurrentState = newState;
            LoadStateAssets();
        }

        private void LoadStateAssets()
        {
            // reset to avoid re-playing previous state's song
            background = null;
            music = null;

            // bakgrund + musik per state
            switch (CurrentState)
            {
                case State.Menu:
                    background = TextureManager.zledaBackground;
                    music = TextureManager.menuMusic;
                    break;
                case State.Playing:
                    //background = TextureManager.playingBackground;
                    //music = TextureManager.playingMusic;
                    break;
                case State.Options:
                    //background = TextureManager.optionsBackground;
                    //music = TextureManager.optionsMusic;
                    break;
                case State.GameOver:
                    //background = TextureManager.gameOverBackground;
                    //music = TextureManager.gameOverMusic;
                    break;
                case State.Win:
                    //background = TextureManager.winBackground;
                    //music = TextureManager.winMusic;
                    break;
            }

            if (music != null)
            {
                if (MediaPlayer.State == MediaState.Playing) MediaPlayer.Stop();
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(music);
            }
        }

        public void Update(GameTime gameTime)
        {
            var kb = Keyboard.GetState();

            switch (CurrentState)
            {
                case State.Menu:
                    if (JustPressed(kb, Keys.Up))
                        selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;

                    if (JustPressed(kb, Keys.Down))
                        selectedIndex = (selectedIndex + 1) % menuItems.Length;

                    if (JustPressed(kb, Keys.Enter))
                    {
                        string pick = menuItems[selectedIndex];
                        if (pick == "New Game")
                            ChangeState(State.Playing);
                        else if (pick == "Options")
                            ChangeState(State.Options);
                        else if (pick == "Exit")
                            RequestExit = true; // Game1 kallar Exit()
                    }
                    break;

                case State.Playing:
                    // (valfritt) paus/meny
                    if (JustPressed(kb, Keys.Escape))
                        ChangeState(State.Menu);
                    break;

                case State.Options:
                    if (JustPressed(kb, Keys.Back))
                        ChangeState(State.Menu);
                    break;
            }

            prevKb = kb;
        }
        public void Draw(SpriteBatch sb)
        {
            // 1) Bakgrund
            var screenRect = new Rectangle(0, 0, 800, 600);
            if (background != null)
                sb.Draw(background, screenRect, Color.White);

            // 2) Överlägg per state
            if (CurrentState == State.Menu)
            {
                var start = new Vector2(300, 420);
                for (int i = 0; i < menuItems.Length; i++)
                {
                    var color = (i == selectedIndex) ? Color.Yellow : Color.White;
                    sb.DrawString(uiFont, menuItems[i], start + new Vector2(0, i * 40), color);
                }
                // liten hint
                sb.DrawString(uiFont, "Use Up/Down, Enter", start + new Vector2(0, menuItems.Length * 40 + 20), Color.LightGray);
            }
            else if (CurrentState == State.Options)
            {
                sb.DrawString(uiFont, "OPTIONS (Back to return)", new Vector2(280, 220), Color.White);

            }
            else if (CurrentState == State.GameOver)
            {
                sb.DrawString(uiFont, "GAME OVER", new Vector2(320, 220), Color.Red);
            }
            else if (CurrentState == State.Win)
            {
                sb.DrawString(uiFont, "YOU WIN!", new Vector2(330, 220), Color.Lime);
            }
        }

        // Draw HUD 
        public void DrawHUD(SpriteBatch sb)
        {
            if (CurrentState != State.Playing) return;
            if (uiFont == null) return;

            // Simple HUD: lives + score
            sb.DrawString(uiFont, $"Lives: {PlayerLives}", new Vector2(10, 10), Color.Red);
            sb.DrawString(uiFont, $"Score: {Score}", new Vector2(10, 36), Color.White);

            // small hint in top-right
            var hint = "Esc: Menu";
            var pos = new Vector2(800 - uiFont.MeasureString(hint).X - 10, 10);
            sb.DrawString(uiFont, hint, pos, Color.LightGray);
        }

        private bool JustPressed(KeyboardState kb, Keys key)
            => kb.IsKeyDown(key) && !prevKb.IsKeyDown(key);
    }
}
