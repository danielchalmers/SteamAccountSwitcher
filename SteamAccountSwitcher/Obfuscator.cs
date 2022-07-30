using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SteamAccountSwitcher
{
    public class Obfuscator
    {
        private static readonly byte[] Key =
        {
            144, 26, 171, 192, 29, 143, 155, 228, 245, 136, 60, 49, 25, 150, 224, 206, 92, 2,
            161, 45, 173, 7, 254, 222, 239, 234, 121, 62, 104, 93, 97, 255
        };

        private static readonly byte[] Vector =
        {
            99, 2, 62, 188, 110, 135, 59, 43, 101, 4, 100, 220, 31, 102, 47, 36
        };

        private readonly ICryptoTransform _decryptor;
        private readonly UTF8Encoding _encoder;
        private readonly ICryptoTransform _encryptor;

        public Obfuscator()
        {
            var rm = new RijndaelManaged();
            _encryptor = rm.CreateEncryptor(Key, Vector);
            _decryptor = rm.CreateDecryptor(Key, Vector);
            _encoder = new UTF8Encoding();
        }

        public string Obfuscate(string original)
        {
            return Convert.ToBase64String(Obfuscate(_encoder.GetBytes(original)));
        }

        public string Deobfuscate(string obfuscated)
        {
            return _encoder.GetString(Deobfuscate(Convert.FromBase64String(obfuscated)));
        }

        private byte[] Obfuscate(byte[] buffer)
        {
            return Transform(buffer, _encryptor);
        }

        private byte[] Deobfuscate(byte[] buffer)
        {
            return Transform(buffer, _decryptor);
        }

        private byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            var stream = new MemoryStream();
            using (var cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }

            return stream.ToArray();
        }
    }
}