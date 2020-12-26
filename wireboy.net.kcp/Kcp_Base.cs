using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using wireboy.net.kcp.Extention;
using wireboy.net.kcp.Models;

namespace wireboy.net.kcp
{
    public class Kcp_Base
    {
        /*
         * 1.数据包解析
         * 2.数据包封包
         * 3.数据包分片
         * 4.分片包组合
         * 5.ack包处理
         * 6.ack包发送
         * 7.数据包封包
         */
        KcpBufferQueue KcpQueue { set; get; }
        public Kcp_Base()
        {
            KcpQueue = new KcpBufferQueue(SendPacket);
        }
        protected virtual void SendPacket(byte[] data)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 是否Udp模式（Udp模式没有粘包）
        /// </summary>
        protected bool IsUdpMode { set; get; } = false;

        /// <summary>
        /// 数据发送方法（此数据已封包）
        /// </summary>
        public Action<byte[]> Func_Send;

        /// <summary>
        /// 接收到完整数据包（此数据已拆包）
        /// </summary>
        public Action<byte[]> Func_Receive;

        PacketSt Packet { set; get; } = new PacketSt().Init();
        public void ParsePacket(byte[] data)
        {
            data.ReadAsBinaryReader(sr =>
            {
                while (sr.PeekChar() >= 0)
                {
                    if (ReadOnePacket(sr, Packet))
                    {
                        if (!ExcuteCmd(Packet))
                        {
                            throw new NotSupportedException($"不支持的命令：{Packet.Cmd}");
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 读取一个完整数据包
        /// </summary>
        /// <param name="sr">数据流</param>
        /// <param name="packet">返回的结果</param>
        /// <returns>是否读取完整包</returns>
        private bool ReadOnePacket(BinaryReader sr, PacketSt packet)
        {
            //会话id->命令->是否分片->接收窗口大小->时间序列->序号->下一个可接收的序列号->数据长度->用户数据
            packet.BeginRead(sr).ReadSessionId().ReadCmd().ReadIsFragment()
                .ReadReceiveBufferSize().ReadTimestamp().ReadSegmentNum()
                .ReadUna().ReadDataLength().ReadData().EndRead();
            return packet.IsLastSuccessed;
        }

        /// <summary>
        /// 执行相应命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private bool ExcuteCmd(PacketSt packet)
        {
            bool ret = KcpQueue.ExcuteCmd(packet);
            //处理完后，重置数据包
            Packet.Init();
            return ret;
        }
    }
}
