
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics; // Pour Debugger.Break();

using Util;
using Logotron.Src.Util; // clsMsgDelegue

namespace LogotronLib
{
    public sealed class clsBase
    {

        private int m_iNbColonnes;

        private const int iColSegment = 0;
        private const int iColPrefixe = 0;
        private const int iColSuffixe = 0;
        private const int iColSens = 1;
        private const int iColLogotron = 2;
        private const int iColNiveau = 3;
        private const int iColEtym = 4;
        private const int iColUnicite = 5;
        private const int iColOrigine = 6; 
        private const int iColFrequence = 7; 

        private List<string> m_lstSegments;

        public clsBase(int iNbColonnes)
        {
            this.m_iNbColonnes = 0;
            this.m_lstSegments = new List<string>();
            this.m_iNbColonnes = iNbColonnes;
        }

        public clsMsgDelegue MsgDelegue { get; set; }

        public int iLireNbSegments()
        {
            if (this.m_iNbColonnes == 0)
            {
                if (clsConst.bDebug) Debugger.Break();
                return 0;
            }
            int iNbSegments = this.m_lstSegments.Count / this.m_iNbColonnes;
            return iNbSegments;
        }

        public int iLireNbSegmentsUniques(
            List<string> lstNiv, List<string> lstFreq, 
            bool bGrecoLatin, bool bNeoRigolo)
        {
            //  Retourner la liste de segments uniques selon le sens, avec les niveaux indiqués
            //  (sélection du Logotron)
            if (lstNiv == null) throw new ArgumentNullException("lstNiv");
            if (lstFreq == null) throw new ArgumentNullException("lstFreq");

            HashSet<string> hsSegments = new HashSet<string>();
            int iNbSegments = iLireNbSegments();
            for (int i = 0; i <= iNbSegments - 1; i++)
            {
                clsSegmentBase segment = null;
                if (!bLireSegment(i, ref segment)) continue;

                if (!lstNiv.Contains(segment.sNiveau)) continue;
                if (!lstFreq.Contains(segment.sFrequence)) continue;

                if (segment.sLogotron != clsConst.sSelectLogotron) continue;

                if ((bGrecoLatin || !bNeoRigolo) &&
                    segment.sOrigine == enumOrigine.sNeologismeAmusant)
                {
                    //if (LogotronLib.clsConst.bDebug)
                    //    Console.WriteLine("Segment non retenu : " +
                    //        segment.sSegment + " : " + segment.sOrigine);
                    continue;
                }

                if (bGrecoLatin && 
                     !(segment.sOrigine == enumOrigine.sGrec || 
                       segment.sOrigine == enumOrigine.sLatin ||
                       segment.sOrigine == enumOrigine.sGrecoLatin))
                {
                    //if (LogotronLib.clsConst.bDebug)
                    //    Console.WriteLine("Segment non gréco-latin : " + 
                    //        segment.sSegment + " : " + segment.sOrigine);
                    continue;
                }

                string sSensSansArticle = clsBase.sSupprimerArticle(segment.sSens);
                string sCleUniciteSens = sSensSansArticle;
                if ((segment.sUnicite.Length > 0)) sCleUniciteSens = segment.sUnicite;
                if (!hsSegments.Contains(sCleUniciteSens)) hsSegments.Add(sCleUniciteSens);
            }
            return hsSegments.Count;
        }

        public int iTirageSegment(bool bComplet, 
            List<string> lstNiv, List<string> lstFreq, bool bGrecoLatin, bool bNeoRigolo)
        {
            return this.iTirageSegment(bComplet, lstNiv, lstFreq, 
                new clsInitTirage(), bGrecoLatin, bNeoRigolo);
        }

