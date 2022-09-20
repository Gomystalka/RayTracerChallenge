using System;

namespace RayTracer.Core
{
    public class Object
    {
        public Guid InstanceId { get; set; }
        public string Name { get; set; }

        public Object()
            => InstanceId = Guid.NewGuid();
    }
}
