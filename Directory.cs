using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCallController
{
    class Directory
    {
        public Dictionary<string, SNP> usernameToAddress;

        public Directory()
        {
            usernameToAddress = new Dictionary<string, SNP>();
        }

        public SNP translateUsernameToAddress(string username)
        {
            SNP snp = new SNP();
            usernameToAddress.TryGetValue(username, out snp);

            return snp;
        }
    }
}