        public int iTirageSegment(bool bComplet,
            List<string> lstNiv, List<string> lstFreq,
            clsInitTirage it, bool bGrecoLatin, bool bNeoRigolo)
        {

            // bComplet : tous les segments (y compris ceux du dictionnaire),
            //  ou sinon seulement ceux du Logotron
            // lstNiveaux : combinaison des niveaux "1", "2" et/ou "3"
            // lstNumSegmentDejaTires  : ne pas tirer à nouveau un segment déjà tiré
            //  (pour avoir un mot avec plusieurs préfixes distincts)
            // lstSegmentDejaTires     : ne pas tirer à nouveau un segment déjà tiré
            //  (cette fois le segment doit être unique, dans le cas où des segments 
            //   seraient présents avec plusieurs sens)
            // lstSensSegmentDejaTires : ne pas tirer à nouveau un sens déjà tiré
            // lstUnicitesSegmentDejaTires : lié au champ unicité 
            //  (unicité explicite car le sens peut varier plus ou moins)
            // bGrecoLatin : seulement les segments d'origine greco-latine,
            //  sinon les segments de toutes origines
            // bNeoRigolo : inclure les néologismes amusants

            // Il faut vérifier que le tirage est possible : compter qu'il y a 
            //  au moins 1 candidat, sinon boucle infinie dans le tirage

            var enreg = from seg0 in ObtenirSegmentBases()
                where
                    lstNiv.Contains(seg0.sNiveau) &&
                    lstFreq.Contains(seg0.sFrequence) &&
                    ((it.lstNumSegmentDejaTires == null) ||
                     !it.lstNumSegmentDejaTires.Contains(seg0.iNumSegment)) && 
                    ((it.lstSegmentsDejaTires == null) ||
                     !it.lstSegmentsDejaTires.Contains(seg0.sSegment)) && 
                    ((it.lstSensSegmentDejaTires == null) ||
                     !it.lstSensSegmentDejaTires.Contains(seg0.sSens)) && 
                    ((it.lstUnicitesSegmentDejaTires == null) ||
                     !it.lstUnicitesSegmentDejaTires.Contains(seg0.sUnicite)) &&
                    (bComplet || seg0.sLogotron == clsConst.sSelectLogotron) &&
                    ((!bGrecoLatin && 
                      (bNeoRigolo || 
                       seg0.sOrigine != LogotronLib.enumOrigine.sNeologismeAmusant)) ||
                     (bGrecoLatin && 
                      (seg0.sOrigine == LogotronLib.enumOrigine.sGrec || 
                       seg0.sOrigine == LogotronLib.enumOrigine.sLatin ||
                       seg0.sOrigine == LogotronLib.enumOrigine.sGrecoLatin)))
                    select seg0;

            int iNbEnreg = enreg.Count();
            if (iNbEnreg == 0)
            {
                if (clsConst.bDebug) Debugger.Break();
                MsgDelegue.AfficherMsg(
                    "Aucun élément ne correspond à la sélection : Tirage impossible !");
                return clsConst.iTirageImpossible;
            }

            int iNbSegmentsFilres = iNbEnreg;
            // On tire un nombre compris entre 0 et iNbSegmentsFilres - 1 (liste filtrée)
            int iNumSegment2 = clsUtil.iRandomiser(0, iNbSegmentsFilres - 1);
            var seg = enreg.ElementAtOrDefault(iNumSegment2);
            int iNumSegment = seg.iNumSegment; // Retourner l'indice du segment dans la liste complète

            if (it.lstNumSegmentDejaTires != null)
                it.lstNumSegmentDejaTires.Add(iNumSegment);
            if (it.lstSegmentsDejaTires != null)
                it.lstSegmentsDejaTires.Add(seg.sSegment);
            if (it.lstSensSegmentDejaTires != null)
                it.lstSensSegmentDejaTires.Add(seg.sSens);
            if (seg.sUnicite.Length > 0 &&
                it.lstUnicitesSegmentDejaTires != null)
                it.lstUnicitesSegmentDejaTires.Add(seg.sUnicite);
            
            return iNumSegment;
        }

        public string sTrouverEtymologie(string sSegment, string sUniciteSynth)
        {
            var enreg = from seg0 in ObtenirSegmentBases()
                where seg0.sSegment == sSegment && seg0.sUniciteSynth == sUniciteSynth
                select seg0;
            if (enreg.Count() == 0) return "";
            foreach (clsSegmentBase seg in enreg) return seg.sEtym;
            return "";
        }
        
