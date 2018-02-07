/*
 * Copyright (c) 2015, InWorldz Halcyon Developers
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice, this
 *     list of conditions and the following disclaimer.
 * 
 *   * Redistributions in binary form must reproduce the above copyright notice,
 *     this list of conditions and the following disclaimer in the documentation
 *     and/or other materials provided with the distribution.
 * 
 *   * Neither the name of halcyon nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Linq;
using System.Net;
using ProtoBuf;

namespace InWorldz.Data.Assets.Stratus {
	/// <summary>
	/// An asset in protobuf format for storage inside a stratus engine
	/// </summary>
	[ProtoContract]
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
	// As this is a wierd class that is used more like a strcut than a class, GetHashCode doesn't make much sense since there is nothing immutable in this.
	// Maybe at some point a way to work with Protobuf's needs and have encapsulation will be figured out.
	public sealed class StratusAsset : IEquatable<StratusAsset> {
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
		/// <summary>
		/// Size of the packet header
		/// </summary>
		private const short HEADER_SIZE = 39;
		/// <summary>
		/// location of the type tag
		/// </summary>
		private const short TYPE_TAG_LOC = 32;
		/// <summary>
		/// location of the local tag
		/// </summary>
		private const short LOCAL_TAG_LOC = 33;
		/// <summary>
		/// Location of the temporary tag
		/// </summary>
		private const short TEMPORARY_TAG_LOC = 34;
		/// <summary>
		/// Location of the create time tag
		/// </summary>
		private const short CREATE_TIME_TAG_LOC = 35;
		/// <summary>
		/// Location of the size of the name field
		/// </summary>
		private const short NAME_SIZE_TAG_LOC = 39;
		/// <summary>
		/// Length of the UUID byte array
		/// </summary>
		private const short UUID_LEN = 32;

		[ProtoMember(1)]
		public Guid Id;

		[ProtoMember(2)]
		public sbyte Type;

		[ProtoMember(3)]
		public bool Local;

		[ProtoMember(4)]
		public bool Temporary;

		[ProtoMember(5)]
		public DateTime CreateTime;

		[ProtoMember(6)]
		public string Name;

		[ProtoMember(7)]
		public string Description;

		[ProtoMember(8)]
		public byte[] Data;

		[ProtoMember(9)]
		public uint StorageFlags;

		public static string GetProto() {
			return Serializer.GetProto<StratusAsset>();
		}

		#region Conversions

		public static StratusAsset FromWHIPAsset(Whip.Client.Asset whipAsset) {
			return new StratusAsset {
				Id = Guid.Parse(whipAsset.Uuid),
				Type = (sbyte)whipAsset.Type,
				Local = whipAsset.Local,
				Temporary = whipAsset.Temporary,
				CreateTime = UnixToUTCDateTime(whipAsset.CreateTime),
				Name = whipAsset.Name,
				Description = whipAsset.Description,
				Data = whipAsset.Data,
			};
		}

		public static Whip.Client.Asset ToWHIPAsset(StratusAsset asset) {
			return new Whip.Client.Asset(
				asset.Id.ToString(),
				(byte)asset.Type,
				asset.Local,
				asset.Temporary,
				(int)UTCDateTimeToEpoch(asset.CreateTime), // At some point this'll need to be corrected to a 64bit timestamp...
				asset.Name,
				asset.Description,
				asset.Data
			);
		}


		public static StratusAsset FromWHIPSerialized(byte[] data) {
			if (data.Length < HEADER_SIZE) {
				throw new ArgumentOutOfRangeException(nameof(data), "Not enough data given to deserialize an asset.");
			}

			var createUnixTime = NTOHL(data, CREATE_TIME_TAG_LOC);

			var asset = new StratusAsset {
				Id = ByteStringToGuid(data, 0),
				Type = (sbyte)data[TYPE_TAG_LOC],
				Local = data[LOCAL_TAG_LOC] == 1,
				Temporary = data[TEMPORARY_TAG_LOC] == 1,
				CreateTime = UnixToUTCDateTime(createUnixTime),
			};

			// Now the dynamic sized fields
			if (NAME_SIZE_TAG_LOC < data.Length) {
				var nameFieldSize = data[NAME_SIZE_TAG_LOC];
				var nameDataLoc = NAME_SIZE_TAG_LOC + 1;
				if (nameFieldSize > 0 && nameDataLoc < data.Length) {
					asset.Name = System.Text.Encoding.UTF8.GetString(data, nameDataLoc, Math.Min(nameFieldSize, data.Length - nameDataLoc));
				}
				else {
					asset.Name = string.Empty;
				}

				//the description field
				var descSizeFieldLoc = NAME_SIZE_TAG_LOC + nameFieldSize + 1;
				if (descSizeFieldLoc < data.Length) {
					var descFieldSize = data[descSizeFieldLoc];
					var descDataLoc = descSizeFieldLoc + 1;
					if (descFieldSize > 0 && descDataLoc < data.Length) {
						asset.Description = System.Text.Encoding.UTF8.GetString(data, descDataLoc, Math.Min(descFieldSize, data.Length - descDataLoc));
					}
					else {
						asset.Description = string.Empty;
					}


					//finally, get the location of the data and it's size
					var dataSizeFieldLoc = descSizeFieldLoc + descFieldSize + 1;
					if (dataSizeFieldLoc + 4 < data.Length) {
						var dataLoc = dataSizeFieldLoc + 4;
						var dataSize = Math.Min(NTOHL(data, dataSizeFieldLoc), data.Length - dataLoc);

						//create the array now so that it will be shared between all reqestors
						if (dataSize > 0) {
							asset.Data = new byte[dataSize];
							Buffer.BlockCopy(data, dataLoc, asset.Data, 0, dataSize);
						}
						else {
							asset.Data = new byte[0];
						}
					}
				}
			}

			return asset;
		}

		public static byte[] ToWHIPSerialized(StratusAsset asset) {
			byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(asset.Name);
			byte[] descBytes = System.Text.Encoding.UTF8.GetBytes(asset.Description);

			if (nameBytes.Length > 255) {
				throw new ArgumentException($"Serialized asset name would be too long after encoding {asset.Name} {asset.Id}");
			}

			if (descBytes.Length > 255) {
				throw new ArgumentException($"Serialized asset description would be too long after encoding {asset.Description} {asset.Id}");
			}

			//see the packet diagram to understand where the size calculation is coming from
			var retArray = new byte[HEADER_SIZE + 1 + nameBytes.Length + 1 + descBytes.Length + 4 + asset.Data.Length];

			Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes(asset.Id.ToString("N")), 0, retArray, 0, UUID_LEN);
			retArray[TYPE_TAG_LOC] = (byte)asset.Type;
			retArray[LOCAL_TAG_LOC] = (byte)(asset.Local ? 1 : 0);
			retArray[TEMPORARY_TAG_LOC] = (byte)(asset.Temporary ? 1 : 0);
			Buffer.BlockCopy(HTONL((int)UTCDateTimeToEpoch(asset.CreateTime)), 0, retArray, CREATE_TIME_TAG_LOC, 4);

			var appendLoc = NAME_SIZE_TAG_LOC;
			retArray[appendLoc] = (byte)nameBytes.Length;
			Buffer.BlockCopy(nameBytes, 0, retArray, ++appendLoc, (byte)nameBytes.Length);
			appendLoc += (byte)nameBytes.Length;

			retArray[appendLoc] = (byte)descBytes.Length;
			Buffer.BlockCopy(descBytes, 0, retArray, ++appendLoc, (byte)descBytes.Length);
			appendLoc += (byte)descBytes.Length;

			Buffer.BlockCopy(HTONL(asset.Data.Length), 0, retArray, appendLoc, 4);
			appendLoc += 4;
			Buffer.BlockCopy(asset.Data, 0, retArray, appendLoc, asset.Data.Length);

			return retArray;
		}

		#endregion

		#region Conversion Utilities

		private static Guid ByteStringToGuid(byte[] bytes, int offset) {
			var idbytes = new byte[UUID_LEN];
			Buffer.BlockCopy(bytes, offset, idbytes, 0, UUID_LEN);
			return Guid.Parse(System.Text.Encoding.ASCII.GetString(idbytes));
		}

		/// <summary>
		/// Network to host long.  Given an array of bytes converts to a host long
		/// </summary>
		/// <param name="bytes">Array of bytes from the network</param>
		/// <param name="offset">offset in the array to start looking</param>
		/// <returns>A host endian 32 bit number</returns>
		private static int NTOHL(byte[] bytes, int offset) {
			return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, offset));
		}

		/// <summary>
		/// Host to network long.  Given a 32 bit integer, converts to an array of bytes
		/// </summary>
		/// <param name="number">The number to convert</param>
		/// <returns>An array of bytes in network order</returns>
		private static byte[] HTONL(int number) {
			return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(number));
		}

		// Unix-epoch starts at January 1st 1970, 00:00:00 UTC. And all our times in the server are (or at least should be) in UTC.
		private static readonly DateTime UNIX_EPOCH = DateTime.ParseExact("1970-01-01 00:00:00 +0", "yyyy-MM-dd hh:mm:ss z", System.Globalization.DateTimeFormatInfo.InvariantInfo).ToUniversalTime();

		private static DateTime UnixToUTCDateTime(long seconds) {
			return UNIX_EPOCH.AddSeconds(seconds);
		}

		private static long UTCDateTimeToEpoch(DateTime timestamp) {
			return (long)(timestamp.Subtract(UNIX_EPOCH)).TotalSeconds;
		}

		#endregion

		#region Comparison

		public override bool Equals(object obj) {
			return Equals(obj as StratusAsset);
		}

		public bool Equals(StratusAsset other) {
			return
				Id == other.Id &&
				Type == other.Type &&
				Local == other.Local &&
				Temporary == other.Temporary &&
				CreateTime == other.CreateTime &&
				Name == other.Name &&
				Description == other.Description &&
				StorageFlags == other.StorageFlags &&
				(
					(Data == null && other.Data == null)
					||
					(other.Data != null && (Data?.SequenceEqual(other.Data) ?? false))
				)
			;
		}

		#endregion
	}
}
