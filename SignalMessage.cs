using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCallController
{
    public enum messageType
    {
        callRequest, callTeardown, callAccept, callCoordination, callConfirmation, callRelease, connectionRequest, peerCoordination, tableQuery, connectionConfirmation
    };
    public enum componentType
    {
        CPCC, NCC, CC, RC, LRM
    };
    public class SignalMessage
    {
        public string srcUsername;
        public string destUsername;
        public bool callAccept;
        public bool callTeardown;
        public int callCapacity;
        public int requestId;
        public List<SNP> SNPs;
        public List<SNPP> SNPPs;


        public messageType msgType;
        public componentType sourceType;
        public int sourceId;
        public bool connect;


        public SignalMessage()
        {

            msgType = new messageType();
            sourceType = new componentType();
            SNPPs = new List<SNPP>();
            SNPs = new List<SNP>();
        }
    }
}
