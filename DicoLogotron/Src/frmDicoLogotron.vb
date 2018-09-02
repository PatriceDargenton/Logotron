
Imports System.Text

Public Class frmDicoLogotron

    Private WithEvents m_msgDelegue As clsMsgDelegue = New clsMsgDelegue

    Private Sub frmDicoLogotron_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim sVersion$ = " - V" & sVersionAppli & " (" & sDateVersionAppli & ")"
        Dim sDebug$ = " - Debug"
        Dim sTxt$ = Me.Text & sVersion
        If bDebug Then sTxt &= sDebug
        Me.Text = sTxt

        If bRelease Then Me.cmdDicoFusion.Visible = False

        InitBases()
        Dim sCheminLogotronCsv$ = sDossierParent(Application.StartupPath) & "\Logotron" & sLang & ".csv"
        InitialisationPrefixes(sCheminLogotronCsv, sModeLecture)
        InitialisationSuffixes(sModeLecture)

    End Sub

    Private Sub cmdAnnuler_Click(sender As Object, e As EventArgs) Handles cmdAnnuler.Click
        m_msgDelegue.m_bAnnuler = True
    End Sub

    Private Sub AfficherMessage(sender As Object, e As clsMsgEventArgs) _
        Handles m_msgDelegue.EvAfficherMessage
        AfficherTexte(e.sMessage)
    End Sub

    Private Sub AfficherTexte(sTxt$)
        Static iIndex% = 0
        AfficherTexteListBox(sTxt, iIndex, Me, Me.lbResultats)
    End Sub

    Private Sub Sablier(Optional bDesactiver As Boolean = False)

        ' Me.Cursor : Curseur de la fenêtre
        ' Cursor.Current : Curseur de l'application

        If bDesactiver Then
            Me.Cursor = Cursors.Default
        Else
            Me.Cursor = Cursors.WaitCursor
        End If

    End Sub

    Private Sub Activation(bActiver As Boolean)

        m_msgDelegue.m_bAnnuler = False

        Me.cmdAnnuler.Enabled = Not bActiver
        Sablier(bDesactiver:=bActiver)

        Me.cmdDico.Enabled = bActiver
        Me.cmdDicoFusion.Enabled = bActiver
        Me.cmdFreq.Enabled = bActiver
        Me.cmdCopier.Enabled = bActiver

    End Sub

    Private Sub cmdDico_Click(sender As Object, e As EventArgs) Handles cmdDico.Click

        Activation(bActiver:=False)

        InitBasesPot()
        InitialisationPrefixesEtSuffixesPot()

        AnalyseDico(m_msgDelegue)

        Activation(bActiver:=True)

    End Sub

    Private Sub cmdDicoFusion_Click(sender As Object, e As EventArgs) Handles cmdDicoFusion.Click

        Activation(bActiver:=False)

        'Dim sCheminDicoAFus$ = Application.StartupPath & _
        '    "\Doc\DicoFrLibreOfficeSansNomPropre.txt"
        Dim sCheminDicoAExclure$ = Application.StartupPath &
            "\Doc\DicoFrLibreOfficeNomPropre.txt"
        If Not bFichierExiste(sCheminDicoAExclure, bPrompt:=True) Then GoTo Fin
        Dim sCheminDico0$ = Application.StartupPath & "\Doc\" & sCheminDico

        If Not bFichierExiste(sCheminDico0, bPrompt:=True) Then GoTo Fin
        Dim asLignes$() = asLireFichier(sCheminDico0)
        If IsNothing(asLignes) Then GoTo Fin
        Dim iNumLigne% = 0
        Dim iNbLignes% = asLignes.GetUpperBound(0)

        ' Dédoublonnage, fusion, correction du dico
        Dim hsMots As New HashSet(Of String)
        Dim hsMotsMin As New HashSet(Of String)

        'Dim sb As New StringBuilder

        ' Séparation des mots avec espace
        ''Dim sb As New StringBuilder  ' Mot sans espace
        ''Dim sb2 As New StringBuilder ' Mot avec espace

        Dim sMemMot$ = ""
        For Each sMotDico As String In asLignes
            iNumLigne += 1

            'If Not sMotDico.StartsWith("st") Then Continue For
            'If sMotDico = "stéréocardiogramme" Then
            '    Debug.WriteLine("!")
            'End If
            'Debug.WriteLine(sMotDico )
            If bDebug AndAlso iNumLigne > 10000 Then Exit For

            If iNumLigne Mod 10000 = 0 OrElse iNumLigne = iNbLignes Then
                Dim rPC! = iNumLigne / iNbLignes
                Dim sPC$ = rPC.ToString(sFormatPC)
                m_msgDelegue.AfficherMsg(sPC)
                If m_msgDelegue.m_bAnnuler Then Exit For
            End If

            Dim sMotDicoL = sMotDico.ToLower
            If hsMotsMin.Contains(sMotDicoL) Then Continue For
            hsMotsMin.Add(sMotDicoL)
            hsMots.Add(sMotDico)

            ' Suppression des espaces
            ''If sMotDico.IndexOf(" ") >= 0 Then
            ''    'Debug.WriteLine(sMotDico)
            ''    sb2.AppendLine(sMotDico)
            ''    Continue For
            ''End If

            'sb.AppendLine(sMotDicoL.Trim)

        Next

        ' Ajout des mots du dico2 qui ne sont pas dans le dico principal

        'If bFichierExiste(sCheminDicoAFus) Then
        '    Dim asLignes2$() = asLireFichier(sCheminDicoAFus, bUnicodeUTF8:=True)
        '    For Each sMotDico As String In asLignes2
        '        Dim sMotTrim$ = sMotDico.Trim
        '        Dim sMotDicoL = sMotTrim.ToLower
        '        If Not hsMotsMin.Contains(sMotDicoL) Then
        '            'sb.AppendLine(sMotDicoL)
        '            hsMotsMin.Add(sMotDicoL)
        '            hsMots.Add(sMotTrim)
        '        End If
        '    Next
        'End If

        ' Exclusion des noms propres

        If bFichierExiste(sCheminDicoAExclure) Then
            Dim asLignes2$() = asLireFichier(sCheminDicoAExclure, bUnicodeUTF8:=True)
            For Each sMotDico As String In asLignes2
                Dim sMotTrim$ = sMotDico.Trim
                Dim sMotDicoL = sMotTrim.ToLower
                If hsMotsMin.Contains(sMotDicoL) Then
                    hsMotsMin.Remove(sMotDicoL)
                    hsMots.Remove(sMotTrim)
                End If
            Next
        End If

        m_msgDelegue.AfficherMsg("Tri...")
        Dim lst = hsMots.ToList
        ' Déjà triée
        lst.Sort(StringComparer.Create(New Globalization.CultureInfo("fr-FR"), ignoreCase:=True))
        Dim sb As New StringBuilder
        For Each sMot In lst
            sb.AppendLine(sMot)
        Next

        Dim sCheminDicoOut$ = Application.StartupPath & "\Doc\DicoDest.txt"
        bEcrireFichier(sCheminDicoOut, sb, bEncodageUTF8:=True)
        m_msgDelegue.AfficherMsg("Terminé.")

        'Dim sCheminDicoOut2$ = Application.StartupPath & "\Doc\DicoMotsAvecEspace.txt"
        'bEcrireFichier(sCheminDicoOut2, sb2)

Fin:
        Activation(bActiver:=True)

    End Sub

    Private Sub cmdFreq_Click(sender As Object, e As EventArgs) Handles cmdFreq.Click

        Activation(bActiver:=False)

        CalculFrequenceMots(m_msgDelegue)

Fin:
        Activation(bActiver:=True)

    End Sub

    Private Sub cmdCopier_Click(sender As Object, e As EventArgs) Handles cmdCopier.Click
        Dim sTxt$ = sLireListBox(lbResultats)
        If bCopierPressePapier(sTxt) Then _
            MsgBox(sMsgCopiePressePapier, MsgBoxStyle.Exclamation, m_sTitreMsg)
    End Sub

End Class
