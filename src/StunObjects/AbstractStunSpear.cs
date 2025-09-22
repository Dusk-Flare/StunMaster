using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StunMaster.StunObjects
{
    internal class AbstractStunSpear : AbstractSpear
    {
        public AbstractStunSpear(World world, WorldCoordinate pos, EntityID ID) : base(world, null, pos, ID, explosive: false)
        {

        }
        public override void Realize()
        {
            if (realizedObject == null)
            {
                realizedObject = new StunSpear(this, world);
            }
        }
    }
}