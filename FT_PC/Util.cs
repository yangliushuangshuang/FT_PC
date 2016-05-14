using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;
using System.Text.RegularExpressions;
namespace util
{
	public static class Util
	{
		public const int HELLOSHAKE_SIZE = 256;
		public const int BLOCK_SIZE = 1024*10;
        public const int HEAD_LEN = 8;
        public const string SPLITER = "/_/";
        public const string RCV_DIR = System.Environment.CurrentDirectory+"\\"+"rcvFiles";
		public static int sendIndex
		{
			get
			{
				return sendIndex;
			}
			set
			{
				sendIndex=value;
			}
		}
		public static int rcvIndex
		{
			get
			{
				return rcvIndex;
			}
			set
			{
				rcvIndex = value;
			}
		}
        public static bool sendInfo(string ip ,int port,string info)
        {
            TcpClient client = new TcpClient();
            client.Connect(ip, port);
            if (!client.Connected) return false;
            NetworkStream streamToServer = client.GetStream();
            byte[] buffer = Encoding.Unicode.GetBytes(info);

            try
            {
                lock (streamToServer)
                {
                    streamToServer.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                return false;
            }
            streamToServer.Dispose();
            client.Close();
            return true;
        }
        public static string rcvInfo(TcpListener listener)
        {
            StringBuilder sb = new StringBuilder() ;
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream streamToClient = client.GetStream();
            byte[] buf = new byte[BLOCK_SIZE];
            try
            {
                lock (streamToClient)
                {
                    int len = streamToClient.Read(buf,0,buf.Length);
                    sb.Append(BitConverter.ToString(buf,0,len));
                }
                sb.Append(Util.SPLITER+client.Client.RemoteEndPoint.ToString());
            }
            catch
            {
                return null;
            }
            streamToClient.Dispose();
            return sb.ToString();
        }
        public static bool copyFile(StreamReader @in ,StreamWriter @out , bool isIn)
        {
            char[] buf = new char[BLOCK_SIZE+HEAD_LEN];
            int len;
            try {
                sendIndex = 0;
                rcvIndex = 0;
                while ((len = @in.Read(buf, HEAD_LEN, BLOCK_SIZE))>0)
                {
                    int offset=HEAD_LEN;
                    if(isIn){
                        byte[] a = new byte[HEAD_LEN];
                        for(int i=0;i<HEAD_LEN;i++)a[i]=(byte)buf[i];
                        rcvIndex = (int)bytes2long(a);      
                    }
                    else {
                        byte[] a = long2bytes(++sendIndex);
                        for(int i=0;i<a.Length;i++)buf[i] = (char)a[i];
                    }
                    @out.Write(buf, offset, len);
                    @out.Flush();
                }
                @out.Close();
                @in.Close();
            } catch {
                return false;
            }
            return true;
        }
        public static byte[] long2bytes(long num) {
            byte[] b = new byte[HEAD_LEN];
            for (int i=0;i<HEAD_LEN;i++) {
                b[i] = (byte)(num/(int)Math.Pow(2,56-(i*HEAD_LEN)));
            }
            return b;
        }
        public static long bytes2long(byte[] b)
        {
            long temp = 0;
            long res = 0;
            for (int i = 0; i < HEAD_LEN; i++)
            {
                res <<= HEAD_LEN;
                temp = b[i] & 0xff;
                res |= temp;
            }
            return res;
        }


        /// <summary>
        /// �õ�����IP
        /// </summary>
        public static string GetLocalIP()
        {
            //����IP��ַ
            string strLocalIP = "";
            //�õ��������
            string strPcName = Dns.GetHostName();
            //�õ�����IP��ַ����
            IPHostEntry ipEntry = Dns.GetHostEntry(strPcName);
            //��������
            foreach (var IPadd in ipEntry.AddressList)
            {
                //�жϵ�ǰ�ַ����Ƿ�Ϊ��ȷIP��ַ
                if (IsRightIP(IPadd.ToString()))
                {
                    //�õ�����IP��ַ
                    strLocalIP = IPadd.ToString();
                    //����ѭ��
                    break;
                }
            }
            //���ر���IP��ַ
            return strLocalIP;
        }

        

        /// <summary>
        /// �ж��Ƿ�Ϊ��ȷ��IP��ַ
        /// </summary>
        /// <param name="strIPadd">��Ҫ�жϵ��ַ���</param>
        /// <returns>true = �� false = ��</returns>
        private static bool IsRightIP(string strIPadd)
        {
            //����������ʽ�ж��ַ����Ƿ����IPv4��ʽ
            if (Regex.IsMatch(strIPadd, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
            {
                //����С����ֲ��ַ���
                string[] ips = strIPadd.Split('.');
                if (ips.Length == 4 || ips.Length == 6)
                {
                    //�������IPv4����
                    if (System.Int32.Parse(ips[0]) < 256 && System.Int32.Parse(ips[1]) < 256 & System.Int32.Parse(ips[2]) < 256 & System.Int32.Parse(ips[3]) < 256)
                        //��ȷ
                        return true;
                    //���������
                    else
                        //����
                        return false;
                }
                else
                    //����
                    return false;
            }
            else
                //����
                return false;
        }
	}
}