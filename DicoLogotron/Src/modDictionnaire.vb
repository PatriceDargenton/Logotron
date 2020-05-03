
Imports System.Text

Module modDictionnaire

    Const bElision As Boolean = modConstLogotron.bElision

    Const bDebugPrefixeEtSuffixePot As Boolean = False
    Const bAfficherDoublonMotsComplexes As Boolean = True
    Const bDebugSensMultiple As Boolean = False
    Const iNivMaxAffDoublonMotsComplexes% = 5 ' Ex.: 2 = 2 préfixes et 1 suffixe
    Const iNivMaxMotsComplexesUnicite% = 5 ' 0 : Désactivé

    Const iLongSuffixeMin% = 3
    Const iNbExemplesMax% = 3

    Const iNbCarMinPrefixePot% = 3
    Const iNbCarMinSuffixePot% = 3

    ' Nombre de mots (non fléchis) en français : Combien de mots en français ?
    ' https://fr.wiktionary.org/wiki/Wiktionnaire:Statistiques#Combien_de_mots_en_fran.C3.A7ais_.3F
    ' Et en anglais, difficile de compter : 
    ' https://en.oxforddictionaries.com/explore/how-many-words-are-there-in-the-english-language
    ' On peut comptabiliser autant de mot que le français, pas grave
    Const iNbMotsFrancais% = 68075

    Private m_prefixesPot As clsBase, m_suffixesPot As clsBase

    ' Exclusion des définitions fausses
    Private m_hsExclDef As HashSet(Of String)
    Private m_hsExclDefUtil As HashSet(Of String)

    Private m_msgDelegue As clsMsgDelegue
    Private m_bAfficherAvertDoublon As Boolean = False

#Region "Classes"

    Private Class clsHsSb ' Classe gérant un HashSet avec un StringBuilder

        Public hs As New HashSet(Of String)
        Public sb As New StringBuilder
        Public iNbLignes% ' En comptant les doublons autorisés

        Public Function bAjouterLigne(sCle$, sLigne$) As Boolean
            ' Ajouter une ligne si la clé n'existe pas déjà
            If hs.Contains(sCle) Then Return False
            hs.Add(sCle)
            sb.AppendLine(sLigne)
            iNbLignes += 1
            Return True
        End Function

        Public Sub AjouterLigneDoublon(sCle$, sLigne$)
            ' Ajouter une ligne même si la clé existe déjà 
            '(ex.: cette ligne doublonne, mais c'est normal :
            ' chloro : VERT(E) : chloro-
            ' chloro : CHLORÉ(E) : chloro-)
            If Not hs.Contains(sCle) Then hs.Add(sCle)
            sb.AppendLine(sLigne)
            iNbLignes += 1
        End Sub

    End Class

    Private Class clsPrm
        Public sMotDico$, sMotDicoMinuscule$, sMotDicoUniforme$, sPrefixe$, sSuffixe$
        Public bPrefixe, bSuffixe, bPluriel As Boolean
        Public iLongSuffixe, iLongPrefixe, iLongMot As Integer
        Public iNbMotsPrefixeExistants%
        Public iNbMotsSuffixeExistants%
        Public motsSimples As New clsHsSb   ' mots logotroniques simples   du dico : préfixe   + suffixe
        Public motsComplexes As New clsHsSb ' mots logotroniques complexes du dico : préfixes  + suffixe
        Public mots As New clsHsSb          ' mots logotroniques           du dico : préfixe(s)+ suffixe
        Public iNbMotsLogotronExistantsUnicite%
        Public aPrefixes(), aSuffixes() As clsSegment
        Public lstSuffixeDef As New List(Of String)
        Public lst2PrefixeDef As New List(Of List(Of String))
        Public sbAutres,
            sbPrefixeEtSuffixeFichierTxt, sbPrefixeEtSuffixeFichierCsv,
            sbMotSimpleFichierCode,
            sbPrefixesEtSuffixe2Unicite,
            sbPrefixes, sbSuffixes As New StringBuilder
        Public dicoDefIncompletes As New DicoTri(Of String, clsLigne) ' 04/03/2018
        Public dicoPrefSuff As New DicoTri(Of String, clsSegmentStat)
        Public dicoPrefixesManquants As New DicoTri(Of String, clsSegmentStat)

        Public dicoSegmentsSensMultiples As New DicoTri(Of String, clsSegmentStat) ' 30/12/2018

        ' Compexité des mots logotroniques du dico : (niv préfixe + 1) x (niv suffixe + 1)
        Public dicoComplex As New DicoTri(Of String, clsMot)
        Public iNiveauPrefixe%, iNiveauSuffixe%
        Public sFreqPrefixe$, sFreqSuffixe$ ' 21/08/2018
        Public lst2UnicitesPrefixe As New List(Of List(Of String))
        Public lstUnicitesSuffixe As New List(Of String)
        Public iPasse% ' 26/08/2018
        Public hsUnicitesDefMots As New HashSet(Of String) ' 26/08/2018 Mots simples et complexes
        Public sMotTrouve$ ' 25/08/2018
        Public bMotTrouve As Boolean ' 25/08/2018
        Public bPremiereCombi As Boolean
        Public hsUnicitesDoublons As New HashSet(Of String)  ' 29/08/2018 

        ' Mots complexes
        ' 16/12/2012 sCle : sMot -> 1ère définition
        Public dicoUnicitesDoublons2 As New DicoTri(Of String, String)
        Public hsUnicitesDoublons2 As New HashSet(Of String)
        Public dicoUnicitesDoublons2Suff As New DicoTri(Of String, String) ' sClé : sMot -> Suffixe
        Public dicoUnicitesDoublons2Pref As New DicoTri(Of String, String) ' sClé : sMot -> Préfixe
    End Class

    Private Class clsMot
        Public sMot$, sListeSegments$, sDef$
        Public iComplexite%, iNiv%
        Public Sub New(sMot0$, iComplexite0%, sListeSegments0$, sDef0$, iNiv0%)
            sMot = sMot0
            iComplexite = iComplexite0
            sListeSegments = sListeSegments0
            sDef = sDef0
            iNiv = iNiv0
        End Sub
    End Class

    Private Class clsLigne
        Public sLigne$, sCle$, sManque$, sMot$, sMotUniforme$
        Public rRatio% ' rRatio est utilisé dans le tri
        Public Sub New(sLigne$, sCle$, sManque$, sMot$)
            Me.sLigne = sLigne
            Me.sCle = sCle
            Me.sManque = sManque
            Me.sMot = sMot
            Me.sMotUniforme = sEnleverAccents(sMot, bMinuscule:=True) ' 07/12/2019
            'Me.rRatio = sManque.Length / sCle.Length
            Me.rRatio = sCle.Length - sManque.Length ' 28/04/2018
        End Sub
    End Class

    Private Class clsSegment ' Préfixe ou suffixe
        Public sSegment$, sSegmentUniforme$, sFrequence$
        Public iNiveau%, iLong% ' iLong est utilisé dans le tri
        Public lstDefinitions As New List(Of String)
        Public lstUnicites As New List(Of String)
    End Class

    Private Class clsSegmentManquant
        Public sCle$
        Public iNbOccDicoFr%
        Public lstMotsDico As New List(Of String)
    End Class

    Private Class clsSegmentStat ' Statistiques des préfixes ou suffixes
        Public sSegment$, iNbOccDicoFr%
        Public bPrefixe As Boolean ' Sinon suffixe
        Public lstMots As New List(Of String)
        ' Si le mot est formé complètement par un préfixe et un suffixe, alors ils sont composables
        ' (on va les lister en 1er dans le bilan, car ils sont plus intéressants pour former des mots)
        Public bComposable As Boolean ' bComposable est utilisé dans le tri
        Public dicoSegManquant As DicoTri(Of String, clsSegmentManquant)
        Public sSens$, sSegmentUnicite$ ' 30/12/2018
    End Class

#End Region

