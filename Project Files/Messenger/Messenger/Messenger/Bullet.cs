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
using System.Collections;


namespace Messenger
{
    class Bullet : GameObjectAbstract
    {
        //Information for creating an individual bullet ship
        public Texture2D texture;
        Rectangle rect = new Rectangle(900, 240, 20, 20);
        //references for timer, ship, and speed
        private int timer = 0;
        Ship ship;
        int speed;
        //creates a bullet ship
        public Bullet(ref Ship sh)
        {
            ship = sh;
            
            Init();
        }
        public override void Update(GameTime gameTime)
        {
            try
            {
                //based on time, the ship will enter a new wave and it may go faster.
                timer++;
                if (timer <= 60)
                {
                    rect.X -= 5;
                }
                if (timer > 60 && timer <= 180)
                {
                    //locks onto target of ship
                    //This will move the bullet ship upward
                    if (ship.getShipRect().Y + 5 > rect.Y)
                    {
                        rect.Y += 12;
                    }
                    //This will move the bullet ship downward
                    else if (ship.getShipRect().Y - 5 < rect.Y)
                    {
                        rect.Y -= 12;
                    }
                }
                //rate the bullet will trave at if past three seconds (last wave)
                if (timer > 180)
                {
                    rect.X -= 60;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //Checks if bullet hits ship
            if(rect.Intersects(ship.getShipRect()))
            {
                ship.hit();
            }
            
        }
        //checks when round is complete
        public bool roundComplete()
        {
            return rect.X < -10;
        }
        //draws each individual bullet that is created
        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            sb.Draw(texture, rect, Color.White);
        }
        //resets variables back to default
        public override void reset()
        {
            timer = 0;
            rect = new Rectangle(900, 240, 20, 20);
            Init();
        }
        //increases buffness or speed of the bullet upon each wave
        public void buff()
        {
            speed += 10;
        }

    }
}
