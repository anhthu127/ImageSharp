// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if SUPPORTS_RUNTIME_INTRINSICS
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif

namespace SixLabors.ImageSharp
{
    /// <summary>
    /// Provides optimized static methods for trigonometric, logarithmic,
    /// and other common mathematical functions.
    /// </summary>
    internal static class Numerics
    {
#if SUPPORTS_RUNTIME_INTRINSICS
        private const int BlendAlphaControl = 0b_10_00_10_00;
        private const int ShuffleAlphaControl = 0b_11_11_11_11;
#endif

        /// <summary>
        /// Determine the Greatest CommonDivisor (GCD) of two numbers.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GreatestCommonDivisor(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }

            return a;
        }

        /// <summary>
        /// Determine the Least Common Multiple (LCM) of two numbers.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeastCommonMultiple(int a, int b)
        {
            // https://en.wikipedia.org/wiki/Least_common_multiple#Reduction_by_the_greatest_common_divisor
            return (a / GreatestCommonDivisor(a, b)) * b;
        }

        /// <summary>
        /// Calculates <paramref name="x"/> % 2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Modulo2(int x) => x & 1;

        /// <summary>
        /// Calculates <paramref name="x"/> % 4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Modulo4(int x) => x & 3;

        /// <summary>
        /// Calculates <paramref name="x"/> % 8
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Modulo8(int x) => x & 7;

        /// <summary>
        /// Fast (x mod m) calculator, with the restriction that
        /// <paramref name="m"/> should be power of 2.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ModuloP2(int x, int m) => x & (m - 1);

        /// <summary>
        /// Returns the absolute value of a 32-bit signed integer.
        /// Uses bit shifting to speed up the operation compared to <see cref="Math"/>.
        /// </summary>
        /// <param name="x">
        /// A number that is greater than <see cref="int.MinValue"/>, but less than
        /// or equal to <see cref="int.MaxValue"/>
        /// </param>
        /// <returns>The <see cref="int"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int x)
        {
            int y = x >> 31;
            return (x ^ y) - y;
        }

        /// <summary>
        /// Returns a specified number raised to the power of 2
        /// </summary>
        /// <param name="x">A single-precision floating-point number</param>
        /// <returns>The number <paramref name="x" /> raised to the power of 2.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow2(float x) => x * x;

        /// <summary>
        /// Returns a specified number raised to the power of 3
        /// </summary>
        /// <param name="x">A single-precision floating-point number</param>
        /// <returns>The number <paramref name="x" /> raised to the power of 3.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow3(float x) => x * x * x;

        /// <summary>
        /// Implementation of 1D Gaussian G(x) function
        /// </summary>
        /// <param name="x">The x provided to G(x).</param>
        /// <param name="sigma">The spread of the blur.</param>
        /// <returns>The Gaussian G(x)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Gaussian(float x, float sigma)
        {
            const float Numerator = 1.0f;
            float denominator = MathF.Sqrt(2 * MathF.PI) * sigma;

            float exponentNumerator = -x * x;
            float exponentDenominator = 2 * Pow2(sigma);

            float left = Numerator / denominator;
            float right = MathF.Exp(exponentNumerator / exponentDenominator);

            return left * right;
        }

        /// <summary>
        /// Returns the result of a normalized sine cardinal function for the given value.
        /// SinC(x) = sin(pi*x)/(pi*x).
        /// </summary>
        /// <param name="f">A single-precision floating-point number to calculate the result for.</param>
        /// <returns>
        /// The sine cardinal of <paramref name="f" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SinC(float f)
        {
            if (MathF.Abs(f) > Constants.Epsilon)
            {
                f *= MathF.PI;
                float result = MathF.Sin(f) / f;
                return MathF.Abs(result) < Constants.Epsilon ? 0F : result;
            }

            return 1F;
        }

        /// <summary>
        /// Returns the value clamped to the inclusive range of min and max.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        /// <returns>The clamped <see cref="byte"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Clamp(byte value, byte min, byte max)
        {
            // Order is important here as someone might set min to higher than max.
            if (value > max)
            {
                return max;
            }

            if (value < min)
            {
                return min;
            }

            return value;
        }

        /// <summary>
        /// Returns the value clamped to the inclusive range of min and max.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        /// <returns>The clamped <see cref="uint"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Clamp(uint value, uint min, uint max)
        {
            if (value > max)
            {
                return max;
            }

            if (value < min)
            {
                return min;
            }

            return value;
        }

        /// <summary>
        /// Returns the value clamped to the inclusive range of min and max.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        /// <returns>The clamped <see cref="uint"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            if (value > max)
            {
                return max;
            }

            if (value < min)
            {
                return min;
            }

