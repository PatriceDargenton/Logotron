
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics; // Pour Debugger.Break();

using Util;
using Logotron.Src.Util; // clsMsgDelegue

namespace LogotronLib
{
    public static class clsGestBase
    {
        public const int iNbColonnes = 6;
        public const int iTirageImpossible = -1;

        public static clsMsgDelegue m_msgDelegue;

        public static clsBase m_prefixes;
        public static clsBase m_suffixes;

        private static Dictionary<string, clsMotExistant> m_dicoMotsExistants;
        public static void ChargerMotsExistants(
            Dictionary<string, clsMotExistant> dicoMotsExistants)
        {
            m_dicoMotsExistants = dicoMotsExistants;
        }

        public static int iNbMotsExistants(List<string> lstNiv, List<string> lstFreq)
        {
            var enreg = 
                from mot0 in m_dicoMotsExistants.ToList()
                where
                    lstNiv.Contains(mot0.Value.sNivPrefixe) &&
                    lstNiv.Contains(mot0.Value.sNivSuffixe) &&
                    lstFreq.Contains(mot0.Value.sFreqPrefixe) &&
                    lstFreq.Contains(mot0.Value.sFreqSuffixe)
                group mot0 by new {
                    mot0.Value.sUnicitePrefixeSynth,
                    mot0.Value.sUniciteSuffixeSynth
                } into newGroup select newGroup;

             return enreg.Count();
        }
        
        public static List<clsMotExistant> lstMotsExistants(
            List<string> lstNiv, List<string> lstFreq) 
        {
            var enreg = 
                from mot0 in m_dicoMotsExistants.ToList()
                where
                    lstNiv.Contains(mot0.Value.sNivPrefixe) &&
                    lstNiv.Contains(mot0.Value.sNivSuffixe) &&
                    lstFreq.Contains(mot0.Value.sFreqPrefixe) &&
                    lstFreq.Contains(mot0.Value.sFreqSuffixe)
                select mot0;
            List<clsMotExistant> lst = new List<clsMotExistant>();
            HashSet<string> hs = new HashSet<string>();
            foreach (var mot in enreg) 
            {
                string sCle = 
                    mot.Value.sUnicitePrefixeSynth + ":" + 
                    mot.Value.sUniciteSuffixeSynth;
                if (hs.Contains(sCle)) continue ;
                hs.Add(sCle);
                lst.Add(mot.Value);
            }
            return lst;
        }
        
        public static int iNbPrefixesMotsExistants(
            List<string> lstNiv, List<string> lstFreq)
        {
            // Compter tous les préfixes distincts des mots 
            //  du ou des niveaux demandé(s)
            //  de la ou des fréquence(s) demandée(s)
            var enreg = 
                (from mot0 in m_dicoMotsExistants.ToList()
                 where
                     lstNiv.Contains(mot0.Value.sNivPrefixe) &&
                     lstNiv.Contains(mot0.Value.sNivSuffixe) &&
                     lstFreq.Contains(mot0.Value.sFreqPrefixe) && 
                     lstFreq.Contains(mot0.Value.sFreqSuffixe)
                 select mot0.Value.sUnicitePrefixeSynth).Distinct();
            
            int iNbEnreg = enreg.Count();
            return iNbEnreg;
        }
        
        public static int iNbSuffixesMotsExistants(
            List<string> lstNiv, List<string> lstFreq)
        {
            // Compter tous les suffixes distincts des mots 
            //  du ou des niveaux demandé(s)
            //  de la ou des fréquence(s) demandée(s)
            var enreg = 
                (from mot0 in m_dicoMotsExistants.ToList()
                 where
                     lstNiv.Contains(mot0.Value.sNivPrefixe) &&
                     lstNiv.Contains(mot0.Value.sNivSuffixe) &&
                     lstFreq.Contains(mot0.Value.sFreqPrefixe) &&
                     lstFreq.Contains(mot0.Value.sFreqSuffixe)
                 select mot0.Value.sUniciteSuffixeSynth).Distinct();

            int iNbEnreg = enreg.Count();
            return iNbEnreg;
        }

