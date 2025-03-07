namespace MessagePack
{
	public static class MessagePackCode
	{
		public const byte MinFixInt = 0;

		public const byte MaxFixInt = 127;

		public const byte MinFixMap = 128;

		public const byte MaxFixMap = 143;

		public const byte MinFixArray = 144;

		public const byte MaxFixArray = 159;

		public const byte MinFixStr = 160;

		public const byte MaxFixStr = 191;

		public const byte Nil = 192;

		public const byte NeverUsed = 193;

		public const byte False = 194;

		public const byte True = 195;

		public const byte Bin8 = 196;

		public const byte Bin16 = 197;

		public const byte Bin32 = 198;

		public const byte Ext8 = 199;

		public const byte Ext16 = 200;

		public const byte Ext32 = 201;

		public const byte Float32 = 202;

		public const byte Float64 = 203;

		public const byte UInt8 = 204;

		public const byte UInt16 = 205;

		public const byte UInt32 = 206;

		public const byte UInt64 = 207;

		public const byte Int8 = 208;

		public const byte Int16 = 209;

		public const byte Int32 = 210;

		public const byte Int64 = 211;

		public const byte FixExt1 = 212;

		public const byte FixExt2 = 213;

		public const byte FixExt4 = 214;

		public const byte FixExt8 = 215;

		public const byte FixExt16 = 216;

		public const byte Str8 = 217;

		public const byte Str16 = 218;

		public const byte Str32 = 219;

		public const byte Array16 = 220;

		public const byte Array32 = 221;

		public const byte Map16 = 222;

		public const byte Map32 = 223;

		public const byte MinNegativeFixInt = 224;

		public const byte MaxNegativeFixInt = byte.MaxValue;

		private static readonly MessagePackType[] typeLookupTable;

		private static readonly string[] formatNameTable;

		static MessagePackCode()
		{
			typeLookupTable = new MessagePackType[256];
			formatNameTable = new string[256];
			for (int i = 0; i <= 127; i++)
			{
				typeLookupTable[i] = MessagePackType.Integer;
				formatNameTable[i] = "positive fixint";
			}
			for (int j = 128; j <= 143; j++)
			{
				typeLookupTable[j] = MessagePackType.Map;
				formatNameTable[j] = "fixmap";
			}
			for (int k = 144; k <= 159; k++)
			{
				typeLookupTable[k] = MessagePackType.Array;
				formatNameTable[k] = "fixarray";
			}
			for (int l = 160; l <= 191; l++)
			{
				typeLookupTable[l] = MessagePackType.String;
				formatNameTable[l] = "fixstr";
			}
			typeLookupTable[192] = MessagePackType.Nil;
			typeLookupTable[193] = MessagePackType.Unknown;
			typeLookupTable[194] = MessagePackType.Boolean;
			typeLookupTable[195] = MessagePackType.Boolean;
			typeLookupTable[196] = MessagePackType.Binary;
			typeLookupTable[197] = MessagePackType.Binary;
			typeLookupTable[198] = MessagePackType.Binary;
			typeLookupTable[199] = MessagePackType.Extension;
			typeLookupTable[200] = MessagePackType.Extension;
			typeLookupTable[201] = MessagePackType.Extension;
			typeLookupTable[202] = MessagePackType.Float;
			typeLookupTable[203] = MessagePackType.Float;
			typeLookupTable[204] = MessagePackType.Integer;
			typeLookupTable[205] = MessagePackType.Integer;
			typeLookupTable[206] = MessagePackType.Integer;
			typeLookupTable[207] = MessagePackType.Integer;
			typeLookupTable[208] = MessagePackType.Integer;
			typeLookupTable[209] = MessagePackType.Integer;
			typeLookupTable[210] = MessagePackType.Integer;
			typeLookupTable[211] = MessagePackType.Integer;
			typeLookupTable[212] = MessagePackType.Extension;
			typeLookupTable[213] = MessagePackType.Extension;
			typeLookupTable[214] = MessagePackType.Extension;
			typeLookupTable[215] = MessagePackType.Extension;
			typeLookupTable[216] = MessagePackType.Extension;
			typeLookupTable[217] = MessagePackType.String;
			typeLookupTable[218] = MessagePackType.String;
			typeLookupTable[219] = MessagePackType.String;
			typeLookupTable[220] = MessagePackType.Array;
			typeLookupTable[221] = MessagePackType.Array;
			typeLookupTable[222] = MessagePackType.Map;
			typeLookupTable[223] = MessagePackType.Map;
			formatNameTable[192] = "nil";
			formatNameTable[193] = "(never used)";
			formatNameTable[194] = "false";
			formatNameTable[195] = "true";
			formatNameTable[196] = "bin 8";
			formatNameTable[197] = "bin 16";
			formatNameTable[198] = "bin 32";
			formatNameTable[199] = "ext 8";
			formatNameTable[200] = "ext 16";
			formatNameTable[201] = "ext 32";
			formatNameTable[202] = "float 32";
			formatNameTable[203] = "float 64";
			formatNameTable[204] = "uint 8";
			formatNameTable[205] = "uint 16";
			formatNameTable[206] = "uint 32";
			formatNameTable[207] = "uint 64";
			formatNameTable[208] = "int 8";
			formatNameTable[209] = "int 16";
			formatNameTable[210] = "int 32";
			formatNameTable[211] = "int 64";
			formatNameTable[212] = "fixext 1";
			formatNameTable[213] = "fixext 2";
			formatNameTable[214] = "fixext 4";
			formatNameTable[215] = "fixext 8";
			formatNameTable[216] = "fixext 16";
			formatNameTable[217] = "str 8";
			formatNameTable[218] = "str 16";
			formatNameTable[219] = "str 32";
			formatNameTable[220] = "array 16";
			formatNameTable[221] = "array 32";
			formatNameTable[222] = "map 16";
			formatNameTable[223] = "map 32";
			for (int m = 224; m <= 255; m++)
			{
				typeLookupTable[m] = MessagePackType.Integer;
				formatNameTable[m] = "negative fixint";
			}
		}

		public static MessagePackType ToMessagePackType(byte code)
		{
			return typeLookupTable[code];
		}

		public static string ToFormatName(byte code)
		{
			return formatNameTable[code];
		}
	}
}
