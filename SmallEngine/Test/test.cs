using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Test
{
    class test
    {
        public static void Main()
        {
            using (TestGame g = new TestGame())
            {
                g.Run();
            }
        }
    }
}
