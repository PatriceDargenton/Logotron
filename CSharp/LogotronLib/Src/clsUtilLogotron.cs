
//using System;
using System.Collections.Generic;
using System.Diagnostics; // Pour Debugger.Break
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace LogotronLib
{
    // ToDo : à déplacer au plus près : cf. version VB
    public sealed class clsUtilLogotron
    {
        //public static void InitMots(List<string> lstMots,
        //    Dictionary<string, clsMotExistant> dicoMotsExistants)
        //{
        //    int iNbItems = lstMots.Count;
        //    int iNbMots = iNbItems / clsMotExistant.iNbColonnes;
        //    for (int i = 0; i <= iNbMots - 1; i++)
        //    {
        //        clsMotExistant mot = null;
        //        if (!bLireMot(lstMots, i, ref mot))
        //        {
        //            if (clsConst.bDebug) Debugger.Break();
        //            continue;
        //        }
        //        if (dicoMotsExistants.ContainsKey(mot.sMot))
        //        {
        //            if (clsConst.bDebug) Debugger.Break();
        //            continue;
        //        }
        //        dicoMotsExistants.Add(mot.sMot, mot);
        //    }
        //}

        //private static bool bLireMot(List<string> lstMots, int iNumMot,
        //    ref clsMotExistant mot)
        //{
        //    mot = new clsMotExistant();
        //    mot.iNumMotExistant = iNumMot; // 30/06/2018
        //    int iNumSegment = (iNumMot * clsMotExistant.iNbColonnes);
        //    mot.sMot = lstMots[iNumSegment + clsMotExistant.iColMot];
        //    mot.sDef = lstMots[iNumSegment + clsMotExistant.iColDef];
        //    mot.sPrefixe = lstMots[iNumSegment + clsMotExistant.iColPrefixe];
        //    mot.sSuffixe = lstMots[iNumSegment + clsMotExistant.iColSuffixe];
        //    mot.sNivPrefixe = lstMots[iNumSegment + clsMotExistant.iColNivPrefixe];
        //    mot.iNivPrefixe = int.Parse(mot.sNivPrefixe);
        //    mot.sNivSuffixe = lstMots[iNumSegment + clsMotExistant.iColNivSuffixe];
        //    mot.iNivSuffixe = int.Parse(mot.sNivSuffixe);
        //    mot.sUnicitePrefixe = lstMots[iNumSegment + clsMotExistant.iColUnicitePrefixe];
        //    mot.sUniciteSuffixe = lstMots[iNumSegment + clsMotExistant.iColUniciteSuffixe];
        //    mot.sFreqPrefixe = lstMots[iNumSegment + clsMotExistant.iColFreqPrefixe];
        //    mot.sFreqSuffixe = lstMots[iNumSegment + clsMotExistant.iColFreqSuffixe];

        //    mot.ParserDefinition();
        //    mot.Synthese();
        //    return true;
        //}
    }
}
