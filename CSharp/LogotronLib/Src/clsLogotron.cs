
//using Newtonsoft.Json;
using Logotron.Src.Util;
using LogotronLib.Src;
//using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms; // Pour Application.StartupPath
//using UtilFichier = Util.FileHelpers;
using Util; // clsMsgDelegue

namespace LogotronLib
{
    public static class clsLogotron
    {
        public static bool bTirage(bool bComplet, string sNbPrefixesSuccessifs,
            List<string> lstNiv, List<string> lstFreq, 
            ref string sMot, ref string sExplication,
            ref string sDetail, ref List<string> lstEtymFin, 
            bool bGrecoLatin, bool bNeoRigolo, clsMsgDelegue msgDelegue)
        {
            List<string> lstEtym = new List<string>();
            int iNbTiragesPrefixes = 0;
            if (sNbPrefixesSuccessifs == clsConst.sHasard) 
            {
                iNbTiragesPrefixes = clsUtil.iRandomiser(1, 5);
                float rProba = 1f;
                switch (iNbTiragesPrefixes)
                {
                    case 1:
                        rProba = 1f; // Toujours accepté
                        break;
                    case 2:
                        rProba = 1/2f; // Une fois sur 2
                        //rProba = 0.1f; // Une fois sur 10
                        break;
                    case 3:
                        rProba = 1/4f; // Une fois sur 4
                        //rProba = 1/3f; // Une fois sur 3
                        //rProba = 0.05f;
                        break;
                    case 4:
                        rProba = 1/8f; // Une fois sur 8
                        //rProba = 1/4f; // Une fois sur 4
                        //rProba = 0.03f;
                        break;
                    case 5:
                        rProba = 1/16f; // Une fois sur 16
                        //rProba = 1/5f; // Une fois sur 5
                        //rProba = 0.01f;
                        break;
                }
                if (rProba < 1f)
                {
                    float rTirage = clsUtil.rRandomiser();
                    if (rTirage > rProba) iNbTiragesPrefixes = 1;
                }
            }
            else
                iNbTiragesPrefixes = int.Parse(sNbPrefixesSuccessifs);

            string sPrefixesMaj = "";
            string sSensPrefixesMaj = "";
            string sDetailPrefixesMaj = "";
            clsInitTirage itPref = new clsInitTirage();
            clsGestBase.m_prefixes.MsgDelegue = msgDelegue;
            clsGestBase.m_suffixes.MsgDelegue = msgDelegue;
            for (int i = 0; i <= iNbTiragesPrefixes - 1; i++)
            {
                //if (LogotronLib.clsConst.bDebug)
                //    Console.WriteLine("Tirage préfixe n°" + (i + 1));
                int iNumPrefixe = clsGestBase.m_prefixes.iTirageSegment(bComplet,
                    lstNiv, lstFreq, itPref, bGrecoLatin, bNeoRigolo);
                clsSegmentBase prefixe = null;
                if (!clsGestBase.m_prefixes.bLireSegment(iNumPrefixe, ref prefixe)) return false;
                string sNiveauP = prefixe.sNiveau;
                string sPrefixe = prefixe.sSegment;
                string sPrefixeMaj = sPrefixe.ToUpper();
                string sSensPrefixeMaj2 = prefixe.sSens.ToUpper();
                sSensPrefixeMaj2 = clsBase.sCompleterPrefixe(sSensPrefixeMaj2);
                sPrefixesMaj += sPrefixeMaj;
                sSensPrefixesMaj = sSensPrefixesMaj + " " + sSensPrefixeMaj2;
                sDetailPrefixesMaj = sDetailPrefixesMaj + sPrefixeMaj + "(" + sNiveauP + ") - ";
                string sEtymPrefixe = prefixe.sEtym;
                if (sEtymPrefixe.Length > 0)
                    lstEtym.Add(sPrefixe + "- : " + sEtymPrefixe);
            }
            //if (LogotronLib.clsConst.bDebug) Console.WriteLine("Tirage suffixe");
            int iNumSuffixe = clsGestBase.m_suffixes.iTirageSegment(bComplet,
                lstNiv, lstFreq, new clsInitTirage(), bGrecoLatin, bNeoRigolo);
            clsSegmentBase suffixe = null;
            if (!clsGestBase.m_suffixes.bLireSegment(iNumSuffixe, ref suffixe)) return false;
            string sNiveauS = suffixe.sNiveau;
            string sSuffixe = suffixe.sSegment;
            string sSuffixeMaj = sSuffixe.ToUpper();
            string sDetailSuffixeMaj = sSuffixeMaj + "(" + sNiveauS + ")";
            string sSensSuffixeMaj = suffixe.sSens.ToUpper();
            sMot = sPrefixesMaj + sSuffixeMaj;
            sExplication = sSensSuffixeMaj + sSensPrefixesMaj;
            sDetail = sDetailPrefixesMaj + sDetailSuffixeMaj;
            string sEtymSuffixe = suffixe.sEtym;
            if (sEtymSuffixe.Length > 0) 
                lstEtym.Add("-" + sSuffixe + " : " + sEtymSuffixe);
            lstEtymFin = lstEtym;

            //msgDelegue.AfficherMsg("Test MsgBox");

            return true;
        }

