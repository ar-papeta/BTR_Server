using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BTR_Server.Protocol
{
    public class MyTCPClient
    {

        private String server;
        private int port;
        //byte[] requestData = { 0x7e, 0x1, 0x2 };
        //byte[] responceData = new byte[255];
        
        public MyTCPClient(String server, int port)
        {
            this.server = server;
            this.port = port;
        }

        public byte[] TCPrequest(byte[] requestData, int dataLength)
        {
            byte[] responceData = new byte[255];
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(server, port);
                NetworkStream stream = client.GetStream();


                stream.Write(requestData, 0, dataLength);
                while (stream.DataAvailable)
                {
                    int bytes = stream.Read(responceData, 0, responceData.Length);
                }
                 // пока данные есть в потоке
                                              // Закрываем потоки
                stream.Close();
                client.Close();
                return responceData;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                return responceData;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                return responceData;
            }

        }
    }
}
