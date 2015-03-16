﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.Utility
{
    public static class Extensions
    {
        public static int ToEpoch(this DateTime date)
        {
            if (date == null) return int.MinValue;
            DateTime epoch = new DateTime(1970, 1, 1);
            TimeSpan epochTimeSpan = date - epoch;
            return (int)epochTimeSpan.TotalSeconds;
        }
    }
}
