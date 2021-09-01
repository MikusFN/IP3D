///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using TrabalhoPratico;
using System;
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class Tank : ICollidable
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Variables
    public Matrix world;
    public Matrix view;
    public Matrix projection;
    public BasicEffect effect;
    public Model myModel;
    public ModelBone cannonBone;
    public ModelBone turretBone;
    public ModelBone rightFrontWheelBone;
    public ModelBone leftFrontWheelBone;
    public ModelBone rightBackWheelBone;
    public ModelBone leftBackWheelBone;
    public ModelBone rightSteerBone;
    public ModelBone leftSteerBone;
    public Matrix cannonTransform;
    public Matrix turretTransform;
    public Matrix rightFrontWheelTransform;
    public Matrix leftFrontWheelTransform;
    public Matrix rightBackWheelTransform;
    public Matrix leftBackWheelTransform;
    public Matrix rightSteerTransform;
    public Matrix leftSteerTransform;
    public Matrix[] boneTransforms;
    public Matrix rotation;
    public Vector3 direction;
    public Matrix translacao;
    public Vector3 tankNormal;
    public Vector3 tankRight;
    public Vector3 tankDir;
    public float scale;
    public float turretAngle = 0.0f;
    public float cannonAngle = 0.0f;
    public float yaw = 0.0f;
    public float wheelSpeed = 0.0f;
    public float steerRange = 0.0f;
    public float acelaration = 1.1f;
    public Vector3 position;
    public Vector3 lastPosition;
    public Vector3 Position { get { return position; } }
    float totalTime = 0.0f;
    public float Radius
    { get; set; } = 1f;
    protected bool col = false;
    public Vector3 Center
    { get; set; }
    
  protected  float velocityForce=0.0f;
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Constructor
    public Tank(GraphicsDevice graphics, ContentManager content, Camera cam)
    {
        effect = new BasicEffect(graphics);
        

        myModel = content.Load<Model>("tank");
        myModel.Root.Transform = Matrix.CreateTranslation(position);
        //<<BONES>>
        turretBone = myModel.Bones["turret_geo"];                 //TURRET
        cannonBone = myModel.Bones["canon_geo"];                  //CANNON
        rightFrontWheelBone = myModel.Bones["r_front_wheel_geo"]; //FRONT RIGHT
        leftFrontWheelBone = myModel.Bones["l_front_wheel_geo"]; //FRONT LEFT
        rightBackWheelBone = myModel.Bones["r_back_wheel_geo"]; //BACK RIGHT
        leftBackWheelBone = myModel.Bones["l_back_wheel_geo"]; //BACK LEFT
        rightSteerBone = myModel.Bones["r_steer_geo"];           //RIGHT STEER
        leftSteerBone = myModel.Bones["l_steer_geo"];           //LEFT STEER
        //<<TRANSFORM>>
        turretTransform = turretBone.Transform;                   //TURRET
        cannonTransform = cannonBone.Transform;                  //CANNON
        rightFrontWheelTransform = rightFrontWheelBone.Transform; //FRONT RIGHT
        leftFrontWheelTransform = leftFrontWheelBone.Transform;  //FRONT LEFT
        rightBackWheelTransform = rightBackWheelBone.Transform;  //BACK RIGHT
        leftBackWheelTransform = leftBackWheelBone.Transform;   //BACK LEFT
        rightSteerTransform = rightSteerBone.Transform;           //RIGHT STEER
        leftSteerTransform = leftSteerBone.Transform;            //LEFT STEER
        boneTransforms = new Matrix[myModel.Bones.Count];
        //<POSIÇÃO INICIAL TANK>
        Radius = 1f;
        //<TAMANHO DO TANK>
        scale = 0.003f;
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region CalculateInclination
    public Vector3 CalculateInclination(HeightMap map, float cameraX, float cameraZ)
    {

        VertexPositionNormalTexture vertex1, vertex2, vertex3, vertex4;
        vertex1 = map.vertices[(int)(cameraZ) * map.mWidth + (int)(cameraX)];
        vertex2 = map.vertices[(int)(cameraZ) * map.mWidth + (int)(cameraX + 1)];
        vertex3 = map.vertices[(int)(cameraZ + 1) * map.mWidth + (int)(cameraX)];
        vertex4 = map.vertices[(int)(cameraZ + 1) * map.mWidth + (int)(cameraX + 1)];
        float peso1, peso2;
        Vector3 inter1, inter2, interFinal;
        //INTERPOLAÇÃO 1
        peso1 = cameraX - vertex1.Position.X;
        peso2 = 1 - peso1;
        inter1 = (vertex1.Normal) * peso2 + (vertex2.Normal) * peso1;
        //INTERPOLAÇÃO 2
        inter2 = (vertex3.Normal) * peso2 + (vertex4.Normal) * peso1;
        peso1 = cameraZ - vertex1.Position.Z;
        peso2 = 1 - peso1;
        //INTERPOLAÇÃO FINAL
        interFinal = (inter1) * peso2 + (inter2) * peso1;
       // Debug.Write(interFinal + "\n");
        return interFinal;
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Update
    public virtual void Update(GameTime gametime, KeyboardState key, HeightMap map, Tank tank2)
    {
       col = TankColision(this, tank2);
       
        rotation = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);
        direction = Vector3.Transform(Vector3.UnitX, rotation);
        if (col == false)
        {
            position.Y = map.CalculateInterpolation(position.X, position.Z);
            velocityForceCalcule();
            translacao = Matrix.CreateTranslation(position);
        } else
        {
            //bounce(position - lastPosition, Vector3.Negate(direction));
            position = lastPosition;// +tank2.tankDir*-velocityForce;
            //if(ground==true)
            position.Y = map.CalculateInterpolation(position.X, position.Z);

            translacao = Matrix.CreateTranslation(position);
        }
       // if(ground ==true)
        tankNormal = Vector3.Normalize(CalculateInclination(map, position.X, position.Z));
        tankRight = Vector3.Normalize(Vector3.Cross(direction, tankNormal));
        tankDir = Vector3.Normalize(Vector3.Cross(tankNormal, tankRight));

        rotation = Matrix.Identity;
        rotation.Up = tankNormal;
        rotation.Right = tankRight;
        rotation.Forward = tankDir;
       

        //TANK
        myModel.Root.Transform = Matrix.CreateScale(scale) * rotation * translacao;
        //TURRET
        turretBone.Transform = Matrix.CreateRotationY(turretAngle) * turretTransform;
        //CANNON
        cannonBone.Transform = Matrix.CreateRotationX(cannonAngle) * cannonTransform;
        //WHEELS
        rightFrontWheelBone.Transform = Matrix.CreateRotationX(wheelSpeed) * rightFrontWheelTransform;
        leftFrontWheelBone.Transform = Matrix.CreateRotationX(wheelSpeed) * leftFrontWheelTransform;
        rightBackWheelBone.Transform = Matrix.CreateRotationX(wheelSpeed) * rightBackWheelTransform;
        leftBackWheelBone.Transform = Matrix.CreateRotationX(wheelSpeed) * leftBackWheelTransform;
        //STEER
        rightSteerBone.Transform = Matrix.CreateRotationY(steerRange) * rightSteerTransform;
        leftSteerBone.Transform = Matrix.CreateRotationY(steerRange) * leftSteerTransform;


        myModel.CopyAbsoluteBoneTransformsTo(boneTransforms);
        //if (this.position.Y > 2f)
        //{
        //    ground = false;
        //    gravityPull(gametime);
        //}
        lastPosition = Position;
        Debug.Print(this.position.ToString());
    }
    #endregion
    public bool TankColision(Tank tank)
    {
        float distancia = Vector3.Distance(this.Position, tank.position);
        if (distancia < this.Radius + tank.Radius)
        {
            return true;
        }
        else return false;
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Draw
    public virtual void Draw(GraphicsDevice graphics, Camera camera)
    {
        foreach (ModelMesh mesh in myModel.Meshes)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.World = boneTransforms[mesh.ParentBone.Index];
                effect.View = camera.view;
                effect.Projection = camera.projection;
               
            }
            mesh.Draw();
            
        }
    }

    public  bool TankColision(Tank tank1, Tank tank2)
    {
        float distance = Vector3.Distance(tank1.Position, tank2.position);
        if (distance < tank1.Radius + tank2.Radius)
            return true;
        else return false;
    }

    private void gravityPull(GameTime timePassed)
    {
        Vector3 velocity = Vector3.Normalize(tankDir) * velocityForce;



        Vector3 prevPosiiton = this.position;
        // accumulate overall time
        totalTime += (float)timePassed.ElapsedGameTime.Milliseconds / 4096.0f;
        // flight path where y-coordinate is additionally effected by gravity
        position = position + velocity * ((float)timePassed.ElapsedGameTime.Milliseconds / 90.0f);
        position.Y = position.Y - 0.5f * 9.8f * (float)Math.Pow(totalTime, 2);
    }
    #endregion
    public void bounce(Tank tank)
    {

        Vector3 velocityOpp = Vector3.Normalize(tank.tankDir) * tank.velocityForce;
        //Vector3 myVelocity= Vector3.Normalize(tankDir) * velocityForce;
        this.position += tank.tankDir * velocityOpp;
        ////reflect the incoming projectile and normalize it so it's "just" a direction
        //Vector3 direction = Vector3.Normalize(reflectionAxis * incomingDirection);
        //velocityForce -= 0.001f; // reduces the speed so the arche becomes lower
        //velocity = velocityForce * direction; // the new velocity vector
        //totalTime = 0; // gravity starts all over again
        ////if (speed <= 0) bmoving = false; // no speed no movement
    }
    //Veos valores do cos para ver se quiandpo desce esta a aumentar senao é perciso por clausulas.
    public void velocityForceCalcule()
    {
      float dot= Vector3.Dot(Vector3.Up, Vector3.Normalize(this.tankNormal));
        velocityForce*= (float)Math.Cos( dot)*10;  
        
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////