using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Trype
{
  class ParallaxingBackground
  {
    Texture2D _texture;

    // An array of positions of the parallaxing background.
    Vector2[] _positions;

    int _speed;
    int _screenHeight;
    int _screenWidth;

    public void Initialize(ContentManager content, String texturePath, int screenWidth, int screenHeight, int speed)
    {
      _screenHeight = screenHeight;
      _screenWidth = screenWidth;

      _texture = content.Load<Texture2D>(texturePath);

      _speed = speed;

      // If we divide the screen with the texture width then we can determine the number of tiles needed.
      // We add 1 to it so that we won't have a gap in the tiling.
      int numOfTiles = (int)(Math.Ceiling(_screenWidth / (float)_texture.Width) + 1);
      _positions = new Vector2[numOfTiles];

      // Set the initial positions of the parallazing background
      for (int i = 0; i < _positions.Length; i++)
      {
        _positions[i] = new Vector2(i * _texture.Width, 0);
      }
    }

    public void Update(GameTime gameTime)
    {
      // Update the positions of the background
      for (int i = 0; i < _positions.Length; i++)
      {
        _positions[i].X += _speed;

        // If the speed has the background moving to the left.
        //                if (_speed <= 0)
        //                {
        //                    // Check if the texture is out of view and then put that texture at the end of the screen.
        //                    if (_positions[i].X <= -_texture.Width)
        //                    {
        //                        _positions[i].X = _texture.Width * (_positions.Length - 1);
        //                    }
        //                }
        //                else
        //                {
        //                    // Check if the texture is out of view then position it to the start of the screen
        //                    if (_positions[i].X >= _texture.Width * (_positions.Length - 1))
        //                    {
        //                        _positions[i].X = -_texture.Width;
        //                    }
        //                }
      }

      for (int i = 0; i < _positions.Length; i++)
      {
        if (_speed <= 0)
        {
          // Check if the texture is out of view and then put that texture at the end of the screen.
          if (_positions[i].X <= -_texture.Width)
          {
            WrapTextureToLeft(i);
          }
        }
        else
        {
          if (_positions[i].X >= _texture.Width * (_positions.Length - 1))
          {
            WrapTextureToRight(i);
          }
        }
      }
    }

    private void WrapTextureToLeft(int index)
    {
      // If the textures are scrolling to the left, when the tile wraps, it should be put at the
      // one pixel to the right of the tile before it.
      int prevTexture = index - 1;
      if (prevTexture < 0)
        prevTexture = _positions.Length - 1;

      _positions[index].X = _positions[prevTexture].X + _texture.Width;
    }

    private void WrapTextureToRight(int index)
    {
      // If the textures are scrolling to the right, when the tile wraps, it should be placed to the left
      // of the tile that comes after it.
      int nextTexture = index + 1;
      if (nextTexture == _positions.Length)
        nextTexture = 0;

      _positions[index].X = _positions[nextTexture].X - _texture.Width;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      for (int i = 0; i < _positions.Length; i++)
      {
        var rectBg = new Rectangle((int)_positions[i].X, (int)_positions[i].Y,
                                   _texture.Width,
                                   _screenHeight);
        spriteBatch.Draw(_texture, rectBg, Color.White);
      }
    }
  }
}
