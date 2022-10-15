using System.ComponentModel;
using Sirenix.OdinInspector;
using UnityEngine;
using Values.Variables;

namespace Values.References
{
    [InlineProperty]
    [LabelWidth(75)]
    public abstract class Reference<Value, Variable> where Variable: Variable<Value>
    {
        [HorizontalGroup("Reference", MaxWidth = 100)] 
        [ValueDropdown("valueList")] 
        [HideLabel] 
        [SerializeField] 
        protected bool useValue = true;

        [ShowIf("useValue", Animate = false)] 
        [HorizontalGroup("Reference")]
        [HideLabel] 
        [SerializeField]
        protected Value _value;

        [HideIf("useValue", Animate = false)]
        [HorizontalGroup("Reference")]
        [OnValueChanged("UpdateAsset")]
        [HideLabel]
        [SerializeField]
        protected Variable asset;

        [ShowIf("@asset != null && useValue == false")]
        [LabelWidth(100)]
        [SerializeField]
        protected bool editAsset = false;

        [ShowIf("@asset != null && useValue == false")]
        [EnableIf("editAsset")]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        [SerializeField]
        protected Variable _asset;

        private static ValueDropdownList<bool> valueList = new ValueDropdownList<bool>() {
            { "Value", true },
            { "Reference", false },
        };

        public Value value {
            get {
                if (asset == null || useValue) {
                    return _value;
                } else {
                    return asset.value;
                }
            }
        }

        protected void UpdateAsset()
        {
            _asset = asset;
        }

        public static implicit operator Value(Reference<Value, Variable> reference)
        {
            return reference.value;
        }
    }
}