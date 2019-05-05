
Imports System.Text

Public Module modBase

    Public Const iNbColonnes% = 8 ' 01/07/2018 Fréquence ajoutée
    Public Const iTirageImpossible% = -1

    Public m_prefixes As clsBase, m_suffixes As clsBase

    Public m_defFls As clsDefExclusives

#Region "Mots existants"

    Public Class clsMotExistant

        Public Const iNbColonnesME% = 10

        Public Const iColMot% = 0
        Public Const iColDef% = 1
        Public Const iColPrefixe% = 2
        Public Const iColSuffixe% = 3
        Public Const iColNivPrefixe% = 4
        Public Const iColNivSuffixe% = 5
        Public Const iColUnicitePrefixe% = 6
        Public Const iColUniciteSuffixe% = 7
        Public Const iColFreqPrefixe% = 8
        Public Const iColFreqSuffixe% = 9

        Public sMot$, sDef$, sPrefixe$, sSuffixe$, sDefPrefixe$, sDefSuffixe$,
            sNivPrefixe$, sNivSuffixe$,
            sUnicitePrefixe$, sUniciteSuffixe$
        Public sUnicitePrefixeSynth$, sUniciteSuffixeSynth$
        Public sFreqPrefixe$, sFreqSuffixe$
        Public iNivPrefixe%, iNivSuffixe%
        Public iNumMotExistant%
        Public bElisionPrefixe As Boolean ' bElisionSuffixe 
        Public Sub New()
        End Sub
        Public Sub New(sMot$, sDef$, sPrefixe$, sSuffixe$, sDefPrefixe$, sDefSuffixe$,
            sNivPrefixe$, sNivSuffixe$, sUnicitePrefixe$, sUniciteSuffixe$, iNumMot%,
            sFreqPrefixe$, sFreqSuffixe$, bElisionPrefixe As Boolean)
            Me.sMot = sMot
            Me.sDef = sDef
            Me.sPrefixe = sPrefixe
            Me.sSuffixe = sSuffixe
            Me.sDefPrefixe = sDefPrefixe
            Me.sDefSuffixe = sDefSuffixe
            Me.sNivPrefixe = sNivPrefixe
            Me.sNivSuffixe = sNivSuffixe
            Me.iNivPrefixe = Integer.Parse(Me.sNivPrefixe)
            Me.iNivSuffixe = Integer.Parse(Me.sNivSuffixe)
            Me.sUnicitePrefixe = sUnicitePrefixe
            Me.sUniciteSuffixe = sUniciteSuffixe
            Me.iNumMotExistant = iNumMot
            Me.sFreqPrefixe = sFreqPrefixe
            Me.sFreqSuffixe = sFreqSuffixe
            Me.bElisionPrefixe = bElisionPrefixe
            'Me.bElisionSuffixe = bElisionSuffixe

            Synthese()

        End Sub

        Public Sub Synthese()
            Dim sSensPrefixeSansArticle$ = sSupprimerArticle(sDefPrefixe)
            Me.sUnicitePrefixeSynth = sSensPrefixeSansArticle
            If sUnicitePrefixe.Length > 0 Then Me.sUnicitePrefixeSynth = sUnicitePrefixe
            Dim sSensSuffixeSansArticle$ = sSupprimerArticle(sDefSuffixe)
            Me.sUniciteSuffixeSynth = sSensSuffixeSansArticle
            If sUniciteSuffixe.Length > 0 Then Me.sUniciteSuffixeSynth = sUniciteSuffixe
        End Sub

        Public Shared Sub ParserDefinition(sDef$, ByRef sDefSuffixe$, ByRef sDefPrefixe$)
            Dim asChamps2$() = sDef.Split(New String() {sSepDef}, StringSplitOptions.None)
            Dim iNbChamps2% = asChamps2.GetUpperBound(0) + 1
            sDefSuffixe = Nothing
            sDefPrefixe = Nothing
            If iNbChamps2 >= 1 Then sDefSuffixe = asChamps2(0).Trim
            If iNbChamps2 >= 2 Then sDefPrefixe = asChamps2(1).Trim
        End Sub
        Public Sub ParserDefinition()
            Dim asChamps2$() = Me.sDef.Split(New String() {sSepDef}, StringSplitOptions.None)
            Dim iNbChamps2% = asChamps2.GetUpperBound(0) + 1
            Me.sDefSuffixe = Nothing
            Me.sDefPrefixe = Nothing
            If iNbChamps2 >= 1 Then Me.sDefSuffixe = asChamps2(0).Trim
            If iNbChamps2 >= 2 Then Me.sDefPrefixe = asChamps2(1).Trim
        End Sub

        Public Overrides Function ToString$()
            Dim sDef$ = Me.sDefSuffixe.ToUpper & sSepDef & sCompleterPrefixe(Me.sDefPrefixe.ToUpper)
            Dim sTxt$ = Me.sMot & " : " & sDef & " : " &
                Me.sPrefixe & "(" & Me.sNivPrefixe & ")-" &
                Me.sSuffixe & "(" & Me.sNivSuffixe & ")"
            If Me.sUnicitePrefixe.Length > 0 Then sTxt &= " (unicité préfixe : " & Me.sUnicitePrefixe & ")"
            If Me.sUniciteSuffixe.Length > 0 Then sTxt &= " (unicité suffixe : " & Me.sUniciteSuffixe & ")"
            Return sTxt
        End Function

        Public Function sAfficherPrefixe$()
            Dim sDef$ = Me.sDefSuffixe.ToUpper & sSepDef & sCompleterPrefixe(Me.sDefPrefixe.ToUpper)
            Dim sTxt$ = Me.sPrefixe & "-" &
                " (" & Me.sMot & " : " & sDef & " : " &
                Me.sPrefixe & "(" & Me.sNivPrefixe & ") " &
                Me.sSuffixe & "(" & Me.sNivSuffixe & "))"
            If Me.sUnicitePrefixe.Length > 0 Then sTxt &= " (unicité préfixe : " & Me.sUnicitePrefixe & ")"
            If Me.sUniciteSuffixe.Length > 0 Then sTxt &= " (unicité suffixe : " & Me.sUniciteSuffixe & ")"
            Return sTxt
        End Function

        Public Function sAfficherSuffixe$()
            Dim sDef$ = Me.sDefSuffixe.ToUpper & sSepDef & sCompleterPrefixe(Me.sDefPrefixe.ToUpper)
            Dim sTxt$ = "-" & Me.sSuffixe &
                " (" & Me.sMot & " : " & sDef & " : " &
                Me.sPrefixe & "(" & Me.sNivPrefixe & ") " &
                Me.sSuffixe & "(" & Me.sNivSuffixe & "))"
            If Me.sUnicitePrefixe.Length > 0 Then sTxt &= " (unicité préfixe : " & Me.sUnicitePrefixe & ")"
            If Me.sUniciteSuffixe.Length > 0 Then sTxt &= " (unicité suffixe : " & Me.sUniciteSuffixe & ")"
            Return sTxt
        End Function

    End Class

    Public m_dicoMotsExistants As Dictionary(Of String, clsMotExistant)

    Public Sub ChargerMotsExistantsCsv()

        m_dicoMotsExistants = New Dictionary(Of String, clsMotExistant)
        Dim sChemin$ = Application.StartupPath & "\MotsSimples" & sLang & ".csv"
        If Not bFichierExiste(sChemin, bPrompt:=True) Then Exit Sub

        Dim sb As New StringBuilder
        Const bFiltrer As Boolean = False
        Const rTauxFiltre! = 0.02

        Dim asLignes$() = asLireFichier(sChemin)
        Dim iNumLigne% = 0
        Dim iNumMot% = 0
        For Each sLigne In asLignes
            iNumLigne += 1
            If iNumLigne < 2 Then Continue For ' 1 ligne d'entête

            If bFiltrer Then
                Dim r = rRandomiser()
                If r < rTauxFiltre OrElse sLigne.IndexOf("MÉDECIN") > 0 Then _
                    sb.AppendLine(sLigne)
            End If

            Dim asChamps$() = sLigne.Split(";"c) ' ":"c
            Dim iNbChamps% = asChamps.GetUpperBound(0) + 1
            Dim sMot$, sDef$, sDecoup$, sPrefixe$, sSuffixe$, sDefPrefixe$, sDefSuffixe$
            Dim sNivPrefixe$, sNivSuffixe$, sUnicitePrefixe$, sUniciteSuffixe$
            Dim sFreqPrefixe$, sFreqSuffixe$
            Dim bElisionPrefixe As Boolean ' bElisionSuffixe 

            sMot = "" : sDef = "" : sDecoup = "" : sPrefixe = "" : sSuffixe = ""
            sDefPrefixe = "" : sDefSuffixe = ""
            sNivPrefixe = "" : sNivSuffixe = ""
            sUnicitePrefixe = "" : sUniciteSuffixe = ""
            sFreqPrefixe = "" : sFreqSuffixe = ""
            bElisionPrefixe = False ': bElisionSuffixe = False

            If iNbChamps >= 1 Then sMot = asChamps(0).Trim
            If iNbChamps >= 2 Then
                sDef = asChamps(1).Trim
                clsMotExistant.ParserDefinition(sDef, sDefSuffixe, sDefPrefixe)
            End If

            If iNbChamps >= 3 Then sPrefixe = asChamps(2).Trim
            If iNbChamps >= 4 Then sSuffixe = asChamps(3).Trim
            If iNbChamps >= 5 Then sNivPrefixe = asChamps(4).Trim
            If iNbChamps >= 6 Then sNivSuffixe = asChamps(5).Trim
            If iNbChamps >= 7 Then sUnicitePrefixe = asChamps(6).Trim
            If iNbChamps >= 8 Then sUniciteSuffixe = asChamps(7).Trim
            If iNbChamps >= 9 Then sFreqPrefixe = asChamps(8).Trim
            If iNbChamps >= 10 Then sFreqSuffixe = asChamps(9).Trim

            ' 28/04/2019
            If bElision AndAlso sPrefixe.EndsWith(sCarElisionO) Then
                bElisionPrefixe = True
                sPrefixe = sPrefixe.Replace(sCarElisionO, sCarO)
            End If
            'If sSuffixe.EndsWith(sCarO) Then
            '    bElisionSuffixe = True
            'End If

            If sNivPrefixe = "" OrElse sNivSuffixe = "" Then
                If bDebug Then Stop
                Continue For
            End If

            If Not m_dicoMotsExistants.ContainsKey(sMot) Then
                m_dicoMotsExistants.Add(sMot, New clsMotExistant(sMot, sDef,
                    sPrefixe, sSuffixe, sDefPrefixe, sDefSuffixe,
                    sNivPrefixe, sNivSuffixe, sUnicitePrefixe, sUniciteSuffixe, iNumMot,
                    sFreqPrefixe, sFreqSuffixe, bElisionPrefixe))
                iNumMot += 1
            End If

        Next

        If bFiltrer Then
            Dim sCheminFiltre$ = Application.StartupPath & "\MotsSimplesSelect.csv"
            If Not bEcrireFichier(sCheminFiltre, sb) Then Exit Sub
        End If

    End Sub

    Public Function bLireMot(lstMots As List(Of String), iNumMot%,
        ByRef mot As clsMotExistant) As Boolean

        mot = New clsMotExistant()
        mot.iNumMotExistant = iNumMot
        Dim iNumSegment% = iNumMot * clsMotExistant.iNbColonnesME

        mot.sMot = lstMots(iNumSegment + clsMotExistant.iColMot)
        If bDebug AndAlso (mot.sMot Is Nothing) Then Stop

        mot.sDef = lstMots(iNumSegment + clsMotExistant.iColDef)
        If bDebug AndAlso (mot.sDef Is Nothing) Then Stop

        mot.sPrefixe = lstMots(iNumSegment + clsMotExistant.iColPrefixe)
        If bDebug AndAlso (mot.sPrefixe Is Nothing) Then Stop

        ' 28/04/2019
        mot.bElisionPrefixe = False
        If bElision AndAlso mot.sPrefixe.EndsWith(sCarElisionO) Then
            mot.bElisionPrefixe = True
            mot.sPrefixe = mot.sPrefixe.Replace(sCarElisionO, sCarO)
        End If

        mot.sSuffixe = lstMots(iNumSegment + clsMotExistant.iColSuffixe)
        If bDebug AndAlso (mot.sSuffixe Is Nothing) Then Stop

        mot.sNivPrefixe = lstMots(iNumSegment + clsMotExistant.iColNivPrefixe)
        If bDebug AndAlso (mot.sNivPrefixe Is Nothing) Then Stop
        mot.iNivPrefixe = Integer.Parse(mot.sNivPrefixe)

        mot.sNivSuffixe = lstMots(iNumSegment + clsMotExistant.iColNivSuffixe)
        If bDebug AndAlso (mot.sNivSuffixe Is Nothing) Then Stop
        mot.iNivSuffixe = Integer.Parse(mot.sNivSuffixe)

        mot.sUnicitePrefixe = lstMots(iNumSegment + clsMotExistant.iColUnicitePrefixe)
        If bDebug AndAlso (mot.sUnicitePrefixe Is Nothing) Then Stop

        mot.sUniciteSuffixe = lstMots(iNumSegment + clsMotExistant.iColUniciteSuffixe)
        If bDebug AndAlso (mot.sUniciteSuffixe Is Nothing) Then Stop

        mot.sFreqPrefixe = lstMots(iNumSegment + clsMotExistant.iColFreqPrefixe)
        If bDebug AndAlso (mot.sFreqPrefixe Is Nothing) Then Stop
        mot.sFreqSuffixe = lstMots(iNumSegment + clsMotExistant.iColFreqSuffixe)
        If bDebug AndAlso (mot.sFreqSuffixe Is Nothing) Then Stop

        mot.ParserDefinition()
        mot.Synthese()

        Return True

    End Function

    Public Function iNbPrefixesMotsExistants%(
        lstNiv As List(Of String), lstFreq As List(Of String))

        ' Compter tous les préfixes distincts des mots 
        '  du ou des niveaux demandé(s)
        '  de la ou des fréquence(s) demandée(s)
        Dim enreg = (From mot0 In m_dicoMotsExistants.ToList()
            Where
                lstNiv.Contains(mot0.Value.sNivPrefixe) AndAlso
                lstNiv.Contains(mot0.Value.sNivSuffixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqPrefixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqSuffixe)
            Select mot0.Value.sUnicitePrefixeSynth).Distinct
        Dim iNbEnreg% = enreg.Count
        Return iNbEnreg

    End Function

    Public Function lstPrefixesMotsExistants(
        lstNiv As List(Of String), lstFreq As List(Of String)) As List(Of clsMotExistant)

        ' Lister tous les préfixes des mots de niveau inférieur ou égal au niveau demandé
        ' (sans les mots dont les niveaux du préfixe et suffixe sont inférieurs)
        Dim enreg = From mot0 In m_dicoMotsExistants.ToList()
            Where
                lstNiv.Contains(mot0.Value.sNivPrefixe) AndAlso
                lstNiv.Contains(mot0.Value.sNivSuffixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqPrefixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqSuffixe)
        Dim lst As New List(Of clsMotExistant)
        Dim hs As New HashSet(Of String)
        For Each mot In enreg
            Dim sCle$ = mot.Value.sUnicitePrefixeSynth
            ' Ignorer les mots ayant la même racine (via l'unicité)
            If hs.Contains(sCle) Then Continue For
            hs.Add(sCle)
            lst.Add(mot.Value)
        Next
        Return lst

    End Function

    Public Function iNbSuffixesMotsExistants%(
        lstNiv As List(Of String), lstFreq As List(Of String))

        ' Compter tous les suffixes distincts des mots de niveau inférieur ou égal au niveau demandé
        ' (sans les mots dont les niveaux du préfixe et suffixe sont inférieurs)
        Dim enreg = (From mot0 In m_dicoMotsExistants.ToList()
            Where
                lstNiv.Contains(mot0.Value.sNivPrefixe) AndAlso
                lstNiv.Contains(mot0.Value.sNivSuffixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqPrefixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqSuffixe)
            Select mot0.Value.sUniciteSuffixeSynth).Distinct
        Dim iNbEnreg% = enreg.Count
        Return iNbEnreg

    End Function

    Public Function lstSuffixesMotsExistants(
        lstNiv As List(Of String), lstFreq As List(Of String)) As List(Of clsMotExistant)

        ' Lister tous les suffixes des mots de niveau inférieur ou égal au niveau demandé
        ' (sans les mots dont les niveaux du préfixe et suffixe sont inférieurs)
        Dim enreg = From mot0 In m_dicoMotsExistants.ToList()
            Where
                lstNiv.Contains(mot0.Value.sNivPrefixe) AndAlso
                lstNiv.Contains(mot0.Value.sNivSuffixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqPrefixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqSuffixe)
        Dim lst As New List(Of clsMotExistant)
        Dim hs As New HashSet(Of String)
        For Each mot In enreg
            Dim sCle$ = mot.Value.sUniciteSuffixeSynth
            ' Ignorer les mots ayant la même racine (via l'unicité)
            If hs.Contains(sCle) Then Continue For
            hs.Add(sCle)
            lst.Add(mot.Value)
        Next
        Return lst

    End Function

    Public Function iNbMotsExistants%(
        lstNiv As List(Of String), lstFreq As List(Of String))

        ' Compter tous les mots du niveau demandé

        ' Note : en VB. Net, le regroupement sur des clés multiples exige
        '  le mot clé Key, sinon, ça ne marche pas

        Dim enreg = From mot0 In m_dicoMotsExistants.ToList()
            Where
                lstNiv.Contains(mot0.Value.sNivPrefixe) AndAlso
                lstNiv.Contains(mot0.Value.sNivSuffixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqPrefixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqSuffixe)
            Group mot0 By regroupement = New With {
                Key mot0.Value.sUnicitePrefixeSynth,
                Key mot0.Value.sUniciteSuffixeSynth
            } Into Group Select New With {
                regroupement.sUnicitePrefixeSynth,
                regroupement.sUniciteSuffixeSynth
            }
        Dim iNbEnreg% = enreg.Count

        Return iNbEnreg

    End Function

    Public Function lstMotsExistants(
        lstNiv As List(Of String),
        lstFreq As List(Of String)) As List(Of clsMotExistant)

        ' Lister tous les mots du niveau demandé

        Dim enreg = From mot0 In m_dicoMotsExistants.ToList()
            Where
                lstNiv.Contains(mot0.Value.sNivPrefixe) AndAlso
                lstNiv.Contains(mot0.Value.sNivSuffixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqPrefixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqSuffixe)
        Dim lst As New List(Of clsMotExistant)
        Dim hs As New HashSet(Of String)
        For Each mot In enreg
            Dim sCle$ = mot.Value.sUnicitePrefixeSynth & ":" & mot.Value.sUniciteSuffixeSynth
            ' Ignorer les mots ayant la même racine (via l'unicité)
            If hs.Contains(sCle) Then Continue For
            hs.Add(sCle)
            lst.Add(mot.Value)
        Next
        Return lst

    End Function

    Public Function iTirageMotExistant%(
        lstNiv As List(Of String), lstFreq As List(Of String),
        ByRef prefixe As clsSegmentBase, ByRef suffixe As clsSegmentBase)

        ' Tirer au hasard un mot du niveau demandé

        ' 01/05/2019 Test élision : mot0.Value.bElisionPrefixe AndAlso

        Dim enreg = From mot0 In m_dicoMotsExistants.ToList()
            Where
                lstNiv.Contains(mot0.Value.sNivPrefixe) AndAlso
                lstNiv.Contains(mot0.Value.sNivSuffixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqPrefixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqSuffixe)
        Dim iNbEnreg% = enreg.Count
        If iNbEnreg = 0 Then
            VBMessageBox("Aucun mot ne correspond à la sélection : Tirage impossible !")
            Return iTirageImpossible
        End If

        Dim iNbMotsExistantsFiltres% = iNbEnreg
        ' On tire un nombre compris entre 0 et iNbMotsExistantsFiltres - 1 (liste filtrée)
        Dim iNumMotExistantTire = iRandomiser(0, iNbMotsExistantsFiltres - 1)
        Dim mot = enreg(iNumMotExistantTire).Value
        If IsNothing(mot) Then
            If bDebug Then Stop
            Return iTirageImpossible
        End If
        ' Indice du mot dans la liste complète
        Dim iNumMotExistant% = mot.iNumMotExistant
        If iNumMotExistant = 0 Then
            If bDebug Then Stop
            Return iTirageImpossible
        End If

        prefixe = New clsSegmentBase
        suffixe = New clsSegmentBase

        prefixe.sSegment = mot.sPrefixe
        prefixe.sLogotron = sSelectDictionnaire
        prefixe.sNiveau = mot.sNivPrefixe
        prefixe.sSens = mot.sDefPrefixe
        prefixe.sEtym = ""
        prefixe.sUnicite = mot.sUnicitePrefixe
        prefixe.sUniciteSynth = mot.sUnicitePrefixeSynth
        prefixe.sFrequence = mot.sFreqPrefixe

        ' 28/04/2019
        prefixe.bElision = mot.bElisionPrefixe
        prefixe.sSegmentElision = prefixe.sSegment
        If prefixe.bElision Then _
            prefixe.sSegmentElision = prefixe.sSegment.Substring(0, prefixe.sSegment.Length - 1)

        suffixe.sSegment = mot.sSuffixe
        suffixe.sLogotron = sSelectDictionnaire
        suffixe.sNiveau = mot.sNivSuffixe
        suffixe.sSens = mot.sDefSuffixe
        suffixe.sEtym = ""
        suffixe.sUnicite = mot.sUniciteSuffixe
        suffixe.sUniciteSynth = mot.sUniciteSuffixeSynth
        suffixe.sFrequence = mot.sFreqSuffixe

        Return iNumMotExistant

    End Function

    Public Function iTirageMotExistantAutre%(
        lstNiv As List(Of String), lstFreq As List(Of String), iNumMotExistant%,
        itPrefixe As clsInitTirage, itSuffixe As clsInitTirage,
        lstNumMotExistant As List(Of Integer), ByRef motAutre As clsMotExistant)

        ' Tirer au hasard un autre mot du niveau demandé

        motAutre = Nothing

        Dim enreg = From mot0 In m_dicoMotsExistants.ToList()
            Where
                lstNiv.Contains(mot0.Value.sNivPrefixe) AndAlso
                lstNiv.Contains(mot0.Value.sNivSuffixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqPrefixe) AndAlso
                lstFreq.Contains(mot0.Value.sFreqSuffixe) AndAlso
                Not lstNumMotExistant.Contains(iNumMotExistant) AndAlso
                Not itPrefixe.lstSegmentsDejaTires.Contains(mot0.Value.sPrefixe) AndAlso
                Not itSuffixe.lstSegmentsDejaTires.Contains(mot0.Value.sSuffixe) AndAlso
                Not itPrefixe.lstSensSegmentDejaTires.Contains(mot0.Value.sDefPrefixe) AndAlso
                Not itSuffixe.lstSensSegmentDejaTires.Contains(mot0.Value.sDefSuffixe) AndAlso
                Not itPrefixe.lstUnicitesSegmentDejaTires.Contains(mot0.Value.sUnicitePrefixeSynth) AndAlso
                Not itSuffixe.lstUnicitesSegmentDejaTires.Contains(mot0.Value.sUniciteSuffixeSynth)

        Dim iNbEnreg% = enreg.Count
        If iNbEnreg = 0 Then
            VBMessageBox("Aucun mot ne correspond à la sélection : Tirage impossible !")
            Return iTirageImpossible
        End If

        Dim iNbMotsExistantsFiltres% = iNbEnreg
        ' On tire un nombre compris entre 0 et iNbMotsExistantsFiltres - 1 (liste filtrée)
        Dim iNumMotExistantTire = iRandomiser(0, iNbMotsExistantsFiltres - 1)
        motAutre = enreg(iNumMotExistantTire).Value
        If IsNothing(motAutre) Then
            If bDebug Then Stop
            Return iTirageImpossible
        End If
        ' Indice du mot dans la liste complète
        Dim iNumMotExistantAutre% = motAutre.iNumMotExistant

        Const bDebugTirage As Boolean = False
        If bDebugTirage Then
            Dim mot = m_dicoMotsExistants.Values(iNumMotExistant)
            Debug.WriteLine("Mot choisi : " & mot.ToString)
            Debug.WriteLine("Mot autre : " & motAutre.ToString)
            Dim iNum% = 0
            For Each iNumMotE In lstNumMotExistant
                iNum += 1
                Dim motE = m_dicoMotsExistants.Values(iNumMotE)
                Debug.WriteLine("Mot autre n° " & iNum & " : " & motE.ToString)
            Next
        End If

        lstNumMotExistant.Add(iNumMotExistantAutre)
        itPrefixe.lstSegmentsDejaTires.Add(motAutre.sPrefixe)
        itSuffixe.lstSegmentsDejaTires.Add(motAutre.sSuffixe)
        itPrefixe.lstSensSegmentDejaTires.Add(motAutre.sDefPrefixe)
        itSuffixe.lstSensSegmentDejaTires.Add(motAutre.sDefSuffixe)
        itPrefixe.lstUnicitesSegmentDejaTires.Add(motAutre.sUnicitePrefixeSynth)
        itSuffixe.lstUnicitesSegmentDejaTires.Add(motAutre.sUniciteSuffixeSynth)

        Return iNumMotExistant

    End Function

