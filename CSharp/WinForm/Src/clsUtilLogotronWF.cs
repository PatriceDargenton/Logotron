
using System.Collections.Generic;
using System.Text;

using UtilFichier = UtilWinForm.FileHelpers;
using LogotronLib;
using System.Diagnostics; // Pour Debugger.Break

namespace Logotron.Src
{
    class clsUtilLogotronWF // WinForm
    {
        public static void ChargerMotsExistantsCsv(
            Dictionary<string, clsMotExistant> m_dicoMotsExistants, string sChemin) 
        {
            //m_dicoMotsExistants = new Dictionary<string, clsMotExistant>();
            //string sChemin = (Application.StartupPath + ("\\MotsSimples" + (sLang + ".csv")));
            if (!UtilFichier.FileExistsPrompt(sChemin)) return;

            // Le code page 1252 correspond à FileOpen de VB .NET, l'équivalent en VB6 de
            //  Open sCheminFichier For Input As #1
            // Mettre & pour Long en DotNet1 et % pour Integer en DotNet2
            const int iCodePageWindowsLatin1252 = 1252; // windows-1252 = msoEncodingWestern
            var encod = Encoding.GetEncoding(iCodePageWindowsLatin1252);
            string[] asLignes = UtilFichier.asReadFile(sChemin, encod); //Encoding.UTF8);
            int iNumLigne = 0;
            int iNumMot = 0;
            foreach (string sLigne in asLignes)
            {
                iNumLigne++;
                if (iNumLigne < 2) continue ;
            
                //  1 ligne d'entête
                string[] asChamps = sLigne.Split(';');
                int iNbChamps = asChamps.GetUpperBound(0) + 1;
                string sMot = "";
                string sDef = "";
                //string sDecoup = "";
                string sPrefixe = "";
                string sSuffixe = "";
                string sDefPrefixe = "";
                string sDefSuffixe = "";
                string sNivPrefixe = "";
                string sNivSuffixe = "";
                string sUnicitePrefixe = "";
                string sUniciteSuffixe = "";
                string sFreqPrefixe = "";
                string sFreqSuffixe = "";
                bool bElisionPrefixe = false;

                if (iNbChamps >= 1) sMot = asChamps[0].Trim();

                //if (sMot == "") Debugger.Break();

                if (iNbChamps >= 2)
                {
                    sDef = asChamps[1].Trim();
                    clsMotExistant.ParserDefinition(sDef, 
                        ref sDefSuffixe, ref sDefPrefixe);
                    //string[] asChamps2 = sDef.Split(
                    //    new string[1] { "  " }, StringSplitOptions.None);
                    //int iNbChamps4 = asChamps2.GetUpperBound(0) + 1;
                    //if (iNbChamps4 >= 1) sDefSuffixe = asChamps2[0].Trim();
                    //if (iNbChamps4 >= 2) sDefPrefixe = asChamps2[1].Trim();
                }

                //if (iNbChamps >= 2)
                //{
                //    sDef = asChamps[1].Trim();
                //    string[] asChamps2 = sDef.Split(
                //        new string[1] { "  " }, StringSplitOptions.None);
                //    int iNbChamps2 = (asChamps2.GetUpperBound(0) + 1);
                //    if (iNbChamps2 >= 1) sDefSuffixe = asChamps2[0].Trim();
                //    if (iNbChamps2 >= 2) sDefPrefixe = asChamps2[1].Trim();
                //}

                if (iNbChamps >= 3) sPrefixe = asChamps[2].Trim();
                if (iNbChamps >= 4) sSuffixe = asChamps[3].Trim();
                if (iNbChamps >= 5) sNivPrefixe = asChamps[4].Trim();
                if (iNbChamps >= 6) sNivSuffixe = asChamps[5].Trim();
                if (iNbChamps >= 7) sUnicitePrefixe = asChamps[6].Trim();
                if (iNbChamps >= 8) sUniciteSuffixe = asChamps[7].Trim();
                if (iNbChamps >= 9) sFreqPrefixe = asChamps[8].Trim();
                if (iNbChamps >= 10) sFreqSuffixe = asChamps[9].Trim();
                if (sNivPrefixe == "" || sNivSuffixe == "" ||
                    sFreqPrefixe == "" || sFreqSuffixe == "") {
                    if (clsConst.bDebug) Debugger.Break();
                    continue;
                }

                // 01/05/2019
                if (clsConst.bElision && sPrefixe.EndsWith(clsConst.sCarElisionO)) {
                    bElisionPrefixe = true;
                    sPrefixe = sPrefixe.Replace(clsConst.sCarElisionO, clsConst.sCarO);
                }

                if (!m_dicoMotsExistants.ContainsKey(sMot)) {
                    m_dicoMotsExistants.Add(sMot, new clsMotExistant(
                        sMot, sDef, sPrefixe, sSuffixe, sDefPrefixe, sDefSuffixe, 
                        sNivPrefixe, sNivSuffixe, sUnicitePrefixe, sUniciteSuffixe, 
                        iNumMot++, sFreqPrefixe, sFreqSuffixe, bElisionPrefixe)); // 30/06/2018
                }
            }
        }
    }
}
