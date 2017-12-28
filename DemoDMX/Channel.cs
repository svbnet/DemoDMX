using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theia;

namespace DemoDMX
{
    public class Channel
    {
        private readonly DmxController controller;
        private byte value;
        private readonly int address;

        public Channel(DmxController controller, int relativeChannel, int address)
        {
            this.controller = controller;
            this.address = address;
            RelativeChannel = relativeChannel;
            value = controller.GetOffsetChannelValue(address, relativeChannel);
        }

        public int RelativeChannel { get; }

        public byte Value
        {
            get { return value; }
            set
            {
                this.value = value;
                controller.SetOffsetChannelValue(address, RelativeChannel, value);
            }
        }
    }
}
