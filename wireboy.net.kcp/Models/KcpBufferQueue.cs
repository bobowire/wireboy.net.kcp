using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace wireboy.net.kcp.Models
{
    internal class KcpBufferQueue
    {
        public Action<byte[]> SendPacket;
        int ReSendCount = 3;
        int TimeOutCount = 3;
        /// <summary>
        /// 待发送队列最大值
        /// </summary>
        public int MaxSendSize { set; get; } = 1024 * 1024 * 1;

        /// <summary>
        /// 待发送队列
        /// </summary>
        ConcurrentQueue<QueueItem> SendQueueBuffer { set; get; } = new ConcurrentQueue<QueueItem>();
        /// <summary>
        /// 接收队列
        /// </summary>
        ConcurrentQueue<byte[]> ReceiveQueueBuffer { set; get; } = new ConcurrentQueue<byte[]>();
        /// <summary>
        /// Ack确认队列
        /// </summary>
        ConcurrentDictionary<uint, QueueItem> AckQueueBuffer { set; get; } = new ConcurrentDictionary<uint, QueueItem>();

        public KcpBufferQueue(Action<byte[]> sendPacket)
        {
            SendPacket = sendPacket;
        }

        public byte[] GetSendPacket()
        {
            if (SendQueueBuffer.TryDequeue(out QueueItem item))
            {
                if (AckQueueBuffer.TryAdd(item.Num, item))
                {
                    throw new ArgumentException($"数据包确认号重复");
                }
                return item.Data;
            }
            return null;
        }

        /// <summary>
        /// 处理数据包
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public bool ExcuteCmd(PacketSt packet)
        {
            bool ret = false;
            switch (packet.Cmd)
            {
                case GlobalConst.IKCP_CMD_ACK:
                    {
                        UpdateAckQueue(packet);
                        ret = true;
                        break;
                    }
                case GlobalConst.IKCP_CMD_PUSH:
                    {
                        ret = true;
                        //todo:接收数据，并发送确认包
                        break;
                    }
                case GlobalConst.IKCP_CMD_WASK:
                    {
                        ret = true;
                        //todo:直接调用方法发送数据
                        break;
                    }
                case GlobalConst.IKCP_CMD_WINS:
                    {
                        ret = true;
                        //todo:直接调用方法发送数据
                        break;
                    }
            }
            return ret;
        }

        /// <summary>
        /// 更新ack队列
        /// </summary>
        /// <param name="packet"></param>
        public void UpdateAckQueue(PacketSt packet)
        {
            if (AckQueueBuffer.TryRemove(packet.Una - 1, out QueueItem item))
            {
                //说明ack数据包有效，开始处理确认队列
                List<uint> keys = AckQueueBuffer.Keys.Where(t => t < packet.Una).ToList();
                foreach (uint key in keys)
                {
                    if (AckQueueBuffer.TryGetValue(key, out QueueItem ackItem))
                    {
                        //判断是否需要重新发包
                        if (ackItem.TimeoutNum > TimeOutCount)
                        {
                            //判断是否超过重发包次数
                            if (ackItem.ReSendCount < ReSendCount)
                            {
                                //开始重新发包
                                SendPacket(ackItem.Data);
                                ackItem.ReSendCount++;
                            }
                            //重新计数
                            ackItem.TimeoutNum = 0;
                        }
                        else
                        {
                            ackItem.TimeoutNum++;
                        }
                    }
                }
            }
        }

    }
}
