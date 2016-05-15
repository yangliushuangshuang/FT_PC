using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using util;
namespace rudp
{

	/// <summary>
	/// Created by lenovo on 2016/4/6.
	/// </summary>

	/// <summary>
	/// 数据报发送
	/// 1.发送消息线程负责发送，发送后将消息放入容器中等待应答
	/// 2.接收线程接收应答，从容器中匹配后删除
	/// 3.重发线程负责重发，未收到应答的消息，发送3次后移除
	/// @author Administrator
	/// 
	/// </summary>
	public class DatagramSend
	{

		private IPEndPoint localAddr; //本地要发送的地址对象
		private UdpClient dSender; //发送的Socket对象
		private IPEndPoint destAddr; //目标地址
		private string file;
		//本地缓存已发送的消息Map  key为消息ID  value为消息对象本身
		internal IDictionary<int?, NetJavaMsg> msgQueue = new ConcurrentDictionary<int?,NetJavaMsg>();


		public DatagramSend(string file, string localIp, string rmIp, int localPort, int rmPort)
		{
            localAddr = new IPEndPoint(IPAddress.Parse(localIp), localPort);
			dSender = new UdpClient(localAddr);
            destAddr = new IPEndPoint(IPAddress.Parse(rmIp), rmPort);
			this.file = file;
			//启动三个线程
			startSendThread();
			startRecvResponseThread();
			startReSendThread();


		}


		//启动发送线程
		public virtual void startSendThread()
		{
			new Thread(() =>
			{

				try
				{
					send();

				}
				catch (Exception e)
				{
					// TODO Auto-generated catch block
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

			}).Start();

		}


		//模拟发送消息
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void send() throws Exception
		public virtual void send()
		{
			Console.WriteLine("发送端-发送数据线程启动...");
			int id = 0;
			System.IO.FileStream @in = new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			while (true)
			{
				id++;
				byte[] msgData = new byte[Util.BLOCK_SIZE];
				int len;
				if ((len = @in.Read(msgData,0,msgData.Length)) == -1)
				{
					break;
				}
				//创建要发送的消息对象
				NetJavaMsg sendMsg;
				if (len != Util.BLOCK_SIZE)
				{
					sendMsg = new NetJavaMsg(id,msgData,0,len);
				}
				else
				{
					sendMsg = new NetJavaMsg(id,msgData);
				}


				//要发送的数据：将要发送的数据转为字节数组
				byte[] buffer = sendMsg.toByte();

				//创建数据包，指定内容，指定目标地址
				dSender.Send(buffer,buffer.Length,destAddr); //发送

				sendMsg.SendCount = 1;
				sendMsg.LastSendTime = DateTimeHelperClass.CurrentUnixTimeMillis();
				sendMsg.RecvRespAdd = localAddr;
				sendMsg.DestAdd = destAddr;

				msgQueue[id] = sendMsg;
				Console.WriteLine("客户端-数据已发送" + sendMsg);
				Util.sendIndex = id;
				//Thread.sleep(1000);
			}
		}


		//启动接收应答线程
		public virtual void startRecvResponseThread()
		{
			new Thread(() =>
			{

				try
				{
					recvResponse();

				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

			}).Start();
		}


		//接收应答消息
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void recvResponse() throws Exception
		public virtual void recvResponse()
		{
			Console.WriteLine("接收端-接收应答线程启动...");
			while (true)
			{
                IPEndPoint listen = new IPEndPoint(IPAddress.Any,localAddr.Port);
                byte[] recvData = dSender.Receive(ref listen);

				//创建接收数据包对象

				
				NetJavaRespMsg resp = new NetJavaRespMsg(recvData);

				int respID = resp.RepId;
				NetJavaMsg msg = msgQueue[new int?(respID)];

				if (msg != null)
				{
					Console.WriteLine("接收端-已收到：" + msg);
					msgQueue.Remove(respID);
				}

			}
		}



		//启动重发线程
		public virtual void startReSendThread()
		{
			new Thread(() =>
			{

				try
				{
					while (true)
					{
						resendMsg();
						Thread.Sleep(1000);
					}

				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

			}).Start();
		}


		//判断Map中的消息，如果超过3秒未收到应答，则重发
		public virtual void resendMsg()
		{
			ICollection<int?> keyset = msgQueue.Keys;
			IEnumerator<int?> it = keyset.GetEnumerator();
			while (it.MoveNext())
			{
				int? key = it.Current;
				NetJavaMsg msg = msgQueue[key];

				if (msg.SendCount > 3)
				{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
					it.Dispose();
					Console.WriteLine("***发送端---检测到丢失的消息" + msg);
				}

				long cTime = DateTimeHelperClass.CurrentUnixTimeMillis();
				if ((cTime - msg.LastSendTime) > 3000 && msg.SendCount < 3)
				{
					byte[] buffer = msg.toByte();
					try
					{
						dSender.Send(buffer,buffer.Length,msg.DestAdd);
						msg.SendCount = msg.SendCount + 1;
						Console.WriteLine("客户端--重发消息:" + msg);

					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);

					}
				}
			}
		}
	}

}