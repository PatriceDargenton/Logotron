
Module modListeSuffixes

    Public Sub InitialisationSuffixes(sModeLecture$)

        ' Suffixes :

        If sModeLecture <> enumModeLecture.sCode Then Exit Sub

        ' Cette liste peut être récupérée via PrefixesSuffixes2.txt

        Dim suffixes = New List(Of String) From {
            "able", "qui peut être", "D", "1", "Du latin -abilis (« capable de »), suffixe formateur de noms sur la base de verbes en -are.", "", "Latin", "Frequent",
            "acanthe", "épine", "L", "3", "Du grec ἄκανθος, akanthos (« épine »).", "acantho", "Grec", "Rare",
            "acousie", "audition", "L", "2", "Du grec ancien ἀκούω, akoúô (« entendre »).", "", "Grec", "Moyen",
            "adelphe", "frère", "L", "3", "Du grec ancien ἀδελφός, adelphós (« utérin, frère »).", "", "Grec", "Rare",
            "adelphie", "frère", "L", "3", "Du grec ancien ἀδελφός, adelphós (« utérin, frère »).", "", "Grec", "Absent",
            "agogie", "guidage", "L", "2", "Du grec ancien ἀγωγή, agôgê (« action de mener ») ou ἀγωγός, agôgos (« qui conduit, qui guide ») dérivé de ἄγω, agô (« mener »).", "agogie : guider", "Grec", "Moyen",
            "agogique", "guidage", "D", "2", "Du grec ancien ἀγωγή, agôgê (« action de mener ») ou ἀγωγός, agôgos (« qui conduit, qui guide ») dérivé de ἄγω, agô (« mener »).", "agogie : guider", "Grec", "Moyen",
            "agogue", "meneur", "L", "2", "Du grec ancien ἀγωγή, agôgê (« action de mener ») ou ἀγωγός, agôgos (« qui conduit, qui guide ») dérivé de ἄγω, agô (« mener »).", "agogie : guider", "Grec", "Moyen",
            "agogue", "conduction -> écoulement", "L", "2", "Du grec ancien ἀγωγή, agôgê (« action de mener ») ou ἀγωγός, agôgos (« qui conduit, qui guide ») dérivé de ἄγω, agô (« mener »).", "agogie : écouler", "Grec", "Moyen",
            "algésie", "douleur", "L", "2", "Du grec ancien ἄλγος algos (« douleur »).", "", "Grec", "Moyen",
            "algésique", "douleur", "D", "2", "Du grec ancien ἄλγος algos (« douleur »).", "", "Grec", "Rare",
            "algie", "douleur", "L", "2", "Du grec ancien ἄλγος algos (« douleur »).", "", "Grec", "Frequent",
            "algique", "douleur", "D", "2", "Du grec ancien ἄλγος algos (« douleur »).", "", "Grec", "Moyen",
            "amniotique", "bassin", "D", "3", "Du grec ancien ἀμνίον, amníov (« bassin »).", "", "Grec", "Rare",
            "andre", "homme -> male", "L", "2", "Du grec ancien ἀνδρός, andrós, génitif singulier de ἀνήρ, anếr (« homme »).", "andro", "Grec", "Moyen",
            "andrie", "homme -> male", "L", "2", "Du grec ancien ἀνδρός, andrós, génitif singulier de ἀνήρ, anếr (« homme »).", "andro", "Grec", "Moyen",
            "anthe", "fleur", "L", "2", "Du grec ancien ἄνθος, ánthos (« fleur »).", "antho", "Grec", "Rare",
            "anthème", "fleur", "L", "2", "Du grec ancien ἄνθημα, anthêma (« inflorescence »).", "antho", "Grec", "Rare",
            "anthrope", "homme", "L", "2", "Du grec ancien ἄνθρωπος, ánthrôpos (« être humain »).", "anthropo", "Grec", "Rare",
            "anthropie", "homme", "L", "2", "Du grec ancien ἄνθρωπος, ánthrôpos (« être humain »).", "anthropo", "Grec", "Rare",
            "anthropique", "homme", "D", "2", "Du grec ancien ἄνθρωπος, ánthrôpos (« être humain »).", "anthropo", "Grec", "Rare",
            "arcat", "gouvernement", "L", "2", "Du grec ancien ἀρχι-, arkhi-, dérivé de ἄρχω, árkhô (« commencer, mener, gouverner »).", "arche", "Grec", "Rare",
            "arche", "gouverneur", "L", "2", "Du grec ancien ἀρχι-, arkhi-, dérivé de ἄρχω, árkhô (« commencer, mener, gouverner »).", "arche", "Grec", "Rare",
            "archie", "gouvernance", "L", "2", "Du grec ancien ἀρχι-, arkhi-, dérivé de ἄρχω, árkhô (« commencer, mener, gouverner »).", "arche", "Grec", "Moyen",
            "archique", "gouvernance", "D", "2", "Du grec ancien ἀρχι-, arkhi-, dérivé de ἄρχω, árkhô (« commencer, mener, gouverner »).", "arche", "Grec", "Moyen",
            "archisme", "gouvernance", "L", "2", "Du grec ancien ἀρχι-, arkhi-, dérivé de ἄρχω, árkhô (« commencer, mener, gouverner »).", "arche", "Grec", "Rare",
            "arque", "gouverneur", "L", "2", "Du grec ancien ἀρχι-, arkhi-, dérivé de ἄρχω, árkhô (« commencer, mener, gouverner »).", "arche", "Grec", "Moyen",
            "asthénie", "faiblesse", "L", "3", "Du grec ancien ἀσθένεια, asthéneia (« faiblesse, absence de force »).", "", "Grec", "Moyen",
            "ateur", "agent", "D", "1", "Du latin -or ou -ator qui donne -our en ancien français, puis -eur en moyen français.", "", "Latin", "Moyen",
            "athlon", "sport", "L", "2", "Du latin athlon (« concours »).", "", "Latin", "Rare",
            "ation", "action", "D", "2", "Du latin -atio.", "ation", "Latin", "Frequent",
            "âtre", "un peu", "L", "2", "Du latin -aster.", "", "Latin", "Rare",
            "atrice", "agente", "D", "2", "", "", "Gréco-latin", "Moyen"
        }
        m_suffixes.DefinirSegments(suffixes, iNbColonnes)

    End Sub

End Module
