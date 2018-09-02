
using System;
using Bridge.Html5;

using ProductiveRage.Immutable;
using ProductiveRage.NonBlankTrimmedString;

namespace Bridge.React.Logotron.Components
{
    public sealed class CheckBox : PureComponent<CheckBox.Props>
    {
        public CheckBox(
            bool disabled,
            bool checked0, // checked est un mot réservé
            Action<string> onChange,
            Optional<NonBlankTrimmedString> className = 
                new Optional<NonBlankTrimmedString>(),
            Optional<NonBlankTrimmedString> title = 
                new Optional<NonBlankTrimmedString>()
            ) : base(new Props(
                className, title, disabled, checked0, onChange
                )) { }

        public sealed class Props : IAmImmutable
        {
            public Props(
                Optional<NonBlankTrimmedString> className,
                Optional<NonBlankTrimmedString> title,
                bool disabled,
                bool checked0,
                Action<string> onChange
                )
            {
                this.CtorSet(_ => _.ClassName, className);
                this.CtorSet(_ => _.Title, title);
                this.CtorSet(_ => _.Disabled, disabled);
                this.CtorSet(_ => _.Checked, checked0);
                this.CtorSet(_ => _.OnChange, onChange);
            }
            public Optional<NonBlankTrimmedString> ClassName { get; }
            public Optional<NonBlankTrimmedString> Title { get; }
            public bool Disabled { get; }
            public bool Checked { get; }
            public Action<string> OnChange { get; }
        }

        public override ReactElement Render()
        {
            // Margin : marge externe, Padding : marge interne
            var lblAtt = new LabelAttributes
            { Style = Style.Margin(20).Padding(5) }; //.FontSize(12)

            var iAtt = new InputAttributes {
                Type = InputType.Checkbox,
                ClassName = props.ClassName.Value,
                Checked = props.Checked,
                Disabled = props.Disabled,
                OnChange = e => props.OnChange(e.CurrentTarget.Value)
            };

            return
                DOM.Span(
                    new Attributes { ClassName = props.ClassName.Value },
                    DOM.Label(lblAtt, props.Title.Value),
                    DOM.Input(iAtt)
                );
        }
    }
}