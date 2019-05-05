
Module modConstLogotron

    Public Const sGm$ = """"

    Public Const sFormatPC$ = "0 %"
    Public Const sFormatPC1$ = "0.0 %"
    Public Const sFormatPC2$ = "0.00 %"
    Public Const sFormatPC4$ = "0.0000 %"

    Public Const sSelectLogotron = "L"
    Public Const sSelectDictionnaire = "D"
    Public Const iNonSelect% = 0
    Public Const iSelectDictionnaire% = 1
    Public Const iSelectLogotron% = 2
    Public Const sNonSelectNum$ = "0"
    Public Const sSelectDictionnaireNum$ = "1"
    Public Const sSelectLogotronNum$ = "2"

    ' Séparateur entre la définition du suffixe et celle(s) du(des) préfixe(s)
    Public Const sSepDef$ = "  "

    Public ReadOnly sLang$ = enumLangue.Fr
    'Public ReadOnly sLang$ = enumLangue.En ' Ok si sModeLecture = enumModeLecture.sCsv

    ' Si sModeLecture = enumModeLecture.sCode, on ne peut pas exclure cela (sauf au tirage) :
    Public Const bInclureNeologismesAmusants As Boolean = True

    Public Const sMsgCopiePressePapier$ = "Le texte a été copié dans le presse papier !"

    Public Const bDebugElision As Boolean = False
    Public Const bElision As Boolean = True ' 28/04/2019
    Public Const sCarElisionO$ = "(o)"
    Public Const sCarO$ = "o"
    Public Const iLongPrefixeMinElision% = 2

End Module