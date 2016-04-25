using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Core
{
    class Rsa
    {
        private byte p; //destroy
        private byte q; //destroy
        private ushort phi; //destroy
        private ushort n;
        private ushort e;
        private Int32 d;

        private struct ExtendedEuclideanResult
        {
            public int u1;
            public int u2;
            public int gcd;
        }

        public Rsa()
        {
            //InitKeyData();
        }

        public void InitKeyData()
        {
            Random random = new Random();

            byte[] simple = GetNotDivideable();
            this.p = simple[random.Next(0, simple.Length)];
            this.q = simple[random.Next(0, simple.Length)];
            this.n = (ushort)(this.p * this.q);
            this.phi = (ushort)((p - 1) * (q - 1));
            List<ushort> possibleE = GetAllPossibleE(this.phi);

            do
            {
                this.e = possibleE[random.Next(0, possibleE.Count)];
                this.d = ExtendedEuclide(this.e % this.phi, this.phi).u1;
            } while (this.d < 0);
        }

        public Int32 GetNKey()
        {
            return this.n;
        }

        public Int32 GetEKey()
        {
            return this.e;
        }

        public Int32 GetDKey()
        {
            return this.d;
        }

        public string encode(string text)
        {
            InitKeyData();

            string outStr = "";
            
            //System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            byte[] strBytes = Encoding.UTF8.GetBytes(text);
            

            //List<byte> strBytes = new List<byte>();

            //foreach (var VARIABLE in text)
            //{
            //    strBytes.Add(Convert.ToByte(VARIABLE));
            //}

            foreach (byte value in strBytes)
            {
                int encryptedValue = ModuloPow(value, this.e, this.n);
                outStr += encryptedValue + "|";
            }

           // outStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(outStr));
            return outStr;
        }

        public string decode(string text, string n_s, string d_s)
        {
            //text = Encoding.UTF8.GetString(Convert.FromBase64String(text));

            string outStr = "";
            Int32 n = Int32.Parse(n_s);
            Int32 d = Int32.Parse(d_s);
            Int32[] arr = GetDecArrayFromText(text);
            byte[] bytes = new byte[arr.Length];
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            int j=0;
            foreach (int i in arr)
            {
                byte decryptedValue = (byte)ModuloPow(i, d, n);

                bytes[j] = decryptedValue;
                j++;
                
            }
            outStr += enc.GetString(bytes);
            return outStr;
        }

        private Int32[] GetDecArrayFromText(string text)
        {
            int i = 0;
            foreach (char c in text)
            {
                if (c == '|')
                {
                    i++;
                }
            }

            Int32[] result = new Int32[i];
            i = 0;

            string tmp = "";

            foreach (char c in text)
            {
                if (c != '|')
                {
                    tmp += c;
                }
                else
                {
                    result[i] = Int32.Parse(tmp);
                    i++;
                    tmp = "";
                }
            }

            return result;
        }

        static int ModuloPow(int value, int pow, int modulo)
        {
            int result = value;
            for (int i = 0; i < pow - 1; i++)
            {
                result = (result * value) % modulo;
            }
            return result;
        }

        /// Получить все варианты для e
        static List<ushort> GetAllPossibleE(ushort phi)
        {
            List<ushort> result = new List<ushort>();

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
            int u1 = 1;
            int u3 = a;
            int v1 = 0;
            int v3 = b;

            while (v3 > 0)
            {
                int q0 = u3 / v3;
                int q1 = u3 % v3;

                int tmp = v1 * q0;
                int tn = u1 - tmp;
                u1 = v1;
                v1 = tn;

                u3 = v3;
                v3 = q1;
            }

            int tmp2 = u1 * (a);
            tmp2 = u3 - (tmp2);
            int res = tmp2 / (b);

            ExtendedEuclideanResult result = new ExtendedEuclideanResult()
            {
                u1 = u1,
                u2 = res,
                gcd = u3
            };

            return result;
        }

        static private byte[] GetNotDivideable()
        {
            List<byte> notDivideable = new List<byte>();

            for (int x = 2; x < 256; x++)
            {
                int n = 0;
                for (int y = 1; y <= x; y++)
                {
                    if (x % y == 0)
                        n++;
                }

                if (n <= 2)
                    notDivideable.Add((byte)x);
            }
            return notDivideable.ToArray();
        }

    }
}