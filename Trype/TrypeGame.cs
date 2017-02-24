using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Trype
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class TrypeGame : Game
  {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    Player player;

    KeyboardState currentKeyboardState;
    KeyboardState previousKeyboardState;

    GamePadState currentGamePadState;
    GamePadState previousGamePadState;

    MouseState currentMouseState;
    MouseState previousMouseState;

    float playerMoveSpeed;

    Texture2D mainBackground;
    Rectangle rectBackground;
    float scale = 1f;
    ParallaxingBackground bgLayer1;
    ParallaxingBackground bgLayer2;

    Texture2D enemyTexture;
    List<Enemy> enemies;
    TimeSpan enemySpawnTime;
    TimeSpan previousSpawnTime;
    Random random;

    Texture2D laserTexture;
    List<Laser> laserBeams;
    TimeSpan laserSpawnTime;
    TimeSpan previousLaserSpawnTime;

    public TrypeGame()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
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
      player = new Player();
      playerMoveSpeed = 8.0f;

      bgLayer1 = new ParallaxingBackground();
      bgLayer2 = new ParallaxingBackground();
      rectBackground = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

      enemies = new List<Enemy>();
      previousSpawnTime = TimeSpan.Zero;
      enemySpawnTime = TimeSpan.FromSeconds(1.0f);
      random = new Random();

      laserBeams = new List<Laser>();
      const float SECONDS_IN_MINUTE = 60f;
      const float RATE_OF_FIRE = 200f;
      laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
      previousLaserSpawnTime = TimeSpan.Zero;

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

      // Load the player ressources
      Animation playerAnimation = new Animation();
      Texture2D playerTexture = Content.Load<Texture2D>("sprites\\player\\player_blue_down");
      playerAnimation.Initialize(playerTexture, Vector2.Zero, 33, 17, 3, 100, Color.White, 1F, true);

      Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
      player.Initialize(playerAnimation, playerPosition);

      bgLayer1.Initialize(Content, "background/bgLayer1", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);
      bgLayer2.Initialize(Content, "background/bgLayer2", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);

      enemyTexture = Content.Load<Texture2D>("sprites\\enemy\\enemy_idle");

      laserTexture = Content.Load<Texture2D>("projectiles\\fire_1");

      mainBackground = Content.Load<Texture2D>("background/mainbackground");
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    protected void FireLaser(GameTime gameTime)
    {
      if (gameTime.TotalGameTime - previousLaserSpawnTime > laserSpawnTime)
      {
        previousLaserSpawnTime = gameTime.TotalGameTime;
        AddLaser();
      }
    }

    protected void AddLaser()
    {
      Animation laserAnimation = new Animation();
      laserAnimation.Initialize(laserTexture,
          player.Position,
          32,
          14,
          1,
          30,
          Color.White,
          1f,
          true);

      Laser laser = new Laser();

      var laserPostion = player.Position;
      laserPostion.Y += 0;
      laserPostion.X += 30;

      laser.Initialize(laserAnimation, laserPostion);
      laserBeams.Add(laser);
      /* todo: add code to create a laser. */
      // laserSoundInstance.Play();
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();
      // Save the previous state of the keyboard and game pad so we can determine single key/button press
      previousGamePadState = currentGamePadState;
      previousKeyboardState = currentKeyboardState;
      previousMouseState = currentMouseState;

      // Read the current state of the keyboard and gamepad and store it
      currentKeyboardState = Keyboard.GetState();
      currentGamePadState = GamePad.GetState(PlayerIndex.One);
      currentMouseState = Mouse.GetState();

      UpdatePlayer(gameTime);
      bgLayer1.Update(gameTime);
      bgLayer2.Update(gameTime);

      for (var i = 0; i < laserBeams.Count; i++)
      {
        laserBeams[i].Update(gameTime);
        if (!laserBeams[i].Active || laserBeams[i].Position.X > GraphicsDevice.Viewport.Width)
        {
          laserBeams.Remove(laserBeams[i]);
        }
      }


      UpdateEnemies(gameTime);
      UpdateCollision();

      base.Update(gameTime);
    }

    private void AddEnemy()
    {
      Animation enemyAnimation = new Animation();
      enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 26, 22, 1, -1, Color.White, 1f, true);
      Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));
      Enemy enemy = new Enemy();
      enemy.Initialize(enemyAnimation, position);
      enemies.Add(enemy);
    }

    private void UpdateCollision()
    {
      Rectangle laserRectangle;
      Rectangle rectangle1;
      Rectangle rectangle2;
      rectangle1 = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
      // detect collisions between the player and all enemies.
      enemies.ForEach(e => {
        //create a retangle for the enemy
        rectangle2 = new Rectangle(
            (int)e.Position.X,
            (int)e.Position.Y,
            e.Width,
            e.Height);

        // now see if this enemy collide with any laser shots
        laserBeams.ForEach(lb => {
          // create a rectangle for this laserbeam
          laserRectangle = new Rectangle(
          (int)lb.Position.X,
          (int)lb.Position.Y,
          lb.Width,
          lb.Height);

          // test the bounds of the laser and enemy
          if (laserRectangle.Intersects(rectangle2))
          {
            // play the sound of explosion.
            //var explosion = explosionSound.CreateInstance();
            //explosion.Play();

            // Show the explosion where the enemy was...
            //AddExplosion(e.Position);

            // kill off the enemy
            e.Health = 0;

            //record the kill
            //myGame.Stage.EnemiesKilled++;

            // kill off the laserbeam
            lb.Active = false;

            // record your score
            //myGame.Score += e.Value;
          }
        });
      });
    }

    private void UpdateEnemies(GameTime gameTime)
    {
      if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
      {
        previousSpawnTime = gameTime.TotalGameTime;
        AddEnemy();
      }
      for (int i = enemies.Count - 1; i >= 0; i--)
      {
        enemies[i].Update(gameTime);
        if (enemies[i].Active == false)
        {
          enemies.RemoveAt(i);
        }
      }
    }

    private void UpdatePlayer(GameTime gameTime)
    {
      player.Update(gameTime);
      // Mouse Handling
      Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
      if (currentMouseState.LeftButton == ButtonState.Pressed)
      {
        Vector2 posDelta = mousePosition - player.Position;
        posDelta.Normalize();
        posDelta = posDelta * playerMoveSpeed;
        player.Position = player.Position + posDelta;
      }

      // Gamepad handling
      player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
      player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

      // Keyboard handling
      if (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed)
      {
        player.Position.X -= playerMoveSpeed;
      }
      if (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed)
      {
        player.Position.X += playerMoveSpeed;
      }
      if (currentKeyboardState.IsKeyDown(Keys.Up) || currentGamePadState.DPad.Up == ButtonState.Pressed)
      {
        player.Position.Y -= playerMoveSpeed;
      }
      if (currentKeyboardState.IsKeyDown(Keys.Down) || currentGamePadState.DPad.Down == ButtonState.Pressed)
      {
        player.Position.Y += playerMoveSpeed;
      }

      // Out of bounds handling
      player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
      player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);

      if (currentKeyboardState.IsKeyDown(Keys.Space) || currentGamePadState.Buttons.X == ButtonState.Pressed)
      {
        FireLaser(gameTime);
      }
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.WhiteSmoke);

      spriteBatch.Begin();
      spriteBatch.Draw(mainBackground, rectBackground, Color.White);
      bgLayer1.Draw(spriteBatch);
      bgLayer2.Draw(spriteBatch);
      for (int i = 0; i < enemies.Count; i++)
      {
        enemies[i].Draw(spriteBatch);
      }
      foreach (var l in laserBeams)
      {
        l.Draw(spriteBatch);
      }
      player.Draw(spriteBatch);
      spriteBatch.End();
      base.Draw(gameTime);
    }
  }
}
