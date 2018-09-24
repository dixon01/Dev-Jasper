// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpClient.cs" company="Gorba AG">
//   Copyright © 2011 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   FTP Client class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

/* Copyright (c) 2006, J.P. Trosclair
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification, are permitted 
 * provided that the following conditions are met:
 *
 *  * Redistributions of source code must retain the above copyright notice, this list of conditions and 
 *		the following disclaimer.
 *  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions 
 *		and the following disclaimer in the documentation and/or other materials provided with the 
 *		distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR 
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF 
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * Based on FTPFactory.cs code, pretty much a complete re-write with FTPFactory.cs
 * as a reference.
 * 
 ***********************
 * Authors of this code:
 ***********************
 * J.P. Trosclair    (jptrosclair@judelawfirm.com)
 * Filipe Madureira  (filipe_madureira@hotmail.com) 
 * Carlo M. Andreoli (cmandreoli@numericaprogetti.it)
 * Sloan Holliday    (sloan@ipass.net)
 * 
 *********************** 
 * FTPFactory.cs was written by Jaimon Mathew (jaimonmathew@rediffmail.com)
 * and modified by Dan Rolander (Dan.Rolander@marriott.com).
 *	http://www.csharphelp.com/archives/archive9.html
 ***********************
 * 
 * ** DO NOT ** contact the authors of FTPFactory.cs about problems with this code. It
 * is not their responsibility. Only contact people listed as authors of THIS CODE.
 * 
 *  Any bug fixes or additions to the code will be properly credited to the author.
 * 
 *  BUGS: There probably are plenty. If you fix one, please email me with info
 *   about the bug and the fix, code is welcome.
 * 
 * All calls to the ftplib functions should be:
 * 
 * try 
 * { 
 *		// ftplib function call
 * } 
 * catch(Exception ex) 
 * {
 *		// error handeler
 * }
 * 
 * If you add to the code please make use of OpenDataSocket(), CloseDataSocket(), and
 * ReadResponse() appropriately. See the comments above each for info about using them.
 * 
 * The Fail() function terminates the entire connection. Only call it on critical errors.
 * Non critical errors should NOT close the connection.
 * All errors should throw an exception of type Exception with the response string from
 * the server as the message.
 * 
 * See the simple ftp client for examples on using this class
 */

