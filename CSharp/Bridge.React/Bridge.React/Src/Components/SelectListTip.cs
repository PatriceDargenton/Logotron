
// Liste de sélection avec une infobulle (tip)

// https://developer.mozilla.org/fr/docs/Web/HTML/Element/select

// <select id="mySelect" size="4">
//   <option>Apple</option>
//   <option>Pear</option>
//   <option>Banana</option>
//   <option>Orange</option>
// </select>

// <select name="list_box_name" size="number_of_options">
// <option value="Option1">Option 1</option>
// <option value="Option2">Option 2</option>
// <option value="Option3">Option 3</option>
// ...
// </select>

// Comment appliquer un style ?
//<style>
// .bloc { display:inline-block; vertical-align:top; overflow:hidden; border:solid grey 1px; }
// .bloc select { padding:10px; margin:-5px -20px -5px -5px; }
//</style>

//<!-- Années -->
//<div class="bloc">
//  <select name = "year" size="5">
//    <option value = "2010" >2010</option>
//    <option value = "2011" >2011</option>
//    <option value = "2012" SELECTED>2012</option>
//    <option value = "2013" >2013</option>
//    <option value = "2014" >2014</option>
//   </select>
//</div>

using System;
using System.Collections.Generic;
using System.Linq; // .Select
using Bridge.React.Logotron.API; // IReadAndWriteItems

using ProductiveRage.Immutable;
using ProductiveRage.NonBlankTrimmedString;

namespace Bridge.React.Logotron.Components
{
    public sealed class SelectListTip : PureComponent<SelectListTip.Props>
    {
        public SelectListTip(
            string title,
            string tip,
            IReadAndWriteItems itemAPI,
            Action<string> onChange,
            string itemSelected = "",
            bool disabled = false,
            bool multiple = false,
            bool oneListPerLine = false,
            Optional<bool[]> checkBoxArray = new Optional<bool[]>(),
            Optional<NonBlankTrimmedString> className =
                new Optional<NonBlankTrimmedString>()
            ) : base(new Props(
                className, title, tip, disabled, multiple, oneListPerLine,
                onChange, itemSelected, checkBoxArray, itemAPI))
        { }

        public sealed class Props : IAmImmutable
        {
            public Props(
                Optional<NonBlankTrimmedString> className,
                string title,
                string tip,
                Optional<bool> disabled,
                Optional<bool> multiple,
                Optional<bool> oneListPerLine, // Isolé : Fieldset : saut de ligne
                Action<string> onChange,
                Optional<string> itemSelected,
                Optional<bool[]> checkBoxArray,
                IReadAndWriteItems itemApi)
            {
                this.CtorSet(_ => _.ClassName, className);
                this.CtorSet(_ => _.Title, title);
                this.CtorSet(_ => _.Tip, tip);
                this.CtorSet(_ => _.Disabled, disabled);
                this.CtorSet(_ => _.Multiple, multiple);
                this.CtorSet(_ => _.OneListPerLine, oneListPerLine);
                this.CtorSet(_ => _.OnChange, onChange);
                this.CtorSet(_ => _.ItemSelected, itemSelected);
                this.CtorSet(_ => _.CheckBoxArray, checkBoxArray);
                this.CtorSet(_ => _.ItemApi, itemApi);
            }

            public string Title { get; }
            public string Tip { get; }
            public Action<string> OnChange { get; }
            public IReadAndWriteItems ItemApi { get; }

            public Optional<NonBlankTrimmedString> ClassName { get; }
            public Optional<bool> Disabled { get; }
            public Optional<bool> Multiple { get; }
            public Optional<bool> OneListPerLine { get; }
            public Optional<string> ItemSelected { get; } // Multiple = false
            public Optional<bool[]> CheckBoxArray { get; } // Multiple = true
            
        }

        private ReactElement GetOptionX(int index, string txt)
        {
            var optionX = DOM.Option(
                new OptionAttributes { Key = index, Value = txt },
                txt
            );
            return optionX;
        }

        private string[] SetItems(Optional<bool[]> checkBoxArray)
        {
            var itemsList = props.ItemApi.GetItemList();
            var itemsSelected = new List<string>();
            int nbItems = itemsList.Count;
            for (int i = 0; i < nbItems; i++)
            {
                string item = itemsList[i];
                var itemVal = checkBoxArray.Value[i];
                if (itemVal) itemsSelected.Add(item);
            }
            var checkBoxArrayDest = itemsSelected.ToArray();
            return checkBoxArrayDest;
        }

