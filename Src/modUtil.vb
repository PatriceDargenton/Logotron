
Imports System.Text ' Pour StringBuilder
Imports System.Text.Encoding ' Pour GetEncoding

Public Module modUtil

    Public Function rRandomiser!()

        Dim rRnd!
        Static rRndGenerateur As New Random
        Dim rRndDouble As Double = rRndGenerateur.NextDouble
        rRnd = CSng(rRndDouble)
        Return rRnd

    End Function

    Public Function iRandomiser%(ByVal iMin%, ByVal iMax%)

        ' La borne sup. est la borne max. possible, la borne min. est la borne min. possible

        Dim iRes As Integer = 0
        If iMin = iMax Then Return iMin

        Dim rRnd!
        Static rRndGenerateur As New Random
        Dim rRndDouble As Double = rRndGenerateur.NextDouble
        rRnd = CSng(rRndDouble)

        Dim rVal! = iMin + rRnd * (iMax + 1 - iMin)
        ' Fix : Partie entière sans arrondir à l'entier le plus proche
        iRes = iFix(rVal)
        ' Au cas où Rnd() renverrait 1.0 et qq
        If iRes > iMax Then iRes = iMax

        'Debug.WriteLine("Tirage entier entre " & iMin & " et " & iMax & " = " & iRes)

        Return iRes

    End Function

    Public Function iFix%(ByVal rVal!)

        ' Fix : Partie entière sans arrondir à l'entier le plus proche
        ' Pour les nombres négatifs, on enlève la partie décimale aussi
        ' Floor   arrondi les négatifs à l'entier le plus petit, tandis que
        ' Ceiling arrondi les négatifs à l'entier le plus grand (le plus petit en valeur absolu).
        ' Fix arrondi toujours à l'entier le plus petit en valeur absolu
        iFix = CInt(IIf(rVal >= 0, Math.Floor(rVal), Math.Ceiling(rVal)))

    End Function

    Public Function sEnleverAccents$(ByVal sChaine$, bTexteUnicode As Boolean, _
        Optional ByVal bMinuscule As Boolean = True)

        ' Enlever les accents

        If String.IsNullOrEmpty(sChaine) Then Return ""

        Const sEncodageIso8859_15$ = "iso-8859-15"
        Const sEncodageIso8859_8$ = "iso-8859-8"
        'Const sEncodageDest$ = "windows-1252"

        ' Frédéric François, cœur
        ' iso-8859-8   -> windows-1252 : Frederic Francois, cour ' Meilleure solution
        ' windows-1251 -> windows-1252 : Frederic Francois, c?ur ' Ancienne solution
        ' iso-8859-15  -> windows-1252 : Frédéric François, c½ur ' Utile pour détecter <>

        ' Codepage 1241 = "windows-1251" = cyrillic
        ' Tableau de caractères sur 8 bit
        'Dim aOctets As Byte() = GetEncoding(1251).GetBytes(sChaine)
        ' Chaîne de caractères sur 7 bit
        'sEnleverAccents = ASCII.GetString(aOctets) ' Ok mais reste cœur qui est converti en c?ur

        Dim iEncodageDest% = iCodePageWindowsLatin1252
        If bTexteUnicode Then iEncodageDest = iEncodageUnicodeUTF8
        Dim encodage1252 As Encoding = GetEncoding(iCodePageWindowsLatin1252)
        Dim encodage8859_8 As Encoding = GetEncoding(sEncodageIso8859_8)
        Dim encodageDest As Encoding = GetEncoding(iEncodageDest)
        Dim encodageIso8859_15 As Encoding = GetEncoding(sEncodageIso8859_15)

        Dim aOctets As Byte() = encodage8859_8.GetBytes(sChaine) ' "iso-8859-8"
        sEnleverAccents = encodageDest.GetString(aOctets)        ' 1252 ou UTF8
        'If bDebug Then Debug.WriteLine("' " & sEncodageSrc & " -> " & sEncodageDest & " : " & sEnleverAccents)

        ' Détection des caractères propres à iso-8859-15 : ¤ ¦ ¨ ´ ¸ ¼ ½ ¾ € Š š Ž ž Œ œ Ÿ
        ' http://fr.wikipedia.org/wiki/ISO_8859-15
        If String.Compare(encodageIso8859_15.GetString( _
            encodage1252.GetBytes(sChaine)), sChaine) = 0 Then GoTo Fin

        Dim i% = 0
        Dim iLen% = sChaine.Length
        Dim sChaineIso$ = encodageIso8859_15.GetString(encodageDest.GetBytes(sChaine))
        Dim ac1, ac2, ac3 As Char()
        ac1 = sChaine.ToCharArray
        ac2 = sChaineIso.ToCharArray
        ac3 = sEnleverAccents.ToCharArray
        Dim sbDest As New StringBuilder
        For i = 0 To iLen - 1
            If ac1(i) = ac2(i) Then
                sbDest.Append(ac3(i))
            Else
                Select Case ac1(i) ' ¤ ¦ ¨ ´ ¸ ¼ ½ ¾ € Š š Ž ž Œ œ Ÿ
                Case "¤"c : sbDest.Append("o")
                Case "¦"c : sbDest.Append("|")
                Case "¨"c : sbDest.Append("..")
                Case "´"c : sbDest.Append("'")
                Case "¸"c : sbDest.Append(",")
                Case "¼"c : sbDest.Append("1/4")
                Case "½"c : sbDest.Append("1/2")
                Case "¾"c : sbDest.Append("3/4")
                Case "€"c : sbDest.Append("E")
                Case "Š"c : sbDest.Append("S")
                Case "š"c : sbDest.Append("s")
                Case "Ž"c : sbDest.Append("Z")
                Case "ž"c : sbDest.Append("z")
                Case "œ"c : sbDest.Append("oe")
                Case "Œ"c : sbDest.Append("OE")
                Case "Ÿ"c : sbDest.Append("Y")
                Case Else
                    'If bDebug Then Debug.WriteLine("?? : " & ac1(i) & ac2(i) & ac3(i))
                    sbDest.Append(ac1(i)) ' 22/05/2010 Laisser le car. si non trouvé
                End Select
            End If
        Next i
        sEnleverAccents = sbDest.ToString

