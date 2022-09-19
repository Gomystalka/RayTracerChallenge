using RayTracer.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace RayTracer.Geometry.Primitives
{
    public class Sphere
    {
        Float4 position = Float4.Point(0, 0, 0);
        //Float4 scale;
        public Guid instanceId;

        public Sphere() {
            instanceId = Guid.NewGuid();
            position = Float4.Point(0, 0, 0);
        }

        public float[] GetIntersections(Core.Ray ray) {
            Float4 dir = ray.origin - position;

            float d = Float4.Dot(ray.direction, ray.direction);
            float dirDot = Float4.Dot(ray.direction, dir) * 2f;
            float sDot = Float4.Dot(dir, dir) - 1f;

            float discriminant = (dirDot * dirDot) - 4f * d * sDot;
            if (discriminant >= 0) {
                float[] intersections = new float[2];

                discriminant = MathF.Sqrt(discriminant);

                intersections[0] = (-dirDot - discriminant) / (d * 2f);
                intersections[1] = (-dirDot + discriminant) / (d * 2f);

                return intersections;
            }

            return Array.Empty<float>();
        }

        public Float4[] GetIntersectionPoints(Core.Ray ray)
        {
            float[] intersections = GetIntersections(ray);
            return new Float4[] {
                ray.GetPositionAtTime(intersections[0]),
                ray.GetPositionAtTime(intersections[1])
            };
        }
    }
}