        public static bool[] SetItem(string newItem, bool[] checkBoxArray, 
            List<string> itemsList)
        {
            var itemsSelectedList = new List<bool> { };
            int nbItems = itemsList.Count;
            for (int i = 0; i < nbItems; i++)
            {
                string item = itemsList[i];
                bool chk = checkBoxArray[i];
                if (item == newItem) chk = !chk;
                itemsSelectedList.Add(chk);
            }
            var checkBoxArrayDest = itemsSelectedList.ToArray();
            return checkBoxArrayDest;
        }

        public override ReactElement Render()
        {
            var itemsElements = props.ItemApi.GetItems().Select(idAndString =>
                GetOptionX(idAndString.Item1, idAndString.Item2));

            // Margin : marge externe, Padding : marge interne
            var lblAtt = new LabelAttributes
            { Style = Style.Margin(20).Padding(5) }; //.FontSize(12)

            var tooltipAtt = new Attributes { ClassName = "tooltip" };
            var tooltiptextAtt = new Attributes { ClassName = "tooltiptext" };

            if (props.Multiple.Value)
            {
                var selAttMultiple = new SelectAttributes
                {
                    ClassName = props.ClassName.Value,
                    Name = props.Title.ToString(),
                    Values = SetItems(props.CheckBoxArray),
                    Multiple = props.Multiple.Value,
                    Size = itemsElements.Count(),
                    Disabled = props.Disabled.Value,
                    // Pour cacher le scroll vertical, il faudrait appliquer un ReactStyle :
                    // OverFlow = Hidden, : appartient à ReactStyle
                    //Padding = 10,
                    //ReadOnly = true,
                    OnChange = e => props.OnChange(e.CurrentTarget.Value),
                };

                if (props.OneListPerLine.Value)
                {
                    if (string.IsNullOrEmpty(props.Tip))
                        return
                            DOM.FieldSet(
                                new FieldSetAttributes { ClassName = props.ClassName.Value },
                                DOM.Legend(null, props.Title.ToString()),
                                DOM.Select(selAttMultiple, itemsElements)
                            );
                    else
                        return
                            DOM.FieldSet(
                                new FieldSetAttributes { ClassName = props.ClassName.Value },
                                DOM.Div(tooltipAtt, props.Title,
                                    DOM.Span(tooltiptextAtt, props.Tip)),
                                DOM.Select(selAttMultiple, itemsElements)
                            );
                }

                if (string.IsNullOrEmpty(props.Tip))
                    return
                        DOM.Span(
                            new Attributes { ClassName = props.ClassName.Value },
                            DOM.Label(lblAtt, props.Title.ToString()),
                            DOM.Select(selAttMultiple, itemsElements)
                        );
                else
                    return
                    DOM.Span(
                        new Attributes { ClassName = props.ClassName.Value },
                        DOM.Div(tooltipAtt, props.Title,
                            DOM.Span(tooltiptextAtt, props.Tip)),
                        DOM.Select(selAttMultiple, itemsElements)
                    );
            }

            var selAttSimple = new SelectAttributes
            {
                ClassName = props.ClassName.Value,
                Name = props.Title.ToString(),
                Value = props.ItemSelected.Value,
                Multiple = props.Multiple.Value,
                Size = itemsElements.Count(),
                Disabled = props.Disabled.Value,
                OnChange = e => props.OnChange(e.CurrentTarget.Value)
            };

            if (props.OneListPerLine.Value)
            {
                // Saut de ligne, un seul SelectList sur une ligne
                // The <fieldset> tag is used to group related elements in a form.
                // The <fieldset> tag draws a box around the related elements.
                // https://www.w3schools.com/tags/tag_fieldset.asp

                if (string.IsNullOrEmpty(props.Tip))
                    return
                        DOM.FieldSet(
                            new FieldSetAttributes { ClassName = props.ClassName.Value },
                            DOM.Legend(null, props.Title.ToString()),
                            DOM.Select(selAttSimple, itemsElements)
                        );
                else
                    return
                        DOM.FieldSet(
                            new FieldSetAttributes { ClassName = props.ClassName.Value },
                            DOM.Div(tooltipAtt, props.Title,
                                DOM.Span(tooltiptextAtt, props.Tip)),
                            DOM.Select(selAttSimple, itemsElements)
                        );
            }

            // Pas de saut de ligne, on peut avoir plusieurs SelectList sur une ligne
            if (string.IsNullOrEmpty(props.Tip))
                return
                    DOM.Span(
                        new Attributes { ClassName = props.ClassName.Value },
                        DOM.Label(lblAtt, props.Title.ToString()),
                        DOM.Select(selAttSimple, itemsElements)
                    );
            else
                return
                    DOM.Span(
                        new Attributes { ClassName = props.ClassName.Value },
                        DOM.Div(tooltipAtt, props.Title,
                            DOM.Span(tooltiptextAtt, props.Tip)),
                        DOM.Select(selAttSimple, itemsElements)
                    );
        }
    }
}