        public List<clsSegmentBase> lstSegments(List<string> lstNiv, bool bNeoRigolo)
        {

            // Lister tous les segments pour le niveau demandé

            var enreg = 
                from seg0 in ObtenirSegmentBases()
                where 
                    lstNiv.Contains(seg0.sNiveau) &&
                    (bNeoRigolo || seg0.sOrigine != enumOrigine.sNeologismeAmusant)
                select seg0;
            var lst = enreg.ToList();
            return lst;
        }

        public List<clsSegmentBase> lstSegmentsAutreOrigine(
            List<string> lstNiv, bool bNeoRigolo)
        {

            // Lister tous les segments avec une autre origine,
            //  pour le niveau demandé

            var enreg = 
                from seg0 in ObtenirSegmentBases()
                where 
                    lstNiv.Contains(seg0.sNiveau) &&
                    (bNeoRigolo ||
                     seg0.sOrigine != enumOrigine.sNeologismeAmusant) &&
                    !(seg0.sOrigine == enumOrigine.sGrec ||
                      seg0.sOrigine == enumOrigine.sLatin ||
                      seg0.sOrigine == enumOrigine.sGrecoLatin)
                select seg0;
            var lst = enreg.ToList();
            return lst;
        }

        public void DefinirSegments(List<string> segments, int iNbColonnes)
        {
            this.m_lstSegments = segments;
            this.m_iNbColonnes = iNbColonnes;
        }

        public List<string> ObtenirSegments()
        {
            return this.m_lstSegments;
        }

        public List<clsSegmentBase> ObtenirSegmentBases()
        {
            List<clsSegmentBase> lst = new List<clsSegmentBase>();
            int iNbSegments = this.iLireNbSegments();
            for (int i = 0; i <= iNbSegments - 1; i++)
            {
                clsSegmentBase segment = null;
                if (this.bLireSegment(i, ref segment)) lst.Add(segment);
            }
            return lst;
        }

        public void AjouterSegment(clsSegmentBase segment)
        {
            if (segment == null) throw new ArgumentNullException("segment");
            this.m_lstSegments.Add(segment.sSegment);
            this.m_lstSegments.Add(segment.sSens);
            this.m_lstSegments.Add(segment.sLogotron);
            this.m_lstSegments.Add(segment.sNiveau);
            this.m_lstSegments.Add(segment.sEtym);
            this.m_lstSegments.Add(segment.sUnicite);
            this.m_lstSegments.Add(segment.sOrigine); 
            this.m_lstSegments.Add(segment.sFrequence); 
        }

        public bool bLireSegment(int iNumSegmentL, ref clsSegmentBase segment)
        {
            segment = null;
            if (this.m_iNbColonnes <= 0)
            {
                if (clsConst.bDebug) Debugger.Break();
                return false;
            }

            if (iNumSegmentL == clsConst.iTirageImpossible) return false;

            segment = new clsSegmentBase();
            segment.iNumSegment = iNumSegmentL;
            int iNumSegment = iNumSegmentL * this.m_iNbColonnes;

            segment.sSegment = this.m_lstSegments[iNumSegment + iColSegment];
            if (clsConst.bDebug && segment.sSegment == null) Debugger.Break();

            if (this.m_iNbColonnes <= iColSens) return true;
            segment.sSens = this.m_lstSegments[iNumSegment + iColSens];
            if (clsConst.bDebug && segment.sSens == null) Debugger.Break();

            if (this.m_iNbColonnes <= iColLogotron) return true;
            segment.sLogotron = this.m_lstSegments[iNumSegment + iColLogotron];
            if (clsConst.bDebug && segment.sLogotron == null) Debugger.Break();

            if (this.m_iNbColonnes <= iColNiveau) return true;
            segment.sNiveau = this.m_lstSegments[iNumSegment + iColNiveau];
            if (clsConst.bDebug && segment.sNiveau == null) Debugger.Break();

            segment.iNiveau = int.Parse(segment.sNiveau);
            if (this.m_iNbColonnes <= iColEtym) return true;
            segment.sEtym = this.m_lstSegments[iNumSegment + iColEtym];
            if (clsConst.bDebug && segment.sEtym == null) Debugger.Break();

            if (this.m_iNbColonnes <= iColUnicite) return true;
            segment.sUnicite = this.m_lstSegments[iNumSegment + iColUnicite];
            if (clsConst.bDebug && segment.sUnicite == null) Debugger.Break();

            if (this.m_iNbColonnes <= iColOrigine) return true;
            segment.sOrigine = this.m_lstSegments[iNumSegment + iColOrigine];
            if (clsConst.bDebug && segment.sOrigine == null) Debugger.Break();

            if (this.m_iNbColonnes <= iColFrequence) return true;
            segment.sFrequence = this.m_lstSegments[iNumSegment + iColFrequence];
            if (clsConst.bDebug && segment.sFrequence == null) Debugger.Break();

            string sSensSansArticle = clsBase.sSupprimerArticle(segment.sSens);
            string sUniciteFinale = sSensSansArticle;
            segment.sUniciteSynth = sUniciteFinale;
            if (segment.sUnicite.Length > 0) segment.sUniciteSynth = segment.sUnicite;

            if (this.m_iNbColonnes <= clsConst.iNbColonnes) return true;
            return true;
        }

