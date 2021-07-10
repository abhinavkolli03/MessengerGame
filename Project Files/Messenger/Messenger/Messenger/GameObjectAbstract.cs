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
    //abstract class that all class inherit
    public abstract class GameObjectAbstract
    {
        //mandatory data fields that must be present
        protected bool isExists;
        protected static Game1 game;
        protected static int enemeies = 0;
        public enum Type { SHIP, ASTEROID, ENEMY, BOSS, DEFAULT, LASER };
        protected Type type;

        //sets up game
        public static void setGame(Game1 g)
        {
            game = g;
        }
        //initiailize variable for saying object exists
        protected void Init()
        {
            isExists = true;
        }
        //destroy method to check isExists to false
        protected void Destroy()
        {
            isExists = false;
        }
        //checks if destroyed
        public bool isDestroyed()
        {
            return !this.isExists;
        }
        //returns type of the object
        public Type getType()
        {
            return type;
        }
        //required methods
        public abstract void reset();
        
        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch sb);
    }
}