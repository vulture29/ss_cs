using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using System.Threading;
using System;

public class GameController : MonoBehaviour {

	public static string username;
	public string data = "init";

	Socket serverSocket; //服务器端socket  
	IPAddress ip; //主机ip  
	IPEndPoint ipEnd;   
	string recvStr; //接收的字符串  
	string sendStr; //发送的字符串  
	byte[] recvData=new byte[1024]; //接收的数据，必须为字节  
	byte[] sendData=new byte[1024]; //发送的数据，必须为字节  
	int recvLen; //接收的数据长度  
	Thread connectThread; //连接线程  

	public string getData() {
		return data;
	}

	void InitSocket()  
	{  
		//定义服务器的IP和端口，端口与服务器对应  
		ip=IPAddress.Parse("127.0.0.1"); //可以是局域网或互联网ip，此处是本机  
		ipEnd=new IPEndPoint(ip,5566);  


		//开启一个线程连接，必须的，否则主线程卡死  
		connectThread=new Thread(new ThreadStart(SocketReceive));  
		connectThread.Start();  
	}

	void SocketConnet()  
	{  
		if(serverSocket!=null)  
			serverSocket.Close();   
		serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);  
		 
		serverSocket.Connect(ipEnd);

		print (username);
		if (username == null || username.Length <= 0) {
			username = "Guest";
		}
		JSONObject userJson = new JSONObject ();

		userJson.AddField ("type", "playerUsername");
		userJson.AddField ("value", username);

		SocketSend (userJson.ToString());
	}

	public void SocketSend(string sendStr)  
	{  
		byte[] headLen = BitConverter.GetBytes(sendStr.Length);
		if (BitConverter.IsLittleEndian)
			Array.Reverse(headLen);
		serverSocket.Send(headLen, headLen.Length, SocketFlags.None); 

		sendData = new byte[1024];
		sendData = Encoding.ASCII.GetBytes(sendStr);
		serverSocket.Send(sendData,sendData.Length,SocketFlags.None);  
//		print ("Sent: " + sendStr);
	}  

	void SocketReceive()  
	{  
		SocketConnet();  
		//不断接收服务器发来的数据  

		while (true) {
			if (serverSocket.Poll (-1, SelectMode.SelectRead)) {
				print ("Start receiving:");
				recvLen = 0;
				byte[] recvData_length_str = new byte[4];
				while (recvLen < 4) {
					recvLen += serverSocket.Receive (recvData_length_str, recvLen, 4, SocketFlags.None);
				}
				int recvData_length = BitConverter.ToInt32(recvData_length_str, 0);
				recvLen = 0;
				recvStr = "";
				while (recvLen < recvData_length) {
					if (recvData_length - recvLen > 1024) {
						recvLen += serverSocket.Receive (recvData, 0, 1024, SocketFlags.None);
						recvStr += Encoding.ASCII.GetString(recvData, 0, 1024);
					} else {
						int recvLen_tmp = recvData_length - recvLen;
						recvLen += serverSocket.Receive (recvData, 0, recvData_length - recvLen, SocketFlags.None);
						recvStr += Encoding.ASCII.GetString(recvData, 0, recvLen_tmp);
					}
				}

//				print ("Received: " + recvStr);
				data = recvStr;
			}
		}
	}  

	public void SocketQuit()  
	{  
		if(connectThread!=null) {  
			connectThread.Interrupt();  
			connectThread.Abort();  
		} 
		if(serverSocket!=null)  
			serverSocket.Close();  
		print("diconnect");  
	}  

	// Use this for initialization
	void Start () {
		InitSocket();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnApplicationQuit()  
	{  
		SocketQuit();  
	} 
}
