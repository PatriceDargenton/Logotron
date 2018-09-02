
using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

using UtilFichier = UtilWinForm.FileHelpers;
using LogotronLib;
using Logotron.Src; // frmQuiz
using LogotronLib.Src;
//using System.Runtime.CompilerServices;
using Logotron.Src.Util; // clsMsgDelegue

namespace Logotron
{

    public partial class frmLogotron : Form
    {
        private bool bDebug = clsConstWinForm.bDebug;
        bool m_bMajViaCode=false;
        
        public clsMsgDelegue m_msgDelegue;

        private void MsgBox(object sender, clsMsgEventArgs e)
        {
            UtilWinForm.clsMessageUtil.MsgBox(e.sMessage);
        }

        private void MsgBox(string sTxt)
        {
            UtilWinForm.clsMessageUtil.MsgBox(sTxt);
        }

        private int m_iIndex = 0;
        private void AfficherTexte(string sTxt)
        {
            UtilWinForm.clsUtil.AfficherTexteListBox(
                sTxt, ref m_iIndex, this, this.lbResultats);
        }

        public frmLogotron()
        {
            InitializeComponent();
            if (bDebug) this.StartPosition = FormStartPosition.CenterScreen;
            
            m_msgDelegue = new clsMsgDelegue();
            EventHandler<clsMsgEventArgs> obj = this.MsgBox;
            m_msgDelegue.EvAfficherMessage += obj;
            
            // S'il y a besoin de rafraichir (pas besoin pour un MsgBox
            //  mais util pour une barre de msg par exemple)
            //EventHandler<clsDoEventsEventArgs> obj2 = this.DoEvents;
            //m_msgDelegue.EvDoEvents += obj2;
        }
        
        private void DoEvents(object sender, clsDoEventsEventArgs e)
        {
            Application.DoEvents(); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Initialisations();
        }

        private void Initialisations()
        {
            string sTxt = Text + " - V" + clsConstWinForm.sVersionAppli +
                " (" + clsConstWinForm.sDateAppli + ")";
            if (bDebug) sTxt += " - Debug";
            this.Text = sTxt;

            Util.clsAppUtil.AppTitle = clsConstWinForm.sNomAppli; 

            m_bMajViaCode = true;
            this.lbNiveau.SetSelected(0, true);
            this.lbNiveau.SetSelected(1, true);
            this.lbNiveau.SetSelected(2, true);
            this.lbNbPrefixes.Text = LogotronLib.clsConst.sHasard;
            this.lbFreq.SetSelected(0, true);
            this.lbFreq.SetSelected(1, true);
            this.lbFreq.SetSelected(2, true);
            this.lbFreq.SetSelected(3, true);
            m_bMajViaCode = false;

            clsGestBase.InitBases();

            var dicoMotsExistants = new Dictionary<string, clsMotExistant>();
            if (clsConstWinForm.sModeLectureMotsExistants == 
                enumModeLectureMotExistant.Csv)
            {
                string sChemin = Application.StartupPath + "\\MotsSimples" + clsConstWinForm.sLang + ".csv";
                clsUtilLogotronWF.ChargerMotsExistantsCsv(dicoMotsExistants, sChemin);
            }
            else if (clsConstWinForm.sModeLectureMotsExistants == 
                enumModeLectureMotExistant.Code)
            {
                clsListeMotsExistants.ChargerMotsExistantsCode(dicoMotsExistants);
            }
            
            clsGestBase.ChargerMotsExistants(dicoMotsExistants);
            //string sCheminLogotronCsv = Application.StartupPath + "\\Logotron.csv";
            //string sCheminLogotronJson = Application.StartupPath + "\\Logotron.json";
            //if (clsConstWinForm.sModeLecture == enumModeLecture.JSon)
            //    clsLogotron.LireLogotronJSon();
            //else if (clsConstWinForm.sModeLecture == enumModeLecture.Code)
                clsLogotron.LireLogotronCode();
            //clsLogotron.InitialisationPrefixes(sCheminLogotronCsv, clsConst.sModeLecture);
            //InitialisationSuffixes(sModeLecture)

            MajNbMotLogotron();
        }

