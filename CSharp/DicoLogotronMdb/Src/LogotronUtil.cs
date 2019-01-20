
using System.Data.Common;
using System.Data.OleDb;
using JetEntityFrameworkProvider; // JetConnection

namespace DicoLogotronMdb
{
    static class HelpersL
    {
        public static DbConnection GetConnection(bool bBaseVide)
        {
            // Take care because according to this article
            // http://msdn.microsoft.com/en-us/library/dd0w4a2z(v=vs.110).aspx
            // to make the following line work the provider must be installed in the GAC and we also need an entry in machine.config
            /*
            DbProviderFactory providerFactory = System.Data.Common.DbProviderFactories.GetFactory("JetEntityFrameworkProvider");
            DbConnection connection = providerFactory.CreateConnection();
            */
            DbConnection connection = new JetConnection();
            connection.ConnectionString = GetConnectionString(bBaseVide);
            return connection;
        }

        public static string GetConnectionString(bool bBaseVide)
        {
            OleDbConnectionStringBuilder oleDbConnectionStringBuilder = new OleDbConnectionStringBuilder();
            oleDbConnectionStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            if (bBaseVide)
                oleDbConnectionStringBuilder.DataSource = @".\" +
                    clsConstMdb.sBaseLogotronVide + 
                    clsConstMdb.sLang + clsConstMdb.sExtMdb; 
            else
                oleDbConnectionStringBuilder.DataSource = @".\" +
                    clsConstMdb.sBaseLogotron + 
                    clsConstMdb.sLang + clsConstMdb.sExtMdb; 
            return oleDbConnectionStringBuilder.ToString();
        }
    }
}
