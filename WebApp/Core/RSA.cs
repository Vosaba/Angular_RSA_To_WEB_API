using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApp.Core
{
    class Rsa
    {
        //private byte p; //destroy
        //private byte q; //destroy
        //private ushort phi; //destroy
        private ushort n;
        private ushort e;
        private int d;

        private struct ExtendedEuclideanResult
        {
            public int u1;
            public int u2;
            public int gcd;
        }

        public void InitKeyData()
        {
            var random = new Random();

            var simple = GetNotDivideable();
            var p = simple[random.Next(0, simple.Length)];
            var q = simple[random.Next(0, simple.Length)];
            n = (ushort) (p*q);
            var phi = (ushort) ((p - 1)*(q - 1));
            var possibleE = GetAllPossibleE(phi);

            do
            {
                e = possibleE[random.Next(0, possibleE.Count)];
                d = ExtendedEuclide(e%phi, phi).u1;
            } while (d < 0);
        }

        public int GetNKey()
        {
            return n;
        }

        public int GetEKey()
        {
            return e;
        }

        public int GetDKey()
        {
            return d;
        }

        public string encode(string text)
        {
            InitKeyData();

            var outStr = "";

            var strBytes = Encoding.UTF8.GetBytes(text);


            foreach (var value in strBytes)
            {
                var encryptedValue = ModuloPow(value, e, n);
                outStr += encryptedValue + "|";
            }

            // outStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(outStr));
            return outStr;
        }

        public string decode(string text, string n_s, string d_s)
        {
            //text = Encoding.UTF8.GetString(Convert.FromBase64String(text));

            var outStr = "";
            var n = int.Parse(n_s);
            var d = int.Parse(d_s);
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
                    result[i] = int.Parse(tmp);
                    i++;
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

        /// Получить все варианты для e
        static List<ushort> GetAllPossibleE(ushort phi)
        {
            var result = new List<ushort>();

            for (ushort i = 2; i < phi; i++)
            {
                if (ExtendedEuclide(i, phi).gcd == 1)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        /// <summary>
        /// u1 * a + u2 * b = u3
        /// </summary>
        /// <param name="a">Число a</param>
        /// <param name="b">Модуль числа</param>
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
                u1 = u1,
                u2 = res,
                gcd = u3
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