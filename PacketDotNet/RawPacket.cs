using System;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Raw packet as loaded from a pcap device or file
    /// </summary>
    public class RawPacket
    {
        /// <value>
        /// Link layer from which this packet was captured
        /// </value>
        public LinkLayers LinkLayerType
        {
            get;
            set;
        }

        /// <value>
        /// The unix timeval when the packet was created
        /// </value>
        public PosixTimeval Timeval
        {
            get;
            set;
        }

        /// <summary> Fetch data portion of the packet.</summary>
        public virtual byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LinkLayerType">
        /// A <see cref="LinkLayers"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="Data">
        /// A <see cref="System.Byte"/>
        /// </param>
        public RawPacket(LinkLayers LinkLayerType,
                         PosixTimeval Timeval,
                         byte[] Data)
        {
            this.LinkLayerType = LinkLayerType;
            this.Timeval = Timeval;
            this.Data = Data;
        }

        /// <value>
        /// Color used when generating the text description of a packet
        /// </value>
        public virtual System.String Color
        {
            get
            {
                return AnsiEscapeSequences.Black;
            }
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            string color = "";
            string colorEscape = "";

            if(outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if(outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                buffer.AppendFormat("[{0}RawPacket{1}: LinkLayerType={2}, Timeval={3}, Data={4}]",
                    color,
                    colorEscape,
                    LinkLayerType,
                    Timeval,
                    Data);
            }

            // TODO: Add verbose string support here
            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                throw new NotImplementedException("The following feature is under developemnt");
            }

            return buffer.ToString();
        }
    }
}
