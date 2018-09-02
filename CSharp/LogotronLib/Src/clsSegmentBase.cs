
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogotronLib
{
    public sealed class clsSegmentBase
    {
        public string sSegment, sSens, sLogotron, sNiveau, sEtym, sUnicite, sUniciteSynth;
        public string sOrigine; // 16/06/2018 Origine étymologique : Latin, Grec, ...
        public string sFrequence; // 21/08/2018 Fréquence du segment dans la liste des mots existants (seulement les complets)
        public int iNiveau, iNumSegment;

        public clsSegmentBase()
        {
        }

        public string sAfficher(bool bPrefixe)
        {
            string sTxt = "";
            if (bPrefixe)
                sTxt = this.sSegment + "-";
            else
                sTxt = "-" + this.sSegment;
            string sTxtComplement = sTxt + "(" + this.sNiveau + ") : " + 
                this.sSens + 
                ", origine : " + this.sOrigine + 
                ", fréquence : " + this.sFrequence;
            return sTxtComplement;
        }
    }
}
