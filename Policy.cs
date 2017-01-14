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

        public Policy()
        {
            userPolicy = new Dictionary<string, int>();
        }

        public bool checkPolicy(CallParameters cp)
        {
            int capacity = 0;
            userPolicy.TryGetValue(cp.UserName, out capacity);

            if (cp.UserCallCapacity <= capacity)
                return true;
            else
                return false;
        }
    }
}
