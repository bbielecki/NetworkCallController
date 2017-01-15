using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCallController
{
    class Policy
    {
        //nazwa uzytkownika i przypisana mu maksymalna 
        //pojemnosc jaka jego polaczenie moze zajac
        private Dictionary<string, int> userPolicy;
        private CallParameters callParameters;
        private Dictionary<int, int> domainPolicy;

        public Policy()
        {
            userPolicy = new Dictionary<string, int>();
            domainPolicy = new Dictionary<int, int>();
            userPolicy.Add("Abacki", 10);
            domainPolicy.Add(1, 8);
            domainPolicy.Add(2, 8);

        }

        public bool checkPolicy(CallParameters cp)
        {
            int capacity = 0;
            userPolicy.TryGetValue(cp.UserName, out capacity);
            Console.WriteLine("wymagana pojemnosc :" + capacity);

            if (cp.UserCallCapacity <= capacity)
                return true;
            else
                return false;
        }
        public bool checkForeignPolicy(CallParameters cp)
        {
            int capacity = 0;
            domainPolicy.TryGetValue(cp.userDomain, out capacity);

            if (cp.UserCallCapacity <= capacity)
                return true;
            else
                return false;
        }
    }
}
