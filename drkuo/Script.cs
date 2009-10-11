using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace drkuo
{
    class Script
    {
        uonetwork uonet;
        public Script(uonetwork muonet)
        {
            uonet = muonet;
        }

        public void main()
        {
            uoobject tempob = (uoobject)uonet.GameObjects[3];
            uonet.display(tempob.x + " !!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }


    }
}
