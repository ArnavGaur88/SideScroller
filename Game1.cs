using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace SideScroller
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Which game state the game is in
        public static GameStates gameState;

        //Background class
        ScrollingBackground scroll;
        SpriteFont backFont;

        //Random numbers for enemy generation
        int enemyCreateRandom;
        Random randomness;
        int checkRand = 0;

        //Player class
        Vector2 PlayerPosition;
        Player player;

        //Screen Dimensions open to all
        public static int screenWidth, screenHeight;

        //Enemy textures...
        Texture2D helicopterTexture;
        Texture2D jetTexture;
        Texture2D explosionTexture;
        Texture2D SkullTexture;

        //GameStats Textures
        Texture2D healthBar;

        //Timing controls
        double time = 0;
         
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Screen Size
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ToggleFullScreen();
            graphics.ApplyChanges();

            //Game State Controls
            gameState = GameStates.SIDESCROLLER;
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

            //Screen Dimensions
            screenWidth = GraphicsDevice.Viewport.Bounds.Right;
            screenHeight = GraphicsDevice.Viewport.Bounds.Bottom;

            //Side-Scroller game state
            if (gameState == GameStates.SIDESCROLLER)
            {
                scroll = new ScrollingBackground();
                scroll.Initialize(Content.Load<Texture2D>("CityScape1"), Content.Load<Texture2D>("NightSky1"));
                PlayerPosition = new Vector2(0, 0);
                player = new Player();
                //player.Initialize(Content.Load<Texture2D>("Flight"), Content.Load<Texture2D>("Vision"),
                //  PlayerPosition);

                player.Initialize(Content.Load<Texture2D>("Flight"), Content.Load<Texture2D>("Vision"),
                    PlayerPosition, Content.Load<SpriteFont>("Font2"));

                GameStats.Initialize(Content.Load<Texture2D>("LoseBackgrounds/Retry"), Content.Load<Texture2D>("LoseBackgrounds/LoseSymbol"),
                    Content.Load<Texture2D>("LoseBackgrounds/RetryExit"), 
                    Content.Load<Texture2D>("GameStatsFolder/health"),
                    Content.Load<Texture2D>("GameStatsFolder/health"));
            }
            //Initializing randomness
            randomness = new Random();

            base.Initialize();
        }

        //This method is called whenever the game changes states
        /*public static void InitializeNewState(GameStates prevState)
        {
            if(gameState == GameStates.RETRY_OR_EXIT)
            {
                GameStats.Update();
            }
        }*/

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //If game state is sidescroller
            if (gameState == GameStates.SIDESCROLLER)
            {
                // Create a new SpriteBatch, which can be used to draw textures.
                spriteBatch = new SpriteBatch(GraphicsDevice);
                backFont = Content.Load<SpriteFont>("Font2");

                //Loading enemy textures
                helicopterTexture = Content.Load<Texture2D>("fightHelicopter");
                jetTexture = Content.Load<Texture2D>("fightJet");
                explosionTexture = Content.Load<Texture2D>("ExplosionSprite");
                SkullTexture = Content.Load<Texture2D>("SkullShip");

                // TODO: use this.Content to load your game content here
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //--------------------------------------------SIDESCROLLER-STATE-----------------------
            if (gameState == GameStates.SIDESCROLLER)
            {
                time += gameTime.ElapsedGameTime.Milliseconds;

                //Inventories Clearing...
                for (int i = 0; i < Player.heat.Count; i++)
                {
                    if (Player.heat[i].timeForDeletion == true)
                    {
                        Player.heat[i] = null;
                        Player.heat.RemoveAt(i);
                    }
                }

                //Enemies Clearing...
                for (int i = 0; i < Player.enemies.Count; i++)
                {
                    if (Player.enemies[i].timeForDeletion == true)
                    {
                        Player.enemies[i] = null;
                        Player.enemies.RemoveAt(i);
                    }
                }


                //Checking if enemy needs to be created...
                enemyCreateRandom = randomness.Next(733);

                //Time in pulses for enemy to be created...
                if (time > 999)
                {

                    if (Player.enemies.Count > checkRand)
                        checkRand = Player.enemies.Count;

                    //enemyCreateRandom = randomness.Next(733);
                    if (enemyCreateRandom % 2 == 0)
                        Player.enemies.Add(new RocketEnemy(helicopterTexture,
                            new Vector2(1281, (int)randomness.Next(733)), explosionTexture));
                    else
                        Player.enemies.Add(new RocketEnemy(jetTexture,
                            new Vector2(1281, (int)randomness.Next(733)), explosionTexture));
                            
                    time = 0;
            }

                //Add enemy at random
                if (enemyCreateRandom % 65 == 0)
                {
                    if (enemyCreateRandom % 20 == 0)
                        Player.enemies.Add(new RocketEnemy(helicopterTexture,
                            new Vector2(1281, (int)enemyCreateRandom), explosionTexture));
                    else
                        Player.enemies.Add(new RocketEnemy(jetTexture,
                            new Vector2(1281, (int)enemyCreateRandom), explosionTexture));
                }

                /*if(Player.enemies.Count == 0)
                {
                    Player.enemies.Add(new SkullEnemy(SkullTexture, new Vector2(1281, 400), explosionTexture));
                }*/


                scroll.update();
                player.Update(gameTime);
                GameStats.Update();

                //Updating heat vision and other projectiles and enemies
                foreach (Projectiles heatProj in Player.heat)
                {
                    heatProj.Update();
                }

                //Updating enemies
                foreach (SidescrollerEnemy baddie in Player.enemies)
                {
                    if(baddie.GetType() == typeof(SkullEnemy))
                        baddie.Update(screenWidth, screenHeight);
                    else
                        baddie.Update();
                }
            }
            //---------------------------------------------SIDESCROLLER-STATE----------------------

            //---------------------------------------------RETRY-OR-EXIT-STATE---------------------
            //Check if game should end...
            switch (GameStats.loseCursorChoice)
            {
                case GameStats.Selection.RETRY:
                    GameStats.score = 0;
                    GameStats.Health = 100;
                    Game1.gameState = GameStates.SIDESCROLLER;
                    GameStats.loseCursorChoice = GameStats.Selection.NEUTRAL;
                    break;

                case GameStats.Selection.EXIT:
                    Exit();
                    break;
            }
            GameStats.Update();
            //---------------------------------------------RETRY-OR-EXIT-STATE---------------------

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, null);

            //------------------------------------------------SIDESCROLLER-STATE-------------------------
            if (gameState == GameStates.SIDESCROLLER)
            {
                scroll.Draw(spriteBatch);

                foreach (SidescrollerEnemy baddies in Player.enemies)
                    baddies.Draw(spriteBatch);


                player.Draw(spriteBatch);
                GameStats.Draw(spriteBatch, backFont);
            }
            //------------------------------------------------SIDESCROLLER-STATE--------------------------

            //------------------------------------------------RETRY-OR-EXIT-STATE-------------------------
            else if (gameState == GameStates.RETRY_OR_EXIT)
            {
                GameStats.DrawRetry(spriteBatch, backFont);
            }
            //------------------------------------------------RETRY-OR-EXIT-STATE-------------------------
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    class ScrollingBackground
    {
        Texture2D sprite;
        Texture2D Sky;
        Vector2 Position;
        Vector2 skyPos;
        int width, height;

        Rectangle destRect;
        Rectangle skyRect;

        public void Initialize(Texture2D spr, Texture2D skySpr)
        {
            Position = new Vector2(0, 0);
            skyPos = new Vector2(0, 0);
            sprite = spr;
            Sky = skySpr;
            width = Game1.screenWidth;
            height = Game1.screenHeight;
            destRect = new Rectangle((int)Position.X, (int)Position.Y, width, height);
            skyRect = new Rectangle((int)skyPos.X, (int)skyPos.Y, width, height);

        }

        public void update()
        {

            //------------------------------------------SIDESCROLLER-STATE------------------------
            if (Game1.gameState == GameStates.SIDESCROLLER)
            {
                if (Position.X <= -(2 * width))
                    Position.X = 0;
                if (skyPos.X <= -(2 * width))
                    skyPos.X = 0;

                Position.X -= 9;
                destRect.X = (int)Position.X;

                skyPos.X -= 6;
                skyRect.X = (int)skyPos.X;
            }
            //-------------------------------------------SIDESCROLLER-STATE----------------------
        }

        public void Draw(SpriteBatch batch)
        {

            //Night sky
            batch.Draw(Sky, skyRect, Color.White);
            batch.Draw(Sky, new Rectangle(skyRect.Right, skyRect.Y, width, height), Color.White);
            batch.Draw(Sky, new Rectangle(skyRect.Right + width, skyRect.Y, width, height), Color.White);

            //Skyscrapers
            batch.Draw(sprite, destRect, Color.White);
            batch.Draw(sprite, new Rectangle(destRect.Right, destRect.Y, width, height), Color.White);
            batch.Draw(sprite, new Rectangle(destRect.Right + width, destRect.Y, width, height), Color.White);
        }
    }

    class Player
    {
        //This class is slightly more complex, what with collision detection, health management, etc.

        //Player
        Texture2D playerSprite;                 //Texture for player

        SpriteFont sprFont;

        //Projectiles and Miscellaneous
        Texture2D heatTexture;                  //Texture for Heat Vision



        //Collision detection
        public Rectangle collisionRectangle;
        public Rectangle destinationRectangle;         //The position of the player never changes, it is for initialization
                                                       //It is the destinationRectangle that changes position.
        public Vector2 Position;                       //Current position of the player
        

        int SidescrollerWidth, SidescrollerHeight;        //Height and Width values of sidescroller    



        //Animation Controls
        int losingCurrentFrame = 0;
        int totalFrames = 0;


        //For maintaining the game time
        int gameTiming = 0;

        //Inventory of Heat Vision
        public static List<Projectiles> heat = new List<Projectiles>();

        //Inventory of enemies
        public static List<SidescrollerEnemy> enemies = new List<SidescrollerEnemy>();

        public void Initialize(Texture2D sprite, Texture2D hT, Vector2 Position, SpriteFont font)
        {
            playerSprite = sprite;                  //Called in every state. Superman will always need new sprite

            sprFont = font;

            //Position of the player
            this.Position = Position;               //He will always need a new vector when initialize is called

            //Width and Height of Textures
            SidescrollerWidth = 96 + 64;
            SidescrollerHeight = 64 + 16;

            //Also always called
            destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, SidescrollerWidth,
                SidescrollerHeight);
            collisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, SidescrollerWidth, 
                SidescrollerHeight);
            heatTexture = hT;
        }

        public void Draw(SpriteBatch batch)
        {
            //------------------------------------------------------SIDESCROLLER-STATE-----------------------------
            if (Game1.gameState == GameStates.SIDESCROLLER)
            {
                batch.Draw(playerSprite, destinationRectangle, Color.White);

                foreach (Projectiles heatProj in Player.heat)
                    heatProj.Draw(batch);
            }
            //------------------------------------------------------SIDESCROLLER-STATE-----------------------------
        }

        public void Update(GameTime timing)
        {
            int tempWidth = 0, tempHeight = 0;          //Cause players width and height keep changing

            //-----------------------------------------SIDESCROLLER-STATE---------------------------
            if (Game1.gameState == GameStates.SIDESCROLLER)
            {
                tempWidth = SidescrollerWidth;
                tempHeight = SidescrollerHeight;

                destinationRectangle.Width = tempWidth;
                destinationRectangle.Height = tempHeight;

                collisionRectangle.Width = tempWidth;
                collisionRectangle.Height = tempHeight;
            }
            //---------------------------------------------------SIDESCROLLER-STATE---------------------------

            //---------------------------------------------------RETRY-OR-EXIT-STATE--------------------------

            else if (Game1.gameState == GameStates.RETRY_OR_EXIT)
            {
                totalFrames = 8;

                //Slowly move to the center of the screen
                if (destinationRectangle.X > (Game1.screenWidth / 2))
                    destinationRectangle.X -= 2;
                else if (destinationRectangle.X < (Game1.screenWidth / 2))
                    destinationRectangle.X += 2;

                if (destinationRectangle.Y > (Game1.screenHeight / 2))
                    destinationRectangle.Y -= 2;
                else if (destinationRectangle.Y < (Game1.screenHeight / 2))
                    destinationRectangle.Y += 2;

                //Play animation
                if (losingCurrentFrame < totalFrames)
                    losingCurrentFrame++;

                if (losingCurrentFrame >= totalFrames)
                    return;               //Ensures that this method is no longer called

            }

            //---------------------------------------------------RETRY-OR-EXIT-STATE---------------------------

            else
            {
                tempWidth = SidescrollerWidth;
                tempHeight = SidescrollerHeight;

                destinationRectangle.Width = tempWidth;
                destinationRectangle.Height = tempHeight;

                collisionRectangle.Width = tempWidth;
                collisionRectangle.Height = tempHeight;
            }

            //------------------------------------------Update always-----------------------------

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (destinationRectangle.X + 2 < (Game1.screenWidth - tempWidth))
                    destinationRectangle.X += 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (destinationRectangle.X - 2 > 0)
                    destinationRectangle.X -= 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (destinationRectangle.Y - 2 > 0)
                    destinationRectangle.Y -= 10;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (destinationRectangle.Y + 2 < (Game1.screenHeight - tempHeight))
                    destinationRectangle.Y += 10;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (gameTiming > 33)
                {
                    heat.Add(new Projectiles(heatTexture, new Vector2(destinationRectangle.Right - 32, destinationRectangle.Y + 20)));
                    gameTiming = 0;
                    
                }

                gameTiming += timing.ElapsedGameTime.Milliseconds;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                gameTiming = 99;
            }

            //Make sure that the collision rectangle also moves
            collisionRectangle.X = destinationRectangle.X;
            collisionRectangle.Y = destinationRectangle.Y;

            //Check whether any collision occured and change health accordingly
            foreach (SidescrollerEnemy baddies in Player.enemies)
            {
                if (collisionRectangle.Intersects(baddies.collisionRect))
                {
                    if (baddies.timeForTermination == false)
                    {
                        baddies.timeForTermination = true;
                        GameStats.Health -= 25;
                    }
                }
            }

            //--------------------------------------------------Update always----------------------
        }
        
    }

    class Projectiles
    {
        protected Texture2D Sprite;
        protected Vector2 Position;

        //Height and Width details
        protected int height, width;

        //If time for deletion is true, remove from insert!
        public bool timeForDeletion = false;

        //Collision
        public Rectangle collisionRectangle;

        public Projectiles(Texture2D tex, Vector2 pos)
        {
            Sprite = tex;                                               //Initializing Texture
            Position = pos;                                             //Initializing Position
            width = Sprite.Width;                                       //Width of object
            height = Sprite.Height;                                     //Height of object

            //Rectangle for collision detection.
            collisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }

        public void Update()
        {
            if (Position.X < (Game1.screenWidth - width))
            {
                Position.X += 8;
                collisionRectangle.X += 8;
            }
            else timeForDeletion = true;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Sprite, Position, Color.White);
        }
    }

    class SidescrollerEnemy
    {
        protected Texture2D Sprite;
        protected Texture2D explosionSprite;
        public Vector2 Position;
        public Rectangle collisionRect;
        public Rectangle destinationRect;

        protected int width, height;

        //Explosion details
        protected const int explosionWidth = 64;
        protected const int explosionHeight = 64;
        protected const int totalFrames = 16;
        protected int currentFrame;

        //Health...

        //Treated like projectiles
        public bool timeForDeletion = false;

        //Killed
        public bool timeForTermination = false;

        public SidescrollerEnemy(Texture2D tex, Vector2 pos, Texture2D explosionTexture)
        {
            Sprite = tex;
            width = 96 + 32;
            height = 64 + 16;

            //Explosion initialization
            explosionSprite = explosionTexture;
            currentFrame = 0;

            collisionRect = new Rectangle((int)Position.X, (int)Position.Y, width, height);
            destinationRect = new Rectangle((int)Position.X, (int)Position.Y, width, height);

            if (Player.enemies.Count > 4)
                timeForDeletion = true;
        }

        public void Draw(SpriteBatch batch)
        {
            if(timeForTermination == false)
            batch.Draw(Sprite, destinationRect, Color.White);

            else
                batch.Draw(explosionSprite, new Vector2(destinationRect.X, destinationRect.Y), 
                    new Rectangle((int)(currentFrame % 4) * explosionWidth, (int)(currentFrame / 4) * explosionHeight,
                    explosionWidth, explosionHeight), Color.White);
        }


        public void Update(int screenWidth, int screenHeight)
        {
            if (destinationRect.X > (screenWidth / 4) * 3)
            {
                Position.X -= 2;
                destinationRect.X = (int)Position.X;
                collisionRect.X = destinationRect.X;
            }
        }

        public void Update()
        {
            if (destinationRect.Left < 0)
                timeForDeletion = true;
            else
            {
                //Delete if touching other enemies
                foreach (SidescrollerEnemy baddies in Player.enemies)
                {
                    if (collisionRect.Intersects(baddies.collisionRect))
                    {
                        if(!Object.ReferenceEquals(this, baddies))
                        timeForDeletion = true;
                    }
                }

                destinationRect.X -= 10;
                collisionRect.X -= 10;
            }

            if (destinationRect.Bottom > 733)
            {
                destinationRect.Y = 733 - destinationRect.Height;
                collisionRect.Y = 733 - destinationRect.Height;
            }

            //If collision has been done!
            if(timeForTermination == true)
            {
                if (currentFrame < totalFrames)
                    currentFrame++;
                else if (currentFrame >= totalFrames)
                    timeForDeletion = true;
            }
        }

    }

    class RocketEnemy : SidescrollerEnemy  //All ships
    {
        public RocketEnemy(Texture2D texture, Vector2 Position, Texture2D explode) : base(texture, Position, explode)
        {
            Sprite = texture;
            this.Position = Position;

            collisionRect = new Rectangle((int)Position.X, (int)Position.Y, width, height);
            destinationRect = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }
    }
    
    class SkullEnemy : SidescrollerEnemy
    {
        public SkullEnemy(Texture2D texture, Vector2 pos, Texture2D explode) : base(texture, pos, explode)
        {
            Sprite = texture;
            Position = pos;

            collisionRect = new Rectangle((int)Position.X, (int)Position.Y, width, height);
            destinationRect = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }

    }

    static class GameStats
    {
        public static int Health;     //Player health at a given time
        public static int score;      //Score that player has procured.

        //Lose screens
        static Texture2D GeneralLoss, loseToShow, loseCursor, Buttons;

        //Lose cursor information
        static int currentFrame, totalFrames;
        static int frameWidth, frameHeight;
        static int buttonWidth, buttonHeight;
        static Vector2 loseCursorPosition;

        //Lose Cursor Choice
        public enum Selection
        {
            RETRY,
            EXIT, 
            NEUTRAL
        };
        public static Selection loseCursorChoice;

        static Dictionary<int, Vector2> loseCursorFrames;

        //HUD Display
        static Texture2D HealthBar, VisionBar;

        static GameStates state; //Previous state of the game

        public static void Initialize(Texture2D lose, Texture2D loseCursorTex, Texture2D buttons,
            Texture2D healthTexture, Texture2D visionTexture)
        {
            Health = 100;
            score = 0;
            state = GameStates.SIDESCROLLER;

            //Make sure lose cursor has not chosen a choice yet...
            loseCursorChoice = Selection.NEUTRAL;

            //Animation of loseCursor
            loseCursor = loseCursorTex;
            loseCursorPosition = new Vector2(Game1.screenWidth / 2 - 112, 300);
            Buttons = buttons;
            loseCursorFrames = new Dictionary<int, Vector2>();
            int frameIndex = 0;
            frameWidth = loseCursor.Width / 4;              //Width of a single frame in lose cursor
            frameHeight = loseCursor.Height / 4;            //Height of a single frame in lose cursor
            int xFrame = 0;                                 
            int yFrame = 0;
            while(frameIndex < 15)
            {
                if(xFrame == 4)
                {
                    xFrame = 0;
                    yFrame += 1;
                }

                loseCursorFrames.Add(frameIndex, new Vector2(xFrame * frameWidth, yFrame * frameHeight));
                xFrame += 1;
                frameIndex++;
            }

            buttonWidth = Buttons.Width / 2;
            buttonHeight = Buttons.Height;

            currentFrame = 0;
            totalFrames = 15;

            //Loss screens
            GeneralLoss = lose;

            //HUD Display Items
            HealthBar = healthTexture;
            VisionBar = visionTexture;
        }

        private static Vector2 returnFrame(int key)
        {
            if (loseCursorFrames.ContainsKey(key))
                return loseCursorFrames[key];
            else
                return loseCursorFrames[0];
        }

        public static void Update()
        {

            //-----------------------------------------SIDESCROLLER-----------------------------
            if (Game1.gameState == GameStates.SIDESCROLLER)
            {
                foreach (Projectiles playerProjectile in Player.heat)
                {
                    foreach(SidescrollerEnemy baddies in Player.enemies)
                    {
                        if(playerProjectile.collisionRectangle.Intersects(baddies.collisionRect))
                        {
                            if (baddies.timeForTermination == false)
                            {
                                baddies.timeForTermination = true;
                                playerProjectile.timeForDeletion = true;
                                score += 250;
                            }
                        }
                    }
                }


                if (Health < 0)
                {
                    RetryOrExit(Game1.gameState);
                }
            }
            //----------------------------------------------SIDESCROLLER---------------------

            state = Game1.gameState;

            if(state != Game1.gameState)
            {
                score = 0;
                Health = 100;
            }

            if(Game1.gameState == GameStates.RETRY_OR_EXIT)
            {
                //Drawing cursor
                currentFrame += 1;
                if (currentFrame >= totalFrames)
                    currentFrame = 0;

                //Controlling Cursor position...
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    loseCursorPosition.Y = 300 + 128;


                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    loseCursorPosition.Y = 300;

                if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (loseCursorPosition.Y == 300 + 128)
                        loseCursorChoice = Selection.EXIT;
                    else
                        loseCursorChoice = Selection.RETRY;
                }
            }
        }

        static void RetryOrExit(GameStates prevState)
        {
            switch (prevState)
            {
                case GameStates.SIDESCROLLER:
                    loseToShow = GeneralLoss;
                    break;
            }

            Game1.gameState = GameStates.RETRY_OR_EXIT;   
        }

        public static void Draw(SpriteBatch batch, SpriteFont fontness)
        {
            //Health Bar
            batch.Draw(HealthBar, new Vector2(0, 0), Color.DarkGoldenrod);
            batch.Draw(HealthBar, new Vector2(20, 26), new Rectangle(20, 26, (int)(3.56 * Health), 42), Color.Yellow);

            batch.DrawString(fontness, "Health: " + Health, new Vector2(0, 50), Color.LightCyan);

            //Vision Bar

            batch.DrawString(fontness, "Score: " + score, new Vector2(0, 100), Color.Beige);
            //Draw Heat-Vision Bar
            //Draw Score
        }

        public static void DrawRetry(SpriteBatch batch, SpriteFont fontness)
        {
            batch.Draw(loseToShow, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.White);

            //Drawing cursor
            batch.Draw(loseCursor, new Rectangle((int)loseCursorPosition.X, (int)loseCursorPosition.Y, 64, 64), new Rectangle((int)returnFrame(currentFrame).X,
                (int)returnFrame(currentFrame).Y, frameWidth, frameHeight), Color.White);

            //Drawing retry and exit buttons
            batch.DrawString(fontness, "Score: " + score, new Vector2(Game1.screenWidth / 2 - 48, 50), Color.Yellow);
            batch.Draw(Buttons, new Rectangle(Game1.screenWidth / 2 - 48, 300, 96, 64), new Rectangle(0, 0, buttonWidth, buttonHeight),
                Color.White);
            batch.Draw(Buttons, new Rectangle(Game1.screenWidth / 2 - 48, 300 + 128, 96, 64), new Rectangle(buttonWidth, 0, buttonWidth, buttonHeight),
                Color.White);
        }
    }
}
