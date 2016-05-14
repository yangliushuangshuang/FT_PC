using System;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;
using rudp;
using util;
namespace rudp
{

	/// <summary>
	/// Created by lenovo on 2016/4/6.
	/// </summary>
	public class DatagramRecive
	{

		private IPEndPoint localAddr;
		private UdpClient dSender;
		private string file;
       
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DatagramRecive(java.io.File file,String localIp,int port) throws Exception
		public DatagramRecive(string file, string localIp, int port)
		{
            localAddr = new IPEndPoint(IPAddress.Parse(localIp), port);
            dSender = new UdpClient(port);
			this.file = file;   
			//启动接收线程
			startRecvThread();
		}


		public virtual void startRecvThread()
		{
			Thread thread = new Thread(() =>
			{

				try
				{
					recvMsg();

				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);

				}

			});
            thread.Start();
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void recvMsg() throws Exception
		public virtual void recvMsg()
		{
			Console.WriteLine("接收线程启动");
			while (true)
			{
                IPEndPoint eplisten = new IPEndPoint(IPAddress.Any,localAddr.Port);
                byte[] recvData = dSender.Receive(ref eplisten);
                FileStream @out = new FileStream(file, FileMode.Create);
				NetJavaMsg recvMsg = new NetJavaMsg(recvData);
				@out.Write(recvMsg.Data, 0, recvMsg.Data.Length);
				NetJavaRespMsg resp = new NetJavaRespMsg(recvMsg.Id,(byte)0,DateTimeHelperClass.CurrentUnixTimeMillis());
				Util.rcvIndex = recvMsg.Id;
				byte[] data = resp.toByte();
				dSender.Send(data,data.Length,eplisten);

				Console.WriteLine("接收端-已发送应答" + resp);
			}
		}
	}

}