        //public static void LireLogotronJSon()
        //{
            //string sCheminJson = Application.StartupPath + "\\Logotron.json";
            //string[] aStr = UtilFichier.asReadFile(sCheminJson, Encoding.UTF8);
            //var sb = new StringBuilder();
            //foreach (string sLigne in aStr) sb.AppendLine(sLigne);
            //string json = sb.ToString();

            //clsLogotronJson lignes = null;
            //try
            //{
            //    lignes = JsonConvert.DeserializeObject<clsLogotronJson>(json);
            //}
            //catch (Exception ex2)
            //{
            //    Util.clsMessageUtil.ShowErrorMsg(ex2, "LireLogotronJSon");
            //}

            ////DicoTri<string, clsSensSegment> dicoRacines = new DicoTri<string, clsSensSegment>();
            ////DicoTri<string, clsSensSegment> dicoSegments = new DicoTri<string, clsSensSegment>();
            //clsLogotronSegmentJson[] segments = lignes.segments;
            //foreach (clsLogotronSegmentJson seg in segments)
            //{
            //    //clsLogotronSegmentJson seg = segments[j];
            //    string sSegmentTiret2 = "";
            //    bool bPrefixe = false;
            //    if (seg.type == "préfixe")
            //    {
            //        bPrefixe = true;
            //        sSegmentTiret2 = seg.segment + "-";
            //    }
            //    else
            //    {
            //        sSegmentTiret2 = "-" + seg.segment;
            //    }
            //    string sSelect = "D";
            //    //int iSelect = 1;
            //    if (seg.logotron)
            //    {
            //        //iSelect = 2;
            //        sSelect = "L";
            //    }
            //    if (string.IsNullOrEmpty(seg.unicité))
            //    {
            //        seg.unicité = "";
            //    }
            //    if (string.IsNullOrEmpty(seg.étymologie))
            //    {
            //        seg.étymologie = "";
            //    }
            //    clsSegmentBase suffixe = new clsSegmentBase();
            //    clsSegmentBase prefixe = new clsSegmentBase();
            //    if (bPrefixe)
            //    {
            //        prefixe.sSegment = seg.segment;
            //        prefixe.sLogotron = sSelect;
            //        prefixe.sNiveau = seg.niveau.ToString();
            //        prefixe.sSens = seg.sens;
            //        prefixe.sEtym = seg.étymologie;
            //        prefixe.sUnicite = seg.unicité;
            //        clsGestBase.m_prefixes.AjouterSegment(prefixe);
            //    }
            //    else
            //    {
            //        suffixe.sSegment = seg.segment;
            //        suffixe.sLogotron = sSelect;
            //        suffixe.sNiveau = seg.niveau.ToString();
            //        suffixe.sSens = seg.sens;
            //        suffixe.sEtym = seg.étymologie;
            //        suffixe.sUnicite = seg.unicité;
            //        clsGestBase.m_suffixes.AjouterSegment(suffixe);
            //    }
            //    //if (0 == 0)
            //    //{
            //    //    modLogotron.DecompteSegment(seg.segment, sSegmentTiret2, bPrefixe, iSelect, seg.sens, seg.unicité, dicoSegments, seg.niveau);
            //    //    modLogotron.DecompteRacine(seg.segment, sSegmentTiret2, bPrefixe, iSelect, seg.sens, seg.unicité, dicoRacines, seg.niveau);
            //    //}
            //}
            ////if (0 == 0)
            ////{
            ////    modLogotron.CreerListeRacines(dicoRacines);
            ////    modLogotron.CreerListeSegments(dicoSegments);
            ////}
        //}

        public static void LireLogotronCode()
        {
            clsListePrefixes.LirePrefixesCode();
            clsListeSuffixes.LireSuffixesCode();
        }
    }
}