Fin:
        If bMinuscule Then sEnleverAccents = sEnleverAccents.ToLower

    End Function

    Public Sub AfficherMsgErreur(ex As Exception, _
        Optional sTitreFct$ = "", Optional sInfo$ = "", _
        Optional sDetailMsgErr$ = "", _
        Optional bCopierMsgPressePapier As Boolean = True, _
        Optional ByRef sMsgErrFinal$ = "")

        If Not Cursor.Current.Equals(Cursors.Default) Then _
            Cursor.Current = Cursors.Default
        Dim sMsg$ = ""
        If sTitreFct <> "" Then sMsg = "Fonction : " & sTitreFct
        If sInfo <> "" Then sMsg &= vbCrLf & sInfo
        If sDetailMsgErr <> "" Then sMsg &= vbCrLf & sDetailMsgErr
        If ex IsNot Nothing AndAlso Not String.IsNullOrEmpty(ex.Message) Then
            sMsg &= vbCrLf & ex.Message.Trim
            If ex.InnerException IsNot Nothing AndAlso _
               Not String.IsNullOrEmpty(ex.InnerException.Message) Then _
                sMsg &= vbCrLf & ex.InnerException.Message
        End If
        If bCopierMsgPressePapier Then bCopierPressePapier(sMsg)
        sMsgErrFinal = sMsg
        MsgBox(sMsg, MsgBoxStyle.Critical)

    End Sub

    Public Function bCopierPressePapier(sInfo$, Optional ByRef sMsgErr$ = "") As Boolean

        ' Copier des informations dans le presse-papier de Windows
        ' (elles resteront jusqu'à ce que l'application soit fermée)

        Try
            Dim dataObj As New DataObject
            dataObj.SetData(DataFormats.Text, sInfo)
            Clipboard.SetDataObject(dataObj)
            Return True
        Catch ex As Exception
            ' Le presse-papier peut être indisponible
            AfficherMsgErreur(ex, "CopierPressePapier", bCopierMsgPressePapier:=False, _
                sMsgErrFinal:=sMsgErr)
            Return False
        End Try

    End Function

    Public Sub AfficherMsgErreur2(ex As Exception, _
        Optional sTitreFct$ = "", Optional sInfo$ = "", _
        Optional sDetailMsgErr$ = "", _
        Optional bCopierMsgPressePapier As Boolean = True, _
        Optional ByRef sMsgErrFinal$ = "")

        If Not Cursor.Current.Equals(Cursors.Default) Then _
            Cursor.Current = Cursors.Default
        Dim sMsg$ = ""
        If sTitreFct <> "" Then sMsg = "Fonction : " & sTitreFct
        If sInfo <> "" Then sMsg &= vbCrLf & sInfo
        If sDetailMsgErr <> "" Then sMsg &= vbCrLf & sDetailMsgErr
        If Not ex Is Nothing AndAlso Not String.IsNullOrEmpty(ex.Message) Then
            sMsg &= vbCrLf & ex.Message.Trim
            If Not IsNothing(ex.InnerException) Then _
                sMsg &= vbCrLf & ex.InnerException.Message
        End If
        If bCopierMsgPressePapier Then bCopierPressePapier(sMsg)
        sMsgErrFinal = sMsg
        MsgBox(sMsg, MsgBoxStyle.Critical)

    End Sub

    Public Sub TraiterMsgSysteme_DoEvents()

        Application.DoEvents()

    End Sub

    Public Sub AfficherTexteListBox(sTxtOrig$, ByRef iIndexTxtLb%, frm As Form, lb As ListBox)

        ' Afficher un texte dans une ListBox en découpant au besoin en plusieurs lignes

        If frm Is Nothing Then Throw New ArgumentNullException("frm")
        If lb Is Nothing Then Throw New ArgumentNullException("lb")
        If String.IsNullOrEmpty(sTxtOrig) Then
            'Exit Sub
            GoTo Fin
        End If

        ' Pour cela il faut mesurer la largeur du texte affiché de façon précise
        Dim graphics = frm.CreateGraphics() ' En cas d'affichage intensif, optimiser ici
        Dim szTxtOrig = graphics.MeasureString(sTxtOrig, lb.Font)
        Const rFact! = 1.04! ' Selon la police utilisée, il faut appliquer un facteur (ou marge) de correction
        Dim rLargeurTxtOrig! = szTxtOrig.Width * rFact
        Dim rLargeurDispo! = lb.Width
        Dim rDiv! = lb.Width / rLargeurTxtOrig
        If rDiv < 1 Then

            ' Le texte ne loge pas, il faut le découper en lignes

            Dim iLongTot% = sTxtOrig.Length + 1 ' +1 pour le car. saut de ligne ajouté éventuel
            Dim rNbTroncons! = rLargeurTxtOrig / lb.Width
            Dim iLongMoyTroncon% = Math.Ceiling(iLongTot / rNbTroncons) ' Arrondi sup.
            'Dim iNbTroncons% = Math.Ceiling(rNbTroncons) ' Arrondi sup.

            Dim iTxtAff% = 0
            Dim sTxtFinVerif$ = ""
            Dim iNumTroncon% = 0
            Do

                Dim iNbCarEnTrop% = 1 ' Tenir compte du saut de ligne ↲
                Dim iLongTroncon% = iLongMoyTroncon
                Dim sTxtTroncon = ""
                Dim sTxtTronconVerif = ""
                Dim bFin As Boolean = False
                Do
                    Dim bAjoutCarSautDeLigne = True
                    Dim iLongRest% = iLongTroncon - iNbCarEnTrop
                    If iLongRest + iTxtAff >= iLongTot Then
                        iLongRest = iLongTot - iTxtAff - 1
                        bFin = True
                        bAjoutCarSautDeLigne = False ' Pas besoin sur la dernière ligne
                    End If
                    sTxtTroncon = sTxtOrig.Substring(iTxtAff, iLongRest)
                    sTxtTronconVerif = sTxtTroncon
                    If bAjoutCarSautDeLigne Then sTxtTroncon &= sCarSautDeLigne
                    Dim szTailleSubTxt = graphics.MeasureString(sTxtTroncon, lb.Font)
                    Dim rSubTxtLarg0! = szTailleSubTxt.Width * rFact
                    If rSubTxtLarg0 <= rLargeurDispo Then Exit Do
                    iNbCarEnTrop += 1
                    If iNbCarEnTrop > iLongTroncon Then Exit Do
                Loop While True

                lb.Items.Add(sTxtTroncon)
                sTxtFinVerif &= sTxtTronconVerif
                iIndexTxtLb += 1
                iTxtAff += iLongTroncon - iNbCarEnTrop

                iNumTroncon += 1
                If bFin OrElse sTxtFinVerif.Length >= iLongTot Then Exit Do

            Loop While True

            lb.SelectedIndex = iIndexTxtLb - 1
            If sTxtOrig <> sTxtFinVerif Then
                If bDebug Then Stop
            End If

        Else

