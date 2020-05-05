using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareSaber_API.Types
{
    public enum ShareSaberRole
    {
        None = 0,
        Verified = 1,
        Trusted = 2,
        Admin = 4,
        Owner = 8
    }
    // Might make this a flagged enum, so setting up support now.
}
