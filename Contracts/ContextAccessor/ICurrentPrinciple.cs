using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Dtos.User;

namespace Contracts.ContextAccessor
{
    public interface ICurrentPrinciple
    {
        CurrentUser GetCurrentUser();
    }
}
