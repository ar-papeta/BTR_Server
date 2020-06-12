using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
                    
                    stream = client.GetStream();
                      
                }
                


            }
            catch (Exception e) {
                Console.WriteLine(e.Message);           
            }

        }



        public byte[] TCPrequest(byte[] requestData, int dataLength, out int bytes)
        {
            bytes = 0;
            byte[] responseData = new byte[255];
            try
            {
                
                if (stream != null )
                {
                    stream.Write(requestData, 0, dataLength);
                    Thread.Sleep(10);
                    while (stream.DataAvailable)
                    {
                        bytes = stream.Read(responseData, 0, responseData.Length);
                    }
                    // пока данные есть в потоке
                    // Закрываем потоки
                    //stream.Close();
                    //client.Close();
                }

                return responseData;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                return responseData;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                return responseData;
            }

        }
    }
}
