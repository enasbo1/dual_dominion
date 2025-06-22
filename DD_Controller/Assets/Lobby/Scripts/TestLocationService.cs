using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class TestLocationService : MonoBehaviour
{
    public string localIp;
    public string globalIp;

    private void Start()
    {
        localIp = GetLocalIPAddress();
        globalIp = GetGlobalIPAddress();
    }
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    
    public static string GetGlobalIPAddress(){
        HttpWebRequest myExtIPWWW = WebRequest.CreateHttp("http://checkip.dyndns.org");

        Stream steam = myExtIPWWW.GetResponse().GetResponseStream();
        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

        // Pipe the stream to a higher level stream reader with the required encoding format. 
        StreamReader readStream = new StreamReader(steam, encode);
        
        string myExtIP = readStream.ReadToEnd();
        Debug.Log(myExtIP);
        myExtIP=myExtIP.Substring(myExtIP.IndexOf(":", StringComparison.Ordinal)+2);
        myExtIP=myExtIP.Substring(0,myExtIP.IndexOf("<", StringComparison.Ordinal));
        
        return myExtIP;
    }

}