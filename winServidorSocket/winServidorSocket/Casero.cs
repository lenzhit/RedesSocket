using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace winServidorSocket
{
    class Casero
    {
        public string Nick { get; set; }
        public string Contra { get; set; }

        public Casero(string nick,string contra)
        {

            Nick = nick;
            Contra = contra;
        }
    }
}
