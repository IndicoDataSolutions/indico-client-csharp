using System;

namespace Indico.Entity
{

    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    public class Workflow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool ReviewEnabled { get; set; }
    }
}
