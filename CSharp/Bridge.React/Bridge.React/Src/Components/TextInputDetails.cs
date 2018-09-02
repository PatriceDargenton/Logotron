
using ProductiveRage.Immutable;

namespace Bridge.React.Logotron
{
	public sealed class TextInputDetails : IAmImmutable
	{
		public TextInputDetails(
            int id,
            string content
            )
		{
			this.CtorSet(_ => _.Id, id);
			this.CtorSet(_ => _.Content, content);
        }
		public int Id { get; }
        public string Content { get; }
    }
}