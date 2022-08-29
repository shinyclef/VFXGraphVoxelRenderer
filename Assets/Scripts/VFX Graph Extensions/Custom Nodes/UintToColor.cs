using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace UnityEditor.VFX.Block
{
    [VFXInfo(category = "Custom", experimental = true)]
    internal class UintToColor : VFXBlock
    {
        public override string libraryName => "Set Color Uint";
        public override string name => "Set Color Uint";
        public override VFXContextType compatibleContexts => VFXContextType.All;
        public override VFXDataType compatibleData => VFXDataType.Particle;
        
        public class InputProperties
        {
            [Tooltip("A Color32 packed in a uint.")]
            public uint packedColor;
        }
        
        public override IEnumerable<VFXAttributeInfo> attributes
        {
            get
            {
                yield return new VFXAttributeInfo(VFXAttribute.Color, VFXAttributeMode.ReadWrite);
            }
        }

        public override string source =>
            "color = float4(((packedColor & 0xff000000) >> 24), ((packedColor & 0xff0000) >> 16), ((packedColor & 0xff00) >> 8), ((packedColor & 0xff))) / 255;";

        private static string GenerateLocalAttributeName(string name) => 
            $"_{name[0].ToString().ToUpper(CultureInfo.InvariantCulture)}{name.Substring(1)}";
    }
}
