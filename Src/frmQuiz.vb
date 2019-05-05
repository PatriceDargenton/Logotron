
Public Class frmQuiz

    Const bDebugUnicite = False ' Afficher l'unicité si elle existe
    Const bDebugUniciteSynth = False ' Afficher toujours l'unicité ou le sens, sinon

    Const iNbMotsExistantsMin% = 20
    Const iNbSegmentsMin% = 8

    Private m_bAnnuler As Boolean = False
    Private m_bAttendre As Boolean = False

    Private m_bMajViaCode As Boolean = False

    ' Coefficient de bonus lorsque le préfixe et le suffixe sont justes
    Private Const iCoefBonus% = 3

    Private Const sTipsValider$ = "Valider une réponse du quiz"

    Private Sub frmQuiz_FormClosing(sender As Object, e As FormClosingEventArgs) _
        Handles Me.FormClosing
        m_bAnnuler = True
        m_bAttendre = False
    End Sub

    Private Sub frmQuiz_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.ToolTip1.SetToolTip(Me.cmdValider, sTipsValider)
        m_bMajViaCode = True
        Me.lbNbQuestions.Text = "5"
        Me.lbAlternatives.Text = "1"
        Me.lbNiveau.Text = "1"
        Me.lbFreq.SetSelected(0, True)
        Me.lbFreq.SetSelected(1, True)
        Me.lbFreq.SetSelected(2, True)
        Me.lbFreq.SetSelected(3, True)
        m_bMajViaCode = False
        MajNbMotsQuiz()
        AfficherMsgBarreMsg("")

    End Sub

    Private Sub lbResultats_Click(sender As Object, e As EventArgs) Handles lbResultats.Click

        AfficherMsgBarreMsg(lbResultats.Text)

    End Sub

    Private Sub lbSuffixesPossibles_Click(sender As Object, e As EventArgs) _
        Handles lbSuffixesPossibles.Click

        AfficherMsgBarreMsg(lbSuffixesPossibles.Text)

    End Sub

    Private Sub lbPrefixesPossibles_Click(sender As Object, e As EventArgs) _
        Handles lbPrefixesPossibles.Click

        AfficherMsgBarreMsg(lbPrefixesPossibles.Text)

    End Sub

    Private Sub Activation(bActiver As Boolean, Optional bToutCtrl As Boolean = False)

        Me.cmdQuiz.Enabled = bActiver
        Me.cmdValider.Enabled = False
        Me.lbNbQuestions.Enabled = bActiver
        Me.lbAlternatives.Enabled = bActiver
        Me.lbNiveau.Enabled = bActiver

        If bToutCtrl Then
            Me.chkMotsExistants.Enabled = bActiver
            Me.chkInversion.Enabled = bActiver
        End If

    End Sub

    Private Sub lbNiveau_SelectedValueChanged(sender As Object, e As EventArgs) _
        Handles lbNiveau.SelectedValueChanged
        If m_bMajViaCode Then Exit Sub
        MajNbMotsQuiz()
    End Sub

    Private Sub lbFreq_SelectedValueChanged(sender As Object, e As EventArgs) _
        Handles lbFreq.SelectedValueChanged
        If m_bMajViaCode Then Exit Sub
        MajNbMotsQuiz()
    End Sub

    Private Sub chkMotsExistants_Click(sender As Object, e As EventArgs) Handles chkMotsExistants.Click
        If m_bMajViaCode Then Exit Sub
        ' Pour les mots existants, toutes les origines sont incluses
        ' (sinon il faudrait ajouter l'origine des préfixes et suffixes dans le fichier des mots existants)
        If Me.chkMotsExistants.Checked Then Me.chkOrigineGrecoLatin.Checked = False
        If Me.chkMotsExistants.Checked Then Me.chkOrigineNeoRigolo.Checked = False
        MajNbMotsQuiz()
    End Sub

    Private Sub chkOrigineGrecoLatin_Click(sender As Object, e As EventArgs) Handles chkOrigineGrecoLatin.Click
        If m_bMajViaCode Then Exit Sub
        ' Si sélectionne les seules origines Greco-latines,
        '  on ne peut plus se baser sur les mots existants (toutes origines)
        If Me.chkOrigineGrecoLatin.Checked Then Me.chkMotsExistants.Checked = False
        If Me.chkOrigineGrecoLatin.Checked Then Me.chkOrigineNeoRigolo.Checked = False
        MajNbMotsQuiz()
    End Sub

    Private Sub chkOrigineNeoRigolo_Click(sender As Object, e As EventArgs) _
        Handles chkOrigineNeoRigolo.Click
        If m_bMajViaCode Then Exit Sub
        If Me.chkOrigineNeoRigolo.Checked Then Me.chkOrigineGrecoLatin.Checked = False
        If Me.chkOrigineNeoRigolo.Checked Then Me.chkMotsExistants.Checked = False
        MajNbMotsQuiz()
    End Sub

    Private Sub MajNbMotsQuiz()

        'Dim sNiv$ = Me.lbNiveau.SelectedItem
        'Dim iNiv% = Integer.Parse(sNiv)
        Dim lstNiv As New List(Of String)
        'lstNiv.Add(sNiv)
        ' 08/07/2018 Plusieurs niveaux possibles
        Dim sNiveaux$ = ""
        For Each obj In Me.lbNiveau.SelectedItems
            sNiveaux &= obj.ToString & " "
            lstNiv.Add(obj.ToString)
        Next
        'AfficherTexte("Niveau sélectionné : " & sNiv)
        AfficherTexte("Niveaux sélectionnés : " & sNiveaux)

        ' 21/08/2018
        Dim lstFreq As New List(Of String)
        Dim sFreq$ = ""
        For Each obj In Me.lbFreq.SelectedItems
            Dim sFreqSelAbrege$ = obj.ToString
            Dim sFreqSelComplet$ = enumFrequenceAbrege.sConv(sFreqSelAbrege)
            sFreq &= sFreqSelAbrege & " "
            lstFreq.Add(sFreqSelComplet)
        Next
        AfficherTexte("Fréquence(s) sélectionnée(s) : " & sFreq)

        Me.cmdQuiz.Enabled = True
        If Me.chkMotsExistants.Checked Then

            Dim iNbMotsExistants0% = iNbMotsExistants(lstNiv, lstFreq)
            Dim sNbMots = sFormaterNumeriqueLong(iNbMotsExistants0)
            AfficherTexte(sNbMots & " mots existants pour le quiz")
            If iNbMotsExistants0 < iNbMotsExistantsMin Then Me.cmdQuiz.Enabled = False

            Dim iNbPE% = iNbPrefixesMotsExistants(lstNiv, lstFreq)
            Dim sNbPE = sFormaterNumeriqueLong(iNbPE)
            AfficherTexte(sNbPE & " préfixes distincts pour les mots existants")
            Dim iNbSE% = iNbSuffixesMotsExistants(lstNiv, lstFreq)
            Dim sNbSE = sFormaterNumeriqueLong(iNbSE)
            AfficherTexte(sNbSE & " suffixes distincts pour les mots existants")

            Const bDebugMots As Boolean = False
            If bDebugMots Then
                Dim i% = 0
                For Each mot In lstMotsExistants(lstNiv, lstFreq)
                    i += 1
                    AfficherTexte(i & " : " & mot.ToString())
                    If i > 50 Then
                        AfficherTexte("...")
                        Exit For
                    End If
                Next
            End If

            Const bDebugPrefixes As Boolean = False
            If bDebugPrefixes Then
                Dim i% = 0
                For Each mot In lstPrefixesMotsExistants(lstNiv, lstFreq)
                    i += 1
                    AfficherTexte(i & " : " & mot.sAfficherPrefixe())
                Next
            End If

            Const bDebugSuffixes As Boolean = False
            If bDebugSuffixes Then
                Dim i% = 0
                For Each mot In lstSuffixesMotsExistants(lstNiv, lstFreq)
                    i += 1
                    AfficherTexte(i & " : " & mot.sAfficherSuffixe())
                Next
            End If

        Else

            Dim bGrecoLatin = Me.chkOrigineGrecoLatin.Checked
            Dim bNeoRigolo = Me.chkOrigineNeoRigolo.Checked
            Dim iNbPrefixes = m_prefixes.iLireNbSegmentsUniques(lstNiv, lstFreq, bGrecoLatin, bNeoRigolo)
            Dim iNbSuffixes = m_suffixes.iLireNbSegmentsUniques(lstNiv, lstFreq, bGrecoLatin, bNeoRigolo)

            Dim lNbCombi& = iNbPrefixes * iNbSuffixes
            Dim sNbCombi = sFormaterNumeriqueLong(lNbCombi)

            AfficherTexte(iNbPrefixes & " préfixes" & " x " & iNbSuffixes & " suffixes = " & _
                sNbCombi & " combinaisons pour le quiz")
            If iNbPrefixes < iNbSegmentsMin Then Me.cmdQuiz.Enabled = False
            If iNbSuffixes < iNbSegmentsMin Then Me.cmdQuiz.Enabled = False

        End If

        AfficherTexte("")

    End Sub

    'Private Sub chkInversion_Click(sender As Object, e As EventArgs) Handles chkInversion.Click
    'End Sub

    Private Sub cmdQuiz_Click(sender As Object, e As EventArgs) Handles cmdQuiz.Click

        m_bAnnuler = False
        Activation(bActiver:=False)
        EffacerMessages()

        AfficherTexte("Préfixe juste : 1 point")
        AfficherTexte("Suffixe juste : 1 point")
        AfficherTexte("Préfixe et suffixe juste : 3 points")

        Dim iNiveau = Integer.Parse(Me.lbNiveau.Text)
        'Dim iCoefNiv% = 0 'iNiveau + 1
        ' 08/07/2018 Plusieurs niveaux possibles
        Dim lstNiv As New List(Of String)
        Dim sNiveaux$ = ""
        For Each obj In Me.lbNiveau.SelectedItems
            Dim sNiv$ = obj.ToString
            lstNiv.Add(sNiv)
            sNiveaux &= sNiv & " "
        Next
        Dim iCoefNiv = enumNiveau.iCoef(sNiveaux)

        ' 21/08/2018
        Dim lstFreq As New List(Of String)
        Dim sFreq$ = ""
        For Each obj In Me.lbFreq.SelectedItems
            Dim sFreqSelAbrege$ = obj.ToString
            Dim sFreqSelComplet$ = enumFrequenceAbrege.sConv(sFreqSelAbrege)
            sFreq &= sFreqSelAbrege & " "
            lstFreq.Add(sFreqSelComplet)
        Next
        Dim iCoefFreq = enumFrequenceAbrege.iCoef(sFreq)

        Dim iNbQuestions = Integer.Parse(Me.lbNbQuestions.Text)

        Dim iAlternatives = Integer.Parse(Me.lbAlternatives.Text)
        Dim iScoreTot% = 0
        Dim iCoefAlternatives% = iAlternatives + 1

        Dim iCoefNBQ% = iNbQuestions

        For iNumQuestion As Integer = 0 To iNbQuestions - 1

            AfficherTexte("")
            AfficherTexte("Question n°" & iNumQuestion + 1 & " / " & iNbQuestions)

            If Me.chkInversion.Checked Then
                Me.ToolTip1.SetToolTip(Me.lbSuffixesPossibles, _
                    "Choisir le suffixe parmi la liste")
                Me.ToolTip1.SetToolTip(Me.lbPrefixesPossibles, _
                    "Choisir le préfixe parmi la liste")
            Else
                Me.ToolTip1.SetToolTip(Me.lbSuffixesPossibles, _
                    "Choisir le sens du suffixe parmi la liste")
                Me.ToolTip1.SetToolTip(Me.lbPrefixesPossibles, _
                    "Choisir le sens du préfixe parmi la liste")
            End If

            Dim bErreur = False
            Dim iScore% = 0
            If Me.chkInversion.Checked Then
                If Me.chkMotsExistants.Checked Then
                    QuizSegmentMotExistant(lstNiv, lstFreq, iAlternatives, iScore, bErreur)
                Else
                    QuizSegment(m_prefixes, m_suffixes, _
                        lstNiv, lstFreq, iAlternatives, iScore, bErreur)
                End If
            Else
                If Me.chkMotsExistants.Checked Then
                    QuizDefinitionMotExistant(lstNiv, lstFreq, iAlternatives, iScore, bErreur)
                Else
                    QuizDefinition(m_prefixes, m_suffixes, _
                        lstNiv, lstFreq, iAlternatives, iScore, bErreur)
                End If
            End If
            If m_bAnnuler Then Exit Sub

            iScoreTot += iScore
            Dim sScore$ = "Résultat = " & iScoreTot & " / " & (iNumQuestion + 1) * iCoefBonus

            AfficherTexte(sScore)

            If bErreur Then
                ' Boucle d'attente pour comprendre l'erreur
                Activation(bActiver:=False, bToutCtrl:=True)
                Me.cmdValider.Text = "Poursuivre"
                Me.ToolTip1.SetToolTip(Me.cmdValider, "Poursuivre le quiz")
                Me.cmdValider.Enabled = True
                m_bAttendre = True
                While m_bAttendre
                    If m_bAnnuler Then Exit While
                    Application.DoEvents()
                End While
                Me.cmdValider.Text = "Valider"
                Me.ToolTip1.SetToolTip(Me.cmdValider, sTipsValider)
                Activation(bActiver:=True, bToutCtrl:=True)
                Activation(bActiver:=False) ' Mode normale
            End If

        Next

        'Dim sAffNiv$ = "niveau " & iNiveau
        Dim sAffNiv$ = "niveau(x) " & sNiveaux & ", "
        Dim sAffFreq$ = "fréquence(s) " & sFreq
        If sFreq = "Fréq. Moy. Rare Abs. " Then
            sAffFreq = "" ' Pas besoin d'afficher la fréquence alors
        Else
            sAffFreq &= ", "
        End If
        Dim sResultatFinal$ = "Résultat final " & sAffNiv & sAffFreq & _
            " et difficulté " & iAlternatives & " avec " & iNbQuestions & " questions = " & _
            iScoreTot & " / " & iNbQuestions * iCoefBonus
        Dim sScoreFinal$ = "Score final = " &
            iScoreTot * iCoefNiv * iCoefFreq * iCoefAlternatives * iCoefNBQ
        AfficherTexte(sResultatFinal)
        AfficherTexte(sScoreFinal)

        Activation(bActiver:=True)

    End Sub

    Private Sub QuizDefinition(basePrefixe As clsBase, baseSuffixe As clsBase, _
        lstNiv As List(Of String), lstFreq As List(Of String), iAlternatives%, _
        ByRef iScore%, ByRef bErreur As Boolean) ' iNiveau%

        ' Quiz sur la définition du préfixe et du suffixe

        iScore = 0

        Dim bGrecoLatin = Me.chkOrigineGrecoLatin.Checked
        Dim bNeoRigolo = Me.chkOrigineNeoRigolo.Checked

        ' Les préfixes et suffixes des mots du dictionnaire sont plus nombreux
        '  ne prendre que ceux qui forme des mots potentiels plausibles
        Const bComplet = False

        ' 08/07/2018 Plusieurs niveaux
        Dim iNumPrefixe% = basePrefixe.iTirageSegment(bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo)
        Dim prefixe As clsSegmentBase = Nothing
        If Not basePrefixe.bLireSegment(iNumPrefixe, prefixe) Then Exit Sub
        Dim sNiveauP = prefixe.sNiveau

        Dim iNumSuffixe% = baseSuffixe.iTirageSegment(bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo)
        Dim suffixe As clsSegmentBase = Nothing
        If Not baseSuffixe.bLireSegment(iNumSuffixe, suffixe) Then Exit Sub
        Dim sNiveauS = suffixe.sNiveau

        Dim sPrefixe = prefixe.sSegment
        Dim sSuffixe = suffixe.sSegment
        Dim sPrefixeMaj = sPrefixe.ToUpper()
        Dim sSuffixeMaj = sSuffixe.ToUpper()
        Dim sSensPrefixeMaj = prefixe.sSens.ToUpper()
        sSensPrefixeMaj = sCompleterPrefixe(sSensPrefixeMaj)
        Dim sEtymPrefixe = prefixe.sEtym
        Dim sSensSuffixeMaj = suffixe.sSens.ToUpper()
        Dim sEtymSuffixe = suffixe.sEtym

        Dim sMot$ = sPrefixeMaj & sSuffixeMaj
        Dim sDetail$ = sPrefixeMaj & "(" & sNiveauP & ")-" & sSuffixeMaj & "(" & sNiveauS & ")"
        Dim sExplication$ = sSensSuffixeMaj & " " & sSensPrefixeMaj

        Dim lstEtym = New List(Of String)
        If sEtymPrefixe.Length > 0 Then lstEtym.Add(sPrefixe & "- : " & sEtymPrefixe)
        If sEtymSuffixe.Length > 0 Then lstEtym.Add("-" & sSuffixe & " : " & sEtymSuffixe)

        Dim itPrefixe As New clsInitTirage(prefixe)
        Dim itSuffixe As New clsInitTirage(suffixe)

        If bDebugUnicite Then
            If prefixe.sUnicite.Length > 0 Then sSensPrefixeMaj &= " [" & prefixe.sUnicite & "]"
            If suffixe.sUnicite.Length > 0 Then sSensSuffixeMaj &= " [" & suffixe.sUnicite & "]"
        End If
        If bDebugUniciteSynth Then
            sSensPrefixeMaj &= " (" & prefixe.sUniciteSynth & ")"
            sSensSuffixeMaj &= " (" & suffixe.sUniciteSynth & ")"
        End If

        Dim lstExplicationsPrefixe As New List(Of String)
        lstExplicationsPrefixe.Add(sSensPrefixeMaj)
        Dim lstExplicationsSuffixe As New List(Of String)
        lstExplicationsSuffixe.Add(sSensSuffixeMaj)

        For j As Integer = 0 To iAlternatives - 1

            Dim iNumPrefixeAutre% = basePrefixe.iTirageSegment(bComplet,
                lstNiv, lstFreq, itPrefixe, bGrecoLatin, bNeoRigolo)
            Dim prefixeP2 As clsSegmentBase = Nothing
            If Not basePrefixe.bLireSegment(iNumPrefixeAutre, prefixeP2) Then Exit For
            Dim sSensPrefixeAutre = prefixeP2.sSens.ToUpper()
            sSensPrefixeAutre = sCompleterPrefixe(sSensPrefixeAutre)

            Dim iNumSuffixeAutre% = baseSuffixe.iTirageSegment(bComplet,
                lstNiv, lstFreq, itSuffixe, bGrecoLatin, bNeoRigolo)
            Dim suffixeS2 As clsSegmentBase = Nothing
            If Not baseSuffixe.bLireSegment(iNumSuffixeAutre, suffixeS2) Then Exit For
            Dim sSensSuffixeAutre = suffixeS2.sSens.ToUpper()

            If bDebugUnicite Then
                If prefixeP2.sUnicite.Length > 0 Then _
                    sSensPrefixeAutre &= " [" & prefixeP2.sUnicite & "]"
                If suffixeS2.sUnicite.Length > 0 Then _
                    sSensSuffixeAutre &= " [" & suffixeS2.sUnicite & "]"
            End If
            If bDebugUniciteSynth Then
                sSensPrefixeAutre &= " (" & prefixeP2.sUniciteSynth & ")"
                sSensSuffixeAutre &= " (" & suffixeS2.sUniciteSynth & ")"
            End If

            lstExplicationsPrefixe.Add(sSensPrefixeAutre)
            lstExplicationsSuffixe.Add(sSensSuffixeAutre)

        Next

        RemplirListBoxAuHasard(lbPrefixesPossibles, lstExplicationsPrefixe)
        RemplirListBoxAuHasard(lbSuffixesPossibles, lstExplicationsSuffixe)

        AfficherTexte(sMot)

        Me.cmdValider.Enabled = True
        While Me.cmdValider.Enabled
            If m_bAnnuler Then
                Activation(bActiver:=True)
                Exit Sub
            End If
            Application.DoEvents()
        End While

        Dim sSensPrefixeChoisi$ = Me.lbPrefixesPossibles.Text
        Dim sSensSuffixeChoisi$ = Me.lbSuffixesPossibles.Text

        Dim bPrefixeOk, bSuffixeOk As Boolean
        bPrefixeOk = False : bSuffixeOk = False
        If sSensPrefixeChoisi = sSensPrefixeMaj AndAlso _
            sSensSuffixeChoisi = sSensSuffixeMaj Then
            iScore += iCoefBonus : bPrefixeOk = True : bSuffixeOk = True
        ElseIf sSensPrefixeChoisi = sSensPrefixeMaj Then
            iScore += 1 : bPrefixeOk = True
        ElseIf sSensSuffixeChoisi = sSensSuffixeMaj Then
            iScore += 1 : bSuffixeOk = True
        End If

        Dim sAffPrefixe = sSensPrefixeChoisi & " : Faux ! "
        Dim sAffSuffixe = sSensSuffixeChoisi & " : Faux ! "
        If sSensPrefixeChoisi.Length = 0 Then sAffPrefixe = ""
        If sSensSuffixeChoisi.Length = 0 Then sAffSuffixe = ""
        AfficherTexte(sDetail)
        If lstEtym.Count > 0 Then
            For Each sEtym In lstEtym
                AfficherTexte(sEtym)
            Next
        End If
        bErreur = True
        If bPrefixeOk AndAlso bSuffixeOk Then
            AfficherTexte(sExplication & " : Exact !!")
            bErreur = False
        ElseIf bPrefixeOk Then
            AfficherTexte(sSensPrefixeMaj & " : Exact !")
            AfficherTexte(sAffSuffixe & "Réponse = -" & sSuffixe & " : " & sSensSuffixeMaj)
        ElseIf bSuffixeOk Then
            AfficherTexte(sAffPrefixe & "Réponse = " & sPrefixe & "- : " & sSensPrefixeMaj)
            AfficherTexte(sSensSuffixeMaj & " : Exact !")
        Else
            AfficherTexte(sAffPrefixe & "Réponse = " & sPrefixe & "- : " & sSensPrefixeMaj)
            AfficherTexte(sAffSuffixe & "Réponse = -" & sSuffixe & " : " & sSensSuffixeMaj)
        End If

    End Sub

    Private Sub QuizDefinitionMotExistant(
        lstNiv As List(Of String), lstFreq As List(Of String), iAlternatives%, _
        ByRef iScore%, ByRef bErreur As Boolean)

        ' Quiz sur la définition du préfixe et du suffixe

        iScore = 0

        Dim prefixe As clsSegmentBase = Nothing
        Dim suffixe As clsSegmentBase = Nothing
        Dim iNumMotExistant = iTirageMotExistant(lstNiv, lstFreq, prefixe, suffixe)
        If iNumMotExistant = iTirageImpossible Then Exit Sub

        Dim sNiveauP = prefixe.sNiveau
        Dim sNiveauS = suffixe.sNiveau

        Dim sPrefixe = prefixe.sSegment
        Dim sPrefixeElision = prefixe.sSegmentElision
        Dim sSuffixe = suffixe.sSegment
        'Dim sPrefixeMaj = sPrefixe.ToUpper()
        Dim sPrefixeMaj = sPrefixeElision.ToUpper() ' 28/04/2019
        Dim sSuffixeMaj = sSuffixe.ToUpper()
        Dim sSensPrefixeMaj = prefixe.sSens.ToUpper()
        sSensPrefixeMaj = sCompleterPrefixe(sSensPrefixeMaj)
        prefixe.sEtym = m_prefixes.sTrouverEtymologie(sPrefixe, prefixe.sUniciteSynth) ' 10/05/2018
        Dim sEtymPrefixe = prefixe.sEtym
        Dim sSensSuffixeMaj = suffixe.sSens.ToUpper()
        suffixe.sEtym = m_suffixes.sTrouverEtymologie(sSuffixe, suffixe.sUniciteSynth) ' 10/05/2018
        Dim sEtymSuffixe = suffixe.sEtym

        Dim sMot$ = sPrefixeMaj & sSuffixeMaj
        Dim sDetail$ = sPrefixeMaj & "(" & sNiveauP & ")-" & sSuffixeMaj & "(" & sNiveauS & ")"
        Dim sExplication$ = sSensSuffixeMaj & " " & sSensPrefixeMaj

        Dim lstEtym = New List(Of String)
        If sEtymPrefixe.Length > 0 Then lstEtym.Add(sPrefixe & "- : " & sEtymPrefixe)
        If sEtymSuffixe.Length > 0 Then lstEtym.Add("-" & sSuffixe & " : " & sEtymSuffixe)

        Dim lstNumMotExistant As New List(Of Integer)
        Dim itPrefixe As New clsInitTirage(prefixe)
        Dim itSuffixe As New clsInitTirage(suffixe)

        Dim lstExplicationsPrefixe As New List(Of String)
        If bDebugUnicite Then
            Debug.WriteLine("iNumMotExistant = " & iNumMotExistant)
            If prefixe.sUnicite.Length > 0 Then sSensPrefixeMaj &= " [" & prefixe.sUnicite & "]"
            If suffixe.sUnicite.Length > 0 Then sSensSuffixeMaj &= " [" & suffixe.sUnicite & "]"
        End If
        If bDebugUniciteSynth Then
            Debug.WriteLine("iNumMotExistant = " & iNumMotExistant)
            sSensPrefixeMaj &= " (" & prefixe.sUniciteSynth & ")"
            sSensSuffixeMaj &= " (" & suffixe.sUniciteSynth & ")"
        End If
        lstExplicationsPrefixe.Add(sSensPrefixeMaj)
        Dim lstExplicationsSuffixe As New List(Of String)
        lstExplicationsSuffixe.Add(sSensSuffixeMaj)

        For j As Integer = 0 To iAlternatives - 1

            Dim motAutre As clsMotExistant = Nothing
            Dim iNumMotExistantAutre = iTirageMotExistantAutre(lstNiv, lstFreq, iNumMotExistant, _
                itPrefixe, itSuffixe, lstNumMotExistant, motAutre)
            If iNumMotExistantAutre = iTirageImpossible Then Exit For
            If IsNothing(motAutre) Then Exit For

            Dim sDefPrefixe = motAutre.sDefPrefixe
            Dim sDefSuffixe = motAutre.sDefSuffixe
            If bDebugUnicite Then
                If motAutre.sUnicitePrefixe.Length > 0 Then _
                    sDefPrefixe &= " [" & motAutre.sUnicitePrefixe & "]"
                If motAutre.sUniciteSuffixe.Length > 0 Then _
                    sDefSuffixe &= " [" & motAutre.sUniciteSuffixe & "]"
            End If
            If bDebugUniciteSynth Then
                sDefPrefixe &= " (" & motAutre.sUnicitePrefixeSynth & ")"
                sDefSuffixe &= " (" & motAutre.sUniciteSuffixeSynth & ")"
            End If
            lstExplicationsPrefixe.Add(sDefPrefixe)
            lstExplicationsSuffixe.Add(sDefSuffixe)

        Next

        RemplirListBoxAuHasard(lbPrefixesPossibles, lstExplicationsPrefixe)
        RemplirListBoxAuHasard(lbSuffixesPossibles, lstExplicationsSuffixe)

        AfficherTexte(sMot)

        Me.cmdValider.Enabled = True
        While Me.cmdValider.Enabled
            If m_bAnnuler Then
                Activation(bActiver:=True)
                Exit Sub
            End If
            Application.DoEvents()
        End While

        Dim sSensPrefixeChoisi$ = Me.lbPrefixesPossibles.Text
        Dim sSensSuffixeChoisi$ = Me.lbSuffixesPossibles.Text

        Dim bPrefixeOk, bSuffixeOk As Boolean
        bPrefixeOk = False : bSuffixeOk = False
        If sSensPrefixeChoisi = sSensPrefixeMaj AndAlso _
            sSensSuffixeChoisi = sSensSuffixeMaj Then
            iScore += iCoefBonus : bPrefixeOk = True : bSuffixeOk = True
        ElseIf sSensPrefixeChoisi = sSensPrefixeMaj Then
            iScore += 1 : bPrefixeOk = True
        ElseIf sSensSuffixeChoisi = sSensSuffixeMaj Then
            iScore += 1 : bSuffixeOk = True
        End If

        Dim sAffPrefixe = sSensPrefixeChoisi & " : Faux ! "
        Dim sAffSuffixe = sSensSuffixeChoisi & " : Faux ! "
        If sSensPrefixeChoisi.Length = 0 Then sAffPrefixe = ""
        If sSensSuffixeChoisi.Length = 0 Then sAffSuffixe = ""
        AfficherTexte(sDetail)
        If lstEtym.Count > 0 Then
            For Each sEtym In lstEtym
                AfficherTexte(sEtym)
            Next
        End If
        bErreur = True
        If bPrefixeOk AndAlso bSuffixeOk Then
            AfficherTexte(sExplication & " : Exact !!")
            bErreur = False
        ElseIf bPrefixeOk Then
            AfficherTexte(sSensPrefixeMaj & " : Exact !")
            AfficherTexte(sAffSuffixe & "Réponse = -" & sSuffixe & " : " & sSensSuffixeMaj)
        ElseIf bSuffixeOk Then
            AfficherTexte(sAffPrefixe & "Réponse = " & sPrefixe & "- : " & sSensPrefixeMaj)
            AfficherTexte(sSensSuffixeMaj & " : Exact !")
        Else
            AfficherTexte(sAffPrefixe & "Réponse = " & sPrefixe & "- : " & sSensPrefixeMaj)
            AfficherTexte(sAffSuffixe & "Réponse = -" & sSuffixe & " : " & sSensSuffixeMaj)
        End If

    End Sub

    Private Sub QuizSegment(basePrefixe As clsBase, baseSuffixe As clsBase, _
        lstNiv As List(Of String), lstFreq As List(Of String), iAlternatives%, _
        ByRef iScore%, ByRef bErreur As Boolean)

        ' Quiz sur le préfixe et le suffixe correspondant à une définition

        iScore = 0

        Dim bGrecoLatin = Me.chkOrigineGrecoLatin.Checked
        Dim bNeoRigolo = Me.chkOrigineNeoRigolo.Checked

        ' Les préfixes et suffixes des mots du dictionnaire sont plus nombreux
        '  ne prendre que ceux qui forme des mots potentiels plausibles
        Const bComplet = False

        Dim iNumPrefixe% = basePrefixe.iTirageSegment(bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo)
        Dim prefixe As clsSegmentBase = Nothing
        If Not basePrefixe.bLireSegment(iNumPrefixe, prefixe) Then Exit Sub
        Dim sNiveauP = prefixe.sNiveau

        Dim iNumSuffixe% = baseSuffixe.iTirageSegment(bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo)
        Dim suffixe As clsSegmentBase = Nothing
        If Not baseSuffixe.bLireSegment(iNumSuffixe, suffixe) Then Exit Sub
        Dim sNiveauS = suffixe.sNiveau

        Dim sPrefixe = prefixe.sSegment
        Dim sSuffixe = suffixe.sSegment
        Dim sPrefixeMaj = sPrefixe.ToUpper()
        Dim sSuffixeMaj = sSuffixe.ToUpper()
        Dim sSensPrefixe = prefixe.sSens
        Dim sSensPrefixeMaj = sSensPrefixe.ToUpper()
        sSensPrefixeMaj = sCompleterPrefixe(sSensPrefixeMaj)
        Dim sEtymPrefixe = prefixe.sEtym
        Dim sSensSuffixe = suffixe.sSens
        Dim sSensSuffixeMaj = sSensSuffixe.ToUpper()
        Dim sEtymSuffixe = suffixe.sEtym

        Dim sMot$ = sPrefixeMaj & sSuffixeMaj
        Dim sDetail$ = sPrefixeMaj & "(" & sNiveauP & ")-" & sSuffixeMaj & "(" & sNiveauS & ")"
        Dim sExplication$ = sSensSuffixeMaj & " " & sSensPrefixeMaj
        Dim sPrefixeMajT$ = sPrefixeMaj & "-"
        Dim sTSuffixeMaj$ = "-" & sSuffixeMaj

        Dim lstEtym = New List(Of String)
        If sEtymPrefixe.Length > 0 Then lstEtym.Add(sPrefixe & "- : " & sEtymPrefixe)
        If sEtymSuffixe.Length > 0 Then lstEtym.Add("-" & sSuffixe & " : " & sEtymSuffixe)

        If bDebugUnicite Then
            If prefixe.sUnicite.Length > 0 Then sPrefixeMajT &= " (" & prefixe.sUnicite & ")"
            If suffixe.sUnicite.Length > 0 Then sTSuffixeMaj &= " (" & suffixe.sUnicite & ")"
        End If
        If bDebugUniciteSynth Then
            sPrefixeMajT &= " (" & prefixe.sUniciteSynth & ")"
            sTSuffixeMaj &= " (" & suffixe.sUniciteSynth & ")"
        End If

        Dim itPrefixe As New clsInitTirage(prefixe)
        Dim lstPrefixesMajT As New List(Of String)
        lstPrefixesMajT.Add(sPrefixeMajT)

        Dim itSuffixe As New clsInitTirage(suffixe)
        Dim lstSuffixesTMaj As New List(Of String)
        lstSuffixesTMaj.Add(sTSuffixeMaj)

        For j As Integer = 0 To iAlternatives - 1

            Dim iNumPrefixeAutre% = basePrefixe.iTirageSegment(bComplet,
                lstNiv, lstFreq, itPrefixe, bGrecoLatin, bNeoRigolo)
            Dim prefixeP2 As clsSegmentBase = Nothing
            If Not basePrefixe.bLireSegment(iNumPrefixeAutre, prefixeP2) Then Exit For

            Dim iNumSuffixeAutre% = baseSuffixe.iTirageSegment(bComplet,
                lstNiv, lstFreq, itSuffixe, bGrecoLatin, bNeoRigolo)
            Dim suffixeS2 As clsSegmentBase = Nothing
            If Not baseSuffixe.bLireSegment(iNumSuffixeAutre, suffixeS2) Then Exit For

            Dim sPrefixeAutre$ = prefixeP2.sSegment.ToUpper() & "-"
            Dim sSuffixeAutre$ = "-" & suffixeS2.sSegment.ToUpper()
            If bDebugUnicite Then
                If prefixeP2.sUnicite.Length > 0 Then _
                    sPrefixeAutre &= " [" & prefixeP2.sUnicite & "]"
                If suffixeS2.sUnicite.Length > 0 Then _
                    sSuffixeAutre &= " [" & suffixeS2.sUnicite & "]"
            End If
            If bDebugUniciteSynth Then
                sPrefixeAutre &= " (" & prefixeP2.sUniciteSynth & ")"
                sSuffixeAutre &= " (" & suffixeS2.sUniciteSynth & ")"
            End If
            lstPrefixesMajT.Add(sPrefixeAutre)
            lstSuffixesTMaj.Add(sSuffixeAutre)

        Next

        RemplirListBoxAuHasard(lbPrefixesPossibles, lstPrefixesMajT)
        RemplirListBoxAuHasard(lbSuffixesPossibles, lstSuffixesTMaj)

        AfficherTexte(sExplication)

        Me.cmdValider.Enabled = True
        While Me.cmdValider.Enabled
            If m_bAnnuler Then
                Activation(bActiver:=True)
                Exit Sub
            End If
            Application.DoEvents()
        End While

        Dim sPrefixeChoisi$ = Me.lbPrefixesPossibles.Text
        Dim sSuffixeChoisi$ = Me.lbSuffixesPossibles.Text

        Dim bPrefixeOk, bSuffixeOk As Boolean
        bPrefixeOk = False : bSuffixeOk = False
        If sPrefixeChoisi = sPrefixeMajT AndAlso _
            sSuffixeChoisi = sTSuffixeMaj Then
            iScore += iCoefBonus : bPrefixeOk = True : bSuffixeOk = True
        ElseIf sPrefixeChoisi = sPrefixeMajT Then
            iScore += 1 : bPrefixeOk = True
        ElseIf sSuffixeChoisi = sTSuffixeMaj Then
            iScore += 1 : bSuffixeOk = True
        End If

        Dim sAffPrefixe = sPrefixeChoisi & " : Faux ! "
        Dim sAffSuffixe = sSuffixeChoisi & " : Faux ! "
        If sPrefixeChoisi.Length = 0 Then sAffPrefixe = ""
        If sSuffixeChoisi.Length = 0 Then sAffSuffixe = ""
        AfficherTexte(sDetail)
        If lstEtym.Count > 0 Then
            For Each sEtym In lstEtym
                AfficherTexte(sEtym)
            Next
        End If
        bErreur = True
        If bPrefixeOk AndAlso bSuffixeOk Then
            AfficherTexte(sMot & " : Exact !!")
            bErreur = False
        ElseIf bPrefixeOk Then
            AfficherTexte(sPrefixeMaj & " : Exact !")
            AfficherTexte(sAffSuffixe & "Réponse = -" & sSuffixe & " : " & sSensSuffixeMaj)
        ElseIf bSuffixeOk Then
            AfficherTexte(sAffPrefixe & "Réponse = " & sPrefixe & "- : " & sSensPrefixeMaj)
            AfficherTexte(sSensSuffixeMaj & " : Exact !")
        Else
            AfficherTexte(sAffPrefixe & "Réponse = " & sPrefixe & "- : " & sSensPrefixeMaj)
            AfficherTexte(sAffSuffixe & "Réponse = -" & sSuffixe & " : " & sSensSuffixeMaj)
        End If

    End Sub

    Private Sub QuizSegmentMotExistant(
        lstNiv As List(Of String), lstFreq As List(Of String), iAlternatives%, _
        ByRef iScore%, ByRef bErreur As Boolean)

        ' Quiz sur le préfixe et le suffixe correspondant à une définition

        iScore = 0

        Dim prefixe As clsSegmentBase = Nothing
        Dim suffixe As clsSegmentBase = Nothing
        Dim iNumMotExistant = iTirageMotExistant(lstNiv, lstFreq, prefixe, suffixe)
        If iNumMotExistant = iTirageImpossible Then Exit Sub

        Dim sNiveauP = prefixe.sNiveau
        Dim sNiveauS = suffixe.sNiveau

        Dim sPrefixe = prefixe.sSegment
        Dim sPrefixeElision = prefixe.sSegmentElision
        Dim sSuffixe = suffixe.sSegment
        'Dim sPrefixeMaj = sPrefixe.ToUpper()
        Dim sPrefixeMaj = sPrefixeElision.ToUpper() ' 28/04/2019
        Dim sSuffixeMaj = sSuffixe.ToUpper()
        Dim sSensPrefixe = prefixe.sSens
        Dim sSensPrefixeMaj = sSensPrefixe.ToUpper()
        sSensPrefixeMaj = sCompleterPrefixe(sSensPrefixeMaj)
        prefixe.sEtym = m_prefixes.sTrouverEtymologie(sPrefixe, prefixe.sUniciteSynth) ' 10/05/2018
        Dim sEtymPrefixe = prefixe.sEtym
        Dim sSensSuffixe = suffixe.sSens
        Dim sSensSuffixeMaj = sSensSuffixe.ToUpper()
        suffixe.sEtym = m_suffixes.sTrouverEtymologie(sSuffixe, suffixe.sUniciteSynth) ' 10/05/2018
        Dim sEtymSuffixe = suffixe.sEtym

        Dim sMot$ = sPrefixeMaj & sSuffixeMaj
        Dim sDetail$ = sPrefixeMaj & "(" & sNiveauP & ")-" & sSuffixeMaj & "(" & sNiveauS & ")"
        Dim sExplication$ = sSensSuffixeMaj & " " & sSensPrefixeMaj
        Dim sPrefixeMajT$ = sPrefixeMaj & "-"
        Dim sTSuffixeMaj$ = "-" & sSuffixeMaj

        Dim lstEtym = New List(Of String)
        If sEtymPrefixe.Length > 0 Then lstEtym.Add(sPrefixe & "- : " & sEtymPrefixe)
        If sEtymSuffixe.Length > 0 Then lstEtym.Add("-" & sSuffixe & " : " & sEtymSuffixe)

        If bDebugUnicite Then
            If prefixe.sUnicite.Length > 0 Then sPrefixeMajT &= " [" & prefixe.sUnicite & "]"
            If suffixe.sUnicite.Length > 0 Then sTSuffixeMaj &= " [" & suffixe.sUnicite & "]"
        End If
        If bDebugUniciteSynth Then
            sPrefixeMajT &= " (" & prefixe.sUniciteSynth & ")"
            sTSuffixeMaj &= " (" & suffixe.sUniciteSynth & ")"
        End If

        Dim lstPrefixesMajT As New List(Of String)
        lstPrefixesMajT.Add(sPrefixeMajT)

        Dim lstSuffixesTMaj As New List(Of String)
        lstSuffixesTMaj.Add(sTSuffixeMaj)

        Dim lstNumMotExistant As New List(Of Integer)
        Dim itPrefixe As New clsInitTirage(prefixe)
        Dim itSuffixe As New clsInitTirage(suffixe)

        For j As Integer = 0 To iAlternatives - 1

            Dim motAutre As clsMotExistant = Nothing
            Dim iNumMotExistantAutre = iTirageMotExistantAutre(lstNiv, lstFreq, iNumMotExistant, _
                itPrefixe, itSuffixe, lstNumMotExistant, motAutre)
            If iNumMotExistantAutre = iTirageImpossible Then Exit For
            If IsNothing(motAutre) Then Exit For

            Dim sDefPrefixe = motAutre.sPrefixe.ToUpper() & "-"
            Dim sDefSuffixe = "-" & motAutre.sSuffixe.ToUpper()
            If bDebugUnicite Then
                If motAutre.sUnicitePrefixe.Length > 0 Then _
                    sDefPrefixe &= " [" & motAutre.sUnicitePrefixe & "]"
                If motAutre.sUniciteSuffixe.Length > 0 Then _
                    sDefSuffixe &= " [" & motAutre.sUniciteSuffixe & "]"
            End If
            If bDebugUniciteSynth Then
                sDefPrefixe &= " (" & motAutre.sUnicitePrefixeSynth & ")"
                sDefSuffixe &= " (" & motAutre.sUniciteSuffixeSynth & ")"
            End If
            lstPrefixesMajT.Add(sDefPrefixe)
            lstSuffixesTMaj.Add(sDefSuffixe)

        Next

        RemplirListBoxAuHasard(lbPrefixesPossibles, lstPrefixesMajT)
        RemplirListBoxAuHasard(lbSuffixesPossibles, lstSuffixesTMaj)

        AfficherTexte(sExplication)

        Me.cmdValider.Enabled = True
        While Me.cmdValider.Enabled
            If m_bAnnuler Then
                Activation(bActiver:=True)
                Exit Sub
            End If
            Application.DoEvents()
        End While

        Dim sPrefixeChoisi$ = Me.lbPrefixesPossibles.Text
        Dim sSuffixeChoisi$ = Me.lbSuffixesPossibles.Text

        Dim bPrefixeOk, bSuffixeOk As Boolean
        bPrefixeOk = False : bSuffixeOk = False
        If sPrefixeChoisi = sPrefixeMajT AndAlso _
            sSuffixeChoisi = sTSuffixeMaj Then
            iScore += iCoefBonus : bPrefixeOk = True : bSuffixeOk = True
        ElseIf sPrefixeChoisi = sPrefixeMajT Then
            iScore += 1 : bPrefixeOk = True
        ElseIf sSuffixeChoisi = sTSuffixeMaj Then
            iScore += 1 : bSuffixeOk = True
        End If

        Dim sAffPrefixe = sPrefixeChoisi & " : Faux ! "
        Dim sAffSuffixe = sSuffixeChoisi & " : Faux ! "
        If sPrefixeChoisi.Length = 0 Then sAffPrefixe = ""
        If sSuffixeChoisi.Length = 0 Then sAffSuffixe = ""
        AfficherTexte(sDetail)
        If lstEtym.Count > 0 Then
            For Each sEtym In lstEtym
                AfficherTexte(sEtym)
            Next
        End If
        bErreur = True
        If bPrefixeOk AndAlso bSuffixeOk Then
            AfficherTexte(sMot & " : Exact !!")
            bErreur = False
        ElseIf bPrefixeOk Then
            AfficherTexte(sPrefixeMaj & " : Exact !")
            AfficherTexte(sAffSuffixe & "Réponse = -" & sSuffixe & " : " & sSensSuffixeMaj)
        ElseIf bSuffixeOk Then
            AfficherTexte(sAffPrefixe & "Réponse = " & sPrefixe & "- : " & sSensPrefixeMaj)
            AfficherTexte(sSensSuffixeMaj & " : Exact !")
        Else
            AfficherTexte(sAffPrefixe & "Réponse = " & sPrefixe & "- : " & sSensPrefixeMaj)
            AfficherTexte(sAffSuffixe & "Réponse = -" & sSuffixe & " : " & sSensSuffixeMaj)
        End If

    End Sub

    Private m_iIndex% = 0
    Private Sub AfficherTexte(sTxt$)
        AfficherTexteListBox(sTxt$, m_iIndex, Me, Me.lbResultats)
        AfficherMsgBarreMsg(sTxt)
    End Sub
    Private Sub EffacerMessages()
        lbResultats.Items.Clear()
        m_iIndex = 0
    End Sub

    Private Sub AfficherMsgBarreMsg(sTxt$)

        Me.ToolStripStatusLabelBarreMsg.Text = sTxt

    End Sub

    Private Sub cmdValider_Click(sender As Object, e As EventArgs) Handles cmdValider.Click

        If m_bAttendre Then
            m_bAttendre = False
            Exit Sub
        End If

        Me.cmdValider.Enabled = False

    End Sub

    Private Sub cmdCopier_Click(sender As Object, e As EventArgs) Handles cmdCopier.Click

        Dim sTxt$ = sLireListBox(lbResultats)
        If bCopierPressePapier(sTxt) Then _
            MsgBox(sMsgCopiePressePapier, MsgBoxStyle.Exclamation, m_sTitreMsg)

    End Sub

End Class