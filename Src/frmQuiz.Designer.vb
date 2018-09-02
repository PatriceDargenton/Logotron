<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmQuiz
    Inherits System.Windows.Forms.Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmQuiz))
        Me.lbNbQuestions = New System.Windows.Forms.ListBox()
        Me.lbAlternatives = New System.Windows.Forms.ListBox()
        Me.lbSuffixesPossibles = New System.Windows.Forms.ListBox()
        Me.lbPrefixesPossibles = New System.Windows.Forms.ListBox()
        Me.cmdQuiz = New System.Windows.Forms.Button()
        Me.lblNbQuestions = New System.Windows.Forms.Label()
        Me.lblAlternatives = New System.Windows.Forms.Label()
        Me.lblDefPrefixe = New System.Windows.Forms.Label()
        Me.lblDefSuffixe = New System.Windows.Forms.Label()
        Me.lbResultats = New System.Windows.Forms.ListBox()
        Me.cmdValider = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.cmdCopier = New System.Windows.Forms.Button()
        Me.chkInversion = New System.Windows.Forms.CheckBox()
        Me.lbNiveau = New System.Windows.Forms.ListBox()
        Me.chkMotsExistants = New System.Windows.Forms.CheckBox()
        Me.chkOrigineGrecoLatin = New System.Windows.Forms.CheckBox()
        Me.lbFreq = New System.Windows.Forms.ListBox()
        Me.chkOrigineNeoRigolo = New System.Windows.Forms.CheckBox()
        Me.lblNiv = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabelBarreMsg = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblFreq = New System.Windows.Forms.Label()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lbNbQuestions
        '
        Me.lbNbQuestions.FormattingEnabled = True
        Me.lbNbQuestions.Items.AddRange(New Object() {"5", "10", "20", "30", "40", "50"})
        Me.lbNbQuestions.Location = New System.Drawing.Point(12, 36)
        Me.lbNbQuestions.Name = "lbNbQuestions"
        Me.lbNbQuestions.Size = New System.Drawing.Size(25, 82)
        Me.lbNbQuestions.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.lbNbQuestions, "Indiquer le nombre de questions du quiz")
        '
        'lbAlternatives
        '
        Me.lbAlternatives.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lbAlternatives.FormattingEnabled = True
        Me.lbAlternatives.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.lbAlternatives.Location = New System.Drawing.Point(15, 281)
        Me.lbAlternatives.Name = "lbAlternatives"
        Me.lbAlternatives.Size = New System.Drawing.Size(22, 121)
        Me.lbAlternatives.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lbAlternatives, "Indiquer la difficulté du quiz (1 seule alternative, 2 alternatives, ... 9 altern" & _
        "atives)")
        '
        'lbSuffixesPossibles
        '
        Me.lbSuffixesPossibles.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lbSuffixesPossibles.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbSuffixesPossibles.FormattingEnabled = True
        Me.lbSuffixesPossibles.ItemHeight = 16
        Me.lbSuffixesPossibles.Location = New System.Drawing.Point(108, 266)
        Me.lbSuffixesPossibles.Name = "lbSuffixesPossibles"
        Me.lbSuffixesPossibles.Size = New System.Drawing.Size(270, 164)
        Me.lbSuffixesPossibles.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.lbSuffixesPossibles, "Choisir le sens du suffixe parmi la liste")
        '
        'lbPrefixesPossibles
        '
        Me.lbPrefixesPossibles.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lbPrefixesPossibles.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbPrefixesPossibles.FormattingEnabled = True
        Me.lbPrefixesPossibles.ItemHeight = 16
        Me.lbPrefixesPossibles.Location = New System.Drawing.Point(407, 266)
        Me.lbPrefixesPossibles.Name = "lbPrefixesPossibles"
        Me.lbPrefixesPossibles.Size = New System.Drawing.Size(270, 164)
        Me.lbPrefixesPossibles.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.lbPrefixesPossibles, "Choisir le sens du préfixe parmi la liste")
        '
        'cmdQuiz
        '
        Me.cmdQuiz.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdQuiz.Location = New System.Drawing.Point(52, 205)
        Me.cmdQuiz.Name = "cmdQuiz"
        Me.cmdQuiz.Size = New System.Drawing.Size(51, 29)
        Me.cmdQuiz.TabIndex = 4
        Me.cmdQuiz.Text = "Quiz"
        Me.ToolTip1.SetToolTip(Me.cmdQuiz, "Démarrer le quiz")
        Me.cmdQuiz.UseVisualStyleBackColor = True
        '
        'lblNbQuestions
        '
        Me.lblNbQuestions.AutoSize = True
        Me.lblNbQuestions.Location = New System.Drawing.Point(12, 20)
        Me.lblNbQuestions.Name = "lblNbQuestions"
        Me.lblNbQuestions.Size = New System.Drawing.Size(78, 13)
        Me.lblNbQuestions.TabIndex = 5
        Me.lblNbQuestions.Text = "Nb. questions :"
        '
        'lblAlternatives
        '
        Me.lblAlternatives.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblAlternatives.AutoSize = True
        Me.lblAlternatives.Location = New System.Drawing.Point(12, 252)
        Me.lblAlternatives.Name = "lblAlternatives"
        Me.lblAlternatives.Size = New System.Drawing.Size(29, 13)
        Me.lblAlternatives.TabIndex = 6
        Me.lblAlternatives.Text = "Diff.:"
        '
        'lblDefPrefixe
        '
        Me.lblDefPrefixe.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblDefPrefixe.AutoSize = True
        Me.lblDefPrefixe.Location = New System.Drawing.Point(105, 250)
        Me.lblDefPrefixe.Name = "lblDefPrefixe"
        Me.lblDefPrefixe.Size = New System.Drawing.Size(45, 13)
        Me.lblDefPrefixe.TabIndex = 7
        Me.lblDefPrefixe.Text = "Suffixe :"
        '
        'lblDefSuffixe
        '
        Me.lblDefSuffixe.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblDefSuffixe.AutoSize = True
        Me.lblDefSuffixe.Location = New System.Drawing.Point(404, 250)
        Me.lblDefSuffixe.Name = "lblDefSuffixe"
        Me.lblDefSuffixe.Size = New System.Drawing.Size(45, 13)
        Me.lblDefSuffixe.TabIndex = 8
        Me.lblDefSuffixe.Text = "Préfixe :"
        '
        'lbResultats
        '
        Me.lbResultats.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbResultats.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbResultats.FormattingEnabled = True
        Me.lbResultats.ItemHeight = 16
        Me.lbResultats.Location = New System.Drawing.Point(108, 20)
        Me.lbResultats.Name = "lbResultats"
        Me.lbResultats.Size = New System.Drawing.Size(568, 164)
        Me.lbResultats.TabIndex = 9
        Me.ToolTip1.SetToolTip(Me.lbResultats, "Questions et résultats")
        '
        'cmdValider
        '
        Me.cmdValider.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdValider.Enabled = False
        Me.cmdValider.Location = New System.Drawing.Point(353, 205)
        Me.cmdValider.Name = "cmdValider"
        Me.cmdValider.Size = New System.Drawing.Size(75, 29)
        Me.cmdValider.TabIndex = 10
        Me.cmdValider.Text = "Valider"
        Me.cmdValider.UseVisualStyleBackColor = True
        '
        'cmdCopier
        '
        Me.cmdCopier.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdCopier.Location = New System.Drawing.Point(51, 157)
        Me.cmdCopier.Name = "cmdCopier"
        Me.cmdCopier.Size = New System.Drawing.Size(51, 29)
        Me.cmdCopier.TabIndex = 11
        Me.cmdCopier.Text = "Copier"
        Me.ToolTip1.SetToolTip(Me.cmdCopier, "Copier le résultat dans le presse-papier de Windows")
        Me.cmdCopier.UseVisualStyleBackColor = True
        '
        'chkInversion
        '
        Me.chkInversion.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkInversion.AutoSize = True
        Me.chkInversion.Location = New System.Drawing.Point(143, 212)
        Me.chkInversion.Name = "chkInversion"
        Me.chkInversion.Size = New System.Drawing.Size(61, 17)
        Me.chkInversion.TabIndex = 12
        Me.chkInversion.Text = "Inversé"
        Me.ToolTip1.SetToolTip(Me.chkInversion, "Cocher pour inverser le quiz (pour les questions suivantes) : trouver le préfixe " & _
        "et le suffixe d'une définition")
        Me.chkInversion.UseVisualStyleBackColor = True
        '
        'lbNiveau
        '
        Me.lbNiveau.FormattingEnabled = True
        Me.lbNiveau.Items.AddRange(New Object() {"1", "2", "3"})
        Me.lbNiveau.Location = New System.Drawing.Point(12, 157)
        Me.lbNiveau.Name = "lbNiveau"
        Me.lbNiveau.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lbNiveau.Size = New System.Drawing.Size(23, 43)
        Me.lbNiveau.TabIndex = 13
        Me.ToolTip1.SetToolTip(Me.lbNiveau, "Indiquer le niveau du quiz (1 : facile, 2 : moyen, 3 : difficile) (multisélection" & _
        " possible)")
        '
        'chkMotsExistants
        '
        Me.chkMotsExistants.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkMotsExistants.AutoSize = True
        Me.chkMotsExistants.Checked = True
        Me.chkMotsExistants.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkMotsExistants.Location = New System.Drawing.Point(230, 212)
        Me.chkMotsExistants.Name = "chkMotsExistants"
        Me.chkMotsExistants.Size = New System.Drawing.Size(63, 17)
        Me.chkMotsExistants.TabIndex = 15
        Me.chkMotsExistants.Text = "Existant"
        Me.ToolTip1.SetToolTip(Me.chkMotsExistants, "Cocher pour faire le quiz sur les mots existants du dictionnaire (pour les questi" & _
        "ons suivantes)")
        Me.chkMotsExistants.UseVisualStyleBackColor = True
        '
        'chkOrigineGrecoLatin
        '
        Me.chkOrigineGrecoLatin.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkOrigineGrecoLatin.AutoSize = True
        Me.chkOrigineGrecoLatin.Location = New System.Drawing.Point(469, 212)
        Me.chkOrigineGrecoLatin.Name = "chkOrigineGrecoLatin"
        Me.chkOrigineGrecoLatin.Size = New System.Drawing.Size(77, 17)
        Me.chkOrigineGrecoLatin.TabIndex = 16
        Me.chkOrigineGrecoLatin.Text = "Gréco-latin"
        Me.ToolTip1.SetToolTip(Me.chkOrigineGrecoLatin, "Cocher pour ne sélectionner que les préfixes et suffixes d'origine gréco-latine")
        Me.chkOrigineGrecoLatin.UseVisualStyleBackColor = True
        '
        'lbFreq
        '
        Me.lbFreq.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lbFreq.FormattingEnabled = True
        Me.lbFreq.Items.AddRange(New Object() {"Fréq.", "Moy.", "Rare", "Abs."})
        Me.lbFreq.Location = New System.Drawing.Point(52, 281)
        Me.lbFreq.Name = "lbFreq"
        Me.lbFreq.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lbFreq.Size = New System.Drawing.Size(37, 56)
        Me.lbFreq.TabIndex = 17
        Me.ToolTip1.SetToolTip(Me.lbFreq, "Indiquer la fréquence du segment dans les mots existants (multisélection possible" & _
        ")")
        '
        'chkOrigineNeoRigolo
        '
        Me.chkOrigineNeoRigolo.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkOrigineNeoRigolo.AutoSize = True
        Me.chkOrigineNeoRigolo.Location = New System.Drawing.Point(564, 212)
        Me.chkOrigineNeoRigolo.Name = "chkOrigineNeoRigolo"
        Me.chkOrigineNeoRigolo.Size = New System.Drawing.Size(81, 17)
        Me.chkOrigineNeoRigolo.TabIndex = 19
        Me.chkOrigineNeoRigolo.Text = "Néologisme"
        Me.ToolTip1.SetToolTip(Me.chkOrigineNeoRigolo, "Cocher pour inclure des néologismes amusants dans les préfixes et suffixes")
        Me.chkOrigineNeoRigolo.UseVisualStyleBackColor = True
        '
        'lblNiv
        '
        Me.lblNiv.AutoSize = True
        Me.lblNiv.Location = New System.Drawing.Point(12, 141)
        Me.lblNiv.Name = "lblNiv"
        Me.lblNiv.Size = New System.Drawing.Size(29, 13)
        Me.lblNiv.TabIndex = 14
        Me.lblNiv.Text = "Niv.:"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabelBarreMsg})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 445)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(701, 22)
        Me.StatusStrip1.TabIndex = 0
        '
        'ToolStripStatusLabelBarreMsg
        '
        Me.ToolStripStatusLabelBarreMsg.Name = "ToolStripStatusLabelBarreMsg"
        Me.ToolStripStatusLabelBarreMsg.Size = New System.Drawing.Size(120, 17)
        Me.ToolStripStatusLabelBarreMsg.Text = "ToolStripStatusLabel1"
        '
        'lblFreq
        '
        Me.lblFreq.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblFreq.AutoSize = True
        Me.lblFreq.Location = New System.Drawing.Point(49, 252)
        Me.lblFreq.Name = "lblFreq"
        Me.lblFreq.Size = New System.Drawing.Size(34, 13)
        Me.lblFreq.TabIndex = 18
        Me.lblFreq.Text = "Fréq.:"
        '
        'frmQuiz
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(701, 467)
        Me.Controls.Add(Me.chkOrigineNeoRigolo)
        Me.Controls.Add(Me.lblFreq)
        Me.Controls.Add(Me.lbFreq)
        Me.Controls.Add(Me.chkOrigineGrecoLatin)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.chkMotsExistants)
        Me.Controls.Add(Me.lblNiv)
        Me.Controls.Add(Me.lbNiveau)
        Me.Controls.Add(Me.chkInversion)
        Me.Controls.Add(Me.cmdCopier)
        Me.Controls.Add(Me.cmdValider)
        Me.Controls.Add(Me.lbResultats)
        Me.Controls.Add(Me.lblDefSuffixe)
        Me.Controls.Add(Me.lblDefPrefixe)
        Me.Controls.Add(Me.lblAlternatives)
        Me.Controls.Add(Me.lblNbQuestions)
        Me.Controls.Add(Me.cmdQuiz)
        Me.Controls.Add(Me.lbPrefixesPossibles)
        Me.Controls.Add(Me.lbSuffixesPossibles)
        Me.Controls.Add(Me.lbAlternatives)
        Me.Controls.Add(Me.lbNbQuestions)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmQuiz"
        Me.Text = "Quiz Logotron : dévinez le sens des néologismes (ou l'inverse)"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

