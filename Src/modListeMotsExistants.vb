
Module ModListeMotsExistants

    Public Sub ChargerMotsExistantsCode(ByRef dicoMotsExistants As Dictionary(Of String, clsMotExistant))

        dicoMotsExistants = New Dictionary(Of String, clsMotExistant)

        ' Cette liste peut être récupérée via DicoLogotron\Doc\MotsSimplesCode_fr.txt

#If DEBUG Then

        ' Liste incomplète ici, simple test
        Dim lstMots As New List(Of String) From {
                "abiotique", "VIE  SANS", "a", "biotique", "1", "1", "a", "bio", "Frequent", "Moyen",
                "acalorique", "CHALEUR  SANS", "a", "calorique", "1", "2", "a", "calori", "Frequent", "Moyen",
                "acanthocéphale", "TÊTE  DE L'ÉPINE", "acantho", "céphale", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthocyte", "CELLULE  DE L'ÉPINE", "acantho", "cyte", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthodactyle", "DOIGT  DE L'ÉPINE", "acantho", "dactyle", "3", "2", "acantho", "dactylo", "Moyen", "Frequent",
                "acanthoglosse", "LANGUE  DE L'ÉPINE", "acantho", "glosse", "3", "3", "acantho", "glosso", "Moyen", "Moyen",
                "acantholyse", "DÉCOMPOSITION  DE L'ÉPINE", "acantho", "lyse", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthomètre", "MESUREUR  DE L'ÉPINE", "acantho", "mètre", "3", "1", "acantho", "métro (métron)", "Moyen", "Frequent",
                "acanthozoïde", "ANIMAL  DE L'ÉPINE", "acantho", "zoïde", "3", "1", "acantho", "", "Moyen", "Moyen",
                "acarpe", "POIGNET  SANS", "a", "carpe", "1", "3", "a", "", "Frequent", "Moyen",
                "acentrique", "CENTRÉ  SANS", "a", "centrique", "1", "1", "a", "centro", "Frequent", "Frequent",
                "acéphale", "TÊTE  SANS", "a", "céphale", "1", "2", "a", "", "Frequent", "Frequent",
                "acéphalie", "TÊTE  SANS", "a", "céphalie", "1", "2", "a", "", "Frequent", "Frequent",
                "acère", "CORNE  SANS", "a", "cère", "1", "3", "a", "cérato", "Frequent", "Moyen",
                "achrome", "COULEUR  SANS", "a", "chrome", "1", "1", "a", "chromo", "Frequent", "Frequent",
                "achromie", "COULEUR  SANS", "a", "chromie", "1", "1", "a", "chromo", "Frequent", "Frequent",
                "achronique", "TEMPOREL(LE)  SANS", "a", "chronique", "1", "1", "a", "chrono", "Frequent", "Moyen",
                "acide", "QUI TUE  SANS", "a", "cide", "1", "1", "a", "", "Frequent", "Frequent",
                "acinèse", "MOUVEMENT  SANS", "a", "cinèse", "1", "2", "a", "cinèse", "Frequent", "Moyen",
                "acinésie", "MOUVEMENT  SANS", "a", "cinésie", "1", "2", "a", "cinèse", "Frequent", "Moyen",
                "acosmique", "UNIVERSALITÉ  SANS", "a", "cosmique", "1", "1", "a", "cosmo", "Frequent", "Rare",
                "acoumètre", "MESUREUR  DE L'AUDITION", "acou", "mètre", "2", "1", "", "métro (métron)", "Moyen", "Frequent",
                "acoumétrie", "MESURE  DE L'AUDITION", "acou", "métrie", "2", "1", "", "métro (métron)", "Moyen", "Frequent",
                "acoumétrique", "MESURAGE  DE L'AUDITION", "acou", "métrique", "2", "1", "", "métro (métron)", "Moyen", "Frequent",
                "acouphène", "APPARITION  DE L'AUDITION", "acou", "phène", "2", "3", "", "", "Moyen", "Moyen",
                "acrocarpe", "POIGNET  ÉLEVÉ(E) / EXTRÊME", "acro", "carpe", "3", "3", "", "", "Frequent", "Moyen",
                "acrocentrique", "CENTRÉ  ÉLEVÉ(E) / EXTRÊME", "acro", "centrique", "3", "1", "", "centro", "Frequent", "Frequent",
                "acrocéphale", "TÊTE  ÉLEVÉ(E) / EXTRÊME", "acro", "céphale", "3", "2", "", "", "Frequent", "Frequent",
                "acrocéphalie", "TÊTE  ÉLEVÉ(E) / EXTRÊME", "acro", "céphalie", "3", "2", "", "", "Frequent", "Frequent",
                "acrocéphalique", "TÊTE  ÉLEVÉ(E) / EXTRÊME", "acro", "céphalique", "3", "2", "", "", "Frequent", "Rare",
                "acrodonte", "DENT  ÉLEVÉ(E) / EXTRÊME", "acr(o)", "odonte", "3", "3", "", "", "Frequent", "Moyen",
                "acrodynie", "ÉNERGIE  ÉLEVÉ(E) / EXTRÊME", "acro", "dynie", "3", "2", "", "dyne", "Frequent", "Moyen",
                "acroïde", "APPARENCE  ÉLEVÉ(E) / EXTRÊME", "acro", "ïde", "3", "3", "", "", "Frequent", "Frequent",
                "acrolithe", "PIERRE  ÉLEVÉ(E) / EXTRÊME", "acro", "lithe", "3", "2", "", "litho", "Frequent", "Frequent",
                "acromégalie", "GRANDEUR  ÉLEVÉ(E) / EXTRÊME", "acro", "mégalie", "3", "1", "", "mégalo", "Frequent", "Moyen",
                "acronyme", "NOM  ÉLEVÉ(E) / EXTRÊME", "acr(o)", "onyme", "3", "3", "", "", "Frequent", "Moyen",
                "acronymie", "NOM  ÉLEVÉ(E) / EXTRÊME", "acro", "nymie", "3", "3", "", "", "Frequent", "Moyen",
                "acropathie", "MALADIE  ÉLEVÉ(E) / EXTRÊME", "acro", "pathie", "3", "2", "", "patho (souffrance)", "Frequent", "Frequent",
                "acropète", "QUI VA VERS  ÉLEVÉ(E) / EXTRÊME", "acro", "pète", "3", "2", "", "", "Frequent", "Rare",
                "acrophobie", "AVERSION  ÉLEVÉ(E) / EXTRÊME", "acro", "phobie", "3", "1", "", "phobe", "Frequent", "Frequent",
                "acrophonie", "VOIX  ÉLEVÉ(E) / EXTRÊME", "acro", "phonie", "3", "1", "", "", "Frequent", "Frequent",
                "acropode", "PIED  ÉLEVÉ(E) / EXTRÊME", "acro", "pode", "3", "2", "", "", "Frequent", "Frequent",
                "acropole", "VILLE  ÉLEVÉ(E) / EXTRÊME", "acro", "pole", "3", "2", "", "", "Frequent", "Moyen",
                "acrosome", "CORPS  ÉLEVÉ(E) / EXTRÊME", "acro", "some", "3", "2", "", "somato", "Frequent", "Frequent",
                "actinifère", "PORTER  DU RAYON", "actini", "fère", "3", "2", "", "", "Rare", "Frequent",
                "actinique", "À PROPOS  DU RAYON", "actin", "ique", "3", "2", "", "", "Moyen", "Frequent",
                "actinisme", "DOCTRINE / ÉTAT / PROFESSION  DU RAYON", "actin", "isme", "3", "2", "", "", "Moyen", "Frequent",
                "actinite", "INFLAMMATION  DU RAYON", "actin", "ite", "3", "2", "", "", "Moyen", "Frequent",
                "actinologie", "ÉTUDE  DU RAYON", "actino", "logie", "3", "1", "", "logo", "Moyen", "Frequent",
                "actinologique", "ÉTUDE  DU RAYON", "actino", "logique", "3", "1", "", "logo", "Moyen", "Frequent",
                "actinomètre", "MESUREUR  DU RAYON", "actino", "mètre", "3", "1", "", "métro (métron)", "Moyen", "Frequent",
                "actinométrie", "MESURE  DU RAYON", "actino", "métrie", "3", "1", "", "métro (métron)", "Moyen", "Frequent",
                "actinométrique", "MESURAGE  DU RAYON", "actino", "métrique", "3", "1", "", "métro (métron)", "Moyen", "Frequent",
                "actinomorphe", "FORME  DU RAYON", "actino", "morphe", "3", "1", "", "morpho", "Moyen", "Frequent",
                "actinomycète", "CHAMPIGNON  DU RAYON", "actino", "mycète", "3", "2", "", "", "Moyen", "Moyen",
                "actinomycose", "CHAMPIGNON  DU RAYON", "actino", "mycose", "3", "2", "", "", "Moyen", "Moyen",
                "actinopode", "PIED  DU RAYON", "actino", "pode", "3", "2", "", "", "Moyen", "Frequent",
                "actinoscopie", "EXAMEN  DU RAYON", "actino", "scopie", "3", "1", "", "scope", "Moyen", "Frequent",
                "actinothérapie", "MÉDECINE  DU RAYON", "actino", "thérapie", "3", "1", "", "thérapie", "Moyen", "Frequent",
                "actinotropisme", "ATTIRANCE  DU RAYON", "actino", "tropisme", "3", "2", "", "trope", "Moyen", "Frequent",
                "acyclique", "CIRCULARITÉ  SANS", "a", "cyclique", "1", "1", "a", "cyclo", "Frequent", "Frequent"
            }
