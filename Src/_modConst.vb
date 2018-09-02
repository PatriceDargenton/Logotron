
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
    Private Const sDateVersionLogotron$ = "02/09/2018"
    Public Const sDateVersionAppli$ = sDateVersionLogotron

    Public ReadOnly sVersionAppli$ =
        My.Application.Info.Version.Major & "." &
        My.Application.Info.Version.Minor &
        My.Application.Info.Version.Build

    Public ReadOnly sModeLecture$ = enumModeLecture.sCsv
    'Public ReadOnly sModeLecture$ = enumModeLecture.sCode
    'Public ReadOnly sModeLecture$ = enumModeLecture.sJSon

    Public ReadOnly sModeLectureMotsExistants$ = enumModeLectureMotExistant.sCsv
    'Public ReadOnly sModeLectureMotsExistants$ = enumModeLectureMotExistant.sCode

    Public Const sHasard$ = "H"
    Public Const sCarSautDeLigne$ = "↲"

End Module