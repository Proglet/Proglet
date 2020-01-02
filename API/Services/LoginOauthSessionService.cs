using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class LoginOauthSessionService
    {
        private Dictionary<Guid, string> sessions = new Dictionary<Guid, string>();
        public Guid NewSession()
        {
            Guid newId = Guid.NewGuid();
            while (sessions.ContainsKey(newId))
                newId = Guid.NewGuid();
            sessions[newId] = "";
            return newId;
        }

        public string this[Guid id] {
            get {
                return sessions[id];
            }
            set {
                sessions[id] = value;
            }
        }

        internal bool hasSession(Guid sessionId)
        {
            return sessions.ContainsKey(sessionId);
        }
    }
}
