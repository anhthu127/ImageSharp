// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using BenchmarkDotNet.Attributes;

using SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder.ColorConverters;

namespace SixLabors.ImageSharp.Benchmarks.Codecs.Jpeg
{
    [Config(typeof(Config.ShortClr))]
    public class YCbCrColorConversion : ColorConversionBenchmark
    {
        public YCbCrColorConversion()
            : base(3)
        {
        }

        [Benchmark]
        public void Scalar()
        {
            var values = new JpegColorConverter.ComponentValues(this.input, 0);

            new JpegColorConverter.FromYCbCrBasic(8).ConvertToRgba(values, this.output);
        }

        [Benchmark(Baseline = true)]
        public void SimdVector()
        {
            var values = new JpegColorConverter.ComponentValues(this.input, 0);

            new JpegColorConverter.FromYCbCrVector(8).ConvertToRgba(values, this.output);
        }

        [Benchmark]
        public void SimdVector8()
        {
            var values = new JpegColorConverter.ComponentValues(this.input, 0);

            new JpegColorConverter.FromYCbCrVector8(8).ConvertToRgba(values, this.output);
        }

        [Benchmark]
        public void SimdVectorAvx2()
        {
            var values = new JpegColorConverter.ComponentValues(this.input, 0);

            new JpegColorConverter.FromYCbCrAvx2(8).ConvertToRgba(values, this.output);
        }
    }
}
