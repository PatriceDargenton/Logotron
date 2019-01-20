
using System.Data.Entity;
using System.Data.Common;

namespace DicoLogotronMdb
{
    public class Context : System.Data.Entity.DbContext
    {
        public Context() { }

        public Context(DbConnection connection) : base(connection, false) { }

        public DbSet<Prefixe> Prefixes { get; set; }
        public DbSet<Suffixe> Suffixes { get; set; }
        public DbSet<Segment> Segments { get; set; }
        public DbSet<Concept> Concepts { get; set; }
        public DbSet<Racine> Racines { get; set; }
        public DbSet<Mot> Mots { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Ce n'est pas une relation de 1 à 1, sinon voici comment il faudrait faire :
            // Configuration de Segment et Préfixe
            //modelBuilder
            //    .Entity<Segment>()
            //    // Rendre la propriété Préfixe comme optionnelle dans l'entité Segment
            //    .HasOptional(s => s.Prefixe)
            //    // Rendre la propriété Segment comme requise dans l'entité Préfixe
            //    // On ne peut pas sauver Préfixe sans son Segment
            //    .WithRequired(s => s.Segment); 
        }
    }
}
