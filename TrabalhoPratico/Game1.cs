///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TrabalhoPratico;
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class Game1 : Game
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Variables
    GraphicsDeviceManager graphics;
    //Assinaturas dos dois objectos necessarios para o trabalho.
    Camera camera;
    HeightMap map;
    PlayerTank tank;
    OpponentTank tank2;
    //Variaveis que guardam os estados tanto do teclado como do rato.
    KeyboardState key;
    MouseState mouse;
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Game1
    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Initialize
    protected override void Initialize()
    {
        base.Initialize();
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region LoadContent
    protected override void LoadContent()
    {
        camera = new Camera(GraphicsDevice);
        map = new HeightMap(GraphicsDevice, Content, camera);
        tank = new PlayerTank(GraphicsDevice, Content, camera);
        tank2 = new OpponentTank(GraphicsDevice, Content, camera);
        //Colocar uma posição inicial pre-definida para o rato de modo a quando se inicia o jogo a camera fica centrada.
        Mouse.SetPosition(this.Window.Position.X / 100, this.Window.Position.Y / 2);
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region UnloadContent
    protected override void UnloadContent()
    {

    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Update
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        //A variavel key guarda quais as teclas que estão a ser pressionadas.
        key = Keyboard.GetState();
        mouse = Mouse.GetState();

        //Update da posiçâo da camera.
        camera.Update(GraphicsDevice, key, mouse, map, tank);
        tank.Update(gameTime, Keyboard.GetState(), map, tank2);
        tank2.Update(gameTime, Keyboard.GetState(), map, tank);
        //Para aumentar a "frame-rate" para 100 fps.
        TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 100.0f);
        //Para reduzir o tempo de "call" entre o draw e o update do game.
        this.IsFixedTimeStep = false;
        //O melhor é fazer o teste aqui no game1 e 
        //no Update faço o teste na poisição que vao estar e depois dou essa posiçao caso nao haja colisao
        //Portanto o update passa a receber um bool e secalhar fazer uma abstract para usar static metods
        bool col = tank.TankColision(tank, tank2);
        base.Update(gameTime);


    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Draw
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightSkyBlue);
        map.Draw(GraphicsDevice, camera);
        tank.Draw(GraphicsDevice, camera);
       // tank.DrawBala(GraphicsDevice, camera);
        tank2.Draw(GraphicsDevice, camera);
        base.Draw(gameTime);
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////