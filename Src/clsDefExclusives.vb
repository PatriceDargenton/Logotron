
Public Class clsDefExclusives

    Private m_dicoDefExcl As Dictionary(Of String, String) ' Clé : sMot -> sSens exclusif
    Private m_dicoDefSegExcl As Dictionary(Of String, String) ' Clé : sSegment -> sSens exclusif

    Public Sub New()
        m_dicoDefExcl = New Dictionary(Of String, String)
        m_dicoDefSegExcl = New Dictionary(Of String, String)
    End Sub

    Public Sub AjouterListe(bPrefixe As Boolean, sSegment$, sSens$, sListeMotsExcl$)
        Dim asChamps2() = sListeMotsExcl.Split(",")
        Dim iNbChamps2% = asChamps2.GetUpperBound(0) + 1
        Dim sCle$ = "-" & sSegment
        If bPrefixe Then sCle = sSegment & "-"
        If Not m_dicoDefSegExcl.ContainsKey(sCle) Then m_dicoDefSegExcl.Add(sCle, sSens)
        For i As Integer = 0 To iNbChamps2 - 1
            Dim sMotExcl$ = asChamps2(i).Trim

            ' 12/07/2020
            Dim iNumPrefixe% = 1
            If sMotExcl.Contains(":") Then
                Dim asChps$() = sMotExcl.Split(":")
                sMotExcl = asChps(0)
                Dim sNumPrefixe$ = asChps(1).Trim
                iNumPrefixe = CInt(sNumPrefixe)
                Dim iNumPrefixe0% = 0
                If Integer.TryParse(sNumPrefixe, iNumPrefixe0) Then _
                    iNumPrefixe = iNumPrefixe0
            End If

            Dim sType$ = "S:"
            If bPrefixe Then
                sType = "P:"
                If iNumPrefixe > 1 Then sType = "P" & iNumPrefixe & ":" ' 12/07/2020
            End If
            Dim sCleMot$ = sType & sMotExcl
            If Not m_dicoDefExcl.ContainsKey(sCleMot) Then
                m_dicoDefExcl.Add(sCleMot, sSens)
            End If
        Next
    End Sub

    Public Function bSensExclusifAutre(sCleExcl$, sMotDico$, sSensSeg$,
        bPrefixe As Boolean, Optional iNumPrefixe% = 1) As Boolean

        Dim sType$ = "S:"
        If bPrefixe Then
            sType = "P:"
            If iNumPrefixe > 1 Then sType = "P" & iNumPrefixe & ":" ' 12/07/2020
        End If

        Dim sCleMot$ = sType & sMotDico ' 05/01/2018
        If m_dicoDefSegExcl.ContainsKey(sCleExcl) Then
            ' Le segment contient un sens spécifique à certains mots
            Dim sSensExclSeg$ = m_dicoDefSegExcl(sCleExcl)
            If Not m_dicoDefExcl.ContainsKey(sCleMot) Then
                ' Ce mot ne fait pas partie des exclusivités
                If sSensExclSeg = sSensSeg Then
                    ' Donc le sens exclusif doit être ignoré
                    'Debug.WriteLine("Exclusion : " & sMotDico & " : " & sSensSeg)
                    Return True
                End If
                ' Sens général : on accepte
            Else
                ' Ce mot fait partie des exclusivités
                Dim sSensExcl$ = m_dicoDefExcl(sCleMot)
                If sSensExcl <> sSensSeg Then
                    ' Sens général : on ignore
                    'Debug.WriteLine("Exclusion : " &
                    '    sMotDico & " : " & sSensSeg & " <> " & sSensExclMot)
                    Return True
                End If
                ' Sens exclusif pour un mot en exclusivité : on accepte
            End If
        End If
        Return False
    End Function

End Class
