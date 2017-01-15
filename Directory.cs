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
        public Dictionary<string, int> userDomain;

        public Directory()
        {
            usernameToAddress = new Dictionary<string, SNP>();
            userDomain = new Dictionary<string, int>();
            SNP snp1 = new SNP(1);
            SNP snp2 = new SNP(2);
            usernameToAddress.Add("Abacki", snp1);
            usernameToAddress.Add("Babacki", snp2);
            userDomain.Add("Abacki", 1);
            userDomain.Add("Babacki", 2);
        }

        public int translateUsernameToAddress(string username)
        {
            SNP snp = new SNP();
            usernameToAddress.TryGetValue(username, out snp);
            int userID = snp.routerId;

            return userID;
        }

        public int checkUserDomain(string username)
        {
            int id = 0;
            userDomain.TryGetValue(username, out id);

            return id;
        }

        public SNP getUserSNP(string username)
        {
            SNP userSNP = new SNP();
            usernameToAddress.TryGetValue(username, out userSNP);

            return userSNP;
        }
    }
}
