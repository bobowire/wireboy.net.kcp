using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using wireboy.net.kcp.Models;

namespace wireboy.net.kcp.Extention
{
    internal static class PacketStEx
    {
        internal static PacketSt BeginRead(this PacketSt _this, BinaryReader sr)
        {
            _this.IsLastSuccessed = true;
            _this.sr = sr;
            return _this;
        }
        internal static PacketSt EndRead(this PacketSt _this)
        {
            _this.sr = null;

            return _this;
        }
        private static PacketSt ReadData(this PacketSt _this, Func<BinaryReader, bool> callBack)
        {
            if (_this.IsLastSuccessed)
            {
                _this.IsLastSuccessed = false;
                if (_this.sr.PeekChar() >= 0)
                {
                    _this.IsLastSuccessed = callBack(_this.sr);
                }
            }
            return _this;
        }
        private static bool GetValue(this PacketSt _this, BinaryReader sr, int length, Action<byte[]> action)
        {
            bool ret = false;
            //计算已经读取的数据
            length = length - _this.Buffer.Count;
            //计算流中剩余的数据
            long srLength = sr.BaseStream.Length - sr.BaseStream.Position;
            if (srLength >= length)
            {
                _this.Buffer.AddRange(sr.ReadBytes(length));
                action(_this.Buffer.ToArray());
                //缓存用完后要清空，避免影响下一次使用
                _this.Buffer.Clear();
                ret = true;
            }
            else
            {
                //如果剩余长度不够，则加入缓存
                _this.Buffer.AddRange(sr.ReadBytes((int)srLength));
            }
            return ret;
        }

        internal static bool MatchStep(this PacketSt _this, PacketSt.StepEnum step)
        {
            return (_this.Step & step) > 0;
        }

        internal static PacketSt ReadSessionId(this PacketSt _this)
        {
            //如果已经赋值，则不再处理
            if (_this.MatchStep(PacketSt.StepEnum.SessionId))
            {
                _this.ReadData(sr =>
                {
                    return _this.GetValue(sr, sizeof(uint), data =>
                    {
                        _this.SessionId = BitConverter.ToUInt32(data, 0);
                        _this.Step |= PacketSt.StepEnum.SessionId;
                    });
                });
            }
            return _this;
        }
        internal static PacketSt ReadCmd(this PacketSt _this)
        {
            if (_this.MatchStep(PacketSt.StepEnum.Cmd))
            {
                _this.ReadData(sr =>
                {
                    return _this.GetValue(sr, sizeof(byte), data =>
                    {
                        _this.Cmd = data[0];
                        _this.Step |= PacketSt.StepEnum.Cmd;
                    });
                });
            }
            return _this;
        }
        internal static PacketSt ReadIsFragment(this PacketSt _this)
        {
            if (_this.MatchStep(PacketSt.StepEnum.IsFragment))
            {
                _this.ReadData(sr =>
                {
                    return _this.GetValue(sr, sizeof(byte), data =>
                    {
                        _this.IsFragment = data[0];
                        _this.Step |= PacketSt.StepEnum.IsFragment;
                    });
                });
            }
            return _this;
        }
        internal static PacketSt ReadReceiveBufferSize(this PacketSt _this)
        {
            if (_this.MatchStep(PacketSt.StepEnum.ReceiveBufferSize))
            {
                _this.ReadData(sr =>
                {
                    return _this.GetValue(sr, sizeof(ushort), data =>
                    {
                        _this.ReceiveBufferSize = BitConverter.ToUInt16(data, 0);
                        _this.Step |= PacketSt.StepEnum.ReceiveBufferSize;
                    });
                });
            }
            return _this;
        }
        internal static PacketSt ReadTimestamp(this PacketSt _this)
        {
            if (_this.MatchStep(PacketSt.StepEnum.Timestamp))
            {
                _this.ReadData(sr =>
                {
                    return _this.GetValue(sr, sizeof(uint), data =>
                    {
                        _this.Timestamp = BitConverter.ToUInt32(data, 0);
                        _this.Step |= PacketSt.StepEnum.Timestamp;
                    });
                });
            }
            return _this;
        }
        internal static PacketSt ReadSegmentNum(this PacketSt _this)
        {
            if (_this.MatchStep(PacketSt.StepEnum.SegmentNum))
            {
                _this.ReadData(sr =>
                {
                    return _this.GetValue(sr, sizeof(uint), data =>
                    {
                        _this.SegmentNum = BitConverter.ToUInt32(data, 0);
                        _this.Step |= PacketSt.StepEnum.SegmentNum;
                    });
                });
            }
            return _this;
        }
        internal static PacketSt ReadUna(this PacketSt _this)
        {
            if (_this.MatchStep(PacketSt.StepEnum.Una))
            {
                _this.ReadData(sr =>
                {
                    return _this.GetValue(sr, sizeof(uint), data =>
                    {
                        _this.Una = BitConverter.ToUInt32(data, 0);
                        _this.Step |= PacketSt.StepEnum.Una;
                    });
                });
            }
            return _this;
        }
        internal static PacketSt ReadDataLength(this PacketSt _this)
        {
            if (_this.MatchStep(PacketSt.StepEnum.DataLength))
            {
                _this.ReadData(sr =>
                {
                    return _this.GetValue(sr, sizeof(uint), data =>
                    {
                        _this.DataLength = BitConverter.ToUInt32(data, 0);
                        _this.Step |= PacketSt.StepEnum.DataLength;
                    });
                });
            }
            return _this;
        }
        internal static PacketSt ReadData(this PacketSt _this)
        {
            if (_this.MatchStep(PacketSt.StepEnum.Data))
            {
                _this.ReadData(sr =>
                {
                    return _this.GetValue(sr, (int)_this.DataLength, data =>
                    {
                        data.CopyTo(_this.Data, 0);
                        _this.Step |= PacketSt.StepEnum.Data;
                    });
                });
            }
            return _this;
        }
    }
}
