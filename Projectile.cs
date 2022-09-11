using RayTracer.Maths;
using RayTracer.UnitTesting;
using RayTracer.Debugging;

namespace RayTracer.Sandbox
{
    public class Projectile
    {
        public Float4 position;
        public Float4 velocity;

        public void Update(World world) {
            position += velocity;
            velocity += world.gravity + world.windVelocity;
            Debug.Log($"Projectile Position: {position}");
        }
    }
}
