using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace TrabalhoPratico
{
    class PlayerTank : Tank, ICollidable
    {
        //VertexPositionColor[] bala;
        Projectil bala;
       
        public PlayerTank(GraphicsDevice graphics, ContentManager content, Camera cam)
            : base(graphics, content, cam)
        {
            bala = new Projectil();
            //bala = new VertexPositionColor[2];
            position = new Vector3(64.0f,0.0f, 64.0f);
            Center = position;
            //bala[0] = new VertexPositionColor(position, Color.Red);
            //bala[1]= new VertexPositionColor(position, Color.Red);
        }
        public override void Update(GameTime gametime, KeyboardState key, HeightMap map, Tank tank2)
        {
            //CONTROLS
            #region Controls TANK 1

            #region Cannon
            if (key.IsKeyDown(Keys.Left))
            {
                this.turretAngle += MathHelper.ToRadians(1.0f);
            }
            if (key.IsKeyDown(Keys.Right))
            {
                this.turretAngle -= MathHelper.ToRadians(1.0f);
            }
            if (key.IsKeyDown(Keys.Up))
            {
                this.cannonAngle += MathHelper.ToRadians(1.0f);
            }
            if (key.IsKeyDown(Keys.Down))
            {
                this.cannonAngle -= MathHelper.ToRadians(1.0f);
            }
            #endregion
            #region Tank

            if (!(key.IsKeyDown(Keys.W) || key.IsKeyDown(Keys.S)))
            {
                velocityForce = 0;
            }
            if (key.IsKeyDown(Keys.A))
            {
                this.steerRange += 0.14f;
                if (steerRange > 0.7f) steerRange = 0.7f;
            }
            if (key.IsKeyDown(Keys.A) && key.IsKeyDown(Keys.W)) this.yaw += MathHelper.ToRadians(2.0f);
            if (key.IsKeyDown(Keys.A) && key.IsKeyDown(Keys.S)) this.yaw -= MathHelper.ToRadians(2.0f);

            if (key.IsKeyDown(Keys.D))
            {
                this.steerRange -= 0.14f;
                if (steerRange < -0.7f) steerRange = -0.7f;
            }
            if (key.IsKeyDown(Keys.D) && key.IsKeyDown(Keys.W)) this.yaw -= MathHelper.ToRadians(2.0f);
            if (key.IsKeyDown(Keys.D) && key.IsKeyDown(Keys.S)) this.yaw += MathHelper.ToRadians(2.0f);

            if (key.IsKeyDown(Keys.W))
            {
                if (velocityForce > .1f)
                    velocityForce = 0.1f;
                else
                    velocityForce += 0.001f;
                this.position -= velocityForce * tankDir;
                this.wheelSpeed += 0.08f;
            }
            if (key.IsKeyDown(Keys.S))
            {
                velocityForce += 0.0001f;
                this.position -= velocityForce * -tankDir;
                //Same as position = positionAnterior - velocityForce * -tankDir
                //same as velocity = velocitySnterior - (escalar * direction)->velocity = direction * scalar
                // F = mA
                // A = F/mass
                this.wheelSpeed -= 0.08f;
            }

            if (steerRange > 0)
            {
                this.steerRange -= 0.07f;
            }
            else if (steerRange < 0)
            {
                this.steerRange += 0.07f;
            }
            if (steerRange == 0) steerRange = 0;
            #endregion

            if (key.IsKeyDown(Keys.Space))
            {
                Vector3 posicaoTank = this.position;
                Vector3 direcaoTank = this.tankDir;
                Debug.Print("direcaoTank->" + direcaoTank);
                bala.startFlight(posicaoTank, direcaoTank, 1f);
                //Debug.Print("beforeWhile"+bala[1].Position.Y.ToString());
                //while (bala[1].Position.Y >= -100)
                //{
                //    bala[1].Position += this.tankDir * 1f;
                //    bala[1].Position.Y += -tankNormal.Y * 0.1f;

                //    Debug.Print("balaPosition->" + bala[1].Position.ToString());
                //}
            }
            #endregion

            if (bala.isFlying)
                bala.UpdateFlight(gametime);


            //if (this.col)
            //    bounce(tank2);
            Debug.Print("bala->Position" + bala.actualPosition.ToString());
            base.Update(gametime, key, map, tank2);
        }

       
       
        //public override void Draw(GraphicsDevice graphics, Camera camera)
        //{
        //    VertexPositionColor[] balas = new VertexPositionColor[2];
        //   VertexPositionColor verticeBala = new VertexPositionColor(bala.actualPosition, Color.Black);
        //    VertexPositionColor verticeBalaBeginning = new VertexPositionColor(bala.prevPosiiton, Color.Black);
        //    balas[0] = verticeBalaBeginning;
        //    balas[1] = verticeBala;
        //    effect.CurrentTechnique.Passes[0].Apply();
        //    //Esta propiedade permite indicar à placa grafica o que fazer "render"->verices, quantos de fazer->3, em que indice comecar->0 e de que metodo faz render->LineList (Uma lista de linhas).
        //    graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, balas, 0, 1);
        //    base.Draw(graphics, camera);
        //}
        //public void DrawBala(GraphicsDevice graphics, Camera cam)
        //{
        //    base.Draw(graphics, cam);
        //    effect.View = cam.view;
        //    effect.CurrentTechnique.Passes[0].Apply();
        //    //Esta propiedade permite indicar à placa grafica o que fazer "render"->verices, quantos de fazer->3, em que indice comecar->0 e de que metodo faz render->LineList (Uma lista de linhas).
        //    graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, bala, 0, 1);
        //}
    }
}
