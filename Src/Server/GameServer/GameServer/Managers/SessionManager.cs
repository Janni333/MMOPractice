using Common;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class SessionManager : Singleton<SessionManager>
    {
        public Dictionary<int, NetConnection<NetSession>> Sessions = new Dictionary<int, NetConnection<NetSession>>();

        public void AddSession(int chaid, NetConnection<NetSession> session)
        {
            this.Sessions[chaid] = session;
        }

        public void RemoveSession(int chaid)
        {
            this.Sessions.Remove(chaid);
        }

        public NetConnection<NetSession> GetSession(int chaid)
        {
            NetConnection<NetSession> session = null;
            this.Sessions.TryGetValue(chaid, out session);
            return session;
        }
    }
}
