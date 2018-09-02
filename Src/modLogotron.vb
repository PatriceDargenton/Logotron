
' Source : Jean-Pierre Petit, d'après une 1ère version pour Apple II
'  et transcrit d'après la source programmé en Javascript par Daniel Oddon
' https://www.jp-petit.org/Divers/LOGOTRON/logotron.HTM
' http://jeux.javascript.free.fr/logotron/logotron.htm

Imports System.Text

Module modLogotron

    'Public bDebugSegmentLettre1 As Boolean = bDebug
    'Public bDebugRacineNiveau As Boolean = False
    'Public bDebugSegmentNiveau As Boolean = bDebug
    'Public bDebugSensMultiple As Boolean = bDebug

    Private Class clsSensRacine
        Public sSens$ = ""
        Public sRacine$ = ""
        Public iNbRacines% = 0
        Public lstRacines As New List(Of String)
        'Public lstVariantesSens As New List(Of String) ' Racines multiples
        Public iSelectMax% ' 0 : non sélect., 1 : dico, 2 : Logotron
        Public iNiveau%
        Public sOrigine$ ' 28/08/2018
    End Class

    Private Class clsSensSegment ' Préfixe ou suffixe
        Public bPrefixe As Boolean ' Sinon suffixe
        Public sSensSegment$ ' Ex.: le son / son
        Public sSegmentUnique$
        Public sSegment$ ' Ex.: phono- ou phon- / -phone, -phonie, -phonique, -phonisme
        Public lstVariantes As New List(Of String) ' Ex.: -phone, -phonie, -phonique, -phonisme
        Public iSelectMax% ' 0 : non sélect., 1 : dico, 2 : Logotron
        Public iNiveau%
        Public lstSens As New List(Of String) ' Racines multiples
        Public sOrigine$ ' 28/08/2018
    End Class

    Private Class clsSegment
        Public bPrefixe As Boolean ' Sinon suffixe
        Public sSegmentUnique$
        Public sSegment$ ' Ex.: -mane
        Public sSens$ ' Presque tjrs 1 seul sens
        Public lstSens As New List(Of String) ' Ex.: -mane : main, maniaque
        Public lstSensM As New List(Of String) ' Racines multiples
        Public lstVariantes As New List(Of String) ' Ex.: -phone, -phonie, -phonique, -phonisme
        Public iSelectMax% ' 0 : non sélect., 1 : dico, 2 : Logotron
        Public iNiveau%
        Public sOrigine$ ' 28/08/2018
    End Class

    Public Sub LireLogotronCsv(sCheminLogotronCsv$)

        If Not bFichierExiste(sCheminLogotronCsv, bPrompt:=True) Then Exit Sub
        Dim asLignes$() = asLireFichier(sCheminLogotronCsv,
            bLectureSeule:=True, bUnicodeUTF8:=True)

        Dim iNumLigne% = 0
        Dim iNbLignes% = asLignes.Count
        For Each sLigne In asLignes
            iNumLigne += 1
            If iNumLigne = 1 Then Continue For ' Ignorer l'entête
            Dim asChamps() = sLigne.Split(";"c)
            Dim iNbChamps% = asChamps.GetUpperBound(0) + 1
            Dim sSelect$, sNiveau$, sSegment$, sPrefixe$, sSuffixe$, sSens$, sEtym$
            Dim sSegmentTiret$, sUnicite$, sOrigine$, sFrequence$, sListeMotsExcl$
            sSegmentTiret = "" : sUnicite = "" : sOrigine = "" : sFrequence = ""
            sSelect = "" : sNiveau = "" : sListeMotsExcl = ""
            sSegment = "" : sPrefixe = "" : sSuffixe = "" : sSens = "" : sEtym = ""
            Dim iNiveau% = 0
            'If iNbChamps >= 1 Then sLettre = asChamps(0)
            If iNbChamps >= 2 Then sSelect = asChamps(1)

            If sSelect = sNonSelectNum Then Continue For ' Ignorer les lignes avec 0

            ' Ignorer les lignes avec 1 (sauf si on veut la liste complète pour 
            '  l'analyse du dictionnaire français)
            'If Not bComplet AndAlso sSelect = "1" Then Continue For

            Dim iSelect% = iSelectDictionnaire
            Dim bSelectLogotron As Boolean = False
            If sSelect = sSelectLogotronNum Then _
                bSelectLogotron = True : iSelect = iSelectLogotron

            If iNbChamps >= 3 Then
                sNiveau = asChamps(2)
                iNiveau = Integer.Parse(sNiveau) ' 26/11/2017
            End If

            If iNbChamps >= 4 Then sSegment = asChamps(3)
            If iNbChamps >= 5 Then sPrefixe = asChamps(4)
            If iNbChamps >= 6 Then sSuffixe = asChamps(5)
            If iNbChamps >= 7 Then sSens = asChamps(6)
            If iNbChamps >= 8 Then sEtym = asChamps(7)
            If iNbChamps >= 9 Then sUnicite = asChamps(8)
            'If iNbChamps >= 10 Then sExplication = asChamps(9)
            'If iNbChamps >= 11 Then sExemple = asChamps(10)

            If iNbChamps >= 12 Then
                sOrigine = asChamps(11) ' 16/06/2018
                If sOrigine = enumOrigine.sGrec Then
                    ' Ok
                ElseIf sOrigine = enumOrigine.sLatin Then
                    ' Ok
                ElseIf String.IsNullOrEmpty(sOrigine) Then
                    sOrigine = enumOrigine.sDefaut ' Si non précisé, alors Greco-latin
                ElseIf sOrigine = enumOrigine.sNeologismeAmusant Then
                    If Not bInclureNeologismesAmusants Then Continue For
                Else
                    'sOrigine = enumOrigine.sAutre ' Sinon Autre origine (Anglais, ...)
                End If
            End If

            If iNbChamps >= 13 Then
                sFrequence = asChamps(12) ' 01/07/2018
                ' 23/08/2018
                If String.IsNullOrEmpty(sFrequence) Then sFrequence = enumFrequence.Defaut
            End If

            If iNbChamps >= 14 Then sListeMotsExcl = asChamps(13) ' 31/08/2018

            Dim suffixe As New clsSegmentBase
            Dim prefixe As New clsSegmentBase
            Dim bPrefixe As Boolean = False
            If sPrefixe.Length > 0 Then
                bPrefixe = True
                sSegmentTiret = sPrefixe
                ' N'enlever qu'un seul - à la fin (garder vice- avec tiret : vice-champion)
                If sPrefixe.EndsWith("-") Then sPrefixe = sPrefixe.Substring(0, sPrefixe.Length - 1)
                prefixe.sSegment = sPrefixe
                prefixe.sSens = sSens
                Dim sLogotronSrc$ = "D"
                If bSelectLogotron Then sLogotronSrc = "L"
                prefixe.sLogotron = sLogotronSrc
                prefixe.sNiveau = sNiveau
                prefixe.sEtym = sEtym
                prefixe.sUnicite = sUnicite
                prefixe.sOrigine = sOrigine
                prefixe.sFrequence = sFrequence
                m_prefixes.AjouterSegment(prefixe)
            ElseIf sSuffixe.Length > 0 Then
                sSegmentTiret = sSuffixe
                If sSuffixe.StartsWith("-") Then sSuffixe = sSuffixe.Substring(1, sSuffixe.Length - 1)
                suffixe.sSegment = sSuffixe
                suffixe.sSens = sSens
                Dim sLogotronSrc$ = "D"
                If bSelectLogotron Then sLogotronSrc = "L"
                suffixe.sLogotron = sLogotronSrc
                suffixe.sNiveau = sNiveau
                suffixe.sEtym = sEtym
                suffixe.sUnicite = sUnicite
                suffixe.sOrigine = sOrigine
                suffixe.sFrequence = sFrequence
                m_suffixes.AjouterSegment(suffixe)
            End If

            If sListeMotsExcl.Length > 0 Then
                m_defFls.AjouterListe(bPrefixe, sSegment, sSens, sListeMotsExcl)
            End If

            If sEtym.Length > 0 Then
                If sEtym.IndexOf(sGm) > -1 Then
                    MsgBox("Erreur : le signe " & sGm & " n'est pas autorisé ici : " & vbLf &
                        sSegment & " : " & sEtym, MsgBoxStyle.Information, m_sTitreMsg)
                End If
            End If

        Next

    End Sub

    Private m_bAfficherAvert As Boolean
    Private m_msgDelegue As clsMsgDelegue
    Public Sub TraiterEtExporterDonnees(bAfficherAvert As Boolean, msgDelegue As clsMsgDelegue)

        ' Exporter du format csv vers les formats code et json

        ' Exporter ssi la source de départ est csv
        If sModeLecture <> enumModeLecture.sCsv Then Exit Sub

        m_bAfficherAvert = bAfficherAvert
        m_msgDelegue = msgDelegue

        ' Version Logotron seule
        Dim sbLignesCodeSrc0, sbLignesPCodeSrc0, sbLignesSCodeSrc0 As New StringBuilder

        ' Version Complète
        Dim sbLignesCodeSrc, sbLignesPCodeSrc, sbLignesSCodeSrc As New StringBuilder

        ' Version avec étymologie, ...
        Dim sbLignesCodeSrc2, sbLignesPCodeSrc2, sbLignesSCodeSrc2 As New StringBuilder
        Dim sbLignesCodeSrcJSon As New StringBuilder

        sbLignesCodeSrcJSon.AppendLine("{")
        sbLignesCodeSrcJSon.AppendLine("    ""segments"": [")

        ' Racines uniques : préfixes ou suffixes
        ' (ex.: phono- et -phone sont issus de la même racine)
        Dim dicoRacines As New DicoTri(Of String, clsSensSegment)

        ' Segments uniques : préfixes uniques et suffiques uniques
        ' (ex.: phono- et phon- sont deux variantes du même segment)
        Dim dicoSegments As New DicoTri(Of String, clsSensSegment)

        Dim iNbLignes% = m_prefixes.iLireNbSegments()
        For i As Integer = 0 To iNbLignes - 1
            Dim prefixe As clsSegmentBase = Nothing
            m_prefixes.bLireSegment(i, prefixe)
            Dim sPrefixe = prefixe.sSegment
            Dim sSens = prefixe.sSens
            Dim iNiveau% = prefixe.iNiveau
            Dim sUnicite = prefixe.sUnicite
            Dim sOrigine = prefixe.sOrigine
            Dim sFrequence = prefixe.sFrequence
            Dim sSegment$ = sPrefixe
            Dim sSegmentTiret$ = sPrefixe & "-"
            Dim iSelect = iSelectDictionnaire
            Dim bSelectLogotron = False
            If prefixe.sLogotron = sSelectLogotron Then iSelect = iSelectLogotron : bSelectLogotron = True
            Const bPrefixe As Boolean = True
            DecompteSegment(sSegment, sSegmentTiret, bPrefixe,
                iSelect, sSens, sUnicite, dicoSegments, iNiveau, sOrigine)
            DecompteRacine(sSegment, sSegmentTiret, bPrefixe,
                iSelect, sSens, sUnicite, dicoRacines, iNiveau, sOrigine)

            Dim sLigneSrc$ = "            " & sGm & sPrefixe & sGm & ", " & sGm & sSens & sGm
            If bSelectLogotron Then
                If sbLignesPCodeSrc0.Length > 0 Then sbLignesPCodeSrc0.Append("," & vbCrLf)
                sbLignesPCodeSrc0.Append(sLigneSrc)
            End If
            If sbLignesPCodeSrc.Length > 0 Then sbLignesPCodeSrc.Append("," & vbCrLf)
            sbLignesPCodeSrc.Append(sLigneSrc)

            Dim sLogotronSrc$ = "D"
            If bSelectLogotron Then sLogotronSrc = "L"
            prefixe.sLogotron = sLogotronSrc
            sLigneSrc &= ", " & sGm & sLogotronSrc & sGm
            sLigneSrc &= ", " & sGm & iNiveau.ToString & sGm
            sLigneSrc &= ", " & sGm & prefixe.sEtym & sGm
            sLigneSrc &= ", " & sGm & sUnicite & sGm
            sLigneSrc &= ", " & sGm & sOrigine & sGm ' 16/06/2018
            sLigneSrc &= ", " & sGm & sFrequence & sGm ' 01/07/2018
            If sbLignesPCodeSrc2.Length > 0 Then sbLignesPCodeSrc2.Append("," & vbCrLf)
            sbLignesPCodeSrc2.Append(sLigneSrc)

            TraiterJSon(sbLignesCodeSrcJSon, bPrefixe, sPrefixe, bSelectLogotron,
                iNiveau.ToString, sSens, prefixe.sEtym, sUnicite,
                sOrigine, sFrequence, i, iNbLignes)

        Next
        iNbLignes = m_suffixes.iLireNbSegments()
        For i As Integer = 0 To iNbLignes - 1
            Dim suffixe As clsSegmentBase = Nothing
            m_suffixes.bLireSegment(i, suffixe)
            Dim sSuffixe = suffixe.sSegment
            Dim sSens = suffixe.sSens
            Dim iNiveau% = suffixe.iNiveau
            Dim sUnicite = suffixe.sUnicite
            Dim sOrigine = suffixe.sOrigine
            Dim sFrequence = suffixe.sFrequence
            Dim sSegment$ = sSuffixe
            Dim sSegmentTiret$ = "-" & sSuffixe
            Dim iSelect = iSelectDictionnaire
            Dim bSelectLogotron = False
            If suffixe.sLogotron = sSelectLogotron Then iSelect = iSelectLogotron : bSelectLogotron = True
            Const bPrefixe As Boolean = False
            DecompteSegment(sSegment, sSegmentTiret, bPrefixe,
                iSelect, sSens, sUnicite, dicoSegments, iNiveau, sOrigine)
            DecompteRacine(sSegment, sSegmentTiret, bPrefixe,
                iSelect, sSens, sUnicite, dicoRacines, iNiveau, sOrigine)

            Dim sLigneSrc$ = "            " & sGm & sSuffixe & sGm & ", " & sGm & sSens & sGm
            If bSelectLogotron Then
                If sbLignesSCodeSrc0.Length > 0 Then sbLignesSCodeSrc0.Append("," & vbCrLf)
                sbLignesSCodeSrc0.Append(sLigneSrc)
            End If
            If sbLignesSCodeSrc.Length > 0 Then sbLignesSCodeSrc.Append("," & vbCrLf)
            sbLignesSCodeSrc.Append(sLigneSrc)

            Dim sLogotronSrc$ = "D"
            If bSelectLogotron Then sLogotronSrc = "L"
            suffixe.sLogotron = sLogotronSrc
            sLigneSrc &= ", " & sGm & sLogotronSrc & sGm
            sLigneSrc &= ", " & sGm & iNiveau.ToString & sGm
            sLigneSrc &= ", " & sGm & suffixe.sEtym & sGm
            sLigneSrc &= ", " & sGm & sUnicite & sGm
            sLigneSrc &= ", " & sGm & sOrigine & sGm ' 16/06/2018
            sLigneSrc &= ", " & sGm & sFrequence & sGm ' 01/07/2018
            If sbLignesSCodeSrc2.Length > 0 Then sbLignesSCodeSrc2.Append("," & vbCrLf)
            sbLignesSCodeSrc2.Append(sLigneSrc)

            TraiterJSon(sbLignesCodeSrcJSon, bPrefixe, sSuffixe, bSelectLogotron,
                iNiveau.ToString, sSens, suffixe.sEtym, sUnicite,
                sOrigine, sFrequence, i, iNbLignes)

        Next
        CreerListeRacines(dicoRacines)
        CreerListeSegments(dicoSegments)

        sbLignesCodeSrc0.AppendLine("préfixes :")
        sbLignesCodeSrc0.Append(sbLignesPCodeSrc0).Append(vbCrLf)
        sbLignesCodeSrc0.AppendLine()
        sbLignesCodeSrc0.AppendLine("suffixes :")
        sbLignesCodeSrc0.Append(sbLignesSCodeSrc0)
        Dim sCheminLogotronTxt$ = Application.StartupPath & "\PrefixesSuffixes.txt"
        bEcrireFichier(sCheminLogotronTxt, sbLignesCodeSrc0, bEncodageUTF8:=True)

        sbLignesCodeSrc.AppendLine("préfixes :")
        sbLignesCodeSrc.Append(sbLignesPCodeSrc).Append(vbCrLf)
        sbLignesCodeSrc.AppendLine()
        sbLignesCodeSrc.AppendLine("suffixes :")
        sbLignesCodeSrc.Append(sbLignesSCodeSrc)
        sCheminLogotronTxt = Application.StartupPath & "\PrefixesSuffixesComplet.txt"
        bEcrireFichier(sCheminLogotronTxt, sbLignesCodeSrc, bEncodageUTF8:=True)

        sbLignesCodeSrc2.AppendLine("préfixes :")
        sbLignesCodeSrc2.Append(sbLignesPCodeSrc2).Append(vbCrLf)
        sbLignesCodeSrc2.AppendLine()
        sbLignesCodeSrc2.AppendLine("suffixes :")
        sbLignesCodeSrc2.Append(sbLignesSCodeSrc2)
        Dim sCheminLogotronTxt2$ = Application.StartupPath & "\PrefixesSuffixes2.txt"
        bEcrireFichier(sCheminLogotronTxt2, sbLignesCodeSrc2, bEncodageUTF8:=True)

        sbLignesCodeSrcJSon.AppendLine("]}")
        Dim sCheminLogotronJSon$ = Application.StartupPath & "\Logotron" & sLang & ".json"
        bEcrireFichier(sCheminLogotronJSon, sbLignesCodeSrcJSon, bEncodageUTF8:=True)

    End Sub

    Private Sub TraiterJSon(sbLignesCodeSrcJSon As StringBuilder, bPrefixe As Boolean,
        sSegment$, bSelectLogotron As Boolean,
        sNiveau$, sSens$, sEtym$, sUnicite$, sOrigine$, sFrequence$,
        iNumLigne%, iNbLignes%)

        sbLignesCodeSrcJSon.AppendLine("    {")
        Dim sSegmentJson$ = sSegment
        Dim sType$ = "suffixe"
        If bPrefixe Then sType = "préfixe" ': sSegmentJson = sSegment
        Dim sLogotron$ = "false"
        If bSelectLogotron Then sLogotron = "true"
        sbLignesCodeSrcJSon.AppendLine("        ""type"": """ & sType & """,")
        sbLignesCodeSrcJSon.AppendLine("        ""logotron"": " & sLogotron & ",")
        sbLignesCodeSrcJSon.AppendLine("        ""niveau"": " & sNiveau & ",")
        sbLignesCodeSrcJSon.AppendLine("        ""segment"": """ & sSegmentJson & """,")
        sbLignesCodeSrcJSon.AppendLine("        ""sens"": """ & sSens & """,")
        If sEtym.Length > 0 Then _
        sbLignesCodeSrcJSon.AppendLine("        ""étymologie"": """ & sEtym & """,")
        If sUnicite.Length > 0 Then _
        sbLignesCodeSrcJSon.AppendLine("        ""unicité"": """ & sUnicite & """,")
        If sOrigine.Length > 0 Then _
        sbLignesCodeSrcJSon.AppendLine("        ""origine"": """ & sOrigine & """,")
        If sFrequence.Length > 0 Then _
        sbLignesCodeSrcJSon.AppendLine("        ""fréquence"": """ & sFrequence & """,")
        sbLignesCodeSrcJSon.Append("    }")
        If iNumLigne < iNbLignes Then sbLignesCodeSrcJSon.Append(",")
        sbLignesCodeSrcJSon.AppendLine("")

    End Sub

    Public Class clsLogotronJson
        Public segments As clsLogotronSegmentJson()
        'Public segment As clsLogotronSegmentJson
    End Class

    Public Class clsLogotronSegmentJson
        Public type As String
        Public logotron As Boolean
        Public niveau As Integer
        Public segment As String
        Public sens As String
        Public étymologie As String
        Public unicité As String
        Public origine As String ' 16/06/2018
        Public fréquence As String ' 01/07/2018
    End Class

    Public Sub LireLogotronJSon()

        Const bMajListeCsv As Boolean = True

        Dim sCheminJson$ = Application.StartupPath & "\Logotron" & sLang & ".json"
        Dim aStr$() = asLireFichier(sCheminJson, bUnicodeUTF8:=True)
        Dim sb As New StringBuilder
        For Each sLigne In aStr
            sb.AppendLine(sLigne)
        Next
        Dim json$ = sb.ToString
        Dim lignes As clsLogotronJson
        Try
            lignes = Newtonsoft.Json.JsonConvert.DeserializeObject(Of clsLogotronJson)(json)
        Catch ex As Exception
            AfficherMsgErreur2(ex, "LireLogotronJSon")
            Exit Sub
        End Try

        ' Racines uniques : préfixes ou suffixes
        ' (ex.: phono- et -phone sont issus de la même racine)
        Dim dicoRacines As New DicoTri(Of String, clsSensSegment)

        ' Segments uniques : préfixes uniques et suffiques uniques
        ' (ex.: phono- et phon- sont deux variantes du même segment)
        Dim dicoSegments As New DicoTri(Of String, clsSensSegment)

        For Each seg In lignes.segments

            ' 21/06/2018
            If Not bInclureNeologismesAmusants AndAlso
               seg.origine = enumOrigine.sNeologismeAmusant Then
                Continue For
            End If

            Dim sSegmentTiret$ = ""
            Dim bPrefixe As Boolean = False
            If seg.type = "préfixe" Then
                bPrefixe = True
                sSegmentTiret = seg.segment & "-"
            Else
                sSegmentTiret = "-" & seg.segment
            End If
            Dim sSelect$ = sSelectDictionnaire
            Dim iSelect% = iSelectDictionnaire
            If seg.logotron Then iSelect = iSelectLogotron : sSelect = sSelectLogotron

            If IsNothing(seg.unicité) Then seg.unicité = ""
            'If IsNothing(seg.sens) Then seg.sens = ""
            If IsNothing(seg.étymologie) Then seg.étymologie = ""

            Dim suffixe As New clsSegmentBase
            Dim prefixe As New clsSegmentBase

            If bPrefixe Then
                prefixe.sSegment = seg.segment
                prefixe.sLogotron = sSelect
                prefixe.sNiveau = seg.niveau.ToString
                prefixe.sSens = seg.sens
                prefixe.sEtym = seg.étymologie
                prefixe.sUnicite = seg.unicité
                prefixe.sOrigine = seg.origine ' 16/06/2018
                prefixe.sFrequence = seg.fréquence ' 01/07/2018
                m_prefixes.AjouterSegment(prefixe)
            Else
                suffixe.sSegment = seg.segment
                suffixe.sLogotron = sSelect
                suffixe.sNiveau = seg.niveau.ToString
                suffixe.sSens = seg.sens
                suffixe.sEtym = seg.étymologie
                suffixe.sUnicite = seg.unicité
                suffixe.sOrigine = seg.origine ' 16/06/2018
                suffixe.sFrequence = seg.fréquence ' 01/07/2018
                m_suffixes.AjouterSegment(suffixe)
            End If

            If Not bMajListeCsv Then Continue For
            DecompteSegment(seg.segment, sSegmentTiret, bPrefixe,
                iSelect, seg.sens, seg.unicité, dicoSegments, seg.niveau, seg.origine)
            DecompteRacine(seg.segment, sSegmentTiret, bPrefixe,
                iSelect, seg.sens, seg.unicité, dicoRacines, seg.niveau, seg.origine)
        Next

        If Not bMajListeCsv Then Exit Sub
        CreerListeRacines(dicoRacines)
        CreerListeSegments(dicoSegments)

    End Sub

    Private Sub DecompteSegment(sSegment$, sSegmentTiret$, bPrefixe As Boolean,
        iSelect%, sSens$, sUnicite$, dicoSegments As DicoTri(Of String, clsSensSegment),
        iNiveau%, sOrigine$)

        Dim sSensSansArticle$ = sSupprimerArticle(sSens)
        Dim sSegmentUnique$ = sSegment
        Dim sCleUniciteSegment$ = bPrefixe & ":" & sSensSansArticle 'sSens 02/12/2017 sSens -> sSensSansArticle
        'Dim sCleUniciteSegment$ = bPrefixe & ":" & sSensSansArticle & ":" & sSegmentUnique ' 03/12/2017
        If sUnicite.Length > 0 Then
            sCleUniciteSegment = bPrefixe & ":" & sUnicite
            ' On corrige certains oublis (le chant), par contre, on duplique toutes les lignes
            '  pour lesquelles le sens est légèrement décliné (les variations avec agogie) :
            ' P;L;N3;mélos;mélo-;le membre; : manque "le chant"
            ' -> Ok (mélos)
            ' P;L;N3;mélos;mélo-;le membre;
            ' P;L;N2;mélos;mélo-;le chant;
            'sCleUniciteSegment = bPrefixe & ":" & sSensSansArticle & ":" & sUnicite ' 03/12/2017
            ' S;L;N2;agogie;-agogie;guider;-agogie, -agogique, -agogue : bien
            ' -> Non ! pas besoin ! (agogie)
            ' S;L;N2;agogie;-agogie;guider;
            ' S;D;N2;agogie;-agogique;guidage;
            ' S;L;N2;agogie;-agogue;meneur;
            ' S;L;N2;agogie;-agogue;conduction -> écoulement;
            ' Solution : préciser le sens dans l'unicité, si on veut les distinguer
            ' P;L;N2;mélos : chant;mélo-;le chant;
            ' S;L;N3;mélos : membre;-mèle;membre;-mèle, -mélie
            ' P;L;N3;mélos : membre;mélo-;le membre;
            sSegmentUnique = sUnicite
        End If
        If Not dicoSegments.ContainsKey(sCleUniciteSegment) Then
            Dim sensSeg As New clsSensSegment
            sensSeg.bPrefixe = bPrefixe
            sensSeg.sSegmentUnique = sSegmentUnique
            sensSeg.sSegment = sSegment
            sensSeg.sSensSegment = sSens ' On peut laisser l'article, car on ne mélange pas avec les préfixes
            'sensSeg.sSensSegment = sSensSansArticle 'sSens
            sensSeg.lstVariantes.Add(sSegmentTiret)
            sensSeg.iSelectMax = iSelect
            sensSeg.iNiveau = iNiveau
            sensSeg.sOrigine = sOrigine ' 28/08/2018
            dicoSegments.Add(sCleUniciteSegment, sensSeg)
        Else
            Dim sensSeg = dicoSegments(sCleUniciteSegment)
            Dim bExiste As Boolean = False
            For Each sVariante In sensSeg.lstVariantes
                If sVariante = sSegmentTiret Then bExiste = True : Exit For
            Next
            If Not bExiste Then
                sensSeg.lstVariantes.Add(sSegmentTiret)

                ' Si une variante de racine se termine par o
                '  et que le segment principal ne se termine pas par o
                '  alors préférer cette variante comme segment principal
                ' Ex.: métall- et métallo- : préférer métallo-
                If sSegment = sensSeg.sSegmentUnique AndAlso
                    sensSeg.sSegment <> sensSeg.sSegmentUnique Then
                    sensSeg.sSegment = sensSeg.sSegmentUnique
                End If

            End If

            ' Noter le niveau max. atteint par une variante
            ' (si aucune variante n'atteint le niveau 2,
            '  alors le segment reste au niveau 1)
            If iSelect > sensSeg.iSelectMax Then sensSeg.iSelectMax = iSelect

            If m_bAfficherAvert AndAlso iNiveau <> sensSeg.iNiveau Then
                m_msgDelegue.AfficherMsg("Segment : " & sSegmentUnique & " : " &
                    sensSeg.sSensSegment & " : Niveau " & iNiveau & " <> " & sensSeg.iNiveau)
            End If

        End If

    End Sub

    Private Sub DecompteRacine(sSegment$, sSegmentTiret$, bPrefixe As Boolean,
        iSelect%, sSens$, sUnicite$, dicoRacines As DicoTri(Of String, clsSensSegment),
        iNiveau%, sOrigine$)

        Dim sSensSansArticle$ = sSupprimerArticle(sSens)
        Dim sCleUniciteRacine$ = sSensSansArticle
        Dim sSegmentUnique$ = sSegment

        If sUnicite.Length > 0 Then
            sCleUniciteRacine = sUnicite
            sSegmentUnique = sUnicite
        End If

        If Not dicoRacines.ContainsKey(sCleUniciteRacine) Then
            Dim sensSeg As New clsSensSegment
            sensSeg.bPrefixe = bPrefixe
            sensSeg.sSegmentUnique = sSegmentUnique
            sensSeg.sSegment = sSegment
            sensSeg.sSensSegment = sSensSansArticle
            sensSeg.lstVariantes.Add(sSegmentTiret)
            sensSeg.iSelectMax = iSelect
            sensSeg.iNiveau = iNiveau
            sensSeg.lstSens.Add(sSensSansArticle) ' Racines multiples
            sensSeg.sOrigine = sOrigine ' 28/08/2018
            dicoRacines.Add(sCleUniciteRacine, sensSeg)
        Else
            Dim sensSeg = dicoRacines(sCleUniciteRacine)
            Dim bExiste As Boolean = False
            For Each sVariante In sensSeg.lstVariantes
                If sVariante = sSegmentTiret Then bExiste = True : Exit For
            Next
            If Not bExiste Then
                sensSeg.lstVariantes.Add(sSegmentTiret)
            End If

            ' Racines multiples
            bExiste = False
            For Each sSens0 In sensSeg.lstSens
                If sSens0 = sSensSansArticle Then bExiste = True : Exit For
            Next
            If Not bExiste Then
                sensSeg.lstSens.Add(sSensSansArticle)
            End If

            ' Noter le niveau max. atteint par une variante
            ' (si aucune variante n'atteint le niveau 2,
            '  alors le segment reste au niveau 1)
            If iSelect > sensSeg.iSelectMax Then sensSeg.iSelectMax = iSelect

            ' La complexité d'une racine est celle du minimum des préfixes et suffixes liés
            If iNiveau < sensSeg.iNiveau Then sensSeg.iNiveau = iNiveau

        End If

    End Sub

    Private Sub CreerListeRacines(dicoRacines As DicoTri(Of String, clsSensSegment))

        Dim dicoSensRacines As New DicoTri(Of String, clsSegment)
        For Each sensSeg In dicoRacines.Trier("sSegmentUnique, sSensSegment")
            Dim sCleUniciteSens$ = sensSeg.sSegmentUnique

            Dim seg As clsSegment

            If Not dicoSensRacines.ContainsKey(sCleUniciteSens) Then
                seg = New clsSegment
                seg.bPrefixe = sensSeg.bPrefixe
                seg.sSegmentUnique = sensSeg.sSegmentUnique
                seg.sSegment = sensSeg.sSegment
                seg.lstVariantes = sensSeg.lstVariantes
                seg.sSens = sensSeg.sSensSegment
                seg.lstSens.Add(sensSeg.sSensSegment)
                seg.lstSensM = sensSeg.lstSens.ToList ' Copie de la liste des sens
                seg.iSelectMax = sensSeg.iSelectMax
                seg.iNiveau = sensSeg.iNiveau
                seg.sOrigine = sensSeg.sOrigine ' 28/08/2018
                dicoSensRacines.Add(sCleUniciteSens, seg)
            Else

                ' Ajouter les sens associés à une racine, si ce n'est pas déjà fait
                seg = dicoSensRacines(sCleUniciteSens)
                Dim bExiste As Boolean = False
                For Each sSens In seg.lstSens
                    If sSens = sensSeg.sSensSegment Then bExiste = True : Exit For
                Next
                If Not bExiste Then
                    seg.lstSens.Add(sensSeg.sSensSegment)
                End If

                ' Racines multiples
                bExiste = False
                For Each sSens In seg.lstSensM
                    If sSens = sensSeg.sSensSegment Then bExiste = True : Exit For
                Next
                If Not bExiste Then
                    seg.lstSensM.Add(sensSeg.sSensSegment)
                End If

                ' Ajouter les variantes associées à une racine, si ce n'est pas déjà fait
                For Each sVariante In seg.lstVariantes
                    If Not sensSeg.lstVariantes.Contains(sVariante) Then
                        sensSeg.lstVariantes.Add(sVariante)
                    End If
                Next

            End If

            For Each sVariante In seg.lstVariantes
                ' Si une variante de racine se termine par o
                '  et que le segment principal ne se termine pas par o
                '  alors préférer cette variante comme segment principal
                ' Ex.: métall- et métallo- : préférer métallo-
                If sVariante.EndsWith("o-") AndAlso
                   Not seg.sSegmentUnique.EndsWith("o") AndAlso
                   seg.sSegmentUnique = seg.sSegment Then
                    seg.sSegmentUnique = sVariante.Substring(0, sVariante.Length - 1)
                    seg.sSegment = seg.sSegmentUnique
                End If
            Next

            ' Noter le niveau max. atteint par une variante
            ' (si aucune variante n'atteint le niveau 2,
            '  alors le segment reste au niveau 1)
            If sensSeg.iSelectMax > seg.iSelectMax Then seg.iSelectMax = sensSeg.iSelectMax

            ' La complexité d'un concept est celle du minimum des racines qui l'exprime
            If sensSeg.iNiveau < seg.iNiveau Then seg.iNiveau = sensSeg.iNiveau

            ' Pas besoin, car déjà signalé au niveau segment :
            'If bDebugRacineNiveau AndAlso sensSeg.iNiveau <> seg.iNiveau Then
            '    Debug.WriteLine("Racine : " & seg.sSegmentUnique & " : " &
            '        seg.sSens & " : Niveau " & sensSeg.iNiveau & " <> " & seg.iNiveau)
            'End If

        Next

        ' Sens avec des racines distinctes, pour info.
        Dim dicoSens As New DicoTri(Of String, clsSensRacine)

        Dim sb As New StringBuilder(
            "Sel.;Niv.;Racine;Sens;Déclinaisons et variantes;Origine" & vbCrLf)
        For Each sensSeg In dicoSensRacines.Trier("sSegmentUnique")

            Dim sLigne$ = ""

            'sLigne &= sensSeg.iSelectMax & ";"
            If sensSeg.iSelectMax = 1 Then
                sLigne &= sSelectDictionnaire & ";"
            Else
                sLigne &= sSelectLogotron & ";"
            End If

            sLigne &= "N" & sensSeg.iNiveau & ";" ' 26/11/2017
            sLigne &= sensSeg.sSegmentUnique & ";"

            Dim sSensFinal$ = ""
            Dim iNumSens% = 0
            sensSeg.lstSens.Sort() ' 08/04/2018
            For Each sSens In sensSeg.lstSens
                sSensFinal &= sSens
                iNumSens += 1
                If iNumSens < sensSeg.lstSens.Count Then sSensFinal &= ", "
                If m_bAfficherAvert AndAlso sensSeg.lstSens.Count > 1 Then _
                    m_msgDelegue.AfficherMsg("Sens racine multiple " &
                        sensSeg.sSegmentUnique & " : " & sSens)
            Next
            sLigne &= sSensFinal & ";"

            Dim sr As clsSensRacine
            If dicoSens.ContainsKey(sSensFinal) Then
                sr = dicoSens(sSensFinal)
                sr.lstRacines.Add(sensSeg.sSegmentUnique)
                sr.iNbRacines += 1
            Else
                sr = New clsSensRacine
                sr.sSens = sSensFinal
                sr.sRacine = sensSeg.sSegmentUnique
                sr.iNbRacines = 1
                'sr.lstVariantesSens = sensSeg.lstSensM.ToList ' Copie de la liste
                sr.iSelectMax = sensSeg.iSelectMax
                sr.iNiveau = sensSeg.iNiveau
                dicoSens.Add(sSensFinal, sr)
            End If
            ' Noter le niveau max. atteint (Logotron ou Dictionnaire)
            If sensSeg.iSelectMax > sr.iSelectMax Then sr.iSelectMax = sensSeg.iSelectMax
            ' La complexité d'un concept est celle du minimum des racines qui l'exprime
            If sensSeg.iNiveau < sr.iNiveau Then sr.iNiveau = sensSeg.iNiveau

            ' Racines multiples
            For Each sSensV In sensSeg.lstSensM
                If sSensV = sSensFinal Then Continue For
                If dicoSens.ContainsKey(sSensV) Then
                    Dim sr1 = dicoSens(sSensV)
                    sr1.lstRacines.Add(sensSeg.sSegmentUnique)
                    sr1.iNbRacines += 1

                    ' Noter le niveau max. atteint (Logotron ou Dictionnaire)
                    If sensSeg.iSelectMax > sr1.iSelectMax Then sr1.iSelectMax = sensSeg.iSelectMax
                    ' La complexité d'un concept est celle du minimum des racines qui l'exprime
                    If sensSeg.iNiveau < sr1.iNiveau Then sr1.iNiveau = sensSeg.iNiveau

                Else
                    Dim sr1 As New clsSensRacine
                    sr1.sSens = sSensV
                    sr1.sRacine = sensSeg.sSegmentUnique
                    sr1.iNbRacines = 1
                    sr1.iNiveau = sensSeg.iNiveau
                    sr1.iSelectMax = sensSeg.iSelectMax
                    dicoSens.Add(sSensV, sr1)
                End If
            Next

            If sensSeg.lstVariantes.Count > 1 Then
                Dim iNumVar% = 0
                Dim sSegment$ = sensSeg.sSegment
                Dim sCar1$ = sEnleverAccents(sSegment(0), bTexteUnicode:=False)
                sensSeg.lstVariantes.Sort() ' 07/01/2017
                For Each sVariante In sensSeg.lstVariantes
                    sLigne &= sVariante
                    iNumVar += 1
                    If iNumVar < sensSeg.lstVariantes.Count Then sLigne &= ", "

                    ' Afficher le rapport des préfixes d'une même racine qui ne commencent pas 
                    '  par la même lettre : erreur probable (et pareil pour les suffixes)
                    ' Exceptions (il s'agit bien de la même racine, exceptionnellement) :
                    ' kérato- <> cérato, de kéras (« corne ») dont le génitif est kératos.
                    ' -kinèse, kinési-, -kinésie <> cinèse, de kinêsis (« mouvement »).
                    ' -urgie, -urgique <> ergo, de érgon (« travail »).
                    ' -tude <> itude : tude est une variante de itude, du latin -tudo.
                    Dim sCar2$ = ""
                    If sVariante.StartsWith("-") Then
                        If sVariante.Length = 1 Then
                            If bDebug Then Stop
                        Else
                            sCar2 = sVariante(1)
                        End If
                    Else
                        sCar2 = sVariante(0)
                    End If
                    sCar2 = sEnleverAccents(sCar2, bTexteUnicode:=False)
                    If m_bAfficherAvert AndAlso iNumVar > 1 AndAlso sCar2 <> sCar1 Then
                        m_msgDelegue.AfficherMsg("Racines : Segment ne commençant pas par la même lettre : " &
                            sVariante & " <> " & sSegment & " : " & sensSeg.sSens)
                    End If

                Next
            Else
                If sensSeg.bPrefixe Then
                    sLigne &= sensSeg.sSegment & "-"
                Else
                    sLigne &= "-" & sensSeg.sSegment
                End If
            End If
            sLigne &= ";" & sensSeg.sOrigine ' 28/08/2018
            sb.AppendLine(sLigne)
        Next

        Dim sCheminsRacines$ = Application.StartupPath & "\Racines.csv"
        bEcrireFichier(sCheminsRacines, sb)

        ' Afficher le rapport des racines multiples : les sens avec des racines distinctes
        sb = New StringBuilder(
            "Racine;Sens;Racines différentes" & vbCrLf)
        Dim hsDiff As New HashSet(Of String)
        For Each racine In dicoSens.Trier("iNbRacines DESC, sRacine")
            If racine.iNbRacines = 1 Then Continue For
            Dim sLigne$ = racine.sRacine & ";" & racine.sSens & ";"
            Dim sRacinesDiff$ = ""
            racine.lstRacines.Sort() ' 18/11/2017
            For Each sRacineDiff In racine.lstRacines
                If sRacinesDiff.Length > 0 Then sRacinesDiff &= ", "
                sRacinesDiff &= sRacineDiff
            Next
            Dim sDiff = sRacinesDiff & " <> " & racine.sRacine
            If hsDiff.Contains(sDiff) Then Continue For
            sLigne &= sDiff
            sb.AppendLine(sLigne)
            hsDiff.Add(sDiff)
        Next
        Dim sCheminsRacinesMultiples$ = Application.StartupPath & "\RacinesMultiples.csv"
        bEcrireFichier(sCheminsRacinesMultiples, sb)

        ' Afficher le rapport des concepts distincts
        sb = New StringBuilder(
            "Sel.;Niv.;Concepts;Racines" & vbCrLf)
        Dim hsRacines As New HashSet(Of String)
        For Each racine In dicoSens.Trier("sSens")
            Dim sLigne$ = ""
            If racine.iSelectMax = 1 Then
                sLigne &= sSelectDictionnaire & ";"
            Else
                sLigne &= sSelectLogotron & ";"
            End If
            sLigne &= "N" & racine.iNiveau & ";"
            sLigne &= racine.sSens
            Dim sRacines$ = racine.sRacine
            For Each sRacine In racine.lstRacines
                'If sRacine = racine.sRacine Then Continue For
                If sRacines.Length > 0 Then sRacines &= ", "
                sRacines &= sRacine
            Next
            If hsRacines.Contains(sRacines) Then Continue For
            sLigne &= ";" & sRacines
            sb.AppendLine(sLigne)
            hsRacines.Add(sRacines)
        Next
        Dim sCheminsSens$ = Application.StartupPath & "\Concepts.csv"
        bEcrireFichier(sCheminsSens, sb)

    End Sub

    Private Sub CreerListeSegments(dicoSegments As DicoTri(Of String, clsSensSegment))

        Dim dicoSensSeg As New DicoTri(Of String, clsSegment)
        For Each sensSeg In dicoSegments.Trier("bPrefixe, sSegmentUnique, sSensSegment")
            Dim sSensSansArticle$ = sSupprimerArticle(sensSeg.sSensSegment)
            Dim sCleUniciteSens$ = sensSeg.bPrefixe & ":" & sSensSansArticle & ":" &
                sensSeg.sSegmentUnique ' 03/12/2017

            Dim seg As clsSegment
            If Not dicoSensSeg.ContainsKey(sCleUniciteSens) Then
                seg = New clsSegment
                seg.bPrefixe = sensSeg.bPrefixe
                seg.sSegmentUnique = sensSeg.sSegmentUnique
                seg.sSegment = sensSeg.sSegment
                seg.lstVariantes = sensSeg.lstVariantes
                seg.sSens = sensSeg.sSensSegment
                seg.lstSens.Add(sensSeg.sSensSegment)
                seg.iSelectMax = sensSeg.iSelectMax
                seg.iNiveau = sensSeg.iNiveau
                seg.sOrigine = sensSeg.sOrigine ' 28/08/2018
                dicoSensSeg.Add(sCleUniciteSens, seg)
            Else
                seg = dicoSensSeg(sCleUniciteSens)
                Dim bExiste As Boolean = False
                For Each sSens In seg.lstSens
                    If sSens = sensSeg.sSensSegment Then bExiste = True : Exit For
                Next
                If Not bExiste Then
                    seg.lstSens.Add(sensSeg.sSensSegment)
                End If
                ' 03/12/2017 C'est l'inverse
                'For Each sVar1 In seg.lstVariantes
                '    If Not sensSeg.lstVariantes.Contains(sVar1) Then
                '        sensSeg.lstVariantes.Add(sVar1)
                '    End If
                'Next
                For Each sVar1 In sensSeg.lstVariantes
                    If Not seg.lstVariantes.Contains(sVar1) Then
                        seg.lstVariantes.Add(sVar1)
                    End If
                Next
            End If

            If sensSeg.bPrefixe Then
                For Each sVariante In seg.lstVariantes
                    ' Si une variante de racine se termine par o
                    '  et que le segment principal ne se termine pas par o
                    '  alors préférer cette variante comme segment principal
                    ' Ex.: métall- et métallo- : préférer métallo-
                    If sVariante.EndsWith("o-") AndAlso
                       Not seg.sSegmentUnique.EndsWith("o") AndAlso
                       seg.sSegmentUnique = seg.sSegment Then
                        seg.sSegmentUnique = sVariante.Substring(0, sVariante.Length - 1)
                        seg.sSegment = seg.sSegmentUnique
                    End If
                Next
            End If

            ' Noter le niveau max. atteint par une variante
            ' (si aucune variante n'atteint le niveau 2,
            '  alors le segment reste au niveau 1)
            If sensSeg.iSelectMax > seg.iSelectMax Then seg.iSelectMax = sensSeg.iSelectMax

            If m_bAfficherAvert AndAlso sensSeg.iNiveau <> seg.iNiveau Then
                If seg.lstSens.Count > 1 Then
                    For Each sSens In seg.lstSens
                        m_msgDelegue.AfficherMsg("Segment : " & seg.sSegmentUnique & " : " &
                            sSens & " : Niveau " & sensSeg.iNiveau & " <> " & seg.iNiveau)
                    Next
                    ' S'il y a plusieurs sens, il peut y avoir plusieurs niveaux
                    ' Ex.: mélo- pour mélodie : niveau 2, mais mélo- pour membre : niveau 3
                    'seg.iNiveau = sensSeg.iNiveau
                Else
                    m_msgDelegue.AfficherMsg("Segment : " & seg.sSegmentUnique & " : " &
                        seg.sSens & " : Niveau " & sensSeg.iNiveau & " <> " & seg.iNiveau)
                End If
            End If

        Next

        Dim sb As New StringBuilder(
            "P/S;Sel.;Niv.;Racine;Segment;Sens;Déclinaisons et variantes;Origine" & vbCrLf)
        Const sTri$ = "sSegmentUnique, bPrefixe, sSens"
        For Each sensSeg In dicoSensSeg.Trier(sTri)

            Dim sLigne$

            If sensSeg.bPrefixe Then
                sLigne = "P;"
            Else
                sLigne = "S;" ' Sinon Suffixe
            End If

            If sensSeg.iSelectMax = 1 Then
                sLigne &= sSelectDictionnaire & ";"
            Else
                sLigne &= sSelectLogotron & ";"
            End If

            sLigne &= "N" & sensSeg.iNiveau & ";" ' 26/11/2017
            sLigne &= sensSeg.sSegmentUnique & ";"

            If sensSeg.bPrefixe Then
                sLigne &= sensSeg.sSegment & "-"
            Else
                sLigne &= "-" & sensSeg.sSegment
            End If
            sLigne &= ";"

            Dim iNumSens% = 0
            For Each sSens In sensSeg.lstSens
                sLigne &= sSens
                iNumSens += 1
                If iNumSens < sensSeg.lstSens.Count Then sLigne &= ", "
                If m_bAfficherAvert AndAlso sensSeg.lstSens.Count > 1 Then _
                    m_msgDelegue.AfficherMsg("Sens segment multiple " &
                        sensSeg.sSegmentUnique & " : " & sSens)
            Next

            sLigne &= ";"
            If sensSeg.lstVariantes.Count > 1 Then
                Dim iNumVar% = 0
                sensSeg.lstVariantes.Sort() ' 18/11/2017
                For Each sVariante In sensSeg.lstVariantes
                    sLigne &= sVariante
                    iNumVar += 1
                    If iNumVar < sensSeg.lstVariantes.Count Then sLigne &= ", "
                Next
            End If
            sLigne &= ";" & sensSeg.sOrigine ' 28/08/2018
            sb.AppendLine(sLigne)
        Next
        Dim sCheminSegments$ = Application.StartupPath & "\Segments.csv"
        bEcrireFichier(sCheminSegments, sb)

    End Sub

    Public Function bTirage(bComplet As Boolean, sNbPrefixesSuccessifs$,
        lstNiv As List(Of String), lstFreq As List(Of String),
        bGrecoLatin As Boolean, bNeoRigolo As Boolean,
        ByRef sMot$, ByRef sExplication$, ByRef sDetail$,
        ByRef lstEtymFin As List(Of String)) As Boolean

        Dim lstEtym As New List(Of String)

        ' 5 préfixes successifs au maximum
        Dim iNbTiragesPrefixes% = 0

        If sNbPrefixesSuccessifs = sHasard Then

            iNbTiragesPrefixes = iRandomiser(1, 5)
            'If bDebug Then iNbTiragesPrefixes = 1
            ' Diminution de la probabilité de préfixes successifs
            Dim rProba! = 1.0!
            Select Case iNbTiragesPrefixes
                Case 1 : rProba = 1 ' Toujours accepté
                Case 2 : rProba = 1 / 2 ' Une fois sur 2
                Case 3 : rProba = 1 / 4 ' Une fois sur 4
                Case 4 : rProba = 1 / 8 ' Une fois sur 8
                Case 5 : rProba = 1 / 16 ' Une fois sur 16
                    'Case 3 : rProba = 1 / 3 ' Une fois sur 3
                    'Case 4 : rProba = 1 / 4 ' Une fois sur 4
                    'Case 5 : rProba = 1 / 5 ' Une fois sur 5
                    'Case 2 : rProba = 0.1 ' Une fois sur 10
                    'Case 2 : rProba = 0.2 ' Une fois sur 5
                    'Case 3 : rProba = 0.1 ' Une fois sur 10
                    'Case 3 : rProba = 0.05 ' Une fois sur 20
                    'Case 4 : rProba = 0.03
                    'Case 5 : rProba = 0.01 ' Une fois sur 100
            End Select
            If rProba < 1 Then
                Dim rTirage = rRandomiser()
                If rTirage > rProba Then iNbTiragesPrefixes = 1
            End If

        Else

            iNbTiragesPrefixes = Integer.Parse(sNbPrefixesSuccessifs)

        End If

        Dim sPrefixesMaj$ = ""
        Dim sSensPrefixesMaj$ = ""
        Dim sDetailPrefixesMaj$ = ""
        Dim itPref As New clsInitTirage()
        For i As Integer = 0 To iNbTiragesPrefixes - 1
            Dim iNumPrefixe% = m_prefixes.iTirageSegment(bComplet,
                lstNiv, lstFreq, itPref, bGrecoLatin, bNeoRigolo)
            Dim prefixe As clsSegmentBase = Nothing
            If Not m_prefixes.bLireSegment(iNumPrefixe, prefixe) Then Return False
            Dim sNiveauP = prefixe.sNiveau
            Dim sPrefixe = prefixe.sSegment
            Dim sPrefixeMaj = sPrefixe.ToUpper()
            Dim sSensPrefixeMaj = prefixe.sSens.ToUpper()
            sSensPrefixeMaj = sCompleterPrefixe(sSensPrefixeMaj)
            sPrefixesMaj &= sPrefixeMaj
            sSensPrefixesMaj &= " " & sSensPrefixeMaj
            sDetailPrefixesMaj &= sPrefixeMaj & "(" & sNiveauP & ") - "
            Dim sEtymPrefixe = prefixe.sEtym
            If sEtymPrefixe.Length > 0 Then lstEtym.Add(sPrefixe & "- : " & sEtymPrefixe)
        Next

        'Dim iNbSuffixes% = m_suffixes.iLireNbSegments
        Dim iNumSuffixe% = m_suffixes.iTirageSegment(bComplet, lstNiv, lstFreq,
             New clsInitTirage, bGrecoLatin, bNeoRigolo)
        Dim suffixe As clsSegmentBase = Nothing
        If Not m_suffixes.bLireSegment(iNumSuffixe, suffixe) Then Return False
        Dim sNiveauS = suffixe.sNiveau
        Dim sSuffixe = suffixe.sSegment
        Dim sSuffixeMaj = sSuffixe.ToUpper()
        Dim sDetailSuffixeMaj = sSuffixeMaj & "(" & sNiveauS & ")"
        Dim sSensSuffixeMaj = suffixe.sSens.ToUpper()

        sMot = sPrefixesMaj & sSuffixeMaj
        sExplication = sSensSuffixeMaj & sSensPrefixesMaj
        sDetail = sDetailPrefixesMaj & sDetailSuffixeMaj

        Dim sEtymSuffixe = suffixe.sEtym
        If sEtymSuffixe.Length > 0 Then lstEtym.Add("-" & sSuffixe & " : " & sEtymSuffixe)
        lstEtymFin = lstEtym
        Return True

    End Function

End Module
