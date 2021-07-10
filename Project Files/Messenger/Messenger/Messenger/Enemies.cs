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
    //Class controls the second level and creates the bullet objects and the enemy jet bosses
    class Enemies : GameObjectAbstract
    {
        //Develops the ship, bullet, and fighters
        Ship ship;
        Bullet bullet;
        Jets fighters;
        //works as round counter between the bullets and the jets round
        int round = 1;
        //bullet texture
        Texture2D bulletT;
        //checks if wave is complete
        public bool complete = false;
        //initializes objects necessary for the enemies level (round 2)
        public Enemies(ref Ship sh, Texture2D bulleT, Texture2D shooter)
        {
            
            ship = sh;
            bullet = new Bullet(ref ship);
            bullet.texture = bulleT;
            bulletT = bulleT;
            fighters = new Jets(ref ship);
            fighters.texture = shooter;
            Init();
        }
        public override void Update(GameTime gameTime)
        {
            try
            {
                //Creates the 5 rounds that involve the bullet
                if (round > 0 && round < 6)
                {
                    bullet.Update(gameTime);
                    if (bullet.roundComplete())
                    {
                        //resets and buffs after each wave
                        round++;
                        bullet.reset();
                        bullet.buff();
                    }
                }
                //Sets up the fighter jets for round 6
                if (round == 6)
                {
                    fighters.Update(gameTime);
                    if (fighters.complete == true)
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
        //draw methd for all the bullets and fighters
        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            if (round > 0 && round < 6)
            {
                bullet.Draw(gameTime, sb);
            }
            if (round == 6)
            {
                fighters.Draw(gameTime,sb);
            }
        }
        //resets all the variables back to default
        public override void reset()
        {
            round = 1;
            bullet = new Bullet(ref ship);
            fighters.reset();
            bullet.texture = bulletT;
            
        }
    }
}
