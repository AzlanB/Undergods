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
    class Projectile : Object3D
    {
        Texture2D text, shadow;
        Vector3 startpos;
        static Random rand = new Random();
        float direction;
        bool isShot;
        int time = -1, trackTime = 0;

        public Projectile(string t, ContentManager c, Vector3 p) : base(t, p)
        {
            type = t;
            isShot = false;
            startpos = p;
            text = c.Load<Texture2D>(type);
            shadow = c.Load<Texture2D>("shadow");
            direction = 0;
            
        }

        public void shoot(Player player, Level arena)
        {
            //wait for a few secs, maybe 2? to better align w animation
            time = type.Equals("yellowproj") ? 111 : 91;
             
        }

        public void Update(Player player, Level arena)
        {
            if (time >= 0)
            {
                time--;
            }
            if (time == 0)
            {
                if (type.Equals("yellowproj"))
                    trackTime = 180;
                isShot = true;
                base.gridCoords = startpos;
                direction = (float)(rand.NextDouble() * 2 * Math.PI);
            }
            if (gridCoords.X<=arena.GetX() && gridCoords.Y <= arena.GetY() && gridCoords.X > 1 && gridCoords.Y > 1)
            {
                if (trackTime > 0)
                {
                    trackTime--;
                    if (trackTime <= 150)
                        direction = (float)Math.Atan2(player.gridCoords.Y - gridCoords.Y, player.gridCoords.X - gridCoords.X);
                }
                gridCoords.Y += (float)rand.NextDouble() / (trackTime > 0 && trackTime <= 180 ? 17 : 10) * (float)Math.Sin(direction);
                gridCoords.X += (float)rand.NextDouble() / (trackTime > 0 && trackTime <= 180 ? 17 : 10) * (float)Math.Cos(direction);
                if (isShot && player.IsHitBy(gridCoords, arena))
                {
                    player.Hit(1);
                    isShot = false;
                }
            }
            else
            {
                isShot = false;
            }
        }
        public void Draw(SpriteBatch sb, Level arena)
        {
            if (isShot)
            {
                sb.Draw(shadow, arena.GridToIsometric(new Vector3(gridCoords.X, gridCoords.Y, 0), 1280, 940), null, new Color(0, 0, 0, 60), 0, new Vector2(shadow.Width, shadow.Height)/2, type.Equals("yellowproj") ? 1f : 0.75f, SpriteEffects.None, 0);
                sb.Draw(text, arena.GridToIsometric(gridCoords, 1280, 940), null, Color.White, 0, type.Equals("yellowproj") ? new Vector2(97.5f, 71.5f) : new Vector2(98.5f, 91f), 1.5f, SpriteEffects.None, 1);
            }
        }
    }
}