#Region "Initialisations"

    Public Sub InitBasesPot()
        m_prefixesPot = New clsBase(iNbColonnes:=0, bPrefixe:=True)
        m_suffixesPot = New clsBase(iNbColonnes:=0, bPrefixe:=False)
    End Sub

    Public Sub InitialisationPrefixesEtSuffixesPot()

        ' Liste du Dr Aly Abbara

        Dim prefixesPot As New List(Of String) From {
            "a", "a", "ac", "ad", "af", "ag", "al", "am", "ap", "ar", "as", "at", "ab", "abs", "acantho", "acanth", "acou", "acro", "actin", "actini", "actino", "acu", "acumin", "acumini", "acut", "acuti", "ad", "adelph", "adelpho", "adén", "adéno", "adrén", "adréno", "aéro", "agri", "agro", "algo", "alg", "algo", "ali", "allanto", "allo", "all", "ambi", "amnio", "amph", "amphi", "ampho", "amyl", "amylo", "an", "an", "ano", "ana", "andro", "anémo", "angéio", "angi", "angio", "angusti", "anhydro", "anis", "aniso", "ankylo", "ancylo", "anté", "anti", "anth", "antho", "anthraco", "anthropo", "anti", "anto", "aort", "aorto", "api", "apic", "apico", "apo", "aqu", "arché", "archéo", "archi", "argyr", "argyro", "arithm", "arithmé", "arithmo", "artério", "artéri", "arthr", "arthro", "artio", "asbest", "astro", "astéri", "athér", "athéro", "atrio", "atri", "atro", "atto", "audi", "audio", "aur", "auri", "auricul", "auriculo", "auto", "avi", "ax", "axo", "axin",
            "bactéri", "bactério", "baro", "batho", "bathy", "bene", "biblio", "bien", "bi", "bio", "bis", "blast", "blasto", "blenno", "bléphar", "blépharo", "bolo", "brachy", "brady", "bronch", "broncho", "bronchi", "bryo", "bry", "butyro", "byssin",
            "caco", "cach", "calc", "calci", "calcar", "calcari", "calic", "calico", "calli", "calo", "kallo", "calor", "calori", "calyc", "calyco", "calypto", "campano", "campani", "camph", "camphor", "cancéro", "cancéri", "canon", "carb", "carbo", "carcino", "cardi", "cardia", "cardio", "carp", "carpo", "carpi", "carni", "caryo", "cat", "cata", "kata", "cato", "caul", "cauli", "caust", "caut", "celluli", "cellulo", "céno", "cœno", "centi", "centr", "centri", "centro", "céphal", "céphalo", "cérat", "kérat", "cérato", "kérato", "cerco", "cér", "céri", "céro", "cérébri", "cérébro", "cervic", "cervico", "céto", "chalco", "chart", "cart", "charto", "carto", "chéil", "chil", "chéilo", "chilo", "chéir", "chéiro", "chél", "chéli", "chélo", "chélon", "chélono", "chimi", "chimio", "chir", "chiro", "chlor", "chloro", "chol", "cholé", "chondr", "chondro", "chor", "choro", "choré", "chori", "chorio", "chresto", "chromo", "chromato", "chrono", "chrys", "chryso", "ciné", "kiné", "cinémato", "cinémo", "circon", "circum", "cis", "citr", "citro", "clasto", "cléisto", "clin", "clino", "co", "con", "com", "col", "cor", "cocci", "cocco", "cœlo", "cœlio", "col", "coléo", "colo", "col", "colori", "colpo", "com", "con", "conch", "conchyli", "conchylio", "coni", "contra", "contre", "copro", "cor", "coraco", "coralli", "cordi", "coron", "corono", "coronar", "coronaro", "cortici", "cortico", "cosmo", "cost", "costo", "cox", "coxo", "coxal", "crân", "cranio", "créno", "crio", "cristallo", "cruci", "cryo", "crypt", "crypto", "cubit", "cubito", "cune", "cuniculi", "cunni", "cupr", "cupri", "cupro", "curv", "curvi", "cyan", "cyano", "cyber", "cycl", "cyclo", "cycno", "cylindro", "cyno", "cyn", "cyst", "cysti", "cysto", "cyt", "cyto",
            "dactyl", "dactylo", "de", "dé", "des", "dés", "déca", "déci", "demi", "démo", "dendro", "déonto", "dér", "déro", "derma", "dermo", "dermato", "des", "deutér", "deutéro", "dextro", "di", "dia", "dial", "dicho", "digit", "digiti", "digito", "dino", "diplo", "dis", "dodéc", "dodéca", "dolicho", "dynamo", "dys",
            "é", "échino", "écho", "éco", "ecto", "ectro", "ef", "em", "embryo", "émé", "emmén", "en", "en", "encéphal", "encéphalo", "endo", "entér", "entéro", "entomo", "entre", "épi", "épisio", "équ", "équin", "équi", "erg", "ergo", "éreutho", "éry", "éryth", "érythr", "érythro", "éso", "esthési", "esthésio", "éthi", "étho", "ethn", "ethno", "étymo", "eu", "eury", "ex", "ex", "exo", "exa", "extra", "extrin",
            "fau", "fémini", "femto", "ferri", "ferro", "fibrino", "fibro", "fibromato", "flavo", "flor", "flori", "fluo", "fluor", "fluori", "fluoro", "fluv", "fluvio", "folia", "fong", "fongi", "fongo", "for", "four", "fraga", "fragi", "franco", "frigo", "fruct", "fructi", "frugi", "fug", "fugi", "fulmi", "fumi", "fun", "funi",
            "gala", "galact", "galacto", "gamo", "gastéro", "gastr", "gastro", "génio", "généa", "géno", "géo", "géronto", "giga", "gloss", "glosso", "glotto", "gluc", "gluco", "glyc", "glyco", "glycéro", "glyph", "glypho", "glypto", "gono", "gonio", "graph", "grapho", "gym", "gymno", "gyn", "gyno", "gynéc", "gynéco", "gyr", "gyro", "gir", "giro",
            "hagio", "halo", "halo", "hali", "haplo", "hebdo", "hecto", "hect", "hecato", "hébé", "hélici", "hépat", "hépato", "hélico", "hélio", "hémat", "hémato", "hémo", "héma", "hémér", "héméro", "hémi", "hépat", "hépato", "hepta", "hétéro", "hex", "hexa", "hiéro", "hiér", "hipp", "hippo", "hirudini", "hispan", "hispano", "hist", "histo", "histio", "holo", "homéo", "hom", "homo", "horo", "horti", "hydr", "hydro", "hygr", "hygro", "hylé", "hylo", "hymen", "hyper", "hypno", "hypo", "hystér", "hystéro",
            "iatro", "ichty", "ichtyo", "icon", "icono", "icosa", "icosi", "icos", "ictér", "ictéro", "idéo", "idio", "il", "im", "im", "immun", "immuno", "in", "in", "infra", "infundibuli", "ini", "inio", "inter", "intrin", "ir", "ithy", "ischia", "ischio", "iso", "is",
            "juxta",
            "kallo", "callo", "calo", "kaléido", "caléido", "kil", "kilo", "kinési", "kin", "kyst", "kysto",
            "lalo", "laparo", "laryng", "laryngo", "latér", "latéra", "latéro", "lati", "léio", "léonto", "lepto", "leuc", "leuco", "lévi", "lévo", "lexi", "lexio", "limn", "lip", "lipo", "lio", "lith", "litho", "logo", "longi", "luté", "lutéo", "lyc", "lyco", "lymph", "lympho", "lyo",
            "macro", "mal", "malaco", "malé", "malt", "mast", "masto", "mau", "mé", "méga", "mégalo", "mélan", "mélano", "mél", "mélo", "mén", "méno", "méro", "méri", "mes", "méso", "méta", "méte", "métem", "méten", "météor", "météoro", "méto", "métr", "metro", "métr", "métro", "métr", "mi", "micro", "milli", "mis", "miso", "mném", "mnémo", "mon", "mono", "morph", "muco", "multi", "mycét", "mycéto", "myc", "myco", "myél", "myélo", "my", "myo", "myi", "myri", "myst", "mystico", "myth", "mytho", "mytil", "mytili", "mytilo", "myx", "myxo",
            "nan", "nano", "nanno", "naut", "nécr", "nécro", "némo", "néo", "néphr", "néphro", "neur", "neuro", "neutron", "nérv", "nérvo", "neur", "neuro", "nivéal", "nivéo", "nivéol", "non", "nos", "noso", "noto", "novo", "nyct", "nycti", "nicti", "nicto",
            "ob", "oct", "octa", "octi", "octo", "octa", "ochr", "ochro", "odo", "hodo", "odont", "odonto", "œn", "œno", "olé", "oléi", "oléo", "olfacti", "olfacto", "olig", "oligo", "omni", "omo", "omphal", "omphalo", "onc", "onco", "ongui", "onir", "oniro", "ont", "onto", "onych", "onycho", "ophtalm", "ophtalmo", "opso", "opt", "or", "oro", "oré", "oréo", "or", "oro", "ornith", "ornitho", "orth", "ortho", "osm", "osmi", "osmo", "osmo", "ost", "osté", "ostéo", "ot", "oto", "oti", "oxy", "oxyd",
            "pachy", "paléo", "palin", "pan", "pant", "panto", "par", "para", "patho", "patr", "patro", "péd", "pédo", "pédi", "pélo", "pelvi", "pene", "penni", "pén", "péno", "pén", "péné", "penta", "per", "péri", "peta", "péal", "péalo", "phag", "phago", "phall", "phallo", "phanéro", "pharmaco", "pharyngo", "phéno", "phil", "philo", "phléb", "phlébo", "phon", "phono", "phos", "phospho", "photo", "phrén", "phréno", "phtisi", "phtisio", "phyco", "phyll", "phyllo", "physio", "phyt", "phyto", "pico", "piri", "pisci", "pithéco", "plagio", "plani", "plasmo", "pléio", "pléisto", "pléo", "plési", "plésio", "plétho", "pleur", "pleuro", "plouto", "poto", "plouto", "pluri", "pluvio", "pneumat", "pneumato", "pneumo", "podo", "polém", "polémo", "poli", "poly", "post", "potamo", "pour", "pré", "præ", "presby", "pro", "proct", "procto", "protér", "proto", "prot", "pseudo", "psitte", "psych", "psycho", "ptéro", "ptérygo", "ptéryg", "ptyali", "ptyalio", "pub", "publi", "puéri", "puer", "puis", "pusill", "pyél", "pyélo", "pygo", "pyro",
            "quadri", "quadru", "quart", "quater", "quinqu",
            "r", "ra", "re", "ré", "radio", "radio", "radio", "rétro", "rhabdo", "rhin", "rhino", "rhiz", "rhizo", "rhodo", "rhomb", "rhombo", "ribo", "rostri", "rhynch", "rhyncho", "ourynch", "ryncho", "rubé",
            "sacchar", "saccharo", "sacto", "sal", "sali", "salping", "salpingo", "sapro", "sarco", "scalo", "scaph", "scapho", "scaphi", "scapul", "scapulo", "scato", "schis", "schiso", "schizo", "sé", "sélén", "séléno", "séma", "sémant", "sémio", "séméio", "semi", "semper", "sérici", "sesqui", "sial", "sialo", "sidéro", "sidér", "simili", "somato", "soph", "sous", "spélé", "spéléo", "sphéro", "splanchno", "splén", "spléno", "spondyl", "spong", "spongi", "spongio", "spor", "sporo", "stann", "staphyl", "staphylo", "stat", "stato", "stat", "stato", "stauro", "sténo", "stéréo", "stern", "sterno", "stétho", "sthén", "sthéno", "stomato", "stoma", "stréph", "strépho", "strepto", "styl", "stylo", "sub", "super", "supra", "sur", "sus", "sylv", "sylvi", "sym", "syn", "syl", "sy",
            "tachy", "tauto", "taxi", "taxo", "techno", "télé", "ténui", "téra", "térato", "terpsi", "tétra", "thalass", "thalasso", "thanato", "thauma", "thé", "théo", "théo", "therm", "thermo", "thio", "thion", "thixo", "tixo", "thorac", "thoraco", "thora", "thromb", "thrombo", "timo", "toco", "tomo", "top", "topo", "tox", "toxo", "toxi", "toxico", "toxo", "trans", "tre", "tres", "tri", "trich", "trichi", "tricho", "triskaidéka", "troph", "tropho", "typ", "typo", "typhl", "typhlo", "typho", "typto", "tyr", "tyro",
            "u", "ulo", "ultra", "unci", "undéc", "undéci", "ungu", "ungui", "uni", "urano", "ur", "uro", "uri", "uré", "uréo", "urétér", "urétéro", "urétr", "urétro", "uric", "urico", "ur", "uro", "ur", "uro", "uro", "urtic", "utéro", "uvi", "uvé", "uvul", "uxori",
            "vermi", "vésic", "vésico", "vi", "via", "vibro", "vice", "vini", "vino", "vir", "vir", "viro", "viru", "viscéro", "visco", "viti", "vor", "vultu",
            "xanth", "xantho", "xén", "xéno", "xér", "xéro", "xipho", "xyl", "xylo",
            "yocto", "yotta",
            "zepto", "zéro", "zetta", "zo", "zoo", "zon", "zono", "zyg", "zygo", "zym", "zymo"
            }
        m_prefixesPot.DefinirSegments(prefixesPot, iNbColonnes:=1)

        ' "folia", "logo" "omphal" : déjà préfixe, pas de trouvé en tant que suffixe
        ' "zoaires" : pluriel retiré
        Dim suffixesPot As New List(Of String) From {
            "able", "issable", "ible", "uble", "ace", "asse", "acousie", "ade", "adelphe", "adelphie", "age", "agogie", "agogue", "aie", "ail", "aille", "ain", "aine", "aire", "ais", "aison", "ail", "al", "algie", "algique", "algésie", "algésique", "alto", "amniotique", "an", "ance", "ence", "ande", "ende", "andre", "andrie", "ane", "anthe", "anthème", "anthrope", "anthropie", "archie", "arque", "ard", "argyre", "as", "ase", "asse", "astre", "at", "ateur", "ation", "âtre", "ature ou", "ure", "aud", "axe",
            "bacter", "bar", "bare", "bary", "bio", "bie", "biose", "biote", "blaste", "bole", "bolie", "bolisme", "bolite", "boulie",
            "carde", "cardie", "carpe", "caryote", "caule", "causte", "ceau", "cèle", "cène", "centèse", "centrique", "centrisme", "celle", "céphale", "céphalie", "céphalique", "cerque", "cère", "cète", "chire", "chirie", "choréen", "chrèse", "chrome", "chromie", "chrone", "chronique", "chronisme", "cide", "cinésie", "kinésie", "clasie", "claste", "clase", "clave", "cléisis", "coccie", "cole", "colore", "coniose", "cope", "copie", "coque", "corax", "cormie", "cosme", "cule ou", "ule", "culteur", "culture", "crate", "cratie", "crasie", "cratie", "crate", "cratique", "crino", "crine", "crinie", "crète", "cule", "culture", "cole", "culteur", "cycle", "cyste", "cytaire",
            "dactyle", "dactylie", "dendron", "derme", "dermie", "dèse", "didacte", "dipsie", "dox", "doxe", "doxie", "drome", "dromie", "duc", "didyme", "dyme", "dyne", "dynie",
            "é", "ée", "é", "eau", "elle", "èce", "écie", "ectasie", "ectomie", "èdre", "ée", "el", "elle", "ème", "ement ou", "ment", "émèse", "émie", "ène", "entère", "er", "ier", "ière", "érèse", "ergie", "erie", "esque", "esse", "esthésie", "et", "ette", "té", "eté", "ité", "eur ou", "ateur", "eux", "euse",
            "fère", "fiction", "fier", "fication", "ficateur", "fique", "flore", "florum", "folium", "forme", "fuge", "fuge",
            "game", "gamie", "gastre", "gastrie", "gastrique", "gène", "génie", "gène", "genèse", "génèse", "génésie", "génique", "gée", "glosse", "glossie", "glotte", "glyphe", "gnathe", "gnathie", "ganthisme", "gnose", "gnosie", "gnostique", "gone", "gone", "gonie", "grade", "gramme", "graphe ", "graphie", "gym", "gyne", "gyre", "gire", "hélie", "ème", "émie",
            "hémère", "émère", "hypnie",
            "iatre  ", "iatrie", "ible", "issable", "ide", "idés", "ides", "idie", "ie", "ième", "ien", "en", " ier", "ière", "if", "ive", "il", "ille", "illon", "in", "ine", "in", "ine", "iner", "iole", "ique", "ir", "is", "ise", "isme", "iste", "ite", "ité", "itude", "issime", "issimo", "kinésie",
            "lalie", "latère", "lâtrie", "lâtre", "lécithe", "lecte", "lexie", "lingue", "lithe", "lite", "loculaire", "logie", "logiste", "logue", "lyse", "lysie", "lytique",
            "machie", "malacie", "mancie", "mancien", "mane", "mane", "manie", "mastie", "mégalie", "mèle", "mélie", "ment", "mètre", "métrie", "métrique", "mètre", "métrie", "mnésie ", "mnésique", "mobile", "morphe", "morphie", "morphique", "morphisme", "mycète", "myce", "mycose", "mycosique", "myél", "myélo", "myth",
            "naut", "naute", "nautique", "nécrose", "néphr", "néphro", "nésie", "nome", "nomie", "nomique", "nose", "nyque",
            "ocle", "oculaire", "ode", "hode", "odème", "odonte", "odontie", "oïde", "ide", "oir, oire", "ois, oise", "ole", "ome", "on", "onychie", "onyme", "onymie", "onymique", "ophtalmie", "optrie", "optrique", "opsie", "opie", "ope", "optique", "orama", "rama", "orexie", "ornis", "ose", "ose", "osmie", "oste", "ote", "ot", "otte", "oure",
            "page", "pare", "partum", "pathe", "pathe", "pathie", "pathique", "pédie", "pède", "pél", "pénie", "penne", "pepsie", "pétale", "phage", "phagie", "phagique", "phane", "phanie", "phasie", "phémie", "phile", "philie", "phobe", "phobie", "phone", "phonie", "phore", "phorie", "phrénie", "phylaxie", "phylle", "physe", "phyte", "phyton", "pithèque", "plasie", "plasme", "plasmose", "plaste", "plastie", "plégie", "pnée", "pnéique", "pode", "poïèse", "pole", "polite", "potame", "potamie", "praxie", "ptère", "ptérygien", "ptysie", "ptysique", "puncture", "poncture", "pyge", "pygie",
            "rhize", "ron", "rostre", "rragie", "rrhagie", "rraphie", "rrhée", "rre",
            "salpinx", "saure", "scaphe", "scapte", "scope", "scopie", "séléne", "sémie", "sèque", "sialie", "some", "somie", "somatique", "som", "son", "sophe", "sophie", "sion", "ssion", "sphère", "stase", "stat", "stat", "statique", "stérie", "sthénie", "stomie", "stome", "style",
            "taphe", "taxie", "taxe", "technie", "technique", "théose", "théisme", "thé", "théo", "thèque", "thérapie", "therme", "tocie", "tocique", "tomie", "tome", "topie", "topique", "tion", "triche", "trique", "trope", "tropie", "tropisme", "trophe", "trophie", "trophine", "tropine", "type", "typie",
            "u", "ule", "ure", "ature", "urge", "urgue", "urgie", "urie", "ur", "uro",
            "valent", "valente", "via", "vir", "viscido", "vore", "vulte",
            "xène", "xénie", "xion", "xyle",
            "zoo", "zoaire", "zoïque", "zoïde", "zyme"
        }
        m_suffixesPot.DefinirSegments(suffixesPot, iNbColonnes:=1)

    End Sub

    Private Sub AjouterPrefixeDico(
        ByRef dicoPrefixes As DicoTri(Of String, clsSegment),
        ByRef aPrefixes() As clsSegment)

        dicoPrefixes = New DicoTri(Of String, clsSegment)
        Dim iNbPrefixes% = m_prefixes.iLireNbSegments
        For i As Integer = 0 To iNbPrefixes - 1
            Dim prefixe As clsSegmentBase = Nothing
            m_prefixes.bLireSegment(i, prefixe)
            Dim sPrefixe = prefixe.sSegment
            Dim sDefinition = prefixe.sSens
            Dim sNiveau = prefixe.sNiveau
            Dim sFrequence = prefixe.sFrequence
            Dim seg As clsSegment
            If dicoPrefixes.ContainsKey(sPrefixe) Then
                ' Il peut y avoir des doublons : liste de définitions possibles
                seg = dicoPrefixes(sPrefixe)
                seg.lstDefinitions.Add(sDefinition)
                'If Not String.IsNullOrEmpty(prefixe.sUnicite) Then _
                '    seg.lstUnicites.Add(prefixe.sUnicite) ' Test
                seg.lstUnicites.Add(prefixe.sUnicite) ' 09/12/2018 Ajouter même "" pour conserver l'indexe
                'If Not String.IsNullOrEmpty(prefixe.sUnicite) Then _
                '   seg.sUnicitePrincipale = prefixe.sUnicite ' Test
            Else
                seg = New clsSegment
                seg.sSegment = sPrefixe
                seg.lstDefinitions.Add(sDefinition)
                seg.lstUnicites.Add(prefixe.sUnicite) ' 09/12/2018 Ajouter même "" pour conserver l'indexe
                seg.iLong = seg.sSegment.Length
                seg.iNiveau = Integer.Parse(sNiveau)
                seg.sFrequence = sFrequence
                seg.sSegmentUniforme = sEnleverAccents(sPrefixe.ToLower) ', bTexteUnicode:=True)
                'seg.sUnicite = prefixe.sUnicite ' Test
                'If Not String.IsNullOrEmpty(prefixe.sUnicite) Then _
                '    seg.lstUnicites.Add(prefixe.sUnicite) ' Test
                'seg.sUnicitePrincipale = prefixe.sUnicite ' Test
                dicoPrefixes.Add(sPrefixe, seg)
            End If
        Next
        aPrefixes = dicoPrefixes.Trier("iLong Desc, sSegment")

    End Sub

    Private Sub AjouterPrefixePotSansDef(iNbCarMinPrefixe%,
        ByRef dicoPrefixes As DicoTri(Of String, clsSegment),
        ByRef aPrefixes() As clsSegment)

        dicoPrefixes = New DicoTri(Of String, clsSegment)
        Dim iNbPrefixes% = m_prefixesPot.iLireNbSegments()
        For i As Integer = 0 To iNbPrefixes - 1
            Dim prefixe As clsSegmentBase = Nothing
            m_prefixesPot.bLireSegment(i, prefixe)
            Dim sPrefixe = prefixe.sSegment
            If sPrefixe.Length < iNbCarMinPrefixe Then Continue For
            Dim sDefinition = ""
            Dim seg As clsSegment
            If dicoPrefixes.ContainsKey(sPrefixe) Then
                ' Il peut y avoir des doublons : liste de définitions possibles
                seg = dicoPrefixes(sPrefixe)
                seg.lstDefinitions.Add(sDefinition)
                seg.lstUnicites.Add("") ' 15/12/2018 Ajouter même "" pour conserver l'indexe
            Else
                seg = New clsSegment
                seg.sSegment = sPrefixe
                seg.lstDefinitions.Add(sDefinition)
                seg.lstUnicites.Add("") ' 15/12/2018 Ajouter même "" pour conserver l'indexe
                seg.iLong = seg.sSegment.Length
                seg.sSegmentUniforme = sEnleverAccents(sPrefixe.ToLower) ', bTexteUnicode:=True)
                dicoPrefixes.Add(sPrefixe, seg)
            End If
        Next
        aPrefixes = dicoPrefixes.Trier("iLong Desc, sSegment")

    End Sub

    Private Sub AjouterSuffixeDico(
        ByRef dicoSuffixes As DicoTri(Of String, clsSegment),
        ByRef aSuffixes() As clsSegment)

        dicoSuffixes = New DicoTri(Of String, clsSegment)
        Dim iNbSuffixes% = m_suffixes.iLireNbSegments()
        For i As Integer = 0 To iNbSuffixes - 1
            Dim suffixe As clsSegmentBase = Nothing
            m_suffixes.bLireSegment(i, suffixe)
            Dim sSuffixe = suffixe.sSegment
            Dim sDefinition = suffixe.sSens
            Dim sNiveau = suffixe.sNiveau
            Dim sFrequence = suffixe.sFrequence
            Dim seg As clsSegment
            If dicoSuffixes.ContainsKey(sSuffixe) Then
                ' Il peut y avoir des doublons : liste de définitions possibles
                'Debug.WriteLine("Suffixe à sens multiple : " & sPrefixe)
                seg = dicoSuffixes(sSuffixe)
                seg.lstDefinitions.Add(sDefinition)
                'If Not String.IsNullOrEmpty(suffixe.sUnicite) Then _
                '    seg.lstUnicites.Add(suffixe.sUnicite) ' Test
                seg.lstUnicites.Add(suffixe.sUnicite) ' 09/12/2018 Ajouter même "" pour conserver l'indexe
                'If Not String.IsNullOrEmpty(suffixe.sUnicite) Then _
                '   seg.sUnicitePrincipale = suffixe.sUnicite ' Test
            Else
                seg = New clsSegment
                seg.sSegment = sSuffixe
                'seg.sDefinition = sDefinition
                seg.lstDefinitions.Add(sDefinition)
                seg.lstUnicites.Add(suffixe.sUnicite) ' 09/12/2018 Ajouter même "" pour conserver l'indexe
                seg.iLong = seg.sSegment.Length
                seg.iNiveau = Integer.Parse(sNiveau)
                seg.sFrequence = sFrequence
                seg.sSegmentUniforme = sEnleverAccents(sSuffixe.ToLower) ', bTexteUnicode:=True)
                'seg.sMotUniforme = sSuffixe.ToLower
                'seg.sUnicite = suffixe.sUnicite ' Test
                'If Not String.IsNullOrEmpty(suffixe.sUnicite) Then _
                '    seg.lstUnicites.Add(suffixe.sUnicite) ' Test
                'seg.sUnicitePrincipale = suffixe.sUnicite ' Test
                dicoSuffixes.Add(sSuffixe, seg)
            End If
        Next
        aSuffixes = dicoSuffixes.Trier("iLong Desc, sSegment")

    End Sub

    Private Sub AjouterSuffixePotSansDef(iNbCarMinSuffixe%,
        ByRef dicoSuffixes As DicoTri(Of String, clsSegment),
        ByRef aSuffixes() As clsSegment)

        dicoSuffixes = New DicoTri(Of String, clsSegment)
        Dim iNbSuffixes% = m_suffixesPot.iLireNbSegments
        For i As Integer = 0 To iNbSuffixes - 1
            Dim suffixe As clsSegmentBase = Nothing
            m_suffixesPot.bLireSegment(i, suffixe)
            Dim sSuffixe = suffixe.sSegment
            If sSuffixe.Length < iNbCarMinSuffixe Then Continue For
            Dim sDefinition = ""
            Dim seg As clsSegment
            If dicoSuffixes.ContainsKey(sSuffixe) Then
                ' Il peut y avoir des doublons : liste de définitions possibles
                'Debug.WriteLine("Suffixe à sens multiple : " & sPrefixe)
                seg = dicoSuffixes(sSuffixe)
                'mot.lstDefinitions.Add(sDefinition)
            Else
                seg = New clsSegment
                seg.sSegment = sSuffixe
                'mot.sDefinition = sDefinition
                seg.lstDefinitions.Add(sDefinition) ' Définition vide : on veut un seul passage dans le for each def.
                seg.lstUnicites.Add("") ' 15/12/2018 Ajouter même "" pour conserver l'indexe
                seg.iLong = seg.sSegment.Length
                seg.sSegmentUniforme = sEnleverAccents(sSuffixe.ToLower) ', bTexteUnicode:=True)
                'mot.sMotUniforme = sSuffixe.ToLower
                dicoSuffixes.Add(sSuffixe, seg)
            End If
        Next
        aSuffixes = dicoSuffixes.Trier("iLong Desc, sSegment")

    End Sub

    Private Sub VerifierDico()

        Dim sCheminDico0$ = ""
        If bChercherDico(sCheminDico0) Then Exit Sub

        Dim sUrl$ = ""
        Select Case sLang
        Case enumLangue.Fr : sUrl = sURLDicoFr
        Case enumLangue.En : sUrl = sURLDicoEn
        'Case enumLangue.Uk : sUrl = sURLDicoUk
        'Case enumLangue.US : sUrl = sURLDicoUs
        Case Else : sUrl = ""
        End Select
        If sUrl.Length = 0 Then
            MsgBox( _
                "Le dictionnaire est introuvable :" & vbLf & _
                sCheminDico0, MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        If MsgBoxResult.Cancel = MsgBox( _
            "Le dictionnaire est introuvable :" & vbLf & _
            sCheminDico0 & vbLf & _
            "Cliquez sur OK pour le télécharger :" & vbLf & sUrl, _
            MsgBoxStyle.Exclamation Or MsgBoxStyle.OkCancel, m_sTitreMsg) Then Exit Sub

        OuvrirAppliAssociee(sUrl, bVerifierFichier:=False)

    End Sub

    Private Function bChercherDico(ByRef sCheminDicoFinal$) As Boolean

        Dim sCheminDico0 = Application.StartupPath & "\Doc\" & sCheminDico
        sCheminDicoFinal = sCheminDico0
        Dim bExiste0 = bFichierExiste(sCheminDico0)
        Return bExiste0

    End Function

#End Region

    Public Sub AnalyseDico(msgDelegue As clsMsgDelegue)

        m_msgDelegue = msgDelegue
        m_bAfficherAvertDoublon = False

        Dim lstExclVerbesConj As New List(Of String) From {
            "aplane", "aposte", "décèle", "décentre", "déchire", "déchrome", "décline", "décolore",
            "décoque", "déculture", "défère", "déflore", "déforme", "dégrade", "déloque",
            "déparagée", "déparasite", "déprogramme", "désiste", "dévalent", "dévore", "décide", "dépare",
            "incère", "incline", "ingénie", "invoque", "permane", "transite"}
        Dim hsExclVerbesConj As HashSet(Of String) = Nothing
        If Not bListToHashSet(lstExclVerbesConj, hsExclVerbesConj, bPromptErr:=True) Then Exit Sub

        Dim sCheminExclDef$ = Application.StartupPath &
            "\Doc\DefinitionsFausses" & sLang & ".txt"
        If Not bFichierExiste(sCheminExclDef, bPrompt:=True) Then Exit Sub
        Dim asExcl = asLireFichier(sCheminExclDef, bLectureSeule:=True, bUnicodeUTF8:=True)
        Dim lstExclDef = asExcl.ToList
        If Not bListToHashSet(lstExclDef, m_hsExclDef, bPromptErr:=True) Then Exit Sub
        m_hsExclDefUtil = New HashSet(Of String)

        ' 17/11/2019 Liste de mots ajoutés au dictionnaire
        Dim sCheminMotsAjoutes$ = Application.StartupPath &
            "\Doc\MotsAjoutes" & sLang & ".txt"
        If Not bFichierExiste(sCheminMotsAjoutes, bPrompt:=True) Then Exit Sub
        Dim asMotsAjoutes = asLireFichier(sCheminMotsAjoutes, bLectureSeule:=True,
            bUnicodeUTF8:=True)
        Dim lstMotsAjoutes = asMotsAjoutes.ToList
        Dim hsMotsAjoutes As HashSet(Of String) = Nothing
        ' Supprimer les commentaires
        If Not bListToHashSet(lstMotsAjoutes, hsMotsAjoutes, bPromptErr:=True) Then Exit Sub

        VerifierDico()
        Dim sCheminDico0$ = ""
        If Not bChercherDico(sCheminDico0) Then Exit Sub

        Dim prm As New clsPrm
        Dim prmPot As New clsPrm

        Dim dicoPrefixes As New DicoTri(Of String, clsSegment)
        AjouterPrefixeDico(dicoPrefixes, prm.aPrefixes)

        Dim dicoPrefixesPot As New DicoTri(Of String, clsSegment)
        If sLang = enumLangue.Fr Then _
            AjouterPrefixePotSansDef(iNbCarMinPrefixePot, dicoPrefixesPot, prmPot.aPrefixes)

        Dim dicoSuffixes As New DicoTri(Of String, clsSegment)
        AjouterSuffixeDico(dicoSuffixes, prm.aSuffixes)

        Dim dicoSuffixesPot As New DicoTri(Of String, clsSegment)
        If sLang = enumLangue.Fr Then _
            AjouterSuffixePotSansDef(iNbCarMinSuffixePot, dicoSuffixesPot, prmPot.aSuffixes)

        If Not bFichierExiste(sCheminDico0, bPrompt:=True) Then Exit Sub
        Dim asLignes$() = asLireFichier(sCheminDico0)
        'Dim iNbLignes% = asLignes.GetUpperBound(0)
        Dim iNbLignesMA% = hsMotsAjoutes.Count

        ' En premier les mots ajoutés
        Dim lstMotsAjoutesFinale As List(Of String) = hsMotsAjoutes.ToList
        Dim lstMotsDico = asLignes.ToList
        lstMotsAjoutesFinale.AddRange(lstMotsDico)

        'If IsNothing(asLignes) Then Exit Sub
        Dim iNumLigne% = 0
        Dim iNbLignes% = lstMotsAjoutesFinale.Count

        Dim sMemMot$ = ""
        For Each sMotDico As String In lstMotsAjoutesFinale ' asLignes
            iNumLigne += 1

            If hsExclVerbesConj.Contains(sMotDico) Then Continue For

            'If bDebug Then sMotDico = ""
            prm.sMotDico = sMotDico
            prmPot.sMotDico = sMotDico
            'If sMotDico = "" Then
            '    Debug.WriteLine("!")
            'End If
            'If bDebug AndAlso iNumLigne > 1 Then Exit For
            'If bDebug AndAlso iNumLigne > 10000 Then Exit For
            'If bDebug AndAlso iNumLigne > 100000 Then Exit For ' 15%
            'If bDebug AndAlso iNumLigne > 150000 Then Exit For ' 22%
            'If bDebug AndAlso iNumLigne > 200000 Then Exit For ' 30%

            If iNumLigne Mod 5000 = 0 OrElse iNumLigne = iNbLignes Then
                Dim rPC! = iNumLigne / iNbLignes
                'If bDebug AndAlso rPC > 0.33 Then Exit For
                Dim sPC$ = rPC.ToString(sFormatPC1)
                msgDelegue.AfficherMsg(sPC)
                If msgDelegue.m_bAnnuler Then Exit For
            End If

            Dim bPluriel = False
            If sMotDico = sMemMot & "s" Then bPluriel = True
            prm.bPluriel = bPluriel
            prmPot.bPluriel = bPluriel
            sMemMot = sMotDico

            prm.sMotDicoUniforme = sEnleverAccents(sMotDico, bMinuscule:=True) ' 18/04/2019
            prm.sMotDicoMinuscule = sMotDico.ToLower
            prmPot.sMotDicoMinuscule = prm.sMotDicoMinuscule
            prmPot.sMotDicoUniforme = prm.sMotDicoUniforme
            Dim iLongMot% = sMotDico.Length
            prm.iLongMot = iLongMot
            prmPot.iLongMot = iLongMot
            Dim iLongPrefixeMax% = 0
            Dim iLongSuffixeMax% = 0 ' 25/08/2018
            Const iMotLePlusLong% = 256
            Const iNumPasseMax% = 2
            Dim aiLongPrefixeMinTeste%(0 To iNumPasseMax)
            Dim aiLongSuffixeMinTeste%(0 To iNumPasseMax)
            Dim abHypTestee(0 To iNumPasseMax) As Boolean
            For iPasse As Integer = 0 To iNumPasseMax ' 26/08/2018
                aiLongPrefixeMinTeste(iPasse) = iMotLePlusLong
                aiLongSuffixeMinTeste(iPasse) = iMotLePlusLong
                abHypTestee(iPasse) = False
            Next
            Dim bPrefixe0 = False
            Dim bSuffixe0 = False
            Dim bMotPrefixeOuMotSuffixe As Boolean = False
            prm.bPremiereCombi = True
Recommencer:
            Combiner(prm, iNumPasseMax,
                aiLongPrefixeMinTeste, aiLongSuffixeMinTeste,
                iLongPrefixeMax, iLongSuffixeMax, abHypTestee,
                bPrefixe0, bSuffixe0, bMotPrefixeOuMotSuffixe)
            If bMotPrefixeOuMotSuffixe Then Continue For

            ' 26/08/2018
            ' Retester tant qu'on a pas trouvé avec un préfixe et un suffixe d'une longueur donnée
            Dim bToutesLongTestee = True
            For iPasse As Integer = 0 To iNumPasseMax
                If Not abHypTestee(iPasse) Then bToutesLongTestee = False
            Next
            If Not bToutesLongTestee Then
                prm.bPremiereCombi = False
                GoTo Recommencer
            End If

            'If bDebug Then Exit For

            ' Ne pas gérer la complexité pour les mots potentiels
            If sLang <> enumLangue.Fr Then Continue For

            Dim iLongPrefixePot% = 0
            Dim sPrefixePot = ""
            Dim lst2PrefixePotDef As New List(Of List(Of String))
            Dim lstPrefixePotDef As New List(Of String)
            Dim lst2UnicitesP As New List(Of List(Of String))
            Dim lstUnicitesP As New List(Of String)
            Dim bPrefixePot As Boolean = False
            If Not bPrefixe0 AndAlso sLang = enumLangue.Fr Then
                bPrefixePot = bPrefixe(sMotDico, prmPot.aPrefixes,
                    sPrefixePot, lstPrefixePotDef, iLongPrefixePot, lstUnicites:=lstUnicitesP)
                If bPrefixePot Then
                    ' Idée : faire 2 dico distincts (mais pas grave : qu'un seul élément préfixe et suffixe : folia)
                    If prmPot.dicoPrefSuff.ContainsKey(sPrefixePot) Then
                        Dim seg = prmPot.dicoPrefSuff(sPrefixePot)
                        If Not seg.bPrefixe AndAlso bDebugPrefixeEtSuffixePot Then
                            Debug.WriteLine("A la fois préfixe et suffixe potentiel : " & sPrefixePot)
                            'GoTo Suite2
                        End If
                    End If
                    lst2PrefixePotDef.Add(lstPrefixePotDef)
                    lstUnicitesP.Add("") ' 15/12/2018
                    lst2UnicitesP.Add(lstUnicitesP) ' 15/12/2018
                    AjouterSegment(prmPot.dicoPrefSuff, sPrefixePot, prmPot.sMotDicoMinuscule, bPrefixe:=True)
                End If
            End If
            prmPot.sPrefixe = sPrefixePot
            prmPot.bPrefixe = bPrefixePot
            prmPot.iLongPrefixe = iLongPrefixePot
            prmPot.lst2PrefixeDef = lst2PrefixePotDef
            prmPot.lst2UnicitesPrefixe = lst2UnicitesP ' 15/12/2018

            Dim iLongSuffixePot% = 0
            Dim sSuffixePot = ""
            Dim lstSuffixePotDef As New List(Of String)
            Dim lstUnicitesS As New List(Of String)
            Dim bSuffixePot As Boolean = False
            If Not bSuffixe0 AndAlso sLang = enumLangue.Fr Then
                bSuffixePot = bSuffixe(sMotDico, prmPot.aSuffixes,
                    sSuffixePot, lstSuffixePotDef, iLongSuffixePot)
                If bSuffixePot Then
                    lstUnicitesS.Add("") ' 15/12/2018
                    ' Idée : faire 2 dico distincts (mais pas grave : qu'un seul élément préfixe et suffixe : folia)
                    If prmPot.dicoPrefSuff.ContainsKey(sSuffixePot) Then
                        Dim seg = prmPot.dicoPrefSuff(sSuffixePot)
                        If seg.bPrefixe AndAlso bDebugPrefixeEtSuffixePot Then
                            Debug.WriteLine("A la fois préfixe et suffixe potentiel : " & sSuffixePot)
                            GoTo Suite2
                        End If
                    End If
                    AjouterSegment(prmPot.dicoPrefSuff, sSuffixePot, prmPot.sMotDicoMinuscule, bPrefixe:=False)
                End If
            End If
            prmPot.sSuffixe = sSuffixePot
            prmPot.bSuffixe = bSuffixePot
            prmPot.iLongSuffixe = iLongSuffixePot
            prmPot.lstSuffixeDef = lstSuffixePotDef
            prmPot.lstUnicitesSuffixe = lstUnicitesS ' 15/12/2018
