namespace Logotron.Src
{
    partial class frmQuiz
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQuiz));
            this.lblNiv = new System.Windows.Forms.Label();
            this.lbNiveau = new System.Windows.Forms.ListBox();
            this.chkInversion = new System.Windows.Forms.CheckBox();
            this.cmdCopier = new System.Windows.Forms.Button();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkMotsExistants = new System.Windows.Forms.CheckBox();
            this.lbResultats = new System.Windows.Forms.ListBox();
            this.cmdQuiz = new System.Windows.Forms.Button();
            this.lbPrefixesPossibles = new System.Windows.Forms.ListBox();
            this.lbSuffixesPossibles = new System.Windows.Forms.ListBox();
            this.lbAlternatives = new System.Windows.Forms.ListBox();
            this.lbNbQuestions = new System.Windows.Forms.ListBox();
            this.chkOrigineGrecoLatin = new System.Windows.Forms.CheckBox();
            this.lbFreq = new System.Windows.Forms.ListBox();
            this.chkOrigineNeologisme = new System.Windows.Forms.CheckBox();
            this.cmdValider = new System.Windows.Forms.Button();
            this.lblDefSuffixe = new System.Windows.Forms.Label();
            this.lblDefPrefixe = new System.Windows.Forms.Label();
            this.lblAlternatives = new System.Windows.Forms.Label();
            this.lblNbQuestions = new System.Windows.Forms.Label();
            this.lblFreq = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelBarreMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNiv
            // 
            this.lblNiv.AutoSize = true;
            this.lblNiv.Location = new System.Drawing.Point(18, 129);
            this.lblNiv.Name = "lblNiv";
            this.lblNiv.Size = new System.Drawing.Size(29, 13);
            this.lblNiv.TabIndex = 30;
            this.lblNiv.Text = "Niv.:";
            // 
            // lbNiveau
            // 
            this.lbNiveau.FormattingEnabled = true;
            this.lbNiveau.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.lbNiveau.Location = new System.Drawing.Point(18, 145);
            this.lbNiveau.Name = "lbNiveau";
            this.lbNiveau.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbNiveau.Size = new System.Drawing.Size(23, 43);
            this.lbNiveau.TabIndex = 29;
            this.ToolTip1.SetToolTip(this.lbNiveau, "Indiquer le niveau du quiz (1 : facile, 2 : moyen, 3 : difficile) (multisélection" +
        " possible)");
            this.lbNiveau.SelectedValueChanged += new System.EventHandler(this.lbNiveau_SelectedValueChanged);
            // 
            // chkInversion
            // 
            this.chkInversion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkInversion.AutoSize = true;
            this.chkInversion.Location = new System.Drawing.Point(124, 211);
            this.chkInversion.Name = "chkInversion";
            this.chkInversion.Size = new System.Drawing.Size(61, 17);
            this.chkInversion.TabIndex = 28;
            this.chkInversion.Text = "Inversé";
            this.ToolTip1.SetToolTip(this.chkInversion, "Cocher pour inverser le quiz (pour les questions suivantes) : trouver le préfixe " +
        "et le suffixe d\'une définition");
            this.chkInversion.UseVisualStyleBackColor = true;
            this.chkInversion.Click += new System.EventHandler(this.chkInversion_Click);
            // 
            // cmdCopier
            // 
            this.cmdCopier.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdCopier.Location = new System.Drawing.Point(58, 145);
            this.cmdCopier.Name = "cmdCopier";
            this.cmdCopier.Size = new System.Drawing.Size(51, 29);
            this.cmdCopier.TabIndex = 27;
            this.cmdCopier.Text = "Copier";
            this.ToolTip1.SetToolTip(this.cmdCopier, "Copier le résultat dans le presse-papier de Windows");
            this.cmdCopier.UseVisualStyleBackColor = true;
            this.cmdCopier.Click += new System.EventHandler(this.cmdCopier_Click);
            // 
            // chkMotsExistants
            // 
            this.chkMotsExistants.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkMotsExistants.AutoSize = true;
            this.chkMotsExistants.Checked = true;
            this.chkMotsExistants.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMotsExistants.Location = new System.Drawing.Point(206, 211);
            this.chkMotsExistants.Name = "chkMotsExistants";
            this.chkMotsExistants.Size = new System.Drawing.Size(63, 17);
            this.chkMotsExistants.TabIndex = 31;
            this.chkMotsExistants.Text = "Existant";
            this.ToolTip1.SetToolTip(this.chkMotsExistants, "Cocher pour faire le quiz sur les mots existants du dictionnaire (pour les questi" +
        "ons suivantes)");
            this.chkMotsExistants.UseVisualStyleBackColor = true;
            this.chkMotsExistants.Click += new System.EventHandler(this.chkMotsExistants_Click);
            // 
            // lbResultats
            // 
            this.lbResultats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbResultats.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbResultats.FormattingEnabled = true;
            this.lbResultats.ItemHeight = 16;
            this.lbResultats.Location = new System.Drawing.Point(124, 19);
            this.lbResultats.Name = "lbResultats";
            this.lbResultats.Size = new System.Drawing.Size(601, 164);
            this.lbResultats.TabIndex = 25;
            this.ToolTip1.SetToolTip(this.lbResultats, "Questions et résultats");
            this.lbResultats.Click += new System.EventHandler(this.lbResultats_Click);
            // 
            // cmdQuiz
            // 
            this.cmdQuiz.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdQuiz.Location = new System.Drawing.Point(58, 204);
            this.cmdQuiz.Name = "cmdQuiz";
            this.cmdQuiz.Size = new System.Drawing.Size(51, 29);
            this.cmdQuiz.TabIndex = 20;
            this.cmdQuiz.Text = "Quiz";
            this.ToolTip1.SetToolTip(this.cmdQuiz, "Démarrer le quiz");
            this.cmdQuiz.UseVisualStyleBackColor = true;
            this.cmdQuiz.Click += new System.EventHandler(this.cmdQuiz_Click);
            // 
            // lbPrefixesPossibles
            // 
            this.lbPrefixesPossibles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbPrefixesPossibles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrefixesPossibles.FormattingEnabled = true;
            this.lbPrefixesPossibles.ItemHeight = 16;
            this.lbPrefixesPossibles.Location = new System.Drawing.Point(423, 265);
            this.lbPrefixesPossibles.Name = "lbPrefixesPossibles";
            this.lbPrefixesPossibles.Size = new System.Drawing.Size(270, 164);
            this.lbPrefixesPossibles.TabIndex = 19;
            this.ToolTip1.SetToolTip(this.lbPrefixesPossibles, "Choisir le sens du préfixe parmi la liste");
            this.lbPrefixesPossibles.Click += new System.EventHandler(this.lbPrefixesPossibles_Click);
            // 
            // lbSuffixesPossibles
            // 
            this.lbSuffixesPossibles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbSuffixesPossibles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSuffixesPossibles.FormattingEnabled = true;
            this.lbSuffixesPossibles.ItemHeight = 16;
            this.lbSuffixesPossibles.Location = new System.Drawing.Point(124, 265);
            this.lbSuffixesPossibles.Name = "lbSuffixesPossibles";
            this.lbSuffixesPossibles.Size = new System.Drawing.Size(270, 164);
            this.lbSuffixesPossibles.TabIndex = 18;
            this.ToolTip1.SetToolTip(this.lbSuffixesPossibles, "Choisir le sens du suffixe parmi la liste");
            this.lbSuffixesPossibles.Click += new System.EventHandler(this.lbSuffixesPossibles_Click);
            // 
            // lbAlternatives
            // 
            this.lbAlternatives.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbAlternatives.FormattingEnabled = true;
            this.lbAlternatives.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.lbAlternatives.Location = new System.Drawing.Point(21, 280);
            this.lbAlternatives.Name = "lbAlternatives";
            this.lbAlternatives.Size = new System.Drawing.Size(22, 121);
            this.lbAlternatives.TabIndex = 17;
            this.ToolTip1.SetToolTip(this.lbAlternatives, "Indiquer la difficulté du quiz (1 seule alternative, 2 alternatives, ... 9 altern" +
        "atives)");
            // 
            // lbNbQuestions
            // 
            this.lbNbQuestions.FormattingEnabled = true;
            this.lbNbQuestions.Items.AddRange(new object[] {
            "5",
            "10",
            "20",
            "30",
            "40",
            "50"});
            this.lbNbQuestions.Location = new System.Drawing.Point(18, 35);
            this.lbNbQuestions.Name = "lbNbQuestions";
            this.lbNbQuestions.Size = new System.Drawing.Size(25, 82);
            this.lbNbQuestions.TabIndex = 16;
            this.ToolTip1.SetToolTip(this.lbNbQuestions, "Indiquer le nombre de questions du quiz");
            // 
            // chkOrigineGrecoLatin
            // 
            this.chkOrigineGrecoLatin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkOrigineGrecoLatin.AutoSize = true;
            this.chkOrigineGrecoLatin.Location = new System.Drawing.Point(497, 211);
            this.chkOrigineGrecoLatin.Name = "chkOrigineGrecoLatin";
            this.chkOrigineGrecoLatin.Size = new System.Drawing.Size(77, 17);
            this.chkOrigineGrecoLatin.TabIndex = 32;
            this.chkOrigineGrecoLatin.Text = "Gréco-latin";
            this.ToolTip1.SetToolTip(this.chkOrigineGrecoLatin, "Cocher pour ne sélectionner que les préfixes et suffixes d\'origine gréco-latine");
            this.chkOrigineGrecoLatin.UseVisualStyleBackColor = true;
            this.chkOrigineGrecoLatin.Click += new System.EventHandler(this.chkOrigineGrecoLatin_Click);
            // 
            // lbFreq
            // 
            this.lbFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbFreq.FormattingEnabled = true;
            this.lbFreq.Items.AddRange(new object[] {
            "Fréq.",
            "Moy.",
            "Rare",
            "Abs."});
            this.lbFreq.Location = new System.Drawing.Point(58, 280);
            this.lbFreq.Name = "lbFreq";
            this.lbFreq.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFreq.Size = new System.Drawing.Size(37, 56);
            this.lbFreq.TabIndex = 33;
            this.ToolTip1.SetToolTip(this.lbFreq, "Indiquer la fréquence du segment dans les mots existants (multisélection possible" +
        ")");
            this.lbFreq.SelectedValueChanged += new System.EventHandler(this.lbFreq_SelectedValueChanged);
            // 
            // chkOrigineNeologisme
            // 
            this.chkOrigineNeologisme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkOrigineNeologisme.AutoSize = true;
            this.chkOrigineNeologisme.Location = new System.Drawing.Point(599, 211);
            this.chkOrigineNeologisme.Name = "chkOrigineNeologisme";
            this.chkOrigineNeologisme.Size = new System.Drawing.Size(81, 17);
            this.chkOrigineNeologisme.TabIndex = 36;
            this.chkOrigineNeologisme.Text = "Néologisme";
            this.ToolTip1.SetToolTip(this.chkOrigineNeologisme, "Cocher pour inclure des néologismes amusants dans les préfixes et suffixes");
            this.chkOrigineNeologisme.UseVisualStyleBackColor = true;
            this.chkOrigineNeologisme.Click += new System.EventHandler(this.chkOrigineNeologisme_Click);
            // 
            // cmdValider
            // 
            this.cmdValider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdValider.Enabled = false;
            this.cmdValider.Location = new System.Drawing.Point(366, 204);
            this.cmdValider.Name = "cmdValider";
            this.cmdValider.Size = new System.Drawing.Size(75, 29);
            this.cmdValider.TabIndex = 26;
            this.cmdValider.Text = "Valider";
            this.cmdValider.UseVisualStyleBackColor = true;
            this.cmdValider.Click += new System.EventHandler(this.cmdValider_Click);
            // 
            // lblDefSuffixe
            // 
            this.lblDefSuffixe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDefSuffixe.AutoSize = true;
            this.lblDefSuffixe.Location = new System.Drawing.Point(420, 249);
            this.lblDefSuffixe.Name = "lblDefSuffixe";
            this.lblDefSuffixe.Size = new System.Drawing.Size(45, 13);
            this.lblDefSuffixe.TabIndex = 24;
            this.lblDefSuffixe.Text = "Préfixe :";
            // 
            // lblDefPrefixe
            // 
            this.lblDefPrefixe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDefPrefixe.AutoSize = true;
            this.lblDefPrefixe.Location = new System.Drawing.Point(121, 249);
            this.lblDefPrefixe.Name = "lblDefPrefixe";
            this.lblDefPrefixe.Size = new System.Drawing.Size(45, 13);
            this.lblDefPrefixe.TabIndex = 23;
            this.lblDefPrefixe.Text = "Suffixe :";
            // 
            // lblAlternatives
            // 
            this.lblAlternatives.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAlternatives.AutoSize = true;
            this.lblAlternatives.Location = new System.Drawing.Point(18, 251);
            this.lblAlternatives.Name = "lblAlternatives";
            this.lblAlternatives.Size = new System.Drawing.Size(29, 13);
            this.lblAlternatives.TabIndex = 22;
            this.lblAlternatives.Text = "Diff.:";
            // 
            // lblNbQuestions
            // 
            this.lblNbQuestions.AutoSize = true;
            this.lblNbQuestions.Location = new System.Drawing.Point(18, 19);
            this.lblNbQuestions.Name = "lblNbQuestions";
            this.lblNbQuestions.Size = new System.Drawing.Size(78, 13);
            this.lblNbQuestions.TabIndex = 21;
            this.lblNbQuestions.Text = "Nb. questions :";
            // 
            // lblFreq
            // 
            this.lblFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFreq.AutoSize = true;
            this.lblFreq.Location = new System.Drawing.Point(55, 251);
            this.lblFreq.Name = "lblFreq";
            this.lblFreq.Size = new System.Drawing.Size(34, 13);
            this.lblFreq.TabIndex = 34;
            this.lblFreq.Text = "Fréq.:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelBarreMsg});
            this.statusStrip1.Location = new System.Drawing.Point(0, 445);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(744, 22);
            this.statusStrip1.TabIndex = 35;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelBarreMsg
            // 
            this.toolStripStatusLabelBarreMsg.Name = "toolStripStatusLabelBarreMsg";
            this.toolStripStatusLabelBarreMsg.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabelBarreMsg.Text = "toolStripStatusLabel1";
            // 
            // frmQuiz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 467);
            this.Controls.Add(this.chkOrigineNeologisme);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lblFreq);
            this.Controls.Add(this.lbFreq);
            this.Controls.Add(this.chkOrigineGrecoLatin);
            this.Controls.Add(this.lblNiv);
            this.Controls.Add(this.lbNiveau);
            this.Controls.Add(this.chkInversion);
            this.Controls.Add(this.cmdCopier);
            this.Controls.Add(this.chkMotsExistants);
            this.Controls.Add(this.cmdValider);
            this.Controls.Add(this.lbResultats);
            this.Controls.Add(this.lblDefSuffixe);
            this.Controls.Add(this.lblDefPrefixe);
            this.Controls.Add(this.lblAlternatives);
            this.Controls.Add(this.lblNbQuestions);
            this.Controls.Add(this.cmdQuiz);
            this.Controls.Add(this.lbPrefixesPossibles);
            this.Controls.Add(this.lbSuffixesPossibles);
            this.Controls.Add(this.lbAlternatives);
            this.Controls.Add(this.lbNbQuestions);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmQuiz";
            this.Text = "Quiz Logotron : dévinez le sens des mots et néologismes (ou l\'inverse)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmQuiz_FormClosing);
            this.Load += new System.EventHandler(this.frmQuiz_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label lblNiv;
        internal System.Windows.Forms.ListBox lbNiveau;
        internal System.Windows.Forms.ToolTip ToolTip1;
        internal System.Windows.Forms.CheckBox chkInversion;
        internal System.Windows.Forms.Button cmdCopier;
        internal System.Windows.Forms.CheckBox chkMotsExistants;
        internal System.Windows.Forms.ListBox lbResultats;
        internal System.Windows.Forms.Button cmdQuiz;
        internal System.Windows.Forms.ListBox lbPrefixesPossibles;
        internal System.Windows.Forms.ListBox lbSuffixesPossibles;
        internal System.Windows.Forms.ListBox lbAlternatives;
        internal System.Windows.Forms.ListBox lbNbQuestions;
        internal System.Windows.Forms.Button cmdValider;
        internal System.Windows.Forms.Label lblDefSuffixe;
        internal System.Windows.Forms.Label lblDefPrefixe;
        internal System.Windows.Forms.Label lblAlternatives;
        internal System.Windows.Forms.Label lblNbQuestions;
        internal System.Windows.Forms.CheckBox chkOrigineGrecoLatin;
        internal System.Windows.Forms.ListBox lbFreq;
        internal System.Windows.Forms.Label lblFreq;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelBarreMsg;
        internal System.Windows.Forms.CheckBox chkOrigineNeologisme;
    }
}