using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTierArchitecture.Entities.Models;

namespace NTierArchitecture.Entities.Abstractions;

    public interface IJwtProvider
    {
        Task<string> CreateTokenAsync(AppUser  user);
    }

