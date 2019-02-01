using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceCore.Middleware.Routing
{
    public interface IReturn<TResp> //Consider restricting T  to a return type where the TResp is a HasStatusResponseCode
    {
    }
}
