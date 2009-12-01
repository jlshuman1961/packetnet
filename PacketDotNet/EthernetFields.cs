/*
This file is part of Packet.Net

Packet.Net is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Packet.Net is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Packet.Net.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */
using System;

namespace Packet.Net
{
    /// <summary> Ethernet protocol field encoding information.
    /// 
    /// </summary>
    public struct EthernetFields
    {
        /// <summary> Width of the ethernet type code in bytes.</summary>
        public readonly static int TypeLength = 2;

        /// <summary> Position of the destination MAC address within the ethernet header.</summary>
        public readonly static int DestinationMacPosition = 0;

        /// <summary> Position of the source MAC address within the ethernet header.</summary>
        public readonly static int SourceMacPosition;

        /// <summary> Position of the ethernet type field within the ethernet header.</summary>
        public readonly static int TypePosition;

        /// <summary> Total length of an ethernet header in bytes.</summary>
        public readonly static int HeaderLength; // == 14

        static EthernetFields()
        {
            SourceMacPosition = EthernetFields.MAC_ADDRESS_LENGTH;
            TypePosition = EthernetFields.MAC_ADDRESS_LENGTH * 2;
            HeaderLength = EthernetFields.ETH_CODE_POS + EthernetFields.ETH_CODE_LEN;
        }

        // size of an ethernet mac address in bytes
        public readonly static int MacAddressLength = 6;
    }
}
