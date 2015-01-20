using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using ChinaStockQuotes.Configuration;
using SnappyPI;

namespace ChinaStockQuotes.Extensions
{
    public static class BytesExtension
    {
        public static string DeCompress(this byte[] bytesToDecompress)
        {
            byte[] outArr = null;

            byte[] writeData = new byte[4096];

            Stream s2 = new GZipStream(new MemoryStream(bytesToDecompress), CompressionMode.Decompress);
            MemoryStream outStream = new MemoryStream();

            while (true)
            {
                int size = s2.Read(writeData, 0, writeData.Length);
                if (size > 0)
                {
                    outStream.Write(writeData, 0, size);
                }
                else
                {
                    break;
                }
            }
            s2.Close();
            outArr = outStream.ToArray();
            outStream.Close();

            return ConfigurationData.GetDefaultEncoding.GetString(outArr);
        }

        public static string DeflateDecompress(this byte[] bytesToDecompress)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(bytesToDecompress, 0, bytesToDecompress.Length);
                ms.Position = 0;
                using (var stream = new System.IO.Compression.DeflateStream(ms, CompressionMode.Decompress))
                {
                    stream.Flush();
                    const int nSize = 16 * 1024 + 256; //假设字符串不会超过16K
                    byte[] decompressBuffer = new byte[nSize];
                    int nSizeIncept = stream.Read(decompressBuffer, 0, nSize);
                    stream.Close();
                    return ConfigurationData.GetDefaultEncoding.GetString(decompressBuffer, 0, nSizeIncept);   //转换为普通的字符串
                }
            }
        }

        public static string SnappyDecompress(this byte[] bytesToDecompress)
        {
            var decompressed = SnappyCodec.Uncompress(bytesToDecompress, 0, bytesToDecompress.Length);

            return ConfigurationData.GetDefaultEncoding.GetString(decompressed, 0, decompressed.Length);   //转换为普通的字符串
        }
    }
}
