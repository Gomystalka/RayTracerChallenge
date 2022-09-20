using RayTracer.Core;
using RayTracer.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace RayTracer.Geometry.Primitives
{
    public class Sphere : Core.Object
    {
        Float4 position = Float4.Point(0, 0, 0);

        public Sphere() {
            position = Float4.Point(0, 0, 0);
        }

        public RayIntersection[] GetIntersections(Ray ray) {
            Float4 dir = ray.origin - position;

            float d = Float4.Dot(ray.direction, ray.direction);
            float dirDot = Float4.Dot(ray.direction, dir) * 2f;
            float sDot = Float4.Dot(dir, dir) - 1f;

            float discriminant = (dirDot * dirDot) - 4f * d * sDot;
            if (discriminant >= 0) {
                RayIntersection[] intersections = new RayIntersection[2];

                discriminant = MathF.Sqrt(discriminant);

                intersections[0] = new RayIntersection((-dirDot - discriminant) / (d * 2f), this);
                intersections[1] = new RayIntersection((-dirDot + discriminant) / (d * 2f), this);

                return intersections;
            }

            return Array.Empty<RayIntersection>();
        }

        //public Float4[] GetIntersectionPoints(Core.Ray ray)
        //{
        //    float[] intersections = GetIntersections(ray);
        //    return new Float4[] {
        //        ray.GetPositionAtTime(intersections[0]),
        //        ray.GetPositionAtTime(intersections[1])
        //    };
        //}
    }
}
