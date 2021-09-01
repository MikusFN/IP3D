using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrabalhoPratico
{
    class OpponentTank : Tank,ICollidable
    {
        public OpponentTank(GraphicsDevice graphics, ContentManager content, Camera cam)
            : base(graphics, content, cam)
        {

            position = new Vector3(60.0f, 0.0f, 60.0f);
            Center = position;
        }

        public override void Update(GameTime gametime, KeyboardState key, HeightMap map, Tank tank2)
        {
            base.Update(gametime, key, map, tank2);
            #region Controls TANK 2
            #region Cannon
            if (key.IsKeyDown(Keys.Z))
            {
                this.turretAngle += MathHelper.ToRadians(1.0f);
            }
            if (key.IsKeyDown(Keys.X))
            {
                this.turretAngle -= MathHelper.ToRadians(1.0f);
            }
            if (key.IsKeyDown(Keys.Q))
            {
                this.cannonAngle += MathHelper.ToRadians(1.0f);
            }
            if (key.IsKeyDown(Keys.E))
            {
                this.cannonAngle -= MathHelper.ToRadians(1.0f);
            }
            #endregion
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
            if (key.IsKeyDown(Keys.J))
            {
                this.yaw += MathHelper.ToRadians(1.0f);
                this.steerRange += 0.07f;
                if (steerRange > 0.7f) steerRange = 0.7f;
            }
            if (key.IsKeyDown(Keys.L))
            {
                this.yaw -= MathHelper.ToRadians(1.0f);
                this.steerRange -= 0.07f;
                if (steerRange < -0.7f) steerRange = -0.7f;
            }
            if (key.IsKeyDown(Keys.I))
            {
                this.position -= 0.1f * tankDir;
                this.wheelSpeed += 0.08f;
            }
            if (key.IsKeyDown(Keys.K))
            {
                this.position -= 0.1f * -tankDir;
                this.wheelSpeed -= 0.08f;
            }
            if (key.IsKeyUp(Keys.A) && key.IsKeyUp(Keys.D)) steerRange = 0;
        }
        #endregion

    }
}

