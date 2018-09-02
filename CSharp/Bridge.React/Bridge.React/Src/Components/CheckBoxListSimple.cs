
using System;
using System.Linq; // .Select
using Bridge.Html5;
using Bridge.React.Logotron.API; // IReadAndWriteItems

using ProductiveRage.Immutable;
using ProductiveRage.NonBlankTrimmedString;

namespace Bridge.React.Logotron.Components
{
    public sealed class CheckBoxListSimple : PureComponent<CheckBoxListSimple.Props>
    {
        public CheckBoxListSimple(
            bool disabled,
            string selectedItem,
            IReadAndWriteItems itemAPI,
            Action<string> onChange,
            Optional<NonBlankTrimmedString> className =
                new Optional<NonBlankTrimmedString>(),
            string title = ""
            ) : base(new Props(
                className, title, disabled, onChange, selectedItem, itemAPI)) 
        { }

        public sealed class Props : IAmImmutable
        {
            public Props(
                Optional<NonBlankTrimmedString> className,
                string title,
                bool disabled,
                Action<string> onChange,
                string selectedItem,
                IReadAndWriteItems itemAPI)
            {
                this.CtorSet(_ => _.ClassName, className);
                this.CtorSet(_ => _.Title, title);
                this.CtorSet(_ => _.Disabled, disabled);
                this.CtorSet(_ => _.OnChange, onChange);
                this.CtorSet(_ => _.SelectedItem, selectedItem);
                this.CtorSet(_ => _.ItemAPI, itemAPI);
            }

            public Optional<NonBlankTrimmedString> ClassName { get; }
            public string Title { get; }
            public bool Disabled { get; }
            public Action<string> OnChange { get; }
            public string SelectedItem { get; }
            public IReadAndWriteItems ItemAPI { get; }
        }
        
        private ReactElement GetInputX(int index, string txt)
        {
            var inputX = DOM.Input(
                new InputAttributes{
                    Type = InputType.Radio,
                    ClassName = props.ClassName.Value,
                    Value = txt,
                    Checked = bItemSelect(index),
                    Disabled = props.Disabled,
                    OnChange = e => props.OnChange(e.CurrentTarget.Value)
                });
            return inputX;
        }
        
        private bool bItemSelect(int index)
        {
            // Return the Boolean selected in the item table
            // Retourner le booléen selectionné dans le tableau d'item
            var selItem = props.SelectedItem;
            var items = props.ItemAPI.GetItemList();
            int selIndex = -1;
            int indexNum = 0;
            foreach (string item in items)
            {
                if (item == selItem) { selIndex = indexNum; break; }
                indexNum++;
            }
            bool val = (index == selIndex);
            return val;
        }

        public override ReactElement Render()
        {
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

            if (string.IsNullOrEmpty(props.Title)) 
                return
                    DOM.Span(
                        new Attributes { ClassName = props.ClassName.Value },
                        DOM.Span(null, itemsElements)
                    );
            else
            {
                // Margin : marge externe, Padding : marge interne
                var lblAtt = new LabelAttributes
                    { Style = Style.Margin(20).Padding(5) }; //.FontSize(12)
                return
                    DOM.Span(
                        new Attributes { ClassName = props.ClassName.Value },
                        DOM.Label(lblAtt, props.Title),
                        DOM.Span(null, itemsElements)
                    );
            }
        }
    }
}