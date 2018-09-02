<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDicoLogotron
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDicoLogotron))
        Me.cmdAnnuler = New System.Windows.Forms.Button()
        Me.cmdDico = New System.Windows.Forms.Button()
        Me.lbResultats = New System.Windows.Forms.ListBox()
        Me.cmdDicoFusion = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.cmdFreq = New System.Windows.Forms.Button()
        Me.cmdCopier = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cmdAnnuler
        '
        Me.cmdAnnuler.Enabled = False
        Me.cmdAnnuler.Location = New System.Drawing.Point(119, 16)
        Me.cmdAnnuler.Name = "cmdAnnuler"
        Me.cmdAnnuler.Size = New System.Drawing.Size(68, 36)
        Me.cmdAnnuler.TabIndex = 6
        Me.cmdAnnuler.Text = "Annuler"
        Me.cmdAnnuler.UseVisualStyleBackColor = True
        '
        'cmdDico
        '
        Me.cmdDico.Location = New System.Drawing.Point(23, 16)
        Me.cmdDico.Name = "cmdDico"
        Me.cmdDico.Size = New System.Drawing.Size(68, 36)
        Me.cmdDico.TabIndex = 5
        Me.cmdDico.Text = "Dico"
        Me.cmdDico.UseVisualStyleBackColor = True
        '
        'lbResultats
        '
        Me.lbResultats.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbResultats.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbResultats.FormattingEnabled = True
        Me.lbResultats.ItemHeight = 16
        Me.lbResultats.Location = New System.Drawing.Point(18, 69)
        Me.lbResultats.Name = "lbResultats"
        Me.lbResultats.Size = New System.Drawing.Size(459, 180)
        Me.lbResultats.TabIndex = 7
        '
        'cmdDicoFusion
        '
        Me.cmdDicoFusion.Location = New System.Drawing.Point(214, 16)
        Me.cmdDicoFusion.Name = "cmdDicoFusion"
        Me.cmdDicoFusion.Size = New System.Drawing.Size(68, 36)
        Me.cmdDicoFusion.TabIndex = 8
        Me.cmdDicoFusion.Text = "Fusion"
        Me.ToolTip1.SetToolTip(Me.cmdDicoFusion, "Opération diverse sur le dictionnaire (vérification, fusion, nettoyage, ...)")
        Me.cmdDicoFusion.UseVisualStyleBackColor = True
        '
        'cmdFreq
        '
        Me.cmdFreq.Location = New System.Drawing.Point(309, 16)
        Me.cmdFreq.Name = "cmdFreq"
        Me.cmdFreq.Size = New System.Drawing.Size(68, 36)
        Me.cmdFreq.TabIndex = 9
        Me.cmdFreq.Text = "Fréq."
        Me.ToolTip1.SetToolTip(Me.cmdFreq, "Analyse de la fréquence des préfixes et suffixes dans les mots du dictionnaire")
        Me.cmdFreq.UseVisualStyleBackColor = True
        '
        'cmdCopier
        '
        Me.cmdCopier.Location = New System.Drawing.Point(400, 16)
        Me.cmdCopier.Name = "cmdCopier"
        Me.cmdCopier.Size = New System.Drawing.Size(68, 36)
        Me.cmdCopier.TabIndex = 10
        Me.cmdCopier.Text = "Copier"
        Me.ToolTip1.SetToolTip(Me.cmdCopier, "Copier le résultat dans le presse-papier de Windows")
        Me.cmdCopier.UseVisualStyleBackColor = True
        '
        'frmDicoLogotron
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(489, 261)
        Me.Controls.Add(Me.cmdCopier)
        Me.Controls.Add(Me.cmdFreq)
        Me.Controls.Add(Me.cmdDicoFusion)
        Me.Controls.Add(Me.lbResultats)
        Me.Controls.Add(Me.cmdAnnuler)
        Me.Controls.Add(Me.cmdDico)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmDicoLogotron"
        Me.Text = "Dictionnaire Logotron"
        Me.ResumeLayout(False)

End Sub
    Friend WithEvents cmdAnnuler As System.Windows.Forms.Button
    Friend WithEvents cmdDico As System.Windows.Forms.Button
    Friend WithEvents lbResultats As System.Windows.Forms.ListBox
    Friend WithEvents cmdDicoFusion As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents cmdFreq As System.Windows.Forms.Button
    Friend WithEvents cmdCopier As System.Windows.Forms.Button

End Class
