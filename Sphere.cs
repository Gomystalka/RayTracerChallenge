using RayTracer.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace RayTracer.Geometry.Primitives
{
    public class Sphere
    {
        Float4 position;
        //Float4 scale;
        public Guid instanceId;

        public Sphere() {
            instanceId = Guid.NewGuid();
            position = Float4.Point(0, 0, 0);
        }

        public float[] Intersects(Core.Ray ray) {
            //float b = 2f * Float4.Dot();

            float d = Float4.Dot(ray.direction, ray.direction);
            return null;
        }
    }
}
