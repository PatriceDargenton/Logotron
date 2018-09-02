
using System;
using System.Collections.Generic;
using System.Diagnostics; // Pour Debugger.Break

namespace LogotronLib
{
    public sealed class clsMotExistant
    {

        public const int iNbColonnes = 10;
        public const int iColMot = 0;
        public const int iColDef = 1;
        public const int iColPrefixe = 2;
        public const int iColSuffixe = 3;
        public const int iColNivPrefixe = 4;
        public const int iColNivSuffixe = 5;
        public const int iColUnicitePrefixe = 6;
        public const int iColUniciteSuffixe = 7;
        public const int iColFreqPrefixe = 8;
        public const int iColFreqSuffixe = 9;

        public string sMot, sDef, sPrefixe, sSuffixe, sDefPrefixe, sDefSuffixe, 
            sNivPrefixe, sNivSuffixe;
        public string sUnicitePrefixe, sUniciteSuffixe, 
            sUnicitePrefixeSynth, sUniciteSuffixeSynth;
        public int iNivPrefixe, iNivSuffixe;
        public int iNumMotExistant; 
        public string sFreqPrefixe, sFreqSuffixe; 

        public clsMotExistant() { }

        public clsMotExistant(string sMot0, string sDef0, string sPrefixe0,
            string sSuffixe0, string sDefPrefixe0, string sDefSuffixe0,
            string sNivPrefixe0, string sNivSuffixe0,
            string sUnicitePrefixe0, string sUniciteSuffixe0, 
            int iNumMot0,
            string sFreqPrefixe0, string sFreqSuffixe0)
        {
            this.sMot = sMot0;
            this.sDef = sDef0;
            this.sPrefixe = sPrefixe0;
            this.sSuffixe = sSuffixe0;
            this.sDefPrefixe = sDefPrefixe0;
            this.sDefSuffixe = sDefSuffixe0;
            this.sNivPrefixe = sNivPrefixe0;
            this.sNivSuffixe = sNivSuffixe0;
            this.iNivPrefixe = int.Parse(this.sNivPrefixe);
            this.iNivSuffixe = int.Parse(this.sNivSuffixe);
            this.sUnicitePrefixe = sUnicitePrefixe0;
            this.sUniciteSuffixe = sUniciteSuffixe0;
            this.iNumMotExistant = iNumMot0;
            this.sFreqPrefixe = sFreqPrefixe0;
            this.sFreqSuffixe = sFreqSuffixe0;

            Synthese();
        }

        public void Synthese()
        {
            string sSensPrefixeSansArticle = clsBase.sSupprimerArticle(sDefPrefixe);
            this.sUnicitePrefixeSynth = sSensPrefixeSansArticle;
            if (sUnicitePrefixe.Length > 0)
                this.sUnicitePrefixeSynth = sUnicitePrefixe;
            string sSensSuffixeSansArticle = clsBase.sSupprimerArticle(sDefSuffixe);
            this.sUniciteSuffixeSynth = sSensSuffixeSansArticle;
            if (sUniciteSuffixe.Length > 0)
                this.sUniciteSuffixeSynth = sUniciteSuffixe;
            
            if (clsConst.bDebug && string.IsNullOrEmpty(sUnicitePrefixeSynth))
                Debugger.Break();
            if (clsConst.bDebug && string.IsNullOrEmpty(sUniciteSuffixeSynth))
                Debugger.Break();
        }

        public static void ParserDefinition(string sDef, ref string sDefSuffixe, ref string sDefPrefixe)
        {
            string[] asChamps2 = sDef.Split(new string[] {
                clsConst.sSepDef}, StringSplitOptions.None);
            int iNbChamps2 = (asChamps2.GetUpperBound(0) + 1);
            sDefSuffixe = null;
            sDefPrefixe = null;
            if (iNbChamps2 >= 1) sDefSuffixe = asChamps2[0].Trim();
            if (iNbChamps2 >= 2) sDefPrefixe = asChamps2[1].Trim();
        }
        public void ParserDefinition()
        {
            string[] asChamps2 = this.sDef.Split(new string[] {
                clsConst.sSepDef}, StringSplitOptions.None);
            int iNbChamps2 = (asChamps2.GetUpperBound(0) + 1);
            this.sDefSuffixe = null;
            this.sDefPrefixe = null;
            if (iNbChamps2 >= 1) this.sDefSuffixe = asChamps2[0].Trim();
            if (iNbChamps2 >= 2) this.sDefPrefixe = asChamps2[1].Trim();
        }

        public override string ToString()
        {
            string sDef = this.sDefSuffixe.ToUpper() + clsConst.sSepDef + 
                clsBase.sCompleterPrefixe(this.sDefPrefixe.ToUpper());
            string sTxt = this.sMot + " : " + sDef + " : " + this.sPrefixe + 
                "(" + this.sNivPrefixe + ")-" + this.sSuffixe + 
                "(" + this.sNivSuffixe + ")";
            if (this.sUnicitePrefixe.Length > 0)
                sTxt += " (unicité préfixe : " + this.sUnicitePrefixe + ")";
            if (this.sUniciteSuffixe.Length > 0)
                sTxt += " (unicité suffixe : " + this.sUniciteSuffixe + ")";
            return sTxt;
        }

        public static void InitMots(List<string> lstMots,
            Dictionary<string, clsMotExistant> dicoMotsExistants)
        {
            int iNbItems = lstMots.Count;
            int iNbMots = iNbItems / clsMotExistant.iNbColonnes;
            for (int i = 0; i <= iNbMots - 1; i++)
            {
                clsMotExistant mot = null;
                if (!bLireMot(lstMots, i, ref mot))
                {
                    if (clsConst.bDebug) Debugger.Break();
                    continue;
                }
                if (dicoMotsExistants.ContainsKey(mot.sMot))
                {
                    if (clsConst.bDebug) Debugger.Break();
                    continue;
                }
                dicoMotsExistants.Add(mot.sMot, mot);
            }
        }

        private static bool bLireMot(List<string> lstMots, int iNumMot,
            ref clsMotExistant mot)
        {
            mot = new clsMotExistant();
            mot.iNumMotExistant = iNumMot; 
            int iNumSegment = (iNumMot * clsMotExistant.iNbColonnes);
            mot.sMot = lstMots[iNumSegment + clsMotExistant.iColMot];
            mot.sDef = lstMots[iNumSegment + clsMotExistant.iColDef];
            mot.sPrefixe = lstMots[iNumSegment + clsMotExistant.iColPrefixe];
            mot.sSuffixe = lstMots[iNumSegment + clsMotExistant.iColSuffixe];
            mot.sNivPrefixe = lstMots[iNumSegment + clsMotExistant.iColNivPrefixe];
            mot.iNivPrefixe = int.Parse(mot.sNivPrefixe);
            mot.sNivSuffixe = lstMots[iNumSegment + clsMotExistant.iColNivSuffixe];
            mot.iNivSuffixe = int.Parse(mot.sNivSuffixe);
            mot.sUnicitePrefixe = lstMots[iNumSegment + clsMotExistant.iColUnicitePrefixe];
            mot.sUniciteSuffixe = lstMots[iNumSegment + clsMotExistant.iColUniciteSuffixe];
            mot.sFreqPrefixe = lstMots[iNumSegment + clsMotExistant.iColFreqPrefixe];
            mot.sFreqSuffixe = lstMots[iNumSegment + clsMotExistant.iColFreqSuffixe];

            mot.ParserDefinition();
            mot.Synthese();
            return true;
        }
    }
}
