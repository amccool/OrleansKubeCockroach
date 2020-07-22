using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorldGrains
{
    [Serializable]
    public class ParrotState
    {
        public bool Even { get; set; }

        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
