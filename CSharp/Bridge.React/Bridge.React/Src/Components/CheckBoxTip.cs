
using System;
using Bridge.Html5;

using ProductiveRage.Immutable;
using ProductiveRage.NonBlankTrimmedString;

namespace Bridge.React.Logotron.Components
{
    public sealed class CheckBoxTip : PureComponent<CheckBoxTip.Props>
    {
        public CheckBoxTip(
            bool disabled,
            bool checked0, // checked est un mot réservé
            Action<string> onChange,
            string title,
            string tip,
            Optional<NonBlankTrimmedString> className = 
                new Optional<NonBlankTrimmedString>()
            ) : base(new Props(
                className, title, tip, disabled, checked0, onChange
                )) { }

        public sealed class Props : IAmImmutable
        {
            public Props(
                Optional<NonBlankTrimmedString> className,
                string title,
                string tip,
                bool disabled,
                bool checked0,
                Action<string> onChange
                )
            {
                this.CtorSet(_ => _.ClassName, className);
                this.CtorSet(_ => _.Title, title);
                this.CtorSet(_ => _.Tip, tip);
                this.CtorSet(_ => _.Disabled, disabled);
                this.CtorSet(_ => _.Checked, checked0);
                this.CtorSet(_ => _.OnChange, onChange);
            }
            public Optional<NonBlankTrimmedString> ClassName { get; }
            public string Title { get; }
            public string Tip { get; }
            public bool Disabled { get; }
            public bool Checked { get; }
            public Action<string> OnChange { get; }
        }

        public override ReactElement Render()
        {
            var iAtt = new InputAttributes {
                Type = InputType.Checkbox,
                ClassName = props.ClassName.Value,
                Checked = props.Checked,
                Disabled = props.Disabled,
                OnChange = e => props.OnChange(e.CurrentTarget.Value)
            };

            if (!string.IsNullOrEmpty(props.Tip)) 
            {
                var tooltipAtt = new Attributes { ClassName = "tooltip" };
                var tooltiptextAtt = new Attributes { ClassName = "tooltiptext" };
            
                return
                    DOM.Span(
                        new Attributes { ClassName = props.ClassName.Value },
                        DOM.Div(tooltipAtt,
                            props.Title,
                            DOM.Span(tooltiptextAtt, props.Tip)
                        ),
                        DOM.Input(iAtt)
                    );
            }
            else
            {
                // Margin : marge externe, Padding : marge interne
                var lblAtt = new LabelAttributes
                { Style = Style.Margin(20).Padding(5) }; //.FontSize(12)

                return
                    DOM.Span(
                        new Attributes { ClassName = props.ClassName.Value },
                        DOM.Label(lblAtt, props.Title),
                        DOM.Input(iAtt)
                    );
            }
        }
    }
}