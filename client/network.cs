using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System;

public class NetworkManager
{
    public static string username;
    public string data = "init";
    public bool IsConnected = false;

    Socket serverSocket;
    IPAddress ipAddr; 
    IPEndPoint ipEnd;
    //byte[] recvDataBuf = new byte[] { };
    byte[] recvDataBuf = new byte[1024];
    byte[] sendDataBuf = new byte[] { };
    Thread sockThread;
    GameController gameController;

    public NetworkManager()
    {
        // Set connection endpoint
        ipAddr = IPAddress.Parse("127.0.0.1");
        ipEnd = new IPEndPoint(ipAddr, 9998);
    }

    // Start socket thread
    public void StartUp(GameController gameC)
    {
        gameController = gameC;
        sockThread = new Thread(new ThreadStart(StartSocket));
        sockThread.Start();
    }

    void StartSocket()
    {
        SocketConnet();
        SocketListen();
    }

    void SocketConnet()
    {
        Debug.Log("Socket connect");

        // Connect
        if (serverSocket != null)
            serverSocket.Close();
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.NoDelay = true;
        try
        {
            serverSocket.Connect(ipEnd);
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
            IsConnected = false;
        }
        IsConnected = true;
    }

    // Send data
    public void SocketSend(string sendStr)
    {
        //Debug.Log("To send: " + sendStr);
        int size = sendStr.Length + Configuration.NET_HEAD_LENGTH_SIZE;

        byte[] wsize = BitConverter.GetBytes(size);
        byte[] sendDataByte = Encoding.ASCII.GetBytes(sendStr);
        byte[] rawData = wsize.Concat(sendDataByte).ToArray();

        SendRaw(rawData);
    }

    // Write raw data to buffer
    void SendRaw(byte[] data)
    {
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(data);

        sendDataBuf = sendDataBuf.Concat(data).ToArray();
        Process();
    }

    // Process buffer
    void Process()
    {
        //if (IsConnected && serverSocket.Connected)
        //{
        //    TryRecv();
        //}
        if (IsConnected && serverSocket.Connected)
        {
            TrySend();
        }
    }

    // Send data from buffer to socket
    // Return the length of sent bytes
    int TrySend()
    {
        int wsize = 0;

        if (sendDataBuf.Length == 0)
            return 0;
        try
        {
            wsize = serverSocket.Send(sendDataBuf);
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
            return -1;
        }

        sendDataBuf = sendDataBuf.Skip(wsize).ToArray();

        return wsize;
    }

    // Send data from socket to buffer
    // Return the length of received bytes
    //int TryRecv()
    //{
    //    byte[] rData = new byte[] { };

    //    while(true)
    //    {
    //        byte[] text = new byte[1024];
    //        try
    //        {
    //            if (serverSocket.Available == 0)
    //            {
    //                break;
    //            }
    //            int recvSize = serverSocket.Receive(text);
    //            if(recvSize == 0)
    //            {
    //                break;
    //            }
    //            text = text.Take(recvSize).ToArray();
    //        }
    //        catch(Exception e)
    //        {
    //            Debug.Log(e.ToString());
    //            return -1;
    //        }
    //        rData = rData.Concat(text).ToArray();
    //    }

    //    recvDataBuf = recvDataBuf.Concat(rData).ToArray();
        
    //    return rData.Length;
    //}

    // Recv an entire message from buffer
    //String SocketRecv()
    //{
    //    byte[] rSize = PeekRaw(Configuration.NET_HEAD_LENGTH_SIZE);
    //    if(rSize.Length < Configuration.NET_HEAD_LENGTH_SIZE)
    //    {
    //        return "";
    //    }

    //    int size = BitConverter.ToInt32(rSize, 0);
    //    if(recvDataBuf.Length < size)
    //    {
    //        return "";
    //    }

    //    RecvRaw(Configuration.NET_HEAD_LENGTH_SIZE);

    //    byte[] rawMsg = RecvRaw(size - Configuration.NET_HEAD_LENGTH_SIZE);
    //    return Encoding.ASCII.GetString(rawMsg);
    //}

    // peek data from recv_buf (read without delete it)
    //byte[] PeekRaw(int size)
    //{
    //    Process();

    //    if(recvDataBuf.Length == 0)
    //    {
    //        return new byte[] { };
    //    }

    //    if(size > recvDataBuf.Length)
    //    {
    //        size = recvDataBuf.Length;
    //    }

    //    byte[] rData = recvDataBuf.Take(size).ToArray();

    //    return rData;
    //}

    //// read data from recv_buf (read and delete it from recv_buf)
    //byte[] RecvRaw(int size)
    //{
    //    byte[] rData = PeekRaw(size);
    //    size = rData.Length;
    //    recvDataBuf = recvDataBuf.Skip(size).ToArray();
    //    return rData;
    //}

    // Read data from socket every 100 miliseconds
    void SocketListen()
    {
        while (true)
        {
            if (serverSocket.Poll(-1, SelectMode.SelectRead))
            {
                int recvLen = 0;
                byte[] recvData_length_str = new byte[4];
                while (recvLen < 4)
                {
                    recvLen += serverSocket.Receive(recvData_length_str, recvLen, 4, SocketFlags.None);
                }
                int recvData_length = BitConverter.ToInt32(recvData_length_str, 0);
                recvLen = 0;
                string recvStr = "";
                while (recvLen < recvData_length)
                {
                    if (recvData_length - recvLen > 1024)
                    {
                        recvLen += serverSocket.Receive(recvDataBuf, 0, 1024, SocketFlags.None);
                        recvStr += Encoding.ASCII.GetString(recvDataBuf, 0, 1024);
                    }
                    else
                    {
                        int recvLen_tmp = recvData_length - recvLen;
                        recvLen += serverSocket.Receive(recvDataBuf, 0, recvData_length - recvLen, SocketFlags.None);
                        recvStr += Encoding.ASCII.GetString(recvDataBuf, 0, recvLen_tmp);
                    }
                }
                gameController.recvMsg(recvStr);
            }




            //Thread.Sleep(20);

            //String data = SocketRecv();

            //if (data.Length > 0)
            //{
            //    gameController.recvMsg(data);
            //}
        }
    }

    public void SocketQuit()
    {
        IsConnected = false;
        if (sockThread != null)
        {
            sockThread.Interrupt();
            sockThread.Abort();
        }
        if (serverSocket != null)
            serverSocket.Close();
        Debug.Log("diconnect");
    }
}
