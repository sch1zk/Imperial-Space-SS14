using Content.Server.Roles;
using Content.Shared.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server.Mind
{
    public sealed class Memory
    {
        [ViewVariables]
        public string Name { get; set; }
        [ViewVariables]
        public string Value { get; set; }
        public Memory(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
