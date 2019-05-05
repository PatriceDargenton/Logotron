
' Logotron : jouer avec les préfixes et les suffixes de la langue française
' -------------------------------------------------------------------------
' https://github.com/PatriceDargenton/Logotron
' Documentation : Logotron.html
' http://patrice.dargenton.free.fr/CodesSources/Logotron/index.html
' http://patrice.dargenton.free.fr/CodesSources/Logotron/Logotron.vbproj.html
' Version 1.04 du 05/05/2018 : Gestion des élisions (ex.: palé(o) - onto - logie)
' Version 1.01 du 02/09/2018 : Première version
' Par Patrice Dargenton : mailto:patrice.dargenton@free.fr
' http://patrice.dargenton.free.fr/index.html
' http://patrice.dargenton.free.fr/CodesSources/index.html
' -------------------------------------------------------------------------

' D'après la source :
' https://www.jp-petit.org/Divers/LOGOTRON/logotron.HTM

' Conventions de nommage des variables :
' ------------------------------------
' b pour Boolean (booléen vrai ou faux)
' i pour Integer : % (en VB .Net, l'entier a la capacité du VB6.Long)
' l pour Long : &
' r pour nombre Réel (Single!, Double# ou Decimal : D)
' s pour String : $
' c pour Char ou Byte
' d pour Date
' u pour Unsigned (non signé : entier positif)
' a pour Array (tableau) : ()
' m_ pour variable Membre de la classe ou de la feuille (Form)
'  (mais pas pour les constantes)
' frm pour Form
' cls pour Classe
' mod pour Module
' ...
' ------------------------------------

Imports System.Text

