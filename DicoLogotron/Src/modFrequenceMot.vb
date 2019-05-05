
Imports System.Text

Module modFrequenceMot

Const iSeuilFreqMax% = 20 ' à partir de 20 utilisations du segment dans les mots : segment fréquent
Const iSeuilFreqMoy% = 5  ' Entre 5 et  19 utilisations du segment dans les mots : segment moyen (sinon rare)
Const iNbExemplesMax% = 3

Private Class clsSegmentStat ' Statistiques des préfixes ou suffixes
    Public sSegment$, iNbOccDicoFr%
    'Public iLongSegment% ' Pour trier les segments par longueur décroissante
    Public bPrefixe As Boolean ' Sinon suffixe
    Public lstMots As New List(Of String)
    ' Si le mot est formé complètement par un préfixe et un suffixe, alors ils sont composables
    ' (on va les lister en 1er dans le bilan, car ils sont plus intéressants pour former des mots)
    Public bComposable As Boolean ' bComposable est utilisé dans le tri
    'Public dicoSegManquant As DicoTri(Of String, clsSegmentManquant)
    Public lstDef As New List(Of String)
End Class

Public Sub CalculFrequenceMots(msgDelegue As clsMsgDelegue)

    Dim sCheminMots$ = Application.StartupPath & "\Doc\" & "Mots" & sLang & ".txt"
    If Not bFichierExiste(sCheminMots, bPrompt:=True) Then Exit Sub
    Dim asLignes$() = asLireFichier(sCheminMots)
    If IsNothing(asLignes) Then Exit Sub
    Dim iNumLigne% = 0
    Dim iNbLignes% = asLignes.GetUpperBound(0)

    Dim iNbPrefixesTot% = 0
    Dim iNbSuffixesTot% = 0 ' = iNbMots
    Dim dicoPrefixe As New DicoTri(Of String, clsSegmentStat) ' Décompte de chaque préfixe
    Dim dicoSuffixe As New DicoTri(Of String, clsSegmentStat) ' Décompte de chaque préfixe
    For Each sMotDico As String In asLignes

        iNumLigne += 1

        'If bDebug AndAlso iNumLigne > 10 Then Continue For

        If iNumLigne Mod 1000 = 0 OrElse iNumLigne = iNbLignes Then
            Dim rPC! = iNumLigne / iNbLignes
            Dim sPC$ = rPC.ToString(sFormatPC)
            msgDelegue.AfficherMsg(sPC)
            If msgDelegue.m_bAnnuler Then Exit For
        End If
        If iNumLigne < 3 Then Continue For
        If iNumLigne >= iNbLignes Then Continue For

        Dim asChamps$() = sMotDico.Split(New String() {" : "}, StringSplitOptions.None)
        Dim iNbChamps% = asChamps.GetUpperBound(0) + 1
        Dim sMot$, sDef$, sListeSegments$
        sMot = "" : sDef = "" : sListeSegments = ""
        If iNbChamps >= 0 Then sMot = asChamps(0).Trim
        If iNbChamps >= 1 Then sDef = asChamps(1).Trim
        If iNbChamps >= 2 Then
            sListeSegments = asChamps(2).Trim
            Dim asChamps2$() = sListeSegments.Split(New String() {" - "}, StringSplitOptions.None)
            Dim iNbChamps2% = asChamps2.GetUpperBound(0) + 1
            If iNbChamps2 < 2 Then Continue For
            Dim bComposable As Boolean = False
            If iNbChamps2 > 2 Then bComposable = True
            'Debug.WriteLine(sMot & " : liste : ")
            For i = 0 To iNbChamps2 - 1
                'Debug.WriteLine(asChamps2(i))
                Dim sSegment$ = asChamps2(i)
                ' 03/05/2019
                If bElision AndAlso sSegment.EndsWith(sCarElisionO) Then _
                    sSegment = sSegment.Replace(sCarElisionO, sCarO)
                Dim bSuffixe = False
                If i = iNbChamps2 - 1 Then bSuffixe = True
                Dim iNumSegment% = -1
                If bTrouverSegment(sSegment, bSuffixe, iNumSegment) Then
                    'Debug.WriteLine("Segment n°" & iNumSegment)
                    If bSuffixe Then
                        Dim suffixe As clsSegmentBase = Nothing
                        If m_suffixes.bLireSegment(iNumSegment, suffixe) Then
                            iNbSuffixesTot += 1
                            AjouterSegment(dicoSuffixe, suffixe.sSegment, suffixe.sSens, _
                                sMot, bPrefixe:=False, bComposable:=bComposable)
                        Else
                            Debug.WriteLine("Segment non trouvé : " & sSegment)
                            If bDebug Then Stop
                        End If
                    Else
                        Dim prefixe As clsSegmentBase = Nothing
                        If m_prefixes.bLireSegment(iNumSegment, prefixe) Then
                            iNbPrefixesTot += 1
                            AjouterSegment(dicoPrefixe, prefixe.sSegment, prefixe.sSens, _
                                sMot, bPrefixe:=True, bComposable:=bComposable)
                        Else
                            Debug.WriteLine("Segment non trouvé : " & sSegment)
                            If bDebug Then Stop
                        End If
                    End If
                Else
                    Debug.WriteLine("Segment non trouvé : " & sSegment & " : " & sMotDico)
                    If bDebug Then Stop
                End If
            Next
        End If

    Next

    'Dim iNbMots% = dicoSuffixe.Count
    Dim sbBilanCsv As New StringBuilder()
    sbBilanCsv.AppendLine("Prefixe;Segment;Segment;Occurrences;Frequence (%);Frequence;Definition;Exemples")
    Dim sbBilan As New StringBuilder
    Dim aPrefixesFrequents = dicoPrefixe.Trier( _
        "bComposable Desc, iNbOccDicoFr Desc, sSegment")
    sbBilan.AppendLine("")
    sbBilan.AppendLine("Préfixes fréquents (en 1er les composables) :")
    sbBilan.AppendLine("-------------------------------------------")
    Dim iNbPrefixesFrequents% = 0
    Dim iNbOccPrefixesTot% = 0
    For Each stat In aPrefixesFrequents
        iNbPrefixesFrequents += 1
        Dim sFrequence$ = enumFrequence.Absent
        If stat.iNbOccDicoFr >= iSeuilFreqMax Then
            sFrequence = enumFrequence.Frequent
        ElseIf stat.iNbOccDicoFr >= iSeuilFreqMoy Then
            sFrequence = enumFrequence.Moyen
        Else
            sFrequence = enumFrequence.Rare
        End If
        Dim rPC! = stat.iNbOccDicoFr / iNbPrefixesTot
        Dim sPC$ = rPC.ToString(sFormatPC4)
        iNbOccPrefixesTot += stat.iNbOccDicoFr
        Dim sDef$ = sListerTxt(stat.lstDef)
        Dim sEx$ = sListerTxt(stat.lstMots, iNbMax:=iNbExemplesMax)
        Dim sLigne$ = stat.iNbOccDicoFr & " (" & sPC & ") : " & _
            stat.sSegment & "- : " & sDef & " : " & sEx & "." & _
            " - " & sFrequence
        sbBilan.AppendLine(sLigne)
        sbBilanCsv.AppendLine("1;" & stat.sSegment & ";" & stat.sSegment & "-;" &
            stat.iNbOccDicoFr & ";" & sPC & ";" & sFrequence & ";" & sDef & ";" & sEx & ".")
    Next
    sbBilan.AppendLine("-------------------------------------------")
    sbBilan.AppendLine("Nombre de préfixes : " & iNbPrefixesFrequents)
    Dim rPCPrefTot! = iNbOccPrefixesTot / iNbPrefixesTot
    sbBilan.AppendLine("Nb. total d'occurrences = " & iNbOccPrefixesTot & " / " & _
        iNbPrefixesTot & " = " & rPCPrefTot.ToString(sFormatPC2))

    Dim aSuffixesFrequents = dicoSuffixe.Trier( _
        "bComposable Desc, iNbOccDicoFr Desc, sSegment")
    sbBilan.AppendLine("")
    sbBilan.AppendLine("Suffixes fréquents (en 1er les composables) :")
    sbBilan.AppendLine("-------------------------------------------")
    Dim iNbSuffixesFrequents% = 0
    Dim iNbOccSuffixesTot% = 0
    For Each stat In aSuffixesFrequents
        iNbSuffixesFrequents += 1
        Dim sFrequence$ = enumFrequence.Absent
        If stat.iNbOccDicoFr >= iSeuilFreqMax Then
            sFrequence = enumFrequence.Frequent
        ElseIf stat.iNbOccDicoFr >= iSeuilFreqMoy Then
            sFrequence = enumFrequence.Moyen
        Else
            sFrequence = enumFrequence.Rare
        End If
        Dim rPC! = stat.iNbOccDicoFr / iNbSuffixesTot
        Dim sPC$ = rPC.ToString(sFormatPC4)
        iNbOccSuffixesTot += stat.iNbOccDicoFr
        Dim sDef$ = sListerTxt(stat.lstDef)
        Dim sEx$ = sListerTxt(stat.lstMots, iNbMax:=iNbExemplesMax)
        Dim sLigne$ = stat.iNbOccDicoFr & " (" & sPC & ") : " & _
            "-" & stat.sSegment & " : " & sDef & " : " & sEx & "." & _
            " - " & sFrequence
        sbBilan.AppendLine(sLigne)
        sbBilanCsv.AppendLine("0;" & stat.sSegment & ";-" & stat.sSegment & ";" &
            stat.iNbOccDicoFr & ";" & sPC & ";" & sFrequence & ";" & sDef & ";" & sEx & ".")
    Next
    sbBilan.AppendLine("-------------------------------------------")
    sbBilan.AppendLine("Nombre de suffixes : " & iNbSuffixesFrequents)
    Dim rPCSuffTot! = iNbOccSuffixesTot / iNbSuffixesTot
    sbBilan.AppendLine("Nb. total d'occurrences = " & iNbOccSuffixesTot & " / " & _
        iNbSuffixesTot & " = " & rPCSuffTot.ToString(sFormatPC2))

    Dim sCheminBilan$ = Application.StartupPath & "\Doc\" & "Stats" & sLang & ".txt"
    If Not bEcrireFichier(sCheminBilan, sbBilan) Then Exit Sub
    ProposerOuvrirFichier(sCheminBilan)
    Dim sCheminBilanCsv$ = Application.StartupPath & "\Doc\" & "Stats" & sLang & ".csv"
    If Not bEcrireFichier(sCheminBilanCsv, sbBilanCsv) Then Exit Sub

    'msgDelegue.AfficherMsg(sbBilan.ToString())

    msgDelegue.AfficherMsg("Terminé.")
