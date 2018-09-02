
using System.Linq;
using Bridge.React.Logotron.API;
using ProductiveRage.Immutable;

namespace Bridge.React.Logotron.Components
{
	public sealed class MessageHistory : PureComponent<MessageHistory.Props>
	{
		public MessageHistory(
            NonNullList<SavedMessageDetails> messages,
            string className
            )
			: base(new Props(
                className,
                messages
                )) { }

        public sealed class Props : IAmImmutable
        {
            public Props(string className,
                NonNullList<SavedMessageDetails> messages
                )
            {
                this.CtorSet(_ => _.ClassName, className);
                this.CtorSet(_ => _.Messages, messages);
            }
            public string ClassName { get; }
            public NonNullList<SavedMessageDetails> Messages { get; }
        }

        public override ReactElement Render()
		{
			var className = props.ClassName;
			if (!props.Messages.Any())
				className += (className == "" ? "" : " ") + "zero-messages";

            var messagesInv = props.Messages.Reverse();

            var messageElements = messagesInv 
                .Select(savedMessage => DOM.Div(
                    new Attributes { 
                        Key = savedMessage.Id.ToString(), 
                        ClassName = "historical-message" },
					DOM.Span(new Attributes { ClassName = "title" }, 
                        savedMessage.Message.Title), 
					DOM.Span(new Attributes { ClassName = "content" }, 
                        savedMessage.Message.Content) 
				));

			return DOM.FieldSet(
                new FieldSetAttributes { ClassName = className },
				DOM.Legend(null, props.ClassName),
				DOM.Div(messageElements)
			    );
		}
	}
}