#Else
        ' Liste complète (ne compile pas sous Visual Studio 2013)
        Dim lstMots As New List(Of String) From {
                "abiotique", "VIE  SANS", "a", "biotique", "1", "1", "a", "bio", "Frequent", "Moyen",
                "acalorique", "CHALEUR  SANS", "a", "calorique", "1", "2", "a", "calori", "Frequent", "Moyen",
                "acanthocéphale", "TÊTE  DE L'ÉPINE", "acantho", "céphale", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthocyte", "CELLULE  DE L'ÉPINE", "acantho", "cyte", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthodactyle", "DOIGT  DE L'ÉPINE", "acantho", "dactyle", "3", "2", "acantho", "dactylo", "Moyen", "Frequent",
                "acanthoglosse", "LANGUE  DE L'ÉPINE", "acantho", "glosse", "3", "3", "acantho", "glosso", "Moyen", "Moyen",
                "acantholyse", "DÉCOMPOSITION  DE L'ÉPINE", "acantho", "lyse", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthomètre", "MESUREUR  DE L'ÉPINE", "acantho", "mètre", "3", "1", "acantho", "métro (métron)", "Moyen", "Frequent",
                "acanthozoïde", "ANIMAL  DE L'ÉPINE", "acantho", "zoïde", "3", "1", "acantho", "", "Moyen", "Moyen",
                "acarpe", "POIGNET  SANS", "a", "carpe", "1", "3", "a", "", "Frequent", "Moyen",
                "acentrique", "CENTRÉ  SANS", "a", "centrique", "1", "1", "a", "centro", "Frequent", "Frequent",
                "acéphale", "TÊTE  SANS", "a", "céphale", "1", "2", "a", "", "Frequent", "Frequent",
                "acéphalie", "TÊTE  SANS", "a", "céphalie", "1", "2", "a", "", "Frequent", "Frequent",
                "acère", "CORNE  SANS", "a", "cère", "1", "3", "a", "cérato", "Frequent", "Moyen",
                "achrome", "COULEUR  SANS", "a", "chrome", "1", "1", "a", "chromo", "Frequent", "Frequent",
                "achromie", "COULEUR  SANS", "a", "chromie", "1", "1", "a", "chromo", "Frequent", "Frequent",
                "achronique", "TEMPOREL(LE)  SANS", "a", "chronique", "1", "1", "a", "chrono", "Frequent", "Moyen",
                "acide", "QUI TUE  SANS", "a", "cide", "1", "1", "a", "", "Frequent", "Frequent",
                "acinèse", "MOUVEMENT  SANS", "a", "cinèse", "1", "2", "a", "cinèse", "Frequent", "Moyen",
                "acinésie", "MOUVEMENT  SANS", "a", "cinésie", "1", "2", "a", "cinèse", "Frequent", "Moyen",
                "acosmique", "UNIVERSALITÉ  SANS", "a", "cosmique", "1", "1", "a", "cosmo", "Frequent", "Rare",
                "acoumètre", "MESUREUR  DE L'AUDITION", "acou", "mètre", "2", "1", "", "métro (métron)", "Moyen", "Frequent",
                "acoumétrie", "MESURE  DE L'AUDITION", "acou", "métrie", "2", "1", "", "métro (métron)", "Moyen", "Frequent",
                "acoumétrique", "MESURAGE  DE L'AUDITION", "acou", "métrique", "2", "1", "", "métro (métron)", "Moyen", "Frequent",
                "acouphène", "APPARITION  DE L'AUDITION", "acou", "phène", "2", "3", "", "", "Moyen", "Moyen",
                "acrocarpe", "POIGNET  ÉLEVÉ(E) / EXTRÊME", "acro", "carpe", "3", "3", "", "", "Frequent", "Moyen",
                "acrocentrique", "CENTRÉ  ÉLEVÉ(E) / EXTRÊME", "acro", "centrique", "3", "1", "", "centro", "Frequent", "Frequent",
                "acrocéphale", "TÊTE  ÉLEVÉ(E) / EXTRÊME", "acro", "céphale", "3", "2", "", "", "Frequent", "Frequent",
                "acrocéphalie", "TÊTE  ÉLEVÉ(E) / EXTRÊME", "acro", "céphalie", "3", "2", "", "", "Frequent", "Frequent",
                "acrocéphalique", "TÊTE  ÉLEVÉ(E) / EXTRÊME", "acro", "céphalique", "3", "2", "", "", "Frequent", "Rare",
                "acrodonte", "DENT  ÉLEVÉ(E) / EXTRÊME", "acr(o)", "odonte", "3", "3", "", "", "Frequent", "Moyen",
                "acrodynie", "ÉNERGIE  ÉLEVÉ(E) / EXTRÊME", "acro", "dynie", "3", "2", "", "dyne", "Frequent", "Moyen",
                "acroïde", "APPARENCE  ÉLEVÉ(E) / EXTRÊME", "acro", "ïde", "3", "3", "", "", "Frequent", "Frequent",
                "acrolithe", "PIERRE  ÉLEVÉ(E) / EXTRÊME", "acro", "lithe", "3", "2", "", "litho", "Frequent", "Frequent",
                "acromégalie", "GRANDEUR  ÉLEVÉ(E) / EXTRÊME", "acro", "mégalie", "3", "1", "", "mégalo", "Frequent", "Moyen",
                "acronyme", "NOM  ÉLEVÉ(E) / EXTRÊME", "acr(o)", "onyme", "3", "3", "", "", "Frequent", "Moyen",
                "acronymie", "NOM  ÉLEVÉ(E) / EXTRÊME", "acro", "nymie", "3", "3", "", "", "Frequent", "Moyen",
                "acropathie", "MALADIE  ÉLEVÉ(E) / EXTRÊME", "acro", "pathie", "3", "2", "", "patho (souffrance)", "Frequent", "Frequent",
                "acropète", "QUI VA VERS  ÉLEVÉ(E) / EXTRÊME", "acro", "pète", "3", "2", "", "", "Frequent", "Rare",
                "acrophobie", "AVERSION  ÉLEVÉ(E) / EXTRÊME", "acro", "phobie", "3", "1", "", "phobe", "Frequent", "Frequent",
                "acrophonie", "VOIX  ÉLEVÉ(E) / EXTRÊME", "acro", "phonie", "3", "1", "", "", "Frequent", "Frequent",
                "acropode", "PIED  ÉLEVÉ(E) / EXTRÊME", "acro", "pode", "3", "2", "", "", "Frequent", "Frequent",
                "acropole", "VILLE  ÉLEVÉ(E) / EXTRÊME", "acro", "pole", "3", "2", "", "", "Frequent", "Moyen",
                "acrosome", "CORPS  ÉLEVÉ(E) / EXTRÊME", "acro", "some", "3", "2", "", "somato", "Frequent", "Frequent",
                "actinifère", "PORTER  DU RAYON", "actini", "fère", "3", "2", "", "", "Rare", "Frequent",
                "actinique", "À PROPOS  DU RAYON", "actin", "ique", "3", "2", "", "", "Moyen", "Frequent",
                "actinisme", "DOCTRINE / ÉTAT / PROFESSION  DU RAYON", "actin", "isme", "3", "2", "", "", "Moyen", "Frequent",
                "actinite", "INFLAMMATION  DU RAYON", "actin", "ite", "3", "2", "", "", "Moyen", "Frequent",
                "actinologie", "ÉTUDE  DU RAYON", "actino", "logie", "3", "1", "", "logo", "Moyen", "Frequent",
                "actinologique", "ÉTUDE  DU RAYON", "actino", "logique", "3", "1", "", "logo", "Moyen", "Frequent",
                "actinomètre", "MESUREUR  DU RAYON", "actino", "mètre", "3", "1", "", "métro (métron)", "Moyen", "Frequent",
                "actinométrie", "MESURE  DU RAYON", "actino", "métrie", "3", "1", "", "métro (métron)", "Moyen", "Frequent",
                "actinométrique", "MESURAGE  DU RAYON", "actino", "métrique", "3", "1", "", "métro (métron)", "Moyen", "Frequent",
                "actinomorphe", "FORME  DU RAYON", "actino", "morphe", "3", "1", "", "morpho", "Moyen", "Frequent",
                "actinomycète", "CHAMPIGNON  DU RAYON", "actino", "mycète", "3", "2", "", "", "Moyen", "Moyen",
                "actinomycose", "CHAMPIGNON  DU RAYON", "actino", "mycose", "3", "2", "", "", "Moyen", "Moyen",
                "actinopode", "PIED  DU RAYON", "actino", "pode", "3", "2", "", "", "Moyen", "Frequent",
                "actinoscopie", "EXAMEN  DU RAYON", "actino", "scopie", "3", "1", "", "scope", "Moyen", "Frequent",
                "actinothérapie", "MÉDECINE  DU RAYON", "actino", "thérapie", "3", "1", "", "thérapie", "Moyen", "Frequent",
                "actinotropisme", "ATTIRANCE  DU RAYON", "actino", "tropisme", "3", "2", "", "trope", "Moyen", "Frequent",
                "acyclique", "CIRCULARITÉ  SANS", "a", "cyclique", "1", "1", "a", "cyclo", "Frequent", "Frequent"
        }
#End If

        Dim iNbItems% = lstMots.Count
        Dim iNbMots% = iNbItems / clsMotExistant.iNbColonnesME
        For i As Integer = 0 To iNbMots - 1
            Dim mot As clsMotExistant = Nothing
            If Not bLireMot(lstMots, i, mot) Then
                If bDebug Then Stop
                Continue For
            End If
            dicoMotsExistants.Add(mot.sMot, mot)
        Next

    End Sub

End Module