        private void cmdCopier_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            foreach (string sTxt in this.lbResultats.Items)
                sb.AppendLine(sTxt);
            string sTxtFinal = sb.ToString();
            string sTxtFinal2 = sTxtFinal.Replace(
                Util.clsUtil.sCarSautDeLigne + clsConstWinForm.sCrLf, "");
            if (UtilWinForm.clsMessageUtil.bCopyToClipboard(sTxtFinal2))
                MsgBox("Le texte a été copié dans le presse papier !");
        }

        private void cmdGo_Click(object sender, EventArgs e)
        {
            string sMot = "";
            string sExplication = "";
            string sDetail = "";
            List<string> lstEtym = new List<string>();
            string sNbPrefixesSuccessifs = this.lbNbPrefixes.Text;
            
            var lstNiv = new List<string>();
            //int iNumNiv = 0;
            foreach (object obj in this.lbNiveau.SelectedItems)
            {
                //iNumNiv ++;
                lstNiv.Add(obj.ToString());
            }
            //if (iNumNiv == 0) return;

            var lstFreq = new List<string>();
            foreach (object obj in this.lbFreq.SelectedItems)
            {
                string sFreqSelAbrege = obj.ToString();
                string sFreqSelComplet = enumFrequenceAbrege.sConv(sFreqSelAbrege);
                lstFreq.Add(sFreqSelComplet);
            }

            bool bGrecoLatin = this.chkOrigineGrecoLatin.Checked;
            bool bNeoRigolo = this.chkOrigineNeologisme.Checked;

            // Les préfixes et suffixes des mots du dictionnaire sont plus nombreux
            //  ne prendre que ceux qui forme des mots potentiels plausibles
            const bool bComplet = false;

            if (clsLogotron.bTirage(bComplet, sNbPrefixesSuccessifs, lstNiv, lstFreq, 
                ref sMot, ref sExplication, ref sDetail, ref lstEtym, 
                bGrecoLatin, bNeoRigolo, m_msgDelegue))
            {
                this.AfficherTexte(sMot);
                this.AfficherTexte(sExplication);
                this.AfficherTexte(sDetail);
                if (lstEtym.Count > 0)
                    foreach (string item in lstEtym) this.AfficherTexte(item);
                this.AfficherTexte("");
            }

        }

