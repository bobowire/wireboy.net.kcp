using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace wireboy.net.kcp.Models
{
    internal struct PacketSt
    {
        public enum StepEnum
        {
            SessionId = 1,
            Cmd = 2,
            IsFragment = 4,
            ReceiveBufferSize = 8,
            Timestamp = 16,
            SegmentNum = 32,
            Una = 64,
            DataLength = 128,
            Data = 256
        }
        public PacketSt Init()
        {
            Buffer = new List<byte>();
            Step = 0;
            SessionId = 0;
            Cmd = 0;
            IsFragment = 0;
            ReceiveBufferSize = 0;
            Timestamp = 0;
            SegmentNum = 0;
            Una = 0;
            DataLength = 0;
            Data = null;
            return this;
        }
        #region 用于读取数据
        public StepEnum Step;
        /// <summary>
        /// 源数据流
        /// </summary>
        internal BinaryReader sr;
        /// <summary>
        /// 上一次读取是否成功（false时，Parse方法会跳过执行）
        /// </summary>
        public bool IsLastSuccessed;
        /// <summary>
        /// 已经读取的数据
        /// </summary>
        public List<byte> Buffer;
        /// <summary>
        /// 需要读取的长度
        /// </summary>
        //public int RequestLength;
        #endregion
        /// <summary>
        /// 会话Id（conv）
        /// </summary>
        public uint SessionId;
        /// <summary>
        /// 命令（cmd）
        /// </summary>
        public byte Cmd;
        /// <summary>
        /// 是否分片（frg）
        /// </summary>
        public byte IsFragment;
        /// <summary>
        /// 目标接收缓冲区大小（wnd）
        /// </summary>
        public ushort ReceiveBufferSize;
        /// <summary>
        /// 时间（ts）
        /// </summary>
        public uint Timestamp;
        /// <summary>
        /// 分片序号（sn）
        /// </summary>
        public uint SegmentNum;
        /// <summary>
        /// 下一个数据包序列号（una）
        /// </summary>
        public uint Una;
        /// <summary>
        /// 数据长度（len）
        /// </summary>
        public uint DataLength;
        /// <summary>
        /// 数据（data）
        /// </summary>
        public byte[] Data;
    }
}
