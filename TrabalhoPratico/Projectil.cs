using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoPratico
{
    class Projectil
    {
        private Vector3 velocity;
        public Vector3 prevPosiiton;
        public Vector3 actualPosition;

        public  bool isFlying = false;
        private float totalTime;

        public const float GRAVITY = 9.8f;

       public Projectil()
        {

        }

        public void startFlight(Vector3 startPosition, Vector3 diretion, float speed)
        {
            velocity = Vector3.Normalize(diretion) * speed;
            actualPosition = startPosition;
            isFlying = true;
        }

        public void UpdateFlight(GameTime gametime)
        {
            if (isFlying)
                FlyingPattern(gametime);
        }

        private void FlyingPattern(GameTime timePassed)
        {
            prevPosiiton = actualPosition;
            // accumulate overall time
            totalTime += (float)timePassed.ElapsedGameTime.Milliseconds / 4096.0f;
            // flight path where y-coordinate is additionally effected by gravity
            actualPosition = actualPosition + velocity * ((float)timePassed.ElapsedGameTime.Milliseconds / 90.0f);
            actualPosition.Y = actualPosition.Y - 0.5f * GRAVITY * (float)Math.Pow(totalTime, 2);
            Debug.Print("velocity->" + velocity.ToString());
            Debug.Print("totalTime->" + totalTime.ToString());
            Debug.Print("actualPosition->" + actualPosition.ToString());

        }
        #region Copied Orientation defining

        //public Matrix ConstructOrientationMatrix()
        //{
        //    Matrix orientation = new Matrix();
        //    // get orthogonal vectors dependent on the projectile's aim
        //    Vector3 forward = pos - prevPos;
        //    Vector3 right = Vector3.Cross(new Vector3(0, 1, 0), forward);
        //    Vector3 up = Vector3.Cross(right, forward);
        //    // normalize vectors, put them into 4x4 matrix for further transforms
        //    orientation.Right = Vector3.Normalize(right);
        //    orientation.Up = Vector3.Normalize(up);
        //    orientation.Forward = Vector3.Normalize(forward);
        //    orientation.M44 = 1;
        //    return orientation;

        #endregion
    }
}