Public Class frmLogotron

    Private m_bMajViaCode As Boolean = False

    Private WithEvents m_msgDelegue As clsMsgDelegue = New clsMsgDelegue

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim sVersion$ = " - V" & sVersionAppli & " (" & sDateVersionAppli & ")"
        Dim sDebug$ = " - Debug"
        Dim sTxt$ = Me.Text & sVersion
        If bDebug Then sTxt &= sDebug
        Me.Text = sTxt

        Initialisation(bAfficherAvert:=False)

        m_bMajViaCode = True
        Me.lbNiveau.SetSelected(0, True)
        Me.lbNiveau.SetSelected(1, True)
        Me.lbNiveau.SetSelected(2, True)
        Me.lbNbPrefixes.Text = sHasard
        Me.lbFreq.SetSelected(0, True)
        Me.lbFreq.SetSelected(1, True)
        Me.lbFreq.SetSelected(2, True)
        Me.lbFreq.SetSelected(3, True)
        m_bMajViaCode = False

        MajNbMotsLogotron()

    End Sub

    Private Sub Initialisation(bAfficherAvert As Boolean)

        EffacerMessages()
        InitBases()
        If sModeLectureMotsExistants = enumModeLectureMotExistant.sCsv Then
            ChargerMotsExistantsCsv()
        ElseIf sModeLectureMotsExistants = enumModeLectureMotExistant.sCode Then
            ChargerMotsExistantsCode(m_dicoMotsExistants)
        End If

        Dim sCheminLogotronCsv$ = Application.StartupPath & "\Logotron" & sLang & ".csv"
        Dim sCheminSensConcept$ = Application.StartupPath & "\SensConcept" & sLang & ".csv"
        InitialisationPrefixes(sCheminLogotronCsv, sModeLecture, m_msgDelegue)
        InitialisationSuffixes(sModeLecture)
        TraiterEtExporterDonnees(bAfficherAvert, m_msgDelegue, sCheminSensConcept)

    End Sub

    Private Sub cmdAvert_Click(sender As Object, e As EventArgs) Handles cmdAvert.Click
        Initialisation(bAfficherAvert:=True)
    End Sub

    Private Sub cmdAnnuler_Click(sender As Object, e As EventArgs) Handles cmdAnnuler.Click
        m_msgDelegue.m_bAnnuler = True
    End Sub

    Private Sub AfficherMessage(sender As Object, e As clsMsgEventArgs) _
        Handles m_msgDelegue.EvAfficherMessage
        AfficherTexte(e.sMessage)
    End Sub

    Private m_iIndex% = 0
    Private Sub AfficherTexte(sTxt$)
        AfficherTexteListBox(sTxt, m_iIndex, Me, Me.lbResultats)
    End Sub
    Private Sub EffacerMessages()
        lbResultats.Items.Clear()
        m_iIndex = 0
    End Sub

    Private Sub lbNiveau_SelectedValueChanged(sender As Object, e As EventArgs) _
        Handles lbNiveau.SelectedValueChanged

        If m_bMajViaCode Then Exit Sub
        MajNbMotsLogotron()

    End Sub

    Private Sub lbFreq_SelectedValueChanged(sender As Object, e As EventArgs) _
        Handles lbFreq.SelectedValueChanged

        If m_bMajViaCode Then Exit Sub
        MajNbMotsLogotron()

    End Sub

    Private Sub lbNbPrefixes_SelectedValueChanged(sender As Object, e As EventArgs) _
        Handles lbNbPrefixes.SelectedValueChanged

        If m_bMajViaCode Then Exit Sub
        MajNbMotsLogotron()

    End Sub

    Private Sub chkOrigineGrecoLatin_Click(sender As Object, e As EventArgs) _
        Handles chkOrigineGrecoLatin.Click

        If m_bMajViaCode Then Exit Sub
        If Me.chkOrigineGrecoLatin.Checked Then Me.chkOrigineNeoRigolo.Checked = False
        MajNbMotsLogotron()

    End Sub

    Private Sub chkOrigineNeoRigolo_Click(sender As Object, e As EventArgs) _
        Handles chkOrigineNeoRigolo.Click

        If m_bMajViaCode Then Exit Sub
        If Me.chkOrigineNeoRigolo.Checked Then Me.chkOrigineGrecoLatin.Checked = False
        MajNbMotsLogotron()

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

        Me.cmdCopier.Enabled = bActiver
        Me.cmdGo.Enabled = bActiver
        Me.cmdQuiz.Enabled = bActiver
        Me.cmdAvert.Enabled = bActiver

    End Sub

    Private Sub MajNbMotsLogotron()

        Dim lstNiv As New List(Of String)
        Dim sNiveaux$ = ""
        For Each obj In Me.lbNiveau.SelectedItems
            sNiveaux &= obj.ToString & " "
            lstNiv.Add(obj.ToString)
        Next
        AfficherTexte("Niveau(x) sélectionné(s) : " & sNiveaux)

        Dim lstFreq As New List(Of String)
        Dim sFreq$ = ""
        For Each obj In Me.lbFreq.SelectedItems
            Dim sFreqSelAbrege$ = obj.ToString
            Dim sFreqSelComplet$ = enumFrequenceAbrege.sConv(sFreqSelAbrege)
            sFreq &= sFreqSelAbrege & " "
            lstFreq.Add(sFreqSelComplet)
        Next
        AfficherTexte("Fréquence(s) sélectionnée(s) : " & sFreq)

        Dim bGrecoLatin = Me.chkOrigineGrecoLatin.Checked
        Dim bNeoRigolo = Me.chkOrigineNeoRigolo.Checked
        Dim iNbPrefixes = m_prefixes.iLireNbSegmentsUniques(lstNiv, lstFreq, bGrecoLatin, bNeoRigolo)
        Dim iNbSuffixes = m_suffixes.iLireNbSegmentsUniques(lstNiv, lstFreq, bGrecoLatin, bNeoRigolo)
        Dim lNbPrefixesCombi& = 0
        Dim sTxtCombi$ = ""

        Select Case Me.lbNbPrefixes.Text
            Case "H", "1"
                sTxtCombi =
                iNbPrefixes & " préfixes"
                lNbPrefixesCombi = iNbPrefixes
            Case "2"
                sTxtCombi =
                iNbPrefixes & " préfixes x " &
                (iNbPrefixes - 1) & " préfixes"
                lNbPrefixesCombi = iNbPrefixes * (iNbPrefixes - 1)
            Case "3"
                sTxtCombi =
                iNbPrefixes & " préfixes x " &
                (iNbPrefixes - 1) & " préfixes x " &
                (iNbPrefixes - 2) & " préfixes"
                lNbPrefixesCombi = iNbPrefixes * (iNbPrefixes - 1) * (iNbPrefixes - 2)
            Case "4"
                sTxtCombi =
                iNbPrefixes & " préfixes x " &
                (iNbPrefixes - 1) & " préfixes x " &
                (iNbPrefixes - 2) & " préfixes x " &
                (iNbPrefixes - 3) & " préfixes"
                lNbPrefixesCombi = CLng(iNbPrefixes) * (iNbPrefixes - 1) * (iNbPrefixes - 2) *
                (iNbPrefixes - 3)
            Case "5"
                sTxtCombi =
                iNbPrefixes & " préfixes x " &
                (iNbPrefixes - 1) & " préfixes x " &
                (iNbPrefixes - 2) & " préfixes x " &
                (iNbPrefixes - 3) & " préfixes x " &
                (iNbPrefixes - 4) & " préfixes"
                lNbPrefixesCombi = CLng(iNbPrefixes) * (iNbPrefixes - 1) * (iNbPrefixes - 2) *
                (iNbPrefixes - 3) * (iNbPrefixes - 4)
        End Select
        Dim lNbCombi& = lNbPrefixesCombi * iNbSuffixes
        Dim sNbCombi = sFormaterNumeriqueLong(lNbCombi)

        AfficherTexte(sTxtCombi & " x " & iNbSuffixes & " suffixes = " &
            sNbCombi & " combinaisons pour le Logotron")
        AfficherTexte("")

        If Not Me.chkOrigineGrecoLatin.Checked Then
            Dim lstPrefixes = m_prefixes.lstSegmentsAutreOrigine(lstNiv, lstFreq, bNeoRigolo)
            For Each prefixe In lstPrefixes
                AfficherTexte(prefixe.sAfficher(bPrefixe:=True))
            Next
            Dim lstSuffixes = m_suffixes.lstSegmentsAutreOrigine(lstNiv, lstFreq, bNeoRigolo)
            For Each suffixe In lstSuffixes
                AfficherTexte(suffixe.sAfficher(bPrefixe:=False))
            Next
        End If

    End Sub

    Private Sub cmdCopier_Click(sender As Object, e As EventArgs) Handles cmdCopier.Click

        Dim sTxt$ = sLireListBox(lbResultats)
        If bCopierPressePapier(sTxt) Then _
            MsgBox(sMsgCopiePressePapier, MsgBoxStyle.Exclamation, m_sTitreMsg)

    End Sub

    Private Sub cmdGo_Click(sender As Object, e As EventArgs) Handles cmdGo.Click

        Dim sMot$ = "", sExplication$ = "", sDetail$ = ""
        Dim lstEtym As New List(Of String)

        ' Les préfixes et suffixes des mots du dictionnaire sont plus nombreux
        '  ne prendre que ceux qui forme des mots potentiels plausibles
        Const bComplet As Boolean = False

        Dim sNbPrefixesSuccessifs$ = Me.lbNbPrefixes.Text

        Dim lstNiv As New List(Of String)
        Dim iNumNiv% = 0
        For Each obj In Me.lbNiveau.SelectedItems
            iNumNiv += 1
            lstNiv.Add(obj.ToString)
        Next
        If iNumNiv = 0 Then Exit Sub

        Dim lstFreq As New List(Of String)
        Dim iNumFreq% = 0
        For Each obj In Me.lbFreq.SelectedItems
            iNumFreq += 1
            Dim sFreqSelAbrege$ = obj.ToString
            Dim sFreqSelComplet$ = enumFrequenceAbrege.sConv(sFreqSelAbrege)
            lstFreq.Add(sFreqSelComplet)
        Next
        If iNumFreq = 0 Then Exit Sub

        Dim bGrecoLatin = Me.chkOrigineGrecoLatin.Checked
        Dim bNeoRigolo = Me.chkOrigineNeoRigolo.Checked

        If Not bTirage(bComplet, sNbPrefixesSuccessifs, lstNiv, lstFreq, bGrecoLatin,
            bNeoRigolo, sMot, sExplication, sDetail, lstEtym) Then Exit Sub

        AfficherTexte(sMot)
        AfficherTexte(sExplication)
        AfficherTexte(sDetail)
        If lstEtym.Count > 0 Then
            For Each sEtym In lstEtym
                AfficherTexte(sEtym)
            Next
        End If
        AfficherTexte("")

    End Sub

    Private Sub cmdQuiz_Click(sender As Object, e As EventArgs) Handles cmdQuiz.Click

        Activation(bActiver:=False)

        Using frmQuiz0 As New frmQuiz
            frmQuiz0.StartPosition = FormStartPosition.CenterScreen
            frmQuiz0.ShowDialog(Me)
        End Using

        Activation(bActiver:=True)

    End Sub

End Class