Fin:
            lb.Items.Add(sTxtOrig)
            ' Pour sélectionner le dernier texte ajouté dans la listBox, il suffit
            '  de compter les lignes ajoutées
            lb.SelectedIndex = iIndexTxtLb
            iIndexTxtLb += 1

        End If

    End Sub

    Public Function sLireListBox$(lb As ListBox)

        Dim sb As New System.Text.StringBuilder
        For Each sTxt As String In lb.Items
            sb.AppendLine(sTxt)
        Next
        Dim sTxtFinal$ = sb.ToString
        Dim sTxt2$ = sTxtFinal.Replace(sCarSautDeLigne & vbCrLf, "")
        Return sTxt2

    End Function

    Public Sub RemplirListBoxAuHasard(lb As ListBox, lst As List(Of String))

        Dim iNbElements% = lst.Count
        Dim lstIndex As New List(Of Integer)
        Dim lstRnd As New List(Of String)
        For i As Integer = 0 To iNbElements - 1
Recom:
            Dim iNumElement% = iRandomiser(0, iNbElements - 1)
            If lstIndex.Contains(iNumElement) Then GoTo Recom
            lstIndex.Add(iNumElement)
            lstRnd.Add(lst(iNumElement))
        Next
        lb.Items.Clear()
        For Each sPref In lstRnd
            lb.Items.Add(sPref)
        Next

    End Sub

    Public Sub VBMessageBox(sMsg$)
        MsgBox(sMsg, MsgBoxStyle.Exclamation, m_sTitreMsg)
    End Sub

End Module
