
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
