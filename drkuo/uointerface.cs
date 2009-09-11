using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace drkuo
{
    interface uointerface
    {
        void error(Exception e);
        void processChatPacket(String msg);
        void processDisconnect();
        void processServerList(String[] list);
        void processCharList(String[] list);
        void processLoggedIn();
        void processUpdatePlayer();
    }
}
