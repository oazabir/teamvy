using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTool.Entities
{
    [Flags]
    public enum Operations : int
    {
        None = 0,
        Create = 1,
        Read = 2,
        Update = 4,
        Delete = 8,
        Execute = 16,
        All = 31
    }
}