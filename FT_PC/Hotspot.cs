using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Drawing;
using ThoughtWorks.QRCode.Codec;
namespace Hotspot
{
    public class Hotspot
    {
        private string wifiName;    
        private string psw;
        private TcpListener listener;
        private int port;
        private const string HAND_SHAKE = "handshake";
        private string rmIp;
        private int rmPort;
        public Hotspot(string wifi,string psw)
        {
            this.wifiName = wifi;
            this.psw = psw;
            port = 9888;
        }
        public Bitmap getQRCode(string wifiName,string psw)
            //TODO
        {
            MyApWifi apWifi = new MyApWifi();
            apWifi.startApWifi(wifiName,psw);
            //返回二维码
            String info = wifiName + psw;
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 6;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            Bitmap qrCode = qrCodeEncoder.Encode(info);

            return qrCode;
        }
        public bool connect()
        {
            listener = new TcpListener(IPAddress.Any,port);
            string handShake = util.Util.rcvInfo(listener);
            string[] info = handShake.Split(new string[]{util.Util.SPLITER},StringSplitOptions.RemoveEmptyEntries);
            rmIp = info[2];
            rmPort = int.Parse(info[1]);
            return handShake != null && handShake.LastIndexOf(HAND_SHAKE)!=-1;
        }
        
        public bool transFile(string file)
        {
            try
            {
                if (!File.Exists(file)) return false;
                FileInfo fileInfo = new FileInfo(file);
                string info = fileInfo.FullName + fileInfo.Length;
                if (util.Util.sendInfo(rmIp, rmPort, info)) return false;
                StreamReader sr = new StreamReader(File.OpenRead(file), Encoding.UTF8);
                TcpClient client = new TcpClient();
                StreamWriter sw = new StreamWriter(client.GetStream());
                return util.Util.copyFile(sr, sw, false);
            }
            catch
            {
                return false;
            }
        }
        public bool rcvFile()
        {
            string[] info = util.Util.rcvInfo(listener).Split(new string[]{util.Util.SPLITER},StringSplitOptions.RemoveEmptyEntries);
            if (info == null) return false;
            string fileName = info[0];
            long length = long.Parse(info[1]);
            TcpClient client = listener.AcceptTcpClient();
            StreamReader sr = new StreamReader(client.GetStream());
            Directory.CreateDirectory(util.Util.RCV_DIR);
            using (File.Create(util.Util.RCV_DIR + "\\" + fileName)) ; 
            StreamWriter sw = new StreamWriter(util.Util.RCV_DIR+"\\"+fileName,false,Encoding.UTF8);
            return util.Util.copyFile(sr, sw, true);
        }
        public void disconnect()
        {
            if (listener != null) listener.Stop();
            new MyApWifi().closeApWifi();
        }
    }
}
