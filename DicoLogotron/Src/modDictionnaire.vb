
Imports System.Text

Module modDictionnaire

    Const bDebugPrefixeEtSuffixePot As Boolean = False
    Const bAfficherDoublonMotsComplexes As Boolean = False

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

    Private Class clsPrm
        Public sMotDico$, sMotDicoUniforme$, sPrefixe$, sSuffixe$
        Public bPrefixe, bSuffixe, bPluriel As Boolean
        Public iLongSuffixe, iLongPrefixe, iLongMot,
            iNbMotsLogotronExistants, iNbMotsPrefixeExistants, iNbMotsSuffixeExistants,
            iNbMotsLogotronExistants1, iNbMotsLogotronExistants2 As Integer
        Public aPrefixes(), aSuffixes() As clsSegment
        Public lstSuffixeDef As New List(Of String)
        Public lst2PrefixeDef As New List(Of List(Of String))
        ' sbPrefixesEtSuffixes : mots logotroniques           du dico : préfixe(s)+ suffixe
        ' sbPrefixeEtSuffixe   : mots logotroniques simples   du dico : préfixe   + suffixe
        ' sbPrefixesEtSuffixe  : mots logotroniques complexes du dico : préfixes  + suffixe
        Public sbAutres, sbPrefixesEtSuffixes, sbPrefixeEtSuffixeBilan,
            sbPrefixeEtSuffixeFichierTxt, sbPrefixeEtSuffixeFichierCsv,
            sbMotSimpleFichierCode,
            sbPrefixesEtSuffixe, sbPrefixes, sbSuffixes As New StringBuilder
        Public dicoDefIncompletes As New DicoTri(Of String, clsLigne) ' 04/03/2018
        Public dicoPrefSuff As New DicoTri(Of String, clsSegmentStat)
        Public dicoPrefixesManquants As New DicoTri(Of String, clsSegmentStat)
        ' Compexité des mots logotroniques du dico : (niv préfixe + 1) x (niv suffixe + 1)
        Public dicoComplex As New DicoTri(Of String, clsMot)
        Public iNiveauPrefixe%, iNiveauSuffixe%
        Public sFreqPrefixe$, sFreqSuffixe$ ' 21/08/2018
        Public lst2UnicitesPrefixe As New List(Of List(Of String))
        Public lstUnicitesSuffixe As New List(Of String)
        Public iPasse% ' 26/08/2018
        Public hsUnicitesDefMotsSimples As New HashSet(Of String) ' 26/08/2018
        Public sMotTrouve$ ' 25/08/2018
        Public bMotTrouve As Boolean ' 25/08/2018
        Public bPremiereCombi As Boolean
        Public hsUnicitesMots As New HashSet(Of String)  ' 28/08/2018 Mots
        Public hsUnicitesMots1 As New HashSet(Of String) ' 28/08/2018 Mots simples
        Public hsUnicitesMots2 As New HashSet(Of String) ' 28/08/2018 Mots complexes
        Public hsUnicitesDoublons As New HashSet(Of String)  ' 29/08/2018 
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
        Public sLigne$, sCle$, sManque$, sMot$
        'Public iLong% ' iLong est utilisé dans le tri
        Public rRatio% ' rRatio est utilisé dans le tri
        Public Sub New(sLigne$, sCle$, sManque$, sMot$)
            Me.sLigne = sLigne
            Me.sCle = sCle
            Me.sManque = sManque
            Me.sMot = sMot
            'Me.iLong = sCle.Length
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
        'Public iLongSegment% ' Pour trier les segments par longueur décroissante
        Public bPrefixe As Boolean ' Sinon suffixe
        Public lstMots As New List(Of String)
        ' Si le mot est formé complètement par un préfixe et un suffixe, alors ils sont composables
        ' (on va les lister en 1er dans le bilan, car ils sont plus intéressants pour former des mots)
        Public bComposable As Boolean ' bComposable est utilisé dans le tri
        Public dicoSegManquant As DicoTri(Of String, clsSegmentManquant)
    End Class

#End Region