        public static int iTirageMotExistant(
            List<string> lstNiv, List<string> lstFreq,
            ref clsSegmentBase prefixe, ref clsSegmentBase suffixe)
        {
            // Tirer au hasard un mot du niveau demandé

            // 01/05/2019 Test élision : mot0.Value.bElisionPrefixe &&

            var enreg = from mot0 in m_dicoMotsExistants.ToList()
                where
                    lstNiv.Contains(mot0.Value.sNivPrefixe) &&
                    lstNiv.Contains(mot0.Value.sNivSuffixe) &&
                    lstFreq.Contains(mot0.Value.sFreqPrefixe) &&
                    lstFreq.Contains(mot0.Value.sFreqSuffixe)
                select mot0; 

            int iNbEnreg = enreg.Count();
            if (iNbEnreg == 0)
            {
                if (clsConst.bDebug) Debugger.Break();
                m_msgDelegue.AfficherMsg(
                    "Aucun mot ne correspond à la sélection : Tirage impossible !");
                return clsConst.iTirageImpossible;
            }

            int iNbMotsExistantsFiltres = iNbEnreg;
            // On tire un nombre compris entre 0 et iNbSegmentsFilres - 1 (liste filtrée)
            int iNumMotExistantTire = clsUtil.iRandomiser(0, iNbMotsExistantsFiltres - 1);
            var mot = enreg.ElementAtOrDefault(iNumMotExistantTire).Value;
            
            if (mot == null)
            {
                if (clsConst.bDebug) Debugger.Break();
                m_msgDelegue.AfficherMsg(
                    "Aucun mot ne correspond à la sélection : Tirage impossible !");
                return clsConst.iTirageImpossible;
            }

            // Indice du mot dans la liste complète
            int iNumMotExistant = mot.iNumMotExistant;
            if (iNumMotExistant == 0)
            {
                if (clsConst.bDebug) Debugger.Break();
                m_msgDelegue.AfficherMsg(
                    "Aucun mot ne correspond à la sélection : Tirage impossible !");
                return clsConst.iTirageImpossible;
            }

            prefixe = new clsSegmentBase();
            suffixe = new clsSegmentBase();

            prefixe.sSegment = mot.sPrefixe;
            prefixe.sLogotron = clsConst.sSelectDictionnaire;
            prefixe.sNiveau = mot.sNivPrefixe;
            prefixe.sSens = mot.sDefPrefixe;
            prefixe.sEtym = "";
            prefixe.sUnicite = mot.sUnicitePrefixe;
            prefixe.sUniciteSynth = mot.sUnicitePrefixeSynth;
            prefixe.sFrequence = mot.sFreqPrefixe;
            
            // 01/05/2019
            prefixe.bElision = mot.bElisionPrefixe;
            prefixe.sSegmentElision = prefixe.sSegment;
            if (prefixe.bElision)
                prefixe.sSegmentElision = 
                    prefixe.sSegment.Substring(0, prefixe.sSegment.Length - 1);

            suffixe.sSegment = mot.sSuffixe;
            suffixe.sLogotron = clsConst.sSelectDictionnaire;
            suffixe.sNiveau = mot.sNivSuffixe;
            suffixe.sSens = mot.sDefSuffixe;
            suffixe.sEtym = "";
            suffixe.sUnicite = mot.sUniciteSuffixe;
            suffixe.sUniciteSynth = mot.sUniciteSuffixeSynth;
            suffixe.sFrequence = mot.sFreqSuffixe;

            return iNumMotExistant;
        }

        public static int iTirageMotExistantAutre(
            List<string> lstNiv, List<string> lstFreq, int iNumMotExistant,
            clsInitTirage itPrefixe, clsInitTirage itSuffixe,
            List<int> lstNumMotExistant, ref clsMotExistant motAutre)
        {
            // Tirer au hasard un autre mot du niveau demandé
            var enreg = from mot0 in m_dicoMotsExistants.ToList()
                where
                    lstNiv.Contains(mot0.Value.sNivPrefixe) &&
                    lstNiv.Contains(mot0.Value.sNivSuffixe) &&
                    lstFreq.Contains(mot0.Value.sFreqPrefixe) &&
                    lstFreq.Contains(mot0.Value.sFreqSuffixe) &&
                    !lstNumMotExistant.Contains(iNumMotExistant) &&
                    !itPrefixe.lstSegmentsDejaTires.Contains(mot0.Value.sPrefixe) &&
                    !itSuffixe.lstSegmentsDejaTires.Contains(mot0.Value.sSuffixe) &&
                    !itPrefixe.lstSensSegmentDejaTires.Contains(mot0.Value.sDefPrefixe) &&
                    !itSuffixe.lstSensSegmentDejaTires.Contains(mot0.Value.sDefSuffixe) &&
                    !itPrefixe.lstUnicitesSegmentDejaTires.Contains(mot0.Value.sUnicitePrefixeSynth) &&
                    !itSuffixe.lstUnicitesSegmentDejaTires.Contains(mot0.Value.sUniciteSuffixeSynth)
                select mot0;

            int iNbEnreg = enreg.Count();
            if (iNbEnreg == 0)
            {
                //if (clsConst.bDebug) Debugger.Break();
                m_msgDelegue.AfficherMsg(
                    "Aucun mot ne correspond à la sélection : Tirage impossible !");
                return clsConst.iTirageImpossible;
            }

            int iNbMotsExistantsFiltres = iNbEnreg;
            // On tire un nombre compris entre 0 et iNbSegmentsFilres - 1 (liste filtrée)
            int iNumMotExistantTire = clsUtil.iRandomiser(0, iNbMotsExistantsFiltres - 1);
            motAutre = enreg.ElementAtOrDefault(iNumMotExistantTire).Value;
            // Indice du mot dans la liste complète
            int iNumMotExistantAutre = motAutre.iNumMotExistant;

            lstNumMotExistant.Add(iNumMotExistantAutre);
            itPrefixe.lstSegmentsDejaTires.Add(motAutre.sPrefixe);
            itSuffixe.lstSegmentsDejaTires.Add(motAutre.sSuffixe);
            itPrefixe.lstSensSegmentDejaTires.Add(motAutre.sDefPrefixe);
            itSuffixe.lstSensSegmentDejaTires.Add(motAutre.sDefSuffixe);
            itPrefixe.lstUnicitesSegmentDejaTires.Add(motAutre.sUnicitePrefixeSynth);
            itSuffixe.lstUnicitesSegmentDejaTires.Add(motAutre.sUniciteSuffixeSynth);

            if (clsConst.bDebug && string.IsNullOrEmpty(motAutre.sUnicitePrefixeSynth))
                Debugger.Break();
            if (clsConst.bDebug && string.IsNullOrEmpty(motAutre.sUniciteSuffixeSynth))
                Debugger.Break();

            return iNumMotExistant;
        }

        public static void InitBases()
        {
            m_prefixes = new clsBase(clsConst.iNbColonnes, bPrefixe: true);
            m_suffixes = new clsBase(clsConst.iNbColonnes, bPrefixe: false);
        }
    }
}
