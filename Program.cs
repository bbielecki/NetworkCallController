using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCallController
{
    class Program
    {
        /*
         * W batchu nalezy podac argumenty args odpowiednio na miejscach:
         * 1: nazwe klienta 1
         * 2: nazwe klienta 2
         * 3: nazwe klienta 3
         * 4: id NCC
         * 5: port sasiedniego NCC
         */
        static void Main(string[] args)
        {
            string[] clientName = new string[3];
            int nccId;
            int neighbourNCCport;

            if (args == null)
            {
            
                for (int i = 0; i < 3; i++)
                {
                    clientName[i] = args[i];
                }
                nccId = Int32.Parse(args[3]);
                neighbourNCCport = Int32.Parse(args[4]);
            }
            else
            {
                clientName[0] = "Abacki";
                clientName[0] = "Cabacki";
                clientName[0] = "Babacki";

                nccId = 1;
                neighbourNCCport = 50000 + 2;
            }

            NCCstart nccStart = new NCCstart(50000 + nccId, clientName, nccId, neighbourNCCport);
            
        }
    }
}