#Region "Initialisations"

    Public Sub InitBasesPot()
        m_prefixesPot = New clsBase(iNbColonnes:=0)
        m_suffixesPot = New clsBase(iNbColonnes:=0)
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
            Dim mot As clsSegment
            If dicoPrefixes.ContainsKey(sPrefixe) Then
                ' Il peut y avoir des doublons : liste de définitions possibles
                mot = dicoPrefixes(sPrefixe)
                mot.lstDefinitions.Add(sDefinition)
                If Not String.IsNullOrEmpty(prefixe.sUnicite) Then _
                    mot.lstUnicites.Add(prefixe.sUnicite) ' Test
            Else
                mot = New clsSegment
                mot.sSegment = sPrefixe
                mot.lstDefinitions.Add(sDefinition)
                mot.iLong = mot.sSegment.Length
                mot.iNiveau = Integer.Parse(sNiveau)
                mot.sFrequence = sFrequence
                mot.sSegmentUniforme = sEnleverAccents(sPrefixe.ToLower) ', bTexteUnicode:=True)
                'mot.sUnicite = prefixe.sUnicite ' Test
                If Not String.IsNullOrEmpty(prefixe.sUnicite) Then _
                    mot.lstUnicites.Add(prefixe.sUnicite) ' Test
                dicoPrefixes.Add(sPrefixe, mot)
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
            Dim mot As clsSegment
            If dicoPrefixes.ContainsKey(sPrefixe) Then
                ' Il peut y avoir des doublons : liste de définitions possibles
                mot = dicoPrefixes(sPrefixe)
                mot.lstDefinitions.Add(sDefinition)
            Else
                mot = New clsSegment
                mot.sSegment = sPrefixe
                mot.lstDefinitions.Add(sDefinition)
                mot.iLong = mot.sSegment.Length
                mot.sSegmentUniforme = sEnleverAccents(sPrefixe.ToLower) ', bTexteUnicode:=True)
                dicoPrefixes.Add(sPrefixe, mot)
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
            Dim mot As clsSegment
            If dicoSuffixes.ContainsKey(sSuffixe) Then
                ' Il peut y avoir des doublons : liste de définitions possibles
                'Debug.WriteLine("Suffixe à sens multiple : " & sPrefixe)
                mot = dicoSuffixes(sSuffixe)
                mot.lstDefinitions.Add(sDefinition)
                If Not String.IsNullOrEmpty(suffixe.sUnicite) Then _
                    mot.lstUnicites.Add(suffixe.sUnicite) ' Test
            Else
                mot = New clsSegment
                mot.sSegment = sSuffixe
                'mot.sDefinition = sDefinition
                mot.lstDefinitions.Add(sDefinition)
                mot.iLong = mot.sSegment.Length
                mot.iNiveau = Integer.Parse(sNiveau)
                mot.sFrequence = sFrequence
                mot.sSegmentUniforme = sEnleverAccents(sSuffixe.ToLower) ', bTexteUnicode:=True)
                'mot.sMotUniforme = sSuffixe.ToLower
                'mot.sUnicite = suffixe.sUnicite ' Test
                If Not String.IsNullOrEmpty(suffixe.sUnicite) Then _
                    mot.lstUnicites.Add(suffixe.sUnicite) ' Test
                dicoSuffixes.Add(sSuffixe, mot)
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
            Dim mot As clsSegment
            If dicoSuffixes.ContainsKey(sSuffixe) Then
                ' Il peut y avoir des doublons : liste de définitions possibles
                'Debug.WriteLine("Suffixe à sens multiple : " & sPrefixe)
                mot = dicoSuffixes(sSuffixe)
                'mot.lstDefinitions.Add(sDefinition)
            Else
                mot = New clsSegment
                mot.sSegment = sSuffixe
                'mot.sDefinition = sDefinition
                mot.lstDefinitions.Add(sDefinition) ' Définition vide : on veut un seul passage dans le for each def.
                mot.iLong = mot.sSegment.Length
                mot.sSegmentUniforme = sEnleverAccents(sSuffixe.ToLower) ', bTexteUnicode:=True)
                'mot.sMotUniforme = sSuffixe.ToLower
                dicoSuffixes.Add(sSuffixe, mot)
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
            "aplane", "décèle", "décentre", "déchire", "déchrome", "décline", "décolore",
            "décoque", "déculture", "défère", "déflore", "déforme", "dégrade", "déloque",
            "déparasite", "déprogramme", "désiste", "dévalent", "dévore", "décide", "dépare",
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
        If IsNothing(asLignes) Then Exit Sub
        Dim iNumLigne% = 0
        Dim iNbLignes% = asLignes.GetUpperBound(0)

        Dim sMemMot$ = ""
        For Each sMotDico As String In asLignes
            iNumLigne += 1

            If hsExclVerbesConj.Contains(sMotDico) Then Continue For

            prm.sMotDico = sMotDico
            prmPot.sMotDico = sMotDico
            'If Not sMotDico.StartsWith("st") Then Continue For
            'If sMotDico = "aéropathie" Then
            '    Debug.WriteLine("!")
            'End If
            If bDebug AndAlso iNumLigne > 10000 Then Exit For

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

            'Dim sMotDicoUniforme = sEnleverAccents(sMotDico.ToLower, bTexteUnicode:=False)
            prm.sMotDicoUniforme = sMotDico.ToLower
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

            ' Ne pas gérer la complexité pour les mots potentiels
            If sLang <> enumLangue.Fr Then Continue For

            Dim iLongPrefixePot% = 0
            Dim sPrefixePot = ""
            Dim lst2PrefixePotDef As New List(Of List(Of String))
            Dim lstPrefixePotDef As New List(Of String)
            Dim bPrefixePot As Boolean = False
            If Not bPrefixe0 AndAlso sLang = enumLangue.Fr Then
                bPrefixePot = bPrefixe(sMotDico, prmPot.aPrefixes,
                    sPrefixePot, lstPrefixePotDef, iLongPrefixePot)
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
                    AjouterSegment(prmPot.dicoPrefSuff, sPrefixePot, prmPot.sMotDicoUniforme, bPrefixe:=True)
                End If
            End If
            prmPot.sPrefixe = sPrefixePot
            prmPot.bPrefixe = bPrefixePot
            prmPot.iLongPrefixe = iLongPrefixePot
            prmPot.lst2PrefixeDef = lst2PrefixePotDef

            Dim iLongSuffixePot% = 0
            Dim sSuffixePot = ""
            Dim lstSuffixePotDef As New List(Of String)
            Dim bSuffixePot As Boolean = False
            If Not bSuffixe0 AndAlso sLang = enumLangue.Fr Then
                bSuffixePot = bSuffixe(sMotDico, prmPot.aSuffixes,
                    sSuffixePot, lstSuffixePotDef, iLongSuffixePot) ' sMotDicoUniforme
                If bSuffixePot Then
                    ' Idée : faire 2 dico distincts (mais pas grave : qu'un seul élément préfixe et suffixe : folia)
                    If prmPot.dicoPrefSuff.ContainsKey(sSuffixePot) Then
                        Dim seg = prmPot.dicoPrefSuff(sSuffixePot)
                        If seg.bPrefixe AndAlso bDebugPrefixeEtSuffixePot Then
                            Debug.WriteLine("A la fois préfixe et suffixe potentiel : " & sSuffixePot)
                            GoTo Suite2
                        End If
                    End If
                    AjouterSegment(prmPot.dicoPrefSuff, sSuffixePot, prmPot.sMotDicoUniforme, bPrefixe:=False)
                End If
            End If
            prmPot.sSuffixe = sSuffixePot
            prmPot.bSuffixe = bSuffixePot
            prmPot.iLongSuffixe = iLongSuffixePot
            prmPot.lstSuffixeDef = lstSuffixePotDef
