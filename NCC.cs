using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCallController
{
    class NCC
    {
        Policy policy;
        CallParameters callParameters;
        Socket outputSocket;
        int outPort;
        Directory directory;

        public NCC()
        {
            policy = new Policy();
            directory = new Directory();
            callParameters = new CallParameters();
        }

        #region output interfaces
        
        public void CallIndication(string UNIidentyfier)
        {

        }

        public void ConnectionRequestOut(Message message)
        {
            message.type = Message.messageType.connectionRequest;
        }

        public void NetworkCallCoordinationOut(Message message)
        {
            message.type = Message.messageType.callCoordination;
            sendNetworkCoordination(message);
        }

        public void DirectoryRequest(string UNIidentyfier)
        {

        }

        public bool PolicyOut(CallParameters callParameters)
        {
            return policy.checkPolicy(callParameters);
        }

        public void CallReleaseOut(string UNIidentyfier)
        {

        }
        #endregion

        #region input Interfaces
        public bool CallRequestAccept(Message message)
        {
            callParameters.UserName = message.srcUsername;
            callParameters.UserCallCapacity = message.callCapacity;

            if (PolicyOut(callParameters))
            {
                message.SNPstart = directory.translateUsernameToAddress(message.srcUsername);

                NetworkCallCoordinationOut(message);
                return true;
            }
            else
            {
                return false;          
            }
        }

        public void NetworkCallCoordinationIn(string UNIidentifier)
        {

        }

        public void CallReleaseIn(string UNIidentifier)
        {

        }

        #endregion


        public bool sendNetworkCoordination(Message message)
        {
            try
            {
                IPEndPoint pointToSend = new IPEndPoint(IPAddress.Parse("127.0.0.1"), outPort); //TODO trzeba ustalic porty z batcha
                String toSend = JsonConvert.SerializeObject(message);
                byte[] send_buffer = Encoding.ASCII.GetBytes(toSend);
                
                try
                {
                    outputSocket.SendTo(send_buffer, pointToSend);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }
    }
}
