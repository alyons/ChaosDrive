using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChaosDrive.Game_Objects
{
    public interface ITimeAdjuster
    {
        float TimeAdjustment
        {
            get;
        }
        string UniqueName
        {
            get;
        }
    }
}
