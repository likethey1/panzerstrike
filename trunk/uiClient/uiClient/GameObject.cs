﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace uiClient
{
    class GameObject
    {
        public Rectangle bbox;
        public Texture2D sprite;
        public Texture2D turret;
        public Vector2 position;
        public int rotation;
        public Vector2 center;
        public Vector2 velocity;
        public bool alive;

        public GameObject(Texture2D textura)
        {
            rotation = 0;
            position = Vector2.Zero;
            sprite = textura;
            center = new Vector2(sprite.Width / 2, sprite.Height / 2);
            bbox = new Rectangle(0, 0, sprite.Width, sprite.Height);
            velocity = Vector2.Zero;
            alive = false;
        }

        public void MoveTo(Vector2 newpos)
        {
            
            position = newpos;
            bbox.X = (int)position.X;
            bbox.Y = (int)position.Y;
        }
        public void MoveBy(float dx, float dy)
        {
            
            position.X += dx;
            position.Y += dy;
            bbox.X = (int)position.X;
            bbox.Y = (int)position.Y;
        }

        public void MoveRotatBy(float dx, float dy, int ang) {
            MoveBy(dx,dy);
            rotation=ang;
            
            if (ang==90.0f || ang==270.0f) {
                Point p = bbox.Center;
                Rectangle r2 = new Rectangle((int)p.X - (p.Y - bbox.Top), (int)p.Y - (p.X - bbox.Left), bbox.Height, bbox.Width);
                bbox = r2;
            }
        }
    }
}
