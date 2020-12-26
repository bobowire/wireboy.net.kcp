using System;
using System.Collections.Generic;
using System.Text;

namespace wireboy.net.kcp.Models
{
    public struct QueueItem
    {
        /// <summary>
        /// 未收到ack包的次数（超时则设置为50，使其进入重发包逻辑）
        /// </summary>
        public byte TimeoutNum;
        /// <summary>
        /// 已重新发送的次数
        /// </summary>
        public byte ReSendCount;
        /// <summary>
        /// 数据包数据
        /// </summary>
        public byte[] Data;
        /// <summary>
        /// 序号（当前数据包确认号）
        /// </summary>
        public uint Num;

    }
}
