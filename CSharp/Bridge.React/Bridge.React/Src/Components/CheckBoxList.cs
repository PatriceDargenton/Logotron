
using System;
using System.Collections.Generic; // List<string>
using System.Linq; // .Select
using Bridge.Html5;
using Bridge.React.Logotron.API; // IReadAndWriteItems

using ProductiveRage.Immutable;
using ProductiveRage.NonBlankTrimmedString;

namespace Bridge.React.Logotron.Components
{
    public sealed class CheckBoxList : PureComponent<CheckBoxList.Props>
    {
        public CheckBoxList(
            bool disabled,
            bool[] checkBoxArray,
            IReadAndWriteItems itemAPI,
            Action<string> onChange,
            Optional<NonBlankTrimmedString> className =
                new Optional<NonBlankTrimmedString>(),
            Optional<NonBlankTrimmedString> titre = 
                new Optional<NonBlankTrimmedString>()
            ) : base(new Props(
                className, titre, disabled, onChange, checkBoxArray, itemAPI)) { }

        public sealed class Props : IAmImmutable
        {
            public Props(
                Optional<NonBlankTrimmedString> className,
                Optional<NonBlankTrimmedString> titre,
                bool disabled,
                Action<string> onChange,
                bool[] checkBoxArray,
                IReadAndWriteItems itemAPI)
            {
                this.CtorSet(_ => _.ClassName, className);
                this.CtorSet(_ => _.Titre, titre);
                this.CtorSet(_ => _.Disabled, disabled);
                this.CtorSet(_ => _.OnChange, onChange);
                this.CtorSet(_ => _.CheckBoxArray, checkBoxArray);
                this.CtorSet(_ => _.ItemAPI, itemAPI);
            }

            public Optional<NonBlankTrimmedString> ClassName { get; }
            public Optional<NonBlankTrimmedString> Titre { get; }
            public bool Disabled { get; }
            public Action<string> OnChange { get; }
            public bool[] CheckBoxArray { get; }
            public IReadAndWriteItems ItemAPI { get; }

        }
        
        private ReactElement GetInputX(int index, string txt)
        {
            var inputX = DOM.Input(
                new InputAttributes{
                    Type = InputType.Radio,
                    ClassName = props.ClassName.Value,
                    Value = txt,
                    Checked = props.CheckBoxArray[index],
                    Disabled = props.Disabled,
                    OnChange = e => props.OnChange(e.CurrentTarget.Value)
                });
            return inputX;
        }

        public static bool[] SetCheckBoxArray(string itemSelected, 
            bool[] checkBoxArraySrc, List<string> lstItems, bool oneItemRequired) 
        {
            // Basculer le booléen selectionné dans un tableau de booléens
            var checkBoxArrayDest = (bool[])checkBoxArraySrc.Clone();
            int numItemMax = checkBoxArraySrc.GetUpperBound(0);
            int nbItemsSelected = 0;
            for (int i = 0; i <= numItemMax; i++) {
                if (itemSelected == lstItems[i])
                    checkBoxArrayDest[i] = !checkBoxArrayDest[i];
                if (checkBoxArrayDest[i]) nbItemsSelected++;
            }
            if (oneItemRequired && nbItemsSelected == 0) return checkBoxArraySrc;
            return checkBoxArrayDest;
        }

        public override ReactElement Render()
        {
            
            // Div : Vertical : ToDo : faire une option facultative
            //var itemsElements = props.ItemApi.GetItems()
            //    .Select(
            //        idAndString => DOM.Div(
            //        new Attributes { Key = idAndString.Item1, ClassName = "ItemCBLG" },
            //        DOM.Label(new LabelAttributes { }, idAndString.Item2),
            //        GetInputX(idAndString.Item1, idAndString.Item2)
            //        )
            //    );

            // Span : Horizontal
            var itemsElements = props.ItemAPI.GetItems()
                .Select(
                    idAndString => DOM.Span(
                        new Attributes{ 
                            Key = idAndString.Item1, 
                            ClassName = props.ClassName.Value
                        },
                        DOM.Label(new LabelAttributes{ }, idAndString.Item2),
                            GetInputX(idAndString.Item1, idAndString.Item2)
                    )
                );

            // Margin : marge externe, Padding : marge interne
            var lblAtt = new LabelAttributes
            { Style = Style.Margin(20).Padding(5) }; //.FontSize(12)

            return
                DOM.Span(
                    new Attributes { ClassName = props.Titre.Value },
                    DOM.Label(lblAtt, props.Titre.Value),
                    DOM.Span(null, itemsElements)
                );
        }
    }
}