#End Region

    Public Sub InitBases()
        m_prefixes = New clsBase(iNbColonnes, bPrefixe:=True)
        m_suffixes = New clsBase(iNbColonnes, bPrefixe:=False)
        m_defFls = New clsDefExclusives
    End Sub

#Region "Utile"

    Public Function bTrouverSegment(sSegment$, bSuffixe As Boolean, ByRef iNumSegmentTrouve%) _
        As Boolean

        ' Trouver le segment demandé

        If bSuffixe Then
            Return m_suffixes.bTrouverSegment(sSegment, iNumSegmentTrouve)
        Else
            Return m_prefixes.bTrouverSegment(sSegment, iNumSegmentTrouve)
        End If

    End Function

    Public Function sCompleterPrefixe$(sSensPrefixeOrig$)

        ' Correction de L', LE et LES en DE L', DU et DES
        ' Ex.: L'AIR -> DE L'AIR, LE MÉTAL -> DU MÉTAL, LES IDÉES -> DES IDÉES

        If String.IsNullOrEmpty(sSensPrefixeOrig) Then Return ""

        If sLang <> enumLangue.Fr Then Return sSensPrefixeOrig

        Dim sSensPrefixe$ = sSensPrefixeOrig
        Dim iLongSensPrefixe% = sSensPrefixeOrig.Length
        If iLongSensPrefixe = 0 Then Return sSensPrefixe
        If sSensPrefixeOrig.Substring(0, 3) = "LE " Then
            sSensPrefixe = "DU " & sSensPrefixeOrig.Substring(3)
        ElseIf iLongSensPrefixe >= 4 AndAlso sSensPrefixeOrig.Substring(0, 4) = "LES " Then
            sSensPrefixe = "DES " & sSensPrefixeOrig.Substring(4)
        ElseIf sSensPrefixeOrig.Substring(0, 3) = "LA " OrElse
               sSensPrefixeOrig.Substring(0, 2) = "L'" Then
            sSensPrefixe = "DE " + sSensPrefixeOrig
        End If

        ' En cas de sens multiple, ex. : "histo" -> "la trame / le tissu" -> "de trame / du tissu"
        'Dim sSensPrefixe2 = sSensPrefixe. _
        '    Replace("/ LE", "/ DU"). _
        '    Replace("/ LES", "/ DES"). _
        '    Replace("/ LA ", "/ DE "). _
        '    Replace("/ L'", "/ DE ")
        Dim sSensPrefixe2 = sRemplacerCar(sSensPrefixe, "/")
        Dim sSensPrefixe3 = sRemplacerCar(sSensPrefixe2, "->")

        Return sSensPrefixe3

    End Function

    Private Function sRemplacerCar$(sTxt$, sCar$)

        If sLang <> enumLangue.Fr Then Return sTxt

        ' LA MAISON -> L'ÉCOLOGIE / L'ÉCONOMIE
        ' ->
        ' DE LA MAISON -> DE L'ÉCOLOGIE / DE L'ÉCONOMIE

        Dim sTxtCorr = sTxt.
            Replace(sCar & " LE ", sCar & " DU ").
            Replace(sCar & " LES ", sCar & " DES ").
            Replace(sCar & " LA ", sCar & " DE LA ").
            Replace(sCar & " L'É", sCar & " DE L'É").
            Replace(sCar & " L'", sCar & " DE L'")
        Return sTxtCorr

    End Function

    Public Function sSupprimerArticle$(sTxt$)

        ' Supprimer le, la, l'

        If sLang <> enumLangue.Fr Then Return sTxt

        Dim sTxtCorr1 = sSupprimerArticleInterm("les", sTxt)
        Dim sTxtCorr2 = sSupprimerArticleInterm("le", sTxtCorr1)
        Dim sTxtCorr3 = sSupprimerArticleInterm("la", sTxtCorr2)
        'Dim sTxtCorr4 = sSupprimerArticleInterm("l'", sTxtCorr3)
        Dim sTxtCorr4 = sTxtCorr3.Replace("l'", "")

        Return sTxtCorr4

    End Function

    Private Function sSupprimerArticleInterm$(sArticle$, sTxt$)

        Dim sTxtCorr1 = sTxt.Replace("/ " & sArticle, "/")
        Dim sTxtCorr2 = sTxtCorr1.Replace("-> " & sArticle, "->")
        Dim sTxtCorr3 = sTxtCorr2.Replace("," & sArticle, ",")
        ' 07/10/2018 En dernier
        If sTxtCorr3.StartsWith(sArticle & " ") Then
            Dim iLongArticle = sArticle.Length
            Dim sSubs$ = sTxtCorr3.Substring(iLongArticle + 1, sTxtCorr3.Length - iLongArticle - 1)
            Return sSubs
        End If
        Return sTxtCorr3

    End Function