Suite2:
            TraiterMot(prmPot, bPotentiel:=True)

        Next

        Dim sbBilan As New StringBuilder
        sbBilan.AppendLine("Bilan du dictionnaire français (" & iNbLignes & " mots dérivés et conjugués) :")
        sbBilan.AppendLine("------------------------------")

        Bilan(prm, sbBilan, iNbLignes, dicoPrefixes, dicoSuffixes, bMotsAvecPrefixe:=False,
            bMotsAvecSuffixe:=False, bNonTrouves:=True, bDefIncomplete:=True, bPotentiel:=False)

        If sLang = enumLangue.Fr Then
            sbBilan.AppendLine()
            sbBilan.AppendLine()
            sbBilan.AppendLine("Bilan du dictionnaire français avec les segments potentiels :")
            sbBilan.AppendLine("-----------------------------------------------------------")
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
            Dim iNiveauP% = 0
            Dim iLongPrefixe% = 0
            Dim sPrefixe = ""
            Dim sFreqPrefixe = ""
            Dim lst2PrefixeDef As New List(Of List(Of String))
            Dim lstPrefixeDef As New List(Of String)
            bPrefixe0 = bPrefixe(prm.sMotDico, prm.aPrefixes,
                sPrefixe, lstPrefixeDef, iLongPrefixe, iNiveauP, lstUnicitesP,
                sFreqPrefixe, aiLongPrefixeMax)
            If bPrefixe0 Then
                If iLongPrefixe < aiLongPrefixeMinTeste(iPasse) Then aiLongPrefixeMinTeste(iPasse) = iLongPrefixe
                If lstUnicitesP.Count > 0 Then lst2UnicitesP.Add(lstUnicitesP)
                lst2PrefixeDef.Add(lstPrefixeDef)
                AjouterSegment(prm.dicoPrefSuff, sPrefixe, prm.sMotDicoUniforme, bPrefixe:=True)
                If iLongPrefixe = prm.iLongMot Then
                    For Each sPrefixeDef In lstPrefixeDef
                        Dim sDef$ = sPrefixeDef.ToUpper
                        Dim sCle$ = prm.sMotDico & " : " & sDef
                        If prm.hsUnicitesDefMotsSimples.Contains(sCle) Then Continue For
                        prm.hsUnicitesDefMotsSimples.Add(sCle)
                        If Not prm.hsUnicitesMots.Contains(prm.sMotDico) Then
                            prm.hsUnicitesMots.Add(prm.sMotDico)
                            prm.iNbMotsLogotronExistants += 1
                        End If
                        prm.bMotTrouve = True
                        prm.sbPrefixesEtSuffixes.AppendLine(prm.sMotDico & " : " &
                            sDef & " : " & sPrefixe & "-")
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
            prm.iNiveauPrefixe = iNiveauP
            prm.sFreqPrefixe = sFreqPrefixe
            prm.lst2UnicitesPrefixe = lst2UnicitesP

            Dim lstUnicitesS As New List(Of String)
            Dim sSuffixe = ""
            Dim lstSuffixeDef = New List(Of String)
            Dim iNiveauS% = 0
            Dim sFreqSuffixe$ = ""
            Dim iLongSuffixe% = 0
            bSuffixe0 = bSuffixe(prm.sMotDico, prm.aSuffixes,
                sSuffixe, lstSuffixeDef, iLongSuffixe, iNiveauS, lstUnicitesS,
                sFreqSuffixe, iLongSuffixeMax) 'sMotDicoUniforme, 
            If bSuffixe0 Then
                If iLongSuffixe < iLongSuffixeMinTeste(iPasse) Then iLongSuffixeMinTeste(iPasse) = iLongSuffixe
                AjouterSegment(prm.dicoPrefSuff, sSuffixe, prm.sMotDicoUniforme, bPrefixe:=False)
                If iLongSuffixe = prm.iLongMot Then
                    For Each sSuffixeDef In lstSuffixeDef
                        Dim sDef$ = sSuffixeDef.ToUpper
                        Dim sCle$ = prm.sMotDico & " : " & sDef
                        If prm.hsUnicitesDefMotsSimples.Contains(sCle) Then Continue For
                        prm.hsUnicitesDefMotsSimples.Add(sCle)
                        If Not prm.hsUnicitesMots.Contains(prm.sMotDico) Then
                            prm.hsUnicitesMots.Add(prm.sMotDico)
                            prm.iNbMotsLogotronExistants += 1
                        End If
                        prm.bMotTrouve = True
                        prm.sbPrefixesEtSuffixes.AppendLine(prm.sMotDico & " : " &
                            sDef & " : -" & sSuffixe)
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
            prm.iNiveauSuffixe = iNiveauS
            prm.sFreqSuffixe = sFreqSuffixe
            prm.lstUnicitesSuffixe = lstUnicitesS

            'If sMotDico = "astroïde" Then
            '    Debug.WriteLine("!")
            'End If

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
            ' ToDo : à revoir ratio

            Dim sbdi As New StringBuilder
            Dim aDefInc = prm.dicoDefIncompletes.Trier("rRatio Desc, sLigne")
            For Each ligne In aDefInc
                ' Ne pas lister les lignes si le mot a finalement été trouvé 29/08/2018
                If prm.hsUnicitesMots.Contains(ligne.sMot) Then Continue For
                sbdi.AppendLine(ligne.sLigne)
            Next
            sbBilan.Append(sbdi)

        End If

        Dim sb As New StringBuilder
        sb.AppendLine("Mots" & sPot & " avec préfixe(s) et suffixe :")
        sb.AppendLine("-------------------------------" & sTiret)
        sb.Append(prm.sbPrefixesEtSuffixes)
        Dim rPCMotExistTot2! = prm.iNbMotsLogotronExistants / iNbMots
        sb.AppendLine("Nb. total d'occurrences = " &
            prm.iNbMotsLogotronExistants & " / " & iNbMots & " = " & rPCMotExistTot2.ToString(sFormatPC2))
        If Not bPotentiel Then
            Dim sCheminBilan$ = Application.StartupPath & "\Doc\Mots" & sLang & ".txt"
            bEcrireFichier(sCheminBilan, sb)
        End If
        sbBilan.AppendLine("")
        sbBilan.Append(sb)

        sb = New StringBuilder
        sb.AppendLine("Mots" & sPot & " avec des préfixes multiples et un suffixe :")
        sb.AppendLine("----------------------------------------------" & sTiret)
        sb.Append(prm.sbPrefixesEtSuffixe)
        Dim rPCMotExistTot! = prm.iNbMotsLogotronExistants2 / iNbMots
        sb.AppendLine("Nb. total d'occurrences = " &
            prm.iNbMotsLogotronExistants2 & " / " & iNbMots & " = " & rPCMotExistTot.ToString(sFormatPC2))
        If Not bPotentiel Then
            Dim sCheminBilan$ = Application.StartupPath & "\Doc\MotsComplexes" & sLang & ".txt"
            bEcrireFichier(sCheminBilan, sb)
        End If
        sbBilan.AppendLine("")
        sbBilan.Append(sb)

        Dim sLigneTitre$ = "Mots" & sPot & " avec préfixe et suffixe :"
        Dim rPCMotExistTot1! = prm.iNbMotsLogotronExistants1 / iNbMots
        Dim sLigneBilan$ = "Nb. total d'occurrences = " &
            prm.iNbMotsLogotronExistants1 & " / " & iNbMots & " = " & rPCMotExistTot1.ToString(sFormatPC2)
        Dim sLigneLgd$ = "(mot : DÉFINITION : préfixe - suffixe : niveau préfixe niveau suffixe)"
        Dim sLigneTiret$ = "----------------------------" & sTiret
        sb = New StringBuilder
        sb.AppendLine(sLigneTitre)
        If Not bPotentiel Then sb.AppendLine(sLigneLgd)
        sb.AppendLine(sLigneTiret)
        sb.Append(prm.sbPrefixeEtSuffixeBilan)
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
                prm.iNbMotsLogotronExistants & " / " & iNbMots & " = " & rPCMotExistTot2.ToString(sFormatPC2))
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
            'segM.sPrefixePreced = sPrefixePreced
            'segM.sSuffixeSuiv = sSuffixeSuiv
            segM.lstMotsDico.Add(sMotDicoUniforme)
            psStat.dicoSegManquant.Add(sCle, segM)
        End If

    End Sub

    Private Sub TraiterMot(prm As clsPrm, bPotentiel As Boolean,
        Optional ByRef bHypotheseTropLongue As Boolean = False)

        prm.bMotTrouve = False
        prm.sMotTrouve = ""
        bHypotheseTropLongue = False

        If prm.bPrefixe AndAlso prm.bSuffixe Then

            If prm.iLongSuffixe + prm.iLongPrefixe < prm.iLongMot Then

                Dim iLongDelta% = prm.iLongMot - (prm.iLongSuffixe + prm.iLongPrefixe)
                Dim sDelta$ = prm.sMotDico.Substring(prm.iLongPrefixe, iLongDelta)
                Dim sPrefixePot$ = sDelta
                Dim sPrefixeSuiv$ = prm.sPrefixe & " - " & sDelta
                Dim sPrefixeComplexPreced$ = prm.sPrefixe & "(" & prm.iNiveauPrefixe & ")"
                Dim sPrefixeComplexSuiv$ = sPrefixeComplexPreced & " - " & sDelta
                Dim iComplexiteSuiv% = prm.iNiveauPrefixe + 1
                TraiterMotRecursif(prm, prm.sPrefixe, prm.iLongPrefixe, sPrefixePot, prm.sPrefixe,
                    iComplexiteSuiv, sPrefixeSuiv, iComplexiteSuiv, bPotentiel,
                    sPrefixeComplexSuiv, sPrefixeComplexPreced, 2, prm.sPrefixe)

            ElseIf prm.iLongSuffixe + prm.iLongPrefixe > prm.iLongMot Then
                If prm.bPluriel Then Exit Sub

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
                For Each sSuffixeDef0 In prm.lstSuffixeDef
                    For Each sPrefixeDef0 In prm.lst2PrefixeDef
                        For Each sPrefixeDef1 In sPrefixeDef0
                            Dim sCle$ = prm.sPrefixe & " - " & prm.sSuffixe & " : " &
                                prm.iLongSuffixe + prm.iLongPrefixe & " > " & prm.iLongMot & " : " &
                                sDelta & " en trop"
                            Dim sLigne$ = prm.sMotDico & " : " &
                                sSuffixeDef0.ToUpper & sSepDef &
                                sCompleterPrefixe(sPrefixeDef1.ToUpper) & " : " & sCle
                            If Not prm.dicoDefIncompletes.ContainsKey(sLigne) Then _
                                prm.dicoDefIncompletes.Add(sLigne,
                                    New clsLigne(sLigne, sCle, sDelta, prm.sMotDico))
                        Next
                    Next
                Next

            Else ' iLongSuffixe + iLongPrefixe = iLongMot

                If prm.bPluriel Then Exit Sub

                Dim iNbOcc% = 0
                Dim iNumS% = 0
                Dim sMemDef$ = ""
                For Each sSuffixeDef0 In prm.lstSuffixeDef
                    iNumS += 1
                    Dim iNumP1% = 0
                    For Each lstPrefixeDef In prm.lst2PrefixeDef
                        iNumP1 += 1
                        Dim iNumP2% = 0
                        For Each sPrefixeDef0 In lstPrefixeDef
                            iNumP2 += 1

                            'If prm.sMotDico = "ontogenèse" Then
                            '    Debug.WriteLine("!")
                            'End If

                            Dim sDef$ = sSuffixeDef0.ToUpper & sSepDef & sCompleterPrefixe(sPrefixeDef0.ToUpper)
                            Dim sClePrefixe$ = "" 'prm.sPrefixe
                            Dim sCleSuffixe$ = "" 'prm.sSuffixe
                            Dim sLigne$ = prm.sMotDico & " : " &
                                sDef & " : " & prm.sPrefixe & " - " & prm.sSuffixe

                            Dim sDefPot$ = prm.sMotDico & ";" & sDef

                            ' 31/08/2018
                            Dim sCleExcl$ = "-" & prm.sSuffixe
                            If m_defFls.bSensExclusifAutre(sCleExcl, prm.sMotDico, sSuffixeDef0) Then Continue For

                            sCleExcl = prm.sPrefixe & "-"
                            If m_defFls.bSensExclusifAutre(sCleExcl, prm.sMotDico, sPrefixeDef0) Then Continue For

                            If m_hsExclDef.Contains(sDefPot) Then
                                ' Mémoriser les déf. fausses utiles
                                If Not m_hsExclDefUtil.Contains(sDefPot) Then _
                                    m_hsExclDefUtil.Add(sDefPot)
                                Continue For
                            End If

                            Dim sCle$ = prm.sMotDico & " : " & sDef
                            If prm.hsUnicitesDefMotsSimples.Contains(sCle) Then Continue For
                            prm.hsUnicitesDefMotsSimples.Add(sCle)
                            If Not prm.hsUnicitesMots.Contains(prm.sMotDico) Then
                                prm.hsUnicitesMots.Add(prm.sMotDico)
                                prm.iNbMotsLogotronExistants += 1
                            End If
                            If Not prm.hsUnicitesMots1.Contains(prm.sMotDico) Then
                                prm.hsUnicitesMots1.Add(prm.sMotDico)
                                prm.iNbMotsLogotronExistants1 += 1
                            ElseIf Not bPotentiel Then
                                ' 29/08/2018 Double sens généralisé : pas seulement def. multiple d'un suffixe
                                If Not m_bAfficherAvertDoublon Then
                                    Dim sMsg$ = "Double sens : compléter le fichier Doc\DefinitionsFausses.txt :"
                                    m_msgDelegue.AfficherMsg(sMsg)
                                    Debug.WriteLine(sMsg)
                                    m_bAfficherAvertDoublon = True
                                End If
                                If Not prm.hsUnicitesDoublons.Contains(sDefPot) Then
                                    prm.hsUnicitesDoublons.Add(sDefPot)
                                    m_msgDelegue.AfficherMsg(sDefPot)
                                    Debug.WriteLine(sDefPot)
                                End If
                            End If
                            prm.bMotTrouve = True
                            prm.sMotTrouve = prm.sMotDico & " : " & sDef ' Debug
                            Dim sLigneFichier$ = sDefPot & ";" & prm.sPrefixe & ";" & prm.sSuffixe
                            prm.sbPrefixesEtSuffixes.AppendLine(sLigne)
                            Dim sLigneMotSimple$ = sLigne
                            Dim sLigneMotSimpleFichierTxt$ = sLigneFichier
                            Dim sLigneMotSimpleFichierCsv$ = sLigneFichier
                            Dim sLigneMotSimpleFichierCode$ = ""

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
                                        m_msgDelegue.AfficherMsg(sDefPot)
                                        Debug.WriteLine(sMemDef)
                                        Debug.WriteLine(sDefPot)
                                    End If
                                Else
                                    sMemDef = sDefPot
                                End If

                                sLigneMotSimple = sLigne & " : " & prm.iNiveauPrefixe & " " & prm.iNiveauSuffixe
                                prm.sbPrefixeEtSuffixeBilan.AppendLine(sLigneMotSimple)

                                'If prm.lstUnicitesPrefixe.Count >= iNum
                                'If Not String.IsNullOrEmpty(prm.sUnicitePrefixe) AndAlso _
                                '   prm.sUnicitePrefixe <> prm.sPrefixe Then
                                '    sLigneMotSimple &= " (unicité Pfx.:" & prm.sUnicitePrefixe & ")"
                                'End If
                                Const bDebugMS As Boolean = False
                                If prm.lst2UnicitesPrefixe.Count >= iNumP1 Then
                                    Dim lstUnicitesP1 = prm.lst2UnicitesPrefixe(iNumP1 - 1)
                                    If lstUnicitesP1.Count >= iNumP2 Then
                                        Dim sUniciteP = lstUnicitesP1(iNumP2 - 1)
                                        If Not String.IsNullOrEmpty(sUniciteP) Then
                                            ' 17/03/2018 Ici il faut qd même expliciter
                                            ' AndAlso sUniciteP <> prm.sPrefixe 
                                            If bDebugMS Then _
                                                sLigneMotSimple &= " (unicité Pfx.:" & sUniciteP & ")"
                                            sClePrefixe = sUniciteP
                                        End If
                                    End If
                                End If
                                If prm.lstUnicitesSuffixe.Count >= iNumS Then
                                    Dim sUniciteS = prm.lstUnicitesSuffixe(iNumS - 1)
                                    If Not String.IsNullOrEmpty(sUniciteS) Then
                                        ' 17/03/2018 Ici il faut qd même expliciter
                                        ' AndAlso sUniciteS <> prm.sSuffixe 
                                        If bDebugMS Then _
                                            sLigneMotSimple &= " (unicité Sfx.:" & sUniciteS & ")"
                                        sCleSuffixe = sUniciteS
                                    End If
                                End If
                                sLigneMotSimpleFichierTxt = sLigneFichier & ";" &
                                    prm.iNiveauPrefixe & ";" & prm.iNiveauSuffixe
                                sLigneMotSimpleFichierCsv = sLigneMotSimpleFichierTxt & ";" &
                                    sClePrefixe & ";" & sCleSuffixe & ";" &
                                    prm.sFreqPrefixe & ";" & prm.sFreqSuffixe
                                sLigneMotSimpleFichierCode = "        " &
                                    sGm & prm.sMotDico & sGm & ", " &
                                    sGm & sDef & sGm & ", " &
                                    sGm & prm.sPrefixe & sGm & ", " &
                                    sGm & prm.sSuffixe & sGm & ", " &
                                    sGm & prm.iNiveauPrefixe & sGm & ", " &
                                    sGm & prm.iNiveauSuffixe & sGm & ", " &
                                    sGm & sClePrefixe & sGm & ", " &
                                    sGm & sCleSuffixe & sGm & ", " &
                                    sGm & prm.sFreqPrefixe & sGm & ", " &
                                    sGm & prm.sFreqSuffixe & sGm & ","
                            End If
                            If bAjout Then
                                prm.sbPrefixeEtSuffixeFichierTxt.AppendLine(sLigneMotSimpleFichierTxt)
                                prm.sbPrefixeEtSuffixeFichierCsv.AppendLine(sLigneMotSimpleFichierCsv)
                                prm.sbMotSimpleFichierCode.AppendLine("        " & sLigneMotSimpleFichierCode)
                            End If

                            If Not bPotentiel Then
                                Dim iComplex% = (prm.iNiveauPrefixe + 1) * (prm.iNiveauSuffixe + 1)
                                If Not prm.dicoComplex.ContainsKey(prm.sMotDico) Then _
                            prm.dicoComplex.Add(prm.sMotDico,
                                New clsMot(prm.sMotDico, iComplex,
                                    prm.sPrefixe & "(" & prm.iNiveauPrefixe & ") - " &
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

            End If

        ElseIf prm.bPrefixe Then
            If prm.bPluriel Then Exit Sub
            If prm.bPremiereCombi AndAlso prm.iPasse = 0 Then prm.iNbMotsPrefixeExistants += 1
            For Each sPrefixeDef0 In prm.lst2PrefixeDef
                For Each sPrefixeDef1 In sPrefixeDef0
                    prm.sbPrefixes.AppendLine(prm.sMotDico & " : " & prm.sPrefixe & "- : " & sPrefixeDef1)
                Next
            Next
        ElseIf prm.bSuffixe Then
            If prm.bPluriel Then Exit Sub
            If prm.bPremiereCombi AndAlso prm.iPasse = 0 Then prm.iNbMotsSuffixeExistants += 1
            For Each sSuffixeDef0 In prm.lstSuffixeDef
                prm.sbSuffixes.AppendLine(prm.sMotDico & " : -" & prm.sSuffixe & " : " & sSuffixeDef0)
            Next
        Else
            If prm.bPluriel Then Exit Sub
            prm.sbAutres.AppendLine(prm.sMotDico)
        End If

    End Sub

    Private Sub TraiterMotRecursif(prm As clsPrm, sPrefixesPreced$,
        iLongPrefixesPreced%, sPrefixePot$, sPrefixePreced$, iComplexitePreced%,
        ByRef sPrefixeSuiv$, ByRef iComplexiteSuiv%, bPotentiel As Boolean,
        sPrefixeComplexSuiv$, sPrefixesComplexPreced$, iNivGlobal%, sPrefixePrecedBase$)

        Dim sDelta$ = sPrefixePot

        Dim lst2UnicitesP As New List(Of List(Of String))
        Dim lstUnicitesP2 As New List(Of String)
        Dim sMotDico2 = sDelta
        Dim sMotDicoUniforme2 = sDelta.ToLower
        Dim iLongPrefixe2% = 0
        Dim iNiveauP2% = 0
        Dim sPrefixe2 = ""
        Dim lstPrefixeDef2 As New List(Of String)
        Dim bPrefixe2 = bPrefixe(sMotDico2, prm.aPrefixes,
            sPrefixe2, lstPrefixeDef2, iLongPrefixe2%, iNiveauP2%, lstUnicitesP2)
        If bPrefixe2 Then
            prm.lst2PrefixeDef.Add(lstPrefixeDef2) ' ToDo : SSi mot retenu ?
            If lstUnicitesP2.Count > 0 Then prm.lst2UnicitesPrefixe.Add(lstUnicitesP2)
            AjouterSegment(prm.dicoPrefSuff, sPrefixe2, sMotDicoUniforme2, bPrefixe:=True)
        End If

        If Not bPrefixe2 Then
            If prm.bPluriel Then Exit Sub
            For Each sSuffixeDef0 In prm.lstSuffixeDef
                For Each lstPrefixesDef0 In prm.lst2PrefixeDef
                    For Each sPrefixeDef1 In lstPrefixesDef0
                        Dim sCle$ = sPrefixesPreced & " - " & prm.sSuffixe & " : " &
                            prm.iLongSuffixe + iLongPrefixesPreced & " < " & prm.iLongMot &
                            " : manque " & sDelta
                        Dim sLigne$ = prm.sMotDico & " : " &
                            sSuffixeDef0.ToUpper & sSepDef &
                            sCompleterPrefixe(sPrefixeDef1.ToUpper) & " : " & sCle
                        If Not prm.dicoDefIncompletes.ContainsKey(sLigne) Then _
                            prm.dicoDefIncompletes.Add(sLigne,
                                New clsLigne(sLigne, sCle, sDelta, prm.sMotDico))
                        Dim sMotDicoUniforme = prm.sMotDico.ToLower
                        AjouterSegment(prm.dicoPrefixesManquants, sDelta, sMotDicoUniforme, bPrefixe:=True,
                            sPrefixePreced:=sPrefixePreced, sSuffixeSuiv:=prm.sSuffixe)
                    Next
                Next
            Next
        Else

            Dim iLongTot% = prm.iLongSuffixe + iLongPrefixesPreced + iLongPrefixe2
            If iLongTot < prm.iLongMot Then

                ' Appel récurrsif
                Dim sPrefixesPreced2$ = sPrefixesPreced & sPrefixe2
                Dim iLongPrefixesPreced2% = sPrefixesPreced2.Length
                Dim iLongDelta2% = prm.iLongMot - (prm.iLongSuffixe + iLongPrefixesPreced2)
                Dim sDelta2$ = prm.sMotDico.Substring(iLongPrefixesPreced2, iLongDelta2)
                Dim sPrefixePot2$ = sDelta2

                sPrefixeSuiv = sPrefixePrecedBase & " - " & sPrefixe2 & " - " & sPrefixePot2 ' 22/06/2018
                sPrefixePrecedBase = sPrefixePrecedBase & " - " & sPrefixe2 ' 22/06/2018

                sPrefixeComplexSuiv = sPrefixesComplexPreced & " - " &
                    sPrefixe2 & "(" & iNiveauP2 & ") - " & sPrefixePot2
                Dim iComplexiteSuiv2% = iComplexitePreced * (iNiveauP2 + 1)
                TraiterMotRecursif(prm, sPrefixesPreced2, iLongPrefixesPreced2, sPrefixePot2, sPrefixe2,
                     iComplexitePreced, sPrefixeSuiv, iComplexiteSuiv2, bPotentiel,
                     sPrefixeComplexSuiv, sPrefixesComplexPreced, iNivGlobal + 1, sPrefixePrecedBase)

            ElseIf iLongTot > prm.iLongMot Then
                If prm.bPluriel Then Exit Sub
                ' On ne passe plus ici
                Stop
                Dim iLongDelta2% = prm.iLongMot - prm.iLongSuffixe
                Dim sDelta2$ = prm.sMotDico.Substring(prm.iLongMot - prm.iLongSuffixe, iLongDelta2)
                For Each sSuffixeDef0 In prm.lstSuffixeDef
                    For Each lstPrefixesDef0 In prm.lst2PrefixeDef
                        For Each sPrefixeDef1 In lstPrefixesDef0
                            Dim sCle$ = sPrefixesPreced & " - " & prm.sSuffixe & " : " &
                                prm.iLongSuffixe + iLongPrefixesPreced & " > " & prm.iLongMot & " : " &
                                sDelta2 & " en trop"
                            Dim sLigne$ = prm.sMotDico & " : " &
                                sSuffixeDef0.ToUpper & sSepDef &
                                sCompleterPrefixe(sPrefixeDef1.ToUpper) & " : " & sCle
                            If Not prm.dicoDefIncompletes.ContainsKey(sLigne) Then _
                                prm.dicoDefIncompletes.Add(sLigne,
                                    New clsLigne(sLigne, sCle, sDelta, prm.sMotDico))
                            ' 04/03/2018 ---
                            Dim sMotDicoUniforme = prm.sMotDico.ToLower
                            AjouterSegment(prm.dicoPrefixesManquants, sDelta2, sMotDicoUniforme, bPrefixe:=True,
                                sPrefixePreced:=sPrefixePreced, sSuffixeSuiv:=prm.sSuffixe)
                            ' --------------
                        Next
                    Next
                Next
            Else ' If iLongTot = iLongMot Then
                If prm.bPluriel Then Exit Sub
                Dim iComplexiteSuiv2% = iComplexiteSuiv * (iNiveauP2 + 1)
                For Each sSuffixeDef0 In prm.lstSuffixeDef
                    Dim sbPrefixes As New StringBuilder
                    DevelopperDef(prm, 0, sSuffixeDef0, sPrefixeSuiv, iComplexiteSuiv2,
                        sbPrefixes, bPotentiel, iNiveauP2, sPrefixeComplexSuiv, iNivGlobal)
                Next
            End If
        End If

    End Sub

    Private Sub DevelopperDef(prm As clsPrm, iNiv%, sSuffixeDef0$,
        sPrefixeSuiv$, iComplexiteSuiv%, ByRef sbPrefixes As StringBuilder,
        bPotentiel As Boolean, iComplexiteDern%, sPrefixeComplexSuiv$, iNivGlobal%)

        Dim iNivMax% = prm.lst2PrefixeDef.Count
        Dim sbMemPrefixes As New StringBuilder(sbPrefixes.ToString)
        For Each sPrefixesDef0 In prm.lst2PrefixeDef(iNiv) ' Là il faut dédoubler les lignes

            Dim sCompleterPref$ = sCompleterPrefixe(sPrefixesDef0.ToUpper)
            If sbPrefixes.Length > 0 Then sbPrefixes.Append(" ")
            sbPrefixes.Append(sCompleterPref)

            If iNiv < iNivMax - 1 Then
                ' Appel récursif
                DevelopperDef(prm, iNiv + 1, sSuffixeDef0, sPrefixeSuiv, iComplexiteSuiv,
                    sbPrefixes, bPotentiel, iComplexiteDern, sPrefixeComplexSuiv, iNivGlobal)
            ElseIf iNiv = iNivMax - 1 Then

                ' 31/08/2018
                Dim sCleExcl$ = "-" & prm.sSuffixe
                If m_defFls.bSensExclusifAutre(sCleExcl, prm.sMotDico, sSuffixeDef0) Then Continue For

                sCleExcl = prm.sPrefixe & "-"
                If m_defFls.bSensExclusifAutre(sCleExcl, prm.sMotDico, sPrefixesDef0) Then Continue For

                Dim sDef$ = sSuffixeDef0.ToUpper & sSepDef & sbPrefixes.ToString
                Dim sDefPot$ = prm.sMotDico & ";" & sDef
                Dim sCle$ = prm.sMotDico & " : " & sDef
                If Not prm.hsUnicitesDefMotsSimples.Contains(sCle) Then
                    prm.hsUnicitesDefMotsSimples.Add(sCle)

                    If Not prm.hsUnicitesMots.Contains(prm.sMotDico) Then
                        prm.hsUnicitesMots.Add(prm.sMotDico)
                        prm.iNbMotsLogotronExistants += 1
                    End If
                    If Not prm.hsUnicitesMots2.Contains(prm.sMotDico) Then
                        prm.hsUnicitesMots2.Add(prm.sMotDico)
                        prm.iNbMotsLogotronExistants2 += 1
                        'Debug.WriteLine(prm.iNbMotsLogotronExistants2 & " : " & prm.sMotDico)
                    ElseIf Not bPotentiel AndAlso bAfficherDoublonMotsComplexes Then
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
                            m_msgDelegue.AfficherMsg(sDefPot)
                            Debug.WriteLine(sDefPot)
                        End If
                    End If

                    prm.bMotTrouve = True
                    prm.sMotTrouve = prm.sMotDico & " : " & sDef
                    Dim sLigne$ = prm.sMotDico & " : " & sDef & " : " &
                        sPrefixeSuiv & " - " & prm.sSuffixe
                    prm.sbPrefixesEtSuffixes.AppendLine(sLigne)
                    prm.sbPrefixesEtSuffixe.AppendLine(sLigne)
                    If Not bPotentiel Then
                        Dim iComplex% = iComplexiteSuiv * (prm.iNiveauSuffixe + 1)
                        If Not prm.dicoComplex.ContainsKey(prm.sMotDico) Then _
                            prm.dicoComplex.Add(prm.sMotDico,
                                New clsMot(prm.sMotDico, iComplex,
                                    sPrefixeComplexSuiv & "(" & iComplexiteDern & ") - " &
                                    prm.sSuffixe & "(" & prm.iNiveauSuffixe & ")", sDef, iNivGlobal + 1))
                    End If
                End If
            End If
            sbPrefixes = New StringBuilder(sbMemPrefixes.ToString) ' 28/08/2017

        Next

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
        ByRef sPrefixe$, ByRef sPrefixeDef As List(Of String), ByRef iLongPrefixe%,
        Optional ByRef iNiveau% = 0,
        Optional ByRef lstUnicites As List(Of String) = Nothing,
        Optional ByRef sFreqPrefixe$ = "",
        Optional iLongPrefixeMax% = 0) As Boolean

        Dim bPrefixe0 = False
        sPrefixe = ""
        sPrefixeDef = New List(Of String)
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

                bPrefixe0 = True
                sPrefixeDef = prefixe.lstDefinitions
                iLongPrefixe = sPrefixe.Length
                iNiveau = prefixe.iNiveau
                sFreqPrefixe = prefixe.sFrequence
                'sUnicite = prefixe.sUnicite ' Test
                lstUnicites = prefixe.lstUnicites ' Test
                'If sPrefixe = "semio" Then
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
        Optional ByRef sFreqSuffixe$ = "",
        Optional iLongSuffixeMax% = 0) As Boolean 'sMotDicoUniforme$, 

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
