
using ProductiveRage.Immutable;

namespace Bridge.React.Logotron.API
{
	//public sealed class MessageDetails : IAmImmutable
	//{
	//	public MessageDetails(NonBlankTrimmedString title, NonBlankTrimmedString content)
	//	{
	//		this.CtorSet(_ => _.Title, title);
	//		this.CtorSet(_ => _.Content, content);
	//	}
	//	public NonBlankTrimmedString Title { get; private set; }
	//	public NonBlankTrimmedString Content { get; private set; }
	//}
    
    public sealed class MessageDetails : IAmImmutable
    {
        public MessageDetails(string title, string content)
        {
            this.CtorSet(_ => _.Title, title);
            this.CtorSet(_ => _.Content, content);
        }
        public string Title { get; }
        public string Content { get; }
    }
}
