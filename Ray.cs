using System;
using System.Collections.Generic;
using System.Text;
using RayTracer.Geometry.Primitives;
using RayTracer.Maths;

namespace RayTracer.Core
{
    public struct Ray
    {
        public Float4 origin;
        public Float4 direction;

        public Ray(Float4 origin, Float4 direction) {
            this.origin = origin;
            this.direction = direction;
        }

        public Float4 GetPositionAtTime(float t)
            => origin + (direction * t);
    }

    public struct RayIntersection {
        public float hitTime;
        public Object hitObject;

        public RayIntersection(float time, Object source)
        {
            hitTime = time;
            hitObject = source;
        }
    }
}
