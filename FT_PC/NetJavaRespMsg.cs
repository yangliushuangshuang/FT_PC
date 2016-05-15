using System;

namespace rudp
{


	/// <summary>
	/// Created by lenovo on 2016/4/6.
	/// </summary>
	public class NetJavaRespMsg
	{



		private int totalLen;

		private int repId; //回复对应接收到的消息ID

		private byte state = 0; //状态 0：正确接收          其它：错误

		private long resTime; //应答方的发送时间



		public NetJavaRespMsg(int repId, byte state, long resTime)
		{

			this.repId = repId;

			this.state = state;

			this.resTime = resTime;

			totalLen = 4 + 4 + 1 + 8;



		}



		public NetJavaRespMsg(byte[] udpData)
		{

			try
			{
                this.totalLen = BitConverter.ToInt32(udpData, 0);
                this.repId = BitConverter.ToInt32(udpData, 4);
                this.state = udpData[8];
                this.resTime = BitConverter.ToInt64(udpData,9);






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
                byte[] tmpId = BitConverter.GetBytes(repId);
                byte[] tmpTime = BitConverter.GetBytes(resTime);
                for (int i = 0; i < totalLen; i++ )
                {
                    if (i < 4) res[i] = tmpLen[i];
                    else
                        if (i < 8) res[i] = tmpId[i];
                        else
                            if (i == 8) res[i] = state;
                            else
                                res[i] = tmpTime[i];
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

			return "totalLen:" + this.totalLen + "  respID" + this.repId + "  state" + this.state + "  resTime" + resTime;



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


		public virtual int RepId
		{
			get
			{
    
				return repId;
    
			}
			set
			{
    
				this.repId = value;
    
			}
		}


		public virtual byte State
		{
			get
			{
    
				return state;
    
			}
			set
			{
    
				this.state = value;
    
			}
		}


		public virtual long ResTime
		{
			get
			{
    
				return resTime;
    
			}
			set
			{
    
				this.resTime = value;
    
			}
		}

	}
}