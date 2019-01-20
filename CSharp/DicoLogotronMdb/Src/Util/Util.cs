
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Data.Entity.Validation;
using System.Windows.Forms; // DialogResult
using System.Diagnostics; // Process
using JetEntityFrameworkProvider; // JetConnection
using System.Data.Entity; // DbContext

namespace DicoLogotronMdb
{
    static class Util
    {
        public static int CountRows(JetConnection jetConnection, string sqlStatement)
        {
            DbCommand command = jetConnection.CreateCommand(sqlStatement);
            DbDataReader dataReader = command.ExecuteReader();
            int count = 0;
            while (dataReader.Read()) count++;
            return count;
        }

        public static void ShowDataReaderContent(DbConnection dbConnection, string sqlStatement)
        {
            DbCommand command = dbConnection.CreateCommand();
            command.CommandText = sqlStatement;
            DbDataReader dataReader = command.ExecuteReader();

            bool first = true;

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                if (first)
                    first = false;
                else
                    Console.Write("\t");

                Console.Write(dataReader.GetName(i));
            }
            Console.WriteLine();

            while (dataReader.Read())
            {
                first = true;
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    if (first)
                        first = false;
                    else
                        Console.Write("\t");

                    Console.Write("{0}", dataReader.GetValue(i));
                }
                Console.WriteLine();
            }
        }

        public static void ShowDataTableContent(DataTable dataTable)
        {
            bool first = true;

            foreach (DataColumn column in dataTable.Columns)
            {
                if (first)
                    first = false;
                else
                    Console.Write("\t");

                Console.Write(column.ColumnName);
            }
            Console.WriteLine();

            foreach (DataRow row in dataTable.Rows)
            {
                first = true;
                foreach (DataColumn column in dataTable.Columns)
                {
                    if (first)
                        first = false;
                    else
                        Console.Write("\t");
                    Console.Write("{0}", row[column]);
                }
                Console.WriteLine();
            }
        }

        public static string sLireExceptionInterneDbEVE(DbEntityValidationException ex)
        {
            string sMsgErr = "";
            try
            {
                sMsgErr = ex.Message;
                foreach (DbEntityValidationResult val in
                    ex.EntityValidationErrors)
                {
                    foreach (DbValidationError valR in
                        val.ValidationErrors)
                    {
                        sMsgErr += clsConstMdb.sCrLf + valR.ErrorMessage;
                    }
                }
            }
            catch (Exception) { }
            return sMsgErr;
        }

        public static string sLireExceptionInterne(Exception ex)
        {
            string sMsgErr = "";
            try
            {
                sMsgErr = ex.Message;
                if (!(ex.InnerException == null))
                {
                    if (ex.InnerException.Message == ex.Message &&
                        !(ex.InnerException.InnerException == null))
                        sMsgErr += clsConstMdb.sCrLf + ex.InnerException.InnerException.Message;
                    else
                        sMsgErr += clsConstMdb.sCrLf + ex.InnerException.Message;
                }
            }
            catch (Exception) { }
            return sMsgErr;
        }

        public static void LetOpenFile(string sFilePath, string sInfo = "")
        {
            // ToDo : bPrompt si err ici : fct générique
            if (!System.IO.File.Exists(sFilePath)) return;

            //string sMsg = "File created successfully : " +
            string sMsg = "Le fichier a été créé avec succès : " +
                System.IO.Path.GetFileName(sFilePath) + clsConstMdb.sCrLf + sFilePath;
            if (sInfo.Length > 0) sMsg += clsConstMdb.sCrLf + sInfo;

            //sMsg += clsConst.sCrLf + "Would you like to open it ?";
            sMsg += clsConstMdb.sCrLf + "Voulez-vous l'afficher ?";

            DialogResult dialogResult = MessageBox.Show(sMsg, clsConstMdb.sNomAppli,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Cancel) return;

            startAssociateApp(sFilePath);
        }

        public static void startAssociateApp(string sFilePath, bool bMaximized = false,
            bool bWait = false, string sArguments = "")
        {
            // Don't check file if it is a URL to browse
            //if (bCheckFile) { if (!FileExistsPrompt(sFilePath)) return; }
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(sFilePath);
            p.StartInfo.Arguments = sArguments;
            if (bMaximized)
                p.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            p.Start();
            if (bWait) p.WaitForExit();
        }

        public static string hashSetToList(HashSet<string> hs, int iTailleMax = 0) 
        {
            string s="";
            var tab = hs.ToArray();
            Array.Sort(tab);
            foreach (string s1 in tab)
            {
                if (s.Length == 0)
                    s = s1;
                else
                    s += ", " + s1;
            }
            string s2 = s;
            if (iTailleMax > 0 && s.Length > iTailleMax) s2 = s.Substring(0, iTailleMax);
            return s2;
        }

        //public static string listToList(List<string> lst)
        //{
        //    string s = "";
        //    foreach (string s1 in lst)
        //    {
        //        if (s.Length == 0)
        //            s = s1;
        //        else
        //            s += ", " + s1;
        //    }
        //    return s;
        //}

        public static void ActivationLogSQLDansVisualStudio(Context context, bool bActiver)
        {
            //  Penser à changer les formats de date si on veut tester les sql sous phpMyAdmin :
            //  '01/10/2016 00:00:00' -> '2016-10-01' ou '2016-10-01 00-00-00'
            //   et remplacer complètement les paramètres par leur valeur dans la rq : -- p__linq__0: xxx
            //  Par exemple, ici @p__linq__0 est un paramètre, et la ligne suivante est un commentaire
            //   mais le paramètre n'est pas fixé selon le commentaire (c'est juste une info.) :
            //  Dim test = From s In clsDB.context.tableX Where s.id_region = iIdRegion Select s.id
            //  SELECT `Extent1`.`id` FROM `tableX` AS `Extent1` WHERE `Extent1`.`id_region` = @p__linq__0
            //  -- p__linq__0: '1' (Type = Int32, IsNullable = false)
            if ((bActiver && clsConstMdb.bDebug))
            {
                context.Database.Log = Console.Write;
            }
            else
            {
                context.Database.Log = null;
            }
        }

        // Attribut Description
        // https://stackoverflow.com/questions/38996810/in-ef-code-first-is-there-a-data-annotation-for-description-field
        [AttributeUsage(AttributeTargets.Property)]
        public class DescriptionAttribute : Attribute
        {
            string value;

            public DescriptionAttribute(string id)
            {
                this.value = id;
            }

            public string Value
            {
                get { return this.value; }
            }
        }

        public class ColumnsDescription
        {
            public void AddColumnsDescriptions(DbContext mydbContext, bool bBaseVide)
            {
                // Fetch all the DbContext class public properties which contains your attributes
                var dbContextProperies = typeof(DbContext).GetProperties().Select(pi => pi.Name).ToList();

                // Loop each DbSets of type T
                foreach (var item in typeof(Context).GetProperties()
                    .Where(p => dbContextProperies.IndexOf(p.Name) < 0)
                    .Select(p => p))
                {
                    if (!item.PropertyType.GetGenericArguments().Any())
                    {
                        continue;
                    }

                    // Fetch the type of "T"
                    var entityModelType = item.PropertyType.GetGenericArguments()[0];
                    var descriptionInfos = 
                        from prop in entityModelType.GetProperties()
                        where prop.GetCustomAttributes(typeof(DescriptionAttribute), true).Any()
                        select new { 
                            ColumnName = prop.Name, 
                            Attributes = prop.CustomAttributes };

                    foreach (var descriptionInfo in descriptionInfos)
                    {
                        // Sql to create the description column and adding 
                        // Cette requête ne fonctionne pas pour MS-Access :
                        //var addDiscriptionColumnSql =
                        //    @"sp_addextendedproperty @name = N'MS_Description', @value = '"
                        //    + descriptionInfo.Attributes.First().ConstructorArguments.First()
                        //    + @"', @level0type = N'Schema', @level0name = dbo,  @level1type = N'Table',  @level1name = "
                        //    + entityModelType.Name + "s" + ", @level2type = N'Column', @level2name ="
                        //    + descriptionInfo.ColumnName;
                        //var sqlCommandResult = mydbContext.Database.ExecuteSqlCommand(addDiscriptionColumnSql);
                        //var arg = descriptionInfo.Attributes.First().ConstructorArguments.First();
                        string sDescr = "";
                        string sNomColonneModifie = "";
                        foreach  (var attr in descriptionInfo.Attributes)
                        {
                            //Debug.WriteLine(attr.AttributeType.Name + "=" + attr.ToString());
                            if (attr.AttributeType.Name =="DescriptionAttribute")
                            {
                                var descr = attr.ConstructorArguments.First();
                                sDescr = descr.Value.ToString();
                                //break;
                            }
                            if (attr.AttributeType.Name == "ColumnAttribute")
                            {
                                var descr = attr.ConstructorArguments.First();
                                sNomColonneModifie = descr.Value.ToString();
                                //break;
                            }
                            
                        }
                        //string sDescr = arg.ToString();
                        //Debug.WriteLine("ColumnName : " + descriptionInfo.ColumnName + " : " + sDescr);

                        string sBase = clsConstMdb.sBaseLogotron;
                        if (bBaseVide) sBase = clsConstMdb.sBaseLogotronVide;
                        //string sBase = clsConstMdb.sBaseLogotronVide;
                        string sChemin0 = Application.StartupPath + "\\" + sBase + clsConstMdb.sLang;
                        string sCheminMdb = sChemin0 + clsConstMdb.sExtMdb;
                        string sCol = descriptionInfo.ColumnName;
                        if (sNomColonneModifie.Length > 0) sCol = sNomColonneModifie;
                        string sTable = item.Name;
                        if (sTable.EndsWith("s")) 
                            sTable = sTable.Substring(0, sTable.Length - 1);
                        UtilDebuggerStepThrough.DefinirDescrTableOuColonne(sCheminMdb,
                            sTable, sCol, sDescr);

                    }
                }
            }
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class TableDescription : Attribute
        {
            string value;

            public TableDescription(string id)
            {
                this.value = id;
            }

            public string Value
            {
                get { return this.value; }
            }
        }

        public class TablesDescription
        {
            public void AddTablesDescriptions(DbContext mydbContext, bool bBaseVide)
            {
                // Fetch all the DbContext class public properties which contains your attributes
                var dbContextProperies = typeof(DbContext).GetProperties().Select(pi => pi.Name).ToList();

                // Loop each DbSets of type T
                foreach (var item in typeof(Context).GetProperties()
                    .Where(p => dbContextProperies.IndexOf(p.Name) < 0)
                    .Select(p => p))
                {
                    string sTable = item.Name;
                    if (sTable.EndsWith("s"))
                        sTable = sTable.Substring(0, sTable.Length - 1);
                    string sDescr = "";
                    Type typeTable = Type.GetType("DicoLogotronMdb." + sTable);
                    sDescr = typeTable.GetAttributeValue(
                        (TableDescription descr) => descr.Value);
                    //Debug.WriteLine("Table : " + sTable + " : " + sDescr);

                    string sBase = clsConstMdb.sBaseLogotron;
                    if (bBaseVide) sBase = clsConstMdb.sBaseLogotronVide;
                    //string sBase = clsConstMdb.sBaseLogotronVide;
                    string sChemin0 = Application.StartupPath + "\\" + sBase + clsConstMdb.sLang;
                    string sCheminMdb = sChemin0 + clsConstMdb.sExtMdb;
                    
                    UtilDebuggerStepThrough.DefinirDescrTableOuColonne(sCheminMdb,
                        sTable, "", sDescr);

                }
            }
        }
    }

    // https://stackoverflow.com/questions/2656189/how-do-i-read-an-attribute-on-a-class-at-runtime
    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            if (att != null) return valueSelector(att);
            return default(TValue);
        }
    }
}
