using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;


namespace Gorba.Library.Protocols.Qnet
{

    #region alias declaration to make the // with qnet definitions
    using tSOCKET = System.Int16;

    using tQNETport = System.Byte;

    // typedef WORD tQNETaddr;       
    // Iqube network address
    using tQNETaddr = System.UInt16;

    // typedef BYTE tQNETport;       
    // Iqube protocol port (mapped to taskId)    
    using tQnetPort = System.Byte;

    #endregion

    /// <summary>
    /// The wrapper class. Needs the native dll qnetdll.dll!
    /// </summary>
    internal class WrapperQnet
    {
        /// <summary>
        /// At Markus Veser (CSA) qnetdll.dll each method has the method name like that:
        /// _QCMD_Ping(...)
        /// _ is the pre entry point
        /// </summary>
        public const String PRE_EENTRY_POINT = "_";

        private static object locker = new object();
        private tSOCKaddr stationAddress;
        //private tSOCKET socket;

        /// <summary>
        /// Get Version from Qnet Wrapper
        /// </summary>
        /// <returns>Version String</returns>
        public static String getVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }


        /// <summary>
        /// Initialises the QNET wrapper.
        /// Important: 
        /// The qroute.cfg has to be in the same directory as qnetdll.dll
        /// </summary>
        public WrapperQnet()
        {
            Console.WriteLine("WrapperQnet Version: " + getVersion() + " initialising");
            int counter = 0;
            int MAX_COUNTER = 10;   //in OmcProtocolStack.pas definiert
            int SUCCESS = 0;    //in OmcMessageTyp.pas definiert
            Int32 ret;
            do
            { 
                ret = VOS_Init();
                Console.WriteLine("vos_init: 0x" + ret.ToString("X") + ", dez: " + ret.ToString());
                counter++;
                System.Threading.Thread.Sleep(100);
            } while (!(((ushort)ret != 0x8900) || (counter > MAX_COUNTER)));

            if (ret < SUCCESS)
            {
                switch (ret)
                {
                    case 0xffff:
                        Console.WriteLine("'General ProtocolStack Init Error!'+#13+");
                        break;
                    case 0x8900:
                        Console.WriteLine("'Failed to Init COM Port!'");
                        break;
                    default:
                        Console.WriteLine("Failed init QnetDLL");
                        break;
                }
                throw new Exception("VOS_Init() failed. Is Qroute.cfg in the same direcory as the dll? Return value: 0x" + ret.ToString("X") + ", dez: " + ret.ToString());
            }
        }


        /// <summary>
        /// Set / Get the Address from this object
        /// </summary>
        public tSOCKaddr SOCKaddr
        {
            get { return stationAddress; }
            set { stationAddress = value; }
        }

        #region HAL_GetCOMHandle    //EntryPoint = "HAL_GetCOMHandle"
        //HAL_GETComHandle    : function (ComPort:Int16):Int16; cdecl;
        /*   [DllImport("QnetDLL.dll", EntryPoint = PRE_EENTRY_POINT + "HAL_GetCOMHandle", SetLastError = true)]
           private static extern Int16 MyHAL_GetCOMHandle(Int16 comPort);
           public Int16 HAL_GetCOMHandle(Int16 comPort)
           {
               lock (locker)
               {
                   return MyHAL_GetCOMHandle(comPort);
               }
           }*/
        #endregion