End Sub

Private Sub AjouterSegment(dicoPrefSuff As DicoTri(Of String, clsSegmentStat), _
    sSegment$, sDef$, sMot$, bPrefixe As Boolean, bComposable As Boolean)

    'If sMot = "aérothermochimie" Then
    '    Debug.WriteLine(sSegment & " : bComposable = " & bComposable)
    'End If

    Dim psStat As clsSegmentStat
    If Not dicoPrefSuff.ContainsKey(sSegment) Then
        psStat = New clsSegmentStat
        psStat.sSegment = sSegment
        psStat.iNbOccDicoFr = 1
        psStat.bPrefixe = bPrefixe
        psStat.lstMots.Add(sMot)
        psStat.lstDef.Add(sDef)
        psStat.bComposable = bComposable
        dicoPrefSuff.Add(sSegment, psStat)
    Else
        psStat = dicoPrefSuff(sSegment)
        psStat.bPrefixe = bPrefixe ' Correction si à la fois préfixe et suffixe
        psStat.iNbOccDicoFr += 1

        ' ToDo : pas rapide : List.Contains :
        'If Not psStat.lstMots.Contains(sMot) Then psStat.lstMots.Add(sMot)
        psStat.lstMots.Add(sMot)

        ' ToDo : pas rapide : List.Contains :
        If Not psStat.lstDef.Contains(sDef) Then psStat.lstDef.Add(sDef)

        If bComposable AndAlso Not psStat.bComposable Then
            psStat.bComposable = bComposable
            ' Effacer les exemples ?
            'psStat.lstMots = New List(Of String)
            'psStat.lstMots.Add(sMotDicoUniforme)
        End If
    End If

End Sub

End Module
