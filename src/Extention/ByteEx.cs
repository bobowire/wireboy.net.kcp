using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace wireboy.net.kcp.Extention
{
    internal static class ByteEx
    {
        /// <summary>
        /// 将byte数组转为数据流的方式读取
        /// </summary>
        /// <param name="data">byte数组</param>
        /// <param name="func">回调方法</param>
        internal static void ReadAsBinaryReader(this byte[] _, Action<BinaryReader> callBack)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryReader sr = new BinaryReader(ms))
                {
                    callBack(sr);
                }
            }
        }
    }
}
