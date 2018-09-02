
' Fichier modUtilFichier.vb : Module de gestion des fichiers
' -------------------------

Imports System.Text ' Pour StringBuilder
'Imports System.IO ' Pour Path, File, Directory...

Public Module modUtilFichier

Public Const sCauseErrPoss$ = _
    "Le fichier est peut-�tre prot�g� en �criture ou bien verrouill� par une autre application"
Public Const sCauseErrPossDossier$ = _
    "Le dossier est peut-�tre prot�g� en �criture" & vbLf & _
    "ou bien un fichier est verrouill� par une autre application"

' Le code page 1252 correspond � FileOpen de VB .NET, l'�quivalent en VB6 de
'  Open sCheminFichier For Input As #1
' Mettre & pour Long en DotNet1 et % pour Integer en DotNet2
Public Const iCodePageWindowsLatin1252% = 1252 ' windows-1252 = msoEncodingWestern

' L'encodage UTF-8 est le meilleur compromis encombrement/capacit� 
'  il permet l'encodage par exemple du grec, sans doubler la taille du texte
'(mais le d�codage est plus complexe en interne et les caract�res ne s'affichent
' pas bien dans les certains logiciels utilitaires comme WinDiff, 
' ni par exemple en csv pour Excel)
' http://fr.wikipedia.org/wiki/Unicode
' 65001 = Unicode UTF-8, 65000 = Unicode UTF-7
Public Const iEncodageUnicodeUTF8% = 65001

Public Const sEncodageISO_8859_1$ = "ISO-8859-1"

#Region "Gestion des fichiers"

'Public Function bChoisirFichier(ByRef sCheminFichier$, sFiltre$, sExtDef$, _
'    sTitre$, Optional sInitDir$ = "", _
'    Optional bDoitExister As Boolean = True, _
'    Optional bMultiselect As Boolean = False) As Boolean

'    ' Afficher une boite de dialogue pour choisir un fichier
'    ' Exemple de filtre : "|Fichiers texte (*.txt)|*.txt|Tous les fichiers (*.*)|*.*"
'    ' On peut indiquer le dossier initial via InitDir, ou bien via le chemin du fichier

'    Static bInit As Boolean = False

'    Dim ofd As New OpenFileDialog
'    With ofd
'        If Not bInit Then
'            bInit = True
'            If sInitDir.Length = 0 Then
'                If sCheminFichier.Length = 0 Then
'                    .InitialDirectory = Application.StartupPath
'                Else
'                    .InitialDirectory = IO.Path.GetDirectoryName(sCheminFichier)
'                End If
'            Else
'                .InitialDirectory = sInitDir
'            End If
'        End If
'        If Not String.IsNullOrEmpty(sCheminFichier) Then .FileName = sCheminFichier
'        .CheckFileExists = bDoitExister ' 14/10/2007
'        .DefaultExt = sExtDef
'        .Filter = sFiltre
'        .Multiselect = bMultiselect
'        .Title = sTitre
'        .ShowDialog()

'        If .FileName <> "" Then sCheminFichier = .FileName : Return True
'        Return False

'    End With

'End Function

Public Function bFichierExiste(sCheminFichier$, _
    Optional bPrompt As Boolean = False) As Boolean

    ' Retourne True si un fichier correspondant est trouv�
    ' Ne fonctionne pas avec un filtre, par ex. du type C:\*.txt
    Dim bFichierExiste0 As Boolean = IO.File.Exists(sCheminFichier)

    If Not bFichierExiste0 AndAlso bPrompt Then _
        MsgBox("Impossible de trouver le fichier :" & vbLf & sCheminFichier, _
            MsgBoxStyle.Critical, m_sTitreMsg & " - Fichier introuvable")

    Return bFichierExiste0

End Function

Public Function bFichierExisteFiltre(sCheminFiltre$, sFiltre$, _
    Optional bPrompt As Boolean = False) As Boolean

    ' Retourne True si au moins un fichier correspondant au filtre est trouv�
    '  dans le r�pertoire indiqu�, ex.: C:\Tmp avec *.txt
    Dim bFichierExisteFiltre0 As Boolean
    Dim di As New IO.DirectoryInfo(sCheminFiltre)
    If Not di.Exists Then bFichierExisteFiltre0 = False : GoTo Fin
    Dim afi As IO.FileInfo() = di.GetFiles(sFiltre) ' Liste des fichiers trouv�s
    Dim iNbFichiers% = afi.GetLength(0)
    bFichierExisteFiltre0 = (iNbFichiers > 0)

Fin:
    If Not bFichierExisteFiltre0 AndAlso bPrompt Then _
        MsgBox("Impossible de trouver des fichiers du type :" & vbLf & sCheminFiltre, _
            MsgBoxStyle.Critical, m_sTitreMsg & " - Fichiers introuvables")

    Return bFichierExisteFiltre0

End Function

Public Function bFichierExisteFiltre2(sCheminFiltre$, _
    Optional bPrompt As Boolean = False) As Boolean

    ' Retourner True si un fichier correspondant au filtre est trouv�
    '  Exemple de filtre : C:\Tmp\*.txt

    If String.IsNullOrEmpty(sCheminFiltre) Then Return False
    'bFichierExisteFiltre2 = (Len(Dir(sFiltre)) > 0)
    Dim sDossier$ = IO.Path.GetDirectoryName(sCheminFiltre)
    Dim sFiltre$ = IO.Path.GetFileName(sCheminFiltre)
    Return bFichierExisteFiltre(sDossier, sFiltre, bPrompt)

End Function

Public Function iNbFichiersFiltres%(sCheminDossier$, sFiltre$)

    ' Retourner le nombre de fichiers correspondants au filtre, par exemple : C:\ avec *.txt
    Dim di As New IO.DirectoryInfo(sCheminDossier)
    If Not di.Exists Then Return 0
    Dim fi As IO.FileInfo() = di.GetFiles(sFiltre) ' Tableau de FileInfo
    Return fi.GetLength(0)

End Function

Public Function bTrouverFichier(sChemin$, sFiltre$, ByRef sCheminFichierTrouve$, _
    Optional bPromptErr As Boolean = True) As Boolean

    ' Renvoyer le premier fichier correspondant au filtre

    sCheminFichierTrouve = ""
    If Not bDossierExiste(sChemin, bPromptErr) Then Return False

    Dim di As New IO.DirectoryInfo(sChemin)
    For Each fi As IO.FileInfo In di.GetFiles(sFiltre)
        sCheminFichierTrouve = sChemin & "\" & fi.Name
        Return True
    Next

    Return False

End Function

Public Function bCopierFichier(sCheminSrc$, sCheminDest$, _
    Optional bPromptErr As Boolean = True, _
    Optional bVerifierDate As Boolean = False) As Boolean

    If bVerifierDate Then
        If Not bFichierExiste(sCheminSrc, bPrompt:=True) Then Return False
        Dim dDateSrc As Date = IO.File.GetLastWriteTime(sCheminSrc)
        Dim lTailleSrc& = New IO.FileInfo(sCheminSrc).Length
        If bFichierExiste(sCheminDest) Then
            Dim dDateDest As Date = IO.File.GetLastWriteTime(sCheminDest)
            Dim lTailleDest& = New IO.FileInfo(sCheminDest).Length
            ' Si la date et la taille sont les m�mes, la copie est d�j� faite
            ' (la v�rification du hashcode serait plus s�r mais trop longue
            '  de toute fa�on : il serait alors plus rapide de tjrs recopier)
            If dDateSrc = dDateDest And lTailleSrc = lTailleDest Then Return True
            ' Sinon supprimer le fichier de destination
            If Not bSupprimerFichier(sCheminDest) Then Return False
        End If
    End If

    If Not bFichierExiste(sCheminSrc, bPromptErr) Then Return False
    'If bFichierExiste(sDest) Then ' D�j� v�rifi� dans bSupprimerFichier
        If Not bSupprimerFichier(sCheminDest, bPromptErr) Then Return False
    'End If
    Try
        ' Cette fonction vient du kernel32.dll : rien � optimiser
        IO.File.Copy(sCheminSrc, sCheminDest)
        Return True
    Catch ex As Exception
        If bPromptErr Then _
            AfficherMsgErreur2(ex, "bCopierFichier", _
                "Impossible de copier le fichier source :" & vbLf & _
                sCheminSrc & vbLf & "vers le fichier de destination :" & _
                vbLf & sCheminDest, sCauseErrPoss)
        Return False
    End Try