        public static string sSupprimerArticle(string sTxt)
        {
            string sTxtCorr5 = sSupprimerArticleInterm("les", sTxt);
            string sTxtCorr4 = sSupprimerArticleInterm("le", sTxtCorr5);
            string sTxtCorr3 = sSupprimerArticleInterm("la", sTxtCorr4);
            return sTxtCorr3.Replace("l'", "");
        }

        private static string sSupprimerArticleInterm(string sArticle, string sTxt)
        {
            string sTxtCorr1 = sTxt.Replace("/ " + sArticle, "/");
            string sTxtCorr2 = sTxtCorr1.Replace("-> " + sArticle, "->");
            string sTxtCorr3 = sTxtCorr2.Replace("," + sArticle, ",");
            // 07/10/2018 En dernier
            if (sTxtCorr3.StartsWith(sArticle + " "))
            {
                int iLongArticle = sArticle.Length;
                string sTxtFin = sTxtCorr3.Substring(
                    iLongArticle + 1, sTxtCorr3.Length - iLongArticle - 1);
                return sTxtFin;
            }
            return sTxtCorr3;
        }

        public static string sCompleterPrefixe(string sSensPrefixeOrig)
        {
            if (string.IsNullOrEmpty(sSensPrefixeOrig)) return "";
            string sSensPrefixe = sSensPrefixeOrig;
            int iLongSensPrefixe = sSensPrefixeOrig.Length;
            if (iLongSensPrefixe == 0) return sSensPrefixe;
            if (sSensPrefixeOrig.Substring(0, 3) == "LE ")
                sSensPrefixe = "DU " + sSensPrefixeOrig.Substring(3);
            else if (iLongSensPrefixe >= 4 && sSensPrefixeOrig.Substring(0, 4) == "LES ")
                sSensPrefixe = "DES " + sSensPrefixeOrig.Substring(4);
            else if ((sSensPrefixeOrig.Substring(0, 3) == "LA " ||
                      sSensPrefixeOrig.Substring(0, 2) == "L'"))
                sSensPrefixe = "DE " + sSensPrefixeOrig;
            string sSensPrefixe2 = sRemplacerCar(sSensPrefixe, "/");
            return sRemplacerCar(sSensPrefixe2, "->");
        }

        private static string sRemplacerCar(string sTxt, string sCar)
        {
            return sTxt
                .Replace(sCar + " LE ", sCar + " DU ")
                .Replace(sCar + " LES ", sCar + " DES ")
                .Replace(sCar + " LA ", sCar + " DE LA ")
                .Replace(sCar + " L'É", sCar + " DE L'É")
                .Replace(sCar + " L'", sCar + " DE L'");
        }
    }
}
