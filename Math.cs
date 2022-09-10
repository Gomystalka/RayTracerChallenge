﻿using System;
using System.Collections.Generic;
using System.Text;

using SysMath = System.Math;

namespace RayTracer.Math
{
    public struct Float4
    {
        public const float kEpsilon = 0.00001f;

        public float x;
        public float y;
        public float z;
        private float _w;

        public float Magnitude => MathF.Sqrt((x * x) + (y * y) + (z * z) + (_w * _w));
        public Float4 Normalised
        {
            get
            {
                Float4 f = new Float4(this);
                f /= Magnitude;
                return f;
            }
        }

        public Float4(Float4 f)
        { //Copy
            x = f.x;
            y = f.y;
            z = f.y;
            _w = f._w;
        }

        public Float4(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            _w = 0;
        }

        public Float4(float x, float y, float z, byte w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this._w = w;
        }

        public bool IsPoint()
            => _w >= 1;
        public bool IsVector()
            => _w <= 0;

        public Float4 AsVector()
        {
            _w = 0;
            return this;
        }

        public Float4 AsPoint()
        {
            _w = 1;
            return this;
        }

        public static float Dot(Float4 a, Float4 b)
            => (a.x * b.x) + (a.y * b.y) + (a.z * b.z) + (a._w * b._w);

        public static Float4 Cross(Float4 a, Float4 b)
            => new Float4((a.y * b.z - a.z * b.y),
                (a.z * b.x - a.x * b.z),
                (a.x * b.y - a.y * b.x));

        public static Float4 Vector(float x, float y, float z)
            => new Float4(x, y, z, 0);

        public static Float4 Point(float x, float y, float z)
            => new Float4(x, y, z, 1);

        public static implicit operator Float4((float, float, float, byte) tuple)
            => new Float4(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);

        public static implicit operator Float4((float, float, float) tuple)
            => new Float4(tuple.Item1, tuple.Item2, tuple.Item3, 0);

        public static bool operator ==(Float4 vector, Float4 comp)
        {
            return (SysMath.Abs(vector.x - comp.x)) <= kEpsilon &&
                           (SysMath.Abs(vector.y - comp.y)) <= kEpsilon &&
                           (SysMath.Abs(vector.z - comp.z)) <= kEpsilon &&
                           (SysMath.Abs(vector._w - comp._w) <= kEpsilon);
        }

        public static bool operator !=(Float4 vector, Float4 comp)
            => !(vector == comp);

        public static Float4 operator +(Float4 vector, Float4 other)
        {
            vector.x += other.x;
            vector.y += other.y;
            vector.z += other.z;
            vector._w += other._w;
            return vector;
        }

        public static Float4 operator -(Float4 vector, Float4 other)
        {
            vector.x -= other.x;
            vector.y -= other.y;
            vector.z -= other.z;
            vector._w -= other._w;
            //vector._w = Math.Max(vector._w, other._w) - Math.Min(vector._w, other._w);
            return vector;
        }

        public static Float4 operator /(Float4 vector, Float4 other)
        {
            vector.x /= other.x;
            vector.y /= other.y;
            vector.z /= other.z;
            vector._w /= other._w;
            return vector;
        }

        public static Float4 operator *(Float4 vector, Float4 other)
        {
            vector.x *= other.x;
            vector.y *= other.y;
            vector.z *= other.z;
            vector._w *= other._w;
            return vector;
        }

        public static Float4 operator *(Float4 vector, float scalar)
        {
            vector.x *= scalar;
            vector.y *= scalar;
            vector.z *= scalar;
            vector._w *= scalar;
            return vector;
        }

        public static Float4 operator /(Float4 vector, float divider)
        {
            vector.x /= divider;
            vector.y /= divider;
            vector.z /= divider;
            vector._w /= divider;
            return vector;
        }

        public static Float4 operator -(Float4 vector)
        {
            vector.x = -vector.x;
            vector.y = -vector.y;
            vector.z = -vector.z;
            return vector;
        }

        public override bool Equals(object obj)
        {
            if (obj is Float4 comp)
                return (Float4)obj == comp;

            return false;
        }

        public override int GetHashCode()
            => x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ _w.GetHashCode();

        public override string ToString()
            => $"[{x}, {y}, {z}, {_w}]";
    }
}
