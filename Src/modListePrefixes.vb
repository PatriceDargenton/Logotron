
Module modListePrefixes

    Public Sub InitialisationPrefixes(sCheminLogotronCsv$, sModeLecture$,
        msgDelegue As clsMsgDelegue)

        ' Préfixe : Adjectifs ou noms avec déterminant pour compléments de nom

        If sModeLecture = enumModeLecture.sCsv Then
            LireLogotronCsv(sCheminLogotronCsv, msgDelegue)
            Exit Sub
        End If
        If sModeLecture = enumModeLecture.sJSon Then
            LireLogotronJSon()
            Exit Sub
        End If

        ' Cette liste peut être récupérée via PrefixesSuffixes2.txt

        Dim prefixes = New List(Of String) From {
            "a", "sans", "D", "1", "Du grec ancien ἀ-, a- exprimant la privation.", "a", "Grec", "Frequent",
            "acanth", "l'épine", "D", "3", "Du grec ἄκανθος, akanthos (« épine »).", "acantho", "Grec", "Rare",
            "acantho", "l'épine", "L", "3", "Du grec ἄκανθος, akanthos (« épine »).", "acantho", "Grec", "Moyen",
            "acou", "l'audition", "L", "2", "Du grec ancien ἀκούω, akoúô (« entendre »).", "", "Grec", "Moyen",
            "acro", "élevé(e) / extrême", "L", "3", "Du grec ancien ἄκρος, acros (« élevé, extrême »).", "", "Grec", "Frequent",
            "actin", "le rayon", "D", "3", "Du grec ancien ἀκτίς, ἀκτίνος, actis, actinos (« rayon »).", "", "Grec", "Moyen",
            "actini", "le rayon", "D", "3", "Du grec ancien ἀκτίς, ἀκτίνος, actis, actinos (« rayon »).", "", "Grec", "Rare",
            "actino", "le rayon", "L", "3", "Du grec ancien ἀκτίς, ἀκτίνος, actis, actinos (« rayon »).", "", "Grec", "Moyen",
            "acu", "l'aiguille", "L", "2", "Du latin acus (« aiguille »).", "", "Latin", "Absent",
            "addicto", "l'addiction", "L", "1", "De l'anglais addict.", "", "Anglais", "Rare",
            "adén", "la glande", "D", "3", "", "", "Gréco-latin", "Moyen",
            "adéno", "la glande", "L", "3", "", "", "Gréco-latin", "Moyen",
            "aéro", "l'air", "L", "1", "Du grec ancien ἀήρ, aêr.", "", "Grec", "Frequent",
            "agora", "le lieu public", "L", "2", "Du grec ancien ἀγορά agorá (« assemblée, place publique »).", "", "Grec", "Rare",
            "agri", "le champ", "L", "1", "Du latin ager, agri, (« champs »).", "", "Latin", "Rare",
            "agro", "le champ", "L", "1", "Du grec ancien ἀγρός, agros (« champs »).", "", "Grec", "Frequent",
            "algo", "la douleur", "L", "2", "Du grec ancien ἄλγος algos (« douleur »).", "", "Grec", "Moyen",
            "allo", "l'autre", "L", "3", "Du grec ancien ἄλλος, allos (« autre »).", "allo", "Grec", "Frequent",
            "alter", "l'autre", "L", "2", "Du latin alter (« autre »).", "", "Latin", "Absent",
            "alti", "élevé(e)", "L", "2", "Du latin altus (« élevé »).", "", "Latin", "Moyen",
            "ambi", "double", "L", "2", "Du latin ambi-.", "ambi", "Latin", "Rare",
            "amnio", "le bassin", "L", "3", "Du grec ancien ἀμνίον, amníov (« bassin »).", "", "Grec", "Moyen",
            "amph", "des deux côtés", "D", "3", "Du grec ancien ἀμφίς, amphís (« des deux côtés »).", "", "Grec", "Rare",
            "amphi", "des deux côtés", "L", "3", "Du grec ancien ἀμφίς, amphís (« des deux côtés »).", "", "Grec", "Moyen",
            "ampho", "des deux côtés", "L", "3", "Du grec ancien ἀμφίς, amphís (« des deux côtés »).", "", "Grec", "Rare",
            "amyl", "la farine / l'amidon", "D", "3", "Du grec ancien ἄμυλον, amylon (« farine »).", "", "Grec", "Rare",
            "amylo", "la farine / l'amidon", "L", "3", "Du grec ancien ἄμυλον, amylon (« farine »).", "", "Grec", "Moyen",
            "an", "sans", "D", "2", "", "an", "Gréco-latin", "Frequent",
            "ana", "l'autre / à travers / à nouveau / vers le haut", "L", "3", "", "", "Gréco-latin", "Frequent",
            "andr", "l'homme / le male", "D", "2", "Du grec ancien ἀνδρός, andrós, génitif singulier de ἀνήρ, anếr (« homme »).", "andro", "Grec", "Moyen",
            "andro", "l'homme / le male", "L", "2", "Du grec ancien ἀνδρός, andrós, génitif singulier de ἀνήρ, anếr (« homme »).", "andro", "Grec", "Frequent",
            "anémo", "le vent", "L", "3", "Du grec ancien ἄνεμος, ánemos (« vent »).", "", "Grec", "Moyen",
            "angéio", "le vaisseau", "L", "3", "Du grec ancien ἀγγεῖον, angeîon (« vase, capsule, vaisseau »).", "", "Grec", "Moyen",
            "angi", "le vaisseau", "L", "3", "Du grec ancien ἀγγεῖον, angeîon (« vase, capsule, vaisseau »).", "", "Grec", "Moyen",
            "angio", "le vaisseau", "L", "3", "Du grec ancien ἀγγεῖον, angeîon (« vase, capsule, vaisseau »).", "", "Grec", "Frequent",
            "angusti", "étroit(e)", "L", "3", "Du latin angustus (« étroit »).", "angusti", "Latin", "Rare",
            "ankylo", "courbé(e) -> raide / affaibli(e) / insensible", "L", "3", "Du grec ancien ἀγκύλος, ankýlos (« courbé »).", "", "Grec", "Rare",
            "ant", "contre", "D", "1", "Du grec ancien ἀντί, anti- (« contre »).", "anti", "Latin", "Absent",
            "anté", "antérieur", "L", "2", "Du latin ante (« avant »).", "", "Latin", "Rare",
            "anth", "la fleur", "D", "2", "Du grec ancien ἄνθος, ánthos (« fleur »).", "antho", "Grec", "Rare",
            "antho", "la fleur", "L", "2", "Du grec ancien ἄνθος, ánthos (« fleur »).", "antho", "Grec", "Moyen",
            "anthraco", "le charbon", "L", "2", "Du grec ancien ἄνθραξ, ánthrax (« charbon »).", "", "Grec", "Rare",
            "anthrop", "l'homme", "D", "2", "Du grec ancien ἄνθρωπος, ánthrôpos (« être humain »).", "anthropo", "Grec", "Rare",
            "anthropo", "l'homme", "L", "2", "Du grec ancien ἄνθρωπος, ánthrôpos (« être humain »).", "anthropo", "Grec", "Frequent",
            "anti", "contre", "L", "1", "Du grec ancien ἀντί, anti (« au lieu de », « en comparaison de », « contre ») dont est issu anti.", "anti", "Grec", "Frequent",
            "anto", "contre", "L", "1", "Cf. anti : antonyme : Du grec ancien ἀντί, anti- (« contre ») et ὄνομα, onoma (« nom »).", "", "Grec", "Absent",
            "anxio", "inquiet", "L", "2", "Du latin anxiosus, de anxius (« inquiet »), radical de l’adjectif anxieux.", "", "Latin", "Rare",
            "api", "l'abeille", "L", "2", "Du latin apis (« abeille »).", "", "Latin", "Moyen",
            "apic", "le sommet", "D", "3", "Du latin apex (« sommet »).", "", "Latin", "Absent",
            "apico", "le sommet", "L", "3", "Du latin apex (« sommet »).", "", "Latin", "Absent",
            "apo", "hors", "L", "3", "Du grec ancien ἀπό, apó (« hors de »).", "apo", "Grec", "Frequent",
            "aqu", "l'eau", "D", "1", "Du latin aqua (« eau »).", "aqua", "Latin", "Rare",
            "aqua", "l'eau", "L", "1", "Du latin aqua (« eau »).", "aqua", "Latin", "Moyen",
            "arbori", "l'arbre", "L", "1", "Du latin arbor (« arbre »).", "arbori", "Latin", "Rare",
            "arché", "ancien(ne)", "D", "1", "", "archéo", "Gréco-latin", "Rare",
            "archéo", "ancien(ne)", "L", "1", "", "archéo", "Gréco-latin", "Moyen",
            "archi", "le gouverneur -> supérieur(e)", "L", "2", "Du grec ancien ἀρχι-, arkhi-, dérivé de ἄρχω, árkhô (« commencer, mener, gouverner »).", "arche", "Grec", "Rare",
            "aréo", "léger(ère)", "L", "3", "Du grec ancien ἀραιόω, areo, « rendre moins dense, raréfier », basé sur ἀραιός, areos, « mince, étroit, peu profond », à ne pas confondre avec : aéro- : l'air : Du grec ancien ἀήρ, aêr.", "", "Grec", "Moyen",
            "aristo", "excellent(e)", "L", "2", "Du grec. aristos (« excellent »).", "", "Grec", "Rare",
            "arithm", "le nombre", "D", "2", "Du grec ancien ἀριθμός, arithmos (« nombre »).", "", "Grec", "Absent",
            "arithmé", "le nombre", "L", "2", "Du grec ancien ἀριθμός, arithmos (« nombre »).", "", "Grec", "Absent",
            "arithmo", "le nombre", "L", "2", "Du grec ancien ἀριθμός, arithmos (« nombre »).", "", "Grec", "Moyen",
            "arthro", "l'articulation", "L", "2", "", "", "Gréco-latin", "Moyen",
            "astro", "le ciel", "L", "1", "", "astro", "Gréco-latin", "Frequent",
            "atmo", "la vapeur, l'air", "L", "2", "Du grec ancien ἀτμός, atmós (« vapeur »).", "", "Grec", "Moyen",
            "atto", "10^-18", "L", "3", "Du danois atten (« dix-huit »), car dix-huit est l’exposant de 10−1 dans 10−18.", "", "Danois", "Absent",
            "audi", "le son", "D", "1", "Du latin audire (« écouter »).", "audio", "Latin", "Rare",
            "audio", "le son", "L", "1", "Du latin audire (« écouter »).", "audio", "Latin", "Moyen",
            "auri", "l'or", "L", "2", "", "auri", "Gréco-latin", "Rare",
            "auriculo", "l'oreille", "L", "3", "Du latin auricula (lobe de l'oreille). Formé du radical auris (oreille) et du suffixe -culus (petit).", "", "Latin", "Rare",
            "auto", "de soi-même", "L", "1", "", "", "Gréco-latin", "Frequent"
        }
        m_prefixes.DefinirSegments(prefixes, iNbColonnes)

    End Sub

End Module
