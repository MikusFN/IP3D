using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoPratico
{
    interface ICollidable//<T>
    {
         float Radius { get; set; }
         Vector3 Center { get; set; }

        //Secalhar fazer uma classe absttrata ou typeless
        //ref T asset, ref T asset2
         bool TankColision( Tank tank2);
       //bool BulletColision(Tank tank1, Bullet bullet);
    }
}
