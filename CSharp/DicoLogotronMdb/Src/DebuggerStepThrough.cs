
using System;
using System.Windows.Forms; // DialogResult

using dao;

namespace DicoLogotronMdb
{
    static class UtilDebuggerStepThrough
    {
        // Attribut pour éviter que l'IDE s'interrompt en cas d'exception
        // L'attribut fonctionne avec Visual Studio 2013, mais plus depuis Visual Studio 2015
        // Mais on peut maintenant désactiver une exception au niveau du module/classe
        [System.Diagnostics.DebuggerStepThrough]
        public static void DefinirDescrTableOuColonne(string sCheminMdb, string sTable, 
            string sColonne, string sDescr)
        {
            // Modify the description property of an access table from .NET
            // https://itproblemy.pl/questions/37023427/modify-the-description-property-of-an-access-table-from-net

            const string sPropDescription = "Description";
            int iErrPropertyNotFound = -2146825018;

            try
            {
                // Ne fonctionne pas, car spécifique à MSAccess 2013
                // https://www.nuget.org/packages/Microsoft.Office.Interop.Access.Dao/
                // Du coup on doit conserver la dll dao.dll
                //var ws = new Microsoft.Office.Interop.Access.Dao.DBEngine();
                //Microsoft.Office.Interop.Access.Dao.Database db;
                //Microsoft.Office.Interop.Access.Dao.TableDef tbl;
                //Microsoft.Office.Interop.Access.Dao.Property prop;
                //Microsoft.Office.Interop.Access.Dao.Field fld = null;

                var ws = new dao.DBEngine();
                dao.Database db;
                dao.TableDef tbl;
                dao.Property prop;
                dao.Field fld = null;

                db = ws.OpenDatabase(sCheminMdb);
                tbl = db.TableDefs[sTable];
                bool bChamp = false;
                if (sColonne.Length > 0) {
                    fld = tbl.Fields[sColonne];
                    bChamp = true;
                }

                try // Pas de Properties.Contains dans dao ?
                {
                    if (bChamp)
                        prop = fld.Properties[sPropDescription];
                    else
                        prop = tbl.Properties[sPropDescription];
                    prop.Value = sDescr;
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    //const int idbText = 10; // dao.DataTypeEnum.dbText; 
                    if (ex.ErrorCode == iErrPropertyNotFound)
                        if (bChamp)
                            fld.Properties.Append(
                                fld.CreateProperty(sPropDescription, DataTypeEnum.dbText, sDescr));
                        else
                            tbl.Properties.Append(
                                tbl.CreateProperty(sPropDescription, DataTypeEnum.dbText, sDescr));
                    else
                        throw;
                }

                // En Lecture
                //prop = tbl.Properties["Description"];
                //string sDescr = prop.Value;
                //Debug.WriteLine(sDescr);

                ws = null;

            }
            catch (Exception ex)
            {
                string sMsg = ex.Message;
                //string sMsgErrDet = Util.sLireExceptionInterne(ex);
                //sMsg += sMsgErrDet;
                sMsg += clsConstMdb.sCrLf + "Table : " + sTable;
                if (sColonne.Length > 0) 
                    sMsg += clsConstMdb.sCrLf + "Colonne : " + sColonne ;
                MessageBox.Show(sMsg, clsConstMdb.sNomAppli,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