            return value;
        }

        /// <summary>
        /// Returns the value clamped to the inclusive range of min and max.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        /// <returns>The clamped <see cref="float"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            if (value > max)
            {
                return max;
            }

            if (value < min)
            {
                return min;
            }

            return value;
        }

        /// <summary>
        /// Returns the value clamped to the inclusive range of min and max.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        /// <returns>The clamped <see cref="double"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max)
        {
            if (value > max)
            {
                return max;
            }

            if (value < min)
            {
                return min;
            }

            return value;
        }

        /// <summary>
        /// Returns the value clamped to the inclusive range of min and max.
        /// 5x Faster than <see cref="Vector4.Clamp(Vector4, Vector4, Vector4)"/>
        /// on platforms &lt; NET 5.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        /// <returns>The clamped <see cref="Vector4"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Clamp(Vector4 value, Vector4 min, Vector4 max)
            => Vector4.Min(Vector4.Max(value, min), max);

        /// <summary>
        /// Clamps the span values to the inclusive range of min and max.
        /// </summary>
        /// <param name="span">The span containing the values to clamp.</param>
        /// <param name="min">The minimum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(Span<byte> span, byte min, byte max)
        {
            Span<byte> remainder = span.Slice(ClampReduce(span, min, max));

            if (remainder.Length > 0)
            {
                for (int i = 0; i < remainder.Length; i++)
                {
                    ref byte v = ref remainder[i];
                    v = Clamp(v, min, max);
                }
            }
        }

        /// <summary>
        /// Clamps the span values to the inclusive range of min and max.
        /// </summary>
        /// <param name="span">The span containing the values to clamp.</param>
        /// <param name="min">The minimum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(Span<uint> span, uint min, uint max)
        {
            Span<uint> remainder = span.Slice(ClampReduce(span, min, max));

            if (remainder.Length > 0)
            {
                for (int i = 0; i < remainder.Length; i++)
                {
                    ref uint v = ref remainder[i];
                    v = Clamp(v, min, max);
                }
            }
        }

        /// <summary>
        /// Clamps the span values to the inclusive range of min and max.
        /// </summary>
        /// <param name="span">The span containing the values to clamp.</param>
        /// <param name="min">The minimum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(Span<int> span, int min, int max)
        {
            Span<int> remainder = span.Slice(ClampReduce(span, min, max));

            if (remainder.Length > 0)
            {
                for (int i = 0; i < remainder.Length; i++)
                {
                    ref int v = ref remainder[i];
                    v = Clamp(v, min, max);
                }
            }
        }

        /// <summary>
        /// Clamps the span values to the inclusive range of min and max.
        /// </summary>
        /// <param name="span">The span containing the values to clamp.</param>
        /// <param name="min">The minimum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(Span<float> span, float min, float max)
        {
            Span<float> remainder = span.Slice(ClampReduce(span, min, max));

            if (remainder.Length > 0)
            {
                for (int i = 0; i < remainder.Length; i++)
                {
                    ref float v = ref remainder[i];
                    v = Clamp(v, min, max);
                }
            }
        }

        /// <summary>
        /// Clamps the span values to the inclusive range of min and max.
        /// </summary>
        /// <param name="span">The span containing the values to clamp.</param>
        /// <param name="min">The minimum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(Span<double> span, double min, double max)
        {
            Span<double> remainder = span.Slice(ClampReduce(span, min, max));

            if (remainder.Length > 0)
            {
                for (int i = 0; i < remainder.Length; i++)
                {
                    ref double v = ref remainder[i];
                    v = Clamp(v, min, max);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ClampReduce<T>(Span<T> span, T min, T max)
            where T : unmanaged
        {
            if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count)
            {
                int remainder = ModuloP2(span.Length, Vector<T>.Count);
                int adjustedCount = span.Length - remainder;

                if (adjustedCount > 0)
                {
                    ClampImpl(span.Slice(0, adjustedCount), min, max);
                }

                return adjustedCount;
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ClampImpl<T>(Span<T> span, T min, T max)
            where T : unmanaged
        {
            ref T sRef = ref MemoryMarshal.GetReference(span);
            ref Vector<T> vsBase = ref Unsafe.As<T, Vector<T>>(ref MemoryMarshal.GetReference(span));
            var vmin = new Vector<T>(min);
            var vmax = new Vector<T>(max);

            int n = span.Length / Vector<T>.Count;
            int m = Modulo4(n);
            int u = n - m;

            for (int i = 0; i < u; i += 4)
            {
                ref Vector<T> vs0 = ref Unsafe.Add(ref vsBase, i);
                ref Vector<T> vs1 = ref Unsafe.Add(ref vs0, 1);
                ref Vector<T> vs2 = ref Unsafe.Add(ref vs0, 2);
                ref Vector<T> vs3 = ref Unsafe.Add(ref vs0, 3);

                vs0 = Vector.Min(Vector.Max(vmin, vs0), vmax);
                vs1 = Vector.Min(Vector.Max(vmin, vs1), vmax);
                vs2 = Vector.Min(Vector.Max(vmin, vs2), vmax);
                vs3 = Vector.Min(Vector.Max(vmin, vs3), vmax);
            }

            if (m > 0)
            {
                for (int i = u; i < n; i++)
                {
                    ref Vector<T> vs0 = ref Unsafe.Add(ref vsBase, i);
                    vs0 = Vector.Min(Vector.Max(vmin, vs0), vmax);
                }
            }
        }

        /// <summary>
        /// Pre-multiplies the "x", "y", "z" components of a vector by its "w" component leaving the "w" component intact.
        /// </summary>
        /// <param name="source">The <see cref="Vector4"/> to premultiply</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Premultiply(ref Vector4 source)
        {
            float w = source.W;
            source *= w;
            source.W = w;
        }

        /// <summary>
        /// Reverses the result of premultiplying a vector via <see cref="Premultiply(ref Vector4)"/>.
        /// </summary>
        /// <param name="source">The <see cref="Vector4"/> to premultiply</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnPremultiply(ref Vector4 source)
        {
            float w = source.W;
            source /= w;
            source.W = w;
        }

        /// <summary>
        /// Bulk variant of <see cref="Premultiply(ref Vector4)"/>
        /// </summary>
        /// <param name="vectors">The span of vectors</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Premultiply(Span<Vector4> vectors)
        {
#if SUPPORTS_RUNTIME_INTRINSICS
            if (Avx2.IsSupported && vectors.Length >= 2)
            {
                ref Vector256<float> vectorsBase =
                    ref Unsafe.As<Vector4, Vector256<float>>(ref MemoryMarshal.GetReference(vectors));

                // Divide by 2 as 4 elements per Vector4 and 8 per Vector256<float>
                ref Vector256<float> vectorsLast = ref Unsafe.Add(ref vectorsBase, (IntPtr)((uint)vectors.Length / 2u));

                while (Unsafe.IsAddressLessThan(ref vectorsBase, ref vectorsLast))
                {
                    Vector256<float> source = vectorsBase;
                    Vector256<float> multiply = Avx.Shuffle(source, source, ShuffleAlphaControl);
                    vectorsBase = Avx.Blend(Avx.Multiply(source, multiply), source, BlendAlphaControl);
                    vectorsBase = ref Unsafe.Add(ref vectorsBase, 1);
                }

                if (Modulo2(vectors.Length) != 0)
                {
                    // Vector4 fits neatly in pairs. Any overlap has to be equal to 1.
                    Premultiply(ref MemoryMarshal.GetReference(vectors.Slice(vectors.Length - 1)));
                }
            }
            else
#endif
            {
                ref Vector4 baseRef = ref MemoryMarshal.GetReference(vectors);

                for (int i = 0; i < vectors.Length; i++)
                {
                    ref Vector4 v = ref Unsafe.Add(ref baseRef, i);
                    Premultiply(ref v);
                }
            }
        }

        /// <summary>
        /// Bulk variant of <see cref="UnPremultiply(ref Vector4)"/>
        /// </summary>
        /// <param name="vectors">The span of vectors</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnPremultiply(Span<Vector4> vectors)
        {
#if SUPPORTS_RUNTIME_INTRINSICS
            if (Avx2.IsSupported && vectors.Length >= 2)
            {
                ref Vector256<float> vectorsBase =
                    ref Unsafe.As<Vector4, Vector256<float>>(ref MemoryMarshal.GetReference(vectors));

                // Divide by 2 as 4 elements per Vector4 and 8 per Vector256<float>
                ref Vector256<float> vectorsLast = ref Unsafe.Add(ref vectorsBase, (IntPtr)((uint)vectors.Length / 2u));

                while (Unsafe.IsAddressLessThan(ref vectorsBase, ref vectorsLast))
                {
                    Vector256<float> source = vectorsBase;
                    Vector256<float> multiply = Avx.Shuffle(source, source, ShuffleAlphaControl);
                    vectorsBase = Avx.Blend(Avx.Divide(source, multiply), source, BlendAlphaControl);
                    vectorsBase = ref Unsafe.Add(ref vectorsBase, 1);
                }

                if (Modulo2(vectors.Length) != 0)
                {
                    // Vector4 fits neatly in pairs. Any overlap has to be equal to 1.
                    UnPremultiply(ref MemoryMarshal.GetReference(vectors.Slice(vectors.Length - 1)));
                }
            }
            else
#endif
            {
                ref Vector4 baseRef = ref MemoryMarshal.GetReference(vectors);

                for (int i = 0; i < vectors.Length; i++)
                {
                    ref Vector4 v = ref Unsafe.Add(ref baseRef, i);
                    UnPremultiply(ref v);
                }
            }
        }
    }
}
