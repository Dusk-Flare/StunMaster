using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StunMaster.StunCompat
{
    public class ReturnData
    {
        public bool LegacyPowers;
    }
    public class OnlineData : RainMeadow.OnlineResource.ResourceData
    {
        public bool Legacy;

        public OnlineData()
        {
            Legacy = Plugin.OIinstance.LPC.Value;
        }

        public override ResourceDataState MakeState(RainMeadow.OnlineResource resource)
        {
            return new OnlineDataState(this);
        }

        public class OnlineDataState : ResourceDataState
        {
            [OnlineField]
            public bool Legacy;

            public OnlineDataState() : base() { }
            public OnlineDataState(OnlineData data) : base()
            {
                Legacy = data.Legacy;
            }

            public override void ReadTo(RainMeadow.OnlineResource.ResourceData data, RainMeadow.OnlineResource resource)
            {
                if (data is OnlineData onlineData)
                {
                    onlineData.Legacy = Legacy;
                }
            }
            public override Type GetDataType() => typeof(OnlineData);
        }
    }

}