namespace Gorba.Common.Protocols.Ftp
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;

    using NLog;

    public class FtpClient
    {
        #region Public Variables

        /// <summary>
        /// IP address or hostname to connect to
        /// </summary>
        public string server;
        /// <summary>
        /// Username to login as
        /// </summary>
        public string user;
        /// <summary>
        /// Password for account
        /// </summary>
        public string pass;
        /// <summary>
        /// Port number the FtpClient server is listening on
        /// </summary>
        public int port;
        /// <summary>
        /// The timeout (miliseconds) for waiting on data to arrive
        /// </summary>
        public int timeout;

        #endregion

        #region Private Variables

        private static readonly Logger Logger = LogManager.GetLogger(typeof(FtpClient).FullName);

        private string messages; // server messages
        private string responseStr; // server response if the user wants it.
        private bool passive_mode;		// #######################################
        private long bytes_total; // upload/download info if the user wants it.
        private long file_size; // gets set when an upload or download takes place
        private Socket main_sock;
        private IPEndPoint main_ipEndPoint;
        private Socket listening_sock;
        private Socket data_sock;
        private IPEndPoint data_ipEndPoint;
        private FileStream file;
        private int response;
        private string bucket;

        // DAC: Buffering Option
        //private MemoryStream dataBuffer;
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public FtpClient()
        {
            server = null;
            user = null;
            pass = null;
            port = 21;
            passive_mode = true;		// #######################################
            main_sock = null;
            main_ipEndPoint = null;
            listening_sock = null;
            data_sock = null;
            data_ipEndPoint = null;
            file = null;
            bucket = "";
            bytes_total = 0;
            timeout = 10000;	// 10 seconds
            messages = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="server">Server to connect to</param>
        /// <param name="user">Account to login as</param>
        /// <param name="pass">Account password</param>
        public FtpClient(string server, string user, string pass)
        {
            this.server = server;
            this.user = user;
            this.pass = pass;
            port = 21;
            passive_mode = true;		// #######################################
            main_sock = null;
            main_ipEndPoint = null;
            listening_sock = null;
            data_sock = null;
            data_ipEndPoint = null;
            file = null;
            bucket = "";
            bytes_total = 0;
            timeout = 10000;	// 10 seconds
            messages = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="server">Server to connect to</param>
        /// <param name="port">Port server is listening on</param>
        /// <param name="user">Account to login as</param>
        /// <param name="pass">Account password</param>
        public FtpClient(string server, int port, string user, string pass)
        {
            this.server = server;
            this.user = user;
            this.pass = pass;
            this.port = port;
            passive_mode = true;		// #######################################
            main_sock = null;
            main_ipEndPoint = null;
            listening_sock = null;
            data_sock = null;
            data_ipEndPoint = null;
            file = null;
            bucket = "";
            bytes_total = 0;
            timeout = 10000;	// 10 seconds
            messages = "";
        }

        #endregion

        /// <summary>
        /// Connection status to the server
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (main_sock != null)
                    return main_sock.Connected;
                return false;
            }
        }

        /// <summary>
        /// Returns true if the message buffer has data in it
        /// </summary>
        public bool MessagesAvailable
        {
            get
            {
                if (messages.Length > 0)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Server messages if any, buffer is cleared after you access this property
        /// </summary>
        public string Messages
        {
            get
            {
                string tmp = messages;
                messages = "";
                return tmp;
            }
        }

        /// <summary>
        /// The response string from the last issued command
        /// </summary>
        public string ResponseString
        {
            get
            {
                return responseStr;
            }
        }

        /// <summary>
        /// The total number of bytes sent/recieved in a transfer
        /// </summary>
        public long BytesTotal		// #######################################
        {
            get
            {
                return bytes_total;
            }
        }

        /// <summary>
        /// The size of the file being downloaded/uploaded (Can possibly be 0 if no size is available)
        /// </summary>
        public long FileSize		// #######################################
        {
            get
            {
                return file_size;
            }
        }

        /// <summary>
        /// True:  Passive mode [default]
        /// False: Active Mode
        /// </summary>
        public bool PassiveMode		// #######################################
        {
            get
            {
                return passive_mode;
            }
            set
            {
                passive_mode = value;
            }
        }


        private void Fail()
        {
            Disconnect();
            //throw new Exception(responseStr);
        }


        private void SetBinaryMode(bool mode)
        {
            bool okSend = false;
            if (mode)
                okSend = SendCommand("TYPE I");
            else
                okSend = SendCommand("TYPE A");

            if (!okSend)
            {
                this.Disconnect();
                return;
            }
            // else...

            ReadResponse();
            if (response != 200)
                Fail();
        }

        /// <summary>
        /// COS: 16 December 2010
        /// Send a command to the remote FTP server.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <returns>True if the command was sent with success, else false.</returns>
        public bool SendCommand(string command)
        {
            // COS: 16 December 2010
            // I've empowered this function adding try - catch statements
            // in order to prevent crashes on "Send" failure.
            // Sometimes they occour.
            Byte[] cmd = null;
            try
            {
                cmd = Encoding.ASCII.GetBytes((command + "\r\n").ToCharArray());
            }
            catch (Exception)
            {
                // an error was occured getting the byte array version
                // of the incoming string.
                // I cannot send nothing.
                return false;
            }
            // else...

#if (FTP_DEBUG)
            string debugString = (command.Length > 3 && command.Substring(0, 4) == "PASS") ? "\rPASS xxx" : ("\r" + command);
#endif

            // I've the buffer. Let's try to send it.
            int bytesSent = 0;
            try
            {
                bytesSent = this.main_sock.Send(cmd, cmd.Length, 0);
            }
            catch (Exception) { bytesSent = -1; }
            if (bytesSent == -1)
            {
                // an error was occured sending the buffer through the socket.
                // I cannot do nothing.
                // Someone else will close the socket.
                return false;
            }
            // else...

            // ok, it seems that the buffer was gone down...
            // really ?
            bool okSent = (bytesSent == cmd.Length);
            return okSent;
        }

        //OLD_VERSION
        //public void SendCommand(string command)
        //{
        //Byte[] cmd = Encoding.ASCII.GetBytes((command + "\r\n").ToCharArray());

        //#if (FTP_DEBUG)
        //            if (command.Length > 3 && command.Substring(0, 4) == "PASS")
        //                Console.WriteLine("\rPASS xxx");
        //            else
        //                Console.WriteLine("\r" + command);
        //#endif

        //main_sock.Send(cmd, cmd.Length, 0);
        //}

        private void FillBucket()
        {
            Byte[] bytes = new Byte[512];
            long bytesgot;
            int msecs_passed = 0;		// #######################################

            while (main_sock.Available < 1)
            {
                System.Threading.Thread.Sleep(50);
                msecs_passed += 50;
                // this code is just a fail safe option 
                // so the code doesn't hang if there is 
                // no data comming.
                if (msecs_passed > timeout)
                {
                    Disconnect();
                    throw new Exception("Timed out waiting on server to respond.");
                }
            }

            while (main_sock.Available > 0)
            {
                // COS: 16 December 2010
                // I've empowered this while( ... ) cycle adding try - catch
                // statement in order to prevent failures on the "Receive" function.
                // Sometimes they occour.
                //          START
                //  ||       ||        ||
                //  \/       \/        \/
                bytesgot = 0;
                try
                {
                    bytesgot = main_sock.Receive(bytes, 512, 0);
                }
                catch (Exception) { bytesgot = -1; }
                if (bytesgot == -1)
                {
                    // an error is occured reading data from the socket.
                    // I cannot do nothing.
                    System.Threading.Thread.Sleep(50);
                    //this.main_sock.Close(100); // <== I close here the socket.
                    this.main_sock.Close(); // timeout removed because not compatible with .NET CF.
                    return;
                }
                // else...

                bucket += Encoding.ASCII.GetString(bytes, 0, (int)bytesgot);
                // this may not be needed, gives any more data that hasn't arrived
                // just yet a small chance to get there.
                System.Threading.Thread.Sleep(50);
                //           END
                //  /\       /\        /\
                //  ||       ||        ||

                // OLD_VERSION
                //bytesgot = main_sock.Receive(bytes, 512, 0);
                //bucket += Encoding.ASCII.GetString(bytes, 0, (int)bytesgot);
                // this may not be needed, gives any more data that hasn't arrived
                // just yet a small chance to get there.
                //System.Threading.Thread.Sleep(50);
            }
        }


        private string GetLineFromBucket()
        {
            int i;
            string buf = "";

            if ((i = bucket.IndexOf('\n')) < 0)
            {
                while (i < 0)
                {
                    FillBucket();
                    i = bucket.IndexOf('\n');
                }
            }

            buf = bucket.Substring(0, i);
            bucket = bucket.Substring(i + 1);

            return buf;
        }

        /// <summary>
        /// COS: 15 December 2010
        /// I've made this function public, instead of private.
        /// 
        /// Why ? To flush the "responseStr" as needed, because this library
        /// does not treat the arrival of more than one asynch message from the
        /// remote FTP server.
        /// 
        /// Any time a command is sent, use ReadResponse() to get the response
        /// from the server. The variable responseStr holds the entire string and
        /// the variable response holds the response number.
        /// 
        /// </summary>
        public void ReadResponse()
        {
            string buf;
            messages = "";

            while (true)
            {
                // COS: 17 December 2010
                // I've empowered this function adding
                // try - catch statement in order to prevents
                // failures on the function "GetLineFromBucket".
                //             START
                //      ||      ||      ||
                //      \/      \/      \/
                try
                {
                    buf = GetLineFromBucket();
                }
                catch (Exception) { buf = string.Empty; }
                if (string.IsNullOrEmpty(buf))
                {
                    // an error was occured reading from the FTP server.
                    // I return immediately, without parsing nothing.
                    return;
                }
                // else...

                // something was really read...
                //              END
                //      /\      /\      /\
                //      ||      ||      ||

                //OLD_VERSION
                //buf = GetLineFromBucket();
                ////buf = GetLineFromBucket();


#if (FTP_DEBUG)
                Console.WriteLine(buf);
#endif
                // the server will respond with "000-Foo bar" on multi line responses
                // "000 Foo bar" would be the last line it sent for that response.
                // Better example:
                // "000-This is a multiline response"
                // "000-Foo bar"
                // "000 This is the end of the response"
                if (Regex.Match(buf, "^[0-9]+ ").Success)
                {
                    responseStr = buf;
                    response = int.Parse(buf.Substring(0, 3));
                    break;
                }
                else
                    messages += Regex.Replace(buf, "^[0-9]+-", "") + "\n";
            }
        }

        // if you add code that needs a data socket, i.e. a PASV or PORT command required,
        // call this function to do the dirty work. It sends the PASV or PORT command,
        // parses out the port and ip info and opens the appropriate data socket
        // for you. The socket variable is private Socket data_socket. Once you
        // are done with it, be sure to call CloseDataSocket()
        private void OpenDataSocket()
        {
            if (passive_mode)		// #######################################
            {
                string[] pasv;
                string server;
                int port;

                try
                {
                    Connect();
                }
                catch (Exception)
                {
                    // connection problem(s) with the remote FTP server.
                    // I cannot send nothing to it.
                    return;
                }
                // else...

                bool okSend = SendCommand("PASV");
                if (!okSend)
                {
                    this.Disconnect();
                    return;
                }
                // else...

                ReadResponse();
                if (response != 227)
                    Fail();

                try
                {
                    int i1, i2;

                    i1 = responseStr.IndexOf('(') + 1;
                    i2 = responseStr.IndexOf(')') - i1;
                    pasv = responseStr.Substring(i1, i2).Split(',');
                }
                catch (Exception)
                {
                    Disconnect();
                    throw new Exception("Malformed PASV response: " + responseStr);
                }

                if (pasv.Length < 6)
                {
                    Disconnect();
                    throw new Exception("Malformed PASV response: " + responseStr);
                }

                server = string.Format("{0}.{1}.{2}.{3}", pasv[0], pasv[1], pasv[2], pasv[3]);
                port = (int.Parse(pasv[4]) << 8) + int.Parse(pasv[5]);

                try
                {
#if (FTP_DEBUG)
                    Console.WriteLine("Data socket: {0}:{1}", server, port);
#endif
                    CloseDataSocket();

#if (FTP_DEBUG)
                    Console.WriteLine("Creating socket...");
#endif
                    data_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#if (!WindowsCE)
                    data_sock.LingerState = new LingerOption(true, 0);
#endif

#if (FTP_DEBUG)
                    Console.WriteLine("Resolving host");
#endif

                    IPAddress address = IPAddress.Parse(server);
                    data_ipEndPoint = new IPEndPoint(address, port);
                    // data_ipEndPoint = new IPEndPoint(Dns.GetHostEntry(server).AddressList[0], port);


#if (FTP_DEBUG)
                    Console.WriteLine("Connecting..");
#endif
                    data_sock.Connect(data_ipEndPoint);

#if (FTP_DEBUG)
                    Console.WriteLine("Connected.");
#endif
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to connect for data transfer: " + ex.Message);
                }
            }
            else		// #######################################
            {
                Connect();

                try
                {
#if (FTP_DEBUG)
                    Console.WriteLine("Data socket (active mode)");
#endif
                    CloseDataSocket();

#if (FTP_DEBUG)
                    Console.WriteLine("Creating listening socket...");
#endif
                    listening_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#if (!WindowsCE)
                    listening_sock.LingerState = new LingerOption(true, 0);
#endif

#if (FTP_DEBUG)
                    Console.WriteLine("Binding it to local address/port");
#endif
                    // for the PORT command we need to send our IP address; let's extract it
                    // from the LocalEndPoint of the main socket, that's already connected
                    string sLocAddr = main_sock.LocalEndPoint.ToString();
                    int ix = sLocAddr.IndexOf(':');
                    if (ix < 0)
                    {
                        throw new Exception("Failed to parse the local address: " + sLocAddr);
                    }
                    string sIPAddr = sLocAddr.Substring(0, ix);
                    // let the system automatically assign a port number (setting port = 0)
                    System.Net.IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(sIPAddr), 0);

                    listening_sock.Bind(localEP);
                    sLocAddr = listening_sock.LocalEndPoint.ToString();
                    ix = sLocAddr.IndexOf(':');
                    if (ix < 0)
                    {
                        throw new Exception("Failed to parse the local address: " + sLocAddr);
                    }
                    int nPort = int.Parse(sLocAddr.Substring(ix + 1));
#if (FTP_DEBUG)
                    Console.WriteLine("Listening on {0}:{1}", sIPAddr, nPort);
#endif
                    // start to listen for a connection request from the host (note that
                    // Listen is not blocking) and send the PORT command
                    listening_sock.Listen(1);
                    string sPortCmd = string.Format("PORT {0},{1},{2}",
                                                    sIPAddr.Replace('.', ','),
                                                    nPort / 256, nPort % 256);
                    bool okSend = SendCommand(sPortCmd);
                    if (!okSend)
                    {
                        this.Disconnect();
                        return;
                    }
                    // else...

                    ReadResponse();
                    if (response != 200)
                        Fail();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to connect for data transfer: " + ex.Message);
                }
            }
        }


        private void ConnectDataSocket()		// #######################################
        {
            if (data_sock != null)		// already connected (always so if passive mode)
                return;

            try
            {
#if (FTP_DEBUG)
                Console.WriteLine("Accepting the data connection.");
#endif
                data_sock = listening_sock.Accept();	// Accept is blocking
                listening_sock.Close();
                listening_sock = null;

                if (data_sock == null)
                {
                    throw new Exception("Winsock error: " +
                        Convert.ToString(System.Runtime.InteropServices.Marshal.GetLastWin32Error()));
                }
#if (FTP_DEBUG)
                Console.WriteLine("Connected.");
#endif
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to connect for data transfer: " + ex.Message);
            }
        }


        private void CloseDataSocket()
        {
#if (FTP_DEBUG)
            Console.WriteLine("Attempting to close data channel socket...");
#endif
            if (data_sock != null)
            {
                if (data_sock.Connected)
                {
#if (FTP_DEBUG)
                        Console.WriteLine("Closing data channel socket!");
#endif
                    data_sock.Close();
#if (FTP_DEBUG)
                        Console.WriteLine("Data channel socket closed!");
#endif
                }
                data_sock = null;
            }

            data_ipEndPoint = null;
        }
        /// <summary>
        /// Closes all connections to the ftp server
        /// </summary>
        public void Disconnect()
        {
            CloseDataSocket();

            if (main_sock != null)
            {
                if (main_sock.Connected)
                {
                    try
                    {
                        SendCommand("QUIT");
                    }
                    catch
                    { }
                    main_sock.Close();
                }
                main_sock = null;
            }

            CloseTransfer();

            main_ipEndPoint = null;
            file = null;

            this.response = 0;
            this.responseStr = string.Empty;
        }
        /// <summary>
        /// Connect to a ftp server
        /// </summary>
        /// <param name="server">IP or hostname of the server to connect to</param>
        /// <param name="port">Port number the server is listening on</param>
        /// <param name="user">Account name to login as</param>
        /// <param name="pass">Password for the account specified</param>
        public void Connect(string server, int port, string user, string pass)
        {
            this.server = server;
            this.user = user;
            this.pass = pass;
            this.port = port;

            Connect();
        }
        /// <summary>
        /// Connect to a ftp server
        /// </summary>
        /// <param name="server">IP or hostname of the server to connect to</param>
        /// <param name="user">Account name to login as</param>
        /// <param name="pass">Password for the account specified</param>
        public void Connect(string server, string user, string pass)
        {
            this.server = server;
            this.user = user;
            this.pass = pass;

            Connect();
        }
        /// <summary>
        /// Connect to an ftp server
        /// </summary>
        public void Connect()
        {
            if (server == null)
                throw new Exception("No server has been set.");
            if (user == null)
                throw new Exception("No username has been set.");

            // COS: 16 December 2010
            //             START
            //      ||      ||      ||
            //      \/      \/      \/
            bool okSend = false;
            if (this.main_sock != null)
            {
                // valid variable. I can check if the socket is connected.
                if (this.main_sock.Connected)
                {
                    // everything ok
                    return;
                }
                else
                {
                    // it seems that the socket is connected.
                    // are you sure ?
                    //try
                    //{
                    //    okSend = this.SendCommand("NOP");
                    //}
                    //catch (Exception) { okSend = false; }

                    //if (okSend)
                    //{
                    //    // yes, we are really connected.
                    //    // I avoid to connect to the FTP server twice,
                    //    // but before exiting, I read the response...
                    //    this.ReadResponse();
                    //    return;
                    //}
                    //// else...

                    // the sending operation of the NOP command was failed.
                    // no... we are not connected.
                    // I continue with this function in order to connect to the FTP server.
                    // this.main_sock.Close(100);
                    this.main_sock.Close(); // timeout removed because not compatible with .NET CF.
                    this.main_sock = null;
                }
            }
            //              END
            //      /\      /\      /\
            //      ||      ||      ||

                    main_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // var address = Dns.GetHostEntry(server).AddressList[0];
            IPAddress address = IPAddress.Parse(server);
                    main_ipEndPoint = new IPEndPoint(address, port);

                    try
                    {
                        main_sock.Connect(main_ipEndPoint);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }


            // OLD_VERSION
            //if (main_sock != null)
            //    if (main_sock.Connected)
            //		return;


            ReadResponse();
            if (response != 220)
                Fail();

            okSend = SendCommand("USER " + user);
            if (!okSend)
            {
                this.Disconnect();
                return;
            }
            // else...

            ReadResponse();

            switch (response)
            {
                case 331:
                    {
                        if (pass == null)
                        {
                            Disconnect();
                            throw new Exception("No password has been set.");
                        }
                        okSend = SendCommand("PASS " + pass);
                        if (!okSend)
                        {
                            this.Disconnect();
                            return;
                        }
                        // else...

                        ReadResponse();
                        if (response != 230)
                            Fail();
                    }
                    break;

                case 230:
                    break;
            }

            return;
        }
        /// <summary>
        /// Retrieves a list of file names from the ftp server
        /// </summary>
        /// <returns>An ArrayList of files</returns>
        public ArrayList ListNames()
        {
            Byte[] bytes = new Byte[512];
            string file_list = "";
            long bytesgot = 0;
            int msecs_passed = 0;
            ArrayList list = new ArrayList();

            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return list;
            }
            // else...

            OpenDataSocket();
            bool okSend = SendCommand("NLST");
            if (!okSend)
            {
                this.Disconnect();
                return list;
            }
            // else...

            ReadResponse();

            //FILIPE MADUREIRA.
            //Added response 125
            switch (response)
            {
                case 125:
                case 150:
                    break;
                default:
                    CloseDataSocket();
                    throw new Exception(responseStr);
            }
            ConnectDataSocket();		// #######################################

            while (data_sock.Available < 1)
            {
                System.Threading.Thread.Sleep(50);
                msecs_passed += 50;
                // this code is just a fail safe option 
                // so the code doesn't hang if there is 
                // no data comming.
                if (msecs_passed > (timeout / 10))
                {
                    //CloseDataSocket();
                    //throw new Exception("Timed out waiting on server to respond.");

                    //FILIPE MADUREIRA.
                    //If there are no files to list it gives timeout.
                    //So I wait less time and if no data is received, means that there are no files
                    break;//Maybe there are no files
                }
            }

            while (data_sock.Available > 0)
            {
                bytesgot = data_sock.Receive(bytes, bytes.Length, 0);
                file_list += Encoding.ASCII.GetString(bytes, 0, (int)bytesgot);
                System.Threading.Thread.Sleep(50); // *shrug*, sometimes there is data comming but it isn't there yet.
            }

            CloseDataSocket();

            ReadResponse();
            if (response != 226)
                throw new Exception(responseStr);

            foreach (string f in Regex.Split(file_list, "\r\n"))
            {
                list.Add(f);
            }

            return list;
        }
        /// <summary>
        /// Retrieves a list of files from the ftp server
        /// </summary>
        /// <returns>An ArrayList of files</returns>
        public ArrayList List()
        {
            Byte[] bytes = new Byte[512];
            string file_list = "";
            long bytesgot = 0;
            int msecs_passed = 0;
            ArrayList list = new ArrayList();

            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return list;
            }
            // else...

            OpenDataSocket();
            bool okSend = SendCommand("LIST");
            if (!okSend)
            {
                this.Disconnect();
                return list;
            }
            // else...

            ReadResponse();

            //FILIPE MADUREIRA.
            //Added response 125
            switch (response)
            {
                case 125:
                case 150:
                    break;
                default:
                    CloseDataSocket();
                    throw new Exception(responseStr);
            }
            ConnectDataSocket();		// #######################################

            while (data_sock.Available < 1)
            {
                System.Threading.Thread.Sleep(50);
                msecs_passed += 50;
                // this code is just a fail safe option 
                // so the code doesn't hang if there is 
                // no data comming.
                if (msecs_passed > (timeout / 10))
                {
                    //CloseDataSocket();
                    //throw new Exception("Timed out waiting on server to respond.");

                    //FILIPE MADUREIRA.
                    //If there are no files to list it gives timeout.
                    //So I wait less time and if no data is received, means that there are no files
                    break;//Maybe there are no files
                }
            }

            while (data_sock.Available > 0)
            {
                bytesgot = data_sock.Receive(bytes, bytes.Length, 0);
                file_list += Encoding.ASCII.GetString(bytes, 0, (int)bytesgot);
                System.Threading.Thread.Sleep(50); // *shrug*, sometimes there is data comming but it isn't there yet.
            }

            CloseDataSocket();

            ReadResponse();
            if (response != 226)
                throw new Exception(responseStr);

            foreach (string f in file_list.Split('\n'))
            {
                if (f.Length > 0 && !Regex.Match(f, "^total").Success)
                    list.Add(f.Substring(0, f.Length - 1));
            }

            return list;
        }
        /// <summary>
        /// Gets a file list only
        /// </summary>
        /// <returns>ArrayList of files only</returns>
        public ArrayList ListFiles()
        {
            ArrayList list = new ArrayList();

            foreach (string f in List())
            {
                //FILIPE MADUREIRA
                //In Windows servers it is identified by <DIR>
                if ((f.Length > 0))
                {
                    if ((f[0] != 'd') && (f.ToUpper().IndexOf("<DIR>") < 0))
                        list.Add(f);
                }
            }

            return list;
        }
        /// <summary>
        /// Gets a directory list only
        /// </summary>
        /// <returns>ArrayList of directories only</returns>
        public ArrayList ListDirectories()
        {
            ArrayList list = new ArrayList();

            foreach (string f in List())
            {
                //FILIPE MADUREIRA
                //In Windows servers it is identified by <DIR>
                if (f.Length > 0)
                {
                    if ((f[0] == 'd') || (f.ToUpper().IndexOf("<DIR>") >= 0))
                        list.Add(f);
                }
            }

            return list;
        }
        /// <summary>
        /// Returns the 'Raw' DateInformation in ftp format. (YYYYMMDDhhmmss). Use GetFileDate to return a DateTime object as a better option.
        /// </summary>
        /// <param name="fileName">Remote FileName to Query</param>
        /// <returns>Returns the 'Raw' DateInformation in ftp format</returns>
        public string GetFileDateRaw(string fileName)
        {
            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return string.Empty;
            }
            // else...

            bool okSend = SendCommand("MDTM " + fileName);
            if (!okSend)
            {
                this.Disconnect();
                return string.Empty;
            }
            // else...

            ReadResponse();
            if (response != 213)
            {
#if (FTP_DEBUG)
                Console.Write("\r" + responseStr);
#endif
                throw new Exception(responseStr);
            }

            return (this.responseStr.Substring(4));
        }
        /// <summary>
        /// GetFileDate will query the ftp server for the date of the remote file.
        /// </summary>
        /// <param name="fileName">Remote FileName to Query</param>
        /// <returns>DateTime of the Input FileName</returns>
        public DateTime GetFileDate(string fileName)
        {
            return ConvertFTPDateToDateTime(GetFileDateRaw(fileName));
        }

        private DateTime ConvertFTPDateToDateTime(string input)
        {
            if (input.Length < 14)
                throw new ArgumentException("Input Value for ConvertFTPDateToDateTime method was too short.");

            //YYYYMMDDhhmmss": 
            int year = Convert.ToInt16(input.Substring(0, 4));
            int month = Convert.ToInt16(input.Substring(4, 2));
            int day = Convert.ToInt16(input.Substring(6, 2));
            int hour = Convert.ToInt16(input.Substring(8, 2));
            int min = Convert.ToInt16(input.Substring(10, 2));
            int sec = Convert.ToInt16(input.Substring(12, 2));

            return new DateTime(year, month, day, hour, min, sec);
        }
        /// <summary>
        /// Get the working directory on the ftp server
        /// </summary>
        /// <returns>The working directory</returns>
        public string GetWorkingDirectory()
        {
            //PWD - print working directory
            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return string.Empty;
            }
            // else...

            bool okSend = SendCommand("PWD");
            if (!okSend)
            {
                this.Disconnect();
                return string.Empty;
            }
            // else...

            ReadResponse();

            if (response != 257)
                throw new Exception(responseStr);

            string pwd;
            try
            {
                pwd = responseStr.Substring(responseStr.IndexOf("\"", 0) + 1);//5);
                pwd = pwd.Substring(0, pwd.LastIndexOf("\""));
                pwd = pwd.Replace("\"\"", "\""); // directories with quotes in the name come out as "" from the server
            }
            catch (Exception ex)
            {
                throw new Exception("Uhandled PWD response: " + ex.Message);
            }

            return pwd;
        }

        /// <summary>
        /// Change to another directory on the ftp server
        /// </summary>
        /// <param name="path">Directory to change to</param>
        public void ChangeDir(string path)
        {
            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return;
            }
            // else...

            bool okSend = SendCommand("CWD " + path);
            if (!okSend)
            {
                this.Disconnect();
                return;
            }
            // else...

            ReadResponse();
            if (response != 250)
            {
#if (FTP_DEBUG)
                Console.Write("\r" + responseStr);
#endif
                throw new Exception(responseStr);
            }
        }
        /// <summary>
        /// Create a directory on the ftp server
        /// </summary>
        /// <param name="dir">Directory to create</param>
        public void MakeDir(string dir)
        {
            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return;
            }
            // else...

            bool okSend = SendCommand("MKD " + dir);
            if (!okSend)
            {
                this.Disconnect();
                return;
            }
            // else...

            ReadResponse();

            switch (response)
            {
                case 257:
                case 250:
                    break;
                default:
#if (FTP_DEBUG)
                    Console.Write("\r" + responseStr);
#endif
                    throw new Exception(responseStr);
            }
        }
        /// <summary>
        /// Remove a directory from the ftp server
        /// </summary>
        /// <param name="dir">Name of directory to remove</param>
        public void RemoveDir(string dir)
        {
            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return;
            }
            // else...

            bool okSend = SendCommand("RMD " + dir);
            if (!okSend)
            {
                this.Disconnect();
                return;
            }
            // else...

            ReadResponse();
            if (response != 250)
            {
#if (FTP_DEBUG)
                Console.Write("\r" + responseStr);
#endif
                throw new Exception(responseStr);
            }
        }
        /// <summary>
        /// Remove a file from the ftp server
        /// </summary>
        /// <param name="filename">Name of the file to delete</param>
        public void RemoveFile(string filename)
        {
            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return;
            }
            // else...

            bool okSend = SendCommand("DELE " + filename);
            if (!okSend)
            {
                this.Disconnect();
                return;
            }
            // else...

            ReadResponse();
            if (response != 250)
            {
#if (FTP_DEBUG)
                Console.Write("\r" + responseStr);
#endif
                throw new Exception(responseStr);
            }
        }
        /// <summary>
        /// Rename a file on the ftp server
        /// </summary>
        /// <param name="oldfilename">Old file name</param>
        /// <param name="newfilename">New file name</param>
        public void RenameFile(string oldfilename, string newfilename)		// #######################################
        {
            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return;
            }
            // else...

            bool okSend = SendCommand("RNFR " + oldfilename);
            if (!okSend)
            {
                this.Disconnect();
                return;
            }
            // else...

            ReadResponse();
            if (response != 350)
            {
#if (FTP_DEBUG)
                Console.Write("\r" + responseStr);
#endif
                throw new Exception(responseStr);
            }
            else
            {
                okSend = SendCommand("RNTO " + newfilename);
                if (!okSend)
                {
                    this.Disconnect();
                    return;
                }
                // else...

                ReadResponse();
                if (response != 250)
                {
#if (FTP_DEBUG)
                    Console.Write("\r" + responseStr);
#endif
                    throw new Exception(responseStr);
                }
            }
        }

        /// <summary>
        /// COS: 07 Feb 2011
        /// Tells if a specific command is supported or not by the remote FTP server.
        /// The specific command will be sent to the remote FTP server and will be
        /// studied the returned answer.
        /// If is returned: 200 ==> the command is supported, else it is not supported.
        /// </summary>
        /// <param name="cmd">The command to test.</param>
        /// <returns>True if the command is supported else false.</returns>
        public bool IsSupportedCmd(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                // invalid command.
                return false;
            }
            // else...

            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return false;
            }
            // else...

            bool okSend = SendCommand(cmd);
            if (!okSend)
            {
                // an error was occured sending the command.
                this.Disconnect();
                return false;
            }
            // else...

            bool supported = (response == 200);
            return supported;
        }

        /// <summary>
        /// Get the size of a file (Provided the ftp server supports it)
        /// </summary>
        /// <param name="filename">Name of file</param>
        /// <returns>The size of the file specified by filename,
        /// COS: 13 December 2010
        /// or -1 if the function does fail.</returns>
        public long GetFileSize(string filename)
        {
            long retval = 0;
            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return -1;
            }
            // else...

            bool okSend = SendCommand("SIZE " + filename);
            if (!okSend)
            {
                this.Disconnect();
                return -1;
            }
            // else...

            ReadResponse();

            // COS: 13 December 2010
            // function refactoring in order to have also returned
            // values when the function does fail
            long? size;
            try
            {
                size = long.Parse(responseStr.Substring(4));
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Couldn't parse Size response. Response code: {0}", this.response);
                size = null;
            }

            retval = (response == 213 && size.HasValue) ? size.Value : -1;

            // COS: 07 Feb 2011
            // If it is not possible to get the size of the file
            // I've to throw an Exception. Throwing it will
            // ensure that each application takes care about the knowledge or not
            // of the file's size.
            if (retval == -1)
            {
                // the "SIZE" command was failed.
                throw new Exception("SIZE command failed.");
            }

            return retval;

            // OLD_VERSION
            //if (response != 213)
            //{
            //    //#if (FTP_DEBUG)
            //    //        Console.Write("\r" + responseStr);
            //    //#endif
            //    //        throw new Exception(responseStr);
            //}
            //else
            //{
            //    retval = Int64.Parse(responseStr.Substring(4));
            //}

            //return retval;
        }
        /// <summary>
        /// Open an upload with no resume if it already exists
        /// </summary>
        /// <param name="filename">File to upload</param>
        public void OpenUpload(string filename)
        {
            OpenUpload(filename, filename, false);
        }
        /// <summary>
        /// Open an upload with no resume if it already exists
        /// </summary>
        /// <param name="filename">Local file to upload (Can include path to file)</param>
        /// <param name="remotefilename">Filename to store file as on ftp server</param>
        public void OpenUpload(string filename, string remotefilename)
        {
            OpenUpload(filename, remotefilename, false);
        }
        /// <summary>
        /// Open an upload with resume support
        /// </summary>
        /// <param name="filename">Local file to upload (Can include path to file)</param>
        /// <param name="resume">Attempt resume if exists</param>
        public void OpenUpload(string filename, bool resume)
        {
            OpenUpload(filename, filename, resume);
        }
        /// <summary>
        /// Open an upload with resume support
        /// </summary>
        /// <param name="filename">Local file to upload (Can include path to file)</param>
        /// <param name="remote_filename">Filename to store file as on ftp server</param>
        /// <param name="resume">Attempt resume if exists</param>
        public void OpenUpload(string filename, string remote_filename, bool resume)
        {
            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return;
            }
            // else...

            SetBinaryMode(true);
            OpenDataSocket();

            bytes_total = 0;

            try
            {
                file = new FileStream(filename, FileMode.Open);
            }
            catch (Exception ex)
            {
                file = null;
                throw new Exception(ex.Message);
            }

            file_size = file.Length;

            bool okSend = false;
            if (resume)
            {
                long size = GetFileSize(remote_filename);
                if (size > 0)
                {
                    okSend = SendCommand("REST " + size);
                    if (!okSend)
                    {
                        this.Disconnect();
                        return;
                    }
                    // else...

                    ReadResponse();
                    if (response == 350)
                        file.Seek(size, SeekOrigin.Begin);
                }
            }

            okSend = SendCommand("STOR " + remote_filename);
            if (!okSend)
            {
                this.Disconnect();
                return;
            }
            // else...

            ReadResponse();

            switch (response)
            {
                case 125:
                case 150:
                    break;
                default:
                    file.Close();
                    file = null;
                    throw new Exception(responseStr);
            }
            ConnectDataSocket();		// #######################################	

            return;
        }
        /// <summary>
        /// Download a file with no resume
        /// </summary>
        /// <param name="filename">Remote file name</param>
        public void OpenDownload(string filename)
        {
            OpenDownload(filename, filename, false);
        }
        /// <summary>
        /// Download a file with optional resume
        /// </summary>
        /// <param name="filename">Remote file name</param>
        /// <param name="resume">Attempt resume if file exists</param>
        public void OpenDownload(string filename, bool resume)
        {
            OpenDownload(filename, filename, resume);
        }
        /// <summary>
        /// COS: 15 May 2011
        /// I've add a boolean as returned information, after the execution of this function.
        /// Now "OpenDownload" is not anymore a "void" function.
        /// 
        /// Download a file with no attempt to resume
        /// 
        /// </summary>
        /// <param name="filename">Remote filename</param>
        /// <param name="localfilename">Local filename (Can include path to file)</param>
        /// <returns>True if the donwload "stream" was really opened with success, else false.</returns>
        public bool OpenDownload(string filename, string localfilename)
        {
            return OpenDownload(filename, localfilename, false);
        }

        /// <summary>
        /// COS: 15 May 2011
        /// I've add a boolean as returned information, after the execution of this function.
        /// Now "OpenDownload" is not anymore a "void" function.
        /// 
        /// Open a file for download
        /// </summary>
        /// <param name="remote_filename">The name of the file on the FtpClient server</param>
        /// <param name="local_filename">The name of the file to save as (Can include path to file)</param>
        /// <param name="resume">Attempt resume if file exists</param>
        /// <returns>True if the donwload "stream" was really opened with success, else false.</returns>
        public bool OpenDownload(string remote_filename, string local_filename, bool resume)
        {
            try
            {
                Connect();
            }
            catch (Exception)
            {
                // connection problem(s) with the remote FTP server.
                // I cannot send nothing to it.
                return false;
            }
            // else...

            SetBinaryMode(true);

            bytes_total = 0;
            bool okSend = false;

            // COS: 07 Feb 2011
            // It seems unuseful how the "GetFileSize" is used hereafter.
            // So, I've removed it.
            //try
            //{
            //    file_size = GetFileSize(remote_filename);
            //}
            //catch
            //{
            //    file_size = 0;
            //}

            if (resume && File.Exists(local_filename))
            {
                try
                {
                    file = new FileStream(local_filename, FileMode.Open);
                }
                catch (Exception ex)
                {
                    file = null;
                    throw new Exception(ex.Message);
                }

                okSend = SendCommand("REST " + file.Length);
                if (!okSend)
                {
                    this.Disconnect();
                    return false;
                }
                // else...

                ReadResponse();
                if (response != 350)
                    throw new Exception(responseStr);
                file.Seek(file.Length, SeekOrigin.Begin);
                bytes_total = file.Length;
            }
            else
            {
                try
                {
                    file = new FileStream(local_filename, FileMode.Create);
                }
                catch (Exception ex)
                {
                    file = null;
                    throw new Exception(ex.Message);
                }
            }

            OpenDataSocket();
            okSend = SendCommand("RETR " + remote_filename);
            if (!okSend)
            {
                this.Disconnect();
                return false;
            }
            // else...

            ReadResponse();

            switch (response)
            {
                case 125:
                    break;
                case 150:
                    break;

                default:
                    file.Close();
                    file = null;
                    throw new Exception(responseStr);
            }
            ConnectDataSocket();		// #######################################	

            // ok. it seems that the "OpenDownload" function
            // has really opened the download.
            return true;
        }

        /// <summary>
        /// Closes to the file of the download
        /// </summary>
        public void CloseTransfer()
        {
            if (file != null)
            {
                file.Close();
                file = null;
            }
        }

        /// <summary>
        /// Upload the file, to be used in a loop until file is completely uploaded
        /// </summary>
        /// <returns>Bytes sent</returns>
        public long DoUpload()
        {
            Byte[] bytes = new Byte[2000];
            long bytes_got = 0;

            try
            {
                bytes_got = file.Read(bytes, 0, bytes.Length);
                bytes_total += bytes_got;
                data_sock.Send(bytes, (int)bytes_got, 0);
            }
            catch (Exception)
            {
                // send failed, consider data as not sent. Treat a as temporary failure
                bytes_total -= bytes_got;
            }

            if (bytes_got <= 0)
            {
                // the upload is complete or an error occured
                file.Close();
                file = null;

                CloseDataSocket();
                ReadResponse();
                switch (response)
                {
                    case 226:
                    case 250:
                        break;
                    default:
                        //throw new Exception(responseStr);
                        break;
                }

                //SetBinaryMode(false);
            }

            return bytes_got;
        }

        /// <summary>
        /// Download a file, to be used in a loop until the file is completely downloaded
        /// </summary>
        /// <returns>Number of bytes recieved</returns>
        public long DoDownload()
        {
            Byte[] bytes = new Byte[2000];
            int bytes_got = 0;

            bytes_got = data_sock.Receive(bytes, bytes.Length, SocketFlags.None);
  

            if (bytes_got <= 0)
            {
                ReadResponse();
                switch (response)
                {
                    case 226:
                    case 250:
                        break;
                    default:
                        //throw new Exception(responseStr);
                        break;
                }

                //SetBinaryMode(false);
                // DAC: Buffering Option
                //if (dataBuffer != null)
                //{
                //  // Flush buffered data to file
                //  file.Write(dataBuffer.GetBuffer(), 0, (int)dataBuffer.Length);
                //  dataBuffer = null;
                //}
                return bytes_got;
            }

            try
            {
                // DAC: Buffering Option
                //if (dataBuffer == null)
                //{
                //  dataBuffer = new MemoryStream();
                //}
                //dataBuffer.Write(bytes, 0, bytes_got);
                //if (dataBuffer.Length > 500000)
                //{
                //  file.Write(dataBuffer.GetBuffer(), 0, (int)dataBuffer.Length);
                //  dataBuffer = null;
                //}
                file.Write(bytes, 0, (int)bytes_got);
                bytes_total += bytes_got;
            }
            catch (Exception)
            {
                CloseDataSocket();
                CloseTransfer();
            }

            return bytes_got;
        }

        /// <summary>
        /// COS: 16 May 2011
        /// The "OpenDownload" function could be slow if you donwload a file
        /// without knowing the file's size, as done in the QBuzz Project.
        /// The slow is caused by the "timeout" variable. Without knowing the file's size,
        /// I need to "wait for an exception" in order to understand that a download process
        /// is really finished or not.
        /// The time to wait is set into the "timeout" variable.
        /// So, today I've added the Get/Set function around this function just only to customize
        /// the download speed for QBuzz.
        /// </summary>
        /// <returns>The FTP timeout.</returns>
        public int GetTimeout()
        {
            return this.timeout;
        }

        /// <summary>
        /// COS: 16 May 2011
        /// The "OpenDownload" function could be slow if you donwload a file
        /// without knowing the file's size, as done in the QBuzz Project.
        /// The slow is caused by the "timeout" variable. Without knowing the file's size,
        /// I need to "wait for an exception" in order to understand that a download process
        /// is really finished or not.
        /// The time to wait is set into the "timeout" variable.
        /// So, today I've added the Get/Set function around this function just only to customize
        /// the download speed for QBuzz.
        /// </summary>
        /// <param name="newTimeOut">The new FTP timeout.</param>
        public void SetTimeout(int newTimeOut)
        {
            this.timeout = newTimeOut;
        }
    }
}
