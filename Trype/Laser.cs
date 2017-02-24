using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Trype
{
  class Laser
  {
    public Animation LaserAnimation;
    float laserMoveSpeed = 30f;
    public Vector2 Position;
    int Damage = 10;
    public bool Active;
    int Range;
    public int Width
    {
      get { return LaserAnimation.FrameWidth; }
    }
    public int Height
    {
      get { return LaserAnimation.FrameHeight; }
    }

    public void Initialize(Animation animation, Vector2 position)
    {
      LaserAnimation = animation;
      Position = position;
      Active = true;
    }

    public void Update(GameTime gameTime)
    {
      Position.X += laserMoveSpeed;
      LaserAnimation.Position = Position;
      LaserAnimation.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      LaserAnimation.Draw(spriteBatch);
    }

  }
}
