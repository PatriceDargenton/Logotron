
using System;
using System.Windows.Forms;

using System.Data.Common;
using LogotronLib;
using LogotronLib.Src; // clsListeMotsExistants.ChargerMotsExistantsCode
using System.Collections.Generic;
using System.Diagnostics; // Process
using System.Linq;
using System.Data.Entity.Validation;
using System.Data.Entity;

using UtilFichier = UtilWinForm.FileHelpers;
using System.Text;
using UtilWinForm;

namespace DicoLogotronMdb
{
    public partial class frmDicoLogotronMdb : Form
    {
        // Sur une base existante, forcer la màj de certains champs, en cas de m.àj. de l'appli.
        bool m_bForcerMaj = true; 

        const bool bAjouterConcepts = true;
        //const bool bAjouterConcepts = false;
        const bool bAjouterRacines = true;
        //const bool bAjouterRacines = false;
        const bool bAjouterSegments = true;
        //const bool bAjouterSegments = false;
        const bool bAjouterPrefixes = true;
        //const bool bAjouterPrefixes = false;
        const bool bAjouterSuffixes = true;
        //const bool bAjouterSuffixes = false;
        const bool bAjouterMots = true;
        //const bool bAjouterMots = false;
        const bool bAjouterMotsJSon = true;//false;
        const bool bBilanCoherence = true;
        //const bool bBilanCoherence = false; // Le json peut être <>
        const bool bBilanJSon = true;
        //const bool bBilanJSon = false;
        
        /*
        const int iNbPrefixesMax = 200; //1000;
        const int iNbSuffixesMax = 200; //500;
        const int iNbMotsExistantsMax = 200; // 10000 Seulement en mode Release sous Visual Studio 2017
        */
        const int iNbPrefixesMax = 1000; //1000;
        const int iNbSuffixesMax = 500; //500;
        const int iNbMotsExistantsMax = 20000; // 10000 Seulement en mode Release sous Visual Studio 2017
        string m_sDernierTypeElemAjoute = "";
        string m_sDernierElemAjoute = "";
        bool m_bAnnuler = false;
        const string sTermO = "o"; // Terminaison en o
        const string sParenth = "(";

#region "Initialisation"

