using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
namespace rudp
{
    class InetUdp
    {
        private int rmPort;
        private int localPort;
        private string rmAddr;
        private string localAddr;
        private const string HAND_SHAKE = "hello";
        public long length;
        public string fileName;
        private UdpClient udpClient;
        public InetUdp()
        {
            localAddr = util.Util.GetLocalIP();
            localPort = 9888;
            udpClient = new UdpClient();
        }
        public void disconnect()
        {
            udpClient.Close();
        }
        public bool connect()
        {
            
            IPEndPoint listen = new IPEndPoint(IPAddress.Any,localPort);
            byte[] recvData = udpClient.Receive(ref listen);
            String res = BitConverter.ToString(recvData, 0, recvData.Length);
            rmAddr = listen.Address.ToString();
            rmPort = listen.Port;
            if (res.Split(new string[] { util.Util.SPLITER }, StringSplitOptions.RemoveEmptyEntries)[0].Equals(HAND_SHAKE)) return true;
            return false;
        }
        public bool transFile(string file)
        {
            if (!File.Exists(file)) return false;
            if (!Directory.Exists(file)) return false;  
            FileInfo info = new FileInfo(file);
            if (!util.Util.sendInfo(rmAddr, rmPort, info.FullName + util.Util.SPLITER + info.Length)) return false;
            try
            {
                new DatagramSend(file, localAddr, rmAddr, localPort, rmPort);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool rcvFile()
        {
            TcpListener listener = new TcpListener(IPAddress.Any,localPort);
            string[] info = util.Util.rcvInfo(listener).Split(new string[]{util.Util.SPLITER},StringSplitOptions.RemoveEmptyEntries);
            if (info.Length<=0) return false;
            length = long.Parse(info[1]);
            fileName = info[0];
            string filePath = util.Util.RCV_DIR+"\\"+fileName;
            
            try
            {
                new DatagramRecive(filePath, localAddr, localPort);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public Bitmap getQRCode()
            //TODO
        {
            string content = localAddr + util.Util.SPLITER + localPort;
            //返回二维码
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            Bitmap qrCode = qrCodeEncoder.Encode(content);

            return qrCode;
        }
    }
}
