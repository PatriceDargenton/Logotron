
Public Class enumModeLecture
    Public Const sCsv$ = "Csv"
    Public Const sCode$ = "Code"
    Public Const sJSon$ = "JSon"
End Class

Public Class enumModeLectureMotExistant
    Public Const sCsv$ = "Csv"
    Public Const sCode$ = "Code" ' Liste incomplète, simple test
    'Public Const sJSon$ = "JSon"
End Class

Public Class enumLangue
    Public Const Fr$ = "_fr" ' Français
    Public Const En$ = "_en" ' English
End Class

Public Class enumNiveau
    Public Const N1$ = "1"
    Public Const N2$ = "2"
    Public Const N3$ = "3"
    Public Shared Function iCoef%(sNiveaux$)
        Dim iCoefNiv% = 0
        Select Case sNiveaux
        Case "1 " : iCoefNiv = 1
        Case "1 2 " : iCoefNiv = 2
        Case "2 " : iCoefNiv = 3
        Case "1 2 3 " : iCoefNiv = 5
        Case "1 3 " : iCoefNiv = 6
        Case "2 3 " : iCoefNiv = 8
        Case "3 " : iCoefNiv = 10
        Case Else : If bDebug Then Stop
        End Select
        Return iCoefNiv
    End Function
End Class

Public Class enumFrequence
    Public Const Frequent$ = "Frequent"
    Public Const Moyen$ = "Moyen"
    Public Const Rare$ = "Rare"
    Public Const Absent$ = "Absent" ' Impossible, sauf si les fréquences ne sont plus à jour
    Public Const Defaut$ = Frequent
End Class

Public Class enumFrequenceAbrege

    Public Const Frequent$ = "Fréq."
    Public Const Moyen$ = "Moy."
    Public Const Rare$ = "Rare"
    Public Const Absent$ = "Abs."

    Public Shared Function sConv$(sFreqAbrege$)
        Dim sFreqComplet$ = ""
        Select Case sFreqAbrege
        Case enumFrequenceAbrege.Frequent : sFreqComplet = enumFrequence.Frequent
        Case enumFrequenceAbrege.Moyen : sFreqComplet = enumFrequence.Moyen
        Case enumFrequenceAbrege.Rare : sFreqComplet = enumFrequence.Rare
        Case enumFrequenceAbrege.Absent : sFreqComplet = enumFrequence.Absent
        Case Else : If bDebug Then Stop
        End Select
        Return sFreqComplet
    End Function

    Public Shared Function iCoef%(sFrequences$)
        Dim iCoefFreq% = 0
        Select Case sFrequences
        Case "Fréq. " : iCoefFreq = 1
        Case "Fréq. Abs. " : iCoefFreq = 1
        Case "Fréq. Moy. " : iCoefFreq = 2
        Case "Fréq. Moy. Abs. " : iCoefFreq = 2
        Case "Moy. " : iCoefFreq = 3
        Case "Moy. Abs. " : iCoefFreq = 3
        Case "Fréq. Moy. Rare " : iCoefFreq = 5
        Case "Fréq. Moy. Rare Abs. " : iCoefFreq = 5
        Case "Fréq. Rare " : iCoefFreq = 6
        Case "Fréq. Rare Abs. " : iCoefFreq = 6
        Case "Moy. Rare " : iCoefFreq = 8
        Case "Moy. Rare Abs. " : iCoefFreq = 8
        Case "Rare " : iCoefFreq = 10
        Case "Rare Abs. " : iCoefFreq = 10
        Case "Abs. " : iCoefFreq = 10
        Case Else : If bDebug Then Stop
        End Select
        Return iCoefFreq
    End Function

End Class

Public Class enumOrigine
    Public Const sGrec$ = "Grec"
    Public Const sLatin$ = "Latin"
    Public Const sAutre$ = "Autre" ' Danois, Italien, Anglais, ...
    Public Const sNonPrecise$ = "Non précisé"
    Public Const sGrecoLatin$ = "Gréco-latin"
    Public Const sNeologismeAmusant$ = "Néologisme amusant" ' Fiscalo-
    Public Const sDefaut$ = sGrecoLatin
End Class