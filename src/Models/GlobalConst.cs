using System;
using System.Collections.Generic;
using System.Text;

namespace wireboy.net.kcp.Models
{
    public class GlobalConst
    {
        public const UInt32 IKCP_RTO_NDL = 30;        // no delay min rto
        public const UInt32 IKCP_RTO_MIN = 100;       // normal min rto
        public const UInt32 IKCP_RTO_DEF = 200;
        public const UInt32 IKCP_RTO_MAX = 60000;
        /// <summary>
        /// 传输数据
        /// </summary>
        public const byte IKCP_CMD_PUSH = 81;       // cmd: push data
        /// <summary>
        /// ack确认包
        /// </summary>
        public const byte IKCP_CMD_ACK = 82;        // cmd: ack
        /// <summary>
        /// 询问接收区缓冲大小
        /// </summary>
        public const byte IKCP_CMD_WASK = 83;       // cmd: window probe (ask)
        /// <summary>
        /// 告知接收区缓冲大小
        /// </summary>
        public const byte IKCP_CMD_WINS = 84;       // cmd: window size (tell)
        public const UInt32 IKCP_ASK_SEND = 1;        // need to send IKCP_CMD_WASK
        public const UInt32 IKCP_ASK_TELL = 2;        // need to send IKCP_CMD_WINS
        public const UInt32 IKCP_WND_SND = 32;
        public const UInt32 IKCP_WND_RCV = 128;       // must >= max fragment size
        public const UInt32 IKCP_MTU_DEF = 1400;
        public const UInt32 IKCP_ACK_FAST = 3;
        public const UInt32 IKCP_INTERVAL = 100;
        public const UInt32 IKCP_OVERHEAD = 24;
        public const UInt32 IKCP_DEADLINK = 20;
        public const UInt32 IKCP_THRESH_INIT = 2;
        public const UInt32 IKCP_THRESH_MIN = 2;
        public const UInt32 IKCP_PROBE_INIT = 7000;       // 7 secs to probe window size
        public const UInt32 IKCP_PROBE_LIMIT = 120000;    // up to 120 secs to probe window
        public const UInt32 IKCP_FASTACK_LIMIT = 5;		// max times to trigger fastack
    }
}