        public frmDicoLogotronMdb()
        {
            InitializeComponent();
            if (clsConstMdb.bDebug) this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void frmDicoLogotronMdb_Load(object sender, EventArgs e)
        {
            string sTxt = Text + " - V" + clsConstMdb.sVersionAppli + " (" + clsConstMdb.sDateAppli + ")";
            if (clsConstMdb.bDebug) sTxt += " - Debug";
            this.Text = sTxt;
            if (clsConstMdb.bDebug) this.chkBaseVide.Checked = true;
        }

        private void AfficherMsgBarreMsg(string sTxt)
        {
            this.toolStripStatusLabelBarreMsg.Text = sTxt;
            Application.DoEvents(); // Nécessaire
        }
        
        private void cmdAnnuler_Click(object sender, EventArgs e)
        {
            m_bAnnuler = true;
        }

        private void cmdMdb_Click(object sender, EventArgs e)
        {
            string sBase = clsConstMdb.sBaseLogotron;
            if (this.chkBaseVide.Checked) sBase = clsConstMdb.sBaseLogotronVide;
            string sChemin = Application.StartupPath + "\\" + sBase + clsConstMdb.sLang;
            string sCheminMdb = sChemin + clsConstMdb.sExtMdb;
            string sCheminLdb = sChemin + clsConstMdb.sExtLdb;
            // Le fichier .ldb peut persister tant qu'on ne quitte pas l'appli.
            while (System.IO.File.Exists(sCheminLdb))
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Veuillez fermer le fichier :" +
                    clsConst.sCrLf + sCheminMdb, clsConstMdb.sNomAppli,
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (dialogResult == DialogResult.Cancel) return;
            }

            cmdMdb.Enabled = false;
            m_bAnnuler = false;
            cmdAnnuler.Enabled = true;

            clsGestBase.InitBases();
            if (clsConstMdb.sLang == enumLangue.En)
                clsLogotron.LireLogotronCodeEn();
            else
                clsLogotron.LireLogotronCode();

            try
            {
                CreerBase(this.chkBaseVide.Checked);
            }
            //catch (System.Data.Entity.Core.EntityException ex)
            //{
            //    MessageBox.Show(ex.Message, sNomAppli,
            //        MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            catch (DbEntityValidationException ex)
            {
                //MessageBox.Show(ex.Message, sNomAppli,
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                string sMsg = ex.Message;
                string sMsgErrDet = Util.sLireExceptionInterneDbEVE(ex);
                sMsg += sMsgErrDet;
                if (!string.IsNullOrEmpty(m_sDernierTypeElemAjoute))
                    sMsg += clsConstMdb.sCrLf + "Type : " + m_sDernierTypeElemAjoute;
                if (!string.IsNullOrEmpty(m_sDernierElemAjoute))
                    sMsg += clsConstMdb.sCrLf + "Clé : " + m_sDernierElemAjoute;
                MessageBox.Show(sMsg, clsConstMdb.sNomAppli,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                string sMsg = ex.Message;
                string sMsgErrDet = Util.sLireExceptionInterne(ex);
                sMsg += sMsgErrDet;
                if (!string.IsNullOrEmpty(m_sDernierTypeElemAjoute))
                    sMsg += clsConstMdb.sCrLf + "Type : " + m_sDernierTypeElemAjoute;
                if (!string.IsNullOrEmpty(m_sDernierElemAjoute))
                    sMsg += clsConstMdb.sCrLf + "Clé : " + m_sDernierElemAjoute;
                MessageBox.Show(sMsg, clsConstMdb.sNomAppli,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!m_bAnnuler) AfficherMsgBarreMsg("Terminé.");
            Util.LetOpenFile(sCheminMdb);

            cmdMdb.Enabled = true;
            cmdAnnuler.Enabled = false;
        }

#endregion

        public void ChargerMotsComplexesTxt(Context context, string sChemin,
            Dictionary<string, Prefixe> dicoPrefixes,
            Dictionary<string, Suffixe> dicoSuffixes,
            Dictionary<string, Mot> dicoMots, bool bBaseVide, StringBuilder sbBilan)
        {
            if (!UtilFichier.FileExistsPrompt(sChemin)) return;

            // Le code page 1252 correspond à FileOpen de VB .NET, l'équivalent en VB6 de
            //  Open sCheminFichier For Input As #1
            // Mettre & pour Long en DotNet1 et % pour Integer en DotNet2
            const int iCodePageWindowsLatin1252 = 1252; // windows-1252 = msoEncodingWestern
            var encod = Encoding.GetEncoding(iCodePageWindowsLatin1252);
            string[] asLignes = UtilFichier.asReadFile(sChemin, encod); //Encoding.UTF8);
            int iNumLigne = 0;
            var hsBilan = new HashSet<String>(); // Un seul avert. par segment
            sbBilan.AppendLine("");
            sbBilan.AppendLine("");
            sbBilan.AppendLine("S'il y a des préfixes ou suffixes non trouvés, cela signifie seulement que les listes ne sont pas à jour dans le code (clsListePrefixe.cs et clsListeSuffixe.cs)");
            sbBilan.AppendLine("");
            sbBilan.AppendLine("Analyse des mots complexes");
            sbBilan.AppendLine("--------------------------");
            foreach (string sLigne in asLignes)
            {
                iNumLigne++;
                if (iNumLigne < 3) continue; // 2 lignes d'entête

                string[] asChamps = sLigne.Split('|');
                int iNbChamps = asChamps.GetUpperBound(0) + 1;
                if (iNbChamps != 4) continue;
                string sMot = asChamps[0].Trim();
                string sDef = asChamps[1].Trim();
                string sSegments = asChamps[2].Trim();
                string sUnicite = asChamps[3].Trim();

                //if (sMot == "chromoptomètre")
                //    Debug.WriteLine("!");

                string sSuffixe = "", sPrefixe1 ="", sPrefixe2="", sPrefixe3 = "", sPrefixe4 = "";
                if (sSegments.Length > 0)
                {
                    //string[] asChamps3 = sSegments.Split('-');
                    string[] asChamps3 = sSegments.Split(
                        new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                    int iNbChamps3 = asChamps3.GetUpperBound(0) + 1;
                    //if (iNbChamps3 != 3) continue;
                    if (iNbChamps3 > 5) continue;
                    //sPrefixe1 = asChamps3[0].Trim();
                    //sPrefixe2 = asChamps3[1].Trim();
                    //sSuffixe = asChamps3[2].Trim();
                    // 05/01/2019
                    sSuffixe = asChamps3[0].Trim();
                    sPrefixe1 = asChamps3[1].Trim();
                    sPrefixe2 = asChamps3[2].Trim();
                    if (iNbChamps3 >= 4) sPrefixe3 = asChamps3[3].Trim();
                    if (iNbChamps3 >= 5) sPrefixe4 = asChamps3[4].Trim();

                    if (clsConst.bElision) { // 05/05/2019
                        if (sPrefixe1.EndsWith(clsConst.sCarElisionO))
                            sPrefixe1 = sPrefixe1.Replace(clsConst.sCarElisionO, clsConst.sCarO);
                        if (sPrefixe2.EndsWith(clsConst.sCarElisionO))
                            sPrefixe2 = sPrefixe2.Replace(clsConst.sCarElisionO, clsConst.sCarO);
                        if (sPrefixe3.EndsWith(clsConst.sCarElisionO))
                            sPrefixe3 = sPrefixe3.Replace(clsConst.sCarElisionO, clsConst.sCarO);
                        if (sPrefixe4.EndsWith(clsConst.sCarElisionO))
                            sPrefixe4 = sPrefixe4.Replace(clsConst.sCarElisionO, clsConst.sCarO);
                    }

                }

                if (sUnicite.Length > 0)
                {
                    string[] asChamps2 = sUnicite.Split('-');
                    //string[] asChamps2 = sUnicite.Split(
                    //    new[] { " -" }, StringSplitOptions.RemoveEmptyEntries);
                    int iNbChamps2 = asChamps2.GetUpperBound(0) + 1;
                    //if (iNbChamps2 != 2) continue;
                    if (iNbChamps2 > 2) continue;
                    string sUnicitesPrefixes="", sUnicitesSuffixe="";
                    //if (iNbChamps2 > 0) sUnicitesPrefixes = asChamps2[0].Trim();
                    //if (iNbChamps2 > 1) sUnicitesSuffixe = asChamps2[1].Trim();
                    // 05/01/2019
                    if (iNbChamps2 > 0) sUnicitesSuffixe= asChamps2[0].Trim();
                    if (iNbChamps2 > 1) sUnicitesPrefixes = asChamps2[1].Trim();

                    if (sUnicitesPrefixes.Length > 0) // && sUnicitesSuffixe.Length > 0)
                    {
                        string[] asChamps3 = sUnicitesPrefixes.Split('+');
                        int iNbChamps3 = asChamps3.GetUpperBound(0) + 1;
                        //if (iNbChamps3 != 2) continue;
                        if (iNbChamps3 < 2) continue;
                        if (iNbChamps3 > 4) continue;
                        string sUnicitePrefixe1 = asChamps3[0].Trim();
                        string sUnicitePrefixe2 = asChamps3[1].Trim();
                        string sUnicitePrefixe3 = "";
                        if (iNbChamps3 > 2) sUnicitePrefixe3 = asChamps3[2].Trim();
                        string sUnicitePrefixe4 = "";
                        if (iNbChamps3 > 3) sUnicitePrefixe4 = asChamps3[3].Trim();
                        //Debug.WriteLine(sMot + " : " + sDef + " : " + sSegments + " : " + 
                        //    sUnicitePrefixe1 + ", " + sUnicitePrefixe2 + " : " + sUnicitesSuffixe);

                        string sClePrefixe1 = sPrefixe1 + "-";
                        if (sUnicitePrefixe1.Length > 0) sClePrefixe1 += ":" + sUnicitePrefixe1;
                        if (!dicoPrefixes.ContainsKey(sClePrefixe1))
                        {
                            if (!hsBilan.Contains(sClePrefixe1)) {
                                hsBilan.Add(sClePrefixe1);
                                sbBilan.AppendLine("Préfixe n°1 non trouvé : " + 
                                    sClePrefixe1 + " : " + sLigne);
                            }
                            //Debug.WriteLine(sMot + " : Préfixe n°1 non trouvé : " + sClePrefixe1);
                            //AfficherMsgBarreMsg("Préfixe n°1 non trouvé : " + sClePrefixe1);
                            continue;
                        }
                        var prefixe1 = dicoPrefixes[sClePrefixe1];
                        string sClePrefixe2 = sPrefixe2 + "-";
                        if (sUnicitePrefixe2.Length > 0) sClePrefixe2 += ":" + sUnicitePrefixe2;
                        if (!dicoPrefixes.ContainsKey(sClePrefixe2))
                        {
                            if (!hsBilan.Contains(sClePrefixe2)) {
                                hsBilan.Add(sClePrefixe2);
                                sbBilan.AppendLine("Préfixe n°2 non trouvé : " +
                                    sClePrefixe2 + " : " + sLigne);
                            }
                            //Debug.WriteLine(sMot + " : Préfixe n°2 non trouvé : " + sClePrefixe2);
                            //AfficherMsgBarreMsg("Préfixe n°2 non trouvé : " + sClePrefixe2);
                            continue;
                        }
                        var prefixe2= dicoPrefixes[sClePrefixe2];

                        Prefixe prefixe3 = null;
                        if (sPrefixe3.Length > 0)
                        {
                            string sClePrefixe3 = sPrefixe3 + "-";
                            if (sUnicitePrefixe3.Length > 0) sClePrefixe3 += ":" + sUnicitePrefixe3;
                            if (!dicoPrefixes.ContainsKey(sClePrefixe3))
                            {
                                if (!hsBilan.Contains(sClePrefixe3)) {
                                    hsBilan.Add(sClePrefixe3);
                                    sbBilan.AppendLine("Préfixe n°3 non trouvé : " + 
                                        sClePrefixe3 + " : " + sLigne);
                                }
                                //Debug.WriteLine(sMot + " : Préfixe n°3 non trouvé : " + sClePrefixe3);
                                //AfficherMsgBarreMsg("Préfixe n°3 non trouvé : " + sClePrefixe3);
                                continue;
                            }
                            prefixe3 = dicoPrefixes[sClePrefixe3];
                        }

                        Prefixe prefixe4 = null;
                        if (sPrefixe4.Length > 0)
                        {
                            string sClePrefixe4 = sPrefixe4 + "-";
                            if (sUnicitePrefixe4.Length > 0) sClePrefixe4 += ":" + sUnicitePrefixe4;
                            if (!dicoPrefixes.ContainsKey(sClePrefixe4))
                            {
                                if (!hsBilan.Contains(sClePrefixe4)) {
                                    hsBilan.Add(sClePrefixe4);
                                    sbBilan.AppendLine("Préfixe n°4 non trouvé : " + 
                                        sClePrefixe4 + " : " + sLigne);
                                }
                                //Debug.WriteLine(sMot + " : Préfixe n°4 non trouvé : " + sClePrefixe4);
                                //AfficherMsgBarreMsg("Préfixe n°4 non trouvé : " + sClePrefixe4);
                                continue;
                            }
                            prefixe4 = dicoPrefixes[sClePrefixe4];
                        }

                        string sCleSuffixe = "-"+sSuffixe;
                        if (sUnicitesSuffixe.Length > 0) sCleSuffixe += ":" + sUnicitesSuffixe;
                        if (!dicoSuffixes.ContainsKey(sCleSuffixe))
                        {
                            if (!hsBilan.Contains(sCleSuffixe)) {
                                hsBilan.Add(sCleSuffixe);
                                sbBilan.AppendLine("Suffixe     non trouvé : " + 
                                    sCleSuffixe + " : " + sLigne);
                            }
                            //Debug.WriteLine(sMot + " : Suffixe non trouvé : " + sCleSuffixe);
                            //AfficherMsgBarreMsg("Suffixe non trouvé : " + sCleSuffixe);
                            continue;
                        }
                        var suffixe = dicoSuffixes[sCleSuffixe];

                        //Debug.WriteLine(sMot + " : " + sDef + " : " + sSegments + " : " +
                        //    sUnicitePrefixe1 + ", " + sUnicitePrefixe2 + " : " +
                        //    sUnicitesSuffixe + " : " +
                        //    prefixe1.Prefixe_ + " : " + prefixe2.Prefixe_ + " : " +
                        //    suffixe.Suffixe_);

                        Mot mot = null;
                        string sCleMot = sMot;
                        bool bMotExiste = dicoMots.ContainsKey(sCleMot);

                        if (!bMotExiste)
                            mot = new Mot() { Mot_ = sCleMot };
                        else
                            mot = dicoMots[sCleMot];
                        mot.Prefixe = prefixe1;
                        mot.Prefixe2 = prefixe2;
                        mot.Prefixe3 = prefixe3;
                        mot.Prefixe4 = prefixe4;
                        mot.Suffixe = suffixe;

                        if (bMotExiste && m_bForcerMaj)
                        {
                            var entry = context.Entry(mot);
                            // A moins que l'entité n'ait pas encore été sauvée, elle doit être modifiée
                            if (entry.State != EntityState.Added)
                                entry.State = EntityState.Modified;
                        }
                        else if (!bMotExiste)
                        {
                            context.Mots.Add(mot);
                            m_sDernierTypeElemAjoute = "Mot";
                            m_sDernierElemAjoute = mot.Mot_;
                            if (clsConstMdb.bDebugDB) context.SaveChanges();
                            dicoMots.Add(mot.Mot_, mot);
                        }

                    }
                }
            }

            m_sDernierTypeElemAjoute = "Général";
            m_sDernierElemAjoute = "";
            context.SaveChanges();
        }

        public static void ChargerDicoSensConceptCsv(
            Dictionary<string, string> dicoSensConcept, string sChemin)
        {
            if (!UtilFichier.FileExistsPrompt(sChemin)) return;

            // Le code page 1252 correspond à FileOpen de VB .NET, l'équivalent en VB6 de
            //  Open sCheminFichier For Input As #1
            // Mettre & pour Long en DotNet1 et % pour Integer en DotNet2
            const int iCodePageWindowsLatin1252 = 1252; // windows-1252 = msoEncodingWestern
            var encod = Encoding.GetEncoding(iCodePageWindowsLatin1252);
            string[] asLignes = UtilFichier.asReadFile(sChemin, encod); //Encoding.UTF8);
            int iNumLigne = 0;
            foreach (string sLigne in asLignes)
            {
                iNumLigne++;
                if (iNumLigne < 2) continue; // 1 ligne d'entête
                
                string[] asChamps = sLigne.Split(';');
                int iNbChamps = asChamps.GetUpperBound(0) + 1;
                if (iNbChamps != 2) continue;
                string sCle = asChamps[0].Trim();
                string sSensNorm = asChamps[1].Trim();

                if (!dicoSensConcept.ContainsKey(sCle)) dicoSensConcept.Add(sCle, sSensNorm); 
            }
        }
        
        public static void ChargerLogotronCsv(string sChemin,
            HashSet<string> hsSegmentsExclus,
            Dictionary<string, Prefixe> dicoPrefixes,
            Dictionary<string, Suffixe> dicoSuffixes,
            int iNumPasse)
        {
            if (!UtilFichier.FileExistsPrompt(sChemin)) return;

            // Chargement juste des Exemples et de ListeExclusiveMots
            // (qui ne sont pas dans clsLogotron.LireLogotronCode)

            // Le code page 1252 correspond à FileOpen de VB .NET, l'équivalent en VB6 de
            //  Open sCheminFichier For Input As #1
            // Mettre & pour Long en DotNet1 et % pour Integer en DotNet2
            const int iCodePageWindowsLatin1252 = 1252; // windows-1252 = msoEncodingWestern
            var encod = Encoding.GetEncoding(iCodePageWindowsLatin1252);
            string[] asLignes = UtilFichier.asReadFile(sChemin, encod); //Encoding.UTF8);
            int iNumLigne = 0;
            foreach (string sLigne in asLignes)
            {
                iNumLigne++;
                if (iNumLigne < 2) continue; // 1 ligne d'entête

                string[] asChamps = sLigne.Split(';');
                int iNbChamps = asChamps.GetUpperBound(0) + 1;
                if (iNbChamps < 14) continue;
                string sLettre = asChamps[0].Trim();
                string sSelect = asChamps[1].Trim();
                string sNiveau = asChamps[2].Trim();
                string sSegment = asChamps[3].Trim();
                string sPrefixe = asChamps[4].Trim();
                string sSuffixe = asChamps[5].Trim();
                string sSens = asChamps[6].Trim();
                string sEtym = asChamps[7].Trim();
                string sUnicite = asChamps[8].Trim();
                string sExplication = asChamps[9].Trim();
                string sExemples = asChamps[10].Trim();
                string sOrigine = asChamps[11].Trim();
                string sFrequence = asChamps[12].Trim();
                string sListeExclMots = asChamps[13].Trim();

                //if (sPrefixe == "duodéno-") Debug.WriteLine("!");

                if (iNumPasse == 1)
                {
                    if (sSelect == "0" && !hsSegmentsExclus.Contains(sSegment))
                        hsSegmentsExclus.Add(sSegment);
                    continue;
                }

                if (sPrefixe.Length > 0)
                {
                    string sClePrefixe = sPrefixe;
                    //if (sClePrefixe.StartsWith ("algo")) Debug.WriteLine("!");
                    if (sUnicite.Length > 0) sClePrefixe += ":" + sUnicite;
                    if (!dicoPrefixes.ContainsKey(sClePrefixe)) continue;
                    var prefixe = dicoPrefixes[sClePrefixe];
                    if (!string.IsNullOrEmpty(sExemples)) prefixe.Exemples = sExemples;
                    if (!string.IsNullOrEmpty(sListeExclMots)) 
                        prefixe.ListeExclusiveMots = sListeExclMots;
                }
                else
                {
                    string sCleSuffixe = sSuffixe;
                    if (sUnicite.Length > 0) sCleSuffixe += ":" + sUnicite;
                    if (!dicoSuffixes.ContainsKey(sCleSuffixe)) continue;
                    var suffixe = dicoSuffixes[sCleSuffixe];
                    if (!string.IsNullOrEmpty(sExemples)) suffixe.Exemples = sExemples;
                    if (!string.IsNullOrEmpty(sListeExclMots)) 
                        suffixe.ListeExclusiveMots = sListeExclMots;
                }
            }
        }

        private void CreerBase(bool bBaseVide)
        {
            if (bBaseVide) m_bForcerMaj = false;
            var lstNiv = new List<string>() { "1", "2", "3" };
            const bool bNeoRigolo = true;

            // This is the only reason why we need to include the provider
            //JetEntityFrameworkProvider.JetConnection.ShowSqlStatements = true;

            string sChemin = Application.StartupPath + "\\SensConcept" + clsConstMdb.sLang + ".csv";
            var dicoSensConcept = new Dictionary<string, string>();
            ChargerDicoSensConceptCsv(dicoSensConcept, sChemin);
            
            var dicoConcepts = new Dictionary<string, Concept>();
            var dicoRacines = new Dictionary<string, Racine>();
            var dicoRacinesUnicite = new Dictionary<string, Racine>();
            var dicoSegments = new Dictionary<string, Segment>();
            var dicoPrefixes = new Dictionary<string, Prefixe>();
            var dicoSuffixes = new Dictionary<string, Suffixe>();
            var dicoMots = new Dictionary<string, Mot>();
            var hsSegmentsExclus = new HashSet<string>();
            var sbBilan = new StringBuilder();

            // Chargement juste des Exemples et de ListeExclusiveMots
            // (qui ne sont pas dans clsLogotron.LireLogotronCode)
            // Passe n°1, certains segments sont exclus (homo de homo sapiens)
            string sCheminLogotron = Application.StartupPath + "\\Logotron" + clsConstMdb.sLang + ".csv";
            ChargerLogotronCsv(sCheminLogotron, hsSegmentsExclus, dicoPrefixes, dicoSuffixes, 
                iNumPasse:1);

            DbConnection connection = HelpersL.GetConnection(bBaseVide);
            Context context = new Context(connection);
            //Helpers.ActivationLogSQLDansVisualStudio(context, bActiver: true);

            AfficherMsgBarreMsg("Lecture de la base de données...");
            if (m_bAnnuler) return;
            RechargerBase(context, dicoConcepts, dicoRacines, dicoSegments, 
                dicoPrefixes, dicoSuffixes, dicoMots);

            var lstPrefixes = clsGestBase.m_prefixes.lstSegments(lstNiv, bNeoRigolo);
            var lstSuffixes = clsGestBase.m_suffixes.lstSegments(lstNiv, bNeoRigolo);

            TraiterPrefixes(context, lstPrefixes,
                dicoConcepts, dicoRacines, dicoSegments, dicoPrefixes,
                dicoSensConcept, hsSegmentsExclus, lstNiv, bNeoRigolo, 
                dicoRacinesUnicite, sbBilan);

            //if (!bAjouterSuffixes) goto Fin;
            TraiterSuffixes(context, lstSuffixes,
                dicoConcepts, dicoRacines, dicoSegments, dicoSuffixes,
                dicoSensConcept, hsSegmentsExclus, lstNiv, bNeoRigolo, 
                dicoRacinesUnicite, sbBilan);
            
            //Fin:
            AfficherMsgBarreMsg("Détermination des racines finales des concepts...");
            if (m_bAnnuler) return;
            DetRacinesFinalesConcept(context);
            
            // Passe n°2 : Récupérer les mots exemples
            ChargerLogotronCsv(sCheminLogotron, hsSegmentsExclus, dicoPrefixes, dicoSuffixes, 
                iNumPasse: 2);

            if (bAjouterMots && iNbMotsExistantsMax >0) 
            { 
                var dicoMotsExistants = new Dictionary<string, clsMotExistant>();
                if (clsConstMdb.sLang == enumLangue.En)
                    clsListeMotsExistants.ChargerMotsExistantsCodeEn(dicoMotsExistants);
                else
                    clsListeMotsExistants.ChargerMotsExistantsCode(dicoMotsExistants);
                CreerMotExistantMdb(context, dicoMotsExistants, 
                    dicoPrefixes, dicoSuffixes, dicoMots);
            }

            if (bBilanCoherence)
            CreerRapportCoherence(context,
                dicoConcepts, dicoRacines, dicoSegments, dicoPrefixes, dicoSuffixes, bBaseVide,
                sbBilan, sbBilan);

            AfficherMsgBarreMsg("Enregistrement des modifications...");
            if (m_bAnnuler) return;
            context.SaveChanges();

            //MotsComplexes:
            AfficherMsgBarreMsg("Ajout des mots complexes...");
            if (m_bAnnuler) return;
            string sCheminMC = Application.StartupPath + "\\MotsComplexesUnicite" + clsConstMdb.sLang + ".txt";
            ChargerMotsComplexesTxt(context, sCheminMC, 
                dicoPrefixes, dicoSuffixes, dicoMots, bBaseVide, sbBilan);

            AfficherMsgBarreMsg("Production du fichier LogotronBdd_fr.json...");
            if (m_bAnnuler) return;
            if (bBilanJSon)
            CreerRapportJson(context, dicoConcepts, dicoRacines, dicoSegments, dicoPrefixes,
                dicoSuffixes, dicoMots, bIdTxt: false);
            if (bBilanJSon)
            CreerRapportJson(context, dicoConcepts, dicoRacines, dicoSegments, dicoPrefixes,
                dicoSuffixes, dicoMots, bIdTxt: true);

            //DescriptionChamps:
            AfficherMsgBarreMsg("Ajout des descriptions...");
            if (m_bAnnuler) return;
            // Attribut Description
            // https://stackoverflow.com/questions/38996810/in-ef-code-first-is-there-a-data-annotation-for-description-field
            var c = new DicoLogotronMdb.Util.ColumnsDescription();
            c.AddColumnsDescriptions(context, bBaseVide);
            var t = new DicoLogotronMdb.Util.TablesDescription();
            t.AddTablesDescriptions(context, bBaseVide);

            string sCheminBilan = Application.StartupPath +
                "\\Bilan" + clsConstMdb.sLang + ".txt";
            FileHelpers.WriteFile(sCheminBilan, sbBilan, encode: Encoding.UTF8);
        }

        private void CreerMotExistantMdb(Context context,
            Dictionary<string, clsMotExistant> dicoMotsExistants,
            Dictionary<string, Prefixe> dicoPrefixes,
            Dictionary<string, Suffixe> dicoSuffixes,
            Dictionary<string, Mot> dicoMots)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Bilan des mots générés :");
            sb.AppendLine("----------------------");
            int iNumME = 0;
            int iNbME = dicoMotsExistants.Count();
            foreach (var kvp in dicoMotsExistants)
            {
                clsMotExistant motsExistant = kvp.Value;

                Mot mot = null;
                string sCleMot = motsExistant.sMot;
                //if (sCleMot == "acrodonte")
                //    Debug.WriteLine("!");
                bool bMotExiste = dicoMots.ContainsKey(sCleMot);

                if (!bMotExiste)
                    mot = new Mot() { Mot_ = sCleMot };
                else
                    mot = dicoMots[sCleMot];

                string sClePrefixe = motsExistant.sPrefixe + "-";
                // 27/04/2019 Gestion des élisions
                if (motsExistant.sPrefixe.EndsWith("(o)")) {
                    motsExistant.sPrefixe = motsExistant.sPrefixe.Replace("(o)", "o");
                    sClePrefixe = motsExistant.sPrefixe + "-";
                }

                if (motsExistant.sUnicitePrefixe.Length > 0) 
                    sClePrefixe += ":" + motsExistant.sUnicitePrefixe ;
                if (!dicoPrefixes.ContainsKey(sClePrefixe))
                {
                    sb.AppendLine(sCleMot + " : Préfixe non trouvé : " + sClePrefixe);
                    continue;
                }
                var prefixe = dicoPrefixes[sClePrefixe];
                mot.Prefixe = prefixe;

                string sSegmentTiret = "-" + motsExistant.sSuffixe ;
                string sCleSuffixe = sSegmentTiret;
                if (motsExistant.sUniciteSuffixe.Length > 0)
                    sCleSuffixe += ":" + motsExistant.sUniciteSuffixe;
                if (!dicoSuffixes.ContainsKey(sCleSuffixe))
                {
                    if (sCleSuffixe=="-cyte") Debug.WriteLine ("!");
                    sb.AppendLine(sCleMot + " : Suffixe non trouvé : " + sCleSuffixe);
                    continue;
                }

                iNumME++;
                if (clsConst.bDebug && iNumME > iNbMotsExistantsMax) break;
                if (iNumME % 10 == 0 || iNumME >= iNbME)
                {
                    AfficherMsgBarreMsg("Mot n°" + iNumME + "/" + iNbME);
                    if (m_bAnnuler) return;
                }

                var suffixe = dicoSuffixes[sCleSuffixe];
                mot.Suffixe = suffixe;

                if (bMotExiste && m_bForcerMaj)
                {
                    var entry = context.Entry(mot);
                    // A moins que l'entité n'ait pas encore été sauvée, elle doit être modifiée
                    if (entry.State != EntityState.Added)
                        entry.State = EntityState.Modified;
                }
                else if (!bMotExiste)
                {
                    context.Mots.Add(mot);
                    m_sDernierTypeElemAjoute = "Mot";
                    m_sDernierElemAjoute = mot.Mot_;
                    if (clsConstMdb.bDebugDB) context.SaveChanges();
                    dicoMots.Add(mot.Mot_, mot);
                }
            }
            m_sDernierTypeElemAjoute = "Général";
            m_sDernierElemAjoute = "";
            context.SaveChanges();

            // Si le fichier n'est pas vide, c'est que la liste des mots n'est pas à jour
            string sCheminJson = Application.StartupPath +
                "\\BilanMots" + clsConstMdb.sLang + ".txt";
            FileHelpers.WriteFile(sCheminJson, sb, encode: Encoding.UTF8);
        }

        private void RechargerBase(Context context,
            Dictionary<string, Concept> dicoConcepts,
            Dictionary<string, Racine> dicoRacines,
            Dictionary<string, Segment> dicoSegments,
            Dictionary<string, Prefixe> dicoPrefixes,
            Dictionary<string, Suffixe> dicoSuffixes,
            Dictionary<string, Mot> dicoMots)
        {
            var concepts = from c in context.Concepts select c;
            if (bAjouterConcepts)
                foreach (Concept concept in concepts)
                {
                    if (!dicoConcepts.ContainsKey(concept.Concept_))
                        dicoConcepts.Add(concept.Concept_, concept);
                }

            var racines = from r in context.Racines select r;
            if (bAjouterRacines)
                foreach (Racine racine in racines)
                {
                    if (string.IsNullOrEmpty(racine.Sens))
                        racine.Sens = racine.Concept.Concept_; // 30/11/2018
                    if (!dicoRacines.ContainsKey(racine.CleRacine))
                        dicoRacines.Add(racine.CleRacine, racine);
                }

            var segments = from s in context.Segments select s;
            if (bAjouterSegments)
                foreach (Segment segment in segments)
                {
                    string sCleSegment = "0:" + segment.UniciteSynth;
                    if (segment.bPrefixe) sCleSegment = "1:" + segment.UniciteSynth;
                    if (!dicoSegments.ContainsKey(sCleSegment))
                        dicoSegments.Add(sCleSegment, segment);
                    if (string.IsNullOrEmpty(segment.Etymologie) &&
                        !string.IsNullOrEmpty(segment.Racine.Etymologie))
                        segment.Etymologie = segment.Racine.Etymologie;
                    if (string.IsNullOrEmpty(segment.Sens))
                        segment.Sens = segment.SensPrincipal;
                }

            var prefixes = from p in context.Prefixes select p;
            if (bAjouterPrefixes)
                foreach (Prefixe prefixe in prefixes)
                {
                    if (!dicoPrefixes.ContainsKey(prefixe.ClePrefixe))
                        dicoPrefixes.Add(prefixe.ClePrefixe, prefixe);
                    if (string.IsNullOrEmpty(prefixe.Etymologie) &&
                        !string.IsNullOrEmpty(prefixe.Segment.Etymologie))
                        prefixe.Etymologie = prefixe.Segment.Etymologie;
                }

            var suffixes = from s in context.Suffixes select s;
            if (bAjouterSuffixes)
                foreach (Suffixe suffixe in suffixes)
                {
                    if (!dicoSuffixes.ContainsKey(suffixe.CleSuffixe))
                        dicoSuffixes.Add(suffixe.CleSuffixe, suffixe);
                    if (string.IsNullOrEmpty(suffixe.Etymologie) &&
                        !string.IsNullOrEmpty(suffixe.Segment.Etymologie))
                        suffixe.Etymologie = suffixe.Segment.Etymologie;
                }
            
            var mots = from m in context.Mots select m;
            if (bAjouterMots)
                foreach (Mot mot in mots)
                {
                    if (!dicoMots.ContainsKey(mot.Mot_))
                        dicoMots.Add(mot.Mot_, mot);
                }
        }

        private void TraiterPrefixes(Context context,
            List<clsSegmentBase> lstPrefixes,
            Dictionary<string, Concept> dicoConcepts,
            Dictionary<string, Racine> dicoRacines,
            Dictionary<string, Segment> dicoSegments,
            Dictionary<string, Prefixe> dicoPrefixes,
            Dictionary<string, string> dicoSensConcept,
            HashSet<string> hsSegmentsExclus,
            List<string> lstNiv, bool bNeoRigolo,
            Dictionary<string, Racine> dicoRacinesUnicite, StringBuilder sbBilan)
        {
            int iNbPrefixes = lstPrefixes.Count;
            int iNumPrefixe = 0;
            foreach (clsSegmentBase prefixe0 in lstPrefixes)
            {
                iNumPrefixe++;
                if (clsConst.bDebug && iNumPrefixe > iNbPrefixesMax) break;

                if (iNumPrefixe % 10 == 0 || iNumPrefixe >= iNbPrefixes)
                {
                    AfficherMsgBarreMsg("Préfixe n°" + iNumPrefixe + "/" + iNbPrefixes);
                    if (m_bAnnuler) return;
                }

                string sSensSansArticle = clsBase.sSupprimerArticle(prefixe0.sSens);

                string sCleSegment = "1:" + prefixe0.sUniciteSynth; // 07/10/2018

                bool bLogotron = (prefixe0.sLogotron == clsConst.sSelectLogotron ? true : false);
                short iNiveau = (short)prefixe0.iNiveau;

                string sSegmentTiret = prefixe0.sSegment + "-";
                //if (sSegmentTiret == "duo-") Debug.WriteLine("!");
                string sClePrefixe = sSegmentTiret;
                if (prefixe0.sUnicite.Length > 0) 
                    sClePrefixe += ":" + prefixe0.sUnicite; // 10/11/2018

                string sSegment = prefixe0.sSegment;
                Segment segment = LireOuCreerSegment(context,
                    sCleSegment, sSegment, sSegmentTiret, prefixe0.sSens, sSensSansArticle,
                    prefixe0.sOrigine, iNiveau,
                    prefixe0.sEtym, prefixe0.sFrequence, bLogotron,
                    prefixe0.sUnicite, prefixe0.sUniciteSynth,
                    dicoSegments, dicoRacines, dicoConcepts, dicoSensConcept,
                    hsSegmentsExclus, dicoRacinesUnicite, sbBilan, bPrefixe: true);
                if (segment == null) continue;

                Prefixe prefixe = null;
                bool bPrefixeExiste = dicoPrefixes.ContainsKey(sClePrefixe);
                if (!bPrefixeExiste)
                    prefixe = new Prefixe() { 
                        ClePrefixe = sClePrefixe 
                    };
                else
                    prefixe = dicoPrefixes[sClePrefixe];

                prefixe.Prefixe_ = sSegmentTiret;
                prefixe.Segment = segment;
                prefixe.bLogotron = bLogotron;
                prefixe.Etymologie = prefixe0.sEtym;
                prefixe.Frequence = prefixe0.sFrequence;
                prefixe.Unicite = segment.Unicite;
                prefixe.UniciteSynth = segment.UniciteSynth;
                prefixe.Origine = prefixe0.sOrigine;
                if (bPrefixeExiste && m_bForcerMaj && bAjouterPrefixes)
                {
                    var entry = context.Entry(prefixe);
                    // A moins que l'entité n'ait pas encore été sauvée, elle doit être modifiée
                    if (entry.State != EntityState.Added)
                        entry.State = EntityState.Modified;
                }

                if (!bPrefixeExiste)
                {
                    if (bAjouterPrefixes)
                    {
                        context.Prefixes.Add(prefixe);
                        m_sDernierTypeElemAjoute = "Prefixe";
                        m_sDernierElemAjoute = sClePrefixe;
                        if (clsConstMdb.bDebugDB) context.SaveChanges();
                    }
                    dicoPrefixes.Add(sClePrefixe, prefixe);
                }

            }
            m_sDernierTypeElemAjoute = "Général";
            m_sDernierElemAjoute = "";
            context.SaveChanges();
        }
        
        private void TraiterSuffixes(Context context,
            List<clsSegmentBase> lstSuffixes,
            Dictionary<string, Concept> dicoConcepts,
            Dictionary<string, Racine> dicoRacines,
            Dictionary<string, Segment> dicoSegments,
            Dictionary<string, Suffixe> dicoSuffixes,
            Dictionary<string, string> dicoSensConcept,
            HashSet<string> hsSegmentsExclus,
            List<string> lstNiv, bool bNeoRigolo,
            Dictionary<string, Racine> dicoRacinesUnicite, StringBuilder sbBilan)
        {
            int iNbSuffixes = lstSuffixes.Count;
            int iNumSuffixe = 0;
            foreach (clsSegmentBase suffixe0 in lstSuffixes)
            {
                iNumSuffixe++;
                if (clsConst.bDebug && iNumSuffixe > iNbSuffixesMax) break;

                if (iNumSuffixe % 10 == 0 || iNumSuffixe >= iNbSuffixes)
                {
                    AfficherMsgBarreMsg("Suffixe n°" + iNumSuffixe + "/" + iNbSuffixes);
                    if (m_bAnnuler) return;
                }

                string sSensSansArticle = clsBase.sSupprimerArticle(suffixe0.sSens);

                string sCleSegment = "0:" + suffixe0.sUniciteSynth; // 07/10/2018

                bool bLogotron = (suffixe0.sLogotron == clsConst.sSelectLogotron ? true : false);
                short iNiveau = (short)suffixe0.iNiveau;

                string sSegmentTiret = "-" + suffixe0.sSegment;
                string sCleSuffixe = sSegmentTiret; // 07/10/2018
                if (suffixe0.sUnicite.Length > 0)
                    sCleSuffixe += ":" + suffixe0.sUnicite; // 10/11/2018
                string sSegment = suffixe0.sSegment;
                Segment segment = LireOuCreerSegment(context,
                    sCleSegment, sSegment, sSegmentTiret, suffixe0.sSens, sSensSansArticle,
                    suffixe0.sOrigine, iNiveau,
                    suffixe0.sEtym, suffixe0.sFrequence, bLogotron,
                    suffixe0.sUnicite, suffixe0.sUniciteSynth,
                    dicoSegments, dicoRacines, dicoConcepts, dicoSensConcept,
                    hsSegmentsExclus, dicoRacinesUnicite, sbBilan, bPrefixe: false);
                if (segment == null) continue;

                Suffixe suffixe = null;
                bool bSuffixeExiste = dicoSuffixes.ContainsKey(sCleSuffixe);
                if (!bSuffixeExiste)
                    suffixe = new Suffixe() { 
                        CleSuffixe = sCleSuffixe 
                    };
                else
                    suffixe = dicoSuffixes[sCleSuffixe];

                suffixe.Suffixe_ = sSegmentTiret;
                suffixe.Segment = segment;
                suffixe.bLogotron = bLogotron;
                suffixe.Etymologie = suffixe0.sEtym;
                suffixe.Frequence = suffixe0.sFrequence;
                suffixe.Unicite = segment.Unicite;
                suffixe.UniciteSynth = segment.UniciteSynth;
                suffixe.Origine = suffixe0.sOrigine;
                if (bSuffixeExiste && m_bForcerMaj && bAjouterSuffixes)
                {
                    var entry = context.Entry(suffixe);
                    // A moins que l'entité n'ait pas encore été sauvée, elle doit être modifiée
                    if (entry.State != EntityState.Added)
                        entry.State = EntityState.Modified;
                }

                if (!bSuffixeExiste)
                {
                    if (bAjouterSuffixes)
                    {
                        context.Suffixes.Add(suffixe);
                        m_sDernierTypeElemAjoute = "Suffixe";
                        m_sDernierElemAjoute = sCleSuffixe;
                        if (clsConstMdb.bDebugDB) context.SaveChanges();
                    }
                    dicoSuffixes.Add(sCleSuffixe, suffixe);
                }

            }
            m_sDernierTypeElemAjoute = "Général";
            m_sDernierElemAjoute = "";
            context.SaveChanges();
        }

        private void DetRacinesFinalesConcept(Context context)
        {
            foreach (Concept concept0 in context.Concepts)
            {
                int iNbRacines = concept0.lstRacines.Count;
                foreach (Racine racine0 in concept0.lstRacines)
                {
                    string sAjout = racine0.RacinePrincipale;
                    if (!concept0.hsRacines.Contains(sAjout))
                    {
                        concept0.hsRacines.Add(sAjout);
                        concept0.NbRacines = concept0.hsRacines.Count;
                        string sRacines = Util.hashSetToList(concept0.hsRacines);
                        if (sRacines.Length <= clsConstMdb.iMaxCar255)
                        {
                            concept0.Racines = sRacines;
                            var entry = context.Entry(concept0);
                            // A moins que l'entité n'ait pas encore été sauvée, elle doit être modifiée
                            if (entry.State != EntityState.Added)
                            {
                                entry.Property(o => o.Racines).IsModified = true;
                                entry.Property(o => o.NbRacines).IsModified = true;
                            }
                        }
                    }
                }
            }
            context.SaveChanges();
        }

        private void CreerRapportCoherence(Context context,
            Dictionary<string, Concept> dicoConcepts,
            Dictionary<string, Racine> dicoRacines,
            Dictionary<string, Segment> dicoSegments,
            Dictionary<string, Prefixe> dicoPrefixes,
            Dictionary<string, Suffixe> dicoSuffixes, 
            bool bBaseVide, StringBuilder sbBilanIn, StringBuilder sbBilanOut)
        {
            var sb = new StringBuilder();

            if (bBaseVide)
                sb.AppendLine("Bilan (base vide au départ)");
            else
                sb.AppendLine("Bilan (base non vide au départ)");
            sb.AppendLine("");

            sb.Append(sbBilanIn);
            sb.AppendLine("");
            sb.AppendLine("Lorsqu'un segment a des variantes distinctes, ces alertes peuvent être ignorées");
            sb.AppendLine("Lorsqu'un segment a des variantes similaires, vérifier l'origine et l'étymologie de ces variantes");
            sb.AppendLine("");

            AfficherMsgBarreMsg("Analyse de la cohérence des préfixes...");
            if (m_bAnnuler) return;

            sb.AppendLine("Analyse des différences entre les préfixes et les segments");
            sb.AppendLine("----------------------------------------------------------");
            foreach (Prefixe prefixe0 in context.Prefixes.OrderBy(t => t.IdPrefixe))
            {
                if (prefixe0.Segment == null) continue;

                if (prefixe0.Etymologie != prefixe0.Segment.Etymologie)
                {
                    sb.AppendLine("");
                    sb.AppendLine(prefixe0.Prefixe_ +
                        " : Etymologie : Segment = " + prefixe0.Segment.Segment_ +
                        " (" + prefixe0.Segment.Variantes + "), " +
                        "racine = " + prefixe0.Segment.Racine.CleRacine +
                        " (" + prefixe0.Segment.Racine.RacinePrincipale + ")");
                    sb.AppendLine(prefixe0.Prefixe_ + " : Préfixe" + clsConstMdb.sCrLf +
                        "  " + prefixe0.Etymologie + " (spécifique) <> ");
                    sb.AppendLine(prefixe0.Segment.Segment_ + " : Segment" + clsConstMdb.sCrLf +
                        "  " + prefixe0.Segment.Etymologie + " (principale)");
                }
                else
                {
                    prefixe0.Etymologie = null;
                    var entry = context.Entry(prefixe0);
                    if (entry.State != EntityState.Added)
                        entry.Property(o => o.Etymologie).IsModified = true;
                }
                
                if (prefixe0.Origine != prefixe0.Segment.Origine)
                {
                    sb.AppendLine("");
                    sb.AppendLine(prefixe0.Prefixe_ +
                        " : Origine : Segment = " + prefixe0.Segment.Segment_ +
                        " (" + prefixe0.Segment.Variantes + "), " +
                        "racine = " + prefixe0.Segment.Racine.CleRacine +
                        " (" + prefixe0.Segment.Racine.RacinePrincipale + ")");
                    sb.AppendLine(prefixe0.Prefixe_ + " : Préfixe" + clsConstMdb.sCrLf +
                        "  " + prefixe0.Origine + " (spécifique) <> ");
                    sb.AppendLine(prefixe0.Segment.Segment_ + " : Segment" + clsConstMdb.sCrLf +
                        "  " + prefixe0.Segment.Origine + " (principale)");
                }
                else
                {
                    prefixe0.Origine = null;
                    var entry = context.Entry(prefixe0);
                    if (entry.State != EntityState.Added)
                        entry.Property(o => o.Origine).IsModified = true;
                }
            }

            AfficherMsgBarreMsg("Analyse de la cohérence des suffixes...");
            if (m_bAnnuler) return;

            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Analyse des différences entre les suffixes et les segments");
            sb.AppendLine("----------------------------------------------------------");
            foreach (Suffixe suffixe0 in context.Suffixes.OrderBy(t => t.IdSuffixe))
            {
                if (suffixe0.Segment == null) continue;

                if (suffixe0.Etymologie != suffixe0.Segment.Etymologie)
                {
                    sb.AppendLine("");
                    sb.AppendLine(suffixe0.Suffixe_ +
                        " : Etymologie : Segment = " + suffixe0.Segment.Segment_ +
                        " (" + suffixe0.Segment.Variantes + "), " +
                        "Racine = " + suffixe0.Segment.Racine.CleRacine +
                        " (" + suffixe0.Segment.Racine.RacinePrincipale + ")");
                    sb.AppendLine(suffixe0.Suffixe_ + " : Suffixe" + clsConstMdb.sCrLf +
                        "  " + suffixe0.Etymologie + " (spécifique) <> ");
                    sb.AppendLine(suffixe0.Segment.Segment_ + " : Segment" + clsConstMdb.sCrLf +
                        "  " + suffixe0.Segment.Etymologie + " (principale)");
                }
                else
                {
                    suffixe0.Etymologie = null;
                    var entry = context.Entry(suffixe0);
                    if (entry.State != EntityState.Added)
                        entry.Property(o => o.Etymologie).IsModified = true;
                }

                if (suffixe0.Origine != suffixe0.Segment.Origine)
                {
                    sb.AppendLine("");
                    sb.AppendLine(suffixe0.Suffixe_ +
                        " : Origine : Segment = " + suffixe0.Segment.Segment_ +
                        " (" + suffixe0.Segment.Variantes + "), " +
                        "Racine = " + suffixe0.Segment.Racine.CleRacine +
                        " (" + suffixe0.Segment.Racine.RacinePrincipale + ")");
                    sb.AppendLine(suffixe0.Suffixe_ + " : Suffixe" + clsConstMdb.sCrLf +
                        "  " + suffixe0.Origine + " (spécifique) <> ");
                    sb.AppendLine(suffixe0.Segment.Segment_ + " : Segment" + clsConstMdb.sCrLf +
                        "  " + suffixe0.Segment.Origine + " (principale)");
                }
                else
                {
                    suffixe0.Origine = null;
                    var entry = context.Entry(suffixe0);
                    if (entry.State != EntityState.Added)
                        entry.Property(o => o.Origine).IsModified = true;
                }
            }

            AfficherMsgBarreMsg("Analyse de la cohérence des segments...");
            if (m_bAnnuler) return;

            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Analyse des différences entre les segments et les racines");
            sb.AppendLine("---------------------------------------------------------");
            foreach (Segment segment0 in context.Segments.OrderBy(t => t.IdSegment))
            {
                if (segment0.Racine == null) continue;

                if (segment0.Etymologie != segment0.Racine.Etymologie)
                {
                    sb.AppendLine("");
                    sb.AppendLine(segment0.Segment_ +
                        " (" + segment0.Variantes + ") : Etymologie : Segment" + clsConstMdb.sCrLf +
                        "  " + segment0.Etymologie + " (spécifique) <> " + clsConstMdb.sCrLf +
                        segment0.Racine.RacinePrincipale + " : " + segment0.Racine.CleRacine +
                        " : Racine" + clsConstMdb.sCrLf +
                        "  " + segment0.Racine.Etymologie + " (principale)");
                }
                else
                {
                    segment0.Etymologie = null;
                    var entry = context.Entry(segment0);
                    if (entry.State != EntityState.Added)
                        entry.Property(o => o.Etymologie).IsModified = true;
                }
                
                if (segment0.Origine != segment0.Racine.Origine)
                {
                    sb.AppendLine("");
                    sb.AppendLine(segment0.Segment_ +
                        " (" + segment0.Variantes + ") : Origine : Segment" + clsConstMdb.sCrLf +
                        "  " + segment0.Origine + " (spécifique) <> " + clsConstMdb.sCrLf +
                        segment0.Racine.RacinePrincipale + " : " + segment0.Racine.CleRacine +
                        " : Racine" + clsConstMdb.sCrLf +
                        "  " + segment0.Racine.Origine + " (principale)");
                }
                else
                {
                    segment0.Origine = null;
                    var entry = context.Entry(segment0);
                    if (entry.State != EntityState.Added)
                        entry.Property(o => o.Origine).IsModified = true;
                }

                if (segment0.Sens == segment0.SensPrincipal)
                {
                    segment0.Sens = null;
                    var entry = context.Entry(segment0);
                    if (entry.State != EntityState.Added)
                        entry.Property(o => o.Sens).IsModified = true;
                }

            }
            
            AfficherMsgBarreMsg("Analyse de la cohérence des racines...");
            if (m_bAnnuler) return;

            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Analyse des différences entre les racines et les concepts");
            sb.AppendLine("---------------------------------------------------------");
            foreach (Racine racine0 in context.Racines.OrderBy(t => t.IdRacine))
            {
                if (racine0.Concept == null) continue;

                if (racine0.Sens != racine0.Concept.Concept_)
                {
                    sb.AppendLine("");
                    sb.AppendLine(racine0.RacinePrincipale +
                        " (" + racine0.Segments + ") : Sens : Racine" + clsConstMdb.sCrLf +
                        "  " + racine0.Sens + " (spécifique) <> " + clsConstMdb.sCrLf +
                        racine0.Concept.Racines +" : Concept" + clsConstMdb.sCrLf +
                        "  " + racine0.Concept.Concept_ + " (principale)");
                }
                else
                {
                    racine0.Sens = null;
                    var entry = context.Entry(racine0);
                    if (entry.State != EntityState.Added)
                        entry.Property(o => o.Etymologie).IsModified = true;
                }
            }

            sbBilanOut.Append(sb); // Retourner le bilan completé
        }

        private void CreerRapportJson(Context context,
            Dictionary<string, Concept> dicoConcepts,
            Dictionary<string, Racine> dicoRacines,
            Dictionary<string, Segment> dicoSegments,
            Dictionary<string, Prefixe> dicoPrefixes,
            Dictionary<string, Suffixe> dicoSuffixes,
            Dictionary<string, Mot> dicoMots, 
            bool bIdTxt)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            int iNumLigne = 0;
            int iNbLignes = dicoConcepts.Count();
            sb.AppendLine("    \"Concepts\": [");
            foreach (Concept concept0 in context.Concepts.OrderBy (t => t.IdConcept))
            {
                iNumLigne++;
                sb.Append((bIdTxt? concept0.ToJsonTxtId() : concept0.ToJson()));
                if (iNumLigne < iNbLignes) sb.Append(",");
                sb.AppendLine("");
            }
            sb.AppendLine("    ],");

            iNumLigne = 0;
            iNbLignes = dicoRacines.Count();
            sb.AppendLine("");
            sb.AppendLine("    \"Racines\": [");
            foreach (Racine racine0 in context.Racines.OrderBy(t => t.IdRacine))
            {
                iNumLigne++;
                sb.Append((bIdTxt? racine0.ToJsonTxtId() : racine0.ToJson()));
                if (iNumLigne < iNbLignes) sb.Append(",");
                sb.AppendLine("");
            }
            sb.AppendLine("    ],");

            iNumLigne = 0;
            iNbLignes = dicoSegments.Count();
            sb.AppendLine("");
            sb.AppendLine("    \"Segments\": [");
            foreach (Segment segment0 in context.Segments.OrderBy(t => t.IdSegment))
            {
                iNumLigne++;
                sb.Append((bIdTxt? segment0.ToJsonTxtId() : segment0.ToJson()));
                if (iNumLigne < iNbLignes) sb.Append(",");
                sb.AppendLine("");
            }
            sb.AppendLine("    ],");

            iNumLigne = 0;
            iNbLignes = dicoPrefixes.Count();
            sb.AppendLine("");
            sb.AppendLine("    \"Prefixes\": [");
            foreach (Prefixe prefixe0 in context.Prefixes.OrderBy(t => t.IdPrefixe))
            {
                iNumLigne++;
                sb.Append((bIdTxt? prefixe0.ToJsonTxtId() : prefixe0.ToJson()));
                if (iNumLigne < iNbLignes) sb.Append(",");
                sb.AppendLine("");
            }
            sb.AppendLine("    ],");

            iNumLigne = 0;
            iNbLignes = dicoSuffixes.Count();
            sb.AppendLine("");
            sb.AppendLine("    \"Suffixes\": [");
            foreach (Suffixe suffixe0 in context.Suffixes.OrderBy(t => t.IdSuffixe))
            {
                iNumLigne++;
                sb.Append((bIdTxt ? suffixe0.ToJsonTxtId() : suffixe0.ToJson()));
                if (iNumLigne < iNbLignes) sb.Append(",");
                sb.AppendLine("");
            }
            if (!bAjouterMotsJSon)
                sb.AppendLine("    ]");
            else
                sb.AppendLine("    ],");

            if (bAjouterMotsJSon)
            {
                iNumLigne = 0;
                iNbLignes = dicoMots.Count();
                sb.AppendLine("");
                sb.AppendLine("    \"Mots\": [");
                foreach (Mot mot0 in context.Mots.OrderBy(t => t.IdMot))
                {
                    iNumLigne++;
                    sb.Append((bIdTxt ? mot0.ToJsonTxtId() : mot0.ToJson()));
                    if (iNumLigne < iNbLignes) sb.Append(",");
                    sb.AppendLine("");
                }
                sb.AppendLine("    ]");
            }

            sb.AppendLine("}");
            string sCheminJson = Application.StartupPath +
                (bIdTxt ? "\\LogotronBddIdTxt" + clsConstMdb.sLang + ".json" :
                          "\\LogotronBdd" + clsConstMdb.sLang + ".json");
            FileHelpers.WriteFile(sCheminJson, sb, encode: Encoding.UTF8);
        }
                
        private Segment LireOuCreerSegment(Context context,
            string sCleSegment, string sSegment, string sSegmentTiret, 
            string sSens, string sSensSansArticle,
            string sOrigine, short iNiveau,
            string sEtym, string sFrequence, bool bLogotron,
            string sUnicite, string sUniciteSynth, 
            Dictionary<string, Segment> dicoSegments,
            Dictionary<string, Racine> dicoRacines,
            Dictionary<string, Concept> dicoConcepts, 
            Dictionary<string, string> dicoSensConcept,
            HashSet<string> hsSegmentsExclus,
            Dictionary<string, Racine> dicoRacinesUnicite, StringBuilder sbBilan,
            bool bPrefixe)
        {

            //if (sCleSegment == "1:bassin") Debug.WriteLine("!");

            string sCleRacine = sSensSansArticle;
            string sCleConcept = sSensSansArticle;

            // Si on n'utilise pas l'unicité, on ne peut plus rassembler les variantes :
            if (sUnicite.Length > 0) sCleRacine = sUnicite;
            
            if (dicoSensConcept.ContainsKey (sCleConcept)) 
                sCleConcept = dicoSensConcept[sCleConcept];

            // Si une variante de racine se termine par o
            //  et que le segment principal ne se termine pas par o
            //  alors préférer cette variante comme segment principal
            //  Ex.: métall- et métallo- : préférer métallo-
            string sRacinePrincipale = sSegment;
            if (sCleRacine.EndsWith(sTermO) && !sRacinePrincipale.EndsWith(sTermO))
                sRacinePrincipale = sCleRacine; 
            if (sUnicite.Length > 0) sRacinePrincipale = sUnicite;

            // 09/11/2018
            string sCleUniciteRacine = sSegmentTiret;
            if (sUnicite.Length > 0) sCleUniciteRacine += ":" + sUnicite;
            if (dicoRacinesUnicite.ContainsKey(sCleUniciteRacine))
            {
                var racineE = dicoRacinesUnicite[sCleUniciteRacine];
                string sMsg = "Doublon racine : " + sCleUniciteRacine + " : " +
                    sSensSansArticle + ", autre sens déjà défini : " + racineE.Sens +
                    ", segment ignoré (utiliser l'unicité pour créer une racine distincte)";
                Debug.WriteLine(sMsg);
                sbBilan.AppendLine(sMsg);
                return null;
            }

            bool bRacinePrincipaleExclue = false ;
            if (hsSegmentsExclus.Contains(sRacinePrincipale)) bRacinePrincipaleExclue = true;

            bool bSegmentUnicite = false ;
            if (sSegment == sUnicite) bSegmentUnicite = true;

            Segment segment = null;
            bool bSegmentExiste = dicoSegments.ContainsKey(sCleSegment);
            if (!bSegmentExiste)
            {
                segment = new Segment() { CleSegment = sCleSegment };
                segment.Segment_ = sSegmentTiret;
                // Si on part d'une base pleine et qu'on veut le même résultat :
                segment.hsVariantes.Add(sSegmentTiret); // 28/10/2018
                segment.Variantes = sSegmentTiret;
                segment.bPrefixe = bPrefixe;
                segment.SensPrincipal = sSensSansArticle;
                segment.hsSens.Add(sSensSansArticle);
                segment.Sens = sSensSansArticle; // Liste de sens (string)
                segment.NbSens = 1;
                segment.Origine = sOrigine;
                segment.Etymologie = sEtym;
                segment.Unicite = sUnicite;
                segment.UniciteSynth = sUniciteSynth;
            }

            Racine racine = null;
            if (!dicoRacines.ContainsKey(sCleRacine))
            {
                racine = new Racine() { 
                    CleRacine = sCleRacine 
                };
                racine.RacinePrincipale = sRacinePrincipale;

                if (!bRacinePrincipaleExclue && !sRacinePrincipale.Contains(sParenth)) 
                    racine.hsSegments.Add(sRacinePrincipale); // 26/10/2018

                if (!sSegment.Contains(sParenth) && 
                    !racine.hsSegments.Contains(sSegment)) 
                    racine.hsSegments.Add(sSegment); // 02/11/2018
                racine.Segments = Util.hashSetToList(racine.hsSegments, clsConstMdb.iMaxCar255);
                racine.Sens = sSensSansArticle;
                racine.Niveau = iNiveau;
                racine.Etymologie = sEtym;
                racine.Origine = sOrigine;

                CreerConceptRacine(context, racine, sCleConcept, sSensSansArticle, dicoConcepts);

                if (bAjouterRacines)
                {
                    context.Racines.Add(racine);
                    m_sDernierTypeElemAjoute = "Racine";
                    m_sDernierElemAjoute = sCleRacine;
                    if (clsConstMdb.bDebugDB) context.SaveChanges();
                }
                dicoRacines.Add(sCleRacine, racine);

            }
            else
            {
                racine = dicoRacines[sCleRacine];

                if (racine.RacinePrincipale != sRacinePrincipale && 
                    sRacinePrincipale.EndsWith(sTermO) &&
                    !racine.RacinePrincipale.EndsWith(sTermO)) 
                {
                    racine.RacinePrincipale = sRacinePrincipale;
                    racine.Etymologie = sEtym; // 28/10/2018
                    racine.Origine = sOrigine; // 02/11/2018
                    racine.Sens = sSensSansArticle; // 11/11/2018
                    var entry = context.Entry(racine);
                    // A moins que l'entité n'ait pas encore été sauvée, elle doit être modifiée
                    if (entry.State != EntityState.Added) 
                    { 
                        entry.Property(o => o.RacinePrincipale).IsModified = true;
                        entry.Property(o => o.Etymologie).IsModified = true;
                        entry.Property(o => o.Origine).IsModified = true;
                        entry.Property(o => o.Sens).IsModified = true;
                    }
                }

                if (!sRacinePrincipale.Contains(sParenth) && 
                    !bRacinePrincipaleExclue &&
                    !racine.hsSegments.Contains(sRacinePrincipale))
                {
                    racine.hsSegments.Add(sRacinePrincipale);
                    string sSegments = Util.hashSetToList(racine.hsSegments);
                    if (sSegments.Length <= clsConstMdb.iMaxCar255)
                    {
                        racine.Segments = sSegments;
                        var entry = context.Entry(racine);
                        // A moins que l'entité n'ait pas encore été sauvée, elle doit être modifiée
                        if (entry.State != EntityState.Added)
                            entry.Property(o => o.Segments).IsModified = true;
                    }
                }

                // 26/10/2018
                if (!sSegment.Contains(sParenth) && !racine.hsSegments.Contains(sSegment))
                {
                    racine.hsSegments.Add(sSegment);
                    string sSegments = Util.hashSetToList(racine.hsSegments);
                    if (sSegments.Length <= clsConstMdb.iMaxCar255)
                    {
                        racine.Segments = sSegments;
                        var entry = context.Entry(racine);
                        // A moins que l'entité n'ait pas encore été sauvée, elle doit être modifiée
                        if (entry.State != EntityState.Added)
                            entry.Property(o => o.Segments).IsModified = true;
                    }
                }

                racine.Etymologie = sNormaliserEtym(racine.Etymologie, sEtym);

            }
            dicoRacinesUnicite.Add(sCleUniciteRacine, racine); //09/11/2018
            if (bSegmentUnicite) 
            { 
                racine.EtymologieUnicite = sEtym;
            }

            if (!bSegmentExiste)
            {
                segment.Racine = racine;
                if (bAjouterSegments)
                {
                    context.Segments.Add(segment);
                    m_sDernierTypeElemAjoute = "Segment";
                    m_sDernierElemAjoute = sCleSegment;
                    if (clsConstMdb.bDebugDB) context.SaveChanges();
                }
                dicoSegments.Add(sCleSegment, segment);
            }
            else
            { 
                segment = dicoSegments[sCleSegment];
                segment.Racine = racine;
                
                // 02/11/2018 Si on relit d'une base pleine, recharger toutes les infos
                //segment.Origine = sOrigine;
                // 27/06/2020 Ssi l'origine n'était pas renseignée
                if (!String.IsNullOrEmpty(sOrigine) && 
                     String.IsNullOrEmpty(segment.Origine))
                    segment.Origine = sOrigine;

                if (!segment.hsVariantes.Contains(sSegmentTiret))
                {
                    segment.hsVariantes.Add(sSegmentTiret);
                    string sVariantes = Util.hashSetToList(segment.hsVariantes);
                    if (sVariantes.Length <= clsConstMdb.iMaxCar255) 
                    {
                        segment.Variantes = sVariantes;
                        var entry = context.Entry(segment);
                        // A moins que l'entité n'ait pas encore été sauvée, elle doit être modifiée
                        if (entry.State != EntityState.Added)
                            entry.Property(o => o.Variantes).IsModified = true;
                    }
                }

                if (bPrefixe && !bRacinePrincipaleExclue) 
                { 
                    string sTermOFinale = sRacinePrincipale + "-";
                    string sTermOTiret = sTermO + "-";
                    if (segment.Segment_ != sTermOFinale &&
                        sTermOFinale.EndsWith(sTermOTiret) &&
                        !segment.Segment_.EndsWith(sTermOTiret))
                    {
                        segment.Segment_ = sTermOFinale;
                        // Si l'étymologie n'est pas déjà celle de l'unicité alors m.àj.
                        if (segment.Etymologie != racine.EtymologieUnicite)
                            segment.Etymologie = sEtym; // 28/10/2018
                        var entry = context.Entry(segment);
                        // A moins que l'entité n'ait pas encore été sauvée, elle doit être modifiée
                        if (entry.State != EntityState.Added) 
                        { 
                            entry.Property(o => o.Segment_).IsModified = true;
                        }
                    }
                }

                // Ajout d'une variante de sens a un segment existant 
                // Ex.: -phagie, -phage :?? mangeur, alimentation, mangé(e) / qui mange
                if (!segment.hsSens.Contains(sSensSansArticle))
                {
                    segment.hsSens.Add(sSensSansArticle);
                    segment.NbSens = segment.hsSens.Count;
                    string sListeSens = Util.hashSetToList(segment.hsSens, clsConstMdb.iMaxCar255);
                    segment.Sens = sListeSens;
                    var entry = context.Entry(segment);
                    // A moins que l'entité n'ait pas encore été sauvée, elle doit être modifiée
                    if (entry.State != EntityState.Added)
                    {
                        entry.Property(o => o.Sens).IsModified = true;
                        entry.Property(o => o.NbSens).IsModified = true;
                    }
                }

                segment.Etymologie = sNormaliserEtym(segment.Etymologie, sEtym);
            }
            
            return segment; 
        }

        private string sNormaliserEtym(string sEtymPrincip, string sEtymSpecif)
        {
            
            if (string.IsNullOrEmpty (sEtymPrincip)) return sEtymSpecif;

            string sEtymPrincipL = sEtymPrincip.ToLower();
            string sEtymSpecifL = sEtymSpecif.ToLower();
            if (sEtymPrincipL.Contains("variante") &&
                    !sEtymSpecifL.Contains("variante"))
            {
                return sEtymSpecif;
            }
            else
                if (sEtymPrincipL.Contains("forme erronée") &&
                    !sEtymSpecifL.Contains("forme erronée"))
                {
                    return sEtymSpecif;
                }
            return sEtymPrincip;
        }

        private void CreerConceptRacine(Context context, Racine racine, 
            string sCleConcept, string sSensSansArticle,
            Dictionary<string, Concept> dicoConcepts)
        {
            if (!dicoConcepts.ContainsKey(sCleConcept))
            {
                var concept = new Concept() { Concept_ = sCleConcept };
                
                // Il n'y a qu'un sens par concept
                concept.Racines = racine.RacinePrincipale; // "-"; // Champ requis, mais renseigné à la fin seulement
                racine.Concept = concept;
                if (bAjouterConcepts) 
                {
                    context.Concepts.Add(concept);
                    m_sDernierTypeElemAjoute = "Concept";
                    m_sDernierElemAjoute = sCleConcept;
                    if (clsConstMdb.bDebugDB) context.SaveChanges();
                }
                dicoConcepts.Add(sCleConcept, concept);
            }
            else
            {
                var conceptE = dicoConcepts[sCleConcept];
                racine.Concept = conceptE;
            }
        }
    }
}
