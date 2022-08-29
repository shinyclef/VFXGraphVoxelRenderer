using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;

namespace UnityEditor.VFX.Block
{
    [VFXInfo(category = "Custom", experimental = true)]
    internal class GetBufferIndex : VFXBlock
    {
        [Tooltip("Name of the attribute to save the Buffer Index value.")]
        [VFXSetting(VFXSettingAttribute.VisibleFlags.InInspector), Delayed]
        public string IndexAttribute = "BufferIndex";
        
        [Tooltip("If you get a compile error, take a look at the Update Particle compilation output (expand the particle effect to find it), " +
                 "and find the searchBuffer variable name, check its suffix letter and match this param to it.")]
        [VFXSetting(VFXSettingAttribute.VisibleFlags.InInspector), Delayed]
        public string BufferCodeGenSuffixLetter = "a";
        
        public override string libraryName => "Get Buffer Index";
        public override string name => $"Set {IndexAttribute} = Index in Buffer";
        public override VFXContextType compatibleContexts => VFXContextType.Update;
        public override VFXDataType compatibleData => VFXDataType.Particle;
        
        private VFXAttribute IndexAtt => new(IndexAttribute, VFXValueType.Int32);

        public override IEnumerable<VFXAttributeInfo> attributes
        {
            get { yield return new VFXAttributeInfo(IndexAtt, VFXAttributeMode.Write); }
        }

        public class InputProperties
        {
            [Tooltip("Sets the Graphics Buffer to search.")]
            public GraphicsBuffer searchBuffer = null;
            [Tooltip("How many elements to iterate in the buffer.")]
            public int bufferCount;
            [Tooltip("Sets the value to look for.")]
            public uint searchVal;
        }
        
        public override IEnumerable<VFXNamedExpression> parameters
        {
            get
            {
                HackInjectColour();
                foreach (VFXNamedExpression input in GetExpressionsFromSlots(this))
                {
                    yield return input;
                }
            }
        }

        // HACK OF THE CENTURY - OH GOD
        private void HackInjectColour()
        {
            if (new System.Diagnostics.StackTrace().GetFrame(2).GetMethod().Name != "OnInspectorGUI")
            {
                return;
            }
            
            Type type = typeof(VFXSlotContainerEditor.Styles);
            FieldInfo info = type.GetField("valueTypeColors", BindingFlags.NonPublic | BindingFlags.Static);
            object value = info?.GetValue(null);
            Assert.IsNotNull(value, "Value was null. Maybe the VisualEffectGraph package was updated and this is no longer needed? Try opening a graph with the SetBufferContainsAttribute node (e.g. projectileVfx?)");
            Dictionary<VFXValueType, Color> valueTypeColors = (Dictionary<VFXValueType, Color>)value;
            if (!valueTypeColors.ContainsKey(VFXValueType.Buffer))
            {
                Debug.Log(".Adding buffer colour!.");
                valueTypeColors.Add(VFXValueType.Buffer, valueTypeColors[VFXValueType.None]);
            }
        }

        public override string source
        {
            get =>
@"int index = -1;
for (int i = 0; i < bufferCount; i++)
{
    if (searchBuffer_" + BufferCodeGenSuffixLetter + @".Load(i) == searchVal)
    {
        index = i;
        break;
    }
}

" +
$"{IndexAtt.name} = index;";
        }

        private static string GenerateLocalAttributeName(string name) => 
            $"_{name[0].ToString().ToUpper(CultureInfo.InvariantCulture)}{name.Substring(1)}";
    }
}