Suite2:
            TraiterMot(prmPot, bPotentiel:=True)

        Next

        Dim sbBilan As New StringBuilder
        sbBilan.AppendLine("Bilan du dictionnaire " & sLang & " (" & iNbLignes & " mots dérivés et conjugués) :")
        sbBilan.AppendLine("-------------------------")

        Bilan(prm, sbBilan, iNbLignes, dicoPrefixes, dicoSuffixes, bMotsAvecPrefixe:=False,
            bMotsAvecSuffixe:=False, bNonTrouves:=True, bDefIncomplete:=True, bPotentiel:=False)

        If sLang = enumLangue.Fr Then
            sbBilan.AppendLine()
            sbBilan.AppendLine()
            sbBilan.AppendLine("Bilan du dictionnaire " & sLang & " avec les segments potentiels :")
            sbBilan.AppendLine("------------------------------------------------------")
            Bilan(prmPot, sbBilan, iNbLignes, dicoPrefixesPot, dicoSuffixesPot, bMotsAvecPrefixe:=False,
                bMotsAvecSuffixe:=False, bNonTrouves:=False, bDefIncomplete:=False, bPotentiel:=True)
        End If

Fin:
        ' Vérifier les déf. fausses utiles
        Dim bAuMoinsUne As Boolean = False
        For Each sDef In m_hsExclDef
            If String.IsNullOrEmpty(sDef) Then Continue For
            If sDef.StartsWith("//") Then Continue For
            If m_hsExclDefUtil.Contains(sDef) Then Continue For
            If Not bAuMoinsUne Then
                bAuMoinsUne = True
                sbBilan.AppendLine()
                sbBilan.AppendLine()
                sbBilan.AppendLine("Définitions fausses non utilisées :")
                sbBilan.AppendLine("---------------------------------")
            End If
            sbBilan.AppendLine(sDef)
        Next

        Dim sCheminBilan$ = Application.StartupPath & "\Doc\BilanDico" & sLang & ".txt"
        Dim bOk = bEcrireFichier(sCheminBilan, sbBilan)
        If bOk Then ProposerOuvrirFichier(sCheminBilan)

    End Sub

    Private Sub Combiner(prm As clsPrm, iNumPasseMax%,
        aiLongPrefixeMinTeste%(), iLongSuffixeMinTeste%(),
        aiLongPrefixeMax%, iLongSuffixeMax%, abHypTestee() As Boolean,
        ByRef bPrefixe0 As Boolean, ByRef bSuffixe0 As Boolean,
        ByRef bMotPrefixeOuMotSuffixe As Boolean)

        ' Tester toutes les combinaisons, par exemple : apicole :
        ' apico- apic- (le sommet) api- a- -cole : seul api- -cole est juste,
        '  mais api- n'est pourtant pas le préfixe le plus long

        For iPasse As Integer = 0 To iNumPasseMax ' 26/08/2018

            Select Case iPasse
            Case 0
                ' Tester avec un préfixe et un suffixe plus court
                aiLongPrefixeMax = aiLongPrefixeMinTeste(iPasse)
                iLongSuffixeMax = iLongSuffixeMinTeste(iPasse)
            Case 1
                ' Tester avec un préfixe plus court
                aiLongPrefixeMax = aiLongPrefixeMinTeste(iPasse)
                iLongSuffixeMax = 0
            Case 2
                ' Tester avec un suffixe plus court
                iLongSuffixeMax = iLongSuffixeMinTeste(iPasse)
                aiLongPrefixeMax = 0 ' Préfixe le plus long possible
            End Select

            prm.iPasse = iPasse
            Dim lst2UnicitesP As New List(Of List(Of String))
            Dim lstUnicitesP As New List(Of String)
            Dim sUnicitePrincipaleP$ = ""
            Dim iNiveauP% = 0
            Dim iLongPrefixe% = 0
            Dim sPrefixe = ""
            Dim sFreqPrefixe = ""
            Dim lst2PrefixeDef As New List(Of List(Of String))
            Dim lstPrefixeDef As New List(Of String)
            bPrefixe0 = bPrefixe(prm.sMotDico, prm.aPrefixes,
                sPrefixe, lstPrefixeDef, iLongPrefixe, iNiveauP, lstUnicitesP, sUnicitePrincipaleP,
                sFreqPrefixe, aiLongPrefixeMax)
            If bPrefixe0 Then
                If iLongPrefixe < aiLongPrefixeMinTeste(iPasse) Then aiLongPrefixeMinTeste(iPasse) = iLongPrefixe
                lst2PrefixeDef.Add(lstPrefixeDef)
                'If lstUnicitesP.Count > 0 Then lst2UnicitesP.Add(lstUnicitesP)
                lst2UnicitesP.Add(lstUnicitesP) ' 09/12/2018 Ajouter même "" pour conserver l'indexe
                AjouterSegment(prm.dicoPrefSuff, sPrefixe, prm.sMotDicoMinuscule, bPrefixe:=True)
                If iLongPrefixe = prm.iLongMot Then
                    For Each sPrefixeDef In lstPrefixeDef
                        Dim sDef$ = sPrefixeDef.ToUpper

                        ' ontogenèse et ontogénèse : pas besoin de l'accent, on n'en veut qu'un
                        'Dim sCle$ = prm.sMotDico & " : " & sDef
                        Dim sCle$ = prm.sMotDicoUniforme & " : " & sDef ' 18/04/2019
                        If prm.hsUnicitesDefMots.Contains(sCle) Then Continue For
                        prm.hsUnicitesDefMots.Add(sCle)

                        Dim sLigne$ = prm.sMotDico & " : " & sDef & " : " & sPrefixe & "-"
                        prm.mots.AjouterLigneDoublon(prm.sMotDico, sLigne)
                        prm.bMotTrouve = True
                        Dim iNivS% = 0
                        Dim iComplex% = (iNiveauP + 1) * (iNivS + 1)
                        If Not prm.dicoComplex.ContainsKey(prm.sMotDico) Then _
                            prm.dicoComplex.Add(prm.sMotDico,
                                New clsMot(prm.sMotDico, iComplex,
                                    sPrefixe & "(" & iNiveauP & ") -", sDef, 1))
                    Next
                    bMotPrefixeOuMotSuffixe = True
                    Exit For ' Passes
                End If
            End If
            prm.sPrefixe = sPrefixe
            prm.bPrefixe = bPrefixe0
            prm.iLongPrefixe = iLongPrefixe
            prm.lst2PrefixeDef = lst2PrefixeDef
            prm.lst2UnicitesPrefixe = lst2UnicitesP
            prm.iNiveauPrefixe = iNiveauP
            prm.sFreqPrefixe = sFreqPrefixe

            Dim sUnicitePrincipaleS$ = ""
            Dim sSuffixe = ""
            Dim lstSuffixeDef = New List(Of String)
            Dim lstUnicitesS As New List(Of String)
            Dim iNiveauS% = 0
            Dim sFreqSuffixe$ = ""
            Dim iLongSuffixe% = 0
            bSuffixe0 = bSuffixe(prm.sMotDico, prm.aSuffixes,
                sSuffixe, lstSuffixeDef, iLongSuffixe, iNiveauS, lstUnicitesS, sUnicitePrincipaleS,
                sFreqSuffixe, iLongSuffixeMax)
            If bSuffixe0 Then
                If iLongSuffixe < iLongSuffixeMinTeste(iPasse) Then iLongSuffixeMinTeste(iPasse) = iLongSuffixe
                AjouterSegment(prm.dicoPrefSuff, sSuffixe, prm.sMotDicoMinuscule, bPrefixe:=False)
                If iLongSuffixe = prm.iLongMot Then
                    For Each sSuffixeDef In lstSuffixeDef
                        Dim sDef$ = sSuffixeDef.ToUpper
                        'Dim sCle$ = prm.sMotDico & " : " & sDef
                        Dim sCle$ = prm.sMotDicoUniforme & " : " & sDef ' 18/04/2019
                        If prm.hsUnicitesDefMots.Contains(sCle) Then Continue For
                        prm.hsUnicitesDefMots.Add(sCle)
                        Dim sLigne$ = prm.sMotDico & " : " & sDef & " : -" & sSuffixe
                        prm.mots.AjouterLigneDoublon(prm.sMotDico, sLigne)
                        prm.bMotTrouve = True
                        Dim iNivP% = 0
                        Dim iComplex% = (iNivP + 1) * (iNiveauS + 1)
                        If Not prm.dicoComplex.ContainsKey(prm.sMotDico) Then _
                            prm.dicoComplex.Add(prm.sMotDico,
                                New clsMot(prm.sMotDico, iComplex,
                                    "- " & sSuffixe & "(" & iNiveauS & ")", sDef, 1))
                    Next
                    bMotPrefixeOuMotSuffixe = True
                    Exit For ' Passes
                End If
            End If

            prm.sSuffixe = sSuffixe
            prm.bSuffixe = bSuffixe0
            prm.iLongSuffixe = iLongSuffixe
            prm.lstSuffixeDef = lstSuffixeDef
            prm.lstUnicitesSuffixe = lstUnicitesS
            prm.iNiveauSuffixe = iNiveauS
            prm.sFreqSuffixe = sFreqSuffixe

            If prm.sMotDico = "" Then
                Debug.WriteLine("Passe n°" & iPasse & " : " & prm.sMotDico & " : " &
                    sPrefixe & " : " & sSuffixe)
            End If

            Dim bHypotheseTropLongue As Boolean = False
            TraiterMot(prm, bPotentiel:=False, bHypotheseTropLongue:=bHypotheseTropLongue)

            ' Si préfixe non trouvée et que toutes les longueurs ont été testées, 
            '  alors l'hypothèse de la passe iPasse a été testée
            If aiLongPrefixeMinTeste(iPasse) <= 1 AndAlso
                iLongSuffixeMinTeste(iPasse) <= 1 Then abHypTestee(iPasse) = True
            ' Si on a pas préfixe+suffixe, alors l'hypothèse a été testée
            If Not bPrefixe0 OrElse Not bSuffixe0 Then abHypTestee(iPasse) = True

        Next

    End Sub

    Private Sub Bilan(prm As clsPrm, sbBilan As StringBuilder, iNbLignes%,
        dicoPrefixes As DicoTri(Of String, clsSegment), dicoSuffixes As DicoTri(Of String, clsSegment),
        bMotsAvecPrefixe As Boolean, bMotsAvecSuffixe As Boolean, bNonTrouves As Boolean,
        bDefIncomplete As Boolean, bPotentiel As Boolean)

        'Dim iNbMots% = iNbLignes
        Dim iNbMots% = iNbMotsFrancais

        If Not bPotentiel Then
            Dim aSegmentsMultiplesSens = prm.dicoSegmentsSensMultiples.Trier(
                "iNbOccDicoFr Desc, sSegment, sSens")
            sbBilan.AppendLine("")
            sbBilan.AppendLine("Segments à sens multiple")
            sbBilan.AppendLine("------------------------")
            Dim iNbSegmentsMultiples% = 0
            Dim iNbOccSegMTot% = 0
            For Each stat In aSegmentsMultiplesSens
                iNbSegmentsMultiples += 1
                iNbOccSegMTot += stat.iNbOccDicoFr
                Dim sEx$ = sListerTxt(stat.lstMots, iNbMax:=10)
                Dim sSegment$ = "-" & stat.sSegment
                If stat.bPrefixe Then sSegment = stat.sSegment & "-"
                sbBilan.AppendLine(stat.iNbOccDicoFr & " : " &
                    sSegment & " : " & stat.sSens & " : " & sEx & ".")
            Next
            sbBilan.AppendLine("Nombre de segments multiples : " & iNbSegmentsMultiples)
            sbBilan.AppendLine("Nb. total d'occurrences = " & iNbOccSegMTot)
        End If

        Dim aPrefixesFrequents = prm.dicoPrefSuff.Trier(
            "bPrefixe Desc, bComposable Desc, iNbOccDicoFr Desc, sSegment")
        sbBilan.AppendLine("")
        Dim sPot$ = "", sTiret$ = ""
        If bPotentiel Then sPot = " potentiels" : sTiret = "-----------"
        sbBilan.AppendLine("Préfixes fréquents" & sPot & " (en 1er les composables) :")
        sbBilan.AppendLine("-------------------------------------------" & sTiret)
        Dim iNbPrefixesFrequents% = 0
        Dim iNbOccPrefixesTot% = 0

        For Each stat In aPrefixesFrequents
            If Not stat.bPrefixe Then Exit For
            iNbPrefixesFrequents += 1
            Dim lstDef As New List(Of String)
            If dicoPrefixes.ContainsKey(stat.sSegment) Then lstDef = dicoPrefixes(stat.sSegment).lstDefinitions
            Dim rPC! = stat.iNbOccDicoFr / iNbMots
            iNbOccPrefixesTot += stat.iNbOccDicoFr
            Dim sEx$ = sListerTxt(stat.lstMots, iNbMax:=iNbExemplesMax)
            sbBilan.AppendLine(stat.iNbOccDicoFr & " (" & rPC.ToString(sFormatPC4) & ") : " &
                stat.sSegment & "- : " & sListerTxt(lstDef) & " : " & sEx & ".")
        Next
        sbBilan.AppendLine("Nombre de préfixes fréquents : " &
            iNbPrefixesFrequents & " / " & prm.aPrefixes.Count)
        Dim rPCPrefTot! = iNbOccPrefixesTot / iNbMots
        sbBilan.AppendLine("Nb. total d'occurrences = " & iNbOccPrefixesTot & " / " &
            iNbMots & " = " & rPCPrefTot.ToString(sFormatPC2))

        If bNonTrouves Then
            Dim bPNonTrouve As Boolean = False
            For Each kvp In dicoPrefixes
                Dim prefixe = kvp.Value
                Dim sPrefixe = prefixe.sSegment
                If Not prm.dicoPrefSuff.ContainsKey(sPrefixe) Then
                    If Not bPNonTrouve Then bPNonTrouve = True : sbBilan.AppendLine("Préfixes non trouvés :")
                    sbBilan.AppendLine(sPrefixe & "-")
                End If
            Next
        End If

        Dim aSuffixesFrequents = prm.dicoPrefSuff.Trier("bPrefixe, bComposable Desc, iNbOccDicoFr Desc, sSegment")
        sbBilan.AppendLine("")
        sbBilan.AppendLine("Suffixes fréquents" & sPot & " (en 1er les composables) :")
        sbBilan.AppendLine("-------------------------------------------" & sTiret)
        Dim iNbSuffixesFrequents% = 0
        Dim iNbOccSuffixesTot% = 0
        For Each stat In aSuffixesFrequents
            If stat.bPrefixe Then Exit For
            iNbSuffixesFrequents += 1
            Dim lstDef As New List(Of String)
            If dicoSuffixes.ContainsKey(stat.sSegment) Then lstDef = dicoSuffixes(stat.sSegment).lstDefinitions
            Dim rPC! = stat.iNbOccDicoFr / iNbMots
            iNbOccSuffixesTot += stat.iNbOccDicoFr
            Dim sEx$ = sListerTxt(stat.lstMots, iNbMax:=iNbExemplesMax)
            sbBilan.AppendLine(stat.iNbOccDicoFr & " (" & rPC.ToString(sFormatPC4) & ") : -" &
                stat.sSegment & " : " & sListerTxt(lstDef) & " : " & sEx & ".")
        Next
        sbBilan.AppendLine("Nombre de suffixes fréquents : " &
            iNbSuffixesFrequents & " / " & prm.aSuffixes.Count)
        Dim rPCSufTot! = iNbOccSuffixesTot / iNbMots
        sbBilan.AppendLine("Nb. total d'occurrences = " & iNbOccSuffixesTot & " / " &
            iNbMots & " = " & rPCSufTot.ToString(sFormatPC2))

        If bNonTrouves Then
            Dim bSNonTrouve As Boolean = False
            For Each kvp In dicoSuffixes
                Dim suffixe = kvp.Value
                Dim sSuffixe = suffixe.sSegment
                If sSuffixe.Length < iLongSuffixeMin Then Continue For
                If Not prm.dicoPrefSuff.ContainsKey(sSuffixe) Then
                    If Not bSNonTrouve Then bSNonTrouve = True : sbBilan.AppendLine("Suffixes non trouvés :")
                    sbBilan.AppendLine("-" & sSuffixe)
                End If
            Next
        End If

        If bDefIncomplete Then
            sbBilan.AppendLine("")
            sbBilan.AppendLine("Définitions incomplètes :")
            sbBilan.AppendLine("-----------------------")
            'sbBilan.Append(prm.sbDefIncomplete)

            Dim sbdi As New StringBuilder
            Dim aDefInc = prm.dicoDefIncompletes.Trier("rRatio Desc, sLigne")
            For Each ligne In aDefInc
                'If ligne.sMot = "" Then
                '    Debug.WriteLine("!")
                'End If
                ' Ne pas lister les lignes si le mot a finalement été trouvé 29/08/2018
                'If prm.mots.hs.Contains(ligne.sMot) Then Continue For
                If prm.mots.hs.Contains(ligne.sMotUniforme) Then Continue For ' 07/12/2019
                sbdi.AppendLine(ligne.sLigne)
            Next
            sbBilan.Append(sbdi)

        End If

        Dim sb As New StringBuilder
        sb.AppendLine("Mots" & sPot & " avec préfixe(s) et suffixe :")
        sb.AppendLine("-------------------------------" & sTiret)
        sb.Append(prm.mots.sb)
        sb.AppendLine("Nb. total d'occurrences = " & prm.mots.iNbLignes)
        Dim rPCMotExistTot2! = prm.mots.hs.Count / iNbMots
        sb.AppendLine("Nb. total d'occurrences uniques = " &
            prm.mots.hs.Count & " / " & iNbMots & " = " & rPCMotExistTot2.ToString(sFormatPC2))
        If Not bPotentiel Then
            Dim sCheminBilan$ = Application.StartupPath & "\Doc\Mots" & sLang & ".txt"
            bEcrireFichier(sCheminBilan, sb)
        End If
        sbBilan.AppendLine("")
        sbBilan.Append(sb)

        sb = New StringBuilder
        sb.AppendLine("Mots" & sPot & " avec des préfixes multiples et un suffixe :")
        sb.AppendLine("----------------------------------------------" & sTiret)
        sb.Append(prm.motsComplexes.sb)
        Dim rPCMotExistTot! = prm.motsComplexes.hs.Count / iNbMots
        sb.AppendLine("Nb. total d'occurrences = " &
            prm.motsComplexes.hs.Count & " / " & iNbMots & " = " & rPCMotExistTot.ToString(sFormatPC2))
        If Not bPotentiel Then
            Dim sCheminBilan$ = Application.StartupPath & "\Doc\MotsComplexes" & sLang & ".txt"
            bEcrireFichier(sCheminBilan, sb)
        End If
        sbBilan.AppendLine("")
        sbBilan.Append(sb)

        ' 15/12/2018
        If Not bPotentiel Then
            sb = New StringBuilder
            sb.AppendLine("Mots" & sPot & " avec des préfixes multiples et un suffixe avec l'unicité :")
            sb.AppendLine("-------------------------------------------------------------" & sTiret)
            sb.Append(prm.sbPrefixesEtSuffixe2Unicite)
            Dim rPCMotExistTotU! = prm.iNbMotsLogotronExistantsUnicite / iNbMots
            sb.AppendLine("Nb. total d'occurrences = " &
                prm.iNbMotsLogotronExistantsUnicite & " / " & iNbMots & " = " & rPCMotExistTotU.ToString(sFormatPC2))
            Dim sCheminBilan$ = Application.StartupPath & "\Doc\MotsComplexesUnicite" & sLang & ".txt"
            bEcrireFichier(sCheminBilan, sb)
        End If

        Dim sLigneTitre$ = "Mots" & sPot & " avec préfixe et suffixe :"
        Dim rPCMotExistTot1! = prm.motsSimples.hs.Count / iNbMots
        Dim sLigneBilan$ = "Nb. total d'occurrences = " &
            prm.motsSimples.hs.Count & " / " & iNbMots & " = " & rPCMotExistTot1.ToString(sFormatPC2)
        Dim sLigneLgd$ = "(mot : DÉFINITION : préfixe - suffixe : niveau préfixe niveau suffixe)"
        Dim sLigneTiret$ = "----------------------------" & sTiret
        sb = New StringBuilder
        sb.AppendLine(sLigneTitre)
        If Not bPotentiel Then sb.AppendLine(sLigneLgd)
        sb.AppendLine(sLigneTiret)
        sb.Append(prm.motsSimples.sb)
        sb.AppendLine(sLigneBilan)
        sbBilan.AppendLine("")
        sbBilan.Append(sb)
        sb = New StringBuilder
        sb.AppendLine(sLigneTitre)
        sLigneLgd = "(mot;DÉFINITION;préfixe;suffixe;niveau préfixe;niveau suffixe)"
        If Not bPotentiel Then sb.AppendLine(sLigneLgd)
        sb.AppendLine(sLigneTiret)
        sb.Append(prm.sbPrefixeEtSuffixeFichierTxt)
        sb.AppendLine(sLigneBilan)
        If Not bPotentiel Then
            Dim sCheminBilan$ = Application.StartupPath & "\Doc\MotsSimples" & sLang & ".txt"
            bEcrireFichier(sCheminBilan, sb)
        End If
        If Not bPotentiel Then
            sb = New StringBuilder
            sb.AppendLine("mot;DÉFINITION;préfixe;suffixe;niveau préfixe;niveau suffixe;unicité préfixe;unicité suffixe;fréq. préfixe;fréq. suffixe")
            sb.Append(prm.sbPrefixeEtSuffixeFichierCsv)
            Dim sCheminBilanCsv$ = Application.StartupPath & "\Doc\MotsSimples" & sLang & ".csv"
            bEcrireFichier(sCheminBilanCsv, sb)
            sb = New StringBuilder
            sb.AppendLine("                mot, DÉFINITION, préfixe, suffixe, niveau préfixe, niveau suffixe, unicité préfixe, unicité suffixe, fréq. préfixe, fréq. suffixe")
            sb.Append(prm.sbMotSimpleFichierCode)
            Dim sCheminBilanCode$ = Application.StartupPath & "\Doc\MotsSimplesCode" & sLang & ".txt"
            bEcrireFichier(sCheminBilanCode, sb)
        End If

        If bMotsAvecPrefixe Then
            sbBilan.AppendLine("")
            sbBilan.AppendLine("Mots avec préfixe :")
            sbBilan.AppendLine("-----------------")
            sbBilan.Append(prm.sbPrefixes)
            Dim rPCMotPExistTot! = prm.iNbMotsPrefixeExistants / iNbMots
            sbBilan.AppendLine("Nb. total d'occurrences = " & prm.iNbMotsPrefixeExistants & " / " &
                iNbMots & " = " & rPCMotPExistTot.ToString(sFormatPC2))
        End If

        If bMotsAvecSuffixe Then
            sbBilan.AppendLine("")
            sbBilan.AppendLine("Mots avec suffixe :")
            sbBilan.AppendLine("-----------------")
            sbBilan.Append(prm.sbSuffixes)
            Dim rPCMotSExistTot! = prm.iNbMotsSuffixeExistants / iNbMots
            sbBilan.AppendLine("Nb. total d'occurrences = " & prm.iNbMotsSuffixeExistants & " / " &
                iNbMots & " = " & rPCMotSExistTot.ToString(sFormatPC2))
        End If

        If Not bPotentiel Then
            Dim aPrefixesMFrequents = prm.dicoPrefixesManquants.Trier(
                "bPrefixe Desc, iNbOccDicoFr Desc, sSegment")
            sbBilan.AppendLine("")
            sbBilan.AppendLine("Préfixes manquants fréquents :")
            sbBilan.AppendLine("----------------------------")
            Dim iNbPrefixesMFrequents% = 0
            Dim iNbOccPrefixesMTot% = 0
            For Each stat In aPrefixesMFrequents
                If Not stat.bPrefixe Then Exit For
                iNbPrefixesMFrequents += 1

                Dim sbExemples As New StringBuilder
                Dim aSegFreq = stat.dicoSegManquant.Trier("iNbOccDicoFr Desc, sCle")
                For Each segF In aSegFreq
                    If segF.lstMotsDico.Count = 1 Then Exit For
                    Dim sEx0$ = sListerTxt(segF.lstMotsDico, iNbMax:=iNbExemplesMax)
                    If sbExemples.Length > 0 Then sbExemples.Append(" ")
                    sbExemples.Append("[" & segF.sCle & " : " & segF.iNbOccDicoFr & " : " & sEx0 & ".]")
                Next
                Dim lstDef As New List(Of String)
                If dicoPrefixes.ContainsKey(stat.sSegment) Then lstDef = dicoPrefixes(stat.sSegment).lstDefinitions
                Dim rPC! = stat.iNbOccDicoFr / iNbMots
                iNbOccPrefixesMTot += stat.iNbOccDicoFr
                Dim sEx$ = sListerTxt(stat.lstMots, iNbMax:=iNbExemplesMax)
                sbBilan.AppendLine(stat.iNbOccDicoFr & " (" & rPC.ToString(sFormatPC4) & ") : " &
                    stat.sSegment & "- : " & sbExemples.ToString & sListerTxt(lstDef) & " : " & sEx & ".")
            Next
            sbBilan.AppendLine("Nombre de préfixes manquants fréquents : " &
                iNbPrefixesMFrequents & " / " & prm.aPrefixes.Count)
            Dim rPCPrefMTot! = iNbOccPrefixesMTot / iNbMots
            sbBilan.AppendLine("Nb. total d'occurrences = " & iNbOccPrefixesMTot & " / " &
                iNbMots & " = " & rPCPrefMTot.ToString(sFormatPC2))
        End If

        If Not bPotentiel Then
            Dim sbc As New StringBuilder
            Dim aComplex = prm.dicoComplex.Trier("iComplexite, sMot")
            sbc.AppendLine("Tri des mots par complexité")
            sbc.AppendLine("---------------------------")
            For Each mot In aComplex
                sbc.AppendLine(mot.iComplexite & " : " & mot.sMot & " : " &
                    mot.iNiv & " : " & mot.sListeSegments & " : " & mot.sDef)
            Next
            'Dim rPCMotExistTot2! = prm.iNbMotsLogotronExistants / iNbMots
            sbc.AppendLine("Nb. total d'occurrences = " &
                prm.mots.hs.Count & " / " & iNbMots & " = " & rPCMotExistTot2.ToString(sFormatPC2))
            Dim sCheminBilan$ = Application.StartupPath & "\Doc\Complexite" & sLang & ".txt"
            bEcrireFichier(sCheminBilan, sbc)
            sbBilan.AppendLine("")
            sbBilan.Append(sbc)
        End If

        'sbBilan.AppendLine("")
        'sbBilan.AppendLine("Autres :")
        'sbBilan.AppendLine("------")
        'sbBilan.Append(sbAutres)

    End Sub

    Private Sub AjouterSegment(dicoPrefSuff As DicoTri(Of String, clsSegmentStat),
        sSegment$, sMotDicoUniforme$, bPrefixe As Boolean,
        Optional sPrefixePreced$ = "", Optional sSuffixeSuiv$ = "")

        Dim psStat As clsSegmentStat
        If Not dicoPrefSuff.ContainsKey(sSegment) Then
            psStat = New clsSegmentStat
            psStat.sSegment = sSegment
            'psStat.iLongSegment = sSegment.Length
            psStat.iNbOccDicoFr = 1
            psStat.bPrefixe = bPrefixe
            psStat.lstMots.Add(sMotDicoUniforme)
            dicoPrefSuff.Add(sSegment, psStat)
        Else
            psStat = dicoPrefSuff(sSegment)
            psStat.bPrefixe = bPrefixe ' Correction si à la fois préfixe et suffixe
            psStat.iNbOccDicoFr += 1
            psStat.lstMots.Add(sMotDicoUniforme)
        End If

        If String.IsNullOrEmpty(sPrefixePreced) Then Exit Sub
        If IsNothing(psStat.dicoSegManquant) Then _
            psStat.dicoSegManquant = New DicoTri(Of String, clsSegmentManquant)
        Dim sCle$ = sPrefixePreced & "-" & sSegment & "-" & sSuffixeSuiv
        If psStat.dicoSegManquant.ContainsKey(sCle) Then
            Dim segM = psStat.dicoSegManquant(sCle)
            Dim bMotExiste As Boolean = False
            For Each sMot In segM.lstMotsDico
                If sMot = sMotDicoUniforme Then bMotExiste = True
            Next
            If Not bMotExiste Then
                segM.iNbOccDicoFr += 1
                segM.lstMotsDico.Add(sMotDicoUniforme)
            End If
        Else
            Dim segM As New clsSegmentManquant
            segM.sCle = sCle
            segM.iNbOccDicoFr = 1
            segM.lstMotsDico.Add(sMotDicoUniforme)
            psStat.dicoSegManquant.Add(sCle, segM)
        End If

    End Sub

    Private Sub AjouterSegmentMultiple(dico As DicoTri(Of String, clsSegmentStat),
        sSegmentUnicite$, sSegment$, sMotDicoUniforme$, bPrefixe As Boolean, sSens$)

        Dim psStat As clsSegmentStat
        Dim sCle$ = sSegmentUnicite & ":" & sSens
        If Not dico.ContainsKey(sCle) Then
            psStat = New clsSegmentStat
            psStat.sSegment = sSegment
            psStat.iNbOccDicoFr = 1
            psStat.bPrefixe = bPrefixe
            psStat.lstMots.Add(sMotDicoUniforme)
            psStat.sSens = sSens
            psStat.sSegmentUnicite = sSegmentUnicite
            dico.Add(sCle, psStat)
        Else
            psStat = dico(sCle)
            psStat.bPrefixe = bPrefixe ' Correction si à la fois préfixe et suffixe
            psStat.iNbOccDicoFr += 1
            psStat.lstMots.Add(sMotDicoUniforme)
        End If

    End Sub

    Private Sub TraiterMot(prm As clsPrm, bPotentiel As Boolean,
        Optional ByRef bHypotheseTropLongue As Boolean = False)

        prm.bMotTrouve = False
        prm.sMotTrouve = ""
        bHypotheseTropLongue = False

        If prm.bPrefixe AndAlso prm.bSuffixe Then

            If prm.bPluriel Then Exit Sub

            Dim sElision$ = ""
            Dim bElision0 As Boolean = False
            If bElision AndAlso prm.iLongSuffixe + prm.iLongPrefixe = prm.iLongMot + 1 AndAlso
               prm.sPrefixe.EndsWith(sCarO) AndAlso prm.sSuffixe.StartsWith(sCarO) AndAlso
               prm.iLongPrefixe >= iLongPrefixeMinElision AndAlso prm.iLongSuffixe > 1 Then
                bElision0 = True
                sElision = prm.sPrefixe.Substring(0, prm.iLongPrefixe - 1) & sCarElisionO
                If bDebugElision Then Debug.WriteLine("Elision : " & prm.sMotDico & " : " & sElision & " - " & prm.sSuffixe)
            End If

            If Not bElision0 AndAlso prm.iLongSuffixe + prm.iLongPrefixe < prm.iLongMot Then

                Dim iLongDelta% = prm.iLongMot - (prm.iLongSuffixe + prm.iLongPrefixe)
                Dim sDelta$ = prm.sMotDico.Substring(prm.iLongPrefixe, iLongDelta)
                Dim sPrefixePot$ = sDelta
                Dim sPrefixesSuiv$ = prm.sPrefixe & " - " & sDelta
                Dim sPrefixeSuiv$ = sDelta
                Dim sPrefixeComplexPreced$ = prm.sPrefixe & "(" & prm.iNiveauPrefixe & ")"
                Dim sPrefixeComplexSuiv$ = sPrefixeComplexPreced & " - " & sDelta
                Dim iComplexiteSuiv% = prm.iNiveauPrefixe + 1

                If bElision AndAlso prm.iLongSuffixe + iLongDelta + prm.iLongPrefixe = prm.iLongMot AndAlso
                    prm.sPrefixe.EndsWith(sCarO) AndAlso Not sPrefixeSuiv.StartsWith(sCarO) AndAlso
                    prm.sPrefixe.Length > iLongPrefixeMinElision AndAlso iLongDelta > 1 Then
                    Dim sPrefixePotElision = sCarO & sPrefixePot
                    Dim lst2UnicitesP As New List(Of List(Of String))
                    Dim lstUnicitesP2 As New List(Of String)
                    Dim sUnicitePrincipaleP2$ = ""
                    Dim sSegment = sPrefixePotElision
                    Dim sSegmentUniforme = sSegment.ToLower
                    Dim iLongPrefixe2% = 0
                    Dim iNiveauP2% = 0
                    Dim sPrefixe2 = ""
                    Dim lstPrefixeDef2 As New List(Of String)
                    Dim bPrefixe2 = bPrefixe(sSegment, prm.aPrefixes,
                        sPrefixe2, lstPrefixeDef2, iLongPrefixe2, iNiveauP2, lstUnicitesP2,
                        sUnicitePrincipaleP2, iLongPrefixeMin:=sPrefixePotElision.Length)
                    If bPrefixe2 AndAlso lstPrefixeDef2.Count >= 1 Then

                        'If prm.sMotDico = "" Then
                        '    Debug.WriteLine("!")
                        'End If

                        Dim sPrefixeE$ = prm.sPrefixe.Substring(0, prm.iLongPrefixe - 1) & sCarElisionO
                        If bDebugElision Then
                            'Debug.WriteLine(
                            '    "Elision en récursif : " & prm.sMotDico & " : " &
                            '    prm.sPrefixe & " - " & sCarElisionO & sPrefixePot & " - " & prm.sSuffixe)
                            ' 04/05/2019
                            Debug.WriteLine(
                                "Elision en récursif : " & prm.sMotDico & " : " &
                                sPrefixeE & " - " & sPrefixePotElision & " - " & prm.sSuffixe)
                        End If

                        ' 26/04/2019 C'est un mot complexe maintenant : TraiterMotRecursif
                        sDelta = sPrefixePotElision
                        sPrefixePot = sDelta
                        'sPrefixesSuiv = prm.sPrefixe & " - " & sDelta
                        sPrefixesSuiv = sPrefixeE & " - " & sDelta ' 04/05/2019
                        sPrefixeSuiv = sDelta
                        'sPrefixeComplexPreced = prm.sPrefixe & "(" & prm.iNiveauPrefixe & ")"
                        sPrefixeComplexPreced = sPrefixeE & "(" & prm.iNiveauPrefixe & ")" ' 04/05/2019
                        sPrefixeComplexSuiv = sPrefixeComplexPreced & " - " & sDelta
                        TraiterMotRecursif(prm, prm.sPrefixe, prm.iLongPrefixe, sPrefixePot, prm.sPrefixe,
                            iComplexiteSuiv, sPrefixesSuiv, sPrefixeSuiv, iComplexiteSuiv, bPotentiel,
                            sPrefixeComplexSuiv, sPrefixeComplexPreced,
                            prm.sPrefixe, iNivGlobal:=2, iNbCarElision:=-1)

                        Exit Sub
                    End If
                End If

                TraiterMotRecursif(prm, prm.sPrefixe, prm.iLongPrefixe, sPrefixePot, prm.sPrefixe,
                    iComplexiteSuiv, sPrefixesSuiv, sPrefixeSuiv, iComplexiteSuiv, bPotentiel,
                    sPrefixeComplexSuiv, sPrefixeComplexPreced,
                    prm.sPrefixe, iNivGlobal:=2)

            ElseIf Not bElision0 AndAlso prm.iLongSuffixe + prm.iLongPrefixe > prm.iLongMot Then
                'If prm.bPluriel Then Exit Sub

                ' On va tester plusieurs hypothèses de préfixe, en réduisant progressivement
                '  la longueur du préfixe
                bHypotheseTropLongue = True

                ' Ici on fait une supposition que le préfixe est juste et que c'est 
                '  le suffixe qui ne va pas : ce n'est pas forcément vrai
                Dim iLongDelta% = prm.iLongMot - prm.iLongSuffixe
                Dim iDeb% = prm.iLongMot - prm.iLongSuffixe
                Dim iLong% = iDeb + iLongDelta
                If iLong > prm.iLongMot Then iLong = prm.iLongMot
                Dim iDelta2% = iLong - iDeb
                Dim sDelta$ = prm.sMotDico.Substring(iDeb, iDelta2)
                For Each sSuffixeDef In prm.lstSuffixeDef
                    For Each lstPrefixeDef In prm.lst2PrefixeDef
                        For Each sPrefixeDef In lstPrefixeDef
                            Dim sCle$ = prm.sPrefixe & " - " & prm.sSuffixe & " : " &
                                prm.iLongSuffixe + prm.iLongPrefixe & " > " & prm.iLongMot & " : " &
                                sDelta & " en trop"
                            Dim sLigne$ = prm.sMotDico & " : " &
                                sSuffixeDef.ToUpper & sSepDef &
                                sCompleterPrefixe(sPrefixeDef.ToUpper) & " : " & sCle
                            If Not prm.dicoDefIncompletes.ContainsKey(sLigne) Then _
                                prm.dicoDefIncompletes.Add(sLigne,
                                    New clsLigne(sLigne, sCle, sDelta, prm.sMotDico))
                        Next
                    Next
                Next

            Else ' prm.iLongSuffixe + prm.iLongPrefixe = prm.iLongMot

                AjouterMotSimpleRecursif(prm, bPotentiel, bElision0, sElision)

            End If

        ElseIf prm.bPrefixe Then
            If prm.bPluriel Then Exit Sub
            If prm.bPremiereCombi AndAlso prm.iPasse = 0 Then prm.iNbMotsPrefixeExistants += 1
            Dim iNumP1% = 0
            For Each lstPrefixeDef In prm.lst2PrefixeDef
                Dim lstUnicitesP1 = prm.lst2UnicitesPrefixe(iNumP1) ' 15/12/2018
                iNumP1 += 1
                Dim iNumP2% = 0
                For Each sPrefixeDef In lstPrefixeDef
                    Dim sLigne$ = prm.sMotDico & " : " & prm.sPrefixe & "- : " & sPrefixeDef
                    prm.sbPrefixes.AppendLine(sLigne)
                Next
            Next
        ElseIf prm.bSuffixe Then
            If prm.bPluriel Then Exit Sub
            If prm.bPremiereCombi AndAlso prm.iPasse = 0 Then prm.iNbMotsSuffixeExistants += 1
            For Each sSuffixeDef In prm.lstSuffixeDef
                prm.sbSuffixes.AppendLine(prm.sMotDico & " : -" & prm.sSuffixe & " : " & sSuffixeDef)
            Next
        Else
            If prm.bPluriel Then Exit Sub
            prm.sbAutres.AppendLine(prm.sMotDico)
        End If

    End Sub

    Private Sub AjouterMotSimpleRecursif(prm As clsPrm, bPotentiel As Boolean, bElision0 As Boolean,
        sElision$, Optional sDefElision$ = "")

        Dim iNbOcc% = 0
        Dim iNumS% = 0
        Dim sMemDef$ = ""
        Dim iNbSuffixes% = prm.lstSuffixeDef.Count
        For Each sSuffixeDef In prm.lstSuffixeDef
            Dim sUniciteS = prm.lstUnicitesSuffixe(iNumS) ' 15/12/2018
            iNumS += 1
            Dim iNumP1% = 0
            For Each lstPrefixeDef In prm.lst2PrefixeDef
                Dim lstUnicitesP1 = prm.lst2UnicitesPrefixe(iNumP1) ' 15/12/2018
                iNumP1 += 1
                Dim iNumP2% = 0
                Dim iNbPrefixes% = lstPrefixeDef.Count
                For Each sPrefixeDef In lstPrefixeDef
                    Dim sUniciteP = lstUnicitesP1(iNumP2) ' 15/12/2018
                    iNumP2 += 1

                    Dim sDefPrefixe$ = sCompleterPrefixe(sPrefixeDef.ToUpper)
                    Dim sDef$ = sSuffixeDef.ToUpper & sSepDef & sDefPrefixe
                    'Dim sLigne$ = prm.sMotDico & " : " &
                    '    sDef & " : " & prm.sPrefixe & " - " & prm.sSuffixe
                    Dim sPrefixeElision = prm.sPrefixe
                    If bElision0 Then
                        If sDefElision.Length > 0 Then
                            sDefPrefixe &= " " & sCompleterPrefixe(sDefElision.ToUpper)
                        End If
                        sDef = sSuffixeDef.ToUpper & sSepDef & sDefPrefixe
                        sPrefixeElision = sElision
                        'sLigne = prm.sMotDico & " : " & sDef & " : " & sPrefixeElision & " - " & prm.sSuffixe
                    End If
                    Dim sLigne$ = prm.sMotDico & " : " &
                        sDef & " : " & sPrefixeElision & " - " & prm.sSuffixe

                    'If prm.sMotDico = "" Then
                    '    Debug.WriteLine("!")
                    'End If

                    Dim sDefPot$ = prm.sMotDico & ";" & sDef
                    ' 05/05/2019
                    Dim sDefPotCom$ = sDefPot & " // " & sPrefixeElision & " - " & prm.sSuffixe

                    ' 31/08/2018
                    Dim sCleExcl$ = "-" & prm.sSuffixe
                    If m_defFls.bSensExclusifAutre(sCleExcl, prm.sMotDico, sSuffixeDef,
                        bPrefixe:=False) Then Continue For

                    'sCleExcl = sPrefixeElision & "-"
                    sCleExcl = prm.sPrefixe & "-"
                    If m_defFls.bSensExclusifAutre(sCleExcl, prm.sMotDico, sPrefixeDef,
                        bPrefixe:=True) Then Continue For

                    If m_hsExclDef.Contains(sDefPot) Then
                        ' Mémoriser les déf. fausses utiles
                        If Not m_hsExclDefUtil.Contains(sDefPot) Then _
                            m_hsExclDefUtil.Add(sDefPot)
                        Continue For
                    End If

                    'Dim sCle$ = prm.sMotDico & " : " & sDef
                    Dim sCle$ = prm.sMotDicoUniforme & " : " & sDef ' 18/04/2019
                    If prm.hsUnicitesDefMots.Contains(sCle) Then Continue For
                    prm.hsUnicitesDefMots.Add(sCle)

                    prm.bMotTrouve = True
                    prm.sMotTrouve = prm.sMotDico & " : " & sDef ' Debug

                    If Not prm.mots.bAjouterLigne(prm.sMotDicoUniforme, sLigne) Then
                        ' à voir : ici on perd sy - métrique car on a déjà sy - métr - ique
                        ' symétrique : À PROPOS  AVEC DE LA MESURE : sy - métr - ique
                        ' symétrique : MESURAGE  AVEC : sy - métrique
                        Continue For
                    Else
                        ' 19/01/2019 Eviter les doublons entre les mots complexes et simples
                        'Dim sDefPotDecoup$ = sPrefixeElision & " - " & prm.sSuffixe
                        ' 05/05/2019 sDefPotDecoup -> sDefPotCom
                        If Not prm.dicoUnicitesDoublons2.ContainsKey(prm.sMotDicoUniforme) Then _
                            prm.dicoUnicitesDoublons2.Add(prm.sMotDicoUniforme, sDefPotCom) 'sDefPotDecoup)
                        If Not prm.dicoUnicitesDoublons2Suff.ContainsKey(prm.sMotDicoUniforme) Then _
                            prm.dicoUnicitesDoublons2Suff.Add(prm.sMotDicoUniforme, prm.sSuffixe)
                        If Not prm.dicoUnicitesDoublons2Pref.ContainsKey(prm.sMotDicoUniforme) Then _
                            prm.dicoUnicitesDoublons2Pref.Add(prm.sMotDicoUniforme, sPrefixeElision)
                    End If
                    Dim sLigneMotSimple$ = sLigne
                    Dim sLigneFichier$ = sDefPot & ";" & sPrefixeElision & ";" & prm.sSuffixe
                    Dim sLigneMotSimpleFichierTxt$ = sLigneFichier
                    Dim sLigneMotSimpleFichierCsv$ = sLigneFichier
                    Dim sLigneMotSimpleFichierCode$ = ""

                    If iNbSuffixes > 1 OrElse iNbPrefixes > 1 Then
                        Dim sCle0$ = prm.sMotDico & " : " & sDef
                        'Debug.WriteLine("Mot simple avec un sens multiple : " & sLigne & " : " & sMemDef)
                        If iNbPrefixes > 1 Then
                            Dim bPrefixe = True
                            Dim sSegment = sPrefixeElision
                            Dim sSegmentU = sPrefixeElision & ":" & sUniciteP
                            If bDebugSensMultiple Then Debug.WriteLine("Mot simple avec un sens multiple : " &
                                sLigne & " : " & sSegmentU)
                            AjouterSegmentMultiple(prm.dicoSegmentsSensMultiples,
                                sSegmentU, sSegment, prm.sMotDicoMinuscule, bPrefixe, sPrefixeDef)
                        End If
                        If iNbSuffixes > 1 Then
                            Dim bPrefixe = False
                            Dim sSegment = prm.sSuffixe
                            Dim sSegmentU = prm.sSuffixe & ":" & sUniciteS
                            If bDebugSensMultiple Then Debug.WriteLine("Mot simple avec un sens multiple : " &
                                sLigne & " : " & sSegmentU)
                            AjouterSegmentMultiple(prm.dicoSegmentsSensMultiples,
                                sSegmentU, sSegment, prm.sMotDicoMinuscule, bPrefixe, sSuffixeDef)
                        End If
                    End If

                    Dim bAjout As Boolean = True
                    If Not bPotentiel Then

                        iNbOcc += 1
                        If iNbOcc > 1 Then
                            If Not m_bAfficherAvertDoublon Then
                                Dim sMsg$ = "Double sens : compléter le fichier Doc\DefinitionsFausses.txt :"
                                m_msgDelegue.AfficherMsg(sMsg)
                                Debug.WriteLine(sMsg)
                                m_bAfficherAvertDoublon = True
                            End If
                            If Not prm.hsUnicitesDoublons.Contains(sDefPot) Then
                                prm.hsUnicitesDoublons.Add(sDefPot)
                                m_msgDelegue.AfficherMsg(sMemDef)
                                m_msgDelegue.AfficherMsg(sDefPotCom)
                                Debug.WriteLine(sMemDef)
                                Debug.WriteLine(sDefPotCom)
                            End If
                        Else
                            sMemDef = sDefPotCom
                        End If

                        sLigneMotSimple = sLigne & " : " & prm.iNiveauPrefixe & " " & prm.iNiveauSuffixe
                        If Not prm.motsSimples.bAjouterLigne(prm.sMotDicoUniforme, sLigneMotSimple) Then
                            Continue For
                        End If

                        sLigneMotSimpleFichierTxt = sLigneFichier & ";" &
                            prm.iNiveauPrefixe & ";" & prm.iNiveauSuffixe
                        sLigneMotSimpleFichierCsv = sLigneMotSimpleFichierTxt & ";" &
                            sUniciteP & ";" & sUniciteS & ";" &
                            prm.sFreqPrefixe & ";" & prm.sFreqSuffixe
                        sLigneMotSimpleFichierCode = "        " &
                            sGm & prm.sMotDico & sGm & ", " &
                            sGm & sDef & sGm & ", " &
                            sGm & sPrefixeElision & sGm & ", " &
                            sGm & prm.sSuffixe & sGm & ", " &
                            sGm & prm.iNiveauPrefixe & sGm & ", " &
                            sGm & prm.iNiveauSuffixe & sGm & ", " &
                            sGm & sUniciteP & sGm & ", " &
                            sGm & sUniciteS & sGm & ", " &
                            sGm & prm.sFreqPrefixe & sGm & ", " &
                            sGm & prm.sFreqSuffixe & sGm & ","
                    End If
                    If bAjout Then
                        prm.sbPrefixeEtSuffixeFichierTxt.AppendLine(sLigneMotSimpleFichierTxt)
                        prm.sbPrefixeEtSuffixeFichierCsv.AppendLine(sLigneMotSimpleFichierCsv)
                        prm.sbMotSimpleFichierCode.AppendLine("        " & sLigneMotSimpleFichierCode)

                        ' Non, c'est inverse, on veut ajouter ceux-là dans Mots_fr.txt
                        ' 13/01/2019 Ajouter aussi l'unicité dans un autre fichier (mots simples)
                        'If iNivMaxMotsComplexesUnicite > 0 Then
                        '    Dim sLigneU$ = prm.sMotDico & " | " & sDef & " | " &
                        '        prm.sSuffixe & " - " & prm.sPrefixe & " | " &
                        '        sUniciteS & " - " & sUniciteP
                        '    prm.sbPrefixesEtSuffixeUnicite.AppendLine(sLigneU)
                        '    prm.iNbMotsLogotronExistantsUnicite += 1
                        'End If

                    End If

                    If Not bPotentiel Then
                        Dim iComplex% = (prm.iNiveauPrefixe + 1) * (prm.iNiveauSuffixe + 1)
                        If Not prm.dicoComplex.ContainsKey(prm.sMotDico) Then _
                            prm.dicoComplex.Add(prm.sMotDico,
                        New clsMot(prm.sMotDico, iComplex,
                            sPrefixeElision & "(" & prm.iNiveauPrefixe & ") - " &
                            prm.sSuffixe & "(" & prm.iNiveauSuffixe & ")", sDef, 2))
                    End If
                Next
            Next
        Next

        If prm.dicoPrefSuff.ContainsKey(prm.sPrefixe) Then
            Dim segStat = prm.dicoPrefSuff(prm.sPrefixe)
            segStat.bComposable = True
        End If
        If prm.dicoPrefSuff.ContainsKey(prm.sSuffixe) Then
            Dim segStat = prm.dicoPrefSuff(prm.sSuffixe)
            segStat.bComposable = True
        End If

    End Sub

    Private Sub TraiterMotRecursif(prm As clsPrm, sPrefixesPreced$,
        iLongPrefixesPreced%, sPrefixePot$, sPrefixePreced$, iComplexitePreced%,
        ByRef sPrefixesSuiv$, ByRef sPrefixeSuiv$, ByRef iComplexiteSuiv%, bPotentiel As Boolean,
        sPrefixeComplexSuiv$, sPrefixesComplexPreced$,
        sPrefixePrecedBase$, iNivGlobal%, Optional iNbCarElision% = 0)

        Dim sDelta$ = sPrefixePot

        Dim lst2UnicitesP As New List(Of List(Of String))
        Dim lstUnicitesP2 As New List(Of String)
        Dim sUnicitePrincipaleP2$ = ""
        Dim sSegment = sDelta
        Dim sSegmentUniforme = sDelta.ToLower
        Dim iLongPrefixe2% = 0
        Dim iNiveauP2% = 0
        Dim sPrefixe2 = ""
        Dim lstPrefixeDef2 As New List(Of String)
        Dim bPrefixe2 = bPrefixe(sSegment, prm.aPrefixes,
            sPrefixe2, lstPrefixeDef2, iLongPrefixe2, iNiveauP2, lstUnicitesP2, sUnicitePrincipaleP2)
        If bPrefixe2 Then
            prm.lst2PrefixeDef.Add(lstPrefixeDef2) ' ToDo : SSi mot retenu ?
            'If lstUnicitesP2.Count > 0 Then prm.lst2UnicitesPrefixe.Add(lstUnicitesP2)
            prm.lst2UnicitesPrefixe.Add(lstUnicitesP2) ' 09/12/2018 Ajouter même "" pour conserver l'indexe
            AjouterSegment(prm.dicoPrefSuff, sPrefixe2, sSegmentUniforme, bPrefixe:=True)
        End If

        If Not bPrefixe2 Then
            If prm.bPluriel Then Exit Sub
            For Each sSuffixeDef In prm.lstSuffixeDef
                For Each lstPrefixesDef In prm.lst2PrefixeDef
                    For Each sPrefixeDef In lstPrefixesDef
                        Dim sCle$ = sPrefixesPreced & " - " & prm.sSuffixe & " : " &
                            prm.iLongSuffixe + iLongPrefixesPreced & " < " & prm.iLongMot &
                            " : manque " & sDelta
                        Dim sLigne$ = prm.sMotDico & " : " &
                            sSuffixeDef.ToUpper & sSepDef &
                            sCompleterPrefixe(sPrefixeDef.ToUpper) & " : " & sCle
                        If Not prm.dicoDefIncompletes.ContainsKey(sLigne) Then _
                            prm.dicoDefIncompletes.Add(sLigne,
                                New clsLigne(sLigne, sCle, sDelta, prm.sMotDico))
                        AjouterSegment(prm.dicoPrefixesManquants, sDelta, prm.sMotDicoMinuscule, bPrefixe:=True,
                            sPrefixePreced:=sPrefixePreced, sSuffixeSuiv:=prm.sSuffixe)
                    Next
                Next
            Next

        Else

            Dim iLongTot% = prm.iLongSuffixe + iLongPrefixesPreced + iLongPrefixe2 + iNbCarElision
            'Dim sElision$ = ""
            'Dim sPrefixeSuivElision$ = ""
            'Dim sPrefixesSuivElision$ = ""

            If iLongTot < prm.iLongMot Then

                ' Appel récurrsif
                Dim sPrefixesPreced2$ = sPrefixesPreced & sPrefixe2
                Dim iLongPrefixesPreced2% = sPrefixesPreced2.Length
                Dim iLongDelta2% = prm.iLongMot - (prm.iLongSuffixe + iLongPrefixesPreced2)
                Dim sDelta2$ = prm.sMotDico.Substring(iLongPrefixesPreced2, iLongDelta2)
                Dim sPrefixePot2$ = sDelta2

                sPrefixesSuiv = sPrefixePrecedBase & " - " & sPrefixe2 & " - " & sPrefixePot2 ' 22/06/2018
                sPrefixeSuiv = sPrefixePot2
                sPrefixePrecedBase = sPrefixePrecedBase & " - " & sPrefixe2 ' 22/06/2018
                sPrefixeComplexSuiv = sPrefixesComplexPreced & " - " &
                    sPrefixe2 & "(" & iNiveauP2 & ") - " & sPrefixePot2
                Dim iComplexiteSuiv2% = iComplexitePreced * (iNiveauP2 + 1)
                TraiterMotRecursif(prm, sPrefixesPreced2, iLongPrefixesPreced2, sPrefixePot2, sPrefixe2,
                     iComplexitePreced, sPrefixesSuiv, sPrefixeSuiv, iComplexiteSuiv2, bPotentiel,
                     sPrefixeComplexSuiv, sPrefixesComplexPreced,
                     sPrefixePrecedBase, iNivGlobal:=iNivGlobal + 1)

            ElseIf iLongTot > prm.iLongMot Then

                If prm.bPluriel Then Exit Sub
                ' On ne passe plus ici
                If bDebug Then Stop

            Else ' If iLongTot = prm.iLongMot Then

                If prm.bPluriel Then Exit Sub
                Dim iComplexiteSuiv2% = iComplexiteSuiv * (iNiveauP2 + 1)
                Dim iNbSuffixes% = prm.lstSuffixeDef.Count
                Dim iNumS% = 0
                For Each sSuffixeDef0 In prm.lstSuffixeDef
                    Dim sUniciteS$ = prm.lstUnicitesSuffixe(iNumS)
                    iNumS += 1
                    Dim sbPrefixes As New StringBuilder
                    Dim sbUnicites As New StringBuilder
                    DevelopperDef(prm, 0, sSuffixeDef0, sUniciteS,
                        sPrefixesSuiv, sPrefixeSuiv, sPrefixePreced, iComplexiteSuiv2,
                        sbPrefixes, sbUnicites, bPotentiel, iNiveauP2,
                        sPrefixeComplexSuiv, iNivGlobal, iNbSuffixes, 0)
                        'bElision0, sPrefixesSuivElision, sPrefixeSuivElision)
                Next
            End If

        End If

    End Sub

    Private Sub DevelopperDef(prm As clsPrm, iNiv%, sSuffixeDef$, sUniciteS$,
        sPrefixesSuiv$, sPrefixeSuiv$, sPrefixePreced$, iComplexiteSuiv%,
        ByRef sbPrefixes As StringBuilder, ByRef sbUnicites As StringBuilder,
        bPotentiel As Boolean, iComplexiteDern%,
        sPrefixeComplexSuiv$, iNivGlobal%, iNbSuffixes%, iNumPNM1%)
        'bElision1 As Boolean, sPrefixesSuivElision$, sPrefixeSuivElision$)

        Dim iNivMax% = prm.lst2PrefixeDef.Count
        Dim sbMemPrefixes As New StringBuilder(sbPrefixes.ToString)
        Dim sbMemUnicites As New StringBuilder(sbUnicites.ToString)
        Dim iNumP% = 0
        Dim lstUniciteP = prm.lst2UnicitesPrefixe(iNiv)
        Dim iNbPrefixes = prm.lst2PrefixeDef(iNiv).Count
        Dim sMemDef$ = ""
        For Each sPrefixeDef In prm.lst2PrefixeDef(iNiv) ' Là il faut dédoubler les lignes
            Dim sUniciteP$ = lstUniciteP(iNumP)
            'iNumP += 1

            Dim sCompleterPref$ = sCompleterPrefixe(sPrefixeDef.ToUpper)
            If sbPrefixes.Length > 0 Then sbPrefixes.Append(" ")
            sbPrefixes.Append(sCompleterPref)

            ' 15/12/2018
            sbUnicites.Append(sUniciteP)
            If iNiv < iNivMax - 1 Then sbUnicites.Append("+")

            If iNiv < iNivMax - 1 Then
                ' Appel récursif
                DevelopperDef(prm, iNiv + 1, sSuffixeDef, sUniciteS,
                    sPrefixesSuiv, sPrefixeSuiv, sPrefixePreced, iComplexiteSuiv,
                    sbPrefixes, sbUnicites, bPotentiel, iComplexiteDern,
                    sPrefixeComplexSuiv, iNivGlobal, iNbSuffixes, iNumP)
                    'bElision1, sPrefixesSuivElision, sPrefixeSuivElision)
            ElseIf iNiv = iNivMax - 1 Then

                AjouterMotComplexe(prm, iNiv, sSuffixeDef, sUniciteS, sUniciteP,
                    sPrefixesSuiv, sPrefixeSuiv, sPrefixePreced, iComplexiteSuiv,
                    sbPrefixes, sbUnicites, bPotentiel, iComplexiteDern,
                    sPrefixeComplexSuiv, iNivGlobal, iNbSuffixes, iNbPrefixes, iNumPNM1, sPrefixeDef,
                    sMemDef)

            End If

