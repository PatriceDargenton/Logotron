
using System;
using System.Linq; // Count()
using Bridge.React.Logotron.API; // IReadAndWriteItems
using ProductiveRage.Immutable;
using ProductiveRage.NonBlankTrimmedString;

namespace Bridge.React.Logotron.Components
{
    public sealed class ScrollTextBox : PureComponent<ScrollTextBox.Props>
    {

        private Bridge.Html5.HTMLTextAreaElement _element;
        private string _lastLine = "";
        private int _selStart, _selEnd;

        public ScrollTextBox(
                bool disabled,
                IReadAndWriteItems itemAPI,
                Action<string> onChangeSTB,
                Action<string> onChange,
                Action onSave,
                string inputValueSTB,
                Optional<NonBlankTrimmedString> className =
                    new Optional<NonBlankTrimmedString>(),
                Optional<NonBlankTrimmedString> titre =
                    new Optional<NonBlankTrimmedString>()
                ) : base(new Props(
                    className, titre, disabled, itemAPI, inputValueSTB,
                    onChangeSTB, onChange, onSave
                    )) { }
        
        public ScrollTextBox(Props props) : base(props) { }

        public sealed class Props : IAmImmutable
        {
            public Props(
                Optional<NonBlankTrimmedString> className,
                Optional<NonBlankTrimmedString> titre,
                bool disabled,
                IReadAndWriteItems itemApi,
                string inputValueSTB,
                Action<string> onChangeSTB,
                Action<string> onChange,
                Action onSave
                )
            {
                this.CtorSet(_ => _.ClassName, className);
                this.CtorSet(_ => _.Titre, titre);
                this.CtorSet(_ => _.Disabled, disabled);
                this.CtorSet(_ => _.ItemApi, itemApi);
                this.CtorSet(_ => _.InputValueSTB, inputValueSTB);
                this.CtorSet(_ => _.OnChangeSTB, onChangeSTB);
                this.CtorSet(_ => _.OnChange, onChange);
                this.CtorSet(_ => _.OnSave, onSave);
            }
            public Optional<NonBlankTrimmedString> ClassName { get; }
            public bool Disabled { get; }
            public Optional<NonBlankTrimmedString> Titre { get; }
            public IReadAndWriteItems ItemApi { get; }
            public Action<string> OnChangeSTB { get; }
            public Action<string> OnChange { get; }
            public Action OnSave { get; }
            public string InputValueSTB { get; }
            public IReadAndWriteMessages MessageApi { get; }
        }

        public string ListToString(IReadAndWriteItems ItemApi)
        {
            var sb = new System.Text.StringBuilder();
            var lst = ItemApi.GetItemList();
            int numElemMax = lst.Count();
            int nbCar = 0;
            for (int i = 0; i < numElemMax; i++) {
                string txt = lst[i];
                //string txt = lst[numElemMax-i-1]; // Reverse
                _lastLine = txt;
                int txtLen = txt.Length;
                _selStart = nbCar;
                _selEnd = nbCar + txtLen-1;
                sb.AppendLine(txt);
                nbCar += txtLen;
            }
            string text = sb.ToString();
            return text;
        }

        protected override void ComponentDidMount()
        {
            //ScrollToBottom();
        }

        protected override void ComponentDidUpdate(Props previousProps)
        {
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (_element == null) return;

            //if (LogotronLib.clsConst.bDebug)
            //    Console.WriteLine("ScrollToBottom : last line : " +
            //        _lastLine + ", start = " + _selStart + ", end = " + _selEnd);

            // Comment faire autrement ?
            for (int i = 0; i < 200; i++) 
                _element.ScrollIntoView(alignWithTop:false); 
            //_element.SetSelectionRange(_selStart, _selEnd);
            //_element.SetSelectionRange(_selStart, _selEnd, direction:"forward");
            //_element.SetSelectionRange(_selStart, _selEnd, direction: "backward");
            //_element.SelectionStart = _selStart;
            //_element.SelectionEnd = _selEnd;

            //_element.Select(); // Works fine

        }

        public override ReactElement Render()
        {
            var taAtt = new TextAreaAttributes {
                Style = Style.Margin(5).Padding(5).FontSize(18).Width(500),
                ClassName = props.Titre.Value,
                Name = props.Titre.Value,
                //Cols = 10,
                Wrap = Html5.Wrap.Soft,
                Rows = 5,
                MaxLength =10,
                Value = ListToString(props.ItemApi),
                OnChange = e => props.OnChangeSTB(e.CurrentTarget.Value),
                Ref = e => _element = e,
                //SelectionStart = _selStart,
                //SelectionEnd = _selEnd
            };

            var ti = new TextInput(
                disabled: props.Disabled,
                content: props.InputValueSTB.ToString(),
                onChange: e => props.OnChange(e),
                className: new NonBlankTrimmedString("Content")
            );

            var ba = new ButtonAttributes
            {
                Disabled = string.IsNullOrWhiteSpace(props.InputValueSTB.ToString()),
                OnClick = e => props.OnSave()
                //OnClick = async(e) =>
                //{
                //    props.OnSave();
                //    await props.MessageApi.SaveMessage(iDelaiMsec: 1000);
                //    _element.Select();
                //    //ScrollToBottom();
                //},
            };

            return
                DOM.FieldSet(
                    new FieldSetAttributes { ClassName = props.Titre.Value },
                    DOM.Legend(null, props.Titre.Value), // ToDo : éviter saut de ligne
                    DOM.Span(new Attributes { ClassName = "label" }, "Add item : "),
                    ti,
                    DOM.TextArea(taAtt),
                    DOM.Button(ba, "Add")
                );
        }
    }
}