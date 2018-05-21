using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracAlisSatis
{
    class Kontrol
    {

        public static bool boslukKontrol(string[] data)
        {
            for (int i = 0; i < data.Length; i++) if (String.IsNullOrEmpty(data[i])) return false;

            return true; 
        }
    }
}
