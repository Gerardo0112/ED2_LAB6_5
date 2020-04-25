using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
        //Lectura del texto.
        public void read_text(string path1, string path2, string file, string f_name)
        {
            System.IO.StreamReader lecture = new System.IO.StreamReader(path2);
            var key = 0;
            var N = 0;
            while (!lecture.EndOfStream)
            {
                var line = lecture.ReadLine();
                var values = line.Split(Convert.ToChar(","));
                key = Convert.ToInt32(values[0]);
                N = Convert.ToInt32(values[1]);
            }
            byte[] array = BitConverter.GetBytes(N);
            int size = Convert.ToInt32(Math.Ceiling(Math.Log(N, 256)));
            var path_cif = Path.Combine(file, Path.GetFileNameWithoutExtension(f_name) + ".rsacif");
            List<Byte> text_cifrado = new List<Byte>();
            using (var stream = new FileStream(path1, FileMode.Open))
            {
                using (var reading = new BinaryReader(stream))
                {
                    using (var write_stream = new FileStream(path_cif, FileMode.OpenOrCreate))
                    {
                        using (var writing = new BinaryWriter(write_stream))
                        {
                            var bytes = new byte[length];
                            while (reading.BaseStream.Position != reading.BaseStream.Length)
                            {
                                bytes = reading.ReadBytes(length);
                                foreach (var item in bytes)
                                {
                                    BigInteger encryption = BigInteger.ModPow(item, Convert.ToInt32(key), N);
                                    string binary_encryption = Convert.ToString((int)(encryption), 2);
                                    string bloque_cif = binary_encryption.PadLeft(size * 8, '0');
                                    while (bloque_cif.Length != 0)
                                    {
                                        writing.Write(Convert.ToByte(bloque_cif.Substring(0, 8), 2));
                                        bloque_cif = bloque_cif.Remove(0, 8);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //Aplicación de cifrado hacia la lectura.
        public int encryption(int letter, int Key, int N)
        {
            var base_number = letter % N;
            var mult = 1;
            for (var x = 0; x < Key; x++)
            {
                mult = (mult * base_number) % N;
            }
            var encryption_ = Convert.ToInt32(mult);
            return encryption_;
        }
        
        
    }
}
