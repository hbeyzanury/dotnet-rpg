using DOTNET_RPG.Dtos.Character;
using DOTNET_RPG.Dtos.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DOTNET_RPG.Models
{
    public class ServiceResponse<T>
    {
        public  T  data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = null;

    }
}
