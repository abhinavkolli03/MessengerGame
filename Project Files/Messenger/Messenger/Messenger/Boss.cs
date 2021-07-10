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
using Microsoft.Xna.Framework.Design;
using System.Collections;

namespace Messenger
{
    class Boss : GameObjectAbstract
    {
        //checks once the mother ship is dead for the game class
        public bool endTrue = false;
        //information for the mother ship
        public Texture2D texture;
        public Rectangle rect = new Rectangle(900, 190, 100, 100);
        //timers and power levels (health)
        private int timer = 0;
        private int powerLevel;
        private int powerLevelTimer = 0;
        //Checks if the bullet fire has already increased or not based on waves
        bool powerDrop = false;
        //list for bullets
        List<Rectangle> shots;
        //reference to ship
        Ship ship;
        //randomizer for shot variability
        int randomizerInt = 98;
        //checks if dead mother ship
        public bool dead;
        //randomizer
        Random rand = new Random();
        //size of bullet array
        int size = 0;
        //constructs power level, ship, shots
        public Boss(ref Ship sh)
        {
            ship = sh;
            powerLevel = 100;
            shots = new List<Rectangle>();
            Init();
        }
        public override void Update(GameTime gameTime)
        {
            //timer increases
            timer++;
            powerLevelTimer++;
            //decreases power lebel over time
            if (powerLevelTimer / 45 == 1 && powerLevel > 0)
            {
                powerLevel -= 2;
                powerLevelTimer = 0;
            }
            if (powerLevel == 0)
                dead = true;
            try
            {
                //checks while not dead
                if (!dead)
                {
                    //mother ship enters screen from right
                    if (timer <= 120)
                    {
                        rect.X -= 3;
                    }
                    //mother ship starts the attack
                    if (timer > 120)
                    {
                        //locks onto player target
                        if (ship.getShipRect().Y + 5 > rect.Y + 30)
                        {
                            rect.Y += 2;
                        }
                        else if (ship.getShipRect().Y - 5 < rect.Y + 30)
                        {
                            rect.Y -= 2;
                        }
                        //randomizes for shot variability in five different parts of the mother ship. 
                        //They are all separated from each other equally and they will rapidly at various times.
                        for (int i = 0; i < 5; i++)
                        {
                            int randomizer = rand.Next(0, 100);
                            if (randomizer > randomizerInt)
                            {
                                shots.Add(new Rectangle(rect.X, rect.Y + (i * 22), 4, 10));
                                size++;
                            }
                        }
                        //if the power level is at 25, 50, or 75, the mother ship will unleash a power move or beam that 
                        //can vaporize the player.
                        if (powerLevel == 26 || powerLevel == 50 || powerLevel == 74)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                shots.Add(new Rectangle(rect.X, rect.Y + (i * 22), 4, 10));
                                size++;
                            }
                        }
                        //movement of bullets
                        for (int i = 0; i < size; i++)
                        {
                            Rectangle temp = shots[i];
                            temp.X -= 8;
                            shots[i] = temp;
                        }
                        //removes bullets if off of screen
                        for (int i = 0; i < size; i++)
                        {
                            Rectangle temp = shots[i];
                            if (temp.X < -10)
                            {
                                shots.Remove(temp);
                                size--;
                            }
                        }
                        //checks each bullet if it hits the player
                        for (int i = 0; i < size; i++)
                        {
                            Rectangle temp = shots[i];
                            if (temp.Intersects(ship.getShipRect()))
                            {
                                ship.hit();
                            }
                        }
                        //at power level 70, the mother ship will increase firing rate
                        if (powerLevel == 70 && !powerDrop)
                        {
                            randomizerInt -= 2;
                            powerDrop = true;
                        }
                        else if (powerLevel < 70)
                            powerDrop = false;
                    }
                }
                //this is run when the mother ship is dead and the player beat the enemy
                else
                {
                    //for game class
                    endTrue = true;
                    //moves bullets to the left off screen
                    for (int i = 0; i < size; i++)
                    {
                        Rectangle temp = shots[i];
                        temp.X -= 8;
                        shots[i] = temp;
                    }
                    //removes bullets
                    for (int i = 0; i < size; i++)
                    {
                        Rectangle temp = shots[i];
                        if (temp.X < -10)
                        {
                            shots.Remove(temp);
                            size--;
                        }
                    }
                    //checks if bullets hit the player
                    for (int i = 0; i < size; i++)
                    {
                        Rectangle temp = shots[i];
                        if (temp.Intersects(ship.getShipRect()))
                        {
                            ship.hit();
                        }
                    }
                    //checks if mother ship hits the player
                    rect.X--;
                    if (rect.Intersects(ship.getShipRect()))
                        ship.hit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //returns power level
        public int getPowerLevel()
        {
            return powerLevel;
        }
        //draw method creates the mother ship and the bullets
        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            sb.Draw(texture, rect, Color.White);
            for (int i = 0; i < size; i++)
            {
                sb.Draw(texture, shots[i], Color.White);
            }
        }
        //restes all variables to default
        public override void reset()
        {
            powerLevel = 100;
            timer = 0;
            rect = new Rectangle(900, 240, 100, 100);
            shots.Clear();
            size = 0;
            randomizerInt = 97;
            Init();
        }

    }
}
