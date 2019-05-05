
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics; // Pour Debugger.Break();

//using UtilFichier = UtilWinForm.FileHelpers;
using Util;
using LogotronLib;
using Logotron.Src.Util; // clsMsgDelegue

namespace Logotron.Src
{
    public partial class frmQuiz : Form
    {

        private bool bDebug = clsConstWinForm.bDebug;
        const bool bDebugUnicite = false; // Afficher l'unicité si elle existe
        const bool bDebugUniciteSynth = false; // Afficher toujours l'unicité ou le sens, sinon

        bool m_bAnnuler = false;
        bool m_bAttendre = false;

        bool m_bMajViaCode = false;

        // Coefficient de bonus lorsque le préfixe et le suffixe sont justes
        const int iCoefBonus = 3;

        const string sTipsValider = "Valider une réponse du quiz";

        private clsMsgDelegue m_msgDelegue;
        
        private void MsgBox(object sender, clsMsgEventArgs e)
        {
            UtilWinForm.clsMessageUtil.MsgBox(e.sMessage);
        }

        private void MsgBox(string sTxt)
        {
            UtilWinForm.clsMessageUtil.MsgBox(sTxt);
            //MessageBox.Show(sTxt, clsAppUtil.AppTitle,
            //    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void AfficherMsgBarreMsg(string sTxt)
        {
            this.toolStripStatusLabelBarreMsg.Text = sTxt;
        }

        public frmQuiz()
        {
            InitializeComponent();
            if (bDebug) this.StartPosition = FormStartPosition.CenterScreen;
            
            m_msgDelegue = new clsMsgDelegue();
            EventHandler<clsMsgEventArgs> obj = this.MsgBox;
            m_msgDelegue.EvAfficherMessage += obj;
        }

        private void frmQuiz_Load(object sender, EventArgs e)
        {
            Initialisations();
        }

        private void frmQuiz_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_bAnnuler = true;
            m_bAttendre = false;
        }

        private void Initialisations()
        {
            this.ToolTip1.SetToolTip(this.cmdValider, sTipsValider);
            m_bMajViaCode = true;
            this.lbNbQuestions.Text = "5";
            this.lbAlternatives.Text = "1";
            this.lbNiveau.Text = "1";
            this.lbFreq.SetSelected(0, true);
            this.lbFreq.SetSelected(1, true);
            this.lbFreq.SetSelected(2, true);
            this.lbFreq.SetSelected(3, true);
            if (bDebug && clsConstWinForm.sModeLectureMotsExistants ==
                enumModeLectureMotExistant.Code) this.chkMotsExistants.Checked = false;
            m_bMajViaCode = false;
            MajNbMotsQuiz();
            AfficherMsgBarreMsg("");
            clsGestBase.m_prefixes.MsgDelegue = m_msgDelegue;
            clsGestBase.m_suffixes.MsgDelegue = m_msgDelegue;
            clsGestBase.m_msgDelegue = m_msgDelegue;
        }
        
        private void Sablier(bool bDesactiver)
        {
            if (bDesactiver)
                this.Cursor = Cursors.Default;
            else
                this.Cursor = Cursors.WaitCursor;
        }

        private void Activation(bool bActiver, bool bToutCtrl = false)
        {
            this.cmdQuiz.Enabled = bActiver;
            this.cmdValider.Enabled = false;
            this.lbNbQuestions.Enabled = bActiver;
            this.lbAlternatives.Enabled = bActiver;
            this.lbNiveau.Enabled = bActiver;
            this.lbFreq.Enabled = bActiver;

            if (bToutCtrl) {
                this.chkMotsExistants.Enabled = bActiver;
                this.chkInversion.Enabled = bActiver;
            }
        }

