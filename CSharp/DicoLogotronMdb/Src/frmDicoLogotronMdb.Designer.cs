namespace DicoLogotronMdb
{
    partial class frmDicoLogotronMdb
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
            this.cmdMdb = new System.Windows.Forms.Button();
            this.chkBaseVide = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelBarreMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmdAnnuler = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdMdb
            // 
            this.cmdMdb.Location = new System.Drawing.Point(25, 23);
            this.cmdMdb.Name = "cmdMdb";
            this.cmdMdb.Size = new System.Drawing.Size(99, 47);
            this.cmdMdb.TabIndex = 0;
            this.cmdMdb.Text = "Créer la base Logotron.mdb";
            this.cmdMdb.UseVisualStyleBackColor = true;
            this.cmdMdb.Click += new System.EventHandler(this.cmdMdb_Click);
            // 
            // chkBaseVide
            // 
            this.chkBaseVide.AutoSize = true;
            this.chkBaseVide.Checked = true;
            this.chkBaseVide.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBaseVide.Location = new System.Drawing.Point(145, 39);
            this.chkBaseVide.Name = "chkBaseVide";
            this.chkBaseVide.Size = new System.Drawing.Size(73, 17);
            this.chkBaseVide.TabIndex = 1;
            this.chkBaseVide.Text = "Base vide";
            this.chkBaseVide.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelBarreMsg});
            this.statusStrip1.Location = new System.Drawing.Point(0, 185);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(491, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelBarreMsg
            // 
            this.toolStripStatusLabelBarreMsg.Name = "toolStripStatusLabelBarreMsg";
            this.toolStripStatusLabelBarreMsg.Size = new System.Drawing.Size(105, 17);
            this.toolStripStatusLabelBarreMsg.Text = "DicoLogotronMdb";
            // 
            // cmdAnnuler
            // 
            this.cmdAnnuler.Enabled = false;
            this.cmdAnnuler.Location = new System.Drawing.Point(236, 23);
            this.cmdAnnuler.Name = "cmdAnnuler";
            this.cmdAnnuler.Size = new System.Drawing.Size(99, 47);
            this.cmdAnnuler.TabIndex = 3;
            this.cmdAnnuler.Text = "Annuler";
            this.cmdAnnuler.UseVisualStyleBackColor = true;
            this.cmdAnnuler.Click += new System.EventHandler(this.cmdAnnuler_Click);
            // 
            // frmDicoLogotronMdb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 207);
            this.Controls.Add(this.cmdAnnuler);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.chkBaseVide);
            this.Controls.Add(this.cmdMdb);
            this.Name = "frmDicoLogotronMdb";
            this.Text = "DicoLogotronMdb en C#";
            this.Load += new System.EventHandler(this.frmDicoLogotronMdb_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdMdb;
        private System.Windows.Forms.CheckBox chkBaseVide;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelBarreMsg;
        private System.Windows.Forms.Button cmdAnnuler;
    }
}

