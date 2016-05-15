using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace Hotspot
{
	public class MyApWifi
	{
		private string create(string str){
            Process process = new Process
            {
                StartInfo = { FileName = " cmd.exe ", UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, CreateNoWindow = true }
            };
            process.Start();
            process.StandardInput.WriteLine(str);
            process.StandardInput.WriteLine("exit");
            process.WaitForExit();
            string res = process.StandardOutput.ReadToEnd();
            process.Close();
            return res;
		}
		public void startApWifi(string wifiName,string psw)
		{
			string str="netsh wlan set hostednetwork mode=allow ssid="+wifiName+" key="+psw;
			Console.Out.Write( create(str));
            string str1 = "netsh wlan start hostednetwork";
            Console.Out.Write(create(str1));
        }
		public void closeApWifi(){
			string str = "netsh wlan stop hostednetwork"; 
			create(str);
		}
	}
}