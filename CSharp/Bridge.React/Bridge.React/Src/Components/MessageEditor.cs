
using System;
using Bridge.React.Logotron.API;

using ProductiveRage.Immutable;
using ProductiveRage.NonBlankTrimmedString;

namespace Bridge.React.Logotron.Components
{
    public sealed class MessageEditor : PureComponent<MessageEditor.Props>
    {

        public MessageEditor(
            string title,
            string content,
            string className,
            bool disabled,
            Action<MessageDetails> onChange,
            Action onSave) : base(
                new Props(className, title, content, disabled, onChange, onSave)) { }

        public MessageEditor(Props props) : base(props) { }

        public sealed class Props : IAmImmutable
        {
            public Props(
                string className,
                string title,
                string content,
                bool disabled,
                Action<MessageDetails> onChange,
                Action onSave
                )
            {
                this.CtorSet(_ => _.ClassName, className);
                this.CtorSet(_ => _.Title, title);
                this.CtorSet(_ => _.Content, content);
                this.CtorSet(_ => _.Disabled, disabled);
                this.CtorSet(_ => _.OnChange, onChange);
                this.CtorSet(_ => _.OnSave, onSave);
            }
            public string ClassName { get; }
            public string Title { get; }
            public string Content { get; }
            public bool Disabled { get; }
            public Action<MessageDetails> OnChange { get; }
            public Action OnSave { get; }
        }

        public override ReactElement Render()
        {
            var fa = new FieldSetAttributes { ClassName = props.ClassName };
            var lgd = DOM.Legend(null,
                string.IsNullOrWhiteSpace(
                    props.Title + " : " + props.Content) ? "Untitled" :
                    props.Title + " : " + props.Content);
            var la = new Attributes { ClassName = "label" };
            var tiTitle = new TextInput
            (
                disabled: false,
                content: props.Title,
                onChange: e => props.OnChange(new MessageDetails(e, props.Content)),
                className: new NonBlankTrimmedString("Title")
            );
            var tiContent = new TextInput
            (
                disabled: false,
                content: props.Content,
                onChange: e => props.OnChange(new MessageDetails(props.Title, e)),
                className: new NonBlankTrimmedString("Content")
            );
            var ba = new ButtonAttributes
            {
                Disabled = props.Disabled,
                OnClick = e => props.OnSave()
            };
            return DOM.FieldSet(
                fa,
                lgd,
                DOM.Span(la, "Title"), tiTitle,
                DOM.Span(la, "Content"), tiContent,
                DOM.Button(ba, "Save")
            );
        }
    }
}