using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
namespace rudp
{


	/// <summary>
	/// Created by lenovo on 2016/4/6.
	/// </summary>
	public class NetJavaMsg
	{

		private int totalLen; //数据的长度
		private int id; //唯一的ID
		private byte[] data; //消息内容

		//本地参数，为简化起见，不发送
		private IPEndPoint recvRespAdd; //发送者接收应答的地址
        private IPEndPoint destAdd; //接收者地址

		private int sendCount = 0; //发送次数
		private long lastSendTime; //最后一次发送的时间

		/// 
		/// <param name="id">  //唯一的序号 </param>
		/// <param name="data">  //数据内容 </param>
		public NetJavaMsg(int id, byte[] data)
		{
			this.id = id;
			this.data = data;
			totalLen = 4 + 4 + data.Length;
		}
		public NetJavaMsg(int id, byte[] data, int offset, int length)
		{
			this.id = id;
			byte[] temp = new byte[length];
			for (int i = 0;i < length;i++)
			{
				temp[i] = data[offset + i];
			}
			this.data = temp;
			totalLen = 4 + 4 + length;
		}

		/// 
		/// <param name="udpData">  //将受到的udp数据解析为NetJavaMsg对象 </param>
		public NetJavaMsg(byte[] udpData)
		{
			try
			{
				this.totalLen = BitConverter.ToInt32(udpData,0);
                this.id = BitConverter.ToInt32(udpData, 4);

				this.data = new byte[totalLen - 4 - 4];
                for (int i = 0; i < totalLen - 4 - 4;i++ )
                {
                    data[i] = udpData[i + 8];
                }
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

		}


		public virtual byte[] toByte()
		{
			try
			{
                byte[] res = new byte[totalLen-4-4];
                byte[] tmpLen = BitConverter.GetBytes(totalLen);
                byte[] tmpId = BitConverter.GetBytes(id);

                for (int i = 0; i < totalLen; i++ )
                {
                    if (i < 4) res[i] = tmpLen[i];
                    else 
                        if (i < 8) res[i] = tmpId[i];
                        else
                        {
                            res[i] = data[i-8];
                        }
                }
                

				return res;

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			return null;
		}

		public override string ToString()
		{
			// TODO Auto-generated method stub
			return "id:" + id + "  content" + StringHelperClass.NewString(data) + "  totalLen" + totalLen + " sengerAdd:" + recvRespAdd + "  destAdd:" + destAdd;
		}


		public virtual int TotalLen
		{
			get
			{
				return totalLen;
			}
			set
			{
				this.totalLen = value;
			}
		}


		public virtual int Id
		{
			get
			{
				return id;
			}
			set
			{
				this.id = value;
			}
		}


		public virtual byte[] Data
		{
			get
			{
				return data;
			}
			set
			{
				this.data = value;
			}
		}


		public virtual IPEndPoint RecvRespAdd
		{
			get
			{
				return recvRespAdd;
			}
			set
			{
				this.recvRespAdd = value;
			}
		}


		public virtual IPEndPoint DestAdd
		{
			get
			{
				return destAdd;
			}
			set
			{
				this.destAdd = value;
			}
		}


		public virtual int SendCount
		{
			get
			{
				return sendCount;
			}
			set
			{
				this.sendCount = value;
			}
		}


		public virtual long LastSendTime
		{
			get
			{
				return lastSendTime;
			}
			set
			{
				this.lastSendTime = value;
			}
		}

	}


}