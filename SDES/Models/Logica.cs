using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDES.Models
{
    public class Logica
    {
        public string key;
        public string P10;
        public string P8;
        public string P4;
        public string IP;
        public string EP;
        public string K1;
        public string K2;
        public string[,] S0 = { { "01", "00", "11", "10"}, {"11", "10", "01", "00"}, { "00", "10", "01", "11"}, { "11", "01", "11", "10"}};
        public string[,] S1 = { { "00", "01", "10", "11" }, { "10", "00", "01", "11" }, { "11", "00", "01", "00" }, { "10", "01", "00", "11" } };

        #region FUNCIONES PARA OBTENER PERMUTACIONES
        public string ObtenerP(string P)
        {
            var keyP = string.Empty;
            for (int i = 0; i < P.Length; i++)
            {
                keyP += key[Convert.ToInt32(P[i].ToString())];
            }
            return keyP;
        }

        public string ObtenerP8(string P)
        {
            var keyP = string.Empty;
            for (int i = 0; i < 8; i++)
            {
                keyP += P[Convert.ToInt32(P8[i].ToString())];
            }
            return keyP;
        }

        public string ObtenerP4(string P)
        {
            var keyP = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                keyP += P[Convert.ToInt32(P4[i].ToString())];
            }
            return keyP;
        }

        public string ObtenerLS1(string keyP10)
        {
            var LS1 = string.Empty;
            var part1 = string.Empty;
            var part2 = string.Empty;
            for (int i = 1; i < 5; i++)
            {
                part1 += keyP10[i];
                part2 += keyP10[i + 5];
            }
            part1 += keyP10[0];
            part2 += keyP10[5];
            return part1 + part2;
        }

        public string ObtenerLS2(string key)
        {
            var LS1 = string.Empty;
            var part1 = string.Empty;
            var part2 = string.Empty;
            for (int i = 2; i < 5; i++)
            {
                part1 += key[i];
                part2 += key[i + 5];
            }
            part1 += key[0];
            part1 += key[1];
            part2 += key[5];
            part2 += key[6];
            return part1 + part2;
        }

        public string ExpandirPermutar(string grupoBits)
        {
            var keyEP = string.Empty;
            for (int i = 0; i < 8; i++)
            {
                keyEP += grupoBits[Convert.ToInt32(EP[i].ToString())];
            }
            return keyEP;
        }

        public string ObtenerXOR(string key1, string key2)
        {
            var xor = string.Empty;

            for (int i = 0; i < key1.Length; i++)
            {
                if (key1[i] == key2[i])
                {
                    xor += "0";
                }
                else
                {
                    xor += "1";
                }
            }

            return xor;
        }
        
        public string sBoxXoR(string Xor)
        {
            var x = Convert.ToInt32(Convert.ToString(Xor[0]) + Convert.ToString(Xor[3]), 2);
            var y = Convert.ToInt32(Convert.ToString(Xor[1]) + Convert.ToString(Xor[2]), 2);

            var part1 = S0[Convert.ToInt32(x), Convert.ToInt32(y)];

            x = Convert.ToInt32(Convert.ToString(Xor[4]) + Convert.ToString(Xor[7]), 2);
            y = Convert.ToInt32(Convert.ToString(Xor[5]) + Convert.ToString(Xor[6]), 2);

            var part2 = S1[Convert.ToInt32(x), Convert.ToInt32(y)];

            return ObtenerXOR(part1 + part2, IP.Substring(0, 4));
        }

        public string ObtenerInversoIP(string xor)
        {
            var keyIP = string.Empty;
            var AuxDiccionario = new Dictionary<int, int>();

            for (int i = 0; i < 8; i++)
            {
                AuxDiccionario.Add(Convert.ToInt32(IP[i].ToString()), i);
            }

            for (int i = 0; i < 8; i++)
            {
                keyIP += AuxDiccionario[i];
            }

            var keyP = string.Empty;
            for (int i = 0; i < 8; i++)
            {
                keyP += xor[Convert.ToInt32(keyIP[i].ToString())];
            }
            return keyP;
        }
        #endregion

        public void ObtenerClaves(string[] Permutaciones)
        {
            #region LECTURA DE CLAVES
            P10 = Permutaciones[0];
            P8 = Permutaciones[1];
            P4 = Permutaciones[2];

            EP = Permutaciones[3];
            IP = Permutaciones[4];

            key = Permutaciones[5];
            var keyP10 = ObtenerP(P10);
            var LS1 = ObtenerLS1(keyP10);
            K1 = ObtenerP8(LS1);
            var LS2 = ObtenerLS2(LS1);
            K2 = ObtenerP8(LS2);
            #endregion
        }

        public int DescifradoYCifrado(char letra, int opcion)
        {
           
            if (opcion == 2)
            {
                var aux = K1;
                K1 = K2;
                K2 = aux;
            }

            #region PROCESO
            key = Convert.ToString(Convert.ToInt32(letra), 2).PadLeft(8, '0');
            var keyIP = ObtenerP(IP);
            var keyEP = ExpandirPermutar(keyIP.Substring(4));
            var EPxorK1 = ObtenerXOR(K1, keyEP);
            var XORS0S1 = sBoxXoR(EPxorK1);
            var keyP4 = ObtenerP4(XORS0S1);
            var P4XORPI = ObtenerXOR(keyIP.Substring(0,4),keyP4)+ keyIP.Substring(4);
            var SWAP = P4XORPI.Substring(4) + P4XORPI.Substring(0,4);
            var keyEP2 = ExpandirPermutar(SWAP.Substring(4));
            var EP2XOR = ObtenerXOR(K2,keyEP2);
            var SegundoXORS0S1 = sBoxXoR(EP2XOR);
            var SegundokeyP4 = ObtenerP4(SegundoXORS0S1);
            var SegundoP4XORPI = ObtenerXOR(SWAP.Substring(0, 4), SegundokeyP4) + SWAP.Substring(4);
            var InversoIP = ObtenerInversoIP(SegundoP4XORPI);
            return Convert.ToInt32(InversoIP, 2);
            #endregion
        }
    }
}