﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierArchitecture.Business.Behaviors;

public interface ICacheable
{
    string CacheKey { get; set; }
}

