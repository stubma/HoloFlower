using UnityEngine;
using System;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using HoloToolkit.Unity;

#if !UNITY_EDITOR
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Storage.Streams;
using System.Net.Http;
using System.Threading.Tasks;
#endif
using System.Net;

public class DetectPrinter : Singleton<DetectPrinter> {
	public Text Text;
	string message;
	public static string address = "192.168.0.105";
	// if false, the above address will be used, convenient for debugging
	private bool enableDetection = true;

	#if !UNITY_EDITOR
    DatagramSocket listenerSocket;
#endif

	// Use this for initialization
	void Start() {
#if !UNITY_EDITOR
        if (enableDetection)
        {
            Listen();
            Send();
        }
#endif
	}
	
	// Update is called once per frame
	void Update() {
#if !UNITY_EDITOR
        Text.text = address;
#endif
	}

	#if !UNITY_EDITOR
    public string getUrl()
    {
        return "http://" + address + ":8080/print/model";
    }

    private async void Listen()
    {
        listenerSocket = new DatagramSocket();
        listenerSocket.MessageReceived += MessageReceived;
        await listenerSocket.BindServiceNameAsync("59104");
    }

    private async void Send()
    {
        IOutputStream outputStream;
        HostName localHostName = GetLocalIp();
        if (localHostName == null)
        {
            return;
        }
        byte? prefix = localHostName.IPInformation.PrefixLength;
        string localIPString = localHostName.ToString();
        IPAddress localIP = System.Net.IPAddress.Parse(localIPString);
        string subnetMaskString = "255.255.255.0"; // TODO: compute subnet mask
        IPAddress subnetIP = IPAddress.Parse(subnetMaskString);
        IPAddress broadCastIP = GetBroadcastAddress(localIP, subnetIP);
        HostName remoteHostname = new HostName(broadCastIP.ToString());
        outputStream = await listenerSocket.GetOutputStreamAsync(remoteHostname, "59105");

        Debug.Log(broadCastIP.ToString());

        using (DataWriter writer = new DataWriter(outputStream))
        {
            writer.WriteString("joiner v1");
            await writer.StoreAsync();
        }
    }

    private HostName GetLocalIp()
    {
        foreach (HostName localHostName in NetworkInformation.GetHostNames())
        {
            if (localHostName != null && localHostName.IPInformation != null)
            {
                if (localHostName.Type == HostNameType.Ipv4)
                {
                    //Debug.Log(localHostName);
                    return localHostName;
                }
            }
        }
        return null;
    }

    async void MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
    {
        Debug.Log("received");
        Stream streamIn = args.GetDataStream().AsStreamForRead();
        StreamReader reader = new StreamReader(streamIn);
        message = await reader.ReadLineAsync();

        
        Debug.Log(args.RemoteAddress); // printer
        Debug.Log(args.LocalAddress); // hololens

        //sendModel(args.RemoteAddress.ToString());
        address = args.RemoteAddress.ToString();

        Debug.Log(message);
    }

    private IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
    {
        byte[] ipAdressBytes = address.GetAddressBytes();
        byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

        if (ipAdressBytes.Length != subnetMaskBytes.Length)
            throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

        byte[] broadcastAddress = new byte[ipAdressBytes.Length];
        for (int i = 0; i < broadcastAddress.Length; i++)
        {
            broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
        }
        return new IPAddress(broadcastAddress);
    }
#endif
}