#End Region

    Public Class clsInitTirage

        Public lstNumSegmentDejaTires As New List(Of Integer)
        Public lstSegmentsDejaTires As New List(Of String)
        Public lstSensSegmentDejaTires As New List(Of String)
        Public lstUnicitesSegmentDejaTires As New List(Of String)

        Public Sub New()
        End Sub
        Public Sub New(segment As clsSegmentBase)
            If segment Is Nothing Then Throw New ArgumentNullException("segment")
            lstNumSegmentDejaTires.Add(segment.iNumSegment)
            lstSegmentsDejaTires.Add(segment.sSegment)
            lstSensSegmentDejaTires.Add(segment.sSens)
            If segment.sUnicite.Length > 0 Then _
                lstUnicitesSegmentDejaTires.Add(segment.sUnicite)
        End Sub

    End Class

    Public Class clsSegmentBase

        Public sSegment$, sSens$, sLogotron$, sNiveau$, sEtym$, sUnicite$
        Public sUniciteSynth$
        Public sOrigine$ ' Origine étymologique : Latin, Grec, ...
        Public sFrequence$ ' Fréquence du segment dans la liste des mots existants (seulement les complets)
        Public iNiveau%, iNumSegment%
        Public bElision As Boolean, sSegmentElision$

        Public Function sAfficher$(bPrefixe As Boolean)
            Dim sTxt$ = ""
            If bPrefixe Then
                sTxt = Me.sSegment & "-"
            Else
                sTxt = "-" & Me.sSegment
            End If
            Dim sTxtComplement = sTxt & "(" & Me.sNiveau & ") : " & Me.sSens &
                ", origine : " & Me.sOrigine & ", fréquence : " & Me.sFrequence
            Return sTxtComplement
        End Function

    End Class

    Public Class clsBase

        Private m_iNbColonnes% = 0
        Private Const iColSegment% = 0
        Private Const iColPrefixe% = 0
        Private Const iColSuffixe% = 0
        Private Const iColSens% = 1
        Private Const iColLogotron% = 2
        Private Const iColNiveau% = 3
        Private Const iColEtym% = 4
        Private Const iColUnicite% = 5
        Private Const iColOrigine% = 6
        Private Const iColFrequence% = 7

        Private m_lstSegments As List(Of String)
        Private m_bPrefixe As Boolean

        Public Sub New(iNbColonnes%, bPrefixe As Boolean)
            m_lstSegments = New List(Of String)
            m_iNbColonnes = iNbColonnes
            m_bPrefixe = bPrefixe ' 01/05/2019
        End Sub

        Public Function iLireNbSegments%()

            If m_iNbColonnes = 0 Then
                If bDebug Then Stop
                Return 0
            End If
            Dim iNbSegments% = m_lstSegments.Count / m_iNbColonnes
            Return iNbSegments

        End Function

        Public Function iLireNbSegmentsUniques%(
            lstNiveaux As List(Of String), lstFreq As List(Of String),
            bGrecoLatin As Boolean, bNeoRigolo As Boolean)

            ' Retourner la liste de segments uniques selon le sens, avec les niveaux indiqués
            ' (sélection du Logotron)

            If lstNiveaux Is Nothing Then Throw New ArgumentNullException("lstNiveaux")

            Dim hsSegments As New HashSet(Of String)
            Dim iNbSegments% = iLireNbSegments()
            For i As Integer = 0 To iNbSegments - 1
                Dim segment As clsSegmentBase = Nothing
                If Not bLireSegment(i, segment) Then Continue For
                If Not lstNiveaux.Contains(segment.sNiveau) Then Continue For
                If Not IsNothing(lstFreq) AndAlso
                   Not lstFreq.Contains(segment.sFrequence) Then Continue For
                If segment.sLogotron <> sSelectLogotron Then Continue For

                If (bGrecoLatin OrElse Not bNeoRigolo) AndAlso
                    segment.sOrigine = enumOrigine.sNeologismeAmusant Then
                    'Debug.WriteLine("Segment non retenu : " & segment.sSegment & " : " & _
                    '   segment.sOrigine)
                    Continue For
                End If

                If bGrecoLatin AndAlso
                   Not (segment.sOrigine = enumOrigine.sGrec OrElse
                        segment.sOrigine = enumOrigine.sLatin OrElse
                        segment.sOrigine = enumOrigine.sGrecoLatin) Then
                    'Debug.WriteLine("Segment non gréco-latin : " & segment.sSegment & " : " & _
                    '    segment.sOrigine)
                    Continue For
                End If

                Dim sSensSansArticle$ = sSupprimerArticle(segment.sSens)
                Dim sCleUniciteSens$ = sSensSansArticle
                If segment.sUnicite.Length > 0 Then sCleUniciteSens = segment.sUnicite
                If Not hsSegments.Contains(sCleUniciteSens) Then hsSegments.Add(sCleUniciteSens)
            Next

            Return hsSegments.Count

        End Function

        Public Function iTirageSegment%(bComplet As Boolean,
            lstNiveaux As List(Of String), lstFreq As List(Of String),
            bGrecoLatin As Boolean, bNeoRigolo As Boolean)
            Return iTirageSegment(bComplet, lstNiveaux, lstFreq, New clsInitTirage,
                bGrecoLatin, bNeoRigolo)
        End Function

        Public Function iTirageSegment%(bComplet As Boolean,
            lstNiv As List(Of String), lstFreq As List(Of String),
            it As clsInitTirage, bGrecoLatin As Boolean, bNeoRigolo As Boolean)

            ' bComplet : tous les segments (y compris ceux du dictionnaire),
            '  ou sinon seulement ceux du Logotron
            ' lstNiveaux : combinaison des niveaux "1", "2" et/ou "3"
            ' lstNumSegmentDejaTires  : ne pas tirer à nouveau un segment déjà tiré
            '  (pour avoir un mot avec plusieurs préfixes distincts)
            ' lstSegmentDejaTires     : ne pas tirer à nouveau un segment déjà tiré
            '  (cette fois le segment doit être unique, dans le cas où des segments 
            '   seraient présents avec plusieurs sens)
            ' lstSensSegmentDejaTires : ne pas tirer à nouveau un sens déjà tiré
            ' lstUnicitesSegmentDejaTires : lié au champ unicité 
            '  (unicité explicite car le sens peut varier plus ou moins)
            ' bGrecoLatin : seulement les segments d'origine greco-latine,
            '  sinon les segments de toutes origines
            ' bNeoRigolo : inclure les néologismes amusants

            ' Il faut vérifier que le tirage est possible : compter qu'il y a 
            '  au moins 1 candidat, sinon boucle infinie dans le tirage

            ' 01/05/2019 Test élision :
            '((m_bPrefixe     AndAlso seg0.sSegment.EndsWith(sCarO)) OrElse
            ' (Not m_bPrefixe AndAlso seg0.sSegment.StartsWith(sCarO))) AndAlso

            Dim lst = ObtenirSegmentBases()
            Dim enreg = From seg0 In lst Where
                lstNiv.Contains(seg0.sNiveau) AndAlso
                lstFreq.Contains(seg0.sFrequence) AndAlso
                ((it.lstNumSegmentDejaTires Is Nothing) OrElse
                 Not it.lstNumSegmentDejaTires.Contains(seg0.iNumSegment)) AndAlso
                ((it.lstSegmentsDejaTires Is Nothing) OrElse
                 Not it.lstSegmentsDejaTires.Contains(seg0.sSegment)) AndAlso
                ((it.lstSensSegmentDejaTires Is Nothing) OrElse
                 Not it.lstSensSegmentDejaTires.Contains(seg0.sSens)) AndAlso
                ((it.lstUnicitesSegmentDejaTires Is Nothing) OrElse
                 Not it.lstUnicitesSegmentDejaTires.Contains(seg0.sUnicite)) AndAlso
                (bComplet OrElse seg0.sLogotron = sSelectLogotron) AndAlso
                ((Not bGrecoLatin AndAlso
                  (bNeoRigolo OrElse
                   seg0.sOrigine <> enumOrigine.sNeologismeAmusant)) OrElse
                 (bGrecoLatin AndAlso
                    (seg0.sOrigine = enumOrigine.sGrec OrElse
                     seg0.sOrigine = enumOrigine.sLatin OrElse
                     seg0.sOrigine = enumOrigine.sGrecoLatin)))
            Dim iNbEnreg% = enreg.Count
            If iNbEnreg = 0 Then
                VBMessageBox("Aucun élément ne correspond à la sélection : Tirage impossible !")
                Return iTirageImpossible
            End If

            Dim iNbSegmentsFilres% = iNbEnreg
            ' On tire un nombre compris entre 0 et iNbSegmentsFilres - 1 (liste filtrée)
            Dim iNumSegment2 = iRandomiser(0, iNbSegmentsFilres - 1)
            Dim seg = enreg(iNumSegment2)
            Dim iNumSegment = seg.iNumSegment ' Indice du segment dans la liste complète

            If (it.lstNumSegmentDejaTires IsNot Nothing) Then it.lstNumSegmentDejaTires.Add(iNumSegment)
            If (it.lstSegmentsDejaTires IsNot Nothing) Then it.lstSegmentsDejaTires.Add(seg.sSegment)
            If (it.lstSensSegmentDejaTires IsNot Nothing) Then it.lstSensSegmentDejaTires.Add(seg.sSens)
            If seg.sUnicite.Length > 0 AndAlso (it.lstUnicitesSegmentDejaTires IsNot Nothing) Then _
            it.lstUnicitesSegmentDejaTires.Add(seg.sUnicite)
            Return iNumSegment

        End Function

        Public Function sTrouverEtymologie$(sSegment$, sUniciteSynth$)

            Dim enreg = From seg0 In ObtenirSegmentBases() Where
            seg0.sSegment = sSegment AndAlso seg0.sUniciteSynth = sUniciteSynth
            Dim iNbEnreg% = enreg.Count
            If iNbEnreg = 0 Then Return ""
            Dim sEtym$ = ""
            For Each enr In enreg
                sEtym = enr.sEtym
                Exit For
            Next
            Return sEtym

        End Function

        Public Function lstSegmentsAutreOrigine(
            lstNiv As List(Of String),
            lstFreq As List(Of String),
            bNeoRigolo As Boolean) As List(Of clsSegmentBase)

            ' Lister tous les segments avec une autre origine,
            '  pour le niveau demandé et la fréquence demandée

            Dim enreg = From seg0 In ObtenirSegmentBases() Where
                lstNiv.Contains(seg0.sNiveau) AndAlso
                lstFreq.Contains(seg0.sFrequence) AndAlso
                (bNeoRigolo OrElse
                 seg0.sOrigine <> enumOrigine.sNeologismeAmusant) AndAlso
                (Not _
                    (seg0.sOrigine = enumOrigine.sGrec OrElse
                     seg0.sOrigine = enumOrigine.sLatin OrElse
                     seg0.sOrigine = enumOrigine.sGrecoLatin))
            Dim lst = enreg.ToList()
            Return lst

        End Function

        Public Sub DefinirSegments(segments As List(Of String), iNbColonnes%)

            m_lstSegments = segments
            m_iNbColonnes = iNbColonnes

        End Sub

        Public Function ObtenirSegments() As List(Of String)

            Return m_lstSegments

        End Function

        Public Function ObtenirSegmentBases() As List(Of clsSegmentBase)

            Dim lst As New List(Of clsSegmentBase)
            For i As Integer = 0 To iLireNbSegments() - 1
                Dim segment As clsSegmentBase = Nothing
                If bLireSegment(i, segment) Then lst.Add(segment)
            Next
            Return lst

        End Function

        Public Sub AjouterSegment(segment As clsSegmentBase)
            If segment Is Nothing Then Throw New ArgumentNullException("segment")
            m_lstSegments.Add(segment.sSegment)
            m_lstSegments.Add(segment.sSens)
            m_lstSegments.Add(segment.sLogotron)
            m_lstSegments.Add(segment.sNiveau)
            m_lstSegments.Add(segment.sEtym)
            m_lstSegments.Add(segment.sUnicite)
            m_lstSegments.Add(segment.sOrigine)
            m_lstSegments.Add(segment.sFrequence)
        End Sub

        Public Function bLireSegment(iNumSegmentL%, ByRef segment As clsSegmentBase) As Boolean

            segment = Nothing
            If m_iNbColonnes <= 0 Then Return False : If bDebug Then Stop
            If iNumSegmentL = iTirageImpossible Then Return False

            segment = New clsSegmentBase
            segment.iNumSegment = iNumSegmentL
            Dim iNumSegment% = iNumSegmentL * m_iNbColonnes

            segment.sSegment = m_lstSegments(iNumSegment + iColPrefixe)
            If bDebug AndAlso (segment.sSegment Is Nothing) Then Stop

            If m_iNbColonnes <= iColSens Then Return True
            segment.sSens = m_lstSegments(iNumSegment + iColSens)
            If bDebug AndAlso (segment.sSens Is Nothing) Then Stop

            If m_iNbColonnes <= iColLogotron Then Return True
            segment.sLogotron = m_lstSegments(iNumSegment + iColLogotron)
            If bDebug AndAlso (segment.sLogotron Is Nothing) Then Stop

            If m_iNbColonnes <= iColNiveau Then Return True
            segment.sNiveau = m_lstSegments(iNumSegment + iColNiveau)
            If bDebug AndAlso (segment.sNiveau Is Nothing) Then Stop

            segment.iNiveau = Integer.Parse(segment.sNiveau)
            If m_iNbColonnes <= iColEtym Then Return True
            segment.sEtym = m_lstSegments(iNumSegment + iColEtym)
            If bDebug AndAlso (segment.sEtym Is Nothing) Then Stop

            If m_iNbColonnes <= iColUnicite Then Return True
            segment.sUnicite = m_lstSegments(iNumSegment + iColUnicite)
            If bDebug AndAlso (segment.sUnicite Is Nothing) Then Stop

            If m_iNbColonnes <= iColOrigine Then Return True
            segment.sOrigine = m_lstSegments(iNumSegment + iColOrigine)
            If bDebug AndAlso (segment.sOrigine Is Nothing) Then Stop

            If m_iNbColonnes <= iColFrequence Then Return True
            segment.sFrequence = m_lstSegments(iNumSegment + iColFrequence)
            If bDebug AndAlso (segment.sFrequence Is Nothing) Then Stop

            Dim sSensSansArticle$ = sSupprimerArticle(segment.sSens)
            Dim sUniciteFinale$ = sSensSansArticle
            segment.sUniciteSynth = sUniciteFinale
            If segment.sUnicite.Length > 0 Then segment.sUniciteSynth = segment.sUnicite

            If m_iNbColonnes <= iNbColonnes Then Return True

            Return True

        End Function

        Public Function bTrouverSegment(sSegment$, ByRef iNumSegmentTrouve%) As Boolean

            ' Trouver le segment demandé

            Dim enreg = From seg0 In ObtenirSegmentBases() Where
            seg0.sSegment = sSegment Select seg0.iNumSegment
            Dim lst = enreg.ToList()
            For Each iNumSeg In lst
                iNumSegmentTrouve = iNumSeg
                Return True
            Next
            iNumSegmentTrouve = -1
            Return False

        End Function

    End Class

End Module