        #region QCMD_Ping
        //QCMD_Ping           : function (dstAddr:TQNETaddr; size:Byte; timeout:DWORD):Int16; cdecl;
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "QCMD_Ping", SetLastError = true)]
        private static extern Int16 MyQCMD_Ping(tQNETaddr qAddr, byte size, UInt32 timeOut);
        /// <summary>
        /// Ping an iqube or display
        /// </summary>
        /// <param name="qAddr">Destination Address</param>
        /// <param name="size">packet size. for Examp. 10</param>
        /// <param name="timeOut">Response Timeout in ms</param>
        /// <returns>0: Destination gave response</returns>
        public Int16 QCMD_Ping(tQNETaddr qAddr, byte size, UInt32 timeOut)
        {
            lock (locker)
            {
                return MyQCMD_Ping(qAddr, size, timeOut);
            }
        }
        /// <summary>
        /// Send a ping with 10Byte date and an timeout of 1000ms
        /// </summary>
        /// <param name="address">The destination address</param>
        /// <returns>true when response received</returns>
        public bool QCMD_Ping(QnetAddress address)
        {
            return this.QCMD_Ping(address, 10, 1000);
        }
        /// <summary>
        /// Ping an iqube or display
        /// </summary>
        /// <param name="address">destination address</param>
        /// <param name="size">packet size</param>
        /// <param name="timeOut">timeout in ms</param>
        /// <returns>true when response received</returns>
        public bool QCMD_Ping(QnetAddress address, byte size, UInt16 timeOut)
        {
            lock (locker)
            {
                if (MyQCMD_Ping(address.TQNETaddr(), size, timeOut) == 0)
                {
                    Console.WriteLine("Ping: Target received.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Ping: No response from target.");
                    return false;
                }
            }
        }
        #endregion

        #region QCMD_Restart
        //INT16 __EXPORT_TYPE QCMD_Restart    (tQNETaddr dstAddr);
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "QCMD_Restart", SetLastError = true)]
        private static extern Int16 MyQCMD_Restart(tQNETaddr qAddr);
        /// <summary>
        /// Restart an iqube or display
        /// </summary>
        /// <param name="qAddr">destination address</param>
        /// <returns></returns>
        public Int16 QCMD_Restart(tQNETaddr qAddr)
        {
            lock (locker)
            {
                return MyQCMD_Restart(qAddr);
            }
        }
        #endregion

        #region Vos_Init
        //VOS_INIT            : function : INT16; cdecl;
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "VOS_Init", SetLastError = true)]
        private static extern Int16 _VOS_Init();
        /// <summary>
        /// Initialise VOS (Virtual Operation System)
        /// </summary>
        /// <returns>if the return value negative, then error was happen</returns>
        public Int16 VOS_Init()
        {
            lock (locker)
            {
                return _VOS_Init();
            }
        }
        #endregion

        #region Vos_Exit
        //VOS_EXIT            : procedure; cdecl;
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "VOS_Exit", SetLastError = true)]
        private static extern void MyVOS_Exit();
        /// <summary>
        /// Exit VOS (Virtual Operation System)
        /// </summary>
        public void VOS_Exit()
        {
            lock (locker)
            {
                MyVOS_Exit();
            }
        }
        #endregion

        #region Vos_Sleep
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "VOS_Sleep", SetLastError = true)]
        private static extern void MyVOS_Sleep(UInt32 ms);
        /// <summary>
        /// Sleep VOS (Virtual Operation System)
        /// </summary>
        /// <param name="ms"></param>
        public void VOS_Sleep(UInt32 ms)
        {
            lock (locker)
            {
                MyVOS_Sleep(ms);
            }
        }
        #endregion


        #region QNET_GetMyAddr
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "QNET_GetMyAddr", SetLastError = true)]
        private static extern tQNETaddr MyQNET_GetMyAddr();
        /// <summary>
        /// get the local qnetAddress
        /// </summary>
        /// <returns></returns>
        public tQNETaddr QNET_GetMyAddr()
        {
            lock (locker)
            {
                return MyQNET_GetMyAddr();
            }
        }
        #endregion


        #region SOCK_Init
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "SOCK_Init", SetLastError = true)]
        private static extern void MySOCK_Init();
        private void SOCK_Init()
        {
            lock (locker)
            {
                MySOCK_Init();
            }
        }
        #endregion

        #region SOCK_Create
        //SOCK_Create         : function (SockTyp:Int16; Socket:PSocket) : Int16; cdecl;
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "SOCK_Create", SetLastError = true)]
        private static extern tQNETaddr MySOCK_Create(Int16 sockType, out tSOCKET sockPtr);
        /// <summary>
        /// Creates a socket
        /// </summary>
        /// <param name="sockType"></param>
        /// <param name="sockPtr"></param>
        /// <returns></returns>
        public tQNETaddr SOCK_Create(tSOCKtype sockType, out tSOCKET sockPtr)
        {
            lock (locker)
            {
                return MySOCK_Create((Int16)sockType, out sockPtr);
            }
        }
        #endregion

        #region SOCK_Close
        //SOCK_Close          : procedure (Socket:TSocket); cdecl;
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "SOCK_Close", SetLastError = true)]
        private static extern void MySOCK_Close(tSOCKET socket);
        /// <summary>
        /// Close an open Socket
        /// </summary>
        /// <param name="socket">socket</param>
        public void SOCK_Close(tSOCKET socket)
        {
            lock (locker)
            {
                MySOCK_Close(socket);
            }
        }
        #endregion

        #region SOCK_Bind
        //SOCK_Bind           : function (Socket:TSocket; SockAddr:PSockAddr) : Int16; cdecl;
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "SOCK_Bind", SetLastError = true)]
        private static extern tQNETaddr MySOCK_Bind(tSOCKET socket, out tSOCKaddr adrPtr);
        /// <summary>
        /// Bind the socket to the port which is defined in adrPtr.port
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="adrPtr">address (Port) to bind</param>
        /// <returns></returns>
        public tQNETaddr SOCK_Bind(tSOCKET socket, tSOCKaddr adrPtr)
        {
            lock (locker)
            {
                return MySOCK_Bind(socket, out adrPtr);
            }
        }
        #endregion

        #region SOCK_Connect
        //SOCK_Connect := GetProcAddress(FDLLHandle, '_SOCK_Connect');
        //SOCK_Connect        : function (Socket:TSocket; SockAddr:PSockAddr) : Int16; cdecl;
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "SOCK_Connect", SetLastError = true)]
        private static extern Int16 MySOCK_Connect(tSOCKET socket, out tSOCKaddr adrPtr);
        /// <summary>
        /// Connect the socket to the remote address
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="adrPtr">remote address</param>
        /// <returns>if negative, an error was happen</returns>
        public Int16 SOCK_Connect(tSOCKET socket, tSOCKaddr adrPtr)
        {
            lock (locker)
            {
                return MySOCK_Connect(socket, out adrPtr);
            }
        }
        #endregion

        #region SOCK_Receive
        private static Object lockerReceive = new Object();
        //INT16 _EXP_T_ SOCK_Receive     (tSOCKET socket, void *bufPtr, INT16 bufLen);
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "SOCK_Receive", SetLastError = true)]
        private static extern UInt16 MySOCK_Receive(tSOCKET socket, IntPtr bufPtr, Int16 bufLen);
        /// <summary>
        /// Receive a datagram (or ACK)
        /// </summary>
        /// <param name="socket">the corresponding socket</param>
        /// <param name="bufPtr">the buffer to save the data</param>
        /// <param name="bufLen">buffer length</param>
        /// <returns>length of received bytes</returns>
        public UInt16 SOCK_Receive(tSOCKET socket, IntPtr bufPtr, Int16 bufLen)
        {
            //      lock (lockerReceive)
            {
                return MySOCK_Receive(socket, bufPtr, bufLen);
            }
        }
        /*
        /// <summary>
        /// Receive a datagram (or ACK)
        /// </summary>
        /// <param name="bufPtr">the buffer to save the data</param>
        /// <param name="bufLen">buffer length</param>
        /// <returns>length of received bytes</returns>*/
    /*    public Int16 SOCK_Receive(IntPtr bufPtr, Int16 bufLen)
        {
            //      lock (lockerReceive)
            {
                return MySOCK_Receive(this.SOCKET, bufPtr, bufLen);
            }
        }*/
        #endregion

        #region SOCK_ReceiveFrom
        //INT16 _EXP_T_ SOCK_ReceiveFrom (tSOCKET socket, void *bufPtr, INT16 bufLen,
        //                       tSOCKaddr *adrPtr);
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "SOCK_ReceiveFrom", SetLastError = true)]
        private static extern Int16 MySOCK_ReceiveFrom(tSOCKET socket, IntPtr bufPtr, Int16 bufLen, ref tSOCKaddr adrPtr);
        /// <summary>
        /// Receive a datagram (or ACK)
        /// </summary>
        /// <param name="socket">the socket</param>
        /// <param name="bufPtr">the buffer to save the data</param>
        /// <param name="bufLen">buffer length</param>
        /// <param name="adrPtr">Source Address to receive from</param>
        /// <returns>length of received bytes</returns>
        public Int16 SOCK_ReceiveFrom(tSOCKET socket, IntPtr bufPtr, Int16 bufLen, ref tSOCKaddr adrPtr)
        {
            return MySOCK_ReceiveFrom(socket, bufPtr, bufLen, ref adrPtr);
        }
        #endregion

        #region SOCK_ReceiveTimed
        //INT16 _EXP_T_ SOCK_ReceiveTimed(tSOCKET socket, void *bufPtr, INT16 bufLen,
        //                      tSOCKaddr *adrPtr, DWORD timeout);
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "SOCK_ReceiveTimed", SetLastError = true)]
        private static extern UInt16 MySOCK_ReceiveTimed(tSOCKET socket, IntPtr bufPtr, Int16 bufLen, ref tSOCKaddr adrPtr, UInt16 timeOut);
        
        /// <summary>
        /// Receive a datagram
        /// </summary>
        /// <param name="socket">the socket</param>
        /// <param name="bufPtr">the buffer to save the data</param>
        /// <param name="bufLen">buffer length</param>
        /// <param name="adrPtr">Source Address to receive from</param>
        /// <param name="timeOut">timeout in ms</param>
        /// <returns>length of received bytes</returns>
        public UInt16 SOCK_ReceiveTimed(tSOCKET socket, IntPtr bufPtr, Int16 bufLen, ref tSOCKaddr adrPtr, UInt16 timeOut)
        {
            return MySOCK_ReceiveTimed(socket, bufPtr, bufLen, ref adrPtr, timeOut);
        }
        #endregion

        #region SOCK_SendTo
        //  SOCK_SendTo         : function (Socket:TSocket; Message:Pointer; MsgLen:Word; Addr:PSockAddr) : Int16; cdecl;
        //INT16 _EXP_T_ SOCK_SendTo      (tSOCKET socket, void *msgPtr, INT16 msgLen, tSOCKaddr *adrPtr);
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "SOCK_SendTo", SetLastError = true)]
        //private static extern Int16 MySOCK_SendTo(tSOCKET socket, out IntPtr msgPtr, Int16 MsgLen, out tSOCKaddr adrPtr);
        private static extern Int16 MySOCK_SendTo(tSOCKET socket, IntPtr msgPtr, Int16 MsgLen, ref tSOCKaddr adrPtr);
        /// <summary>
        /// Sends data to a specific receiver
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="msgPtr"></param>
        /// <param name="msgLen"></param>
        /// <param name="adrPtr"></param>
        /// <returns></returns>
        public Int16 SOCK_SendTo(tSOCKET socket, IntPtr msgPtr, Int16 msgLen, tSOCKaddr adrPtr)
        {
            return MySOCK_SendTo(socket, msgPtr, msgLen, ref adrPtr);
        }

        #endregion

        #region SOCK_Send
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "SOCK_Send", SetLastError = true)]
        private static extern Int16 MySOCK_Send(tSOCKET socket, IntPtr msgPtr, Int16 MsgLen);
        /// <summary>
        /// Sends data to the receiver which you have connected. 
        /// Only use if the socket is connected...
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="msgPtr"></param>
        /// <param name="msgLen"></param>
        /// <returns></returns>
        public Int16 SOCK_Send(tSOCKET socket, IntPtr msgPtr, Int16 msgLen)
        {
            return MySOCK_Send(socket, msgPtr, msgLen);
        }

        #endregion

        #region QNET_packAddr
        [DllImport("Qnetdll.dll", EntryPoint = PRE_EENTRY_POINT + "QNET_packAddr", SetLastError = true)]
        private static extern tQNETaddr MyQNET_packAddr(tQnetClass adrClass, tQNETaddr subnet, tQNETaddr station, tQNETaddr iqube);
        /// <summary>
        /// Convert an address to tQNETaddr structure
        /// </summary>
        /// <param name="adrClass"></param>
        /// <param name="subnet"></param>
        /// <param name="station"></param>
        /// <param name="iqube"></param>
        /// <returns>the corresponding tQNETaddr</returns>
        public tQNETaddr QNET_packAddr(tQnetClass adrClass, tQNETaddr subnet, tQNETaddr station, tQNETaddr iqube)
        {
            lock (locker)
            {
                return MyQNET_packAddr(adrClass, subnet, station, iqube);
            }
        }
        #endregion
    }
}
