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
    class Aranea : Object3D
    {
        ContentManager content;
        Player play;
        Level arena;
        Texture2D bluesheet;
        Texture2D yellowsheet;
        Texture2D curr;
        Texture2D idle;
        Texture2D shadowTexture;
        Texture2D healthcol;
        Texture2D healthbase;

        Vector2 currentSprite;
        Queue<Texture2D> queue = new Queue<Texture2D>();
        List<Projectile> projects = new List<Projectile>();

        bool isIdle = true;
        int animationTimer = 0;
        int yellowtime = -1;

        int numblue = 60;
        int numyellow = 40;

        public int maxHealth = 45, health;
        int hitInvincibility = 0;
        Rectangle source;

        public Aranea(IServiceProvider service, Player player, Level lev) : base("Aranea",new Vector3(5.5f,5.5f,0))
        {
            health = maxHealth;
            source = new Rectangle(0, 0, 196, 196);
            play = player;
            content = new ContentManager(service, "Content");
            arena = lev;
            idle = content.Load<Texture2D>("Aranea Idle");
            bluesheet = content.Load<Texture2D>("Aranea Blue");
            yellowsheet = content.Load<Texture2D>("Aranea Yellow");
            curr = content.Load<Texture2D>("Aranea Idle");
            shadowTexture = content.Load<Texture2D>("shadow");
            healthcol = content.Load<Texture2D>("Health Color");
            healthbase = content.Load<Texture2D>("Health Base");
            createProjectiles();

        }
        public bool IsHitBy(Vector3[] box, Level arena)
        {
            if (box[0].X > gridCoords.X + (1.5f * (61 / 2.0f)) / (arena.scale * (Tile.frontCorner.X - Tile.leftCorner.X)))
                return false;
            if (box[0].X + box[1].X < gridCoords.X - (1.5f * (61 / 2.0f)) / (arena.scale * (Tile.frontCorner.X - Tile.leftCorner.X)))
                return false;
            if (box[0].Y > gridCoords.Y + (1.5f * (61 / 2.0f)) / (arena.scale * (Tile.rightCorner.X - Tile.frontCorner.X)))
                return false;
            if (box[0].Y + box[1].Y < gridCoords.Y - (1.5f * (61 / 2.0f)) / (arena.scale * (Tile.rightCorner.X - Tile.frontCorner.X)))
                return false;
            if (box[0].Z > gridCoords.Z + (1.5f * (97)) / (arena.scale * Tile.height))
                return false;
            if (box[0].Z + box[1].Z < gridCoords.Z)
                return false;
            return true;
        }
        public void Hit(int damage)
        {
            if (hitInvincibility == 0)
            {
                health -= damage;
                hitInvincibility = 16; // Change this value to change how much invicibility the player gets
            }
        }
        private void createProjectiles()
        {
            for (int i = 0; i < numblue; i++)
            {
                projects.Add(new Projectile("blueproj", content, new Vector3(gridCoords.X, gridCoords.Y, 0.1f)));
            }
            for(int i = 0; i < numyellow; i++)
            {
                projects.Add(new Projectile("yellowproj", content, new Vector3(gridCoords.X, gridCoords.Y, 0.1f)));
            }
            
        }

        public void AttackBlue()
        {
            for(int i = 0; i < numblue; i++)
            {
                projects[i].shoot(play, arena);
            }
        }

        public void AttackYellow()
        {
            yellowtime = 60;
            for (int i = numblue; i < numblue+(numyellow/2); i++)
            {
                projects[i].shoot(play, arena);
            }
        }

        public void Update()
        {
            if (currentSprite.X * 196 >= curr.Width)
            {
                currentSprite.Y++;
                currentSprite.X = 0;
            }
            if (currentSprite.Y * 196 >= curr.Height)
            {
                currentSprite = new Vector2(0, 0);
                if (queue.Count != 0)
                {
                    curr = queue.Dequeue();
                    if (curr == bluesheet)
                    {
                        AttackBlue();
                    }
                    if (curr == yellowsheet)
                    {
                        AttackYellow();
                    }
                }
                else
                {
                    curr = idle;
                    isIdle = true;
                }
            }
            source = new Rectangle((int)currentSprite.X * 196, (int)currentSprite.Y * 196, 195, 195);
            if (animationTimer >= 5) // Change this number to change how fast each frame goes
            {
                animationTimer = 0;
                if (isIdle && queue.Count != 0)
                {
                    currentSprite = new Vector2(0, 0);
                    curr = queue.Dequeue();
                    if (curr == bluesheet)
                    {
                        AttackBlue();
                    }
                    if (curr == yellowsheet)
                    {
                        AttackYellow();
                    }
                    isIdle = false;
                }
                currentSprite.X++;
            }
            else
                animationTimer++;

            if (!isIdle)
            {
                foreach(Projectile p in projects)
                {
                    p.Update(play, arena);
                }
            }
            if(IsHitBy(play.GetAttackBox(), arena))
            {
                Hit(1);
            }
            if (hitInvincibility > 0)
            {
                hitInvincibility--;
            }
            if (yellowtime == 0)
            {
                for (int i = numblue+(numyellow/2); i < projects.Count; i++)
                {
                    projects[i].shoot(play, arena);
                }
                yellowtime = -1;
            }
            if (yellowtime > 0)
                yellowtime--;
        }
        public void pattern1()
        {
            if (queue.Count == 0)
            {
                queue.Enqueue(bluesheet);
                queue.Enqueue(bluesheet);
            }
        }
        public void pattern2()
        {
            if (queue.Count == 0)
            {
                queue.Enqueue(yellowsheet);
                queue.Enqueue(bluesheet);
                queue.Enqueue(yellowsheet);
            }

        }
        public List<Projectile> getProjects()
        {
            return projects;
        }
        public void Draw(SpriteBatch sb)
        {
            //draw the current piece of the sheet, also draw boss's health bar & projectiles (if any)
            sb.Draw(shadowTexture, arena.GridToIsometric(new Vector3(gridCoords.X, gridCoords.Y, 0), 1280, 940), null, new Color(0, 0, 0, 100), 0, new Vector2(shadowTexture.Width, shadowTexture.Height)/2, 4.5f, SpriteEffects.None, 0);
            sb.Draw(curr, arena.GridToIsometric(gridCoords,1280,940), source, Color.White, 0, new Vector2(idle.Width/2, 183), (float)1.5, SpriteEffects.None, 1);
            sb.Draw(healthcol, arena.GridToIsometric(new Vector3(gridCoords.X, gridCoords.Y, gridCoords.Z + 3.15f), 1280, 940) - new Vector2(healthcol.Width*0.375f*(maxHealth-health)/maxHealth, 0), null, Color.White, 0, new Vector2(idle.Width/2, idle.Height), new Vector2(0.75f*health/maxHealth, 0.75f), SpriteEffects.None, 1);
            sb.Draw(healthbase, arena.GridToIsometric(new Vector3(gridCoords.X, gridCoords.Y, gridCoords.Z + 3.15f), 1280, 940), null, Color.White, 0, new Vector2(idle.Width / 2, idle.Height), 0.75f, SpriteEffects.None, 1);
        }
    }
}
