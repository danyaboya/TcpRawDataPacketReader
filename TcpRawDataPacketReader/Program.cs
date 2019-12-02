using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TcpRawDataPacketReader
{
    class Program
    {
        private static string PathToFile = "C:\\Users\\victt\\Desktop\\blackbirdcap1"; // Path to the Raw Data file you have.

        private static string TextToBeProcessed = File.ReadAllText(PathToFile).Replace(Environment.NewLine, ""); // The unprocessed packets into one line to make it easier to read.
        private static int currOffset = 0; // The current offset of the @TextToBeProcessed.
        private static int maxHeaderOffset = 4; // The header of the packet (TCP Length should be an ushort so 4 chars on the Hex).

        private static List<string> TextProcessed = new List<string>(); // The processed packets from @TextToBeProcessed.

        static void Main(string[] args)
        {
            FixPackets();
            ReadPackets();
        }

        /// <summary>
        /// Where the magic begins.
        /// </summary>
        /// <example>
        /// byte ProtocolID = br.ReadByte();
        /// ushort msgType = br. br.ReadUInt16();
        /// </example>
        /// <param name="br"></param>
        private static void Read(BufferReader br)
        {

        }

        /// <summary>
        /// Fixes lines that comes with multiple packets instead of one packet per line.
        /// </summary>
        private static void FixPackets()
        {
            int LengthOfTextToBeProcessed = TextToBeProcessed.Length; // The Length of the @TextToBeProcessed so we don't have to call the .Length every time on the while.
            while (currOffset < LengthOfTextToBeProcessed) // While the offset is lesser than the @TextToBeProcessed, the capture is readable.
            {
                ushort packetLength = GetCorrentMaxLength(TextToBeProcessed.Substring(currOffset, maxHeaderOffset)); // Gets the length of the packet.
                if (currOffset + packetLength < LengthOfTextToBeProcessed) // The packet is complete, if not then you have a broken packet.
                    TextProcessed.Add(TextToBeProcessed.Substring(currOffset, packetLength)); // Adds the complete packet into the List so you can parse it later.
                currOffset += packetLength; // Add the length of the packet to the offset so the next time the loop runs, it reads the next packet.
            }
        }

        /// <summary>
        /// Parse the processed text into a readable buffer.
        /// </summary>
        private static void ReadPackets()
        {
            int count = 0;
            foreach (var packet in TextProcessed)
            {
                try
                {
                    byte[] incoming_msg = StringToByteArray(packet);

                    byte[] array_length = new byte[2];
                    array_length[0] = incoming_msg[0];
                    array_length[1] = incoming_msg[1];
                    ushort packet_length = BufferReader.ReadBufferSize(array_length);

                    List<byte> data = new List<byte>();
                    for (int i = 2; i < incoming_msg.Length; i++)
                    {
                        data.Add(incoming_msg[i]);
                    }

                    BufferReader br = new BufferReader(data.ToArray());
                    Read(br);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                count++;
            }
        }

        /// <summary>
        /// Reads and returns the Length of a hex line (converts into byte[] and reads the header)
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static ushort GetCorrentMaxLength(string line)
        {
            byte[] incoming_msg = new byte[2];
            incoming_msg = StringToByteArray(line);

            byte[] array_length = new byte[2];
            array_length[0] = incoming_msg[0];
            array_length[1] = incoming_msg[1];

            return (ushort)((BufferReader.ReadBufferSize(array_length) + 2) * 2);
        }

        /// <summary>
        /// Converts hex to byte[]
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
