using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Std.Utility
{
	//lifted from asp.net

	/// <summary>
	/// Handy unique id generator. The generated ids are easier to read/type than guids.
	/// NOTE: They're not AS unique as a guid, but good enough for most things
	/// </summary>
	public static class UniqueIdGenerator
	{
        internal const int  NumCharsInEncoding = 32;
        internal const int  EncodingBitsPerChar = 5; 
        internal const int  IdLengthBits  = 120;
        internal const int  IdLengthBytes = (IdLengthBits / 8 );                        // 15
        internal const int  IdLengthChars = (IdLengthBits / EncodingBitsPerChar);    // 24
 
		private static readonly RandomNumberGenerator _defaultRng;

        static readonly char[] _encoding = new []
        { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4', '5' 
        };

        static readonly bool[] _legalChars;

		static UniqueIdGenerator()
		{
			_legalChars = new bool[128]; 
            for (var i = _encoding.Length - 1; i >= 0; i--)
            {
            	var ch = _encoding[i];
            	_legalChars[ch] = true;
            }

			_defaultRng = new RNGCryptoServiceProvider();
		}

		public static string Create(RandomNumberGenerator randgen = null)
		{
			if (randgen == null)
			{
				randgen = _defaultRng;
			}

			var buffer = new byte[15];
			randgen.GetBytes(buffer);
			var encoding = Encode(buffer);
			return encoding;
		}
 
        public static bool IsValid(string s) 
		{
        	if (s == null || s.Length != IdLengthChars)
            {
            	return false;
            }
 
            try 
			{
                var i = IdLengthChars; 
                while (--i >= 0)
                {
                	var ch = s[i];
                	if (!_legalChars[ch])
                    {
                    	return false;
                    }
                }

				return true; 
            }
            catch (IndexOutOfRangeException) 
			{ 
                return false; 
            }
        } 

        static String Encode(byte[] buffer) 
		{
            int i;
        	var chars = new char[IdLengthChars]; 

            Debug.Assert(buffer.Length == IdLengthBytes); 
 
            var j = 0;
            for (i = 0; i < IdLengthBytes; i += 5) 
			{ 
                var n = buffer[i] |
                	(buffer[i+1] << 8)  |
                	(buffer[i+2] << 16) |
                	(buffer[i+3] << 24); 

                var k = (n & 0x0000001F); 
                chars[j++] = _encoding[k]; 

                k = ((n >> 5) & 0x0000001F); 
                chars[j++] = _encoding[k];

                k = ((n >> 10) & 0x0000001F);
                chars[j++] = _encoding[k]; 

                k = ((n >> 15) & 0x0000001F); 
                chars[j++] = _encoding[k]; 

                k = ((n >> 20) & 0x0000001F); 
                chars[j++] = _encoding[k];

                k = ((n >> 25) & 0x0000001F);
                chars[j++] = _encoding[k]; 

                n = ((n >> 30) & 0x00000003) | (buffer[i + 4] << 2); 
 
                k = (n & 0x0000001F);
                chars[j++] = _encoding[k]; 

                k = ((n >> 5) & 0x0000001F);
                chars[j++] = _encoding[k];
            } 

            Debug.Assert(j == IdLengthChars); 
 
            return new string(chars);
        } 

	}
}
