using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Undergods
{
    // Player Version 2.0
    // Using Object3D Version 1.0
    // Using Level Version 1.0
    // Using Tile Version 1.1
    class Player : Object3D
    {
        public static KeyboardState oldKb;
        public static MouseState oldMouse;

        public Texture2D texture, shadowTexture, attackTexture, dashTexture, upAttackTexture, healthbase, healthcol;
        public float speed = 0.03f;
        public float scale = 0.5f, shadowScale = 1.5f;
        public int maxHp = 3, hp;

        private float zSpeed = 0;
        private int dashCooldown = 0;
        private int hitInvincibility = 0;

        private Rectangle[] sprites, attacks, dashes;
        private Vector2[] attackOrigins, dashOrigins;
        private int currentSprite = 0, animationTimer = 0, action = 0;
        private int attackSprite = 0, attackTimer = 0;
        private bool isAttacking = false;

        Random rng = new Random();
        SoundEffect Sword;
        List<SoundEffect> Grunt;

        public Player(SoundEffect Sword, List<SoundEffect> Grunt, Texture2D texture, Texture2D attackTexture, Texture2D upAttackTexture, Texture2D dashTexture, Texture2D shadowTexture, Texture2D healthbase, Texture2D healthcol, Vector3 gridCoords) : base("Player", gridCoords)
        {
            this.texture = texture;
            this.attackTexture = attackTexture;
            this.upAttackTexture = upAttackTexture;
            this.dashTexture = dashTexture;
            this.shadowTexture = shadowTexture;
            this.healthbase = healthbase;
            this.healthcol = healthcol;
            this.Sword = Sword;
            oldKb = Keyboard.GetState();
            oldMouse = Mouse.GetState();

            hp = maxHp;

            this.Grunt = Grunt;

            sprites = new Rectangle[8];
            sprites[0] = new Rectangle(40, 34, 109-40, 133-34);
            sprites[1] = new Rectangle(280, 34, 349-280, 133-34);
            sprites[2] = new Rectangle(520, 34, 589-520, 133-34);
            sprites[3] = new Rectangle(40, 274, 109-40, 373-274);
            sprites[4] = new Rectangle(285, 514, 339-285, 613-514);
            sprites[5] = new Rectangle(525, 274, 579-525, 373-274);
            sprites[6] = new Rectangle(45, 514, 99-45, 613-514);
            sprites[7] = new Rectangle(285, 274, 339-285, 373-275);

            attacks = new Rectangle[12];
            attacks[0] = new Rectangle(20, 62, 101-20, 183-62);
            attacks[1] = new Rectangle(375, 76, 451-375, 183-76);
            attacks[2] = new Rectangle(42, 426, 127-42, 525-426);
            attacks[3] = new Rectangle(76, 50, 177-76, 149-50);
            attacks[4] = new Rectangle(316, 50, 405-316, 168-50);
            attacks[5] = new Rectangle(553, 50, 615-553, 178-50);
            attacks[6] = new Rectangle(81, 286, 170-81, 389-286);
            attacks[7] = new Rectangle(321, 290, 428-321, 389-290);
            attacks[8] = new Rectangle(561, 290, 650-561, 403-290);
            attacks[9] = new Rectangle(37, 526, 125-37, 629-526);
            attacks[10] = new Rectangle(259, 530, 365-259, 629-530);
            attacks[11] = new Rectangle(516, 530, 605-516, 643-530);

            attackOrigins = new Vector2[12];
            attackOrigins[0] = new Vector2((sprites[0].Width-10)/2 + 22, sprites[0].Height + 22);
            attackOrigins[1] = new Vector2((sprites[0].Width-10)/2 + 9, sprites[0].Height + 8);
            attackOrigins[2] = new Vector2((sprites[0].Width-10)/2, sprites[0].Height);
            attackOrigins[3] = new Vector2((sprites[2].Width-10)/2, sprites[2].Height);
            attackOrigins[4] = new Vector2((sprites[2].Width-10)/2, sprites[2].Height);
            attackOrigins[5] = new Vector2((sprites[2].Width-10)/2 + 3, sprites[2].Height);
            attackOrigins[6] = new Vector2((sprites[4].Width-10)/2, sprites[4].Height + 4);
            attackOrigins[7] = new Vector2((sprites[4].Width-10)/2, sprites[4].Height);
            attackOrigins[8] = new Vector2((sprites[4].Width-10)/2, sprites[4].Height);
            attackOrigins[9] = new Vector2((sprites[6].Width-10)/2 + 44, sprites[6].Height + 4);
            attackOrigins[10] = new Vector2((sprites[6].Width-10)/2 + 62, sprites[6].Height);
            attackOrigins[11] = new Vector2((sprites[6].Width-10)/2 + 45, sprites[6].Height);

            dashes = new Rectangle[4];
            dashes[0] = new Rectangle(47, 21, 137-47, 154-21);
            dashes[1] = new Rectangle(257, 55, 321-257, 192-55);
            dashes[2] = new Rectangle(238, 255, 332-238, 354-255);
            dashes[3] = new Rectangle(67, 255, 161-67, 354-255);

            dashOrigins = new Vector2[4];
            dashOrigins[0] = new Vector2(dashes[0].Width/2 - 1, dashes[0].Height);
            dashOrigins[1] = new Vector2(sprites[2].Width/2, sprites[2].Height);
            dashOrigins[2] = new Vector2((sprites[4].Width-10)/2 + 50, sprites[4].Height);
            dashOrigins[3] = new Vector2((sprites[6].Width-10)/2, sprites[6].Height);
        }

        public Vector3[] GetAttackBox()
        {
            Vector3[] output = new Vector3[2];
            if (!isAttacking)
                return new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 0, 0)};

            if (currentSprite/2 == 0)
                output[0] = new Vector3(gridCoords.X, gridCoords.Y, gridCoords.Z);
            else if (currentSprite/2 == 1)
                output[0] = new Vector3(gridCoords.X-0.5f, gridCoords.Y-0.5f, gridCoords.Z);
            else if (currentSprite/2 == 2)
                output[0] = new Vector3(gridCoords.X, gridCoords.Y-0.5f, gridCoords.Z);
            else if (currentSprite/2 == 3)
                output[0] = new Vector3(gridCoords.X-0.5f, gridCoords.Y, gridCoords.Z);

            output[1] = new Vector3(0.5f, 0.5f, 2/3f);
            return output;
        }

        public bool IsHitBy(Vector3[] box, Level arena)
        {
            if (box[0].X > gridCoords.X+(scale*(sprites[currentSprite].Width/2.0f))/(arena.scale*(Tile.frontCorner.X-Tile.leftCorner.X)))
                return false;
            if (box[0].X+box[1].X < gridCoords.X-(scale*(sprites[currentSprite].Width/2.0f))/(arena.scale*(Tile.frontCorner.X-Tile.leftCorner.X)))
                return false;
            if (box[0].Y > gridCoords.Y+(scale*(sprites[currentSprite].Width/2.0f))/(arena.scale*(Tile.rightCorner.X-Tile.frontCorner.X)))
                return false;
            if (box[0].Y+box[1].Y < gridCoords.Y-(scale*(sprites[currentSprite].Width/2.0f))/(arena.scale*(Tile.rightCorner.X-Tile.frontCorner.X)))
                return false;
            if (box[0].Z > gridCoords.Z+(scale*(sprites[currentSprite].Height))/(arena.scale*Tile.height))
                return false;
            if (box[0].Z+box[1].Z < gridCoords.Z)
                return false;
            return true;
        }

        public bool IsHitBy(Vector3 point, Level arena)
        {
            return IsHitBy(new Vector3[] {point, new Vector3()}, arena);
        }

        public void Hit(int damage)
        {
            if (hitInvincibility == 0)
            {
                hp -= damage;
                hitInvincibility = 30; // Change this value to change how much invicibility the player gets
                Grunt[rng.Next(Grunt.Count)].Play();
            }
        }

        public void Update(Level arena)
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (kb.IsKeyDown(Keys.A))
            {
                gridCoords.X -= dashCooldown > 30 ? (kb.IsKeyUp(Keys.W) && kb.IsKeyUp(Keys.S) ? speed*4 : speed*2) : (kb.IsKeyUp(Keys.W) && kb.IsKeyUp(Keys.S) ? speed : speed/2);
                gridCoords.Y += dashCooldown > 30 ? (kb.IsKeyUp(Keys.W) && kb.IsKeyUp(Keys.S) ? speed*4 : speed*2) : (kb.IsKeyUp(Keys.W) && kb.IsKeyUp(Keys.S) ? speed : speed/2);
                action = 4;
            }
            if (kb.IsKeyDown(Keys.D))
            {
                gridCoords.X += dashCooldown > 30 ? (kb.IsKeyUp(Keys.W) && kb.IsKeyUp(Keys.S) ? speed*4 : speed*2) : (kb.IsKeyUp(Keys.W) && kb.IsKeyUp(Keys.S) ? speed : speed/2);
                gridCoords.Y -= dashCooldown > 30 ? (kb.IsKeyUp(Keys.W) && kb.IsKeyUp(Keys.S) ? speed*4 : speed*2) : (kb.IsKeyUp(Keys.W) && kb.IsKeyUp(Keys.S) ? speed : speed/2);
                action = 3;
            }
            if (kb.IsKeyDown(Keys.W))
            {
                gridCoords.X -= dashCooldown > 30 ? (kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D) ? speed*4 : speed*2) : (kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D) ? speed : speed/2);
                gridCoords.Y -= dashCooldown > 30 ? (kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D) ? speed*4 : speed*2) : (kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D) ? speed : speed/2);
                action = 2;
            }
            if (kb.IsKeyDown(Keys.S))
            {
                gridCoords.X += dashCooldown > 30 ? (kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D) ? speed*4 : speed*2) : (kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D) ? speed : speed/2);
                gridCoords.Y += dashCooldown > 30 ? (kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D) ? speed*4 : speed*2) : (kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D) ? speed : speed/2);
                action = 1;
            }
            if (kb.IsKeyUp(Keys.W) && kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.S) && kb.IsKeyUp(Keys.D))
                action = 0;

            if (!isAttacking && mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
            {
                isAttacking = true;
                attackTimer = 15;
                Sword.Play();
            }
            else if (attackTimer > 0)
                attackTimer--;
            else
                isAttacking = false;

            SetSprite();
            if (isAttacking)
                AnimateAttack();

            if (gridCoords.Z == 0 && kb.IsKeyDown(Keys.Space) && oldKb.IsKeyUp(Keys.Space))
                zSpeed = 0.25f;
            gridCoords.Z += zSpeed;
            zSpeed -= 0.025f;

            if (dashCooldown == 0 && kb.IsKeyDown(Keys.LeftShift) && oldKb.IsKeyUp(Keys.LeftShift))
                dashCooldown = 45;
            if (dashCooldown > 0)
                dashCooldown--;

            if (gridCoords.X < 1 + (scale*(sprites[currentSprite].Width/2.0f))/(arena.scale*(Tile.frontCorner.X-Tile.leftCorner.X)))
                gridCoords.X = 1 + (scale*(sprites[currentSprite].Width/2.0f))/(arena.scale*(Tile.frontCorner.X-Tile.leftCorner.X));
            if (gridCoords.X > arena.GetX())
                gridCoords.X = arena.GetX();
            if (gridCoords.Y < 1 + (scale*(sprites[currentSprite].Width/2.0f))/(arena.scale*(Tile.rightCorner.X-Tile.frontCorner.X)))
                gridCoords.Y = 1 + (scale*(sprites[currentSprite].Width/2.0f))/(arena.scale*(Tile.rightCorner.X-Tile.frontCorner.X));
            if (gridCoords.Y > arena.GetY())
                gridCoords.Y = arena.GetY();
            if (gridCoords.Z < 0)
                gridCoords.Z = 0;
            if (gridCoords.Z > arena.GetZ())
                gridCoords.Z = arena.GetZ();

            if (hitInvincibility > 0)
                hitInvincibility--;

            oldKb = kb;
            oldMouse = mouse;
        }

        private void SetSprite()
        {
            if (action == 0)
            {
                animationTimer = 10;
                currentSprite = (currentSprite/2)*2;
            }
            else if (action == 1)
            {
                if (currentSprite/2 != 0)
                {
                    currentSprite = 0;
                    animationTimer = 10;
                }
                if (animationTimer > 0)
                    animationTimer--;
                else
                {
                    currentSprite = (currentSprite+1) % 2;
                    animationTimer = 10;
                }
            }
            else if (action == 2)
            {
                if (currentSprite/2 != 1)
                {
                    currentSprite = 2;
                    animationTimer = 10;
                }
                if (animationTimer > 0)
                    animationTimer--;
                else
                {
                    currentSprite = (currentSprite+1) % 2 + 2;
                    animationTimer = 10;
                }
            }
            else if (action == 3)
            {
                if (currentSprite/2 != 2)
                {
                    currentSprite = 4;
                    animationTimer = 10;
                }
                if (animationTimer > 0)
                    animationTimer--;
                else
                {
                    currentSprite = (currentSprite+1) % 2 + 4;
                    animationTimer = 10;
                }
            }
            else if (action == 4)
            {
                if (currentSprite/2 != 3)
                {
                    currentSprite = 6;
                    animationTimer = 10;
                }
                if (animationTimer > 0)
                    animationTimer--;
                else
                {
                    currentSprite = (currentSprite+1) % 2 + 6;
                    animationTimer = 10;
                }
            }
        }

        private void AnimateAttack()
        {
            if (currentSprite/2 == 1)
            {
                if (attackTimer > 10)
                    attackSprite = 0;
                else if (attackTimer > 5)
                    attackSprite = 1;
                else
                    attackSprite = 2;
            }
            else if (currentSprite/2 == 0)
            {
                if (attackTimer > 10)
                    attackSprite = 3;
                else if (attackTimer > 5)
                    attackSprite = 4;
                else
                    attackSprite = 5;
            }
            else if (currentSprite/2 == 2)
            {
                if (attackTimer > 10)
                    attackSprite = 6;
                else if (attackTimer > 5)
                    attackSprite = 7;
                else
                    attackSprite = 8;
            }
            else if (currentSprite/2 == 3)
            {
                if (attackTimer > 10)
                    attackSprite = 9;
                else if (attackTimer > 5)
                    attackSprite = 10;
                else
                    attackSprite = 11;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Level arena, int screenWidth, int screenHeight)
        {
            spriteBatch.Draw(shadowTexture, arena.GridToIsometric(new Vector3(gridCoords.X-0.05f, gridCoords.Y-0.05f, 0), screenWidth, screenHeight), null, new Color(0, 0, 0, 100), 0, new Vector2(shadowTexture.Width, shadowTexture.Height)/2, shadowScale, SpriteEffects.None, 0);
            spriteBatch.Draw((dashCooldown > 30 ? dashTexture : (isAttacking ? (attackSprite >= 3 ? attackTexture : upAttackTexture) : texture)), arena.GridToIsometric(gridCoords, screenWidth, screenHeight), (dashCooldown > 30 ? dashes[currentSprite/2] : (isAttacking ? attacks[attackSprite] : sprites[currentSprite])), hitInvincibility > 0 ? Color.Red : Color.White, 0, (dashCooldown > 30 ? dashOrigins[currentSprite/2] : (isAttacking ? attackOrigins[attackSprite] : new Vector2(sprites[currentSprite].Width/2, sprites[currentSprite].Height))), scale, SpriteEffects.None, 0);
            spriteBatch.Draw(healthcol, arena.GridToIsometric(new Vector3(gridCoords.X, gridCoords.Y, gridCoords.Z + 1f), 1280, 940) - new Vector2(healthcol.Width*0.3f*(maxHp-hp)/maxHp, 0), null, Color.Red, 0, new Vector2(healthcol.Width/2,healthcol.Height/2), new Vector2(0.6f*hp/maxHp, 0.6f), SpriteEffects.None, 1);
            spriteBatch.Draw(healthbase, arena.GridToIsometric(new Vector3(gridCoords.X, gridCoords.Y, gridCoords.Z + 1f), 1280, 940), null, Color.White, 0, new Vector2(healthbase.Width/2, healthbase.Height/2), 0.6f, SpriteEffects.None, 1);
        }
    }
}
