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
        int NCCid;
        int neighbourNCCport;
        public int outPort;
        string[] clientId;
        string ipAddress = "10.78.17.48";
        string waiting = "waiting";
        string active = "active";
        Dictionary<int, string> status;

        Policy policy;
        CallParameters callParameters;
        public Socket outputSocket;
        Directory directory;

        public NCC(string[] clientId, int NCCid, int neighbourNCCport)
        {
            policy = new Policy();
            directory = new Directory();
            callParameters = new CallParameters();
            status = new Dictionary<int, string>();

            this.NCCid = NCCid;
            this.neighbourNCCport = neighbourNCCport;
            this.clientId = clientId;
            outPort = 10002;
            outputSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public NCC()
        {
            policy = new Policy();
            directory = new Directory();
            callParameters = new CallParameters();
            outputSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            status = new Dictionary<int, string>();

            outPort = 10002;
        }

        #region output interfaces
        
        public void CallIndication(SignalMessage message)
        {
            Console.WriteLine("wysylam prosbe o polaczenie do urzytkownika");
            outPort = 10100 + directory.translateUsernameToAddress(message.destUsername);
            message.msgType = messageType.callAccept;
            message.sourceType = componentType.NCC;

            sendSigPackage(message, ipAddress);
        }

        public void ConnectionRequestOut(SignalMessage message)
        {
            Console.WriteLine("wysylam prosbe o zestawienie polaczenia");
            message.msgType = messageType.connectionRequest;
            message.sourceType = componentType.NCC;

            outPort = 11100 + NCCid; //id CC zaleznego od NCC sie pokrywa
            sendSigPackage(message, ipAddress);

        }

        public void NetworkCallCoordinationOut(SignalMessage message)
        {
            Console.WriteLine("wysylam call coordination do NCC");
            message.msgType = messageType.callCoordination;
            message.sourceType = componentType.NCC;
            
            sendSigPackage(message, ipAddress);
        }

        public int[] DirectoryRequest(string UNIidentyfier)
        {
            Console.WriteLine("sprawdzam dane urzytkownika ");
            int[] userIDandUserDomain = new int[2];
            userIDandUserDomain[0] = directory.translateUsernameToAddress(UNIidentyfier);
            userIDandUserDomain[1] = directory.checkUserDomain(UNIidentyfier);
            return userIDandUserDomain;
        }

        public bool PolicyOut(CallParameters callParameters)
        {
            return policy.checkPolicy(callParameters);
        }

        public void CallReleaseOut(SignalMessage message)
        {

            if (message.sourceType == componentType.CC)
            {
                Console.WriteLine("nie udalo sie zestawic polączenia");
                status.Remove(message.requestId);
                SendCallReleaseToBothEnds(message);
            }
            if (message.sourceType == componentType.NCC)
            {
                if (compareDomains(message.srcUsername))
                {
                    if (status[message.requestId] == active)
                    {
                        Console.WriteLine("uzytkownik z naszej domeny zerwal polaczenie");
                        message.msgType = messageType.callTeardown;
                        message.sourceType = componentType.NCC;
                        outPort = 11100 + NCCid; //id CC zaleznego od NCC sie pokrywa

                        sendSigPackage(message, ipAddress);

                        outPort = 10100 + message.sourceId;
                        sendSigPackage(message, ipAddress);
                    }
                    status.Remove(message.requestId);
                }
                else
                {
                    Console.WriteLine("uzytkownik z innej domeny zerwal polaczenie");
                    message.sourceType = componentType.NCC;

                    message.msgType = messageType.callTeardown;
                    outPort = 10100 + directory.translateUsernameToAddress(message.destUsername);

                    sendSigPackage(message, ipAddress);
                }
            }

            if (message.sourceType == componentType.CPCC)
            {
                if (compareDomains(message.srcUsername))
                {
                    Console.WriteLine("uzytkownik konczy polaczenie wewnatrzdomenowe");
                    message.msgType = messageType.callTeardown;
                    outPort = 11100 + NCCid; //id CC zaleznego od NCC sie pokrywa

                    sendSigPackage(message, ipAddress);
                }
                else
                {
                    Console.WriteLine("uzytkownik konczy polaczenie miedzydomenowe");

                    message.msgType = messageType.callTeardown;
                    outPort = outPort = outPort = 50000 + directory.checkUserDomain(message.srcUsername);

                    sendSigPackage(message, ipAddress);
                }
            }
                
        }

        private void SendCallReleaseToBothEnds(SignalMessage message)
        {
            message.msgType = messageType.callRelease;
            message.sourceType = componentType.NCC;
            message.callTeardown = true;
            int userDomain = directory.checkUserDomain(message.srcUsername);
            int destDomain = directory.checkUserDomain(message.destUsername);
            if (compareDomains(message.srcUsername))
            {
                outPort = 50000 + destDomain;
                sendSigPackage(message, ipAddress);
                outPort = 10100 + directory.translateUsernameToAddress(message.srcUsername);
                sendSigPackage(message, ipAddress);
            }

        }

        public void ConnectionRealese(SignalMessage message)
        {
            message.msgType = messageType.callRelease;

            outPort = 11100 + NCCid; //id CC zaleznego od NCC sie pokrywa
            sendSigPackage(message, ipAddress);
        }
        #endregion

        #region input Interfaces
        public bool CallRequestAccept(SignalMessage message)
        {
            int[] userInformations = DirectoryRequest(message.destUsername);

            callParameters.UserName = message.srcUsername;
            callParameters.UserCallCapacity = message.callCapacity;
            if (message.callAccept)
            {
               
                if (PolicyOut(callParameters))
                {
                    Console.WriteLine("przyznano prawo do polaczenia");
                    status.Add(message.requestId, waiting);

                    if (directory.checkUserDomain(message.destUsername) == directory.checkUserDomain(message.srcUsername))
                    {
                        Console.WriteLine("polaczenie wewnatrzdomenowe");
                        SendCallIndicatioToUserInTheSameDomain(message, userInformations);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("polaczenie miedzydomenowe");
                        SendNetworkCallCoordination(message, userInformations);
                        return true;
                    }


                }
                else
                {
                    Console.WriteLine("nie przyznano prawa do polaczneia");
                    SendRejectToSourceClient(message);
                    return false;
                }
            }
            else
            {
                Console.WriteLine("brak zgody usera na polaczenie");
                if (compareDomains(message.srcUsername))
                {
                    status.Remove(message.requestId);
                    SendRejectToSourceClient(message);
                    return true;
                }
                else
                {
                    CallReleaseOut(message);
                    return true;
                }
                
            }

        }

        private void SendNetworkCallCoordination(SignalMessage message, int[] userInformations)
        {
            message.SNPs.Add(directory.getUserSNP(message.srcUsername));
            message.SNPs.Add(directory.getUserSNP(message.destUsername));

            outPort = 50000 + userInformations[1];
            //message.connect = false;
            NetworkCallCoordinationOut(message);
        }

        private void SendCallIndicatioToUserInTheSameDomain(SignalMessage message, int[] userInformations)
        {
            outPort = 10100 + userInformations[0];
            //message.connect = true;

            message.SNPs.Add(directory.getUserSNP(message.srcUsername));
            message.SNPs.Add(directory.getUserSNP(message.destUsername));

            CallIndication(message);
        }

        private void SendRejectToSourceClient(SignalMessage message)
        {
            outPort = 10100 + directory.translateUsernameToAddress(message.srcUsername);
            message.msgType = messageType.callRequest;
            message.callAccept = false;

            sendSigPackage(message, ipAddress);
        }

        public void NetworkCallCoordinationIn(SignalMessage message)
        {
            CallParameters cp = new CallParameters();
            cp.userDomain = directory.checkUserDomain(message.srcUsername);
            cp.UserCallCapacity = message.callCapacity;

            if (policy.checkForeignPolicy(cp))
            {
                CallIndication(message);
            }
            else
            {
                message.msgType = messageType.callRelease;
                message.sourceType = componentType.NCC;
                message.connect = false;
                message.callAccept = false;

                CallReleaseOut(message);
            }
        }


        public void CallReleaseIn(SignalMessage message)
        {
            CallReleaseOut(message);
        }

        public void CallConfirmed(SignalMessage message)
        {
            int userDomain = directory.checkUserDomain(message.srcUsername);
            if (message.sourceType==componentType.NCC)
            {
                Console.WriteLine("wysylam potwierdzenie odebrania polaczenia");
                message.msgType = messageType.callRequest;
                message.sourceType = componentType.NCC;
                message.connect = true;

                outPort = 10100 + directory.translateUsernameToAddress(message.srcUsername);
              
            }
            else if (message.sourceType == componentType.CC)
            {
                Console.WriteLine("udane zestawienie polaczenia");
                PrepareCallConfirmationForSourceClient(message);
            }
            else if(message.sourceType == componentType.CPCC)
            {
                Console.WriteLine("uzytkownik odebral polaczenie");
                PrepareCallConfirmationFromNCC(message, userDomain);
            }

            sendSigPackage(message, ipAddress);
        }

        private void PrepareCallConfirmationFromNCC(SignalMessage message, int userDomain)
        {
            message.msgType = messageType.callConfirmation;
            message.sourceType = componentType.NCC;

            outPort = 50000 + userDomain;
        }

        private void PrepareCallConfirmationForSourceClient(SignalMessage message)
        {
            message.msgType = messageType.callRequest;
            message.sourceType = componentType.NCC;
            message.connect = true;

            outPort = 10100 + directory.translateUsernameToAddress(message.srcUsername);

            status[message.requestId] = active;
        }

        #endregion

        public bool compareDomains(string name)
        {
            return NCCid == directory.checkUserDomain(name);
        }


        public void sendSigPackage(SignalMessage sp, string ipAddress)
        {
            Console.WriteLine(ipAddress+ "  " + outPort);
            try
            {
                IPEndPoint pointToSend = new IPEndPoint(IPAddress.Parse(ipAddress), outPort); //TODO trzeba ustalic porty z batcha
                String toSend = JsonConvert.SerializeObject(sp);
                byte[] send_buffer = Encoding.ASCII.GetBytes(toSend);

                try
                {
                    outputSocket.SendTo(send_buffer, pointToSend);
                    Console.WriteLine("wysylam");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