End Function

Public Function bCopierFichiers(sCheminSrc$, sFiltre$, sCheminDest$, _
    Optional bPromptErr As Boolean = True) As Boolean

    ' Copier tous les fichiers correspondants au filtre dans le r�pertoire de destination

    If Not bDossierExiste(sCheminSrc, bPromptErr) Then Return False
    If Not bDossierExiste(sCheminDest, bPromptErr) Then Return False

    Dim di As New IO.DirectoryInfo(sCheminSrc)
    For Each fi As IO.FileInfo In di.GetFiles(sFiltre)
        Dim sFichier$ = fi.Name
        Dim sSrc$ = sCheminSrc & "\" & sFichier
        Dim sDest$ = sCheminDest & "\" & sFichier
        If Not bCopierFichier(sSrc, sDest, bPromptErr) Then Return False
    Next

    Return True

End Function

Public Function bSupprimerFichier(sCheminFichier$, _
    Optional bPromptErr As Boolean = False) As Boolean

    ' V�rifier si le fichier existe
    If Not bFichierExiste(sCheminFichier) Then Return True

    If Not bFichierAccessible(sCheminFichier, _
        bPromptFermer:=bPromptErr, bPromptRetenter:=bPromptErr) Then Return False

    ' Supprimer le fichier
    Try
        IO.File.Delete(sCheminFichier)
        Return True

    Catch ex As Exception
        If bPromptErr Then _
            AfficherMsgErreur2(ex, "Impossible de supprimer le fichier :" & vbLf & sCheminFichier, sCauseErrPoss)
        'If bPromptErr Then _
        '    MsgBox("Impossible de supprimer le fichier :" & vbLf & _
        '        sCheminFichier & vbLf & _
        '        sCauseErrPoss, MsgBoxStyle.Critical, m_sTitreMsg)
        Return False
    End Try

End Function

Public Function bSupprimerFichiersFiltres(sCheminDossier$, sFiltre$, _
    Optional bPromptErr As Boolean = False) As Boolean

    ' Supprimer tous les fichiers correspondants au filtre, par exemple : C:\ avec *.txt
    ' Si le dossier n'existe pas, on consid�re que c'est un succ�s
    If Not bDossierExiste(sCheminDossier) Then Return True
    Dim asFichier$() = IO.Directory.GetFileSystemEntries(sCheminDossier, sFiltre)
    For Each sFichier As String In asFichier
        If Not bSupprimerFichier(sFichier, bPromptErr) Then Return False
    Next sFichier
    Return True

End Function

Public Function bRenommerFichier(sSrc$, sDest$, _
    Optional bConserverDest As Boolean = False) As Boolean

    ' Renommer ou d�placer un et un seul fichier

    If Not bFichierExiste(sSrc, bPrompt:=True) Then Return False

    If bConserverDest Then
        ' Cette option permet de conserver le fichier de destination s'il existe
        If bFichierExiste(sDest) Then
            ' Dans ce cas on supprime la source
            If Not bSupprimerFichier(sSrc, bPromptErr:=True) Then Return False
            Return True
        End If
    Else
        If Not bSupprimerFichier(sDest, bPromptErr:=True) Then Return False
    End If

    Try
        IO.File.Move(sSrc, sDest)
        Return True
    Catch ex As Exception
        AfficherMsgErreur2(ex, "bRenommerFichier", _
            "Impossible de renommer le fichier source :" & vbLf & _
            sSrc & vbLf & "vers le fichier de destination :" & vbLf & sDest, _
            sCauseErrPoss)
        Return False
    End Try

End Function

Public Function bDeplacerFichiers2(sSrc$, sDest$) As Boolean

    ' Renommer ou d�placer une arborescence de fichiers
    ' Attention : cette fonction n�cessite la suppression du dossier src 
    ' voir aussi modUtilLT.bDeplacerFichiers et bDeplacerFichiers3

    ' On pourrait faire plus rapide en d�placant les fichiers, mais tant pis
    ' Ex.: D�placer C:\Tmp\*.txt -> C:\Tmp2\
    ' Cette fonction est d�j� dans : modUtilFichierLT.vb

    Dim bStatut As Boolean, sListeErr$ = ""
    If Not bCopierArbo(sSrc, sDest, bStatut, sListeErr) Then Return False
    Dim sDossierSrc$ = IO.Path.GetPathRoot(sSrc)
    If Not bSupprimerDossier(sDossierSrc, bPromptErr:=True) Then Return False
    Return True

End Function

Public Function bDeplacerFichiers3(sCheminSrc$, sFiltre$, sCheminDest$, _
    Optional bConserverDest As Boolean = True, _
    Optional sExtDest$ = "", Optional sPrefixe$ = "") As Boolean

    ' D�placer tous les fichiers correspondants au filtre dans le r�pertoire de destination
    '  en v�rifiant s'ils existent d�j� : dans ce cas, conserver le fichier de destination
    '  (option par d�faut pour conserver la date)

    If Not bVerifierCreerDossier(sCheminDest) Then Return False

    Dim bChExt As Boolean = False
    If Not String.IsNullOrEmpty(sExtDest) Then bChExt = True

    Dim di As New IO.DirectoryInfo(sCheminSrc)
    Dim aFi As IO.FileInfo() = di.GetFiles(sFiltre) ' Liste des fichiers d'archives
    Dim iNbFichiers% = aFi.GetLength(0)
    Dim i%
    For i = 0 To iNbFichiers - 1
        Dim sFichier$ = IO.Path.GetFileName(aFi(i).Name)
        Dim sSrc$ = sCheminSrc & "\" & sFichier
        Dim sDest$ = sCheminDest & "\" & sFichier

        ' Option possible : changer l'extension du fichier
        '  par exemple rename *.csv *.txt
        If bChExt Then sDest = sCheminDest & "\" & sPrefixe & _
            IO.Path.GetFileNameWithoutExtension(sFichier) & sExtDest

        If Not bRenommerFichier(sSrc, sDest, bConserverDest) Then Return False
    Next i

    Return True

End Function

' Attribut pour �viter que l'IDE s'interrompt en cas d'exception
<System.Diagnostics.DebuggerStepThrough()> _
Public Function bFichierAccessible(sCheminFichier$, _
    Optional bPrompt As Boolean = False, _
    Optional bPromptFermer As Boolean = False, _
    Optional bInexistOk As Boolean = False, _
    Optional bPromptRetenter As Boolean = False, _
    Optional bLectureSeule As Boolean = False, _
    Optional bEcriture As Boolean = True) As Boolean

    ' V�rifier si un fichier est accessible en �criture (non verrouill� par Excel par exemple)
    ' bEcriture = True par d�faut (pour la r�trocompatibilit� de la fct bFichierAccessible)
    ' Nouveau : Simple lecture : Mettre bEcriture = False
    ' On conserve l'option bLectureSeule pour alerter qu'un fichier doit �tre ferm�
    '  par l'utilisateur (par exemple un classeur Excel ouvert)

Retenter:
    If bInexistOk Then
        ' Avec cette option, ne pas renvoyer Faux si le fichier n'existe pas
        ' Et ne pas alerter non plus
        If Not bFichierExiste(sCheminFichier) Then Return True
    Else
        If Not bFichierExiste(sCheminFichier, bPrompt) Then Return False
    End If

