using CryptoDoge.ParserService.Models;
using CryptoDoge.Shared;
using Microsoft.Extensions.Logging;
using SpanJson.Resolvers;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CryptoDoge.ParserService
{
    public class ParserService : IParserService
    {
        private const string parserPath = "native/parser.dll";
        private readonly ILogger<ParserService> logger;

        #region Native Bindings
        [DllImport(parserPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr importCaffAsJsonFromString(IntPtr caffBytes, long size);

        [DllImport(parserPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr importCaffAsJson(string filepath);

        [DllImport(parserPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern void freeNativeMem(IntPtr address);
        #endregion

        public ParserService(ILogger<ParserService> logger)
        {
            this.logger = logger;
        }

        public Caff GetCaffFromFile(string filePath)
        {
            using var loggerScope = new LoggerScope(logger);
            if (!File.Exists(filePath))
            {
                var ex = new FileNotFoundException($"{nameof(GetCaffFromFile)}: Caff file not found with path: {filePath}");
                logger.LogError(ex, ex.Message);
                throw ex;
            }

            IntPtr textPtr = IntPtr.Zero;
            try
            {
                textPtr = importCaffAsJson(filePath);
                string json = Marshal.PtrToStringAnsi(textPtr);
                return SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Caff>(Encoding.UTF8.GetBytes(json));
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
            finally
            {
                if (textPtr != IntPtr.Zero)
                {
                    freeNativeMem(textPtr);
                }
            }
        }

        public Caff GetCaffFromMemoryStream(MemoryStream caffMemoryStream)
        {
            using var loggerScope = new LoggerScope(logger);
            var buffer = caffMemoryStream.ToArray();
            return GetCaffFromByteArray(buffer);
        }

        public Caff GetCaffFromByteArray(byte[] buffer)
        {
            using var loggerScope = new LoggerScope(logger);
            if (buffer.Length == 0)
            {
                var ex = new ArgumentException($"{nameof(GetCaffFromByteArray)}: buffer was empty.");
                logger.LogError(ex, ex.Message);
                throw ex;
            }

            IntPtr pnt = IntPtr.Zero;
            IntPtr textPtr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(buffer[0]) * buffer.Length;
                pnt = Marshal.AllocHGlobal(size);

                Marshal.Copy(buffer, 0, pnt, buffer.Length);

                textPtr = importCaffAsJsonFromString(pnt, buffer.Length);
                string json = Marshal.PtrToStringAnsi(textPtr);

                return SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Caff>(Encoding.UTF8.GetBytes(json));
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
            finally
            {
                Marshal.FreeHGlobal(pnt);
                if (textPtr != IntPtr.Zero)
                {
                    freeNativeMem(textPtr);
                }
            }
        }
    }
}
