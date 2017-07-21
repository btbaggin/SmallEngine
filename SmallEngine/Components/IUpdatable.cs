using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public interface IUpdatable
    {
        void Update(float pDeltaTime);
    }
}
