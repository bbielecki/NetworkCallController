using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NetworkCallController
{
    class NCCstart
    {
        int NCCid;
        int neighbourNCCport;
        int outPort;
        string[] clientId;
        int listenerPort;
        string ipAddress;
        

        UdpClient inputSocket;
        bool listenerOn;
        public NCC ncc;
        Socket outputSocket;

        public NCCstart(int listenerPort, string[] clientId, int NCCid, int neighbourNCCport)
        {
            ncc = new NCC();
            this.listenerPort = listenerPort;
            this.NCCid = NCCid;
            this.clientId = clientId;
            this.neighbourNCCport = neighbourNCCport;
            this.ipAddress = "10.78.16.243";

            inputSocket = new UdpClient(listenerPort);
            listenerOn = true;

            Thread receiver = new Thread(getData);
            receiver.Start();

            //TEST
           // test();
            
        }

        private void test()
        {
            SignalMessage sigm = new SignalMessage();
            sigm.srcUsername = "Abacki";
            sigm.destUsername = "Abacki";
            ncc.ConnectionRequestOut(sigm);
        }

        private void getData()
        {
            while (listenerOn)
            {

                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                
                byte[] receivedBytes = inputSocket.Receive(ref remoteIpEndPoint);
                string data = Encoding.ASCII.GetString(receivedBytes);

                SignalMessage message = JsonConvert.DeserializeObject<SignalMessage>(data);
                Console.WriteLine(message);
                CallParameters callParameters = new CallParameters();
                
                
                switch (message.msgType)
                {
                    case messageType.callAccept:

                        if (message.callAccept)
                        {
                            if (ncc.compareDomains(message.srcUsername))
                            {
                                ncc.ConnectionRequestOut(message);
                            }
                            else
                            {                            
                                ncc.CallConfirmed(message);
                            }
                            
                        }
                        else
                        {
                            message.msgType = messageType.callAccept;
                            message.sourceType = componentType.NCC;
                            message.callAccept = false;
                            
                            ncc.CallRequestAccept(message);
                        }

                        break;
                    case messageType.callRequest:


                        if (ncc.CallRequestAccept(message))
                            Console.WriteLine("wyslano do klienta");
                        else
                        {
                            Console.WriteLine("Odmowino wykonania takiego polaczenia");                          
                        }
                       
                        break;
                    case messageType.callTeardown:

                        ncc.CallReleaseOut(message);

                        break;

                    case messageType.callCoordination:
                        
                        ncc.NetworkCallCoordinationIn(message);

                        break;

                    case messageType.connectionConfirmation:

                        if (message.connect)
                        {
                            ncc.CallConfirmed(message);

                        }
                        else
                        {                          
                            ncc.CallReleaseOut(message);
                        }
                        break;

                    case messageType.callConfirmation:

                        ncc.ConnectionRequestOut(message);

                        break;

                    case messageType.callRelease:

                        ncc.CallReleaseIn(message);

                        break;
                    default:
                        Console.WriteLine("missing mesage type");
                        break;
                }
            }
        }


        private void sendSigPackage(SignalMessage sp)
        {
            try
            {
                IPEndPoint pointToSend = new IPEndPoint(IPAddress.Parse(ipAddress), outPort); //TODO trzeba ustalic porty z batcha
                String toSend = JsonConvert.SerializeObject(sp);
                byte[] send_buffer = Encoding.ASCII.GetBytes(toSend);

                try
                {
                    outputSocket.SendTo(send_buffer, pointToSend);
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