End Sub
    Friend WithEvents lbNbQuestions As System.Windows.Forms.ListBox
    Friend WithEvents lbAlternatives As System.Windows.Forms.ListBox
    Friend WithEvents lbSuffixesPossibles As System.Windows.Forms.ListBox
    Friend WithEvents lbPrefixesPossibles As System.Windows.Forms.ListBox
    Friend WithEvents cmdQuiz As System.Windows.Forms.Button
    Friend WithEvents lblNbQuestions As System.Windows.Forms.Label
    Friend WithEvents lblAlternatives As System.Windows.Forms.Label
    Friend WithEvents lblDefPrefixe As System.Windows.Forms.Label
    Friend WithEvents lblDefSuffixe As System.Windows.Forms.Label
    Friend WithEvents lbResultats As System.Windows.Forms.ListBox
    Friend WithEvents cmdValider As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents cmdCopier As System.Windows.Forms.Button
    Friend WithEvents chkInversion As System.Windows.Forms.CheckBox
    Friend WithEvents lbNiveau As System.Windows.Forms.ListBox
    Friend WithEvents lblNiv As System.Windows.Forms.Label
    Friend WithEvents chkMotsExistants As System.Windows.Forms.CheckBox
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabelBarreMsg As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents chkOrigineGrecoLatin As System.Windows.Forms.CheckBox
    Friend WithEvents lbFreq As System.Windows.Forms.ListBox
    Friend WithEvents lblFreq As System.Windows.Forms.Label
    Friend WithEvents chkOrigineNeoRigolo As System.Windows.Forms.CheckBox
End Class
