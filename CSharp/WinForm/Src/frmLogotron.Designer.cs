namespace Logotron
{
    partial class frmLogotron
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogotron));
            this.cmdGo = new System.Windows.Forms.Button();
            this.lbResultats = new System.Windows.Forms.ListBox();
            this.lbNbPrefixes = new System.Windows.Forms.ListBox();
            this.lbNiveau = new System.Windows.Forms.ListBox();
            this.cmdCopier = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkOrigineGrecoLatin = new System.Windows.Forms.CheckBox();
            this.lbFreq = new System.Windows.Forms.ListBox();
            this.chkOrigineNeologisme = new System.Windows.Forms.CheckBox();
            this.cmdQuiz = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdGo
            // 
            this.cmdGo.Location = new System.Drawing.Point(12, 12);
            this.cmdGo.Name = "cmdGo";
            this.cmdGo.Size = new System.Drawing.Size(60, 36);
            this.cmdGo.TabIndex = 0;
            this.cmdGo.Text = "Générer";
            this.cmdGo.UseVisualStyleBackColor = true;
            this.cmdGo.Click += new System.EventHandler(this.cmdGo_Click);
            // 
            // lbResultats
            // 
            this.lbResultats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbResultats.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbResultats.FormattingEnabled = true;
            this.lbResultats.ItemHeight = 16;
            this.lbResultats.Location = new System.Drawing.Point(70, 64);
            this.lbResultats.Name = "lbResultats";
            this.lbResultats.Size = new System.Drawing.Size(479, 180);
            this.lbResultats.TabIndex = 1;
            // 
            // lbNbPrefixes
            // 
            this.lbNbPrefixes.FormattingEnabled = true;
            this.lbNbPrefixes.Items.AddRange(new object[] {
            "H",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.lbNbPrefixes.Location = new System.Drawing.Point(12, 64);
            this.lbNbPrefixes.Name = "lbNbPrefixes";
            this.lbNbPrefixes.Size = new System.Drawing.Size(23, 82);
            this.lbNbPrefixes.TabIndex = 7;
            this.toolTip1.SetToolTip(this.lbNbPrefixes, "Nombre de préfixes successifs : H = Tirage au hasard");
            this.lbNbPrefixes.SelectedValueChanged += new System.EventHandler(this.lbNbPrefixes_SelectedValueChanged);
            // 
            // lbNiveau
            // 
            this.lbNiveau.FormattingEnabled = true;
            this.lbNiveau.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.lbNiveau.Location = new System.Drawing.Point(41, 64);
            this.lbNiveau.Name = "lbNiveau";
            this.lbNiveau.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbNiveau.Size = new System.Drawing.Size(23, 43);
            this.lbNiveau.TabIndex = 8;
            this.toolTip1.SetToolTip(this.lbNiveau, "Niveaux de difficulté (multisélection possible)");
            this.lbNiveau.SelectedValueChanged += new System.EventHandler(this.lbNiveau_SelectedValueChanged);
            // 
            // cmdCopier
            // 
            this.cmdCopier.Location = new System.Drawing.Point(88, 12);
            this.cmdCopier.Name = "cmdCopier";
            this.cmdCopier.Size = new System.Drawing.Size(68, 36);
            this.cmdCopier.TabIndex = 9;
            this.cmdCopier.Text = "Copier";
            this.cmdCopier.UseVisualStyleBackColor = true;
            this.cmdCopier.Click += new System.EventHandler(this.cmdCopier_Click);
            // 
            // chkOrigineGrecoLatin
            // 
            this.chkOrigineGrecoLatin.AutoSize = true;
            this.chkOrigineGrecoLatin.Checked = true;
            this.chkOrigineGrecoLatin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOrigineGrecoLatin.Location = new System.Drawing.Point(362, 23);
            this.chkOrigineGrecoLatin.Name = "chkOrigineGrecoLatin";
            this.chkOrigineGrecoLatin.Size = new System.Drawing.Size(77, 17);
            this.chkOrigineGrecoLatin.TabIndex = 11;
            this.chkOrigineGrecoLatin.Text = "Gréco-latin";
            this.toolTip1.SetToolTip(this.chkOrigineGrecoLatin, "Cocher pour ne sélectionner que les préfixes et suffixes d\'origine gréco-latine");
            this.chkOrigineGrecoLatin.UseVisualStyleBackColor = true;
            this.chkOrigineGrecoLatin.Click += new System.EventHandler(this.chkOrigineGrecoLatin_Click);
            // 
            // lbFreq
            // 
            this.lbFreq.FormattingEnabled = true;
            this.lbFreq.Items.AddRange(new object[] {
            "Fréq.",
            "Moy.",
            "Rare",
            "Abs."});
            this.lbFreq.Location = new System.Drawing.Point(12, 167);
            this.lbFreq.Name = "lbFreq";
            this.lbFreq.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFreq.Size = new System.Drawing.Size(37, 56);
            this.lbFreq.TabIndex = 12;
            this.toolTip1.SetToolTip(this.lbFreq, "Fréquence du segment dans les mots existants (multisélection possible)");
            this.lbFreq.SelectedValueChanged += new System.EventHandler(this.lbFreq_SelectedValueChanged);
            // 
            // chkOrigineNeologisme
            // 
            this.chkOrigineNeologisme.AutoSize = true;
            this.chkOrigineNeologisme.Location = new System.Drawing.Point(445, 23);
            this.chkOrigineNeologisme.Name = "chkOrigineNeologisme";
            this.chkOrigineNeologisme.Size = new System.Drawing.Size(81, 17);
            this.chkOrigineNeologisme.TabIndex = 13;
            this.chkOrigineNeologisme.Text = "Néologisme";
            this.toolTip1.SetToolTip(this.chkOrigineNeologisme, "Cocher pour inclure des néologismes amusants dans les préfixes et suffixes");
            this.chkOrigineNeologisme.UseVisualStyleBackColor = true;
            this.chkOrigineNeologisme.Click += new System.EventHandler(this.chkOrigineNeoRigolo_Click);
            // 
            // cmdQuiz
            // 
            this.cmdQuiz.Location = new System.Drawing.Point(171, 12);
            this.cmdQuiz.Name = "cmdQuiz";
            this.cmdQuiz.Size = new System.Drawing.Size(68, 36);
            this.cmdQuiz.TabIndex = 10;
            this.cmdQuiz.Text = "Quiz";
            this.cmdQuiz.UseVisualStyleBackColor = true;
            this.cmdQuiz.Click += new System.EventHandler(this.cmdQuiz_Click);
            // 
            // frmLogotron
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 261);
            this.Controls.Add(this.chkOrigineNeologisme);
            this.Controls.Add(this.lbFreq);
            this.Controls.Add(this.chkOrigineGrecoLatin);
            this.Controls.Add(this.cmdQuiz);
            this.Controls.Add(this.cmdCopier);
            this.Controls.Add(this.lbNiveau);
            this.Controls.Add(this.lbNbPrefixes);
            this.Controls.Add(this.lbResultats);
            this.Controls.Add(this.cmdGo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLogotron";
            this.Text = "Logotron";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdGo;
        private System.Windows.Forms.ListBox lbResultats;
        internal System.Windows.Forms.ListBox lbNbPrefixes;
        internal System.Windows.Forms.ListBox lbNiveau;
        internal System.Windows.Forms.Button cmdCopier;
        private System.Windows.Forms.ToolTip toolTip1;
        internal System.Windows.Forms.Button cmdQuiz;
        internal System.Windows.Forms.CheckBox chkOrigineGrecoLatin;
        internal System.Windows.Forms.ListBox lbFreq;
        internal System.Windows.Forms.CheckBox chkOrigineNeologisme;
    }
}

