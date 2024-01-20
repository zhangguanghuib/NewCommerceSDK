using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
    public static class SocketExtension
    {
        /// <summary>
        /// Enable keep-alive pings on the socket
        /// </summary>
        /// <param name="s">this socket</param>
        /// <param name="enable">true to enable keep-alive, false to disable</param>
        /// <param name="time">the time in milliseconds between keep-alive packets</param>
        /// <param name="interval">if no response to a keep-alive, how often a packet will be resent. (e.g. 1000)</param>
        /// <returns></returns>
        public static bool SetKeepAlive(this System.Net.Sockets.Socket s, bool enable, ulong time, ulong interval)
        {
            var bytesperlong = 4;
            var bitsperbyte = 8;
            try
            {
                // resulting structure
                var SIO_KEEPALIVE_VALS = new byte[3 * bytesperlong];
                // array to hold input values
                var input = new ulong[3];
                // put input arguments in input array
                input[0] = enable ? 1UL : 0UL; // enable disable keep-alive
                input[1] = (time); // time millis
                input[2] = (interval); // interval millis
                // pack input into byte struct
                for (int i = 0; i < input.Length; i++)
                {
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 3] = (byte)(input[i] >> ((bytesperlong - 1) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 2] = (byte)(input[i] >> ((bytesperlong - 2) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 1] = (byte)(input[i] >> ((bytesperlong - 3) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 0] = (byte)(input[i] >> ((bytesperlong - 4) * bitsperbyte) & 0xff);
                }
                // create bytestruct for result (bytes pending on server socket)
                byte[] result = BitConverter.GetBytes(0);
                // write SIO_VALS to Socket IOControl
                s.IOControl(System.Net.Sockets.IOControlCode.KeepAliveValues, SIO_KEEPALIVE_VALS, result);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
