using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCallController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SNP
    {
        public int label { get; protected set; }
        public int routerId { get; protected set; }
        public int inOutModule { get; protected set; }
        public List<int> controllers;
        public SNP(int router, int label = 1)
        {
            this.label = label;
            this.routerId = router;
            controllers = new List<int>();
        }

        public SNP()
        {
            controllers = new List<int>();
        }
    }

}
