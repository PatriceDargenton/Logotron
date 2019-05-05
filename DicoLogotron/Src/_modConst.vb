
Module _modConst

#If DEBUG Then
    Public Const bDebug As Boolean = True
    Public Const bRelease As Boolean = False
#Else
    Public Const bDebug As Boolean = False
    Public Const bRelease As Boolean = True
#End If

    Public ReadOnly sNomAppli$ = My.Application.Info.Title
    Public ReadOnly m_sTitreMsg$ = sNomAppli
    Private Const sDateVersionLogotron$ = "05/05/2019"
    Public Const sDateVersionAppli$ = sDateVersionLogotron

    Public ReadOnly sVersionAppli$ =
        My.Application.Info.Version.Major & "." &
        My.Application.Info.Version.Minor &
        My.Application.Info.Version.Build

    Public ReadOnly sModeLecture$ = enumModeLecture.sCsv
    'Public ReadOnly sModeLecture$ = enumModeLecture.sCode
    'Public ReadOnly sModeLecture$ = enumModeLecture.sJSon

    Public Const sHasard$ = "H"
    Public Const sCarSautDeLigne$ = "↲"
    Public Const sExtTxt$ = ".txt"

    Public ReadOnly sCheminDico$ = "Dico" & sLang & sExtTxt '".txt"

    Public Const sURLDicoFr$ = "http://patrice.dargenton.free.fr/CodesSources/VBTextFinder/Dico_Fr.zip"
    Public Const sURLDicoEn$ = "http://patrice.dargenton.free.fr/CodesSources/VBTextFinder/Dico_En.zip"
    ' Ce sont les mêmes dico. pour l'instant
    'Public Const sURLDicoUk$ = "http://patrice.dargenton.free.fr/CodesSources/VBTextFinder/Dico_Uk.zip"
    'Public Const sURLDicoUs$ = "http://patrice.dargenton.free.fr/CodesSources/VBTextFinder/Dico_Us.zip"

End Module