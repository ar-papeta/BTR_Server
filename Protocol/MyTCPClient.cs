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
        public static TcpClient client = null;
        public NetworkStream stream;
        public MyTCPClient(String server, int port)
        {
            
            try
            {
                if(client == null)
                {
                    client = new TcpClient();
                    client.Connect(server, port);
                    //client.ConnectAsync(server, port);
                    stream = client.GetStream();

                }
                


            }
            catch (Exception e) {
                Console.WriteLine(e.Message);           
            }

        }

        public bool IsTCPInit(NetworkStream _stream)
        {
            return (_stream != null) ? true : false;
        }

        public byte[] TCPrequest(byte[] requestData, int dataLength, out int bytes)
        {
            bytes = 0;
            byte[] responceData = new byte[255];
            try
            {
                
                if (stream != null )
                {
                    stream.Write(requestData, 0, dataLength);

                    while (stream.DataAvailable)
                    {
                        bytes = stream.Read(responceData, 0, responceData.Length);
                    }
                    // пока данные есть в потоке
                    // Закрываем потоки
                    //stream.Close();
                    //client.Close();
                }

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