'Retenter:
    Dim reponse As MsgBoxResult = MsgBoxResult.Cancel
    Dim fs As IO.FileStream = Nothing
    Try
        ' Si Excel a verrouill� le fichier, une simple ouverture en lecture
        '  est permise � condition de pr�ciser aussi IO.FileShare.ReadWrite
        Dim mode As IO.FileMode = IO.FileMode.Open
        Dim access As IO.FileAccess = IO.FileAccess.ReadWrite
        If Not bEcriture Then access = IO.FileAccess.Read
        fs = New IO.FileStream(sCheminFichier, mode, access, IO.FileShare.ReadWrite)
        fs.Close()
        fs = Nothing
        Return True
    Catch ex As Exception
        Dim msgbs As MsgBoxStyle = MsgBoxStyle.Exclamation
        If bPrompt Then
            AfficherMsgErreur2(ex, "bFichierAccessible", _
                "Impossible d'acc�der au fichier :" & vbLf & _
                sCheminFichier, sCauseErrPoss)
        ElseIf bPromptFermer Then
            Dim sQuestion$ = ""
            If bPromptRetenter Then
                msgbs = msgbs Or MsgBoxStyle.RetryCancel
                sQuestion = vbLf & "Voulez-vous r�essayer ?"
            End If
            ' Attention : le fichier peut aussi �tre en lecture seule pour diverses raisons !
            ' Certains fichiers peuvent aussi �tre inaccessibles pour une simple lecture
            '  par ex. certains fichiers du dossier 
            '  \Documents and Settings\All Users\Application Data\Microsoft\Crypto\RSA\MachineKeys\
            If bLectureSeule Then
                ' Le verrouillage Excel peut ralentir une lecture ODBC,
                '  mais sinon la lecture directe n'est pas possible, m�me avec
                '  IO.FileMode.Open, IO.FileAccess.Read et IO.FileShare.Read ?
                '  (sauf si le fichier a l'attribut lecture seule) 
                ' En fait si, � condition de pr�ciser IO.FileShare.ReadWrite
                reponse = MsgBox( _
                    "Veuillez fermer S.V.P. le fichier :" & vbLf & _
                    sCheminFichier & sQuestion, msgbs, m_sTitreMsg)
            Else
                reponse = MsgBox("Le fichier n'est pas accessible en �criture :" & vbLf & _
                    sCheminFichier & vbLf & _
                    "Le cas �ch�ant, veuillez le fermer, ou bien changer" & vbLf & _
                    "ses attributs de protection, ou alors les droits d'acc�s." & _
                    sQuestion, msgbs, m_sTitreMsg)
            End If
        End If
    Finally
        If fs IsNot Nothing Then fs.Dispose() ' CA2000
    End Try

    If reponse = MsgBoxResult.Retry Then GoTo Retenter
    Return False

End Function

' CA2122
<Security.Permissions.SecurityPermission(Security.Permissions.SecurityAction.LinkDemand)> _
Public Sub ProposerOuvrirFichier(sCheminFichier$, _
    Optional sInfo$ = "")

    If String.IsNullOrEmpty(sCheminFichier) Then Exit Sub
    If Not bFichierExiste(sCheminFichier, bPrompt:=True) Then Exit Sub
    Dim lTailleFichier& = (New IO.FileInfo(sCheminFichier)).Length
    Dim sTailleFichier$ = sFormaterTailleOctets(lTailleFichier)
    Dim sMsg$ = _
        "Le fichier " & IO.Path.GetFileName(sCheminFichier) & _
        " a �t� cr�� avec succ�s :" & vbLf & _
        sCheminFichier
    If Not String.IsNullOrEmpty(sInfo) Then sMsg &= vbLf & sInfo
    sMsg &= vbLf & "Voulez-vous l'afficher ? (" & sTailleFichier & ")"
    If MsgBoxResult.Cancel = MsgBox(sMsg, _
        MsgBoxStyle.Exclamation Or MsgBoxStyle.OkCancel, m_sTitreMsg) Then Exit Sub
    OuvrirAppliAssociee(sCheminFichier)

End Sub

' CA2122
<Security.Permissions.SecurityPermission(Security.Permissions.SecurityAction.LinkDemand)> _
Public Sub OuvrirAppliAssociee(sCheminFichier$, _
    Optional bMax As Boolean = False, _
    Optional bVerifierFichier As Boolean = True, _
    Optional sArguments$ = "")

    If bVerifierFichier Then
        ' Ne pas v�rifier le fichier si c'est une URL � lancer
        If Not bFichierExiste(sCheminFichier, bPrompt:=True) Then Exit Sub
    End If

    Using p As New Process
        p.StartInfo = New ProcessStartInfo(sCheminFichier)
        p.StartInfo.Arguments = sArguments
        ' Il faut indiquer le chemin de l'exe si on n'utilise pas le shell
        'p.StartInfo.UseShellExecute = False
        If bMax Then p.StartInfo.WindowStyle = ProcessWindowStyle.Maximized
        p.Start()
    End Using

End Sub

Public Function sFormaterTailleOctets$(lTailleOctets&, _
    Optional bDetail As Boolean = False, _
    Optional bSupprimerPt0 As Boolean = False)

    ' Renvoyer une taille de fichier bien format�e dans une cha�ne de caract�re
    ' Sinon il existe aussi l'API StrFormatByteSizeA dans shlwapi.dll

    ' 1024 est la norme actuellement employ�e dans Windows, 
    '  mais 1000 sera peut �tre un jour la norme
    ' http://fr.wikipedia.org/wiki/Octet

    Dim rNbKo! = CSng(Math.Round(lTailleOctets / 1024, 1))
    Dim rNbMo! = CSng(Math.Round(lTailleOctets / (1024 * 1024), 1))
    Dim rNbGo! = CSng(Math.Round(lTailleOctets / (1024 * 1024 * 1024), 1))
    Dim sAffichage$ = ""

    If bDetail Then
        sAffichage = sFormaterNumerique(lTailleOctets) & " octets"
        If rNbKo >= 1 Then sAffichage &= " (" & sFormaterNumerique(rNbKo) & " Ko"
        If rNbMo >= 1 Then sAffichage &= " = " & sFormaterNumerique(rNbMo) & " Mo"
        If rNbGo >= 1 Then sAffichage &= " = " & sFormaterNumerique(rNbGo) & " Go"
        If rNbKo >= 1 Or rNbMo >= 1 Or rNbGo >= 1 Then sAffichage &= ")"
    Else
        If rNbGo >= 1 Then
            sAffichage = sFormaterNumerique(rNbGo, bSupprimerPt0) & " Go"
        ElseIf rNbMo >= 1 Then
            sAffichage = sFormaterNumerique(rNbMo, bSupprimerPt0) & " Mo"
        ElseIf rNbKo >= 1 Then
            sAffichage = sFormaterNumerique(rNbKo, bSupprimerPt0) & " Ko"
        Else
            sAffichage = sFormaterNumerique(lTailleOctets, _
                bSupprimerPt0:=True) & " octets"
        End If
    End If

    sFormaterTailleOctets = sAffichage

End Function

Public Function sFormaterTailleKOctets$(lTailleOctets&, _
    Optional bSupprimerPt0 As Boolean = False)

    ' Renvoyer une taille de fichier en Ko bien format�e dans une cha�ne de caract�re
    ' La m�thode d'arrondie est la m�me que celle de l'explorateur de fichiers de Windows

    Dim rNbKo! = CSng(Math.Ceiling(lTailleOctets / 1024))
    sFormaterTailleKOctets = sFormaterNumerique(rNbKo, bSupprimerPt0) & " Ko"

End Function

Public Function sFormaterNumerique$(rVal!, _
    Optional bSupprimerPt0 As Boolean = True, _
    Optional iNbDecimales% = 1)

    ' Formater un num�rique avec une pr�cision d'une d�cimale

    ' Le format num�rique standard est correct (s�paration des milliers et plus), 
    '  il suffit juste d'enlever la d�cimale inutile si 0

    ' NumberGroupSeparator : S�parateur des milliers, millions...
    ' NumberDecimalSeparator : S�parateur d�cimal
    ' NumberGroupSizes : 3 groupes pour milliard, million et millier 
    '  (on pourrait en ajouter un 4�me pour les To : 1000 Go)
    ' NumberDecimalDigits : 1 d�cimale de pr�cision
    Dim nfi As New Globalization.NumberFormatInfo With {
        .NumberGroupSeparator = " ",
        .NumberDecimalSeparator = ".",
        .NumberGroupSizes = New Integer() {3, 3, 3},
        .NumberDecimalDigits = iNbDecimales
    }

    Dim sFormatage$ = rVal.ToString("n", nfi) ' n : num�rique g�n�ral
    ' Enlever la d�cimale si 0
    If bSupprimerPt0 Then
        If iNbDecimales = 1 Then
            sFormatage = sFormatage.Replace(".0", "")
        ElseIf iNbDecimales > 1 Then
            Dim i%
            Dim sb As New StringBuilder(".")
            For i = 1 To iNbDecimales : sb.Append("0") : Next
            sFormatage = sFormatage.Replace(sb.ToString, "")
        End If
    End If
    Return sFormatage

End Function