DefinitionSuivante:
            sbPrefixes = New StringBuilder(sbMemPrefixes.ToString) ' 28/08/2017
            sbUnicites = New StringBuilder(sbMemUnicites.ToString)
            iNumP += 1

        Next

    End Sub

    Private Sub AjouterMotComplexe(prm As clsPrm, iNiv%, sSuffixeDef$, sUniciteS$, sUniciteP$,
        sPrefixesSuiv$, sPrefixeSuiv$, sPrefixePreced$, iComplexiteSuiv%,
        sbPrefixes As StringBuilder, ByRef sbUnicites As StringBuilder,
        bPotentiel As Boolean, iComplexiteDern%,
        sPrefixeComplexSuiv$, iNivGlobal%, iNbSuffixes%, iNbPrefixes%, iNumPNM1%, sPrefixeDef$,
        ByRef sMemDef$)

        ' 31/08/2018
        Dim sCleExcl$ = "-" & prm.sSuffixe
        If m_defFls.bSensExclusifAutre(sCleExcl, prm.sMotDico, sSuffixeDef, bPrefixe:=False) Then _
            Exit Sub

        sCleExcl = sPrefixeSuiv & "-" ' 31/12/2018
        If m_defFls.bSensExclusifAutre(sCleExcl, prm.sMotDico, sPrefixeDef, bPrefixe:=True) Then _
            Exit Sub

        'If prm.sMotDico = "" Then
        '    Debug.WriteLine("!")
        'End If

        ' Il faut aussi tester le 1er préfixe (et ceux au milieu : todo)
        If iNiv = 1 Then
            sCleExcl = prm.sPrefixe & "-"
            Dim iNiv0% = 0 ' Le 1er préfixe pour le moment, ToDo : iNiv - 1
            Dim sPrefixeDef0$ = prm.lst2PrefixeDef(iNiv0)(iNumPNM1)
            If m_defFls.bSensExclusifAutre(sCleExcl, prm.sMotDico, sPrefixeDef0, bPrefixe:=True) Then _
                Exit Sub
        End If

        Dim sDef$ = sSuffixeDef.ToUpper & sSepDef & sbPrefixes.ToString
        Dim sDefPot$ = prm.sMotDico & ";" & sDef
        Dim sDecoup$ = sPrefixesSuiv & " - " & prm.sSuffixe
        'If bElision1 Then sDecoup = sPrefixesSuivElision & " - " & prm.sSuffixe
        'Dim sDefPotDecoup$ = sDefPot & " : " & sDecoup
        Dim sDefPotDecoup$ = sDefPot & " // " & sDecoup ' 05/05/2019

        ' 16/12/2018 Gestion des définitions fausses aussi pour les mots complexes
        If m_hsExclDef.Contains(sDefPot) Then
            ' Mémoriser les déf. fausses utiles
            If Not m_hsExclDefUtil.Contains(sDefPot) Then m_hsExclDefUtil.Add(sDefPot)
            Exit Sub
        End If

        ' 06/01/2019 Si c'est déjà un mot simple, éviter de le couper
        ' (ex.: -graphique : graph- -ique)
        'If prm.mots.hs.Contains(prm.sMotDico) Then Exit Sub
        If prm.mots.hs.Contains(prm.sMotDicoUniforme) Then Exit Sub ' 07/12/2019

        'Dim sCle$ = prm.sMotDico & " : " & sDef
        Dim sCle$ = prm.sMotDicoUniforme & " : " & sDef ' 18/04/2019
        If Not prm.hsUnicitesDefMots.Contains(sCle) Then

            prm.hsUnicitesDefMots.Add(sCle)

            Dim bDoublon As Boolean = False

            Dim sLigne$ = prm.sMotDico & " : " & sDef & " : " & sDecoup
            If prm.motsComplexes.bAjouterLigne(prm.sMotDicoUniforme, sLigne) Then
                If Not prm.dicoUnicitesDoublons2.ContainsKey(prm.sMotDicoUniforme) Then _
                    prm.dicoUnicitesDoublons2.Add(prm.sMotDicoUniforme, sDefPotDecoup) ' 16/12/2018
                If Not prm.dicoUnicitesDoublons2Suff.ContainsKey(prm.sMotDicoUniforme) Then _
                    prm.dicoUnicitesDoublons2Suff.Add(prm.sMotDicoUniforme, prm.sSuffixe) ' 04/01/2019
                If Not prm.dicoUnicitesDoublons2Pref.ContainsKey(prm.sMotDicoUniforme) Then _
                    prm.dicoUnicitesDoublons2Pref.Add(prm.sMotDicoUniforme, prm.sPrefixe) ' 04/01/2019
            ElseIf Not bPotentiel AndAlso bAfficherDoublonMotsComplexes Then
                bDoublon = True
                If iNiv < iNivMaxAffDoublonMotsComplexes Then

                    Dim sMemDef0 = prm.dicoUnicitesDoublons2(prm.sMotDicoUniforme)
                    Dim sSuffixe0 = prm.dicoUnicitesDoublons2Suff(prm.sMotDicoUniforme)
                    Dim sPrefixe0 = prm.dicoUnicitesDoublons2Pref(prm.sMotDicoUniforme)
                    Dim sCombinSuffixe = sPrefixeSuiv & prm.sSuffixe
                    ' 04/01/2019 Si le suffixe est exactement une combinaison
                    '  du dernier préfixe avec le dernier suffixe
                    '  alors ignorer ce mot, car on conserve tjrs le préfixe
                    '  le plus grand : métrique et métr-ique : préférer métrique
                    If sSuffixe0 = sCombinSuffixe Then
                        'Debug.WriteLine(prm.sMotDico & " : " & sMemDef0 & " : " & sCombinSuffixe & " > " & sPrefixeSuiv & "-" & prm.sSuffixe)
                        Exit Sub
                    End If
                    ' Pareil pour le préfixe
                    Dim sCombinPrefixe = prm.sPrefixe & sPrefixePreced
                    If sPrefixe0 = sCombinPrefixe Then
                        'Debug.WriteLine(prm.sMotDico & " : " & sMemDef0 & " : " & sCombinPrefixe & " > " & prm.sPrefixe & "-" & sPrefixePreced)
                        Exit Sub
                    End If

                    ' Affichage des doublons pour les mots complexes : il y a en beaucoup
                    ' 29/08/2018 Double sens généralisé : pas seulement def. multiple d'un suffixe
                    If Not m_bAfficherAvertDoublon Then
                        Dim sMsg$ = "Double sens : compléter le fichier Doc\DefinitionsFausses.txt :"
                        m_msgDelegue.AfficherMsg(sMsg)
                        Debug.WriteLine(sMsg)
                        m_bAfficherAvertDoublon = True
                    End If
                    If Not prm.hsUnicitesDoublons.Contains(sDefPot) Then
                        prm.hsUnicitesDoublons.Add(sDefPot)
                        ' 16/12/2018 Afficher la 1ère occurrence une 1ère fois
                        If Not prm.hsUnicitesDoublons2.Contains(prm.sMotDico) Then
                            prm.hsUnicitesDoublons2.Add(prm.sMotDico)
                            m_msgDelegue.AfficherMsg(sMemDef0)
                            Debug.WriteLine(sMemDef0)
                        End If
                        m_msgDelegue.AfficherMsg(sDefPotDecoup)
                        Debug.WriteLine(sDefPotDecoup)
                    End If
                End If
            End If

            prm.bMotTrouve = True
            prm.sMotTrouve = prm.sMotDico & " : " & sDef
            If Not prm.mots.bAjouterLigne(prm.sMotDicoUniforme, sLigne) Then Exit Sub

            If Not bPotentiel Then

                ' 09/12/2018 Ajouter aussi l'unicité dans un autre fichier
                If iNivMaxMotsComplexesUnicite > 0 AndAlso
                    iNiv < iNivMaxMotsComplexesUnicite Then

                    If Not bDoublon AndAlso (iNbSuffixes > 1 OrElse iNbPrefixes > 1) Then
                        Dim sCle0$ = prm.sMotDico & " : " & sDef
                        'Debug.WriteLine("Mot complexe avec un sens multiple : " & sLigne & " : " & sMemDef)
                        If iNbPrefixes > 1 Then
                            Dim bPrefixe = True
                            Dim sSegment = sPrefixeSuiv
                            Dim sSegmentU = sPrefixeSuiv & ":" & sUniciteP
                            If bDebugSensMultiple Then Debug.WriteLine("Mot complexe avec un sens multiple : " &
                                sLigne & " : " & sSegmentU)
                            AjouterSegmentMultiple(prm.dicoSegmentsSensMultiples,
                                sSegmentU, sSegment, prm.sMotDicoMinuscule, bPrefixe, sPrefixeDef)
                        End If
                        If iNbSuffixes > 1 Then
                            Dim bPrefixe = False
                            Dim sSegment = prm.sSuffixe
                            Dim sSegmentU = prm.sSuffixe & ":" & sUniciteS
                            If bDebugSensMultiple Then Debug.WriteLine("Mot complexe avec un sens multiple : " &
                                sLigne & " : " & sSegmentU)
                            AjouterSegmentMultiple(prm.dicoSegmentsSensMultiples,
                                sSegmentU, sSegment, prm.sMotDicoMinuscule, bPrefixe, sSuffixeDef)
                        End If
                    Else
                        sMemDef = sDefPot
                    End If

                    Dim sUnicitePrefixe = sbUnicites.ToString
                    'Dim sLigneU$ = prm.sMotDico & " | " & sDef & " | " &
                    '    sPrefixesSuiv & " - " & prm.sSuffixe & " | " &
                    '    sUnicitePrefixe & " - " & sUniciteS
                    ' 05/01/2019 D'abord le suffixe, puis les préfixes
                    Dim sLigneU$ = prm.sMotDico & " | " & sDef & " | " &
                        prm.sSuffixe & " - " & sPrefixesSuiv & " | " &
                        sUniciteS & " - " & sUnicitePrefixe
                    prm.sbPrefixesEtSuffixe2Unicite.AppendLine(sLigneU)
                    prm.iNbMotsLogotronExistantsUnicite += 1
                End If

                Dim iComplex% = iComplexiteSuiv * (prm.iNiveauSuffixe + 1)
                If Not prm.dicoComplex.ContainsKey(prm.sMotDico) Then _
                    prm.dicoComplex.Add(prm.sMotDico,
                        New clsMot(prm.sMotDico, iComplex,
                            sPrefixeComplexSuiv & "(" & iComplexiteDern & ") - " &
                            prm.sSuffixe & "(" & prm.iNiveauSuffixe & ")", sDef, iNivGlobal + 1))
            End If
        End If

    End Sub

    Public Function sListerTxt$(lstTxt As List(Of String), Optional iNbMax% = 0)
        Dim sb As New StringBuilder("")
        Dim iNumOcc% = 0
        For Each sDef0 In lstTxt
            If sb.Length > 0 Then sb.Append(", ")
            'sb.Append(sDef0)
            ' 23/06/2018
            If iNbMax = 0 OrElse (iNbMax > 0 AndAlso iNumOcc < iNbMax) Then sb.Append(sDef0)
            iNumOcc += 1
            If iNbMax > 0 Then
                'If iNumOcc >= iNbMax Then sb.Append("..") : Exit For
                ' 23/06/2018
                If iNumOcc > iNbMax Then sb.Append("..") : Exit For ' ...
            End If
        Next
        Return sb.ToString
    End Function

    Private Function bPrefixe(sMotDico$, aPrefixes As clsSegment(),
        ByRef sPrefixe$, ByRef lstPrefixeDef As List(Of String), ByRef iLongPrefixe%,
        Optional ByRef iNiveau% = 0,
        Optional ByRef lstUnicites As List(Of String) = Nothing,
        Optional ByRef sUnicitePrincipale$ = "",
        Optional ByRef sFreqPrefixe$ = "",
        Optional iLongPrefixeMax% = 0,
        Optional iLongPrefixeMin% = 0) As Boolean

        Dim bPrefixe0 = False
        sPrefixe = ""
        lstPrefixeDef = New List(Of String)
        lstUnicites = New List(Of String) ' 15/12/2018
        iLongPrefixe = 0
        iNiveau = 0
        sFreqPrefixe = ""
        For Each prefixe In aPrefixes
            sPrefixe = prefixe.sSegment
            If sMotDico.StartsWith(sPrefixe) Then

                ' 26/05/2018
                If iLongPrefixeMax > 0 Then
                    ' Chercher un préfixe plus petit (passe suivante)
                    Dim iLongP% = sPrefixe.Length
                    If iLongP >= iLongPrefixeMax Then Continue For
                End If

                ' 22/04/2019
                If iLongPrefixeMin > 0 Then
                    Dim iLongP% = sPrefixe.Length
                    If iLongP < iLongPrefixeMin Then Continue For
                End If

                bPrefixe0 = True
                lstPrefixeDef = prefixe.lstDefinitions
                iLongPrefixe = sPrefixe.Length
                iNiveau = prefixe.iNiveau
                sFreqPrefixe = prefixe.sFrequence
                'sUnicite = prefixe.sUnicite ' Test
                lstUnicites = prefixe.lstUnicites ' Test
                'sUnicitePrincipale = prefixe.sUnicitePrincipale ' Test
                'If sPrefixe = "semio" Then
                '    Debug.WriteLine("!")
                'End If
                'If lstPrefixeDef.Count > lstUnicites.Count Then
                '    Debug.WriteLine("!")
                'End If
                Exit For
            End If

            'sPrefixe = prefixe.sPrefixeSuffixeUniforme
            'If sMotDicoUniforme.StartsWith(sPrefixe) Then
            '    bPrefixe0 = True
            '    sPrefixeDef = prefixe.lstDefinitions
            '    iLongPrefixe = sPrefixe.Length
            '    'If sPrefixe = "semio" Then
            '    '    Debug.WriteLine("!")
            '    'End If
            '    'Debug.WriteLine("Correction accent préfixe : " & sMotDico & " : " & _
            '    '    prefixe.sPrefixeSuffixe & " -> " & prefixe.sPrefixeSuffixeUniforme)
            '    Exit For
            'End If

        Next

        Return bPrefixe0

    End Function

    Private Function bSuffixe(sMotDico$, aSuffixes As clsSegment(),
        ByRef sSuffixe$, ByRef lstSuffixeDef As List(Of String), ByRef iLongSuffixe%,
        Optional ByRef iNiveau% = 0,
        Optional ByRef lstUnicites As List(Of String) = Nothing, _
        Optional ByRef sUnicitePrincipale$ = "",
        Optional ByRef sFreqSuffixe$ = "",
        Optional iLongSuffixeMax% = 0) As Boolean

        Dim bSuffixe0 = False
        sSuffixe = ""
        lstSuffixeDef = New List(Of String)
        iLongSuffixe = 0
        iNiveau = 0
        sFreqSuffixe = ""
        For Each suffixe In aSuffixes
            sSuffixe = suffixe.sSegment
            'If sSuffixe = "cole" Then
            '    Debug.WriteLine("!")
            'End If
            If sSuffixe.Length < iLongSuffixeMin Then Continue For
            If sMotDico.EndsWith(sSuffixe) Then

                ' 25/08/2018
                If iLongSuffixeMax > 0 Then
                    ' Chercher un suffixe plus petit (passe suivante)
                    Dim iLongP% = sSuffixe.Length
                    If iLongP >= iLongSuffixeMax Then Continue For
                End If

                bSuffixe0 = True
                lstSuffixeDef = suffixe.lstDefinitions
                iLongSuffixe = sSuffixe.Length
                iNiveau = suffixe.iNiveau
                sFreqSuffixe = suffixe.sFrequence
                'sUnicite = suffixe.sUnicite ' Test
                lstUnicites = suffixe.lstUnicites ' Test
                'sUnicitePrincipale = suffixe.sUnicitePrincipale ' Test
                Exit For
            End If

            ' 21/06/2018 Désactivé : on veut distinguer -ide de -ïde
            'sSuffixe = suffixe.sSegmentUniforme
            '' sSuffixe <> "ide" AndAlso : ide est partout : ex.: acétamide : suffixe amide
            'If sMotDicoUniforme.EndsWith(sSuffixe) Then
            '    bSuffixe0 = True
            '    lstSuffixeDef = suffixe.lstDefinitions
            '    iLongSuffixe = sSuffixe.Length
            '    iNiveau = suffixe.iNiveau
            '    'Debug.WriteLine("Correction accent suffixe : " & sMotDico & " : " & _
            '    '    suffixe.sSegment & " -> " & suffixe.sSegmentUniforme)
            '    Exit For
            'End If

        Next

        Return bSuffixe0

    End Function

End Module
