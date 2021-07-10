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
using System.IO;
namespace Messenger
{
    class Asteroid : GameObjectAbstract
    {
        //Information for developing one asteroid object
        public Texture2D texture;
        //List of asteroids
        public List<Rectangle> oids;
        //rate of asteroid creation on screen
        public int rate = 8;
        //Randomizer
        public Random rand = new Random();
        //length of array
        int size = 0;
        //references used throughout the code for asteroid creation
        private int timer = 0;
        Ship ship;
        //Checks if asteroid was created
        public bool complete = false;
        //Calls on power up class for inventory and sees if ship is hit
        public powerUp powerup;

        //constructor for asteroid and creating default asteroid objects
        public Asteroid(ref Ship sh)
        {
            ship = sh;
            powerup = new powerUp(ref sh);
            type = Type.ASTEROID;
            oids = new List<Rectangle>();
            Init();
        }

        //rates and counters to check for individual asteroids
        private int count;
        private int astSize = 0;
        private int astSpeed = 8;

        public override void Update(GameTime gameTime)
        {
            try
            {
                timer++;
                //Using timer, the asteroids will go faster based on each individual wave. In intervals of 60 seconds
                if (timer / 60 == 10)
                {
                    rate = 7;
                    astSpeed = 9;
                }
                if (timer / 60 == 20)
                {
                    rate = 6;
                    astSpeed = 10;
                }
                if (timer / 60 == 30)
                {
                    rate = 5;
                    astSpeed = 12;
                }
                if (timer / 60 == 40)
                {
                    rate = 4;
                    astSpeed = 14;
                }

                if (timer / 60 >= 48)
                {
                    complete = true;
                }
                if (timer / 60 >= 45)
                {

                }
                else
                {
                    count++;
                    //creates and adds new asteroids to the screen from the right
                    astSize = rand.Next(8, 20);
                    if (count % rate == 0)
                    {
                        oids.Add(new Rectangle(950, rand.Next(0, 470), astSize, astSize));
                        size += 1;
                    }
                }
            }
            catch (InternalBufferOverflowException e)
            {
                Console.WriteLine("Crash Error: Asteroid array overflow");
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            //Goes through asteroids list and updates each asteroid to a new position based on speed.
            for (int i = 0; i < size; i++)
            {
                Rectangle temp = oids[i];
                temp.X -= astSpeed;
                oids[i] = temp;

            }
            //If asteroids are off screen, then they will be removed from the list an will not be updated
            for (int i = 0; i < size; i++)
            {
                Rectangle temp = oids[i];
                if (temp.X < -50)
                {
                    oids.Remove(temp);
                    size--;
                }
            }
            //checks if asteroid hits the ship and institutes the hit method
            for (int i = 0; i < size; i++)
            {
                Rectangle temp = oids[i];
                if (temp.Intersects(ship.getShipRect()))
                {
                    ship.hit();
                }

            }
            //uodates the powerup class object to get more powerups
            powerup.Update(gameTime);
        }
        //draws each individual asteroid
        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            for (int i = 0; i < size; i++)
            {
                sb.Draw(texture, oids[i], Color.White);
            }
            //draws a specific powerup given its right time
            powerup.Draw(gameTime, sb);
            
        }
        //resets all variables back to default if needed to be reset
        public override void reset()
        {
            oids = new List<Rectangle>();
            timer = 0;
            size = 0;
            rate = 7;
            count = 0;
            astSize = 0;
            astSpeed = 8;
            powerup.reset();
            Init();


        }
    }
}