Public Function sFormaterNumerique2$(rVal!)

    ' Formater un num�rique selon le format choisi dans le panneau de config.
    ' Le format num�rique standard est correct (s�paration des milliers et plus), 
    '  il suffit juste d'enlever la d�cimale inutile si 0
    'sFormaterNumerique2 = rVal.ToString("n").Replace(".00", "") ' n : num�rique g�n�ral
    ' V�rifier , et . :
    Dim sVal$ = rVal.ToString("n")
    Dim sVal2$ = sVal.Replace(",00", "").Replace(".00", "") ' n : num�rique g�n�ral
    Return sVal2

End Function

Public Function sFormaterNumeriqueLong$(lVal&)

    ' Formater un num�rique selon le format choisi dans le panneau de config.
    ' Le format num�rique standard est correct (s�paration des milliers et plus), 
    '  il suffit juste d'enlever la d�cimale inutile si 0
    'sFormaterNumerique2 = rVal.ToString("n").Replace(".00", "") ' n : num�rique g�n�ral
    ' V�rifier , et . :
    Dim sVal$ = lVal.ToString("n")
    Dim sVal2$ = sVal.Replace(",00", "").Replace(".00", "") ' n : num�rique g�n�ral
    Return sVal2

End Function

#End Region

#Region "Gestion des dossiers"

Public Function bVerifierCreerDossier(sCheminDossier$, _
    Optional bPrompt As Boolean = True) As Boolean

    ' V�rifier si le dossier existe, et le cr�er sinon

    Dim di As New IO.DirectoryInfo(sCheminDossier)
    If di.Exists Then Return True

    Try

        di.Create()
        di = New IO.DirectoryInfo(sCheminDossier)
        Dim bExiste As Boolean = di.Exists
        Return bExiste

    Catch ex As Exception

        'If bPrompt Then _
        '    MsgBox("Impossible de cr�er le dossier :" & vbCrLf & _
        '        sCheminDossier & vbCrLf & ex.Message, _
        '        MsgBoxStyle.Critical, m_sTitreMsg)
        If bPrompt Then _
            AfficherMsgErreur2(ex, "bVerifierCreerDossier", _
                "Impossible de cr�er le dossier :" & vbCrLf & sCheminDossier)

        Return False

    End Try

End Function

Public Function bDossierExiste(sCheminDossier$, _
    Optional bPrompt As Boolean = False) As Boolean

    ' Retourne True si un dossier correspondant au filtre sFiltre est trouv�
    'Dim di As New IO.DirectoryInfo(sCheminDossier)
    'bDossierExiste = di.Exists()
    Dim bDossierExiste0 As Boolean = IO.Directory.Exists(sCheminDossier)

    If Not bDossierExiste0 AndAlso bPrompt Then _
        MsgBox("Impossible de trouver le dossier :" & vbLf & sCheminDossier, _
            MsgBoxStyle.Critical, m_sTitreMsg & " - Dossier introuvable")

    Return bDossierExiste0

End Function

Public Function bRenommerDossier(sCheminDossierSrc$, sCheminDossierDest$) As Boolean

    ' Renommer ou d�placer un et un seul dossier
    ' Idem bDeplacerDossier

    If Not bDossierExiste(sCheminDossierSrc, bPrompt:=True) Then Return False
    If Not bSupprimerDossier(sCheminDossierDest, bPromptErr:=True) Then Return False

    Try
        IO.Directory.Move(sCheminDossierSrc, sCheminDossierDest)
        Return True
    Catch ex As Exception
        AfficherMsgErreur2(ex, "bRenommerDossier", _
            "Impossible de renommer le dossier source :" & vbLf & _
            sCheminDossierSrc & vbLf & _
            "vers le dossier de destination :" & vbLf & sCheminDossierDest, _
            sCauseErrPossDossier)
        Return False
    End Try

End Function

Public Function bDeplacerDossier(sCheminDossierSrc$, sCheminDossierDest$, _
    Optional bPromptErr As Boolean = True) As Boolean

    ' Renommer ou d�placer un et un seul dossier
    ' Idem bRenommerDossier
    ' Roir aussi My.Computer.MoveDirectory en DotNet2

    If Not bDossierExiste(sCheminDossierSrc, bPrompt:=True) Then Return False
    If Not bSupprimerDossier(sCheminDossierDest, bPromptErr) Then Return False

    Try
        'Dim di As New IO.DirectoryInfo(sCheminDossierSrc)
        'di.MoveTo(sCheminDossierDest)
        IO.Directory.Move(sCheminDossierSrc, sCheminDossierDest)
        Return True
    Catch ex As Exception
        If bPromptErr Then _
            AfficherMsgErreur2(ex, "bDeplacerDossier", _
                "Impossible de d�placer le dossier source :" & vbLf & sCheminDossierSrc & vbLf & _
                "vers le dossier de destination :" & vbLf & sCheminDossierDest, _
                sCauseErrPossDossier)
        Return False
    End Try

End Function

Public Function bSupprimerDossier(sCheminDossier$, _
    Optional bPromptErr As Boolean = False) As Boolean

    ' V�rifier si le dossier existe
    If Not bDossierExiste(sCheminDossier) Then Return True

    Try
        IO.Directory.Delete(sCheminDossier, recursive:=True)

        ' Si l'explorateur est ouvert sur le dossier, il faut attendre qq sec.
        '  pour que le dossier soit bien d�truit
        Dim i% = 0
        While bDossierExiste(sCheminDossier) And i < 10
            'TraiterMsgSysteme_DoEvents()
            'Application.DoEvents()
            Threading.Thread.Sleep(1000)
            i += 1
        End While
        If i = 10 Then
            If bPromptErr Then _
                MsgBox("Impossible de supprimer le dossier :" & vbLf & _
                    sCheminDossier, MsgBoxStyle.Critical, m_sTitreMsg)
            Return False
        End If
        Return True

    Catch ex As Exception
        If bPromptErr Then _
            AfficherMsgErreur2(ex, "bSupprimerDossier", _
                "Impossible de supprimer le dossier :" & vbLf & _
                sCheminDossier, sCauseErrPossDossier)
        Return False
    End Try

End Function

Public Function sDossierParent$(sCheminDossier$)

    ' Renvoyer le chemin du dossier parent
    ' Ex.: C:\Tmp\Tmp2 -> C:\Tmp
    ' (� renommer plutot en sCheminDossierParent ?)
    ' Ex. avec un chemin de fichier
    '   C:\Tmp\MonFichier.txt -> C:\Tmp
    ' Ex. avec un chemin de fichier avec filtre
    '   C:\Tmp\*.txt -> C:\Tmp

    sDossierParent = IO.Path.GetDirectoryName(sCheminDossier)

End Function

