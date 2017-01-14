using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCallController
{
    class Message
    {
        public string srcUsername;
        public string destUsername;
        public bool callAccept;
        public bool callTeardown;
        public int callCapacity;
        public int requestId;
        public SNP SNPstart;
        public SNP SNPdest;
        public enum messageType
        {
            callRequest, callTeardown, callAccept, callCoordination, connectionRequest, callRelease
        };
        public messageType type = new messageType();

        public Message()
        {
            
        }
    }
}
