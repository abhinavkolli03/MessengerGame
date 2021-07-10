using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;

namespace Messenger
{
    public class Ship : GameObjectAbstract
    {
        //information for developing an individual player ship
        public Texture2D texture;
        private KeyboardState oldKB = Keyboard.GetState();
        private Rectangle rect;
        private Color color = Color.White;
        //develops references and specifc powerups, inventory, etc.
        public int inventory;
        public SpriteFont font;
        bool power = false;
        int tTime = 0;

        //constructs hip class to default
        public Ship()
        {
            inventory = 0;
            type = Type.SHIP;
            rect = new Rectangle(50, 235, 30, 30);
            Init();
        }

        //factors that help mve the ship by specific increments based on waves
        private int xfactor = 100;

        //establishes constant of x movement
        public void lockFactors()
        {
            xfactor = 100;
        }
        //checks what to do when hit
        public void hit()
        {
            if(!power)
                this.Destroy();
        }

        //variabalized the xfactor
        public void unlockFactors()
        {
            xfactor = 870;
        }
        
        public override void Update(GameTime gameTime)
        {
            try
            {
                tTime--;
                KeyboardState kb = Keyboard.GetState();
                //keyboard input and movement that corresponds with each
                if (kb.IsKeyDown(Keys.Down))
                {
                    rect.Y = Math.Min(rect.Y + 10, 470);
                }
                if (kb.IsKeyDown(Keys.Up))
                {
                    rect.Y = Math.Max(0, rect.Y - 10);
                }
                if (kb.IsKeyDown(Keys.Right))
                {
                    rect.X = Math.Min(rect.X + 10, xfactor);
                }
                if (kb.IsKeyDown(Keys.Left))
                {
                    rect.X = Math.Max(0, rect.X - 10);
                }
                //uses up the pwer up and removes from the inventory
                if (kb.IsKeyDown(Keys.Space) && !oldKB.IsKeyDown(Keys.Space) && inventory > 0)
                {
                    inventory--;
                    tTime = 180;
                }
                if (tTime > 0)
                {
                    power = true;
                }
                else
                {
                    power = false;
                }
                oldKB = kb;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }            
        }
        //resets and gets new ship rectangle
        public Rectangle getShipRect() { return this.rect; }
        //resets back to original variable values
        public override void reset()
        {
            rect = new Rectangle(50, 235, 30, 30);
            inventory = 0;
            Init();

        }
        //draw class that establishes ship and inventory
        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            sb.Draw(texture, this.getShipRect(), color);
            sb.DrawString(font, "inventory:"  + inventory, new Vector2(700, 450), Color.White);

        }

    }
}