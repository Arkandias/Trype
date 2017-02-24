using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Trype
{
  class Animation
  {
    Texture2D spriteStrip;
    float scale;
    int elapsedTime;
    int frameTime;
    int frameCount;
    int currentFrame;
    Color color;
    Rectangle sourceRect = new Rectangle();
    Rectangle destinationRect = new Rectangle();
    
    public int FrameWidth;
    public int FrameHeight;
    public bool Active;
    public bool Looping;
    public Vector2 Position;
    
    public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount, int frameTime, Color color, float scale, bool looping) {
      this.color = color;
      this.FrameHeight = frameHeight;
      this.FrameWidth = frameWidth;
      this.frameCount = frameCount;
      this.frameTime = frameTime;
      this.scale = scale;

      Looping = looping;
      Position = position;
      spriteStrip = texture;

      // Set the time to zero
      elapsedTime = 0;
      currentFrame = 0;

      // Set the Animation to active by default
      Active = true;
    }

    public void Update(GameTime gameTime) {
      // Do not update the game if we are not active
      if (Active == false) {
        return;
      }

      // Update elapsedTime
      elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

      // If the elapsed time is larger than the frame time, we need to switch frames
      if (elapsedTime > frameTime) {
        // Move to the next frame
        currentFrame++;

        // If the currentFrame is equal to frameCount reset currentFrame to zero
        if (currentFrame == frameCount) {
          currentFrame = 0;

          // If we are not looping deactivate the animation
          if (Looping == false) {
            Active = false;
          }
        }
        // Reset the elapsed time to zero
        elapsedTime = 0;
      }

      // Grab the correct frame in the image strip by multiplying the currentFrame index by the Frame width
      sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
      destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
                                      (int)Position.Y - (int)(FrameHeight * scale) / 2,
                                      (int)(FrameWidth * scale),
                                      (int)(FrameHeight * scale));
    }

    public void Draw(SpriteBatch spriteBatch) {
      // Only draw the animation when it is active
      if (Active) {
        spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
      }
    }
  }
}
