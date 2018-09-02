
using ProductiveRage.Immutable;

namespace Bridge.React.Logotron.API
{
    public sealed class ConfigLogotron : IAmImmutable
    {
        public ConfigLogotron(bool[] niveaux, string nbPrefixes)
        {
            this.CtorSet(_ => _.Niveaux, niveaux);
            this.CtorSet(_ => _.NbPrefixes, nbPrefixes);
        }
        public bool[] Niveaux { get; }
        public string NbPrefixes { get; }
    }
}
