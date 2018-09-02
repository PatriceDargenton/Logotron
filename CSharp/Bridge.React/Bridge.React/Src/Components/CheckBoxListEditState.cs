
using Bridge.React.Logotron.API;
using ProductiveRage.Immutable;
using System.Collections.Generic;

namespace Bridge.React.Logotron.ViewModels
{

    public sealed class CheckBoxEditState : IAmImmutable
    {
        public CheckBoxEditState(bool newChecked)
        {
            this.CtorSet(_ => _.Checked, newChecked);
        }
        public bool Checked { get; }
    }

    public sealed class CheckBoxListMultipleEditState : IAmImmutable
    {
        public CheckBoxListMultipleEditState(bool[] newCheckBoxArray)
        {
            this.CtorSet(_ => _.CheckBoxArray, newCheckBoxArray);
        }
        public bool[] CheckBoxArray { get; }
    }
    
    public sealed class CheckBoxListSimpleEditState : IAmImmutable
    {
        public CheckBoxListSimpleEditState(string text, List<string> lstItems)
        {
            this.CtorSet(_ => _.Text, text);
            this.CtorSet(_ => _.LstItems, lstItems);
        }
        public string Text { get; }
        public List<string> LstItems { get; }
    }
}
