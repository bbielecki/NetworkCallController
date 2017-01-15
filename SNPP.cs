using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCallController
{
    public class SNPP
    {
        List<SNP> snps;
        public SNPP()
        {
            snps = new List<SNP>();
        }
        public SNP getFirst()
        {
            return snps[0];
        }


    }
}
