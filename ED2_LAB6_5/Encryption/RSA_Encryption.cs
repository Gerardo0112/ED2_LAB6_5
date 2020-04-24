using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ED2_LAB6_5.Encryption
{
    public class RSA_Encryption
    {
        #region Difiniciones
        const int length = 1000;
        public static int e;
        List<string> text = new List<string>();
        #endregion
        //Calcular máximo común divisor.
        public int MCD(int a, int b)
        {
            int restante;
            do
            {
                restante = b;
                b = a % b;
                a = restante;
            }
            while (b != 0);
            return restante;
        }
        //Genera las respectivas llaves.
        public void Keys(int n1, int n2, string path)
        {
            var p = n1;
            var q = n2;
            //Obtener n.
            var n = p * q;
            //Obtener calculo de Q(n)
            var QN = (p - 1) * (q - 1);
            //Determinar e.
            int count; int count2;
            for (var x = 2; x < QN; x++)
            {
                count = MCD(x, n);
                count2 = MCD(x, QN);
                if ((count == 1) && (count2 == 1))
                {
                    e = x;
                    break;
                }
            }

            var temporary = 0;
            //Valor de d.
            int d = 2;
            do
            {
                d++;
                temporary = (d * e) % QN;
            } while (temporary != 1);
            using (var writeStream = new FileStream(path + "/" + "Private.Key", FileMode.OpenOrCreate))
            {
                using (var writing = new StreamWriter(writeStream))
                {
                    writing.Write(e.ToString() + "," + n.ToString());
                }
            }
            using (var writeStream2 = new FileStream(path + "/" + "Public.Key", FileMode.OpenOrCreate))
            {
                using (var writing2 = new StreamWriter(writeStream2))
                {
                    writing2.Write(d.ToString() + "," + n.ToString());
                }
            }
        }
        //Calcular la d.
        public int d_calculation(int Qn1, int Qn2, int e, int value, int Q_original)
        {
            var division = Qn1 / e;
            var mult1 = e * division;
            var mult2 = value * division;
            var result1 = Qn1 - mult1;
            var result2 = Qn2 - mult2;

            if (result2 < 0)
            {
                result2 = Q_original % result2;
            }
            if (result1 != 1)
            {
                Qn1 = e;
                e = result1;
                Qn2 = value;
                value = result2;
                return d_calculation(Qn1, Qn2, e, value, Q_original);

            }
            else
            {
                return result2;
            }
        }
    }
}
