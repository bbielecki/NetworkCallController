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
        int listenerPort;
        int NCCid;
        UdpClient inputSocket;
        bool listenerOn;
        NCC ncc;
        int outPort; //z batcha
        Socket outputSocket;

        public NCCstart(int listenerPort, int NCCid)
        {
            ncc = new NCC();
            this.listenerPort = listenerPort;
            this.NCCid = NCCid;

            inputSocket = new UdpClient(listenerPort);
            listenerOn = true;
            Thread receiver = new Thread(getData);
            receiver.Start();
        }

        private void getData()
        {
            while (listenerOn)
            {

                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                
                byte[] receivedBytes = inputSocket.Receive(ref remoteIpEndPoint);
                
                string data = Encoding.ASCII.GetString(receivedBytes);
                Message message = JsonConvert.DeserializeObject<Message>(data);
                CallParameters callParameters = new CallParameters();
                
                switch (message.type)
                {
                    case Message.messageType.callAccept:

                        if (message.callAccept)
                        {

                        }

                        break;
                    case Message.messageType.callRequest:

                        if(ncc.CallRequestAccept(message))
                            Console.WriteLine("wysłano prosbe o polaczenie do NCC");
                        else
                        {
                            message.type = Message.messageType.callAccept;
                            message.callAccept = false;
                            sendSigPackage(message);
                        }
                       
                        break;
                    case Message.messageType.callTeardown:

                        break;

                    case Message.messageType.callCoordination:

                        message.type = Message.messageType.callAccept;
                        sendSigPackage(message);

                        break;
                    default:
                        Console.WriteLine("missing mesage type");
                        break;
                }
            }
        }


        private void sendSigPackage(Message sp)
        {
            try
            {
                IPEndPoint pointToSend = new IPEndPoint(IPAddress.Parse("127.0.0.1"), outPort); //TODO trzeba ustalic porty z batcha
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
