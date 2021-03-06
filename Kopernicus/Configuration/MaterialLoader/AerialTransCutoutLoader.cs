// Material wrapper generated by shader translator tool
using System;
using System.Reflection;
using UnityEngine;

using Kopernicus.MaterialWrapper;

namespace Kopernicus
{
    namespace Configuration
    {
        public class AerialTransCutoutLoader : AerialTransCutout
        {
            // Main Color, default = (1,1,1,1)
            [ParserTarget("color", optional = true)]
            private ColorParser colorSetter
            {
                set { base.color = value.value; }
            }

            // Base (RGB) Trans (A), default = "white" {}
            [ParserTarget("mainTex", optional = true)]
            private Texture2DParser mainTexSetter
            {
                set { base.mainTex = value.value; }
            }

            [ParserTarget("mainTexScale", optional = true)]
            private Vector2Parser mainTexScaleSetter
            {
                set { base.mainTexScale = value.value; }
            }

            [ParserTarget("mainTexOffset", optional = true)]
            private Vector2Parser mainTexOffsetSetter
            {
                set { base.mainTexOffset = value.value; }
            }

            // Alpha cutoff, default = 0.5
            [ParserTarget("texCutoff", optional = true)]
            private NumericParser<float> texCutoffSetter
            {
                set { base.texCutoff = value.value; }
            }

            // AP Fog Color, default = (0,0,1,1)
            [ParserTarget("fogColor", optional = true)]
            private ColorParser fogColorSetter
            {
                set { base.fogColor = value.value; }
            }

            // AP Height Fall Off, default = 1
            [ParserTarget("heightFallOff", optional = true)]
            private NumericParser<float> heightFallOffSetter
            {
                set { base.heightFallOff = value.value; }
            }

            // AP Global Density, default = 1
            [ParserTarget("globalDensity", optional = true)]
            private NumericParser<float> globalDensitySetter
            {
                set { base.globalDensity = value.value; }
            }

            // AP Atmosphere Depth, default = 1
            [ParserTarget("atmosphereDepth", optional = true)]
            private NumericParser<float> atmosphereDepthSetter
            {
                set { base.atmosphereDepth = value.value; }
            }

            // Constructors
            public AerialTransCutoutLoader () : base() { }
            public AerialTransCutoutLoader (string contents) : base (contents) { }
            public AerialTransCutoutLoader (Material material) : base(material) { }
        }
    }
}
