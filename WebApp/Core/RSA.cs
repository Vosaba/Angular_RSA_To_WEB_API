using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace WebApp.Core
{
    class Rsa
    {
        private ushort _n;
        private ushort _e;
        private int _d;

        private struct ExtendedEuclideanResult
        {
            public int U1;
            public int U2;
            public int Gcd;
        }

        public void InitKeyData()
        {
            var random = new Random();

            var simple = GetNotDivideable();
            var p = simple[random.Next(0, simple.Length)];
            var q = simple[random.Next(0, simple.Length)];
            _n = (ushort) (p*q);
            var phi = (ushort) ((p - 1)*(q - 1));
            var possibleE = GetAllPossibleE(phi);

            do
            {
                _e = possibleE[random.Next(0, possibleE.Count)];
                _d = ExtendedEuclide(_e%phi, phi).U1;
            } while (_d < 0);
        }

        public int GetNKey()
        {
            return _n;
        }

        public int GetEKey()
        {
            return _e;
        }

        public int GetDKey()
        {
            return _d;
        }

        public string Encode(string text, string nS, string eS)
        {
            //InitKeyData();

          //  var strBytes = Encoding.UTF8.GetBytes(text);
             _e = (byte)int.Parse(eS);
             _n = (byte)int.Parse(nS);
            //var arr = GetDecArrayFromText
            return  Encode(text);
            // outStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(outStr));
            //return strBytes.Select(value => ModuloPow(value, _e, _n))
            //    .Aggregate("", (current, encryptedValue) => current + (encryptedValue + "|"));
        }

        public string Encode(string text)
        {
            InitKeyData();

            var strBytes = Encoding.UTF8.GetBytes(text);


            // outStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(outStr));
            return strBytes.Select(value => ModuloPow(value, _e, _n))
                .Aggregate("", (current, encryptedValue) => current + (encryptedValue + "|"));
        }

        public string Decode(string text, string nS, string dS)
        {
            //text = Encoding.UTF8.GetString(Convert.FromBase64String(text));

            var outStr = "";
            var n = int.Parse(nS);
            var d = int.Parse(dS);
            var arr = GetDecArrayFromText(text);
            var bytes = new byte[arr.Length];
            var enc = new UTF8Encoding();
            var j = 0;

            foreach (var decryptedValue in arr.Select(i => (byte) ModuloPow(i, d, n)))
            {
                bytes[j] = decryptedValue;
                j++;
            }
            outStr += enc.GetString(bytes);
            return outStr;
        }

        private int[] GetDecArrayFromText(string text)
        {
            var i = text.Count(c => c == '|');

            var result = new int[i];
            i = 0;

            var tmp = "";

            foreach (var c in text)
            {
                if (c != '|')
                {
                    tmp += c;
                }
                else
                {
                    result[i++] = int.Parse(tmp);
                    tmp = "";
                }
            }

            return result;
        }

        static int ModuloPow(int value, int pow, int modulo)
        {
            var result = value;
            for (var i = 0; i < pow - 1; i++)
            {
                result = (result*value)%modulo;
            }
            return result;
        }

        static List<ushort> GetAllPossibleE(ushort phi)
        {
            var result = new List<ushort>();

            for (ushort i = 2; i < phi; i++)
            {
                if (ExtendedEuclide(i, phi).Gcd == 1)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        private static ExtendedEuclideanResult ExtendedEuclide(int a, int b)
        {
            var u1 = 1;
            var u3 = a;
            var v1 = 0;
            var v3 = b;

            while (v3 > 0)
            {
                var q0 = u3/v3;
                var q1 = u3%v3;

                var tmp = v1*q0;
                var tn = u1 - tmp;
                u1 = v1;
                v1 = tn;

                u3 = v3;
                v3 = q1;
            }

            var tmp2 = u1*(a);
            tmp2 = u3 - (tmp2);
            var res = tmp2/(b);

            var result = new ExtendedEuclideanResult
            {
                U1 = u1,
                U2 = res,
                Gcd = u3
            };

            return result;
        }

        private static byte[] GetNotDivideable()
        {
            var notDivideable = new List<byte>();

            for (var x = 2; x < 256; x++)
            {
                var n = 0;
                for (var y = 1; y <= x; y++)
                {
                    if (x%y == 0)
                        n++;
                }

                if (n <= 2)
                    notDivideable.Add((byte) x);
            }
            return notDivideable.ToArray();
        }
    }
}