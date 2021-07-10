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
    class Jets : GameObjectAbstract
    {
        //Information for the texture objects and creating the fighter jets
        public Texture2D texture;
        Rectangle rect = new Rectangle(700, -40, 30, 30);
        Rectangle rect2 = new Rectangle(650, -40, 30, 30);
        private int timer = 0;
        //ship and reference objects
        Ship ship;
        //list of rectangle objects for shits fired
        List<Rectangle> shots;
        //length of array
        int size = 0;
        //rectangle multiplier
        int rectOneMultiplier = 1;
        int rectTwoMultiplier = -1;
        //randomizer and develops ships
        Random rand = new Random();
        public Jets(ref Ship sh)
        {
            shots = new List<Rectangle>();
            ship = sh;
            Init();
        }

        //booleans for checking certain cases
        public bool complete = false;
        bool ready = false;
        public override void Update(GameTime gameTime)
        {
            try
            {
                //checks if jets must be moved onto screen
                if (rect.Y < 235 && timer == 0)
                {
                    rect.Y += 5;
                    rect2.Y += 5;
                }
                else
                {
                    ready = true;
                }
                //starts wave and randomly shifts
                if (ready)
                {
                    timer++;
                }
                if (timer > 0 && timer < 1200 && ready)
                {
                    rect.Y += 5 * rectOneMultiplier;
                    rect2.Y += 5 * rectTwoMultiplier;
                    //randomizer for up and down movements
                    if (rect.Y > 490)
                    {
                        rectOneMultiplier *= -1;
                    }
                    if (rect.Y < 10)
                    {
                        rectOneMultiplier *= -1;
                    }
                    if (rect2.Y > 490)
                    {
                        rectTwoMultiplier *= -1;
                    }
                    if (rect2.Y < 10)
                    {
                        rectTwoMultiplier *= -1;
                    }
                    int randomizer = rand.Next(0, 100);
                    if (randomizer > 95)
                    {
                        rectTwoMultiplier *= -1;
                    }
                    randomizer = rand.Next(0, 100);
                    if (randomizer > 95)
                    {
                        rectOneMultiplier *= -1;
                    }
                    if (rect.Y > 780)
                    {
                        rectOneMultiplier = -1;
                    }
                    if (rect2.Y > 780)
                    {
                        rectTwoMultiplier = -1;
                    }
                    if (rect.Y < 10)
                    {
                        rectOneMultiplier = 1;
                    }
                    if (rect2.Y < 10)
                    {
                        rectTwoMultiplier = 1;
                    }
                    //checks if shots can be added to the list
                    if (timer % rand.Next(10, 18) == 0 && ready)
                    {
                        shots.Add(new Rectangle(rect.X, rect.Y, 2, 10));
                        shots.Add(new Rectangle(rect2.X, rect2.Y, 2, 10));
                        size += 2;
                    }
                    //checks whrre the shots are currently on the screen and moves them
                    for (int i = 0; i < size; i++)
                    {
                        Rectangle temp = shots[i];
                        temp.X -= 14;
                        shots[i] = temp;

                    }
                    //removes shots from the array
                    for (int i = 0; i < size; i++)
                    {
                        Rectangle temp = shots[i];
                        if (temp.X < -10)
                        {
                            shots.Remove(temp);
                            size--;
                        }
                    }
                    //checks if the shots hit the player ship
                    for (int i = 0; i < size; i++)
                    {
                        Rectangle temp = shots[i];
                        if (temp.Intersects(ship.getShipRect()))
                        {
                            ship.hit();
                        }

                    }
                }
                //moves jets off screen when game is complete
                if (timer > 1200)
                {
                    rect.Y -= 30;
                    rect2.Y -= 30;
                    if (rect.Y <= -200)
                    {
                        complete = true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //completion of round or each wave
        public bool roundComplete()
        {
            return rect.X < -10;
        }
        //draws the objects
        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            sb.Draw(texture, rect, Color.White);
            sb.Draw(texture, rect2, Color.White);
            for(int i = 0; i < size; i++)
            {
                sb.Draw(texture, shots[i], Color.White);
            }

        }
        //resets back to default values
        public override void reset()
        {
            timer = 0;
            rect = new Rectangle(700, -20, 30, 30);
            rect2 = new Rectangle(650, -20, 30, 30);
            shots.Clear();
            ready = false;
            size = 0;
            rectOneMultiplier = 1;
            rectTwoMultiplier = -1;
            Init();
        }

    }
}
