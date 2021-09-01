///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class Camera
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Variables
    //Efeito que terá o encargo de albergar os fundamentos que permitem construir um programa a três dimensões.
    public BasicEffect effect;

    //Vectores 3D de orientação e posicionamento da camera.
    public Vector3 posicao;
    Vector3 direcaoFrontal = Vector3.Zero;
    Vector3 direcaoLateral = Vector3.Zero;

    //Variaveis que guardam os valores de regulação de rotação e inclinaçao da camera.
    float yaw = 0.0f;
    float pitch = -4.5f;
    float pitchControl = -4.5f;
    float lastPitch = 0.0f;
    //Matrix que transformam o vector direcão da camera para haver uma rotação da mesma.
    Matrix rotacao;
    //Matrix que caracteriza espacialmente a camera.
    public Matrix view;
    //Matrix que caracteriza as "features", como o seu angulo de abertura, ratio, e o "view frustum" (volume que será renderizado) da camera.
    public Matrix projection;

    //Variaveis que guardam a posição do rato.
    float posicaoXrato;
    float posicaoYrato;

    //Variavel que guarda a posição da altura da camera de acordo com a alturas dos vertices que constituem o mapa.
    float altura;
    float velocidade;

    //Bools para a camera
    bool aFloat = true;
    bool surfaceFollows = false;
    bool modelFollows = false;
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Constructor
    public Camera(GraphicsDevice device)
    {
        effect = new BasicEffect(device);

        // Calcula o aspectRatio.
        float aspectRatio = (float)(device.Viewport.Width / device.Viewport.Height);

        //Posição inicial da camera.
        posicao = new Vector3(34.0f, 10.0f, 65.0f);

        //A camera fica na posição anteriormente indicada, com direcção inicial zero, e orientação vertical.
        view = Matrix.CreateLookAt(posicao, posicao + direcaoFrontal, Vector3.Up);

        //O angulo de abertura é de 45º, com o aspect ratio calculado anteriormente, e o "near plane" esta a 1 valor de
        //distancia da origem da camera e o far plane a 1000 de distancia.
        projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 1000.0f);

        //Apliacar as matrix anteriormente criadas ao effect.
        effect.View = view;
        effect.Projection = projection;
        velocidade = 0.5f;

        //Inicia a posição do rato no centro do ecrã.
        Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Update
    public virtual void Update(GraphicsDevice device, KeyboardState key, MouseState mouse, HeightMap mapa, Tank tank)
    {
        #region Camera Modes
        if (key.IsKeyDown(Keys.F1)) // FOLLOW TERRAIN
        {
            aFloat = true;
            surfaceFollows = false;
            modelFollows = false;
        }
        if (key.IsKeyDown(Keys.F2)) // FREE CAMERA
        {
            surfaceFollows = true;
            modelFollows = false;
            aFloat = false;
        }
        #endregion
        #region Por implementar (Camera que segue o tank)
        if (key.IsKeyDown(Keys.F3))
        {
            modelFollows = true;
            aFloat = false;
            surfaceFollows = false;

        }
        #endregion
        //Guarda a posição inicial em que é colocado o rato.
        Vector2 posicaoI = new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2);

        //Para guardar a distancia entre a posição inicial e actual do rato.
        posicaoXrato = (mouse.Position.X - posicaoI.X);
        posicaoYrato = (mouse.Position.Y - posicaoI.Y);

        //Para se obter um moviemento do yaw(esquerda/direita) por parte do rato.
        yaw -= MathHelper.ToRadians((int)(posicaoXrato * 0.7f));

        //Para se obter um movimento, por parte do rato, do pitch (cima/baixo).         
        pitchControl += MathHelper.ToRadians((int)(posicaoYrato * 0.7f));
        //Bounds para a camera
        if (pitchControl < -8.5f )
        {
            pitch = -8.4f ;
           // Debug.Print("pitchBound" + pitch.ToString());
        }
        else if(pitchControl > -5.5f)
        {
            pitch = -5.6f;
           // Debug.Print("pitchBound" + pitch.ToString());
        }
        else
        {
            pitch = pitchControl;
           // Debug.Print("pitch" + pitch.ToString());
        }
        //if ((surfaceFollows == true || aFloat == true) && modelFollows == false)
        //{
        #region Controls
        //Movimento transversal da camera pelos três eixos ortogonais.
        //Esquerda 
        if (key.IsKeyDown(Keys.NumPad4))
            {
                posicao -= velocidade * direcaoLateral;
            }
            //Direita 
            if (key.IsKeyDown(Keys.NumPad6))
            {
                posicao -= velocidade * -direcaoLateral;
            }
            //Frente
            if (key.IsKeyDown(Keys.NumPad8))
            {
                posicao += velocidade * direcaoFrontal;
            }
            //Tras
            if (key.IsKeyDown(Keys.NumPad5))
            {
                posicao += velocidade * -direcaoFrontal;
            }
        
       
        #endregion



       
            //Aplicação do yaw e pitch para formar uma matrix de rotação.
            rotacao = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            //Alteração da direção da camera pela matrix de rotação anteriormente criada.
            direcaoFrontal = Vector3.Transform(Vector3.Forward, rotacao);
            direcaoLateral = Vector3.Transform(Vector3.Right, rotacao);
        


        //Alteração da posição Y da camera com o calculo da altura anteriormente feito.
        if (surfaceFollows == true && aFloat == false && modelFollows == false)
        {
            altura = mapa.CalculateInterpolation(posicao.X, posicao.Z) + 2.0f;
            posicao.Y = altura;
            view = Matrix.CreateLookAt(posicao, (posicao + direcaoFrontal), Vector3.Cross(direcaoLateral, direcaoFrontal));
        }
        if (aFloat == true && modelFollows == false && surfaceFollows == false)
        {
            if (key.IsKeyDown(Keys.NumPad7))
                posicao.Y += 1.0f;
            else if (key.IsKeyDown(Keys.NumPad1))
                posicao.Y -= 1.0f;
            view = Matrix.CreateLookAt(posicao, (posicao + direcaoFrontal), Vector3.Cross(direcaoLateral, direcaoFrontal));
        }
        if (modelFollows == true && surfaceFollows == false && aFloat == false)
        {
            altura = mapa.CalculateInterpolation(posicao.X, posicao.Z)+tank.Position.Y ;
            posicao.Y = altura;
            view = Matrix.CreateLookAt(new Vector3(tank.position.X, tank.position.Y + posicao.Y * 0.9f, tank.position.Z) + tank.tankDir * 10, new Vector3(tank.position.X, tank.position.Y + 3f, tank.position.Z) - tank.tankDir, Vector3.Up);
        }
        
            //Update da view e assim consequentemente da camera.
            
        lastPitch = pitch;
        //faz o reset da posição para o centro do ecra.
        Mouse.SetPosition((int)posicaoI.X, (int)posicaoI.Y);
        
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////