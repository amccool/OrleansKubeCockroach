using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldGrains
{
    public interface IParrotGrain : IGrainWithGuidKey
    {
        Task SaveState(int message);
    }
}
