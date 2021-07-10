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
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;

namespace Messenger
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //creates enum state for each certain state of the game
        enum SavingState
        {
            NotSaving,
            ReadyToSelectStorageDevice,
            SelectingStorageDevice,

            ReadyToOpenStorageContainer,    // once we have a storage device start here
            OpeningStorageContainer,
            ReadyToSave
        }

        SoundEffect startupMusic;
        //develops initial information and objects essential to the game
        public enum State {START, INTRO, LOAD, STAGEONE, STAGETWO, STAGETHREE, RESET, STOPSCREEN, FINISH}
        GraphicsDeviceManager graphics;
        private State state;
        SpriteBatch spriteBatch;
        StorageDevice device;
        Ship ship;
        Asteroid asteroid;
        //arraylist for all objects to be put into
        private ArrayList objects;
        //some add on material for the game aesthetics
        SpriteFont font;
        Texture2D Screen;
        Rectangle backgroundRect;
        int gameStart;
        int stageOneCounter;
        int attemptCounter;
        State currentStage;
        Enemies battle;
        //develops enemies
        Boss giganto;
        Drones drones;
        //information for ending
        Rectangle backgroundFinal;
        Texture2D backText;
        bool printFinal = false;
        int[] gameData;

        //advances game and creates storing material for loading previews
        StorageDevice storageDevice;
        SavingState savingState = SavingState.NotSaving;
        IAsyncResult asyncResult;
        StorageContainer storageContainer;
        //accesses oading files and text files to get previous data
        string filename = "savegame.sav";
        //Constructs data and saved material
        public struct SaveGameData
        {
            public int attempts;
            public int stage;
        }
        //constructs object for getting data on a new game
        SaveGameData saveGameData = new SaveGameData()
        {
            attempts = 1,
            stage = 1
        
        };
        private KeyboardState oldKB = Keyboard.GetState();
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            GameObjectAbstract.setGame(this);
            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //initializes ame data and information
            gameData = new int[2];

            // TODO: Add your initialization logic here
            //establishes an object array
            objects = new ArrayList();

            //adds all objects to the objects array using the addObjects method
            ship = new Ship();
            addObject(ship);
            asteroid = new Asteroid(ref ship);
            addObject(asteroid);
            backgroundRect = new Rectangle(0, 0, 900, 500);
            gameStart = 0;
            state = State.START;
            //changes preferred hieght and width
            graphics.PreferredBackBufferHeight = 500;
            graphics.PreferredBackBufferWidth = 900;
            graphics.ApplyChanges();
            stageOneCounter = 0;
            attemptCounter = 1;
            currentStage = State.STAGEONE;
            giganto = new Boss(ref ship);
            drones = new Drones(ref ship, ref giganto);
            backgroundFinal = new Rectangle(900, 0, 900, 500);
            gameData[0] = 1;
            gameData[1] = 1;
          
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            try
            {
                // Create a new SpriteBatch, which can be used to draw textures.
                //loads in all files using content loading
                spriteBatch = new SpriteBatch(GraphicsDevice);
                //startupMusic = Content.Load<SoundEffect>("asteroid plus loss");
                ship.texture = Content.Load<Texture2D>("ship");
                asteroid.texture = Content.Load<Texture2D>("asteroid");
                font = Content.Load<SpriteFont>("text");
                Screen = Content.Load<Texture2D>("HomeScreen");
                giganto.texture = Content.Load<Texture2D>("Boss");
                drones.texture = Content.Load<Texture2D>("Drone");
                backText = Content.Load<Texture2D>("background-2");
                ship.font = font;
                asteroid.powerup.texture = Content.Load<Texture2D>("blue");
                //calls new enemies class and loads the data from here for that class to access
                battle = new Enemies(ref ship, Content.Load<Texture2D>("Bullet"), Content.Load<Texture2D>("Jets"));
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            // TODO: use this.Content to load your game content here
        }
        
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            KeyboardState kb = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            try
            {
                switch (state) //alters game states
                {
                    case State.START:
                        //startupMusic.Play();
                        //checks if start or load were selected and outputs specific code for each instance
                        if (kb.IsKeyDown(Keys.Down))
                        {
                            gameStart = 1;
                        }
                        if (kb.IsKeyDown(Keys.Up))
                        {
                            gameStart = 0;
                        }
                        if (kb.IsKeyDown(Keys.Enter) && gameStart == 0)
                        {
                            state = State.INTRO;
                            //startupMusic.Pause();
                        }
                        //loads specific game data based on the previosu venture
                        if (kb.IsKeyDown(Keys.Enter) && gameStart == 1)
                        {
                            InitiateLoad();
                            LoadFromDevice(asyncResult);
                            attemptCounter = gameData[0];
                            if (gameData[1] == 1)
                            {
                                objects.Clear();
                                addObject(ship);
                                addObject(asteroid);
                                state = State.INTRO;
                                //startupMusic.Pause();
                            }
                            if (gameData[1] == 2)
                            {
                                state = State.STAGETWO;
                                objects.Clear();
                                addObject(ship);
                                addObject(battle);
                                //startupMusic.Pause();
                            }
                            if (gameData[1] == 3)
                            {
                                state = State.STAGETHREE;
                                objects.Clear();
                                addObject(drones);
                                addObject(giganto);
                                addObject(asteroid);
                                //startupMusic.Pause();
                            }
                        }
                        break;
                    case State.INTRO:
                        if(kb.IsKeyDown(Keys.Space))
                        {
                            state = State.STAGEONE;
                            objects.Clear();
                            addObject(asteroid);
                            addObject(ship);
                        }
                        break;
                    //establishes game one
                    case State.STAGEONE:
                        for (int i = 0; i < objects.Count; i++)
                        {
                            //calls on abstract class, updates and removes objects according to if they object is not necessary anymore
                            GameObjectAbstract obj = (GameObjectAbstract)objects[i];

                            obj.Update(gameTime);
                            if (obj.isDestroyed())
                            {
                                objects.Remove(obj);
                            }
                        }
                        //method for checking ship desturction
                        if (ship.isDestroyed())
                        {
                            state = State.RESET;
                        }
                        //what to complete if the asteroid is destoryed
                        //sets values back to default and goes into stage two
                        if (asteroid.complete)
                        {
                            objects.Clear();
                            ship.reset();
                            addObject(battle);
                            addObject(ship);
                            stageOneCounter = 0;
                            currentStage = State.STAGETWO;
                            state = State.STAGETWO;
                        }
                        break;
                    case State.STAGETWO:
                        //stage two is the bullets and fighter jets
                        for (int i = 0; i < objects.Count; i++)
                        {
                            GameObjectAbstract obj = (GameObjectAbstract)objects[i];

                            obj.Update(gameTime);
                            //if destroyed, will remove objects from array
                            if (obj.isDestroyed())
                            {
                                objects.Remove(obj);
                            }
                        }
                        //checks if ship is destroyed and starts reset enum
                        if (ship.isDestroyed())
                        {
                            state = State.RESET;
                        }
                        //moves onto stage three and sets up content for that
                        if (battle.complete)
                        {
                            objects.Clear();
                            ship.reset();
                            addObject(ship);
                            addObject(drones);
                            addObject(giganto);
                            stageOneCounter = 0;
                            currentStage = State.STAGETHREE;
                            state = State.STAGETHREE;
                        }
                        break;
                    //resets all to default values
                    case State.RESET:
                        objects.Clear();
                        ship.reset();
                        addObject(ship);
                        asteroid.reset();
                        battle.reset();
                        drones.reset();
                        giganto.reset();
                        //adds back objects to specfic stages
                        if (currentStage == State.STAGEONE)
                        {
                            addObject(asteroid);
                        }
                        if (currentStage == State.STAGETWO)
                        {
                            addObject(battle);
                        }
                        if (currentStage == State.STAGETHREE)
                        {
                            addObject(drones);
                            addObject(giganto);
                        }
                        stageOneCounter = 0;
                        attemptCounter++;

                        state = State.STOPSCREEN;
                        break;
                    case State.STAGETHREE:
                        //runs round three
                        for (int i = 0; i < objects.Count; i++)
                        {
                            GameObjectAbstract obj = (GameObjectAbstract)objects[i];
                            //removes if destroyed
                            obj.Update(gameTime);
                            if (obj.isDestroyed())
                            {
                                objects.Remove(obj);
                            }
                        }
                        //checks if destroyed and resets
                        if (ship.isDestroyed())
                        {
                            state = State.RESET;
                        }
                        //what to do after the giganto is dead. Will show background screen
                        if (giganto.dead && backgroundFinal.X > 50)
                            backgroundFinal.X -= 2;
                        if (giganto.dead && backgroundFinal.X <= 50)
                            printFinal = true;
                        break;
                    case State.STOPSCREEN:
                        //sets up start screen again and holds in data for specific saved files
                        if (kb.IsKeyDown(Keys.Escape))
                        {
                            this.Exit();
                        }
                        if (kb.IsKeyDown(Keys.S))
                        {
                            if (currentStage == State.STAGEONE)
                            {
                                gameData[0] = 1;
                            }
                            if (currentStage == State.STAGETWO)
                            {
                                gameData[0] = 2;
                            }
                            if (currentStage == State.STAGETHREE)
                            {
                                gameData[0] = 3;
                            }
                            gameData[1] = attemptCounter;
                            //constrcust saved data file
                            saveGameData = new SaveGameData()
                            {
                                attempts = attemptCounter,
                                stage = gameData[0]

                            };
                            //updates saved data files with new content
                            UpdateSaveKey(Keys.S, kb, oldKB);
                            UpdateSaving();


                        }
                        if (kb.IsKeyDown(Keys.R))
                        {
                            state = currentStage;
                        }
                        break;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            oldKB = kb;
            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        //method checks if files will be stoed and loaded to device constructor for file loading
        private void InitiateLoad()
        {
            
                device = null;
                StorageDevice.BeginShowSelector( this.LoadFromDevice, null);
                asyncResult = StorageDevice.BeginShowSelector(null, null);


        }
        //Gets previous load and establishes a container for new files
        void LoadFromDevice(IAsyncResult result)
        {
            device = StorageDevice.EndShowSelector(result);
            IAsyncResult r = device.BeginOpenContainer("Game1StorageContainer", null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = device.EndOpenContainer(r);
            result.AsyncWaitHandle.Close();
            //finds the container ofthe file
            if (container.FileExists(filename))
            {
                //opens up data and runs to see if it works
                Stream stream = container.OpenFile(filename, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                SaveGameData SaveData = (SaveGameData)serializer.Deserialize(stream);
                stream.Close();
                container.Dispose();
                //Update the game based on the save game file
                gameData[0] = SaveData.attempts;
                gameData[1] = SaveData.stage;
            }
        }
        //updates content based on user input whether he or she wants to save
        private void UpdateSaveKey(Keys saveKey, KeyboardState currentKeyboardState, KeyboardState oldKeyboardState)
        {
            if (!oldKeyboardState.IsKeyDown(saveKey) && currentKeyboardState.IsKeyDown(saveKey))
            {
                if (savingState == SavingState.NotSaving)
                {
                    savingState = SavingState.ReadyToOpenStorageContainer;
                }
            }
        }
        //update saving method for deneric purposes of saving content. This was rendered directly from XNA Support
        private void UpdateSaving()
        {
            switch (savingState)
            {
                //checks for each case and runs that specific section for saving content
                case SavingState.ReadyToSelectStorageDevice:
#if XBOX
                    if (!Guide.IsVisible)
#endif
                    {
                        asyncResult = StorageDevice.BeginShowSelector(null, null);
                        savingState = SavingState.SelectingStorageDevice;
                    }
                    break;

                case SavingState.SelectingStorageDevice:
                    if (asyncResult.IsCompleted)
                    {
                        storageDevice = StorageDevice.EndShowSelector(asyncResult);
                        savingState = SavingState.ReadyToOpenStorageContainer;
                    }
                    break;
                
                case SavingState.ReadyToOpenStorageContainer:
                    if (storageDevice == null || !storageDevice.IsConnected)
                    {
                        savingState = SavingState.ReadyToSelectStorageDevice;
                    }
                    else
                    {
                        asyncResult = storageDevice.BeginOpenContainer("Game1StorageContainer", null, null);
                        savingState = SavingState.OpeningStorageContainer;
                    }
                    break;

                case SavingState.OpeningStorageContainer:
                    if (asyncResult.IsCompleted)
                    {
                        storageContainer = storageDevice.EndOpenContainer(asyncResult);
                        savingState = SavingState.ReadyToSave;
                    }
                    break;

                case SavingState.ReadyToSave:
                    if (storageContainer == null)
                    {
                        savingState = SavingState.ReadyToOpenStorageContainer;
                    }
                    else
                    {
                        try
                        {
                            DeleteExisting();
                            Save();
                        }
                        catch (IOException e)
                        {
                            // Replace with in game dialog notifying user of error
                            Console.WriteLine("Console error: Data could not be loaded");
                        }
                        finally
                        {
                            storageContainer.Dispose();
                            storageContainer = null;
                            savingState = SavingState.NotSaving;
                        }
                    }
                    break;
            }
        }
        //deletes previous saved files to make room
        private void DeleteExisting()
        {
            if (storageContainer.FileExists(filename))
            {
                storageContainer.DeleteFile(filename);
            }
        }
        //saved proper data
        private void Save()
        {
            using (Stream stream = storageContainer.CreateFile(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                serializer.Serialize(stream, saveGameData);
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            //draws based on specifc state
            switch (state)
            {
                case State.START:
                    //start up screen. Shows itle and gives option to load new or old game
                    spriteBatch.Draw(Screen, backgroundRect, Color.WhiteSmoke);
                    spriteBatch.DrawString(font, "START NEW GAME", new Vector2(320, 300), Color.White);
                    spriteBatch.DrawString(font, "LOAD OLD GAME", new Vector2(330, 400), Color.White);
                    if (gameStart == 0)
                    {
                        spriteBatch.DrawString(font, "<", new Vector2(600, 300), Color.White);
                    }
                    if (gameStart == 1)
                    {
                        spriteBatch.DrawString(font, "<", new Vector2(600, 400), Color.White);
                    }
                    break;
                case State.INTRO:
                    spriteBatch.DrawString(font, "A war is ravaging between two planets and \nthe enemy is gaining the advantage.\nYou are a messenger, the last man who \ncan deliver an important message.\nIt is your friend's last dying wish \nto give a message \nto a fellow planet about imminent danger.\n\nMission:\nDodge enemies, asteroids, and bosses \nof the enemy to get the message to Planet Zen!\nUse left, right, up, and down arrow keys to move.\nClick SPACEBAR to start...", new Vector2(0, 0), Color.Yellow);
                    break;
                case State.STAGEONE:
                    stageOneCounter++;
                    //loads stage one and goes through objects array to draw each
                    foreach (GameObjectAbstract obj in objects)
                    {
                        obj.Draw(gameTime, spriteBatch);
                    }
                    //writes out how many ever attempts occurred
                    if (stageOneCounter < 120)
                    {
                        spriteBatch.DrawString(font, "Stage One", new Vector2(380, 200), Color.White);
                        spriteBatch.DrawString(font, "Attempt: " + attemptCounter, new Vector2(380, 300), Color.White);
                    }
                    break;
                case State.STOPSCREEN:
                    //when failure, draws out restart and save options
                    spriteBatch.DrawString(font, "Enter Escape to Exit the game\nEnter S to save the game\nEnter R to restart the current stage", new Vector2(100, 200), Color.White);
                    break;
                case State.STAGETWO:
                    stageOneCounter++;
                    //runs stage two objects
                    foreach (GameObjectAbstract obj in objects)
                    {
                        obj.Draw(gameTime, spriteBatch);
                    }
                    //writes out specific data and prints to screen for user
                    if (stageOneCounter < 120)
                    {
                        spriteBatch.DrawString(font, "Stage Two", new Vector2(380, 200), Color.White);
                        spriteBatch.DrawString(font, "Attempt: " + attemptCounter, new Vector2(380, 300), Color.White);
                    }
                    break;
                case State.STAGETHREE:
                    stageOneCounter++;
                    //runs stage three as well
                    //if giganti is not dead, objects continue to print and power level is reported
                    if (!giganto.dead)
                    {
                        foreach (GameObjectAbstract obj in objects)
                        {
                            obj.Draw(gameTime, spriteBatch);
                        }
                        spriteBatch.DrawString(font, "Power Level >> " + String.Concat(giganto.getPowerLevel()), new Vector2(550, 0), Color.Red);
                    }
                    //what to print when the mother ship is done. Exits into the background final scene
                    else
                    {
                        //lears objects and adds to arraylist
                        objects.Clear();
                        objects.Add(ship);
                        spriteBatch.Draw(backText, backgroundFinal, Color.White);
                    }
                    //final printing if completed game
                    if (printFinal)
                    {
                        spriteBatch.DrawString(font, "Congratulations!\nMessage DELIVERED!", new Vector2(50, 50), Color.White);
                    }
                    //prints out amt of attempts on stage three
                    if (stageOneCounter < 120)
                    {
                        spriteBatch.DrawString(font, "Stage Three", new Vector2(380, 200), Color.White);
                        spriteBatch.DrawString(font, "Attempt: " + attemptCounter, new Vector2(380, 300), Color.White);
                    }
                    break;

            }
            
            spriteBatch.End();
            base.Draw(gameTime);
        }
        //add object method for list
        public void addObject(GameObjectAbstract obj)
        {
            this.objects.Add(obj);
        }
        //gets specific object
        public ArrayList getObjects()
        {
            return this.objects;
        }

    }

    
}
    
