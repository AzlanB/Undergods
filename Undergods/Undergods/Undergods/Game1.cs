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

namespace Undergods
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int screenWidth = 1280, screenHeight = 940;

        Level arena;
        Player player;
        Aranea aranea;

        GameState state;
        KeyboardState oldKb;
        MouseState oldMouse;
        int selectedButton;

        Texture2D pauseMask, logo, controls, victory, defeat;
        SpriteFont bigFont, smallFont;
        SoundEffect screech;

        Texture2D playerRun, playerAttack, playerDash, shadow;
        Rectangle[] attackingRects = new Rectangle[4];
        Vector2[] attackingOrigins = new Vector2[4];
        int controlsTimer = 0, controlsJump = 0, jumpSpeed = 0;

        Texture2D[] introPanel = new Texture2D[6];
        int currentPanel, endTimer = 0;

        Song PlaySong, MenuSong, DeathSong, WinSong;
        Random rng;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            state = GameState.Start;
            oldKb = Keyboard.GetState();
            oldMouse = Mouse.GetState();
            selectedButton = 0;
            currentPanel = 0;
            rng = new Random();

            attackingRects[0] = new Rectangle(76, 50, 177-76, 149-50);
            attackingRects[1] = new Rectangle(316, 50, 405-316, 168-50);
            attackingRects[2] = new Rectangle(553, 50, 615-553, 178-50);
            attackingRects[3] = new Rectangle(40, 34, 109-40, 133-34);
            attackingOrigins[0] = new Vector2((69-10)/2, 99/2);
            attackingOrigins[1] = new Vector2((69-10)/2, 99/2);
            attackingOrigins[2] = new Vector2((69-10)/2 + 3, 99/2);
            attackingOrigins[3] = new Vector2(69/2, 99/2);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            List<Texture2D> t = new List<Texture2D>();
            for (int i = 1; i <= 4; i++)
                t.Add(Content.Load<Texture2D>("tile"+i));
            arena = new Level(10, 10, 3, t);

            screech = Content.Load<SoundEffect>("Screech");
            victory = Content.Load<Texture2D>("victory");
            defeat = Content.Load<Texture2D>("defeat");

            playerRun = Content.Load<Texture2D>("player");
            playerAttack = Content.Load<Texture2D>("player attack");
            playerDash = Content.Load<Texture2D>("player dash");
            shadow = Content.Load<Texture2D>("shadow");
            List<SoundEffect> grunt = new List<SoundEffect>();
            grunt.Add(Content.Load<SoundEffect>("Grunt"));
            grunt.Add(Content.Load<SoundEffect>("Grunt 2"));
            grunt.Add(Content.Load<SoundEffect>("Grunt 3"));
            player = new Player(Content.Load<SoundEffect>("Sword"), grunt, playerRun, playerAttack, Content.Load<Texture2D>("player up attack"), playerDash, shadow, Content.Load<Texture2D>("Themis Health"), Content.Load<Texture2D>("Health Color"), new Vector3(7, 7, 0));
            
            aranea = new Aranea(Services, player, arena);

            pauseMask = new Texture2D(GraphicsDevice, 1, 1);
            pauseMask.SetData(new Color[] { new Color(0, 0, 0, 200) });
            logo = Content.Load<Texture2D>("logo");
            controls = Content.Load<Texture2D>("controls");
            bigFont = Content.Load<SpriteFont>("big font");
            smallFont = Content.Load<SpriteFont>("small font");

            for (int i = 0; i < introPanel.Length; i++)
                introPanel[i] = Content.Load<Texture2D>("Panel "+(i+1));

            PlaySong = Content.Load<Song>("face_off");
            MenuSong = Content.Load<Song>("renegade_assumptions");
            DeathSong = Content.Load<Song>("death");
            WinSong = Content.Load<Song>("win");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(MenuSong); 

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            KeyboardState kb = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (state == GameState.Start)
            {
                if (IsActive)
                {
                    if (kb.IsKeyDown(Keys.Escape) && oldKb.IsKeyUp(Keys.Escape))
                        this.Exit();
                    if (kb.IsKeyDown(Keys.W) && oldKb.IsKeyUp(Keys.W) || kb.IsKeyDown(Keys.S) && oldKb.IsKeyUp(Keys.S) || kb.IsKeyDown(Keys.A) && oldKb.IsKeyUp(Keys.A) || kb.IsKeyDown(Keys.D) && oldKb.IsKeyUp(Keys.D))
                        selectedButton = (selectedButton + 1) % 2;
                    if (selectedButton == 0 && (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released || kb.IsKeyDown(Keys.Space) && oldKb.IsKeyUp(Keys.Space)))
                        state = GameState.Intro;
                    if (selectedButton == 1 && (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released || kb.IsKeyDown(Keys.Space) && oldKb.IsKeyUp(Keys.Space)))
                        state = GameState.Controls;
                }
            }
            else if (state == GameState.Controls)
            {
                if (IsActive)
                {
                    if (kb.IsKeyDown(Keys.W) && oldKb.IsKeyUp(Keys.W) || kb.IsKeyDown(Keys.A) && oldKb.IsKeyUp(Keys.A) || kb.IsKeyDown(Keys.S) && oldKb.IsKeyUp(Keys.S) || kb.IsKeyDown(Keys.D) && oldKb.IsKeyUp(Keys.D) || kb.IsKeyDown(Keys.Escape) && oldKb.IsKeyUp(Keys.Escape) || kb.IsKeyDown(Keys.Space) && oldKb.IsKeyUp(Keys.Space) || mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        state = GameState.Start;
                }
                if (controlsTimer >= 60)
                    controlsTimer = 0;
                else
                {
                    controlsTimer++;
                    jumpSpeed -= 5;
                }
                if (controlsTimer%30 == 0)
                {
                    jumpSpeed = 30;
                    controlsJump = 0;
                }
                controlsJump += jumpSpeed;
                if (controlsJump < 0)
                    controlsJump = 0;
            }
            else if (state == GameState.Intro)
            {
                if (IsActive)
                {
                    if (kb.IsKeyDown(Keys.W) && oldKb.IsKeyUp(Keys.W) || kb.IsKeyDown(Keys.S) && oldKb.IsKeyUp(Keys.S) || kb.IsKeyDown(Keys.A) && oldKb.IsKeyUp(Keys.A) || kb.IsKeyDown(Keys.D) && oldKb.IsKeyUp(Keys.D) || kb.IsKeyDown(Keys.Escape) && oldKb.IsKeyUp(Keys.Escape) || kb.IsKeyDown(Keys.Space) && oldKb.IsKeyUp(Keys.Space) || kb.IsKeyDown(Keys.LeftShift) && oldKb.IsKeyUp(Keys.LeftShift) || mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        currentPanel++;
                }
                if (currentPanel >= introPanel.Length)
                {
                    state = GameState.Playing;
                    Player.oldKb = kb;
                    Player.oldMouse = mouse;
                    MediaPlayer.Stop();
                    MediaPlayer.Play(PlaySong);
                }
            }
            else if (state == GameState.Playing)
            {
                player.Update(arena);
                switch (rng.Next(3))
                {
                    case 0: aranea.pattern1(); break;
                    case 1:
                    case 2: aranea.pattern2(); break;
                }
                aranea.Update();
                if (!IsActive || kb.IsKeyDown(Keys.Escape) && oldKb.IsKeyUp(Keys.Escape))
                {
                    state = GameState.Paused;
                    MediaPlayer.Pause();
                }
                if (player.hp <= 0)
                {
                    endTimer = 60;
                    state = GameState.Lost;
                    MediaPlayer.Stop();
                    MediaPlayer.Play(DeathSong);
                }
                if (aranea.health <= 0)
                {
                    endTimer = 60;
                    state = GameState.Won;
                    MediaPlayer.Stop();
                    screech.Play();
                    MediaPlayer.Play(WinSong);
                }
            }
            else if (state == GameState.Paused)
            {
                if (IsActive)
                {
                    if (kb.IsKeyDown(Keys.W) && oldKb.IsKeyUp(Keys.W) || kb.IsKeyDown(Keys.S) && oldKb.IsKeyUp(Keys.S) || kb.IsKeyDown(Keys.A) && oldKb.IsKeyUp(Keys.A) || kb.IsKeyDown(Keys.D) && oldKb.IsKeyUp(Keys.D))
                        selectedButton = (selectedButton + 1) % 2;
                    if (selectedButton == 0 && (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released || kb.IsKeyDown(Keys.Space) && oldKb.IsKeyUp(Keys.Space)) || kb.IsKeyDown(Keys.Escape) && oldKb.IsKeyUp(Keys.Escape))
                    {
                        state = GameState.Playing;
                        Player.oldKb = kb;
                        Player.oldMouse = mouse;
                        MediaPlayer.Resume();
                    }
                    if (selectedButton == 1 && (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released || kb.IsKeyDown(Keys.Space) && oldKb.IsKeyUp(Keys.Space)))
                        Initialize();
                }
            }
            else if (state == GameState.Lost || state == GameState.Won)
            {
                if (IsActive)
                {
                    if (endTimer > 0)
                        endTimer--;
                    else if (kb.IsKeyDown(Keys.W) && oldKb.IsKeyUp(Keys.W) || kb.IsKeyDown(Keys.S) && oldKb.IsKeyUp(Keys.S) || kb.IsKeyDown(Keys.A) && oldKb.IsKeyUp(Keys.A) || kb.IsKeyDown(Keys.D) && oldKb.IsKeyUp(Keys.D) || kb.IsKeyDown(Keys.Escape) && oldKb.IsKeyUp(Keys.Escape) || kb.IsKeyDown(Keys.Space) && oldKb.IsKeyUp(Keys.Space) || kb.IsKeyDown(Keys.LeftShift) && oldKb.IsKeyUp(Keys.LeftShift) || mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        Initialize();
                }
            }

            oldKb = kb;
            oldMouse = mouse;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            if (state == GameState.Start)
            {
                spriteBatch.Draw(logo, new Vector2(screenWidth/2, screenHeight/3), null, Color.White, 0, new Vector2(logo.Width, logo.Height)/2, 1.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(smallFont, "Start", new Vector2(screenWidth/2-smallFont.MeasureString("Start").X/2, screenHeight*2/3-smallFont.MeasureString("Start").Y-10), selectedButton == 0 ? Color.White : Color.Gray);
                spriteBatch.DrawString(smallFont, "Controls", new Vector2(screenWidth/2-smallFont.MeasureString("Controls").X/2, screenHeight*2/3+10), selectedButton == 1 ? Color.White : Color.Gray);
            }
            else if (state == GameState.Controls)
            {
                spriteBatch.Draw(controls, new Rectangle(0, (screenHeight-controls.Height*screenWidth/controls.Width)/2, screenWidth, controls.Height*screenWidth/controls.Width), Color.White);
                spriteBatch.Draw(playerRun, new Vector2(screenWidth*5/16, screenHeight/3), (controlsTimer/10)%2 == 0 ? new Rectangle(40, 34, 109-40, 133-34) : new Rectangle(280, 34, 349-280, 133-34), Color.White, 0, new Vector2(69/2, 99/2), 2f, SpriteEffects.None, 0);
                spriteBatch.Draw((controlsTimer/5)%4 == 3 ? playerRun : playerAttack, new Vector2(screenWidth*5/16, screenHeight*2/3), attackingRects[(controlsTimer/5)%4], Color.White, 0, attackingOrigins[(controlsTimer/5)%4], 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(controlsTimer <= 15 ? playerDash : playerRun, new Vector2(screenWidth*27/32, screenHeight/3), controlsTimer <= 15 ? new Rectangle(47, 21, 137-47, 154-21) : new Rectangle(280, 34, 349-280, 133-34), Color.White, 0, controlsTimer <= 15 ? new Vector2(88/2, 99/2 + 34) : new Vector2(69/2, 99/2), 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(playerRun, new Vector2(screenWidth*27/32, screenHeight*2/3 - controlsJump), new Rectangle(280, 34, 349-280, 133-34), Color.White, 0, new Vector2(69/2, 99/2), 2f, SpriteEffects.None, 0);
            }
            else if (state == GameState.Intro)
                spriteBatch.Draw(introPanel[currentPanel], new Vector2(screenWidth, screenHeight)/2, null, Color.White, 0, new Vector2(introPanel[currentPanel].Width, introPanel[currentPanel].Height)/2, screenWidth/introPanel[currentPanel].Width, SpriteEffects.None, 0);
            else if (state == GameState.Playing || state == GameState.Paused)
            {
                arena.Draw(spriteBatch, screenWidth, screenHeight);

                List<Object3D> items = new List<Object3D>();
                items.Add(player);
                items.Add(aranea);
                items.AddRange(aranea.getProjects());
                items.Sort();
                foreach (Object3D o in items)
                {
                    if (o.type.Equals("Player"))
                        ((Player)o).Draw(spriteBatch, arena, screenWidth, screenHeight);
                    else if (o.type.Equals("Aranea"))
                        ((Aranea)o).Draw(spriteBatch);
                    else if (o.type.Equals("blueproj") || o.type.Equals("yellowproj"))
                        ((Projectile)o).Draw(spriteBatch, arena);
                }

                if (state == GameState.Paused)
                {
                    spriteBatch.Draw(pauseMask, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    spriteBatch.DrawString(bigFont, "PAUSED", new Vector2(screenWidth/2, screenHeight/3)-bigFont.MeasureString("PAUSED")/2, Color.White);
                    spriteBatch.DrawString(smallFont, "Resume", new Vector2(screenWidth/2-smallFont.MeasureString("Resume").X/2, screenHeight*2/3-smallFont.MeasureString("Resume").Y-10), selectedButton == 0 ? Color.White : Color.Gray);
                    spriteBatch.DrawString(smallFont, "Main Menu", new Vector2(screenWidth/2-smallFont.MeasureString("Main Menu").X/2, screenHeight*2/3+10), selectedButton == 1 ? Color.White : Color.Gray);
                }
            }
            else if (state == GameState.Lost)
                spriteBatch.Draw(defeat, new Vector2(screenWidth, screenHeight)/2, null, Color.White, 0, new Vector2(defeat.Width, defeat.Height)/2, screenWidth/defeat.Width, SpriteEffects.None, 0);
            else if (state == GameState.Won)
                spriteBatch.Draw(victory, new Vector2(screenWidth, screenHeight)/2, null, Color.White, 0, new Vector2(victory.Width, victory.Height)/2, screenWidth/victory.Width, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