        private void MajNbMotLogotron()
        {
            List<string> lstNiv = new List<string>();
            string sNiveaux = "";
            foreach (object obj in this.lbNiveau.SelectedItems)
            {
                string sNiv = obj.ToString();
                sNiveaux += sNiv + " ";
                lstNiv.Add(sNiv);
            }
            this.AfficherTexte("Niveau(x) sélectionné(s) : " + sNiveaux);
            
            List<string> lstFreq = new List<string>();
            string sFreq = "";
            foreach (object obj in this.lbFreq.SelectedItems)
            {
                string sFreqSelAbrege = obj.ToString();
                string sFreqSelComplet = enumFrequenceAbrege.sConv(sFreqSelAbrege);
                sFreq += sFreqSelAbrege + " ";
                lstFreq.Add(sFreqSelComplet);
            }
            this.AfficherTexte("Fréquence(s) sélectionnée(s) : " + sFreq);

            bool bGrecoLatin = this.chkOrigineGrecoLatin.Checked;
            bool bNeoRigolo = this.chkOrigineNeologisme.Checked;
            int iNbPrefixes = clsGestBase.m_prefixes.iLireNbSegmentsUniques(
                lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            int iNbSuffixes = clsGestBase.m_suffixes.iLireNbSegmentsUniques(
                lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            long lNbPrefixesCombi = 0L;
            string sTxtCombi = "";
            string text = this.lbNbPrefixes.Text;
            if (text == "H" || text == "1")
            {
                sTxtCombi = iNbPrefixes + " préfixes";
                lNbPrefixesCombi = iNbPrefixes;
            }
            else if (text == "2")
            {
                sTxtCombi = iNbPrefixes + " préfixes x " +
                    (iNbPrefixes - 1) + " préfixes";
                lNbPrefixesCombi = iNbPrefixes * (iNbPrefixes - 1);
            }
            else if (text == "3")
            {
                sTxtCombi = iNbPrefixes + " préfixes x " +
                    (iNbPrefixes - 1) + " préfixes x " +
                    (iNbPrefixes - 2) + " préfixes";
                lNbPrefixesCombi = iNbPrefixes * (iNbPrefixes - 1) *
                    (iNbPrefixes - 2);
            }
            else if (text == "4")
            {
                sTxtCombi = iNbPrefixes + " préfixes x " +
                    (iNbPrefixes - 1) + " préfixes x " +
                    (iNbPrefixes - 2) + " préfixes x " +
                    (iNbPrefixes - 3) + " préfixes";
                lNbPrefixesCombi = (long)iNbPrefixes * (long)(iNbPrefixes - 1) * 
                    (iNbPrefixes - 2) * (iNbPrefixes - 3);
            }
            else if (text == "5")
            {
                sTxtCombi = iNbPrefixes + " préfixes x " +
                    (iNbPrefixes - 1) + " préfixes x " + 
                    (iNbPrefixes - 2) + " préfixes x " + 
                    (iNbPrefixes - 3) + " préfixes x " + 
                    (iNbPrefixes - 4) + " préfixes";
                lNbPrefixesCombi = (long)iNbPrefixes * (long)(iNbPrefixes - 1) *
                    (iNbPrefixes - 2) * (iNbPrefixes - 3) * (iNbPrefixes - 4);
            }
            long lNbCombi = lNbPrefixesCombi * iNbSuffixes;
            string sNbCombi = Util.clsUtil.sFormaterNumeriqueLong(lNbCombi);
            this.AfficherTexte(sTxtCombi + " x " + 
                (iNbSuffixes) + " suffixes = " + 
                sNbCombi + " combinaisons pour le Logotron");
            this.AfficherTexte("");

            if (this.chkOrigineGrecoLatin.Checked) return;
            var lstPrefixes = clsGestBase.m_prefixes.lstSegmentsAutreOrigine(lstNiv, bNeoRigolo);
            foreach (clsSegmentBase prefixe in lstPrefixes)
                AfficherTexte(prefixe.sAfficher(bPrefixe: true));
            var lstSuffixes = clsGestBase.m_suffixes.lstSegmentsAutreOrigine(lstNiv, bNeoRigolo);
            foreach (clsSegmentBase suffixe in lstSuffixes)
                AfficherTexte(suffixe.sAfficher(bPrefixe: false));
            this.AfficherTexte("");
        }

        private void lbNiveau_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_bMajViaCode) return;
            MajNbMotLogotron();
        }
        
        private void lbFreq_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_bMajViaCode) return;
            MajNbMotLogotron();
        }

        private void lbNbPrefixes_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_bMajViaCode) return;
            MajNbMotLogotron();
        }
        
        private void chkOrigineGrecoLatin_Click(object sender, EventArgs e)
        {
            if (m_bMajViaCode) return;
            if (this.chkOrigineGrecoLatin.Checked) this.chkOrigineNeologisme.Checked = false;
            MajNbMotLogotron();
        }
        
        private void chkOrigineNeoRigolo_Click(object sender, EventArgs e)
        {
            if (m_bMajViaCode) return;
            if (this.chkOrigineNeologisme.Checked) this.chkOrigineGrecoLatin.Checked = false;
            MajNbMotLogotron();
        }

        private void cmdQuiz_Click(object sender, EventArgs e)
        {
            using (Logotron.Src.frmQuiz frmQuiz0 = new Logotron.Src.frmQuiz())
            {
                frmQuiz0.StartPosition = FormStartPosition.CenterScreen;
                frmQuiz0.ShowDialog(this);
            }
        }
    }
}
