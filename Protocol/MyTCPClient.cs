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
        StringBuilder response = new StringBuilder();
        public MyTCPClient(String server, int port)
        {
            this.server = server;
            this.port = port;
        }

        public void TCPrequest(byte[] requestData, int dataLength, out byte[] responceData)
        {
            responceData = null;
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(server, port);
                NetworkStream stream = client.GetStream();


                stream.Write(requestData, 0, requestData.Length);
                do
                {
                    int bytes = stream.Read(responceData, 0, responceData.Length);
                    response.Append(Encoding.UTF8.GetString(responceData, 0, bytes));
                }
                while (stream.DataAvailable); // пока данные есть в потоке
                                              // Закрываем потоки
                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

        }
    }
}
