﻿/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// An ICMP packet.
    /// See http://en.wikipedia.org/wiki/ICMPv6
    /// </summary>
    public sealed class IcmpV6Packet : InternetPacket
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// Used to prevent a recursive stack overflow
        /// when recalculating in UpdateCalculatedValues()
        /// </summary>
        private bool _skipUpdating;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public IcmpV6Packet(ByteArraySegment byteArraySegment)
        {
            Log.Debug("");

            Header = new ByteArraySegment(byteArraySegment);
        }

        /// <summary>
        /// Constructor with parent packet
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public IcmpV6Packet(ByteArraySegment byteArraySegment, Packet parentPacket) : this(byteArraySegment)
        {
            ParentPacket = parentPacket;
        }

        /// <summary>
        /// Checksum value
        /// </summary>
        public ushort Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + IcmpV6Fields.ChecksumPosition);
            set
            {
                var v = value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + IcmpV6Fields.ChecksumPosition);
            }
        }

        /// <summary>Fetch the ICMP code </summary>
        public byte Code
        {
            get => Header.Bytes[Header.Offset + IcmpV6Fields.CodePosition];
            set => Header.Bytes[Header.Offset + IcmpV6Fields.CodePosition] = value;
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.LightBlue;

        /// <summary>
        /// The Type value
        /// </summary>
        public IcmpV6Type Type
        {
            get => (IcmpV6Type) Header.Bytes[Header.Offset + IcmpV6Fields.TypePosition];
            set => Header.Bytes[Header.Offset + IcmpV6Fields.TypePosition] = (byte) value;
        }

        /// <summary>
        /// Recalculate the checksum
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if (_skipUpdating)
                return;


            // prevent us from entering this routine twice
            // by setting this flag, the act of retrieving the Bytes
            // property will cause this routine to be called which will
            // retrieve Bytes recursively and overflow the stack
            _skipUpdating = true;

            // start with this packet with a zeroed out checksum field
            Checksum = 0;

            var dataToChecksum = BytesSegment;
            var ipv6Parent = ParentPacket as IPv6Packet;

            Checksum = (ushort) ChecksumUtils.OnesComplementSum(dataToChecksum, ipv6Parent?.GetPseudoIPHeader(dataToChecksum.Length) ?? Array.Empty<byte>());

            // clear the skip variable
            _skipUpdating = false;
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if (outputFormat is StringOutputType.Colored or StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                {
                    // build the output string
                    buffer.AppendFormat("{0}[IcmpV6Packet: Type={2}, Code={3}]{1}",
                                        color,
                                        colorEscape,
                                        Type,
                                        Code);

                    break;
                }
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                {
                    // collect the properties and their value
                    var properties = new Dictionary<string, string>
                    {
                        { "type", Type + " (" + (int) Type + ")" },
                        { "code", Code.ToString() },
                        // TODO: Implement a checksum verification for ICMPv6
                        { "checksum", "0x" + Checksum.ToString("x") }
                    };
                    // TODO: Implement ICMPv6 Option fields here?

                    // calculate the padding needed to right-justify the property names
                    var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("ICMP:  ******* ICMPv6 - \"Internet Control Message Protocol (Version 6)\"- offset=? length=" + TotalPacketLength);
                    buffer.AppendLine("ICMP:");
                    foreach (var property in properties)
                    {
                        buffer.AppendLine("ICMP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                    }

                    buffer.AppendLine("ICMP:");
                    break;
                }
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}