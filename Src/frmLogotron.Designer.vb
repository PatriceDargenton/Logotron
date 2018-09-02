<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLogotron
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        'Cet appel est requis par le Concepteur Windows Form.
        InitializeComponent()

        ' Positionnement de la fenêtre par le code : mode manuel
        Me.StartPosition = FormStartPosition.Manual
        If bDebug Then Me.StartPosition = FormStartPosition.CenterScreen

    End Sub

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmLogotron))
        Me.cmdGo = New System.Windows.Forms.Button()
        Me.lbResultats = New System.Windows.Forms.ListBox()
        Me.cmdCopier = New System.Windows.Forms.Button()
        Me.cmdAnnuler = New System.Windows.Forms.Button()
        Me.cmdQuiz = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.lbNbPrefixes = New System.Windows.Forms.ListBox()
        Me.lbNiveau = New System.Windows.Forms.ListBox()
        Me.chkOrigineGrecoLatin = New System.Windows.Forms.CheckBox()
        Me.lbFreq = New System.Windows.Forms.ListBox()
        Me.cmdAvert = New System.Windows.Forms.Button()
        Me.chkOrigineNeoRigolo = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'cmdGo
        '
        Me.cmdGo.Location = New System.Drawing.Point(21, 22)
        Me.cmdGo.Name = "cmdGo"
        Me.cmdGo.Size = New System.Drawing.Size(68, 36)
        Me.cmdGo.TabIndex = 0
        Me.cmdGo.Text = "Générer"
        Me.ToolTip1.SetToolTip(Me.cmdGo, "Générer un mot à partir d'un ou plusieurs préfixes et d'un suffixe")
        Me.cmdGo.UseVisualStyleBackColor = True
        '
        'lbResultats
        '
        Me.lbResultats.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbResultats.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbResultats.FormattingEnabled = True
        Me.lbResultats.ItemHeight = 16
        Me.lbResultats.Location = New System.Drawing.Point(80, 75)
        Me.lbResultats.Name = "lbResultats"
        Me.lbResultats.Size = New System.Drawing.Size(612, 212)
        Me.lbResultats.TabIndex = 1
        '
        'cmdCopier
        '
        Me.cmdCopier.Location = New System.Drawing.Point(109, 22)
        Me.cmdCopier.Name = "cmdCopier"
        Me.cmdCopier.Size = New System.Drawing.Size(68, 36)
        Me.cmdCopier.TabIndex = 2
        Me.cmdCopier.Text = "Copier"
        Me.ToolTip1.SetToolTip(Me.cmdCopier, "Copier le résultat dans le presse-papier de Windows")
        Me.cmdCopier.UseVisualStyleBackColor = True
        '
        'cmdAnnuler
        '
        Me.cmdAnnuler.Enabled = False
        Me.cmdAnnuler.Location = New System.Drawing.Point(388, 22)
        Me.cmdAnnuler.Name = "cmdAnnuler"
        Me.cmdAnnuler.Size = New System.Drawing.Size(68, 36)
        Me.cmdAnnuler.TabIndex = 4
        Me.cmdAnnuler.Text = "Annuler"
        Me.ToolTip1.SetToolTip(Me.cmdAnnuler, "Interrompre l'opération en cours")
        Me.cmdAnnuler.UseVisualStyleBackColor = True
        '
        'cmdQuiz
        '
        Me.cmdQuiz.Location = New System.Drawing.Point(212, 22)
        Me.cmdQuiz.Name = "cmdQuiz"
        Me.cmdQuiz.Size = New System.Drawing.Size(59, 36)
        Me.cmdQuiz.TabIndex = 5
        Me.cmdQuiz.Text = "Quiz"
        Me.ToolTip1.SetToolTip(Me.cmdQuiz, "Afficher le Quiz")
        Me.cmdQuiz.UseVisualStyleBackColor = True
        '
        'lbNbPrefixes
        '
        Me.lbNbPrefixes.FormattingEnabled = True
        Me.lbNbPrefixes.Items.AddRange(New Object() {"H", "1", "2", "3", "4", "5"})
        Me.lbNbPrefixes.Location = New System.Drawing.Point(21, 75)
        Me.lbNbPrefixes.Name = "lbNbPrefixes"
        Me.lbNbPrefixes.Size = New System.Drawing.Size(23, 82)
        Me.lbNbPrefixes.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.lbNbPrefixes, "Nombre de préfixes successifs : H = Tirage au hasard")
        '
        'lbNiveau
        '
        Me.lbNiveau.FormattingEnabled = True
        Me.lbNiveau.Items.AddRange(New Object() {"1", "2", "3"})
        Me.lbNiveau.Location = New System.Drawing.Point(50, 75)
        Me.lbNiveau.Name = "lbNiveau"
        Me.lbNiveau.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lbNiveau.Size = New System.Drawing.Size(23, 43)
        Me.lbNiveau.TabIndex = 7
        Me.ToolTip1.SetToolTip(Me.lbNiveau, "Niveaux de difficulté (multisélection possible)")
        '
        'chkOrigineGrecoLatin
        '
        Me.chkOrigineGrecoLatin.AutoSize = True
        Me.chkOrigineGrecoLatin.Checked = True
        Me.chkOrigineGrecoLatin.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkOrigineGrecoLatin.Location = New System.Drawing.Point(480, 33)
        Me.chkOrigineGrecoLatin.Name = "chkOrigineGrecoLatin"
        Me.chkOrigineGrecoLatin.Size = New System.Drawing.Size(77, 17)
        Me.chkOrigineGrecoLatin.TabIndex = 8
        Me.chkOrigineGrecoLatin.Text = "Gréco-latin"
        Me.ToolTip1.SetToolTip(Me.chkOrigineGrecoLatin, "Cocher pour ne sélectionner que les préfixes et suffixes d'origine gréco-latine")
        Me.chkOrigineGrecoLatin.UseVisualStyleBackColor = True
        '
        'lbFreq
        '
        Me.lbFreq.FormattingEnabled = True
        Me.lbFreq.Items.AddRange(New Object() {"Fréq.", "Moy.", "Rare", "Abs."})
        Me.lbFreq.Location = New System.Drawing.Point(21, 175)
        Me.lbFreq.Name = "lbFreq"
        Me.lbFreq.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lbFreq.Size = New System.Drawing.Size(37, 56)
        Me.lbFreq.TabIndex = 9
        Me.ToolTip1.SetToolTip(Me.lbFreq, "Fréquence du segment dans les mots existants (multisélection possible)")
        '
        'cmdAvert
        '
        Me.cmdAvert.Location = New System.Drawing.Point(297, 22)
        Me.cmdAvert.Name = "cmdAvert"
        Me.cmdAvert.Size = New System.Drawing.Size(68, 36)
        Me.cmdAvert.TabIndex = 10
        Me.cmdAvert.Text = "Avert."
        Me.ToolTip1.SetToolTip(Me.cmdAvert, "Afficher les avertissements")
        Me.cmdAvert.UseVisualStyleBackColor = True
        '
        'chkOrigineNeoRigolo
        '
        Me.chkOrigineNeoRigolo.AutoSize = True
        Me.chkOrigineNeoRigolo.Location = New System.Drawing.Point(563, 33)
        Me.chkOrigineNeoRigolo.Name = "chkOrigineNeoRigolo"
        Me.chkOrigineNeoRigolo.Size = New System.Drawing.Size(81, 17)
        Me.chkOrigineNeoRigolo.TabIndex = 11
        Me.chkOrigineNeoRigolo.Text = "Néologisme"
        Me.ToolTip1.SetToolTip(Me.chkOrigineNeoRigolo, "Cocher pour inclure des néologismes amusants dans les préfixes et suffixes")
        Me.chkOrigineNeoRigolo.UseVisualStyleBackColor = True
        '
        'frmLogotron
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(713, 306)
        Me.Controls.Add(Me.chkOrigineNeoRigolo)
        Me.Controls.Add(Me.cmdAvert)
        Me.Controls.Add(Me.lbFreq)
        Me.Controls.Add(Me.chkOrigineGrecoLatin)
        Me.Controls.Add(Me.lbNiveau)
        Me.Controls.Add(Me.lbNbPrefixes)
        Me.Controls.Add(Me.cmdQuiz)
        Me.Controls.Add(Me.cmdAnnuler)
        Me.Controls.Add(Me.cmdCopier)
        Me.Controls.Add(Me.lbResultats)
        Me.Controls.Add(Me.cmdGo)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmLogotron"
        Me.Text = "Logotron"
        Me.ResumeLayout(False)
        Me.PerformLayout()

End Sub
    Friend WithEvents cmdGo As System.Windows.Forms.Button
    Friend WithEvents lbResultats As System.Windows.Forms.ListBox
    Friend WithEvents cmdCopier As System.Windows.Forms.Button
    Friend WithEvents cmdAnnuler As System.Windows.Forms.Button
    Friend WithEvents cmdQuiz As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents lbNbPrefixes As System.Windows.Forms.ListBox
    Friend WithEvents lbNiveau As System.Windows.Forms.ListBox
    Friend WithEvents chkOrigineGrecoLatin As System.Windows.Forms.CheckBox
    Friend WithEvents lbFreq As System.Windows.Forms.ListBox
    Friend WithEvents cmdAvert As System.Windows.Forms.Button
    Friend WithEvents chkOrigineNeoRigolo As System.Windows.Forms.CheckBox

End Class