        private void lbNiveau_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_bMajViaCode) return ;
            MajNbMotsQuiz();
        }
        
        private void lbFreq_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_bMajViaCode) return;
            MajNbMotsQuiz();
        }

        private void chkMotsExistants_Click(object sender, EventArgs e)
        {
            if (m_bMajViaCode) return;
            // Pour les mots existants, toutes les origines sont incluses
            // (sinon il faudrait ajouter l'origine des préfixes et suffixes dans le fichier des mots existants)
            if (this.chkMotsExistants.Checked) this.chkOrigineGrecoLatin.Checked = false;
            if (this.chkMotsExistants.Checked) this.chkOrigineNeologisme.Checked = false;
            MajNbMotsQuiz();
        }

        private void chkOrigineGrecoLatin_Click(object sender, EventArgs e)
        {
            if (m_bMajViaCode) return;
            // Si sélectionne les seules origines Greco-latines,
            //  on ne peut plus se baser sur les mots existants (toutes origines)
            if (this.chkOrigineGrecoLatin.Checked) this.chkMotsExistants.Checked = false;
            if (this.chkOrigineGrecoLatin.Checked) this.chkOrigineNeologisme.Checked = false;
            MajNbMotsQuiz();
        }
        
        private void chkOrigineNeologisme_Click(object sender, EventArgs e)
        {
            if (m_bMajViaCode) return;
            if (this.chkOrigineNeologisme.Checked) this.chkOrigineGrecoLatin.Checked = false;
            if (this.chkOrigineNeologisme.Checked) this.chkMotsExistants.Checked = false;
            MajNbMotsQuiz();
        }

        private void chkInversion_Click(object sender, EventArgs e)
        {
            //if (m_bMajViaCode) return;
            //MajNbMotsQuiz();
        }

        private void MajNbMotsQuiz()
        {
            string sNiveaux = "";
            var lstNiv = new List<string> { };
            foreach (object obj in this.lbNiveau.SelectedItems)
            {
                string sNiv = obj.ToString();
                sNiveaux += sNiv + " ";
                lstNiv.Add(sNiv);
            }
            this.AfficherTexte("Niveau(x) sélectionné(s) : " + sNiveaux);

            string sFreq = "";
            var lstFreq = new List<string>();
            foreach (object obj in this.lbFreq.SelectedItems)
            {
                string sFreqSelAbrege = obj.ToString();
                sFreq += sFreqSelAbrege + " ";
                string sFreqSelComplet = enumFrequenceAbrege.sConv(sFreqSelAbrege);
                lstFreq.Add(sFreqSelComplet);
            }
            this.AfficherTexte("Fréquence(s) sélectionnée(s) : " + sFreq);

            if (this.chkMotsExistants.Checked) {

                int iNbMotsExistants0 = clsGestBase.iNbMotsExistants(lstNiv, lstFreq);
                string sNbMots = clsUtil.sFormaterNumeriqueLong(iNbMotsExistants0);
                AfficherTexte(sNbMots + " mots existants pour le Logotron");

                int iNbPE = clsGestBase.iNbPrefixesMotsExistants(lstNiv, lstFreq);
                string sNbPE = clsUtil.sFormaterNumeriqueLong(iNbPE);
                AfficherTexte(sNbPE + " préfixes distincts pour les mots existants");
                int iNbSE = clsGestBase.iNbSuffixesMotsExistants(lstNiv, lstFreq);
                string sNbSE = clsUtil.sFormaterNumeriqueLong(iNbSE);
                AfficherTexte(sNbSE + " suffixes distincts pour les mots existants");

                const bool bDebugMots = false;
                if (bDebugMots)
                {
                    int i = 0;
                    foreach (var mot in clsGestBase.lstMotsExistants(lstNiv, lstFreq))
                        AfficherTexte(++i + " : " + mot.ToString());
                }

            } else {

                bool bGrecoLatin = this.chkOrigineGrecoLatin.Checked;
                bool bNeoRigolo = this.chkOrigineNeologisme.Checked;
                int iNbPrefixes = clsGestBase.m_prefixes.iLireNbSegmentsUniques(
                    lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
                int iNbSuffixes = clsGestBase.m_suffixes.iLireNbSegmentsUniques(
                    lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);

                long lNbCombi = iNbPrefixes * iNbSuffixes;
                string sNbCombi = clsUtil.sFormaterNumeriqueLong(lNbCombi);

                AfficherTexte(iNbPrefixes + " préfixes" + " x " + iNbSuffixes + " suffixes = " + 
                    sNbCombi + " combinaisons pour le Logotron");
            }

            AfficherTexte("");
        }

        private int m_iIndex = 0;
        private void AfficherTexte(string sTxt)
        {
            UtilWinForm.clsUtil.AfficherTexteListBox(sTxt, ref m_iIndex, this, this.lbResultats);
            AfficherMsgBarreMsg(sTxt);
        }
        private void EffacerMessages()
        {
            this.lbResultats.Items.Clear();
            m_iIndex = 0;
        }

        private static void RemplirListBoxAuHasard(ListBox lb, List<string> lst) {
            
            int iNbElements = lst.Count;
            List<int> lstIndex = new List<int>();
            List<string> lstRnd = new List<string>();
            
            for (int i = 0; i <= iNbElements - 1; i++) {
            
            Recom:
                int iNumElement = clsUtil.iRandomiser(0, iNbElements - 1);
                if (lstIndex.Contains(iNumElement)) goto Recom;
            
                lstIndex.Add(iNumElement);
                lstRnd.Add(lst[iNumElement]);
            }
        
            lb.Items.Clear();
            foreach (string sPref in lstRnd) lb.Items.Add(sPref);
        
        }

        private void cmdValider_Click(object sender, EventArgs e)
        {
            if (m_bAttendre)
            {
                m_bAttendre = false;
                return;
            }

            this.cmdValider.Enabled = false;
        }

        private void cmdCopier_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            foreach (string sTxt in this.lbResultats.Items)
                sb.AppendLine(sTxt);
            string sTxtFinal = sb.ToString();
            string sTxtFinal2 = sTxtFinal.Replace(
                clsUtil.sCarSautDeLigne + clsConstWinForm.sCrLf, "");
            if (UtilWinForm.clsMessageUtil.bCopyToClipboard(sTxtFinal2)) 
                MsgBox("Le texte a été copié dans le presse papier !");
        }

        private void cmdQuiz_Click(object sender, EventArgs e)
        {
            m_bAnnuler = false;
            Activation(false);
            EffacerMessages();
            AfficherTexte("Préfixe juste : 1 point");
            AfficherTexte("Suffixe juste : 1 point");
            AfficherTexte("Préfixe et suffixe juste : 3 points");
            int iNbQuestions = int.Parse(this.lbNbQuestions.Text);
            //int iNiveau = int.Parse(this.lbNiveau.Text);
            string sNiv = "";
            var lstNiv = new List<string> { };
            foreach (object obj in this.lbNiveau.SelectedItems)
            {
                string sNiv0 = obj.ToString();
                sNiv += sNiv0 + " ";
                lstNiv.Add(sNiv0);
            }
            int iAlternatives = int.Parse(this.lbAlternatives.Text);
            int iScoreTot = 0;
            int iCoefAlternatives = iAlternatives + 1;
            //int iCoefNiv = 0; // iNiveau + 1;
            int iCoefNiv = enumNiveau.iCoef(sNiv);

            var lstFreq = new List<string>();
            string sFreq = "";
            foreach (object obj in this.lbFreq.SelectedItems)
            {
                string sFreqSelAbrege = obj.ToString();
                string sFreqSelComplet = enumFrequenceAbrege.sConv(sFreqSelAbrege);
                sFreq +=sFreqSelAbrege + " ";
                lstFreq.Add(sFreqSelComplet);
            }
            int iCoefFreq = enumFrequenceAbrege.iCoef(sFreq);

            int iCoefNBQ = iNbQuestions;
            for (int iNumQuestion = 0; iNumQuestion <= iNbQuestions - 1; iNumQuestion++)
            {
                AfficherTexte("");
                AfficherTexte("Question n°" + (iNumQuestion + 1) + " / " + iNbQuestions);
                if (this.chkInversion.Checked)
                {
                    this.ToolTip1.SetToolTip(this.lbSuffixesPossibles,
                        "Choisir le suffixe parmi la liste");
                    this.ToolTip1.SetToolTip(this.lbPrefixesPossibles,
                        "Choisir le préfixe parmi la liste");
                }
                else
                {
                    this.ToolTip1.SetToolTip(this.lbSuffixesPossibles,
                        "Choisir le sens du suffixe parmi la liste");
                    this.ToolTip1.SetToolTip(this.lbPrefixesPossibles,
                        "Choisir le sens du préfixe parmi la liste");
                }

                bool bErreur = false;
                int iScore = 0;
                if (this.chkInversion.Checked)
                {
                    if (this.chkMotsExistants.Checked)
                        QuizSegmentMotExistant(lstNiv, lstFreq, iAlternatives, 
                            ref iScore, ref bErreur);
                    else
                        QuizSegment(clsGestBase.m_prefixes, clsGestBase.m_suffixes,
                            lstNiv, lstFreq, iAlternatives, 
                            ref iScore, ref bErreur);
                }
                else if (this.chkMotsExistants.Checked)
                    QuizDefinitionMotExistant(lstNiv, lstFreq, iAlternatives,
                        ref iScore, ref bErreur);
                else
                    QuizDefinition(clsGestBase.m_prefixes, clsGestBase.m_suffixes,
                        lstNiv, lstFreq, iAlternatives, 
                        ref iScore, ref bErreur);

                if (m_bAnnuler) return;

                iScoreTot += iScore;
                string sScore = "Résultat = " + iScoreTot + " / " + 
                    (iNumQuestion + 1) * iCoefBonus;
                AfficherTexte(sScore);
                if (bErreur)
                {
                    //  Boucle d'attente pour comprendre l'erreur
                    Activation(bActiver:false, bToutCtrl:true);
                    this.cmdValider.Text = "Poursuivre";
                    this.ToolTip1.SetToolTip(this.cmdValider, "Poursuivre le quiz");
                    this.cmdValider.Enabled = true;
                    m_bAttendre = true;
                    while (m_bAttendre)
                    {
                        if (m_bAnnuler) break;
                        Application.DoEvents();
                    }

                    this.cmdValider.Text = "Valider";
                    this.ToolTip1.SetToolTip(this.cmdValider, sTipsValider);
                    Activation(bActiver:true, bToutCtrl:true);
                    Activation(bActiver:false);
                }
            }

            string sAffNiv = "niveau(x) " + sNiv + ", " ;
            string sAffFreq = "fréquence(s) " + sFreq;
            if (sFreq == "Fréq. Moy. Rare Abs. ")
                sAffFreq = ""; // Pas besoin d'afficher la fréquence alors
            else
                sAffFreq += ", ";
            string sResultatFinal = "Résultat final niveau "
                + sAffNiv + sAffFreq + " et difficulté " + iAlternatives + " avec "
                + iNbQuestions + " questions = " + iScoreTot + " / "
                + iNbQuestions * iCoefBonus;
            string sScoreFinal = "Score final = " + 
                iScoreTot * iCoefNiv * iCoefFreq * iCoefAlternatives * iCoefNBQ;
            AfficherTexte(sResultatFinal);
            AfficherTexte(sScoreFinal);
            Activation(true);
        }

        private void QuizSegment(LogotronLib.clsBase basePrefixe, LogotronLib.clsBase baseSuffixe,
            List<string> lstNiv, List<string> lstFreq, int iAlternatives, ref int iScore, ref bool bErreur) 
        {
            
            //  Quiz sur le préfixe et le suffixe correspondant à une définition

            iScore = 0;
            bool bGrecoLatin = this.chkOrigineGrecoLatin.Checked;
            bool bNeoRigolo = this.chkOrigineNeologisme.Checked;

            // Les préfixes et suffixes des mots du dictionnaire sont plus nombreux
            //  ne prendre que ceux qui forme des mots potentiels plausibles
            const bool bComplet = false;
            
            int iNumPrefixe = basePrefixe.iTirageSegment(
                bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            LogotronLib.clsSegmentBase prefixe = null;
            if (!basePrefixe.bLireSegment(iNumPrefixe, ref prefixe)) return;
        
            string sNiveauP = prefixe.sNiveau;
            int iNumSuffixe = baseSuffixe.iTirageSegment(
                bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            LogotronLib.clsSegmentBase suffixe = null;
            if (!baseSuffixe.bLireSegment(iNumSuffixe, ref suffixe)) return;
        
            string sNiveauS = suffixe.sNiveau;
            string sPrefixe = prefixe.sSegment;
            string sSuffixe = suffixe.sSegment;
            string sPrefixeMaj = sPrefixe.ToUpper();
            string sSuffixeMaj = sSuffixe.ToUpper();
            string sSensPrefixe = prefixe.sSens;
            string sSensPrefixeMaj = sSensPrefixe.ToUpper();
            sSensPrefixeMaj = clsBase.sCompleterPrefixe(sSensPrefixeMaj);
            string sEtymPrefixe = prefixe.sEtym;
            string sSensSuffixe = suffixe.sSens;
            string sSensSuffixeMaj = sSensSuffixe.ToUpper();
            string sEtymSuffixe = suffixe.sEtym;
            string sMot = sPrefixeMaj + sSuffixeMaj;
            string sDetail = sPrefixeMaj + "(" + sNiveauP + ")-" 
                + sSuffixeMaj + "(" + sNiveauS + ")";
            string sExplication = sSensSuffixeMaj + " " + sSensPrefixeMaj;
            string sPrefixeMajT = sPrefixeMaj + "-";
            string sTSuffixeMaj = "-" + sSuffixeMaj;
            var lstEtym = new List<string>();
            if (sEtymPrefixe.Length > 0) 
                lstEtym.Add(sPrefixe + "- : " + sEtymPrefixe);
        
            if (sEtymSuffixe.Length > 0) 
                lstEtym.Add("-" + sSuffixe + " : " + sEtymSuffixe);
        
            if (bDebugUnicite) {
                if (prefixe.sUnicite.Length > 0)
                    sPrefixeMajT += " (" + prefixe.sUnicite + ")";
                if ((suffixe.sUnicite.Length > 0))
                    sTSuffixeMaj += " (" + suffixe.sUnicite + ")";
            }
        
            if (bDebugUniciteSynth) {
                sPrefixeMajT += " (" + prefixe.sUniciteSynth + ")";
                sTSuffixeMaj += " (" + suffixe.sUniciteSynth + ")";
            }
        
            clsInitTirage itPrefixe = new clsInitTirage(prefixe);
            var lstPrefixesMajT = new List<string>();
            lstPrefixesMajT.Add(sPrefixeMajT);
            clsInitTirage itSuffixe = new clsInitTirage(suffixe);
            var lstSuffixesTMaj = new List<string>();
            lstSuffixesTMaj.Add(sTSuffixeMaj);
            for (int j = 0; j <= iAlternatives - 1; j++) {
                int iNumPrefixeAutre = basePrefixe.iTirageSegment(
                    bComplet, lstNiv, lstFreq, itPrefixe, bGrecoLatin, bNeoRigolo);
                clsSegmentBase prefixeP2 = null;
                if (!basePrefixe.bLireSegment(iNumPrefixeAutre, ref prefixeP2)) break;
            
                int iNumSuffixeAutre = baseSuffixe.iTirageSegment(
                    bComplet, lstNiv, lstFreq, itSuffixe, bGrecoLatin, bNeoRigolo);
                clsSegmentBase suffixeS2 = null;
                if (!baseSuffixe.bLireSegment(iNumSuffixeAutre, ref suffixeS2)) break;
            
                string sPrefixeAutre = (prefixeP2.sSegment.ToUpper() + "-");
                string sSuffixeAutre = ("-" + suffixeS2.sSegment.ToUpper());
                if (bDebugUnicite) {
                    if (prefixeP2.sUnicite.Length > 0) 
                        sPrefixeAutre += " ("  + prefixeP2.sUnicite + "]";
                    if (suffixeS2.sUnicite.Length > 0) 
                        sSuffixeAutre += " ("  + suffixeS2.sUnicite + "]";
                }
                if (bDebugUniciteSynth) {
                    sPrefixeAutre += " ("  + prefixeP2.sUniciteSynth + ")";
                    sSuffixeAutre += " ("  + suffixeS2.sUniciteSynth + ")";
                }
            
                lstPrefixesMajT.Add(sPrefixeAutre);
                lstSuffixesTMaj.Add(sSuffixeAutre);
            }
        
            RemplirListBoxAuHasard(lbPrefixesPossibles, lstPrefixesMajT);
            RemplirListBoxAuHasard(lbSuffixesPossibles, lstSuffixesTMaj);
            AfficherTexte(sExplication);
            this.cmdValider.Enabled = true;
            while (this.cmdValider.Enabled) {
                if (m_bAnnuler) {
                    Activation(true);
                    return;
                }
                Application.DoEvents();
            }
        
            string sPrefixeChoisi = this.lbPrefixesPossibles.Text;
            string sSuffixeChoisi = this.lbSuffixesPossibles.Text;
            bool bPrefixeOk;
            bool bSuffixeOk;
            bPrefixeOk = false;
            bSuffixeOk = false;
            if (sPrefixeChoisi == sPrefixeMajT && sSuffixeChoisi == sTSuffixeMaj) {
                iScore += iCoefBonus;
                bPrefixeOk = true;
                bSuffixeOk = true;
            }
            else if (sPrefixeChoisi == sPrefixeMajT) {
                iScore++;
                bPrefixeOk = true;
            }
            else if (sSuffixeChoisi == sTSuffixeMaj) {
                iScore++;
                bSuffixeOk = true;
            }
        
            string sAffPrefixe = sPrefixeChoisi + " : Faux ! ";
            string sAffSuffixe = sSuffixeChoisi + " : Faux ! ";
            if (sPrefixeChoisi.Length == 0) sAffPrefixe = "";
            if (sSuffixeChoisi.Length == 0) sAffSuffixe = "";
        
            AfficherTexte(sDetail);
            if (lstEtym.Count > 0)
                foreach (string sEtym in lstEtym) AfficherTexte(sEtym);
        
            bErreur = true;
            if (bPrefixeOk && bSuffixeOk) {
                AfficherTexte(sMot + " : Exact !!");
                bErreur = false;
            }
            else if (bPrefixeOk) {
                AfficherTexte(sPrefixeMaj + " : Exact !");
                AfficherTexte(sAffSuffixe + "Réponse = -" + sSuffixe + " : " + 
                    sSensSuffixeMaj);
            }
            else if (bSuffixeOk) {
                AfficherTexte(sAffPrefixe + "Réponse = " + sPrefixe + "- : " + 
                    sSensPrefixeMaj);
                AfficherTexte(sSensSuffixeMaj + " : Exact !");
            }
            else {
                AfficherTexte(sAffPrefixe + "Réponse = " + sPrefixe + "- : " + 
                    sSensPrefixeMaj);
                AfficherTexte(sAffSuffixe + "Réponse = -" + sSuffixe + " : " + 
                    sSensSuffixeMaj);
            }
        }

        private void QuizDefinition(clsBase basePrefixe, clsBase baseSuffixe,
            List<string> lstNiv, List<string> lstFreq, int iAlternatives,
            ref int iScore, ref bool bErreur) 
        {
            //  Quiz sur la définition du préfixe et du suffixe
            iScore = 0;
            bool bGrecoLatin = this.chkOrigineGrecoLatin.Checked;
            bool bNeoRigolo = this.chkOrigineNeologisme.Checked;

            // Les préfixes et suffixes des mots du dictionnaire sont plus nombreux
            //  ne prendre que ceux qui forme des mots potentiels plausibles
            const bool bComplet = false;

            int iNumPrefixe = basePrefixe.iTirageSegment(bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            clsSegmentBase prefixe = null;
            if (!basePrefixe.bLireSegment(iNumPrefixe, ref prefixe)) return;
        
            string sNiveauP = prefixe.sNiveau;
            int iNumSuffixe = baseSuffixe.iTirageSegment(bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            clsSegmentBase suffixe = null;
            if (!baseSuffixe.bLireSegment(iNumSuffixe, ref suffixe)) return;
        
            string sNiveauS = suffixe.sNiveau;
            string sPrefixe = prefixe.sSegment;
            string sSuffixe = suffixe.sSegment;
            string sPrefixeMaj = sPrefixe.ToUpper();
            string sSuffixeMaj = sSuffixe.ToUpper();
            string sSensPrefixeMaj = prefixe.sSens.ToUpper();
            sSensPrefixeMaj = clsBase.sCompleterPrefixe(sSensPrefixeMaj);
            string sEtymPrefixe = prefixe.sEtym;
            string sSensSuffixeMaj = suffixe.sSens.ToUpper();
            string sEtymSuffixe = suffixe.sEtym;
            string sMot = sPrefixeMaj + sSuffixeMaj;
            string sDetail = sPrefixeMaj + "(" + sNiveauP + ")-" + sSuffixeMaj + "(" + 
                sNiveauS + ")";
            string sExplication = sSensSuffixeMaj + " " + sSensPrefixeMaj;
            var lstEtym = new List<string>();
            if (sEtymPrefixe.Length > 0) lstEtym.Add(sPrefixe + "- : " + sEtymPrefixe);
        
            if (sEtymSuffixe.Length > 0) lstEtym.Add("-" + sSuffixe + " : " + sEtymSuffixe);
        
            clsInitTirage itPrefixe = new clsInitTirage(prefixe);
            clsInitTirage itSuffixe = new clsInitTirage(suffixe);
            if (bDebugUnicite) {
                if (prefixe.sUnicite.Length > 0)
                    sSensPrefixeMaj += " (" + prefixe.sUnicite + ")";
                if (suffixe.sUnicite.Length > 0)
                    sSensSuffixeMaj += " (" + suffixe.sUnicite + ")";
            }

            if (clsConst.bDebug && string.IsNullOrEmpty(prefixe.sUniciteSynth))
                Debugger.Break();
            if (clsConst.bDebug && string.IsNullOrEmpty(suffixe.sUniciteSynth))
                Debugger.Break();

            if (bDebugUniciteSynth) {
                sSensPrefixeMaj += " (" + prefixe.sUniciteSynth + ")";
                sSensSuffixeMaj += " (" + suffixe.sUniciteSynth + ")";
            }
        
            List<string> lstExplicationsPrefixe = new List<string>();
            lstExplicationsPrefixe.Add(sSensPrefixeMaj);
            List<string> lstExplicationsSuffixe = new List<string>();
            lstExplicationsSuffixe.Add(sSensSuffixeMaj);
            for (int j = 0; j <= iAlternatives - 1; j++) {
                int iNumPrefixeAutre = basePrefixe.iTirageSegment(
                    bComplet, lstNiv, lstFreq, itPrefixe, bGrecoLatin, bNeoRigolo);
                clsSegmentBase prefixeP2 = null;
                if (!basePrefixe.bLireSegment(iNumPrefixeAutre, ref prefixeP2)) break;
            
                string sSensPrefixeAutre = prefixeP2.sSens.ToUpper();
                sSensPrefixeAutre = clsBase.sCompleterPrefixe(sSensPrefixeAutre);
                int iNumSuffixeAutre = baseSuffixe.iTirageSegment(
                    bComplet, lstNiv, lstFreq, itSuffixe, bGrecoLatin, bNeoRigolo);
                clsSegmentBase suffixeS2 = null;
                if (!baseSuffixe.bLireSegment(iNumSuffixeAutre, ref suffixeS2)) break;
            
                string sSensSuffixeAutre = suffixeS2.sSens.ToUpper();
                if (bDebugUnicite) {
                    if (prefixeP2.sUnicite.Length > 0)
                        sSensPrefixeAutre += " (" + prefixeP2.sUnicite + "]";
                    if (suffixeS2.sUnicite.Length > 0)
                        sSensSuffixeAutre += " (" + suffixeS2.sUnicite + "]";
                }
                if (bDebugUniciteSynth) {
                    sSensPrefixeAutre += " (" + prefixeP2.sUniciteSynth + ")";
                    sSensSuffixeAutre += " (" + suffixeS2.sUniciteSynth + ")";
                }
            
                lstExplicationsPrefixe.Add(sSensPrefixeAutre);
                lstExplicationsSuffixe.Add(sSensSuffixeAutre);
            }
        
            RemplirListBoxAuHasard(lbPrefixesPossibles, lstExplicationsPrefixe);
            RemplirListBoxAuHasard(lbSuffixesPossibles, lstExplicationsSuffixe);
            AfficherTexte(sMot);
            this.cmdValider.Enabled = true;
            while (this.cmdValider.Enabled) {
                if (m_bAnnuler) {
                    Activation(true);
                    return;
                }
                Application.DoEvents();
            }
        
            string sSensPrefixeChoisi = this.lbPrefixesPossibles.Text;
            string sSensSuffixeChoisi = this.lbSuffixesPossibles.Text;
            bool bPrefixeOk;
            bool bSuffixeOk;
            bPrefixeOk = false;
            bSuffixeOk = false;
            if (sSensPrefixeChoisi == sSensPrefixeMaj && 
                sSensSuffixeChoisi == sSensSuffixeMaj) {
                iScore += iCoefBonus;
                bPrefixeOk = true;
                bSuffixeOk = true;
            }
            else if (sSensPrefixeChoisi == sSensPrefixeMaj) {
                iScore++;
                bPrefixeOk = true;
            }
            else if (sSensSuffixeChoisi == sSensSuffixeMaj) {
                iScore++;
                bSuffixeOk = true;
            }
        
            string sAffPrefixe = sSensPrefixeChoisi + " : Faux ! ";
            string sAffSuffixe = sSensSuffixeChoisi + " : Faux ! ";
            if (sSensPrefixeChoisi.Length == 0) sAffPrefixe = "";
            if (sSensSuffixeChoisi.Length == 0) sAffSuffixe = "";
        
            AfficherTexte(sDetail);
            if (lstEtym.Count > 0) 
                foreach (string sEtym in lstEtym) AfficherTexte(sEtym);
        
            bErreur = true;
            if (bPrefixeOk && bSuffixeOk) {
                AfficherTexte(sExplication + " : Exact !!");
                bErreur = false;
            }
            else if (bPrefixeOk) {
                AfficherTexte(sSensPrefixeMaj + " : Exact !");
                AfficherTexte(sAffSuffixe + "Réponse = -" + sSuffixe + " : " + sSensSuffixeMaj);
            }
            else if (bSuffixeOk) {
                AfficherTexte(sAffPrefixe + "Réponse = " + sPrefixe + "- : " + sSensPrefixeMaj);
                AfficherTexte(sSensSuffixeMaj + " : Exact !");
            }
            else {
                AfficherTexte(sAffPrefixe + "Réponse = " + sPrefixe + "- : " + sSensPrefixeMaj);
                AfficherTexte(sAffSuffixe + "Réponse = -" + sSuffixe + " : " + sSensSuffixeMaj);
            }
        }

        private void QuizSegmentMotExistant(
            List<string> lstNiv, List<string> lstFreq, int iAlternatives, 
            ref int iScore, ref bool bErreur) 
        {
            
            //  Quiz sur le préfixe et le suffixe correspondant à une définition

            iScore = 0;
            clsSegmentBase prefixe = null;
            clsSegmentBase suffixe = null;
            int iNumMotExistant = clsGestBase.iTirageMotExistant(lstNiv, lstFreq,
                ref prefixe, ref suffixe);
            if (iNumMotExistant == clsConst.iTirageImpossible) return;
        
            string sNiveauP = prefixe.sNiveau;
            string sNiveauS = suffixe.sNiveau;
            string sPrefixe = prefixe.sSegment;
            string sPrefixeElision = prefixe.sSegmentElision;
            string sSuffixe = suffixe.sSegment;
            //string sPrefixeMaj = sPrefixe.ToUpper();
            string sPrefixeMaj = sPrefixeElision.ToUpper(); // 01/05/2019
            string sSuffixeMaj = sSuffixe.ToUpper();
            string sSensPrefixe = prefixe.sSens;
            string sSensPrefixeMaj = sSensPrefixe.ToUpper();
            sSensPrefixeMaj = clsBase.sCompleterPrefixe(sSensPrefixeMaj);
            prefixe.sEtym = clsGestBase.m_prefixes.sTrouverEtymologie(sPrefixe, prefixe.sUniciteSynth); // 10/05/2018
            string sEtymPrefixe = prefixe.sEtym;
            string sSensSuffixe = suffixe.sSens;
            string sSensSuffixeMaj = sSensSuffixe.ToUpper();
            suffixe.sEtym = clsGestBase.m_suffixes.sTrouverEtymologie(sSuffixe, suffixe.sUniciteSynth); // 10/05/2018
            string sEtymSuffixe = suffixe.sEtym;
            string sMot = sPrefixeMaj + sSuffixeMaj;
            string sDetail = sPrefixeMaj + "(" + sNiveauP + ")-" + 
                sSuffixeMaj + "(" + sNiveauS + ")";
            string sExplication = sSensSuffixeMaj + " " + sSensPrefixeMaj;
            string sPrefixeMajT = sPrefixeMaj + "-";
            string sTSuffixeMaj = "-" + sSuffixeMaj;
            var lstEtym = new List<string>();
            if (sEtymPrefixe.Length > 0) lstEtym.Add(sPrefixe + "- : " + sEtymPrefixe);
            if (sEtymSuffixe.Length > 0) lstEtym.Add("-" + sSuffixe + " : " + sEtymSuffixe);
        
            if (bDebugUnicite) {
                if (prefixe.sUnicite.Length > 0)
                    sSensPrefixeMaj += " (" + prefixe.sUnicite + ")";
                if (suffixe.sUnicite.Length > 0)
                    sSensSuffixeMaj += " (" + suffixe.sUnicite + ")";
            }

            if (clsConst.bDebug && string.IsNullOrEmpty(prefixe.sUniciteSynth))
                Debugger.Break();
            if (clsConst.bDebug && string.IsNullOrEmpty(suffixe.sUniciteSynth))
                Debugger.Break();

            if (bDebugUniciteSynth) {
                sSensPrefixeMaj += " (" + prefixe.sUniciteSynth + ")";
                sSensSuffixeMaj += " (" + suffixe.sUniciteSynth + ")";
            }
        
            List<string> lstPrefixesMajT = new List<string>();
            lstPrefixesMajT.Add(sPrefixeMajT);
            List<string> lstSuffixesTMaj = new List<string>();
            lstSuffixesTMaj.Add(sTSuffixeMaj);
            List<int> lstNumMotExistant = new List<int>();
            clsInitTirage itPrefixe = new clsInitTirage(prefixe);
            clsInitTirage itSuffixe = new clsInitTirage(suffixe);
            for (int j = 0; j <= iAlternatives - 1; j++) {
                clsMotExistant motAutre = null;
                int iNumMotExistantAutre = clsGestBase.iTirageMotExistantAutre(
                    lstNiv, lstFreq, iNumMotExistant, itPrefixe, itSuffixe, 
                    lstNumMotExistant, ref motAutre);
                if (iNumMotExistantAutre == clsConst.iTirageImpossible) break;
                if (motAutre == null) break;

                string sDefPrefixe = motAutre.sPrefixe.ToUpper() + "-";
                string sDefSuffixe = "-" + motAutre.sSuffixe.ToUpper();
                
                if (bDebugUnicite) {
                    if ((motAutre.sUnicitePrefixe.Length > 0)) {
                        if (motAutre.sUnicitePrefixe.Length > 0) 
                            sDefPrefixe += " (" + motAutre.sUnicitePrefixe + ")";
                        if (motAutre.sUniciteSuffixe.Length > 0) 
                            sDefSuffixe += " (" + motAutre.sUniciteSuffixe + ")";
                    }
                }

                if (clsConst.bDebug && string.IsNullOrEmpty(motAutre.sUnicitePrefixeSynth))
                    Debugger.Break();
                if (clsConst.bDebug && string.IsNullOrEmpty(motAutre.sUniciteSuffixeSynth))
                    Debugger.Break();

                if (bDebugUniciteSynth) {
                    sDefPrefixe += " (" + motAutre.sUnicitePrefixeSynth + ")";
                    sDefSuffixe += " (" + motAutre.sUniciteSuffixeSynth + ")";
                }
            
                lstPrefixesMajT.Add(sDefPrefixe);
                lstSuffixesTMaj.Add(sDefSuffixe);
            }
        
            RemplirListBoxAuHasard(lbPrefixesPossibles, lstPrefixesMajT);
            RemplirListBoxAuHasard(lbSuffixesPossibles, lstSuffixesTMaj);
            AfficherTexte(sExplication);
            this.cmdValider.Enabled = true;
            while (this.cmdValider.Enabled) {
                if (m_bAnnuler) {
                    Activation(true);
                    return;
                }
                Application.DoEvents();
            }
        
            string sPrefixeChoisi = this.lbPrefixesPossibles.Text;
            string sSuffixeChoisi = this.lbSuffixesPossibles.Text;
            bool bPrefixeOk;
            bool bSuffixeOk;
            bPrefixeOk = false;
            bSuffixeOk = false;
            if (sPrefixeChoisi == sPrefixeMajT && 
                sSuffixeChoisi == sTSuffixeMaj) {
                iScore += iCoefBonus;
                bPrefixeOk = true;
                bSuffixeOk = true;
            }
            else if (sPrefixeChoisi == sPrefixeMajT) {
                iScore++;
                bPrefixeOk = true;
            }
            else if (sSuffixeChoisi == sTSuffixeMaj) {
                iScore++;
                bSuffixeOk = true;
            }
        
            string sAffPrefixe = sPrefixeChoisi + " : Faux ! ";
            string sAffSuffixe = sSuffixeChoisi + " : Faux ! ";
            if (sPrefixeChoisi.Length == 0) sAffPrefixe = "";
            if (sSuffixeChoisi.Length == 0) sAffSuffixe = "";
        
            AfficherTexte(sDetail);
            if (lstEtym.Count > 0) 
                foreach (string sEtym in lstEtym) AfficherTexte(sEtym);
            
            bErreur = true;
            if (bPrefixeOk && bSuffixeOk) {
                AfficherTexte((sMot + " : Exact !!"));
                bErreur = false;
            }
            else if (bPrefixeOk) {
                AfficherTexte(sPrefixeMaj + " : Exact !");
                AfficherTexte(sAffSuffixe + "Réponse = -" + sSuffixe + " : " + sSensSuffixeMaj);
            }
            else if (bSuffixeOk) {
                AfficherTexte(sAffPrefixe + "Réponse = " + sPrefixe + "- : " + sSensPrefixeMaj);
                AfficherTexte(sSensSuffixeMaj + " : Exact !");
            }
            else {
                AfficherTexte(sAffPrefixe + "Réponse = " + sPrefixe + "- : " + sSensPrefixeMaj);
                AfficherTexte(sAffSuffixe + "Réponse = -" + sSuffixe + " : " + sSensSuffixeMaj);
            }
        }

        private void QuizDefinitionMotExistant(
            List<string> lstNiv, List<string> lstFreq, int iAlternatives, 
            ref int iScore, ref bool bErreur) 
        {
            
            //  Quiz sur la définition du préfixe et du suffixe
            
            iScore = 0;
            // Recomm:
            clsSegmentBase prefixe = null;
            clsSegmentBase suffixe = null;
            int iNumMotExistant = clsGestBase.iTirageMotExistant(
                lstNiv, lstFreq, ref prefixe, ref suffixe);
            if (iNumMotExistant == clsConst.iTirageImpossible) return;
        
            // If iNumMotExistant <> 211 Then GoTo Recomm
            string sNiveauP = prefixe.sNiveau;
            string sNiveauS = suffixe.sNiveau;
            string sPrefixe = prefixe.sSegment;
            string sPrefixeElision = prefixe.sSegmentElision;
            string sSuffixe = suffixe.sSegment;
            //string sPrefixeMaj = sPrefixe.ToUpper();
            string sPrefixeMaj = sPrefixeElision.ToUpper(); // 01/05/2019
            string sSuffixeMaj = sSuffixe.ToUpper();
            string sSensPrefixeMaj = prefixe.sSens.ToUpper();
            sSensPrefixeMaj = clsBase.sCompleterPrefixe(sSensPrefixeMaj);
            prefixe.sEtym = clsGestBase.m_prefixes.sTrouverEtymologie(sPrefixe, prefixe.sUniciteSynth); // 10/05/2018
            string sEtymPrefixe = prefixe.sEtym;
            string sSensSuffixeMaj = suffixe.sSens.ToUpper();
            suffixe.sEtym = clsGestBase.m_suffixes.sTrouverEtymologie(sSuffixe, suffixe.sUniciteSynth); // 10/05/2018
            string sEtymSuffixe = suffixe.sEtym;
            string sMot = sPrefixeMaj + sSuffixeMaj;
            string sDetail = sPrefixeMaj + "(" + sNiveauP + ")-" + sSuffixeMaj + "(" + sNiveauS + ")";
            string sExplication = sSensSuffixeMaj + " " + sSensPrefixeMaj;
            var lstEtym = new List<string>();
            if (sEtymPrefixe.Length > 0) lstEtym.Add(sPrefixe + "- : " + sEtymPrefixe);
            if (sEtymSuffixe.Length > 0) lstEtym.Add("-" + sSuffixe + " : " + sEtymSuffixe);
        
            List<int> lstNumMotExistant = new List<int>();
            clsInitTirage itPrefixe = new clsInitTirage(prefixe);
            clsInitTirage itSuffixe = new clsInitTirage(suffixe);
            List<string> lstExplicationsPrefixe = new List<string>();
            if (bDebugUnicite) {
                //Debug.WriteLine(("iNumMotExistant = " + iNumMotExistant));
                if (prefixe.sUnicite.Length > 0)
                    sSensPrefixeMaj += " (" + prefixe.sUnicite + ")";
                if (suffixe.sUnicite.Length > 0)
                    sSensSuffixeMaj += " (" + suffixe.sUnicite + ")";
            }

            if (clsConst.bDebug && string.IsNullOrEmpty(prefixe.sUniciteSynth))
                Debugger.Break();
            if (clsConst.bDebug && string.IsNullOrEmpty(suffixe.sUniciteSynth))
                Debugger.Break();

            if (bDebugUniciteSynth) {
                //Debug.WriteLine(("iNumMotExistant = " + iNumMotExistant));
                sSensPrefixeMaj += " (" + prefixe.sUniciteSynth + ")";
                sSensSuffixeMaj += " (" + suffixe.sUniciteSynth + ")";
            }
        
            lstExplicationsPrefixe.Add(sSensPrefixeMaj);
            List<string> lstExplicationsSuffixe = new List<string>();
            lstExplicationsSuffixe.Add(sSensSuffixeMaj);
            for (int j = 0; j <= iAlternatives - 1; j++) {
                clsMotExistant motAutre = null;
                int iNumMotExistantAutre = clsGestBase.iTirageMotExistantAutre(
                    lstNiv, lstFreq, iNumMotExistant, itPrefixe, itSuffixe, lstNumMotExistant, ref motAutre);
                if (iNumMotExistantAutre == clsConst.iTirageImpossible) break;
                if (motAutre == null) break;
            
                string sDefPrefixe = motAutre.sDefPrefixe;
                string sDefSuffixe = motAutre.sDefSuffixe;
                if (bDebugUnicite) {
                    if ((motAutre.sUnicitePrefixe.Length > 0)) {
                        if (motAutre.sUnicitePrefixe.Length > 0) 
                            sDefPrefixe += " [" + motAutre.sUnicitePrefixe + "]";
                        if (motAutre.sUniciteSuffixe.Length > 0) 
                            sDefSuffixe += " [" + motAutre.sUniciteSuffixe + "]";
                    }
                }

                if (clsConst.bDebug && string.IsNullOrEmpty(motAutre.sUnicitePrefixeSynth))
                    Debugger.Break();
                if (clsConst.bDebug && string.IsNullOrEmpty(motAutre.sUniciteSuffixeSynth))
                    Debugger.Break();

                if (bDebugUniciteSynth) {
                    sDefPrefixe += " (" + motAutre.sUnicitePrefixeSynth + ")";
                    sDefSuffixe += " (" + motAutre.sUniciteSuffixeSynth + ")";
                }
            
                lstExplicationsPrefixe.Add(sDefPrefixe);
                lstExplicationsSuffixe.Add(sDefSuffixe);
            }
        
            RemplirListBoxAuHasard(lbPrefixesPossibles, lstExplicationsPrefixe);
            RemplirListBoxAuHasard(lbSuffixesPossibles, lstExplicationsSuffixe);
            AfficherTexte(sMot);
            this.cmdValider.Enabled = true;
            while (this.cmdValider.Enabled) {
                if (m_bAnnuler) {
                    Activation(true);
                    return;
                }
                Application.DoEvents();
            }
        
            string sSensPrefixeChoisi = this.lbPrefixesPossibles.Text;
            string sSensSuffixeChoisi = this.lbSuffixesPossibles.Text;
            bool bPrefixeOk;
            bool bSuffixeOk;
            bPrefixeOk = false;
            bSuffixeOk = false;
            if (sSensPrefixeChoisi == sSensPrefixeMaj && 
                sSensSuffixeChoisi == sSensSuffixeMaj) {
                iScore += iCoefBonus;
                bPrefixeOk = true;
                bSuffixeOk = true;
            }
            else if (sSensPrefixeChoisi == sSensPrefixeMaj) {
                iScore++;
                bPrefixeOk = true;
            }
            else if (sSensSuffixeChoisi == sSensSuffixeMaj) {
                iScore++;
                bSuffixeOk = true;
            }
        
            string sAffPrefixe = sSensPrefixeChoisi + " : Faux ! ";
            string sAffSuffixe = sSensSuffixeChoisi + " : Faux ! ";
            if (sSensPrefixeChoisi.Length == 0) sAffPrefixe = "";
            if (sSensSuffixeChoisi.Length == 0) sAffSuffixe = "";
        
            AfficherTexte(sDetail);
            if (lstEtym.Count > 0)
                foreach (string sEtym in lstEtym) AfficherTexte(sEtym);
        
            bErreur = true;
            if (bPrefixeOk && bSuffixeOk) {
                AfficherTexte(sExplication + " : Exact !!");
                bErreur = false;
            }
            else if (bPrefixeOk) {
                AfficherTexte(sSensPrefixeMaj + " : Exact !");
                AfficherTexte(sAffSuffixe + "Réponse = -" + sSuffixe + " : " + sSensSuffixeMaj);
            }
            else if (bSuffixeOk) {
                AfficherTexte(sAffPrefixe + "Réponse = " + sPrefixe + "- : " + sSensPrefixeMaj);
                AfficherTexte(sSensSuffixeMaj + " : Exact !");
            }
            else {
                AfficherTexte(sAffPrefixe + "Réponse = " + sPrefixe + "- : " + sSensPrefixeMaj);
                AfficherTexte(sAffSuffixe + "Réponse = -" + sSuffixe + " : " + sSensSuffixeMaj);
            }
        }

        private void lbSuffixesPossibles_Click(object sender, EventArgs e)
        {
            AfficherMsgBarreMsg(lbSuffixesPossibles.Text);
        }

        private void lbPrefixesPossibles_Click(object sender, EventArgs e)
        {
            AfficherMsgBarreMsg(lbPrefixesPossibles.Text);
        }

        private void lbResultats_Click(object sender, EventArgs e)
        {
            AfficherMsgBarreMsg(lbResultats.Text);
        }

        
    }
}