Public Function sNomDossierFinal$(sCheminDossier$)

    ' Renvoyer le nom du dernier dossier � partir du chemin du dossier 
    ' Exemples :
    ' C:\Tmp\Tmp\MonDossier -> MonDossier
    ' C:\MonDossier\        -> MonDossier
    ' (si on passe un fichier en argument, alors c'est le fichier qui est renvoy�)

    sNomDossierFinal = sCheminDossier
    sCheminDossier = sEnleverSlashFinal(sCheminDossier)
    Dim iPosDossier% = sCheminDossier.LastIndexOf("\")
    If iPosDossier < 0 Then Exit Function
    sNomDossierFinal = sCheminDossier.Substring(iPosDossier + 1)

End Function

Public Function sExtraireChemin$(sCheminFichier$, _
    Optional ByRef sNomFichier$ = "", Optional ByRef sExtension$ = "", _
    Optional ByRef sNomFichierSansExt$ = "")

    ' Retourner le chemin du fichier pass� en argument
    ' Non compris le caract�re \
    ' Retourner aussi le nom du fichier sans le chemin ainsi que son extension
    ' Exemple :
    ' C:\Tmp\MonFichier.txt -> C:\Tmp, MonFichier.txt, .txt, MonFichier

    sExtraireChemin = IO.Path.GetDirectoryName(sCheminFichier)
    sNomFichier = IO.Path.GetFileName(sCheminFichier)
    sNomFichierSansExt = IO.Path.GetFileNameWithoutExtension(sCheminFichier)
    sExtension = IO.Path.GetExtension(sCheminFichier) '(avec le point, ex.: .txt)

End Function

Public Function sNomDossierParent$(sCheminDossierOuFichier$, _
    Optional sCheminReference$ = "")

    ' Renvoyer le nom du dernier dossier parent � partir du chemin du dossier 
    ' et renvoyer aussi le fichier avec si on passe le chemin complet du fichier
    '  sauf si le dossier parent n'existe pas : chemin de r�f�rence
    ' Exemples avec un dossier :
    ' C:\Tmp\Tmp\MonDossier     -> \Tmp\MonDossier
    ' C:\MonDossier             -> \MonDossier
    ' Exemples avec un fichier :
    ' C:\Tmp\Tmp\MonFichier.txt -> \Tmp\MonFichier.txt
    ' C:\MonFichier.txt         -> \MonFichier.txt

    If String.IsNullOrEmpty(sCheminDossierOuFichier) Then Return ""

    sNomDossierParent = ""
    Dim iPosDossier% = sCheminDossierOuFichier.LastIndexOf("\")
    If iPosDossier < 0 Then Exit Function
    sNomDossierParent = sCheminDossierOuFichier.Substring(iPosDossier)

    ' Si c'est le chemin de r�f�rence, on le renvoit tel quel
    Dim sCheminDossierParent$ = IO.Path.GetDirectoryName(sCheminDossierOuFichier)
    If sCheminDossierParent = sEnleverSlashFinal(sCheminReference) Then Exit Function

    Dim iFin% = iPosDossier - 1
    Dim iPosDossierParent% = sCheminDossierOuFichier.LastIndexOf("\", iFin)
    If iPosDossierParent < 0 Then Exit Function
    sNomDossierParent = sCheminDossierOuFichier.Substring(iPosDossierParent)

End Function

Public Function sCheminRelatif$(sCheminFichier$, sCheminReference$)

    ' Renvoyer le chemin relatif au chemin de r�f�rence
    '  � partir du chemin complet du fichier

    ' Exemples avec C:\ pour le chemin de r�f�rence
    ' C:\Tmp\Tmp\MonFichier.txt -> \Tmp\Tmp\MonFichier.txt
    ' C:\MonFichier.txt         -> \MonFichier.txt

    ' Exemple avec C:\Tmp1 pour le chemin de r�f�rence
    ' C:\Tmp1\Tmp2\MonFichier.txt -> \Tmp2\MonFichier.txt

    If String.IsNullOrEmpty(sCheminFichier) Then Return ""
    If String.IsNullOrEmpty(sCheminReference) Then Return ""

    sCheminReference = sEnleverSlashFinal(sCheminReference)
    sCheminRelatif = sCheminFichier.Substring(sCheminReference.Length)

End Function

Public Function sEnleverSlashFinal$(sChemin$)

    ' Enlever le slash final � la fin du chemin, le cas �ch�ant

    If String.IsNullOrEmpty(sChemin) Then Return ""

    If sChemin.EndsWith("\") Then
        Return sChemin.Substring(0, sChemin.Length - 1)
    Else
        Return sChemin
    End If

End Function

Public Function sEnleverSlashInitial$(sChemin$)

    ' Enlever le slash au d�but du chemin, le cas �ch�ant

    If String.IsNullOrEmpty(sChemin) Then Return ""

    If sChemin.StartsWith("\") Then
        Return sChemin.Substring(1)
    Else
        Return sChemin
    End If

End Function

Public Function bCopierArbo(sSrc$, sDest$, _
    ByRef bStatut As Boolean, ByRef sListeErr$, _
    Optional sListeErrExcep$ = "") As Boolean

    ' Copier une arborescence de fichiers
    '  en retournant bStatut : Succ�s ou Echec de la fonction r�cursive
    ' En cas d'�chec, la liste des fichiers n'ayant pu �tre copi�s est
    '  indiqu�e dans sListeErr (sListeErrExcep permet d'exclure des fichiers
    '  de ce rapport si on sait d�j� qu'on ne pourra les copier)

    ' Voir aussi : Zeta Folder XCOPY By Uwe Keim
    ' A small class to perform basic XCOPY like operations from within C# 
    ' http://www.codeproject.com/KB/recipes/ZetaFolderXCopy.aspx

    If String.IsNullOrEmpty(sSrc) Then Return False
    If String.IsNullOrEmpty(sDest) Then Return False

    If sDest.Chars(sDest.Length - 1) <> IO.Path.DirectorySeparatorChar Then _
        sDest &= IO.Path.DirectorySeparatorChar
    Try
        If Not IO.Directory.Exists(sDest) Then IO.Directory.CreateDirectory(sDest)
    Catch ex As Exception
        AfficherMsgErreur2(ex, "bCopierArbo", _
            "Impossible de cr�er le dossier :" & vbLf & _
            sDest, sCauseErrPossDossier)
        Return False
    End Try

    Dim aElements$() = IO.Directory.GetFileSystemEntries(sSrc)
    For Each sCheminElements As String In aElements
        Dim sNomElements$ = IO.Path.GetFileName(sCheminElements)
        If IO.Directory.Exists(sCheminElements) Then
            ' L'�lement est un sous-dossier : le copier
            bCopierArbo(sCheminElements, sDest & sNomElements, bStatut, _
                sListeErr, sListeErrExcep)
        Else
            ' Sinon copier le fichier
            Try
                IO.File.Copy(sCheminElements, sDest & sNomElements, overwrite:=True)
            Catch ex As Exception
                If Not String.IsNullOrEmpty(sListeErrExcep) AndAlso _
                   sListeErrExcep.IndexOf(" " & sNomElements & " ") = -1 Then
                    ' Noter le chemin du fichier imposs. � copier ssi pas exception
                    If sListeErr.Length < 200 Then
                        If sListeErr.Length = 0 Then
                            sListeErr = sDest & sNomElements
                        Else
                            sListeErr &= vbLf & sDest & sNomElements
                        End If
                    ElseIf Right$(sListeErr, 3) <> "..." Then
                        sListeErr &= vbLf & "..."
                    End If
                    bStatut = False ' Il y a des erreurs particuli�res
                End If
            End Try
        End If
    Next

    Return bStatut

End Function

Public Function sLecteurDossier$(sDossier$)

    ' Renvoyer le lecteur du dossier sans \ � la fin
    ' Exemple : C:\Tmp -> C:

    sLecteurDossier = sEnleverSlashFinal(IO.Path.GetPathRoot(sDossier))

End Function

' CA2122
<System.Security.Permissions.SecurityPermissionAttribute( _
    Security.Permissions.SecurityAction.LinkDemand)> _
Public Sub OuvrirDossier(sCheminDossier$)

    ' Ouvrir un dossier via l'explorateur de fichiers

    Using p As New Process

        ' Ne marche pas :
        'Dim sArg$ = ", /e" ' Explorer le dossier
        'p.StartInfo = New ProcessStartInfo(sCheminDossier, sArg)

        Dim startInfo As New ProcessStartInfo
        Dim sSysDir$ = Environment.GetFolderPath(Environment.SpecialFolder.System)
        Dim sWinDir$ = IO.Path.GetDirectoryName(sSysDir)
        startInfo.FileName = sWinDir & "\explorer.exe"
        startInfo.Arguments = sCheminDossier & ", /e"
        p.StartInfo = startInfo

        p.Start()

    End Using

End Sub

#End Region

#Region "Gestion de la lecture et de l'�criture de fichiers"

Public Function sLireFichier$(sCheminFichier$, _
    Optional bLectureSeule As Boolean = False, Optional bUnicodeUTF8 As Boolean = False)

    ' Lire et renvoyer le contenu d'un fichier

    Dim s$ = ""
    If Not bFichierExiste(sCheminFichier, bPrompt:=True) Then Return s

    Dim sbContenu As New StringBuilder
    Dim bDebut As Boolean = False
    Dim fs As IO.FileStream = Nothing
    Try

        Dim encodage As Encoding
        If bUnicodeUTF8 Then
            encodage = Encoding.UTF8
        Else
            encodage = Encoding.GetEncoding(iCodePageWindowsLatin1252)
        End If

        ' Si Excel a verrouill� le fichier, une simple ouverture en lecture
        '  est permise � condition de pr�ciser aussi IO.FileShare.ReadWrite
        Dim share As IO.FileShare = IO.FileShare.Read ' Valeur par d�faut
        If bLectureSeule Then share = IO.FileShare.ReadWrite
        fs = New IO.FileStream(sCheminFichier, IO.FileMode.Open, _
            IO.FileAccess.Read, share)
        ' Encoding.UTF8 fonctionne dans le bloc-notes, mais pas avec Excel via csv
        Using sr As New IO.StreamReader(fs, encodage)
            fs = Nothing
            Do
                Dim sLigne$ = sr.ReadLine()
                If IsNothing(sLigne) Then Exit Do
                If bDebut Then sbContenu.Append(vbCrLf)
                bDebut = True
                sbContenu.Append(sLigne)
            Loop While True
        End Using
        Return sbContenu.ToString

    Catch ex As Exception
        AfficherMsgErreur2(ex, "sLireFichier")
        Return Nothing
    Finally
        If fs IsNot Nothing Then fs.Dispose() ' CA2000
    End Try

End Function

Public Function sbLireFichier(sCheminFichier$, _
    Optional bLectureSeule As Boolean = False) As StringBuilder

    ' Lire et renvoyer le contenu d'un fichier

    Dim sb As New StringBuilder
    If Not bFichierExiste(sCheminFichier, bPrompt:=True) Then Return sb

    Dim bDebut As Boolean = False
    Dim fs As IO.FileStream = Nothing
    Try
        ' Si Excel a verrouill� le fichier, une simple ouverture en lecture
        '  est permise � condition de pr�ciser aussi IO.FileShare.ReadWrite
        Dim share As IO.FileShare = IO.FileShare.Read ' Valeur par d�faut
        If bLectureSeule Then share = IO.FileShare.ReadWrite
        fs = New IO.FileStream(sCheminFichier, IO.FileMode.Open, IO.FileAccess.Read, share)
        ' Encoding.UTF8 fonctionne dans le bloc-notes, mais pas avec Excel via csv
        Using sr As New IO.StreamReader(fs, _
            Encoding.GetEncoding(iCodePageWindowsLatin1252))
            fs = Nothing
            Do
                Dim sLigne$ = sr.ReadLine()
                If IsNothing(sLigne) Then Exit Do
                If bDebut Then sb.Append(vbCrLf)
                bDebut = True
                sb.Append(sLigne)
            Loop While True
        End Using
        Return sb

    Catch ex As Exception
        AfficherMsgErreur2(ex, "sbLireFichier")
        Return Nothing
    Finally
        If fs IsNot Nothing Then fs.Dispose() ' CA2000
    End Try

End Function

Public Function asLireFichier(sCheminFichier$, _
    Optional bLectureSeule As Boolean = False, _
    Optional bVerifierCrCrLf As Boolean = False, _
    Optional bUnicodeUTF8 As Boolean = False) As String()

    ' Lire et renvoyer le contenu d'un fichier
    Dim astr$() = Nothing
    If Not bFichierExiste(sCheminFichier, bPrompt:=True) Then Return astr

    Dim fs As IO.FileStream = Nothing
    Try

        Dim encodage As Encoding
        If bUnicodeUTF8 Then
            encodage = Encoding.UTF8
        Else
            encodage = Encoding.GetEncoding(iCodePageWindowsLatin1252)
        End If

        If bLectureSeule Then

            fs = New IO.FileStream(sCheminFichier, _
                IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
            Using sr As New IO.StreamReader(fs, encodage)
                fs = Nothing

                ' 23/04/2013 Optimisation du mode bLectureSeule
                '  On doit enlever les lignes vides dues au double s�parateur CrLf
                'Return sr.ReadToEnd.Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries)

                ' 24/04/2013 Conserver strictement le m�me comportement de sr.ReadLine()
                '  en RAM
                Dim fluxChaine As New clsFluxChaine(sr.ReadToEnd)
                Return fluxChaine.asLignes(bVerifierCrCrLf)

                'Dim lst As New Collections.Generic.List(Of String)
                'While Not sr.EndOfStream
                '    ' A line is defined as a sequence of characters followed by 
                '    '  a line feed ("\n"), a carriage return ("\r"), or 
                '    '  a carriage return immediately followed by a line feed ("\r\n"). 
                '    ' http://msdn.microsoft.com/en-us/library/system.io.streamreader.readline.aspx
                '    lst.Add(sr.ReadLine())
                'End While
                'Return lst.ToArray

            End Using

        Else
            Return IO.File.ReadAllLines(sCheminFichier, encodage)
        End If

    Catch ex As Exception
        AfficherMsgErreur2(ex, "asLireFichier")
        Return Nothing
    Finally
        If fs IsNot Nothing Then fs.Dispose() ' CA2000
    End Try

End Function

Public Function bListToHashSet(lst As List(Of String), ByRef hs As HashSet(Of String), _
    Optional bPromptErr As Boolean = True) As Boolean

    ' Convertir une liste en HashSet en g�rant les doublons
    ' Si on n'affiche pas les doublons, alors on d�doublonne automatiquement

    'Try : Try Catch inutile, car le constructeur ne g�n�re pas d'exception
    '    ' S'il n'y a pas de doublon, alors le constructeur idoine suffit
    '    hs = New HashSet(Of String)(lst)
    'Catch
        ' S'il y a une exception, alors d�doublonner la liste
    'End Try

    hs = New HashSet(Of String)
    For Each sLigne As String In lst
        If hs.Contains(sLigne) Then
            ' Pour la cha�ne vide, d�doublonner sans signalement
            If bPromptErr AndAlso Not String.IsNullOrEmpty(sLigne) Then MsgBox(
                "bListToHashSet : la liste contient au moins un doublon : " & sLigne,
                MsgBoxStyle.Critical, m_sTitreMsg) : Return False
            Continue For
        End If
        hs.Add(sLigne)
    Next

    Return True

End Function

Public Function bEcrireFichier(sCheminFichier$, _
    sbContenu As StringBuilder, _
    Optional bEncodageDefaut As Boolean = False, _
    Optional bEncodageISO_8859_1 As Boolean = False, _
    Optional bEncodageUTF8 As Boolean = False, _
    Optional bEncodageUTF16 As Boolean = False, _
    Optional iEncodage% = 0, Optional sEncodage$ = "", _
    Optional bPrompt As Boolean = True, _
    Optional ByRef sMsgErr$ = "") As Boolean

    ' 18/12/2017 bPromptErr:=True -> bPromptErr:=bPrompt
    If Not bSupprimerFichier(sCheminFichier, bPromptErr:=bPrompt) Then Return False

    If String.IsNullOrEmpty(sCheminFichier) Then _
        Throw New ArgumentNullException("sCheminFichier")
    If sbContenu Is Nothing Then Throw New ArgumentNullException("sbContenu")
    If String.IsNullOrEmpty(sEncodage) Then sEncodage = ""

    'Dim sw As IO.StreamWriter = Nothing
    Try
        ' Pour produire un document xml valide, il faut laisser l'encodage par d�faut
        '  de DotNet, sinon certains caract�res sp�ciaux ne passent pas, comme �
        Dim encodage As Encoding = Encoding.Default
        If Not bEncodageDefaut Then
            If bEncodageISO_8859_1 Then
                encodage = Encoding.GetEncoding(sEncodageISO_8859_1)
            ElseIf bEncodageUTF8 Then
                encodage = Encoding.UTF8 ' M�me chose que :
                'encodage = Encoding.GetEncoding(iEncodageUnicodeUTF8)
            ElseIf bEncodageUTF16 Then ' 28/01/2013
                encodage = Encoding.Unicode ' = UTF16
            ElseIf iEncodage > 0 Then
                encodage = Encoding.GetEncoding(iEncodage)
            ElseIf sEncodage.Length > 0 Then
                encodage = Encoding.GetEncoding(sEncodage)
            Else
                ' Encodage par d�faut de VB6 et de Windows en fran�ais
                encodage = Encoding.GetEncoding(iCodePageWindowsLatin1252)
            End If
        End If
        Using sw As New IO.StreamWriter(sCheminFichier, append:=False, Encoding:=encodage)
            sw.Write(sbContenu.ToString())
        End Using 'sw.Close()
        Return True
    Catch ex As Exception
        Dim sMsg$ = "Impossible d'�crire les donn�es dans le fichier :" & vbCrLf & _
            sCheminFichier & vbCrLf & sCauseErrPoss
        sMsgErr = sMsg & vbCrLf & ex.Message
        If bPrompt Then AfficherMsgErreur2(ex, "bEcrireFichier", sMsg)
        Return False
    'Finally
    '    If Not IsNothing(sw) Then sw.Close()
    End Try

End Function

Public Function bEcrireFichier(sCheminFichier$, sContenu$, _
    Optional bEncodageDefaut As Boolean = False, _
    Optional bEncodageISO_8859_1 As Boolean = False, _
    Optional bEncodageUFT8 As Boolean = False, _
    Optional iEncodage% = 0, Optional sEncodage$ = "", _
    Optional bPromptErr As Boolean = True, _
    Optional ByRef sMsgErr$ = "") As Boolean

    If Not bSupprimerFichier(sCheminFichier, bPromptErr:=bPromptErr) Then Return False

    If String.IsNullOrEmpty(sCheminFichier) Then _
        Throw New ArgumentNullException("sCheminFichier")
    If String.IsNullOrEmpty(sContenu) Then Throw New ArgumentNullException("sContenu")
    If String.IsNullOrEmpty(sEncodage) Then sEncodage = ""

    'Dim sw As IO.StreamWriter = Nothing
    Try
        ' Pour produire un document xml valide, il faut laisser l'encodage par d�faut
        '  de DotNet, sinon certains caract�res sp�ciaux ne passent pas, comme �
        Dim encodage As Encoding = Encoding.Default
        If Not bEncodageDefaut Then
            If bEncodageISO_8859_1 Then
                encodage = Encoding.GetEncoding(sEncodageISO_8859_1)
            ElseIf bEncodageUFT8 Then
                encodage = Encoding.UTF8 ' M�me chose que :
                'encodage = Encoding.GetEncoding(iEncodageUnicodeUTF8)
            ElseIf iEncodage > 0 Then
                encodage = Encoding.GetEncoding(iEncodage)
            ElseIf sEncodage.Length > 0 Then
                encodage = Encoding.GetEncoding(sEncodage)
            Else
                ' Encodage par d�faut de VB6 et de Windows en fran�ais
                encodage = Encoding.GetEncoding(iCodePageWindowsLatin1252)
            End If
        End If
        Using sw As New IO.StreamWriter(sCheminFichier, append:=False, Encoding:=encodage)
            sw.Write(sContenu)
        End Using 'sw.Close()
        Return True
    Catch ex As Exception
        Dim sMsg$ = "Impossible d'�crire les donn�es dans le fichier :" & vbCrLf & _
            sCheminFichier & vbCrLf & sCauseErrPoss
        sMsgErr = sMsg & vbCrLf & ex.Message
        If bPromptErr Then AfficherMsgErreur2(ex, "bEcrireFichier", sMsg)
        Return False
    'Finally
    '    If Not IsNothing(sw) Then sw.Close()
    End Try

End Function

Public Function bAjouterFichier(sCheminFichier$, sContenu$, _
    Optional bPrompt As Boolean = True, _
    Optional ByRef sMsgErr$ = "") As Boolean

    ' V�rification de l'acces en �criture auparavant
    If Not bFichierAccessible(sCheminFichier, bPromptFermer:=True, _
        bInexistOk:=True, bPromptRetenter:=True) Then Return False

    If String.IsNullOrEmpty(sCheminFichier) Then Throw New ArgumentNullException("sCheminFichier")
    If String.IsNullOrEmpty(sContenu) Then Throw New ArgumentNullException("sContenu")

    'Dim sw As IO.StreamWriter = Nothing
    Try
        Using sw As New IO.StreamWriter(sCheminFichier, append:=True, _
            Encoding:=Encoding.GetEncoding(iCodePageWindowsLatin1252))
        sw.Write(sContenu)
        End Using 'sw.Close()
        Return True
    Catch ex As Exception
        Dim sMsg$ = "Impossible d'�crire les donn�es dans le fichier :" & vbCrLf & _
            sCheminFichier & vbCrLf & sCauseErrPoss
        sMsgErr = sMsg & vbCrLf & ex.Message
        If bPrompt Then AfficherMsgErreur2(ex, "bAjouterFichier", sMsg)
        Return False
    'Finally
    '    If Not IsNothing(sw) Then sw.Close()
    End Try

End Function

Public Function bAjouterFichier(sCheminFichier$, _
    sbContenu As StringBuilder) As Boolean

    ' V�rification de l'acces en �criture auparavant
    If Not bFichierAccessible(sCheminFichier, bPromptFermer:=True, _
        bInexistOk:=True, bPromptRetenter:=True) Then Return False

    If String.IsNullOrEmpty(sCheminFichier) Then Throw New ArgumentNullException("sCheminFichier")
    If sbContenu Is Nothing Then Throw New ArgumentNullException("sbContenu")

    'Dim sw As IO.StreamWriter = Nothing
    Try
        Using sw As New IO.StreamWriter(sCheminFichier, append:=True, _
            Encoding:=Encoding.GetEncoding(iCodePageWindowsLatin1252))
        sw.Write(sbContenu.ToString())
        End Using 'sw.Close()
        Return True
    Catch ex As Exception
        AfficherMsgErreur2(ex, "bAjouterFichier", _
            "Impossible d'�crire les donn�es dans le fichier :" & vbCrLf & sCheminFichier)
        Return False
    'Finally
    '    If Not IsNothing(sw) Then sw.Close()
    End Try

End Function

Public Function bReencoder(sCheminFichier$) As Boolean

    ' R�encoder un fichier avec les sauts de ligne Unix (vbLf) en fichier Windows (vbCrLf)
    If Not bFichierExiste(sCheminFichier, bPrompt:=True) Then Return False
    Dim sb As StringBuilder = sbLireFichier(sCheminFichier)
    If sb.Length = 0 Then Return False
    Return bEcrireFichier(sCheminFichier, sb.Append(vbCrLf))

End Function

#End Region

#Region "Divers"

Public Function asArgLigneCmd(sLigneCmd$, _
    Optional bSupprimerEspaces As Boolean = True) As String()

    ' Retourner les arguments de la ligne de commande

    ' Parser les noms longs (fonctionne quel que soit le nombre de fichiers)
    ' Chaque nom long de fichier est entre guillemets : ""
    '  une fois le nom trait�, les guillemets sont enlev�s
    ' S'il y a un non court parmi eux, il n'est pas entre guillemets

    ' R�utilisation de cette fonction pour parser les "" :
    ' --------------------------------------------------
    ' Cette fonction ne respecte pas le nombre de colonne, elle parse seulement les "" correctement
    '  (on pourrait cependant faire une option pour conserver les colonnes vides)
    ' Cette fonction ne sait pas non plus parser correctement une seconde ouverture de "" entre ;
    '  tel que : xxx;"x""x";xxx ou "xxx";"x""x";"xxx"
    ' En dehors des guillemets, le s�parateur est l'espace et non le ;
    ' --------------------------------------------------

    Dim asArgs$() = Nothing
    If String.IsNullOrEmpty(sLigneCmd) Then
        ReDim asArgs(0)
        asArgs(0) = ""
        asArgLigneCmd = asArgs
        Exit Function
    End If

    ' Parser les noms cours : facile
    'asArgs = Split(Command, " ")

    Dim lstArgs As New List(Of String) ' 16/10/2016
    Const sGm$ = """" ' Un seul " en fait
    'sGm = Chr$(34) ' Guillemets
    Dim sFichier$, sSepar$
    Dim sCmd$, iLongCmd%, iFin%, iDeb%, iDeb2%
    Dim bFin As Boolean, bNomLong As Boolean
    Dim iCarSuiv% = 1
    sCmd = sLigneCmd
    iLongCmd = Len(sCmd)
    iDeb = 1
    Do

        bNomLong = False : sSepar = " "

        ' Cha�ne vide : ""
        Dim s2Car$ = Mid(sCmd, iDeb, 2)
        If s2Car = sGm & sGm Then
            bNomLong = True : sSepar = sGm
            iFin = iDeb + 1
            GoTo Suite
        End If

        ' Si le premier caract�re est un guillement, c'est un nom long
        Dim sCar$ = Mid(sCmd, iDeb, 1)
        'Dim iCar% = Asc(sCar) ' Pour debug
        If sCar = sGm Then bNomLong = True : sSepar = sGm

        iDeb2 = iDeb
        ' Supprimer les guillemets dans le tableau de fichiers
        If bNomLong AndAlso iDeb2 < iLongCmd Then iDeb2 += 1 ' Gestion cha�ne vide
        iFin = InStr(iDeb2 + 1, sCmd, sSepar)

        ' 16/10/2016 On tol�re que un " peut remplacer un espace
        iCarSuiv = 1
        Dim iFinGM% = InStr(iDeb2 + 1, sCmd, sGm)
        If iFinGM > 0 AndAlso iFin > 0 AndAlso iFinGM < iFin Then
            iFin = iFinGM : bNomLong = True : sSepar = sGm : iCarSuiv = 0
        End If

        ' Si le s�parateur n'est pas trouv�, c'est la fin de la ligne de commande
        If iFin = 0 Then bFin = True : iFin = iLongCmd + 1

        sFichier = Mid(sCmd, iDeb2, iFin - iDeb2)
        If bSupprimerEspaces Then sFichier = Trim(sFichier)

        If sFichier.Length > 0 Then lstArgs.Add(sFichier)

        If bFin Or iFin = iLongCmd Then Exit Do

Suite:
        iDeb = iFin + iCarSuiv ' 1

        ' 16/10/2016 On tol�re que un " peut remplacer un espace, plus besoin
        'If bNomLong Then iDeb = iFin + 2

        If iDeb > iLongCmd Then Exit Do ' 09/10/2014 Gestion cha�ne vide

    Loop

    asArgs = lstArgs.ToArray()
    Const iCodeGuillemets% = 34
    For iNumArg As Integer = 0 To UBound(asArgs)
        Dim sArg$ = asArgs(iNumArg)
        ' S'il y avait 2 guillemets, il n'en reste plus qu'un
        '  on le converti en cha�ne vide
        Dim iLong0% = Len(sArg)
        If iLong0 = 1 AndAlso Asc(sArg.Chars(0)) = iCodeGuillemets Then asArgs(iNumArg) = ""
    Next iNumArg

    asArgLigneCmd = asArgs

End Function

Public Function sConvNomDos$(sChaine$, _
    Optional bLimit8Car As Boolean = False, _
    Optional bConserverSignePlus As Boolean = False)

    ' Remplacer les caract�res interdits pour les noms de fichiers DOS
    '  et retourner un nom de fichier 8.3 correct si demand�

    Dim iSel%, sBuffer$, sCar$, iCode%, sCarConv2$, sCarDest$
    Dim bOk As Boolean, bMaj As Boolean
    sBuffer = Trim$(sChaine)
    If bLimit8Car Then sBuffer = Left$(sBuffer, 8)
    Const sCarConv$ = " .��/[]:;|=,*-" ' Caract�res � convertir en soulign�
    sCarConv2 = sCarConv
    If Not bConserverSignePlus Then sCarConv2 &= "+"
    For iSel = 1 To Len(sBuffer)
        sCar = Mid$(sBuffer, iSel, 1)
        iCode = Asc(sCar)
        bMaj = False
        If iCode >= 65 And iCode <= 90 Then bMaj = True
        If iCode >= 192 And iCode <= 221 Then bMaj = True
        If InStr(sCarConv2, sCar) > 0 Then _
            Mid$(sBuffer, iSel, 1) = "_" : GoTo Suite
        If InStr("����", sCar) > 0 Then
            If bMaj Then sCarDest = "E" Else sCarDest = "e"
            Mid$(sBuffer, iSel, 1) = sCarDest
            GoTo Suite
        End If
        If InStr("����", sCar) > 0 Then
            If bMaj Then sCarDest = "A" Else sCarDest = "a"
            Mid$(sBuffer, iSel, 1) = sCarDest
            GoTo Suite
        End If
        If InStr("����", sCar) > 0 Then
            If bMaj Then sCarDest = "I" Else sCarDest = "i"
            Mid$(sBuffer, iSel, 1) = sCarDest
            GoTo Suite
        End If
        If InStr("����", sCar) > 0 Then
            If bMaj Then sCarDest = "U" Else sCarDest = "u"
            Mid$(sBuffer, iSel, 1) = sCarDest
            GoTo Suite
        End If
        If InStr("�����", sCar) > 0 Then ' 08/05/2013
            If bMaj Then sCarDest = "O" Else sCarDest = "o"
            Mid$(sBuffer, iSel, 1) = sCarDest
            GoTo Suite
        End If
        If InStr("�", sCar) > 0 Then ' 12/06/2015
            If bMaj Then sCarDest = "C" Else sCarDest = "c"
            Mid$(sBuffer, iSel, 1) = sCarDest
            GoTo Suite
        End If
        If bConserverSignePlus And iCode = 43 Then GoTo Suite
        'de 65 � 90  maj
        'de 97 � 122 min
        'de 48 � 57 Chiff
        bOk = False
        If (iCode >= 65 And iCode <= 90) Then bOk = True
        If (iCode >= 97 And iCode <= 122) Then bOk = True
        If (iCode >= 48 And iCode <= 57) Then bOk = True
        If Not bOk Then Mid$(sBuffer, iSel, 1) = "_"
Suite:
    Next iSel
    sConvNomDos = sBuffer

End Function

#End Region

#Region "Classe Flux Chaine"

' Equivalent de mscorlib.dll: System.IO.StreamReader.ReadLine() As String
'  mais pour une chaine : optimisation des flux

Public Class clsFluxChaine

    Private m_iNumLigne% = 0 ' Debug
    Private m_sChaine$
    Private m_iPos% = 0
    Private c13 As Char = ChrW(13) ' vbCr
    Private c10 As Char = ChrW(10) ' vbLf

    Public Sub New(sChaine$)
        m_sChaine = sChaine
    End Sub

    Public Function asLignes(Optional bVerifierCrCrLf As Boolean = False) As String()

        Dim lst As New Collections.Generic.List(Of String)
        Dim iNumLigne2% = 0
        Do
            Dim sLigne$ = StringReadLine(bVerifierCrCrLf)
            ' 05/02/2014 Ne pas ignorer les lignes vides, et poursuivre
            'If String.IsNullOrEmpty(sLigne) Then Exit Do
            If IsNothing(sLigne) Then sLigne = ""
            lst.Add(sLigne)
            iNumLigne2 += 1
        Loop While m_iPos < m_sChaine.Length ' 05/02/2014
        'Loop While True
        Return lst.ToArray

    End Function

    Public Function StringReadLine$(Optional bVerifierCrCrLf As Boolean = False)

        If String.IsNullOrEmpty(m_sChaine) Then Return Nothing

        Dim iLong% = m_sChaine.Length

        Dim iNum% = m_iPos
        Do While iNum < iLong
            Dim ch As Char = m_sChaine.Chars(iNum)
            Select Case ch
                Case c13, c10

                    Dim str As String = m_sChaine.Substring(m_iPos, iNum - m_iPos)

                    m_iPos = iNum + 1

                    If Not bVerifierCrCrLf Then ' 24/11/2013
                        If ch = c13 AndAlso m_iPos < iLong AndAlso _
                           m_sChaine.Chars(m_iPos) = c10 Then m_iPos += 1
                        Return str
                    End If

                    Dim chSuiv As Char '= m_sChaine.Chars(m_iPos)
                    ' 17/09/2013 Maintenant qu'on fait +2, tester aussi ce cas
                    If m_iPos < iLong Then chSuiv = m_sChaine.Chars(m_iPos)

                    Dim chSuiv2 As Char
                    If m_iPos < iLong - 1 Then chSuiv2 = m_sChaine.Chars(m_iPos + 1)
                    ' 02/08/2013 Il peut arriver 13+13+10 !?
                    If ch = c13 AndAlso m_iPos < iLong - 1 AndAlso _
                        chSuiv = c13 AndAlso chSuiv2 = c10 Then
                        m_iPos += 2
                    ElseIf ch = c13 AndAlso m_iPos < iLong AndAlso chSuiv = c10 Then
                        m_iPos += 1
                    End If
                    'Debug.WriteLine("L" & m_iNumLigne & ":" & str2)
                    m_iNumLigne += 1
                    Return str
            End Select
            iNum += 1
        Loop
        If iNum > m_iPos Then
            Dim str2$ = m_sChaine.Substring(m_iPos, (iNum - m_iPos))
            m_iPos = iNum
            'Debug.WriteLine("L" & m_iNumLigne & ":" & str2)
            m_iNumLigne += 1
            Return str2
        End If

        Return Nothing

    End Function

End Class

#End Region

End Module