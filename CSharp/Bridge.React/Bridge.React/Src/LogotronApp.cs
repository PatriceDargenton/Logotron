
using System;
using System.Collections.Generic;

using Bridge.React.Logotron.API;
using Bridge.React.Logotron.Components;
using Bridge.React.Logotron.ViewModels;

using ProductiveRage.Immutable;
using ProductiveRage.NonBlankTrimmedString;

using Bridge.React.Examples; // ToDoApp

using Util;
using LogotronLib;
using LogotronLib.Src;

using Logotron.Src.Util; // clsMsgDelegue

namespace Bridge.React.Logotron
{
	public sealed class LogotronApp : Component<LogotronApp.Props, LogotronApp.State>
	{
        private const string sAppliDef = "Logotron"; //"Tuto. B.R.";

        // TaskCard.Props dérive de PropsWithKey, la clé est simplement un int
        //  incrémenté dans le AppendTodo, et l'id est passé ici : base(task.Id)
        private int _nextAvailableIdToDoApp = 0;
        private uint _nextAvailableIdLogotron = 0;
        private uint _nextAvailableIdTuto = 0;
        private int _nextAvailableIdSTB = 0;

        private const int iIdTIBTitre = 100;
        private const int iIdTIBContenu = 101;
        private const string sHistorique = "Historique";
        private const int iMargin = 4;
        private const int iPadding = 4;
        private const int iFontSize = 18;

        private clsMsgDelegue m_msgDelegue;

        private static readonly List<string> lstItemsNiv = new List<string>
            { "1", "2", "3" };
        private static readonly List<string> lstItemsNbPrefixes = new List<string>
            { "H", "1", "2", "3", "4", "5" };

        public sealed class Quiz
        {
            internal uint nextAvailableIdQuiz = 0;

            internal int iNumQuestionQuiz = 0;
            internal bool bQuizEnCours = false;
            internal bool bQuestionEnCours = false;
            internal bool bDernQuestion = false;
            internal bool bQuizInverse = false;
            internal string sMotEnCours = "";
            internal string sQuizPrefixeOk = "";
            internal string sQuizSuffixeOk = "";
            internal string sDetail = "";
            internal int iScoreTot = 0;
            internal List<string> lstEtym;

            // Coefficient de bonus lorsque le préfixe et le suffixe sont justes
            internal const int iCoefBonus = 3;

            internal readonly List<string> lstItemsNbQ = new List<string>
            { "5", "10", "20", "30", "40", "50" };
            internal readonly List<string> lstItemsDiff = new List<string>
            { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        }
        private static Quiz _quiz = new Quiz();

        #region Initialisations

        public sealed class Props
        {
            public string Label = "Logotron - V" + 
                clsConstBR.sVersionAppli + " (" + clsConstBR.sDateAppli + ")" + 
                ((clsConst.bDebug) ? " - Debug" : "") ;
            public IReadAndWriteMessages MessageApi;
        }

        public LogotronApp(LogotronApp.Props props) : base(props) { }

        private void MsgBox(object sender, clsMsgEventArgs e)
        {
            Bridge.Html5.Window.Alert(e.sMessage);
        }

        public sealed class State : IAmImmutable
        {
            public State(
                string inputValue,
                string inputValueSTB,
                NonNullList<TaskDetails> todos,
                CheckBoxListMultipleEditState niveau,
                //CheckBoxListSimpleEditState niveauSimple,
                CheckBoxListSimpleEditState nbPrefixes,
                CheckBoxListSimpleEditState nbQuestions,
                CheckBoxListSimpleEditState difficulte,
                CheckBoxEditState motsExistants,
                CheckBoxEditState quizInverse,
                CheckBoxEditState grecoLatin,
                CheckBoxEditState quizGrecoLatin,
                CheckBoxEditState neologisme,
                CheckBoxEditState quizNeologisme,
                CheckBoxEditState infoBulle,
                string appli,

                // Ancienne démo Bridge.React màj :
                TextInputDetails titre,
                TextInputDetails contenu,
                NonNullList<SavedMessageDetails> messageHistoryLogotron,
                NonNullList<SavedMessageDetails> messageHistoryQuiz,
                NonNullList<SavedMessageDetails> messageHistoryTuto,

                NonNullList<MgsDetails> lstMsg, // Démo ScrollTextBox
                bool[] checkBoxArrayDemo, // Démo SelectList multiple

                string sQuizPrefixeSelect,
                string sQuizSuffixeSelect,
                bool bQuestionEnCours,
                List<string> lstExplicationsPrefixe,
                List<string> lstExplicationsSuffixe,
                string sMot0,

                MessageDetails message

                )
            {
                this.CtorSet(_ => _.InputValue, inputValue);
                this.CtorSet(_ => _.InputValueSTB, inputValueSTB);
                this.CtorSet(_ => _.Todos, todos);
                this.CtorSet(_ => _.Niveau, niveau);
                //this.CtorSet(_ => _.NiveauSimple, niveauSimple);
                this.CtorSet(_ => _.NbPrefixes, nbPrefixes);
                this.CtorSet(_ => _.NbQuestions, nbQuestions);
                this.CtorSet(_ => _.Difficulte, difficulte);
                this.CtorSet(_ => _.MotsExistants, motsExistants);
                this.CtorSet(_ => _.QuizInverse, quizInverse);
                this.CtorSet(_ => _.GrecoLatin, grecoLatin);
                this.CtorSet(_ => _.QuizGrecoLatin, quizGrecoLatin);
                this.CtorSet(_ => _.Neologisme, neologisme);
                this.CtorSet(_ => _.QuizNeologisme, quizNeologisme);
                this.CtorSet(_ => _.InfoBulle, infoBulle);
                this.CtorSet(_ => _.sAppli, appli);
                
                this.CtorSet(_ => _.LstMsg, lstMsg); // Démo ScrollTextBox
                this.CtorSet(_ => _.CheckBoxArrayDemo, checkBoxArrayDemo); // Démo SelectList multiple

                // Ancienne démo Bridge.React màj :
                this.CtorSet(_ => _.Titre, titre);
                this.CtorSet(_ => _.Contenu, contenu);
                this.CtorSet(_ => _.MessageHistoryLogotron, messageHistoryLogotron);
                this.CtorSet(_ => _.MessageHistoryQuiz, messageHistoryQuiz);
                this.CtorSet(_ => _.MessageHistoryTuto, messageHistoryTuto);

                this.CtorSet(_ => _.sQuizPrefixeSelect, sQuizPrefixeSelect);
                this.CtorSet(_ => _.sQuizSuffixeSelect, sQuizSuffixeSelect);
                this.CtorSet(_ => _.BQuestionEnCours, bQuestionEnCours);

                this.CtorSet(_ => _.lstExplicationsPrefixe, lstExplicationsPrefixe);
                this.CtorSet(_ => _.lstExplicationsSuffixe, lstExplicationsSuffixe);
                this.CtorSet(_ => _.sMot, sMot0);

                this.CtorSet(_ => _.Message, message);

            }
            public string InputValue { get; }
            public string InputValueSTB { get; }
            public NonNullList<TaskDetails> Todos { get; }
            public CheckBoxListMultipleEditState Niveau { get; }
            //public CheckBoxListSimpleEditState NiveauSimple { get; }
            public CheckBoxListSimpleEditState NbPrefixes { get; }
            public CheckBoxListSimpleEditState NbQuestions { get; }
            public CheckBoxListSimpleEditState Difficulte { get; }
            public CheckBoxEditState MotsExistants { get; }
            public CheckBoxEditState QuizInverse { get; }
            public CheckBoxEditState GrecoLatin { get; }
            public CheckBoxEditState QuizGrecoLatin { get; }
            public CheckBoxEditState Neologisme { get; }
            public CheckBoxEditState QuizNeologisme { get; }
            public CheckBoxEditState InfoBulle { get; }
            public string sAppli { get; }
            public string sQuizPrefixeSelect { get; }
            public string sQuizSuffixeSelect { get; }
            public bool BQuestionEnCours { get; }

            public NonNullList<MgsDetails> LstMsg { get; } // Démo ScrollTextBox

            // Ancienne démo Bridge.React màj :
            public TextInputDetails Titre { get; }
            public TextInputDetails Contenu { get; }
            public NonNullList<SavedMessageDetails> MessageHistoryLogotron { get; }
            public NonNullList<SavedMessageDetails> MessageHistoryQuiz { get; }
            
            public NonNullList<SavedMessageDetails> MessageHistoryTuto { get; }

            public bool[] CheckBoxArrayDemo { get; } // Démo SelectListMultiple

            public string strSTBLigneSel { get; }

            public List<string> lstExplicationsPrefixe { get; }
            public List<string> lstExplicationsSuffixe { get; }
            public string sMot { get; }

            public MessageDetails Message { get; }
        }

		protected override State GetInitialState()
		{
            return new State(
                inputValue: "",
                inputValueSTB: "", 
                todos: NonNullList.Of(
                    new TaskDetails(_nextAvailableIdToDoApp++, "Learn C#", done: true)
                ),
                niveau: new CheckBoxListMultipleEditState(
                    newCheckBoxArray: new bool[] { true, false, false }),
                //niveauSimple: new CheckBoxListSimpleEditState(
                //    text: "1", lstItems: lstItemsNiv), 
                nbPrefixes: new CheckBoxListSimpleEditState(
                    text: "H", lstItems: lstItemsNbPrefixes), 
                nbQuestions: new CheckBoxListSimpleEditState(
                    text: "5", lstItems: _quiz.lstItemsNbQ), 
                difficulte: new CheckBoxListSimpleEditState(
                    text: "1", lstItems: _quiz.lstItemsDiff), 
                
                // Coché en mode release, décoché en mode debug
                //  (car il n'y a que quelques mots en mode debug)
                motsExistants: new CheckBoxEditState(LogotronLib.clsConst.bRelease),

                quizInverse: new CheckBoxEditState(false),
                grecoLatin: new CheckBoxEditState(true),
                // Décoché pour le quiz, car motsExistants est déjà coché en release
                quizGrecoLatin: new CheckBoxEditState(false),
                neologisme: new CheckBoxEditState(false),
                quizNeologisme: new CheckBoxEditState(false),
                infoBulle: new CheckBoxEditState(true),
                appli: sAppliDef,

                // Ancienne démo Bridge.React màj :
                titre: new TextInputDetails(iIdTIBTitre, "Titre"),
                contenu: new TextInputDetails(iIdTIBContenu, "Contenu"),

                // Remplacer NonNullList par List : pas réussi encore
                // Il manque qqch à List<> par rapport à NonNullList
                // On peut se baser sur NonNullList pour faire une NullList, mais complexe
                // Voir dans le tuto d'origine : il y avait 2 classes, le NonNullList est uniquement pour les msg déjà sauvé
                //messageHistory: new List<SavedMessageDetails> {},
                messageHistoryLogotron: NonNullList.Of(
                    new SavedMessageDetails(_nextAvailableIdLogotron++,
                        new MessageDetails("", ""))), // title, content
                messageHistoryQuiz: NonNullList.Of(
                    new SavedMessageDetails(_quiz.nextAvailableIdQuiz++,
                        new MessageDetails("Score :", 
                            "1 point pour le préfixe ou le suffixe correct, et 3 points pour le préfixe et le suffixe corrects."))), // title, content
                messageHistoryTuto: NonNullList.Of(
                    new SavedMessageDetails(_nextAvailableIdTuto++,
                        new MessageDetails("", ""))), 

                lstMsg: NonNullList.Of(
                    new MgsDetails(_nextAvailableIdSTB++, "")),
                checkBoxArrayDemo: new bool[] { true, false, false }, // Démo SelectList multiple

                sQuizPrefixeSelect: "",
                sQuizSuffixeSelect: "",
                bQuestionEnCours: false,
                lstExplicationsPrefixe: new List<string>(),
                lstExplicationsSuffixe: new List<string>(),
                sMot0: "Dernier mot quiz",

                message: new MessageDetails("","")
            );
		}

        public static void ChargerLogotron()
        {
            clsGestBase.InitBases();
            var dicoMotsExistants = new Dictionary<string, clsMotExistant>();
            clsListeMotsExistants.ChargerMotsExistantsCode(dicoMotsExistants);
            clsGestBase.ChargerMotsExistants(dicoMotsExistants);
            clsLogotron.LireLogotronCode();
        }

        #endregion

        #region Rendu

        public override ReactElement Render()
		{

            const string sLogotron = "Logotron";
            const string sQuiz = "Quiz";
            const string sTuto = "Tuto. B.R."; // Tutoriel Bridge.React
            const string sToDo = "ToDo app.";
            const string sScrollTB = "ScrollT.B."; // ScrollTextBox

            var aAttr = new AnchorAttributes()
            {
                Target = "Target",
                Href = "http://patrice.dargenton.free.fr/CodesSources/Logotron/"
            };
            var sDoc = "Documentation";

            var selectLstAppItems = new ItemApi(new List<string> { sLogotron, sQuiz });
            if (LogotronLib.clsConst.bDebug) 
                selectLstAppItems = new ItemApi(new List<string> { 
                sLogotron, sQuiz, sTuto, sScrollTB, sToDo });
            var selectLstAppAtt = new Attributes { ClassName = "selectLstApp" };
            var selectLstApp = new SelectList(
                className: new NonBlankTrimmedString("ListeAppli"),
                title: "Applications",
                oneListPerLine: true, 
                itemAPI: selectLstAppItems,
                itemSelected: state.sAppli,
                onChange: e => SetState(state.With(_ => _.sAppli, e))
            );

            var fsa = new FieldSetAttributes{ ClassName = props.Label };

            if (state.sAppli == sTuto)
                return RenderTuto(fsa, selectLstApp, selectLstAppAtt, aAttr, sDoc);
            else if (state.sAppli == sToDo)
                return RenderToDo(fsa, selectLstApp, selectLstAppAtt, aAttr, sDoc);
            else if (state.sAppli == sScrollTB)
                return RenderScrollTBDemo(fsa, selectLstApp, selectLstAppAtt, aAttr, sDoc);
            else if (state.sAppli == sQuiz)
                return RenderQuiz(fsa, selectLstApp, selectLstAppAtt, aAttr, sDoc 
                    );
            else //if(state.sAppli == sLogotron)
                return RenderLogotron(fsa, selectLstApp, selectLstAppAtt, aAttr, sDoc);

        }
        #endregion

        #region Logotron

        private void TirerLogotron(ConfigLogotron config,
            out string sMot, out string sTxt)
        {
            string sSautDeLigneHtml = ""; // Ne marche pas : "<br>";
            sMot = "";
            string sExplication = "";
            string sDetail = "";
            var lstEtym = new List<string>();
            string sNbPrefixesSuccessifs = config.NbPrefixes;
            var lstNiv = new List<string>();
            if (config.Niveaux[0]) lstNiv.Add("1");
            if (config.Niveaux[1]) lstNiv.Add("2");
            if (config.Niveaux[2]) lstNiv.Add("3");
            
            var lstFreq = new List<string>();
            lstFreq.Add(enumFrequence.Frequent );
            lstFreq.Add(enumFrequence.Moyen);
            lstFreq.Add(enumFrequence.Rare);
            lstFreq.Add(enumFrequence.Absent);

            const bool bComplet = false;
            bool bGrecoLatin = state.GrecoLatin.Checked;
            bool bNeoRigolo = state.Neologisme.Checked;
            sTxt = "";

            m_msgDelegue = new clsMsgDelegue();
            EventHandler<clsMsgEventArgs> obj = this.MsgBox;
            m_msgDelegue.EvAfficherMessage += obj;

            if (clsLogotron.bTirage(bComplet, sNbPrefixesSuccessifs, lstNiv, lstFreq,
                ref sMot, ref sExplication, ref sDetail, ref lstEtym,
                bGrecoLatin, bNeoRigolo, m_msgDelegue))
            {
                sTxt = sMot + " : " + sExplication + " : " + sSautDeLigneHtml + sDetail + sSautDeLigneHtml;
                if (lstEtym.Count > 0)
                {
                    sTxt += " :";
                    foreach (string item in lstEtym)
                        sTxt += " " + item + sSautDeLigneHtml;
                }
            }
            else
                sTxt = "Echec du tirage !";
        }

        public ReactElement RenderLogotron(
            FieldSetAttributes fsa,
            SelectList selectLstApp, Attributes selectLstAppAtt,
            AnchorAttributes aAttr, string sDoc
            )
        {

            var chkBoxLstNivItemsAPI = new ItemApi(lstItemsNiv);
            var chkBoxLstNivAtt = new Attributes { ClassName = "chkBoxLstNiv" };
            var chkBoxLstNiv = new CheckBoxList(
                className: new NonBlankTrimmedString("chkBoxLstNiv"),
                titre: new NonBlankTrimmedString("Niveaux :"),
                disabled: false,
                checkBoxArray: state.Niveau.CheckBoxArray,
                itemAPI: chkBoxLstNivItemsAPI,
                onChange: async (e) =>
                {
                    await props.MessageApi.Attendre(iDelaiMsec: 10);
                    SetState(state.With(_ => _.Niveau,
                        new CheckBoxListMultipleEditState(
                            CheckBoxList.SetCheckBoxArray(e,
                                state.Niveau.CheckBoxArray, lstItemsNiv,
                                oneItemRequired: true))));
                    MajCombiLogotron();
                }
            );

            var chkBoxLstNbPItemsAPI = new ItemApi(lstItemsNbPrefixes);
            var chkBoxLstNbPAtt = new Attributes { ClassName = "chkBoxLstDiff" };
            var chkBoxLstNbP = new CheckBoxListSimple(
                className: new NonBlankTrimmedString("chkBoxLstDiff"),
                //titre: new NonBlankTrimmedString("Nb. préfixes :"),
                disabled: false,
                selectedItem: state.NbPrefixes.Text,
                itemAPI: chkBoxLstNbPItemsAPI,
                onChange: async (e) =>
                {
                    await props.MessageApi.Attendre(iDelaiMsec: 10);
                    SetState(state.With(_ => _.NbPrefixes,
                        new CheckBoxListSimpleEditState(e, lstItemsNbPrefixes)));
                    MajCombiLogotron();
                }
            );
            
            var chkBoxGrecoLatinAtt = new Attributes { ClassName = "chkBoxGrecoLatin" };
            var chkBoxGrecoLatin = new CheckBoxTip(
                className: new NonBlankTrimmedString("chkBoxGrecoLatin"),
                title: "Gréco-latin :",
                tip: "Cocher pour ne sélectionner que les préfixes et suffixes d'origine gréco-latine",
                disabled: false,
                checked0: state.GrecoLatin.Checked,
                onChange: async (e) =>
                {
                    await props.MessageApi.Attendre(iDelaiMsec: 10);
                    SetState(state.With(_ => _.GrecoLatin,
                        new CheckBoxEditState(!state.GrecoLatin.Checked)));
                    if (state.GrecoLatin.Checked)
                        SetState(state.With(_ => _.Neologisme, new CheckBoxEditState(false)));
                    MajCombiLogotron();
                }
            );
            
            var chkBoxNeoRigoloAtt = new Attributes { ClassName = "chkBoxNeoRigolo" };
            var chkBoxNeoRigolo = new CheckBoxTip(
                className: new NonBlankTrimmedString("chkBoxNeoRigolo"),
                title: "Néologisme :",
                tip: "Cocher pour inclure des néologismes amusants dans les préfixes et suffixes",
                disabled: false,
                checked0: state.Neologisme.Checked,
                onChange: async (e) =>
                {
                    await props.MessageApi.Attendre(iDelaiMsec: 10);
                    SetState(state.With(_ => _.Neologisme ,
                        new CheckBoxEditState(!state.Neologisme.Checked)));
                    if (state.Neologisme.Checked)
                        SetState(state.With(_ => _.GrecoLatin, new CheckBoxEditState(false)));
                    MajCombiLogotron();
                }
            );

            var btnALogotron = new ButtonAttributes {
                Disabled = false,
                OnClick = e => TirageLogotron()
            };

            var btnLogotron = DOM.Button(btnALogotron, "Logotron");

            var msgHist = new MessageHistory(state.MessageHistoryLogotron, sHistorique); // "HistoriqueLogotron");

            var tooltipAtt = new Attributes { ClassName = "tooltip" };
            var tooltiptextAtt = new Attributes { ClassName = "tooltiptext" };

            // ToDo : CheckBoxListSimpleTip
            return DOM.Div(
                new Attributes { Style = Style.Margin(iMargin).Padding(iPadding).FontSize(iFontSize) },
                DOM.A(aAttr, sDoc),
                DOM.H3(props.Label),
                DOM.Span(selectLstAppAtt, selectLstApp),
                DOM.FieldSet(
                    new FieldSetAttributes { ClassName = "legend" },
                    DOM.Legend(null, "Logotron"),
                    DOM.Div(null, 
                        DOM.Span(chkBoxLstNivAtt, chkBoxLstNiv),
                        DOM.Div(tooltipAtt,
                            "Nb. préfixes :",
                            DOM.Span(tooltiptextAtt,
                                "Nombre de préfixes successifs : H = Tirage au hasard")
                        ),
                        DOM.Span(chkBoxLstNbPAtt, chkBoxLstNbP),
                        DOM.Span(chkBoxGrecoLatinAtt, chkBoxGrecoLatin),
                        DOM.Span(chkBoxNeoRigoloAtt, chkBoxNeoRigolo)
                    ),
                    btnLogotron
                    ),
                DOM.Div(null, msgHist)
            );
        }
        
        private void MajCombiLogotron()
        {
            var lstNiv = new List<string>();
            string sNiveaux = "";
            if (state.Niveau.CheckBoxArray[0]) { lstNiv.Add("1"); sNiveaux = "1"; }
            if (state.Niveau.CheckBoxArray[1]) { lstNiv.Add("2"); sNiveaux += " 2"; }
            if (state.Niveau.CheckBoxArray[2]) { lstNiv.Add("3"); sNiveaux += " 3"; }
            
            var lstFreq = new List<string>();
            lstFreq.Add(enumFrequence.Frequent);
            lstFreq.Add(enumFrequence.Moyen);
            lstFreq.Add(enumFrequence.Rare);
            lstFreq.Add(enumFrequence.Absent);

            bool bGrecoLatin = state.GrecoLatin.Checked;
            bool bNeoRigolo = state.Neologisme.Checked;
            int iNbPrefixes = clsGestBase.m_prefixes.iLireNbSegmentsUniques(
                lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            int iNbSuffixes = clsGestBase.m_suffixes.iLireNbSegmentsUniques(
                lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            long lNbPrefixesCombi = 0L;
            string sTxtCombi = "";
            string sNbPrefixesSuccessifs = state.NbPrefixes.Text; 
            if (sNbPrefixesSuccessifs == "H" || sNbPrefixesSuccessifs == "1") {
                sTxtCombi = iNbPrefixes + " préfixes";
                lNbPrefixesCombi = iNbPrefixes;
            }
            else if (sNbPrefixesSuccessifs == "2") {
                sTxtCombi = iNbPrefixes + " préfixes x " +
                    (iNbPrefixes - 1) + " préfixes";
                lNbPrefixesCombi = iNbPrefixes * (iNbPrefixes - 1);
            }
            else if (sNbPrefixesSuccessifs == "3") {
                sTxtCombi = iNbPrefixes + " préfixes x " +
                    (iNbPrefixes - 1) + " préfixes x " +
                    (iNbPrefixes - 2) + " préfixes";
                lNbPrefixesCombi = iNbPrefixes * (iNbPrefixes - 1) *
                    (iNbPrefixes - 2);
            }
            else if (sNbPrefixesSuccessifs == "4") {
                sTxtCombi = iNbPrefixes + " préfixes x " +
                    (iNbPrefixes - 1) + " préfixes x " +
                    (iNbPrefixes - 2) + " préfixes x " +
                    (iNbPrefixes - 3) + " préfixes";
                lNbPrefixesCombi = (long)iNbPrefixes * (long)(iNbPrefixes - 1) *
                    (iNbPrefixes - 2) * (iNbPrefixes - 3);
            }
            else if (sNbPrefixesSuccessifs == "5") {
                sTxtCombi = iNbPrefixes + " préfixes x " +
                    (iNbPrefixes - 1) + " préfixes x " +
                    (iNbPrefixes - 2) + " préfixes x " +
                    (iNbPrefixes - 3) + " préfixes x " +
                    (iNbPrefixes - 4) + " préfixes";
                lNbPrefixesCombi = (long)iNbPrefixes * (long)(iNbPrefixes - 1) *
                    (iNbPrefixes - 2) * (iNbPrefixes - 3) * (iNbPrefixes - 4);
            }
            long lNbCombi = lNbPrefixesCombi * iNbSuffixes;
            string sNbCombi = UtilWeb.clsUtilWeb.sFormaterNumeriqueLongWeb(lNbCombi);
            AfficherTexteLogotron("Config.", sTxtCombi + " x " +
                (iNbSuffixes) + " suffixes = " +
                sNbCombi + " combinaisons pour le Logotron");
            AfficherTexteLogotron("Config.", 
                "Niveau(x) sélectionné(s) : " + sNiveaux + 
                ", nb. préfixes successifs : " + sNbPrefixesSuccessifs);
        }
        
        private void TirageLogotron()
        {
            string sMot = "";
            string sTxt = "";
            TirerLogotron(new ConfigLogotron(
                state.Niveau.CheckBoxArray, state.NbPrefixes.Text), out sMot, out sTxt);
            AfficherTexteLogotron(sMot, sTxt);
        }

        private void AfficherTexteLogotron(string sTitre, string sContenu)
        {
            var smd = new SavedMessageDetails(_nextAvailableIdLogotron++, 
                new MessageDetails(sTitre, sContenu));
            SetState(state.With(_ => _.MessageHistoryLogotron, value => value.Add(smd)));
        }

        #endregion

        #region Quiz

        public ReactElement RenderQuiz(FieldSetAttributes fsa,
            SelectList selectLstApp, Attributes selectLstAppAtt,
            AnchorAttributes aAttr, string sDoc)
        {

            // Liste simple de niveaux : 1 seul niveau à la fois
            //var chkBoxLstNivItemsAPI = new ItemApi(lstItemsNiv);
            //var chkBoxLstNivSimpleAtt = new Attributes { ClassName = "chkBoxLstNiv" };
            //var chkBoxLstNivSimple = new CheckBoxListSimple(
            //    className: new NonBlankTrimmedString("chkBoxLstNiv"),
            //    title: "Niveau :",
            //    disabled: _quiz.bQuizEnCours,
            //    selectedItem: state.NiveauSimple.Text,
            //    itemAPI: chkBoxLstNivItemsAPI,
            //    onChange: async (e) =>
            //    {
            //        await props.MessageApi.Attendre(iDelaiMsec: 10);
            //        SetState(state.With(_ => _.NiveauSimple,
            //            new CheckBoxListSimpleEditState(e, lstItemsNiv)));
            //        MajCombiQuiz();
            //    }
            //);
            
            var chkBoxLstNivItemsAPI = new ItemApi(lstItemsNiv);
            var chkBoxLstNivAtt = new Attributes { ClassName = "chkBoxLstNiv" };
            var chkBoxLstNiv = new CheckBoxList(
                className: new NonBlankTrimmedString("chkBoxLstNiv"),
                titre: new NonBlankTrimmedString("Niveaux :"),
                disabled: _quiz.bQuizEnCours, // 14/09/2018 false -> _quiz.bQuizEnCours
                checkBoxArray: state.Niveau.CheckBoxArray,
                itemAPI: chkBoxLstNivItemsAPI,
                onChange: async (e) =>
                {
                    await props.MessageApi.Attendre(iDelaiMsec: 10);
                    SetState(state.With(_ => _.Niveau,
                        new CheckBoxListMultipleEditState(
                            CheckBoxList.SetCheckBoxArray(e,
                                state.Niveau.CheckBoxArray, lstItemsNiv,
                                oneItemRequired: true))));
                    MajCombiQuiz();
                }
            );

            var chkBoxLstNbQItemsAPI = new ItemApi(_quiz.lstItemsNbQ);
            var chkBoxLstNbQAtt = new Attributes { ClassName = "chkBoxLstNbQ" };
            var chkBoxLstNbQ = new CheckBoxListSimple(
                className: new NonBlankTrimmedString("chkBoxLstNbQ"),
                title: "Nb. questions :",
                disabled: _quiz.bQuizEnCours,
                selectedItem: state.NbQuestions.Text,
                itemAPI: chkBoxLstNbQItemsAPI,
                onChange: e => SetState(
                    state.With(_ => _.NbQuestions,
                        new CheckBoxListSimpleEditState(e, _quiz.lstItemsNbQ)))
            );

            var chkBoxLstDiffItemsAPI = new ItemApi(_quiz.lstItemsDiff);
            var chkBoxLstDiffAtt = new Attributes { ClassName = "chkBoxLstDiff" };
            var chkBoxLstDiff = new CheckBoxListSimple(
                className: new NonBlankTrimmedString("chkBoxLstDiff"),
                title: "Difficulté :",
                disabled: _quiz.bQuizEnCours,
                selectedItem: state.Difficulte.Text,
                itemAPI: chkBoxLstDiffItemsAPI,
                onChange: e => SetState(
                    state.With(_ => _.Difficulte,
                        new CheckBoxListSimpleEditState(e, _quiz.lstItemsDiff)))
            );

            var chkBoxMotsExistantsAtt = new Attributes
            { ClassName = "chkBoxMotsExistants" };
            var chkBoxMotsExistants = new CheckBoxTip(
                className: new NonBlankTrimmedString("chkBoxMotsExistants"),
                title: "Mots existants :",
                tip: "Cocher pour faire le quiz sur les mots existants du dictionnaire (pour les questions suivantes)",
                disabled: false,
                checked0: state.MotsExistants.Checked,
                onChange: async (e) =>
                {
                    await props.MessageApi.Attendre(iDelaiMsec: 10);
                    SetState(state.With(_ => _.MotsExistants,
                        new CheckBoxEditState(!state.MotsExistants.Checked)));
                    // Pour les mots existants, toutes les origines sont incluses
                    // (sinon il faudrait ajouter l'origine des préfixes et suffixes dans le fichier des mots existants)
                    if (state.MotsExistants.Checked) { 
                        SetState(state.With(_ => _.QuizGrecoLatin, new CheckBoxEditState(false)));
                        SetState(state.With(_ => _.QuizNeologisme, new CheckBoxEditState(false)));
                    }
                    MajCombiQuiz();
                }
            );

            var chkBoxQuizInvAtt = new Attributes { ClassName = "chkBoxQuizInv" }; 
            var chkBoxQuizInv = new CheckBoxTip(
                className: new NonBlankTrimmedString("chkBoxQuizInv"),
                title: "Quiz inversé :",
                tip: "Cocher pour inverser le quiz (pour les questions suivantes) : trouver le préfixe et le suffixe d'une définition",
                disabled: false,
                checked0: state.QuizInverse.Checked,
                onChange: e => SetState(
                    state.With(_ => _.QuizInverse,
                        new CheckBoxEditState(!state.QuizInverse.Checked)))
            );

            var chkBoxGrecoLatinAtt = new Attributes { ClassName = "chkBoxGrecoLatin" };
            var chkBoxGrecoLatin = new CheckBoxTip(
                className: new NonBlankTrimmedString("chkBoxGrecoLatin"),
                title: "Gréco-latin :",
                tip: "Cocher pour ne sélectionner que les préfixes et suffixes d'origine gréco-latine (pour les questions suivantes)",
                disabled: false,
                checked0: state.QuizGrecoLatin.Checked,
                onChange: async (e) =>
                {
                    await props.MessageApi.Attendre(iDelaiMsec: 10);
                    SetState(state.With(_ => _.QuizGrecoLatin,
                        new CheckBoxEditState(!state.QuizGrecoLatin.Checked)));
                    // Si sélectionne les seules origines Greco-latines,
                    //  on ne peut plus se baser sur les mots existants (toutes origines)
                    if (state.QuizGrecoLatin.Checked) { 
                        SetState(state.With(_ => _.MotsExistants, new CheckBoxEditState(false)));
                        SetState(state.With(_ => _.QuizNeologisme, new CheckBoxEditState(false)));
                    }
                    MajCombiQuiz();
                }
            );
            
            var chkBoxNeoRigoloAtt = new Attributes { ClassName = "chkBoxNeoRigolo" };
            var chkBoxNeoRigolo = new CheckBoxTip(
                className: new NonBlankTrimmedString("chkBoxNeoRigolo"),
                title: "Néologisme :",
                tip: "Cocher pour inclure des néologismes amusants dans les préfixes et suffixes (pour les questions suivantes)",
                disabled: false,
                checked0: state.QuizNeologisme.Checked,
                onChange: async (e) =>
                {
                    await props.MessageApi.Attendre(iDelaiMsec: 10);
                    SetState(state.With(_ => _.QuizNeologisme,
                        new CheckBoxEditState(!state.QuizNeologisme.Checked)));
                    if (state.QuizNeologisme.Checked) { 
                        SetState(state.With(_ => _.QuizGrecoLatin, new CheckBoxEditState(false)));
                        SetState(state.With(_ => _.MotsExistants, new CheckBoxEditState(false)));
                    }
                    MajCombiQuiz();
                }
            );

            var chkBoxInfoBulleAtt = new Attributes { ClassName = "chkBoxInfoBulle" };
            var chkBoxInfoBulle = new CheckBoxTip(
                className: new NonBlankTrimmedString("chkBoxInfoBulle"),
                title: "Infobulle :",
                tip: "Décocher pour masquer les infobulles du choix des préfixes et suffixes",
                disabled: false,
                checked0: state.InfoBulle.Checked,
                onChange: e => SetState(
                    state.With(_ => _.InfoBulle,
                        new CheckBoxEditState(!state.InfoBulle.Checked)))
            );

            string sTipSuffixe = "Choisir le sens du suffixe parmi la liste";
            string sTipPrefixe = "Choisir le sens du préfixe parmi la liste";

            // Si le quiz n'est pas commencé, suivre l'état immédiatement
            //  sinon tenir compte de l'état au moment où la liste a été établie
            if ((!_quiz.bQuizEnCours && state.QuizInverse.Checked) || _quiz.bQuizInverse)
            {
                sTipSuffixe = "Choisir le suffixe parmi la liste";
                sTipPrefixe = "Choisir le préfixe parmi la liste";
            }
            // Simple vérification du résultat, pas de sélection à faire
            if (_quiz.bQuestionEnCours)
            {
                sTipSuffixe = "Rappel du choix";
                sTipPrefixe = "Rappel du choix";
            }
            if (!state.InfoBulle.Checked)
            {
                sTipSuffixe = "";
                sTipPrefixe = "";
            }

            var selectLstPrefixeItems = new ItemApi(state.lstExplicationsPrefixe);
            var selectLstPrefixeAtt = new Attributes { ClassName = "selectLstPrefixe" };
            var selectLstPrefixe = new SelectListTip(
                className: new NonBlankTrimmedString("ListePrefixes"),
                title: "Préfixes :",
                tip: sTipPrefixe,
                itemAPI: selectLstPrefixeItems,
                itemSelected: state.sQuizPrefixeSelect,
                disabled: _quiz.bQuestionEnCours,
                onChange: e => SetState(state.With(_ => _.sQuizPrefixeSelect, e))
            );

            var selectLstSuffixeItems = new ItemApi(state.lstExplicationsSuffixe);
            var selectLstSuffixeAtt = new Attributes { ClassName = "selectLstSuffixe" };
            var selectLstSuffixe = new SelectListTip(
                className: new NonBlankTrimmedString("ListeSuffixes"),
                title: "Suffixes :",
                tip: sTipSuffixe,
                itemAPI: selectLstSuffixeItems,
                itemSelected: state.sQuizSuffixeSelect,
                disabled: _quiz.bQuestionEnCours,
                onChange: e => SetState(state.With(_ => _.sQuizSuffixeSelect, e))
            );

            var btnQuizAtt = new ButtonAttributes{ 
                OnClick = async (e) =>
                {
                    var lstExplicationsPrefixe0 = new List<string>();
                    var lstExplicationsSuffixe0 = new List<string>();
                    if (state.BQuestionEnCours)
                    {
                        lstExplicationsPrefixe0 = state.lstExplicationsPrefixe;
                        lstExplicationsSuffixe0 = state.lstExplicationsSuffixe;
                    }

                    string sQuizPrefixeSelect0 ="";
                    string sQuizSuffixeSelect0 = "";
                    bool bSucces0 = false;
                    TirageQuiz(lstExplicationsPrefixe0, lstExplicationsSuffixe0,
                        ref sQuizPrefixeSelect0, ref sQuizSuffixeSelect0, ref bSucces0);

                    // En cas de succès, éviter l'étape "Poursuivre" : ne marche pas !?
                    //if (bSucces0)
                    //{
                    //    //SetState(state.With(_ => _.BQuestionEnCours, false));
                    //    _quiz.bQuestionEnCours = false;
                    //    bool bSucces1 = false;
                    //    TirageQuiz(lstExplicationsPrefixe0, lstExplicationsSuffixe0,
                    //        ref sQuizPrefixeSelect0, ref sQuizSuffixeSelect0, ref bSucces1);
                    //    //var sNbQuestions = state.NbQuestions.Text;
                    //    //var iNbQuestions = Int32.Parse(sNbQuestions);
                    //    //if (_quiz.iNumQuestionQuiz < iNbQuestions) _quiz.iNumQuestionQuiz++;
                    //}

                    await props.MessageApi.Attendre(iDelaiMsec: 10);

                    // En cas de succès, éviter l'étape "Poursuivre" : ne marche pas !?
                    //if (bSucces0) 
                    //    SetState(state.With(_ => _.BQuestionEnCours, false));
                    //else
                    SetState(state.With(_ => _.BQuestionEnCours, !state.BQuestionEnCours));
                   

                    if (state.BQuestionEnCours)
                    {
                        SetState(state.With(_ => _.sQuizPrefixeSelect, sQuizPrefixeSelect0));
                        SetState(state.With(_ => _.sQuizSuffixeSelect, sQuizSuffixeSelect0));
                        SetState(state.With(_ => _.lstExplicationsPrefixe, 
                            lstExplicationsPrefixe0));
                        SetState(state.With(_ => _.lstExplicationsSuffixe,
                            lstExplicationsSuffixe0));
                    }

                    if (!_quiz.bQuizEnCours) SetState(state.With(_ => _.BQuestionEnCours, false));

                }
            };
            var btnQuiz = DOM.Button(btnQuizAtt,
                (_quiz.bQuizEnCours) ?
                    (_quiz.bQuestionEnCours) ?
                        "Quiz : question n°" + (_quiz.iNumQuestionQuiz) + " : " + _quiz.sMotEnCours + " Poursuivre" : 
                        "Quiz : question n°" + (_quiz.iNumQuestionQuiz) + " : " + _quiz.sMotEnCours : 
                    "Quiz");

            var msgHist = new MessageHistory(state.MessageHistoryQuiz, sHistorique);

            // Div : bloc avec saut de ligne, Span : bloc en ligne
            return DOM.Div(
                new Attributes { Style = Style.Margin(iMargin).Padding(iPadding).FontSize(iFontSize) },
                DOM.A(aAttr, sDoc),
                DOM.H3(props.Label),
                DOM.Span(selectLstAppAtt, selectLstApp),

                DOM.FieldSet(
                    new FieldSetAttributes { ClassName = "legend" },
                    DOM.Legend(null, "Quiz"),
                    DOM.Div(null,
                        DOM.Span(chkBoxMotsExistantsAtt, chkBoxMotsExistants),
                        DOM.Span(chkBoxQuizInvAtt, chkBoxQuizInv),
                        DOM.Span(chkBoxGrecoLatinAtt, chkBoxGrecoLatin),
                        DOM.Span(chkBoxNeoRigoloAtt, chkBoxNeoRigolo),
                        DOM.Span(chkBoxInfoBulleAtt, chkBoxInfoBulle)
                        ),
                    DOM.Span(chkBoxLstNbQAtt, chkBoxLstNbQ),
                    DOM.Div(null,
                        //DOM.Span(chkBoxLstNivSimpleAtt, chkBoxLstNivSimple),
                        DOM.Span(chkBoxLstNivAtt, chkBoxLstNiv),
                        DOM.Span(chkBoxLstDiffAtt, chkBoxLstDiff)
                        ),
                    btnQuiz,
                    DOM.Div(null,
                        DOM.Span(selectLstPrefixeAtt, selectLstPrefixe),
                        DOM.Span(selectLstSuffixeAtt, selectLstSuffixe)
                        )
                    ),
                DOM.Div(null, msgHist)
                );
        }

        private void MajCombiQuiz()
        {
            var lstNiv = new List<string>();
            //string sNiv = state.NiveauSimple.Text;
            //int iNiveau = int.Parse(sNiv);
            //lstNiv.Add(sNiv); 
            string sNiv = "";
            if (state.Niveau.CheckBoxArray[0]) { lstNiv.Add("1"); sNiv += "1 "; }
            if (state.Niveau.CheckBoxArray[1]) { lstNiv.Add("2"); sNiv += "2 "; }
            if (state.Niveau.CheckBoxArray[2]) { lstNiv.Add("3"); sNiv += "3 "; }

            var lstFreq = new List<string>();
            lstFreq.Add(enumFrequence.Frequent);
            lstFreq.Add(enumFrequence.Moyen);
            lstFreq.Add(enumFrequence.Rare);
            lstFreq.Add(enumFrequence.Absent);

            if (state.MotsExistants.Checked)
            {
                int iNbMotsExistants0 = clsGestBase.iNbMotsExistants(lstNiv, lstFreq);
                int iNbMots = iNbMotsExistants0;
                string sNbMots = UtilWeb.clsUtilWeb.sFormaterNumeriqueWeb(iNbMots);
                AfficherTexteQuiz("Config.", sNbMots + " mots existants pour le quiz");

                int iNbPE = clsGestBase.iNbPrefixesMotsExistants(lstNiv, lstFreq);
                string sNbPE = clsUtil.sFormaterNumeriqueLong(iNbPE);
                AfficherTexteQuiz("Config.", sNbPE + " préfixes distincts pour les mots existants");
                int iNbSE = clsGestBase.iNbSuffixesMotsExistants(lstNiv, lstFreq);
                string sNbSE = clsUtil.sFormaterNumeriqueLong(iNbSE);
                AfficherTexteQuiz("Config.", sNbSE + " suffixes distincts pour les mots existants");

                const bool bDebugMots = false;
                if (bDebugMots)
                {
                    //int i = 0;
                    //foreach (var mot in clsGestBase.lstMotsExistants(lstNiv, lstFreq))
                    //    AfficherTexteQuiz("Config.", ++i + " : " + mot.ToString());
                }

            }
            else
            {
                bool bGrecoLatin = state.QuizGrecoLatin.Checked;
                bool bNeoRigolo = state.QuizNeologisme.Checked;
                int iNbPrefixes = clsGestBase.m_prefixes.iLireNbSegmentsUniques(lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
                int iNbSuffixes = clsGestBase.m_suffixes.iLireNbSegmentsUniques(lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
                long lNbCombi = iNbPrefixes * iNbSuffixes;
                string sNbCombi = UtilWeb.clsUtilWeb.sFormaterNumeriqueLongWeb(lNbCombi);
                AfficherTexteQuiz("Config.", iNbPrefixes + " préfixes" + " x " + iNbSuffixes + " suffixes = " +
                    sNbCombi + " combinaisons pour le quiz");
            }
            //AfficherTexteQuiz("Config.", "Niveau sélectionné : " + sNiv);
            AfficherTexteQuiz("Config.", "Niveau(x) sélectionné(s) : " + sNiv);
        }

        private void AfficherTexteQuiz(string sTitre, string sContenu)
        {
            var smd = new SavedMessageDetails(
                _quiz.nextAvailableIdQuiz++,
                new MessageDetails(sTitre, sContenu));
            SetState(state.With(_ => _.MessageHistoryQuiz, value => value.Add(smd)));
        }

        private void TirageQuiz(
            List<string> lstExplicationsPrefixe,
            List<string> lstExplicationsSuffixe,
            ref string sQuizPrefixeSelect, ref string sQuizSuffixeSelect, 
            ref bool bSucces)
        {

            m_msgDelegue = new clsMsgDelegue();
            EventHandler<clsMsgEventArgs> obj = this.MsgBox;
            m_msgDelegue.EvAfficherMessage += obj;
            clsGestBase.m_prefixes.MsgDelegue = m_msgDelegue;
            clsGestBase.m_suffixes.MsgDelegue = m_msgDelegue;
            clsGestBase.m_msgDelegue = m_msgDelegue;

            string sMot = "";
            string sConfig = "";
            var sNbQuestions = state.NbQuestions.Text;
            var iNbQuestions = Int32.Parse(sNbQuestions);

            var lstNiv = new List<string>();
            //string sNiv = state.NiveauSimple.Text;
            //int iNiveau = int.Parse(sNiv);
            //lstNiv.Add(sNiv);
            string sNiv = "";
            if (state.Niveau.CheckBoxArray[0]) { lstNiv.Add("1"); sNiv += "1 "; }
            if (state.Niveau.CheckBoxArray[1]) { lstNiv.Add("2"); sNiv += "2 "; }
            if (state.Niveau.CheckBoxArray[2]) { lstNiv.Add("3"); sNiv += "3 "; }
            int iCoefNiv = enumNiveau.iCoef(sNiv);

            string sFreq = "";
            var lstFreq = new List<string>();
            lstFreq.Add(enumFrequence.Frequent); sFreq += enumFrequenceAbrege.Frequent + " ";
            lstFreq.Add(enumFrequence.Moyen); sFreq += enumFrequenceAbrege.Moyen + " ";
            lstFreq.Add(enumFrequence.Rare); sFreq += enumFrequenceAbrege.Rare + " ";
            lstFreq.Add(enumFrequence.Absent); sFreq += enumFrequenceAbrege.Absent + " ";
            int iCoefFreq = enumFrequenceAbrege.iCoef(sFreq);

            var sAlt = state.Difficulte.Text;
            var iAlternatives = Int32.Parse(sAlt);

            // Mémoriser le sens du quiz au moment du tirage, jusqu'au prochain tirage
            if (!_quiz.bQuestionEnCours)
                _quiz.bQuizInverse = state.QuizInverse.Checked;

            if (_quiz.bQuizEnCours) 
            {
                _quiz.bQuestionEnCours = !_quiz.bQuestionEnCours;
                
                // Parfois une question en + ?
                //if (!state.BQuestionEnCours) _quiz.iNumQuestionQuiz++;
                if (!state.BQuestionEnCours && _quiz.iNumQuestionQuiz < iNbQuestions) _quiz.iNumQuestionQuiz++;
                
                if (state.BQuestionEnCours && _quiz.iNumQuestionQuiz >= iNbQuestions) _quiz.bDernQuestion = true;
            }
            else
            {
                _quiz.bQuizEnCours = true;
                _quiz.bQuestionEnCours = false;
                _quiz.bDernQuestion = false;
                _quiz.iNumQuestionQuiz = 1;
            }

            if (_quiz.bDernQuestion && !_quiz.bQuestionEnCours) 
            {
                _quiz.iNumQuestionQuiz = 0;
                _quiz.iScoreTot = 0;
                _quiz.bQuizEnCours = false;
                sMot = "Quiz terminé.";
            }

            bSucces = false;
            string sMotADeviner = _quiz.sMotEnCours;
            if (_quiz.iNumQuestionQuiz > 0 && _quiz.bQuizEnCours) 
            {
                if (_quiz.bQuestionEnCours) 
                {
                    sMot = sMotADeviner;
                    sConfig = "Q. n° " + (_quiz.iNumQuestionQuiz) + "/" + sNbQuestions + " : ";
                    sConfig += " " + _quiz.sDetail + " : ";
                    if (_quiz.lstEtym != null && _quiz.lstEtym.Count > 0)
                        foreach (string sEtym in _quiz.lstEtym) sConfig += sEtym + " ";

                    string sSensPrefixeMaj = _quiz.sQuizPrefixeOk;
                    string sSensSuffixeMaj = _quiz.sQuizSuffixeOk;
                    string sPrefixe = _quiz.sQuizPrefixeOk; // ToDo : aussi en minuscule
                    string sSuffixe = _quiz.sQuizSuffixeOk;
                    string sExplication = sSensSuffixeMaj + " " + sSensPrefixeMaj;
                    if (state.QuizInverse.Checked)
                        sExplication = sSensPrefixeMaj + " " + sSensSuffixeMaj;
                    int iScore = 0;
                    string sSensPrefixeChoisi = state.sQuizPrefixeSelect; // this.lbPrefixesPossibles.Text;
                    string sSensSuffixeChoisi = state.sQuizSuffixeSelect; // this.lbSuffixesPossibles.Text;
                    bool bPrefixeOk;
                    bool bSuffixeOk;
                    bPrefixeOk = false;
                    bSuffixeOk = false;
                    if (sSensPrefixeChoisi == sSensPrefixeMaj &&
                        sSensSuffixeChoisi == sSensSuffixeMaj)
                    {
                        iScore += Quiz.iCoefBonus;
                        bPrefixeOk = true;
                        bSuffixeOk = true;
                        bSucces = true;
                    }
                    else if (sSensPrefixeChoisi == sSensPrefixeMaj)
                    {
                        iScore++;
                        bPrefixeOk = true;
                    }
                    else if (sSensSuffixeChoisi == sSensSuffixeMaj)
                    {
                        iScore++;
                        bSuffixeOk = true;
                    }

                    string sAffPrefixe = sSensPrefixeChoisi + " : Faux ! ";
                    string sAffSuffixe = sSensSuffixeChoisi + " : Faux ! ";
                    if (sSensPrefixeChoisi.Length == 0) sAffPrefixe = "";
                    if (sSensSuffixeChoisi.Length == 0) sAffSuffixe = "";

                    if (bPrefixeOk && bSuffixeOk)
                    {
                        sConfig += sExplication + " : Exact !!";
                    }
                    else if (bPrefixeOk)
                    {
                        sConfig += " : Exact ! ";
                        sConfig += sAffSuffixe + "Réponse = -" + sSuffixe + " : " + sSensSuffixeMaj;
                    }
                    else if (bSuffixeOk)
                    {
                        sConfig += sAffPrefixe + "Réponse = " + sPrefixe + "- : " + sSensPrefixeMaj + ". ";
                        sConfig += sSensSuffixeMaj + " : Exact !";
                    }
                    else
                    {
                        sConfig += sAffPrefixe + "Réponse = " + sPrefixe + "- : " + sSensPrefixeMaj + ". ";
                        sConfig += sAffSuffixe + "Réponse = -" + sSuffixe + " : " + sSensSuffixeMaj;
                    }

                    // Calcul du score
                    _quiz.iScoreTot += iScore;
                    string sScore = " -> Résultat = " + _quiz.iScoreTot + " / " + _quiz.iNumQuestionQuiz * Quiz.iCoefBonus;

                    string sScoreFinal = "";
                    if (_quiz.bDernQuestion)
                    { 
                        int iCoefAlternatives = iAlternatives + 1;
                        int iCoefNBQ = iNbQuestions;
                        string sResultatFinal = " : Résultat final niveau(x) "
                            + sNiv + " et difficulté " + iAlternatives + " avec "
                            + iNbQuestions + " questions = " + _quiz.iScoreTot + " / "
                            + iNbQuestions * Quiz.iCoefBonus;
                        sScoreFinal = sResultatFinal + " -> Score final = " +
                            _quiz.iScoreTot * iCoefNiv * iCoefFreq * iCoefAlternatives * iCoefNBQ; 
                    }
                    sConfig += sScore + sScoreFinal;

                }
                else
                {
                    if (state.MotsExistants.Checked)
                    {
                        if (state.QuizInverse.Checked)
                            QuizSegmentMotExistant(clsGestBase.m_prefixes, clsGestBase.m_suffixes,
                                lstNiv, lstFreq, iAlternatives, 
                                lstExplicationsPrefixe, lstExplicationsSuffixe,
                                ref sQuizPrefixeSelect, ref sQuizSuffixeSelect, ref sMotADeviner);
                        else
                            QuizDefinitionMotExistant(clsGestBase.m_prefixes, clsGestBase.m_suffixes,
                                lstNiv, lstFreq, iAlternatives, 
                                lstExplicationsPrefixe, lstExplicationsSuffixe,
                                ref sQuizPrefixeSelect, ref sQuizSuffixeSelect, ref sMotADeviner);
                    }
                    else
                    {
                        if (state.QuizInverse.Checked)
                            QuizSegment(clsGestBase.m_prefixes, clsGestBase.m_suffixes,
                                lstNiv, lstFreq, iAlternatives, 
                                lstExplicationsPrefixe, lstExplicationsSuffixe,
                                ref sQuizPrefixeSelect, ref sQuizSuffixeSelect, ref sMotADeviner);
                        else
                            QuizDefinition(clsGestBase.m_prefixes, clsGestBase.m_suffixes,
                                lstNiv, lstFreq, iAlternatives, 
                                lstExplicationsPrefixe, lstExplicationsSuffixe,
                                ref sQuizPrefixeSelect, ref sQuizSuffixeSelect, ref sMotADeviner);
                    }
                    sMot = sMotADeviner;
                    sConfig = "Q. n° " + (_quiz.iNumQuestionQuiz) + "/" + sNbQuestions;
                    _quiz.sMotEnCours = sMotADeviner;
                }
            }

            //const bool bDebugMots = false;
            //if (bDebugMots)
            //{
            //    int i = 0;
            //    int iNiveau = 1;
            //    sConfig = "Liste des mots : ";
            //    foreach (var mot in clsGestBase.lstMotsExistants(iNiveau))
            //    {
            //        sConfig += mot.ToString() + ", ";
            //    }
            //}

            AfficherTexteQuiz(sMot, sConfig);
        }

        private void QuizSegmentMotExistant(clsBase basePrefixe, clsBase baseSuffixe,
            List<string> lstNiv, List<string> lstFreq, int iAlternatives, 
            List<string> lstExplicationsPrefixe,
            List<string> lstExplicationsSuffixe,
            ref string sQuizPrefixeSelect, ref string sQuizSuffixeSelect, ref string sMot0)
        {
            //  Quiz sur le préfixe et le suffixe correspondant à une définition

            //iScore = 0;
            clsSegmentBase prefixe = null;
            clsSegmentBase suffixe = null;
            int iNumMotExistant = clsGestBase.iTirageMotExistant(lstNiv, lstFreq,
                ref prefixe, ref suffixe);
            if (iNumMotExistant == clsConst.iTirageImpossible) return;

            string sNiveauP = prefixe.sNiveau;
            string sNiveauS = suffixe.sNiveau;
            string sPrefixe = prefixe.sSegment;
            string sPrefixeElision = prefixe.sSegmentElision;
            string sSuffixe = suffixe.sSegment;
            //string sPrefixeMaj = sPrefixe.ToUpper();
            string sPrefixeMaj = sPrefixeElision.ToUpper(); // 01/05/2019
            string sSuffixeMaj = sSuffixe.ToUpper();
            string sSensPrefixe = prefixe.sSens;
            string sSensPrefixeMaj = sSensPrefixe.ToUpper();
            sSensPrefixeMaj = clsBase.sCompleterPrefixe(sSensPrefixeMaj);
            prefixe.sEtym = clsGestBase.m_prefixes.sTrouverEtymologie(sPrefixe, prefixe.sUniciteSynth); // 10/05/2018
            string sEtymPrefixe = prefixe.sEtym;
            string sSensSuffixe = suffixe.sSens;
            string sSensSuffixeMaj = sSensSuffixe.ToUpper();
            suffixe.sEtym = clsGestBase.m_suffixes.sTrouverEtymologie(sSuffixe, suffixe.sUniciteSynth); // 10/05/2018
            string sEtymSuffixe = suffixe.sEtym;
            //string sMot = sPrefixeMaj + sSuffixeMaj;
            string sDetail = sPrefixeMaj + "(" + sNiveauP + ")-" +
                sSuffixeMaj + "(" + sNiveauS + ")";
            string sExplication = sSensSuffixeMaj + " " + sSensPrefixeMaj;
            sMot0 = sExplication;
            string sPrefixeMajT = sPrefixeMaj + "-";
            string sTSuffixeMaj = "-" + sSuffixeMaj;
            var lstEtym = new List<string>();
            if (sEtymPrefixe.Length > 0) lstEtym.Add(sPrefixe + "- : " + sEtymPrefixe);
            if (sEtymSuffixe.Length > 0) lstEtym.Add("-" + sSuffixe + " : " + sEtymSuffixe);

            //if (bDebugUnicite)
            //{
            //    if (prefixe.sUnicite.Length > 0)
            //        sSensPrefixeMaj += " (" + prefixe.sUnicite + ")";
            //    if (suffixe.sUnicite.Length > 0)
            //        sSensSuffixeMaj += " (" + suffixe.sUnicite + ")";
            //}

            //if (clsConst.bDebug && string.IsNullOrEmpty(prefixe.sUniciteSynth))
            //    Debugger.Break();
            //if (clsConst.bDebug && string.IsNullOrEmpty(suffixe.sUniciteSynth))
            //    Debugger.Break();

            //if (bDebugUniciteSynth)
            //{
            //    sSensPrefixeMaj += " (" + prefixe.sUniciteSynth + ")";
            //    sSensSuffixeMaj += " (" + suffixe.sUniciteSynth + ")";
            //}

            List<int> lstNumMotExistant = new List<int>();
            clsInitTirage itPrefixe = new clsInitTirage(prefixe);
            clsInitTirage itSuffixe = new clsInitTirage(suffixe);

            lstExplicationsPrefixe.Add(sPrefixeMajT);
            _quiz.sQuizPrefixeOk = sPrefixeMajT;
            lstExplicationsSuffixe.Add(sTSuffixeMaj);
            _quiz.sQuizSuffixeOk = sTSuffixeMaj;
            _quiz.sDetail = sDetail;
            _quiz.lstEtym = lstEtym;

            for (int j = 0; j <= iAlternatives - 1; j++)
            {
                clsMotExistant motAutre = null;
                int iNumMotExistantAutre = clsGestBase.iTirageMotExistantAutre(
                    lstNiv, lstFreq, iNumMotExistant, itPrefixe, itSuffixe, 
                    lstNumMotExistant, ref motAutre);
                if (iNumMotExistantAutre == clsConst.iTirageImpossible) break;
                if (motAutre == null) break;

                string sDefPrefixe = motAutre.sPrefixe.ToUpper() + "-";
                string sDefSuffixe = "-" + motAutre.sSuffixe.ToUpper();

                //if (bDebugUnicite)
                //{
                //    if ((motAutre.sUnicitePrefixe.Length > 0))
                //    {
                //        if (motAutre.sUnicitePrefixe.Length > 0)
                //            sDefPrefixe += " (" + motAutre.sUnicitePrefixe + ")";
                //        if (motAutre.sUniciteSuffixe.Length > 0)
                //            sDefSuffixe += " (" + motAutre.sUniciteSuffixe + ")";
                //    }
                //}

                //if (clsConst.bDebug && string.IsNullOrEmpty(motAutre.sUnicitePrefixeSynth))
                //    Debugger.Break();
                //if (clsConst.bDebug && string.IsNullOrEmpty(motAutre.sUniciteSuffixeSynth))
                //    Debugger.Break();

                //if (bDebugUniciteSynth)
                //{
                //    sDefPrefixe += " (" + motAutre.sUnicitePrefixeSynth + ")";
                //    sDefSuffixe += " (" + motAutre.sUniciteSuffixeSynth + ")";
                //}

                lstExplicationsPrefixe.Add(sDefPrefixe);
                lstExplicationsSuffixe.Add(sDefSuffixe);
            }
            BrasserListAuHasard(lstExplicationsPrefixe);
            BrasserListAuHasard(lstExplicationsSuffixe);
            // Voir si on peut ne rien sélectionner plutôt
            // Pas possible ? On est obligé de sélectionner un élément ?
            sQuizPrefixeSelect = lstExplicationsPrefixe[0];
            sQuizSuffixeSelect = lstExplicationsSuffixe[0];
            //sQuizPrefixeSelect = ""; sQuizSuffixeSelect = "";
            //sQuizPrefixeSelect = lstExplicationsPrefixe[1];
            //sQuizSuffixeSelect = lstExplicationsSuffixe[1];
        }

        private void QuizDefinitionMotExistant(clsBase basePrefixe, clsBase baseSuffixe,
            List<string> lstNiv, List<string> lstFreq, int iAlternatives, 
            List<string> lstExplicationsPrefixe,
            List<string> lstExplicationsSuffixe,
            ref string sQuizPrefixeSelect, ref string sQuizSuffixeSelect, ref string sMot0)
        {
            //  Quiz sur la définition du préfixe et du suffixe

            //iScore = 0;
            // Recomm:
            clsSegmentBase prefixe = null;
            clsSegmentBase suffixe = null;
            int iNumMotExistant = clsGestBase.iTirageMotExistant(lstNiv, lstFreq,
                ref prefixe, ref suffixe);
            if (iNumMotExistant == clsConst.iTirageImpossible) return;

            // If iNumMotExistant <> 211 Then GoTo Recomm
            string sNiveauP = prefixe.sNiveau;
            string sNiveauS = suffixe.sNiveau;
            string sPrefixe = prefixe.sSegment;
            string sPrefixeElision = prefixe.sSegmentElision;
            string sSuffixe = suffixe.sSegment;
            //string sPrefixeMaj = sPrefixe.ToUpper();
            string sPrefixeMaj = sPrefixeElision.ToUpper(); // 01/05/2019
            string sSuffixeMaj = sSuffixe.ToUpper();
            string sSensPrefixeMaj = prefixe.sSens.ToUpper();
            sSensPrefixeMaj = clsBase.sCompleterPrefixe(sSensPrefixeMaj);
            prefixe.sEtym = clsGestBase.m_prefixes.sTrouverEtymologie(sPrefixe, prefixe.sUniciteSynth); // 10/05/2018
            string sEtymPrefixe = prefixe.sEtym;
            string sSensSuffixeMaj = suffixe.sSens.ToUpper();
            suffixe.sEtym = clsGestBase.m_suffixes.sTrouverEtymologie(sSuffixe, suffixe.sUniciteSynth); // 10/05/2018
            string sEtymSuffixe = suffixe.sEtym;
            sMot0 = sPrefixeMaj + sSuffixeMaj;
            string sDetail = sPrefixeMaj + "(" + sNiveauP + ")-" + sSuffixeMaj + "(" + sNiveauS + ")";
            string sExplication = sSensSuffixeMaj + " " + sSensPrefixeMaj;
            var lstEtym = new List<string>();
            if (sEtymPrefixe.Length > 0) lstEtym.Add(sPrefixe + "- : " + sEtymPrefixe);
            if (sEtymSuffixe.Length > 0) lstEtym.Add("-" + sSuffixe + " : " + sEtymSuffixe);

            List<int> lstNumMotExistant = new List<int>();
            clsInitTirage itPrefixe = new clsInitTirage(prefixe);
            clsInitTirage itSuffixe = new clsInitTirage(suffixe);
            //List<string> lstExplicationsPrefixe = new List<string>();
            //if (bDebugUnicite)
            //{
            //    //Debug.WriteLine(("iNumMotExistant = " + iNumMotExistant));
            //    if (prefixe.sUnicite.Length > 0)
            //        sSensPrefixeMaj += " (" + prefixe.sUnicite + ")";
            //    if (suffixe.sUnicite.Length > 0)
            //        sSensSuffixeMaj += " (" + suffixe.sUnicite + ")";
            //}

            //if (clsConst.bDebug && string.IsNullOrEmpty(prefixe.sUniciteSynth))
            //    Debugger.Break();
            //if (clsConst.bDebug && string.IsNullOrEmpty(suffixe.sUniciteSynth))
            //    Debugger.Break();

            //if (bDebugUniciteSynth)
            //{
            //    //Debug.WriteLine(("iNumMotExistant = " + iNumMotExistant));
            //    sSensPrefixeMaj += " (" + prefixe.sUniciteSynth + ")";
            //    sSensSuffixeMaj += " (" + suffixe.sUniciteSynth + ")";
            //}

            //List<string> lstExplicationsSuffixe = new List<string>();

            lstExplicationsPrefixe.Add(sSensPrefixeMaj);
            _quiz.sQuizPrefixeOk = sSensPrefixeMaj;
            lstExplicationsSuffixe.Add(sSensSuffixeMaj);
            _quiz.sQuizSuffixeOk = sSensSuffixeMaj;
            _quiz.sDetail = sDetail;
            _quiz.lstEtym = lstEtym;

            for (int j = 0; j <= iAlternatives - 1; j++)
            {
                clsMotExistant motAutre = null;
                int iNumMotExistantAutre = clsGestBase.iTirageMotExistantAutre(
                    lstNiv, lstFreq, iNumMotExistant, itPrefixe, itSuffixe, 
                    lstNumMotExistant, ref motAutre);
                if (iNumMotExistantAutre == clsConst.iTirageImpossible) break;
                if (motAutre == null) break;

                string sDefPrefixe = motAutre.sDefPrefixe;
                string sDefSuffixe = motAutre.sDefSuffixe;
                //if (bDebugUnicite)
                //{
                //    if ((motAutre.sUnicitePrefixe.Length > 0))
                //    {
                //        if (motAutre.sUnicitePrefixe.Length > 0)
                //            sDefPrefixe += " [" + motAutre.sUnicitePrefixe + "]";
                //        if (motAutre.sUniciteSuffixe.Length > 0)
                //            sDefSuffixe += " [" + motAutre.sUniciteSuffixe + "]";
                //    }
                //}

                //if (clsConst.bDebug && string.IsNullOrEmpty(motAutre.sUnicitePrefixeSynth))
                //    Debugger.Break();
                //if (clsConst.bDebug && string.IsNullOrEmpty(motAutre.sUniciteSuffixeSynth))
                //    Debugger.Break();

                //if (bDebugUniciteSynth)
                //{
                //    sDefPrefixe += " (" + motAutre.sUnicitePrefixeSynth + ")";
                //    sDefSuffixe += " (" + motAutre.sUniciteSuffixeSynth + ")";
                //}

                lstExplicationsPrefixe.Add(sDefPrefixe);
                lstExplicationsSuffixe.Add(sDefSuffixe);
            }
            BrasserListAuHasard(lstExplicationsPrefixe);
            BrasserListAuHasard(lstExplicationsSuffixe);
            // Voir si on peut ne rien sélectionner plutôt
            sQuizPrefixeSelect = lstExplicationsPrefixe[0];
            sQuizSuffixeSelect = lstExplicationsSuffixe[0];
        }

        private void QuizSegment(clsBase basePrefixe, clsBase baseSuffixe,
            List<string> lstNiv, List<string> lstFreq, int iAlternatives, 
            List<string> lstExplicationsPrefixe,
            List<string> lstExplicationsSuffixe,
            ref string sQuizPrefixeSelect, ref string sQuizSuffixeSelect, ref string sMot0)
        {
            //  Quiz sur le préfixe et le suffixe correspondant à une définition

            bool bGrecoLatin = state.QuizGrecoLatin.Checked;
            bool bNeoRigolo = state.QuizNeologisme.Checked;

            // Les préfixes et suffixes des mots du dictionnaire sont plus nombreux
            //  ne prendre que ceux qui forme des mots potentiels plausibles
            const bool bComplet = false;
            
            int iNumPrefixe = basePrefixe.iTirageSegment(bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            LogotronLib.clsSegmentBase prefixe = null;
            if (!basePrefixe.bLireSegment(iNumPrefixe, ref prefixe)) return;

            string sNiveauP = prefixe.sNiveau;
            int iNumSuffixe = baseSuffixe.iTirageSegment(bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            LogotronLib.clsSegmentBase suffixe = null;
            if (!baseSuffixe.bLireSegment(iNumSuffixe, ref suffixe)) return;

            string sNiveauS = suffixe.sNiveau;
            string sPrefixe = prefixe.sSegment;
            string sSuffixe = suffixe.sSegment;
            string sPrefixeMaj = sPrefixe.ToUpper();
            string sSuffixeMaj = sSuffixe.ToUpper();
            string sSensPrefixe = prefixe.sSens;
            string sSensPrefixeMaj = sSensPrefixe.ToUpper();
            sSensPrefixeMaj = clsBase.sCompleterPrefixe(sSensPrefixeMaj);
            string sEtymPrefixe = prefixe.sEtym;
            string sSensSuffixe = suffixe.sSens;
            string sSensSuffixeMaj = sSensSuffixe.ToUpper();
            string sEtymSuffixe = suffixe.sEtym;
            string sMot = sPrefixeMaj + sSuffixeMaj;
            string sDetail = sPrefixeMaj + "(" + sNiveauP + ")-"
                + sSuffixeMaj + "(" + sNiveauS + ")";
            string sExplication = sSensSuffixeMaj + " " + sSensPrefixeMaj;
            sMot0 = sExplication;
            string sPrefixeMajT = sPrefixeMaj + "-";
            string sTSuffixeMaj = "-" + sSuffixeMaj;
            var lstEtym = new List<string>();
            if (sEtymPrefixe.Length > 0)
                lstEtym.Add(sPrefixe + "- : " + sEtymPrefixe);

            if (sEtymSuffixe.Length > 0)
                lstEtym.Add("-" + sSuffixe + " : " + sEtymSuffixe);

            //if (bDebugUnicite)
            //{
            //    if (prefixe.sUnicite.Length > 0)
            //        sPrefixeMajT += " (" + prefixe.sUnicite + ")";
            //    if ((suffixe.sUnicite.Length > 0))
            //        sTSuffixeMaj += " (" + suffixe.sUnicite + ")";
            //}

            //if (bDebugUniciteSynth)
            //{
            //    sPrefixeMajT += " (" + prefixe.sUniciteSynth + ")";
            //    sTSuffixeMaj += " (" + suffixe.sUniciteSynth + ")";
            //}

            clsInitTirage itPrefixe = new clsInitTirage(prefixe);
            var lstPrefixesMajT = new List<string>();
            lstPrefixesMajT.Add(sPrefixeMajT);
            clsInitTirage itSuffixe = new clsInitTirage(suffixe);
            var lstSuffixesTMaj = new List<string>();
            lstSuffixesTMaj.Add(sTSuffixeMaj);

            lstExplicationsPrefixe.Add(sPrefixeMajT);
            _quiz.sQuizPrefixeOk = sPrefixeMajT;
            lstExplicationsSuffixe.Add(sTSuffixeMaj);
            _quiz.sQuizSuffixeOk = sTSuffixeMaj;
            _quiz.sDetail = sDetail;
            _quiz.lstEtym = lstEtym;

            for (int j = 0; j <= iAlternatives - 1; j++)
            {
                int iNumPrefixeAutre = basePrefixe.iTirageSegment(
                    bComplet, lstNiv, lstFreq, itPrefixe, bGrecoLatin, bNeoRigolo);
                clsSegmentBase prefixeP2 = null;
                if (!basePrefixe.bLireSegment(iNumPrefixeAutre, ref prefixeP2)) break;

                int iNumSuffixeAutre = baseSuffixe.iTirageSegment(
                    bComplet, lstNiv, lstFreq, itSuffixe, bGrecoLatin, bNeoRigolo);
                clsSegmentBase suffixeS2 = null;
                if (!baseSuffixe.bLireSegment(iNumSuffixeAutre, ref suffixeS2)) break;

                string sPrefixeAutre = (prefixeP2.sSegment.ToUpper() + "-");
                string sSuffixeAutre = ("-" + suffixeS2.sSegment.ToUpper());
                //if (bDebugUnicite)
                //{
                //    if (prefixeP2.sUnicite.Length > 0)
                //        sPrefixeAutre += " (" + prefixeP2.sUnicite + "]";
                //    if (suffixeS2.sUnicite.Length > 0)
                //        sSuffixeAutre += " (" + suffixeS2.sUnicite + "]";
                //}
                //if (bDebugUniciteSynth)
                //{
                //    sPrefixeAutre += " (" + prefixeP2.sUniciteSynth + ")";
                //    sSuffixeAutre += " (" + suffixeS2.sUniciteSynth + ")";
                //}

                lstPrefixesMajT.Add(sPrefixeAutre);
                lstSuffixesTMaj.Add(sSuffixeAutre);
                lstExplicationsPrefixe.Add(sPrefixeAutre);
                lstExplicationsSuffixe.Add(sSuffixeAutre);
            }
            
            BrasserListAuHasard(lstExplicationsPrefixe);
            BrasserListAuHasard(lstExplicationsSuffixe);
            // Voir si on peut ne rien sélectionner plutôt
            sQuizPrefixeSelect = lstExplicationsPrefixe[0];
            sQuizSuffixeSelect = lstExplicationsSuffixe[0];

        }

        private void QuizDefinition(clsBase basePrefixe, clsBase baseSuffixe,
            List<string> lstNiv, List<string> lstFreq, int iAlternatives, 
            List<string> lstExplicationsPrefixe,
            List<string> lstExplicationsSuffixe,
            ref string sQuizPrefixeSelect, ref string sQuizSuffixeSelect, ref string sMot0)
        {
            //  Quiz sur la définition du préfixe et du suffixe

            bool bGrecoLatin = state.QuizGrecoLatin.Checked;
            bool bNeoRigolo = state.QuizNeologisme.Checked;

            // Les préfixes et suffixes des mots du dictionnaire sont plus nombreux
            //  ne prendre que ceux qui forme des mots potentiels plausibles
            bool bComplet = false;

            int iNumPrefixe = basePrefixe.iTirageSegment(bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            clsSegmentBase prefixe = null;
            if (!basePrefixe.bLireSegment(iNumPrefixe, ref prefixe)) return;

            string sNiveauP = prefixe.sNiveau;
            int iNumSuffixe = baseSuffixe.iTirageSegment(bComplet, lstNiv, lstFreq, bGrecoLatin, bNeoRigolo);
            clsSegmentBase suffixe = null;
            if (!baseSuffixe.bLireSegment(iNumSuffixe, ref suffixe)) return;

            string sNiveauS = suffixe.sNiveau;
            string sPrefixe = prefixe.sSegment;
            string sSuffixe = suffixe.sSegment;
            string sPrefixeMaj = sPrefixe.ToUpper();
            string sSuffixeMaj = sSuffixe.ToUpper();
            string sSensPrefixeMaj = prefixe.sSens.ToUpper();
            sSensPrefixeMaj = clsBase.sCompleterPrefixe(sSensPrefixeMaj);
            string sEtymPrefixe = prefixe.sEtym;
            string sSensSuffixeMaj = suffixe.sSens.ToUpper();
            string sEtymSuffixe = suffixe.sEtym;
            sMot0 = sPrefixeMaj + sSuffixeMaj;
            string sDetail = sPrefixeMaj + "(" + sNiveauP + ")-" + sSuffixeMaj + "(" +
                sNiveauS + ")";
            string sExplication = sSensSuffixeMaj + " " + sSensPrefixeMaj;
            var lstEtym = new List<string>();
            if (sEtymPrefixe.Length > 0) lstEtym.Add(sPrefixe + "- : " + sEtymPrefixe);

            if (sEtymSuffixe.Length > 0) lstEtym.Add("-" + sSuffixe + " : " + sEtymSuffixe);

            clsInitTirage itPrefixe = new clsInitTirage(prefixe);
            clsInitTirage itSuffixe = new clsInitTirage(suffixe);
            //if (bDebugUnicite)
            //{
            //    if (prefixe.sUnicite.Length > 0)
            //        sSensPrefixeMaj += " (" + prefixe.sUnicite + ")";
            //    if (suffixe.sUnicite.Length > 0)
            //        sSensSuffixeMaj += " (" + suffixe.sUnicite + ")";
            //}

            //if (clsConst.bDebug && string.IsNullOrEmpty(prefixe.sUniciteSynth))
            //    Debugger.Break();
            //if (clsConst.bDebug && string.IsNullOrEmpty(suffixe.sUniciteSynth))
            //    Debugger.Break();

            //if (bDebugUniciteSynth)
            //{
            //    sSensPrefixeMaj += " (" + prefixe.sUniciteSynth + ")";
            //    sSensSuffixeMaj += " (" + suffixe.sUniciteSynth + ")";
            //}

            lstExplicationsPrefixe.Add(sSensPrefixeMaj);
            _quiz.sQuizPrefixeOk = sSensPrefixeMaj;
            lstExplicationsSuffixe.Add(sSensSuffixeMaj);
            _quiz.sQuizSuffixeOk = sSensSuffixeMaj;
            _quiz.sDetail = sDetail;
            _quiz.lstEtym = lstEtym;

            for (int j = 0; j <= iAlternatives - 1; j++)
            {
                int iNumPrefixeAutre = basePrefixe.iTirageSegment(
                    bComplet, lstNiv, lstFreq, itPrefixe, bGrecoLatin, bNeoRigolo);
                clsSegmentBase prefixeP2 = null;
                if (!basePrefixe.bLireSegment(iNumPrefixeAutre, ref prefixeP2)) break;

                string sSensPrefixeAutre = prefixeP2.sSens.ToUpper();
                sSensPrefixeAutre = clsBase.sCompleterPrefixe(sSensPrefixeAutre);
                int iNumSuffixeAutre = baseSuffixe.iTirageSegment(
                    bComplet, lstNiv, lstFreq, itSuffixe, bGrecoLatin, bNeoRigolo);
                clsSegmentBase suffixeS2 = null;
                if (!baseSuffixe.bLireSegment(iNumSuffixeAutre, ref suffixeS2)) break;

                string sSensSuffixeAutre = suffixeS2.sSens.ToUpper();
                //if (bDebugUnicite)
                //{
                //    if (prefixeP2.sUnicite.Length > 0)
                //        sSensPrefixeAutre += " (" + prefixeP2.sUnicite + "]";
                //    if (suffixeS2.sUnicite.Length > 0)
                //        sSensSuffixeAutre += " (" + suffixeS2.sUnicite + "]";
                //}
                //if (bDebugUniciteSynth)
                //{
                //    sSensPrefixeAutre += " (" + prefixeP2.sUniciteSynth + ")";
                //    sSensSuffixeAutre += " (" + suffixeS2.sUniciteSynth + ")";
                //}

                lstExplicationsPrefixe.Add(sSensPrefixeAutre);
                lstExplicationsSuffixe.Add(sSensSuffixeAutre);
                //AfficherTexte("Sens préfixe autre n°" + (j+1) + " : " + sSensPrefixeAutre);
                //AfficherTexte("Sens suffixe autre n°" + (j+1) + " : " + sSensSuffixeAutre);
            }
            BrasserListAuHasard(lstExplicationsPrefixe);
            BrasserListAuHasard(lstExplicationsSuffixe);
            // Voir si on peut ne rien sélectionner plutôt
            sQuizPrefixeSelect = lstExplicationsPrefixe[0];
            sQuizSuffixeSelect = lstExplicationsSuffixe[0];
        }

        private void BrasserListAuHasard(List<string> lst)
        {
            int iNbElements = lst.Count;
            List<int> lstIndex = new List<int>();
            List<string> lstRnd = new List<string>();

            for (int i = 0; i <= iNbElements - 1; i++)
            {
 Recom:
                int iNumElement = clsUtil.iRandomiser(0, iNbElements - 1);
                if (lstIndex.Contains(iNumElement)) goto Recom;
                lstIndex.Add(iNumElement);
                lstRnd.Add(lst[iNumElement]);
            }
            lst.Clear();
            foreach (string sItem in lstRnd) lst.Add(sItem);
        }

        private void AfficherTexte(string sTxt)
        {
            if (LogotronLib.clsConst.bDebug) Console.WriteLine(sTxt);
        }

        private void AfficherListe(string sTitre, List<string> lst)
        {
            if (!LogotronLib.clsConst.bDebug) return;

            Console.WriteLine("Liste " + sTitre + " :");
            foreach (string sElem in lst) Console.WriteLine(sElem);
        }

        private void AfficherListe(string sTitre, NonNullList<string> lst)
        {
            if (!LogotronLib.clsConst.bDebug) return;

            Console.WriteLine("Liste " + sTitre + " :");
            foreach (string sElem in lst) Console.WriteLine(sElem);
        }

        //private static void RemplirListBoxAuHasard(ListBox lb, List<string> lst)
        //{

        //    int iNbElements = lst.Count;
        //    List<int> lstIndex = new List<int>();
        //    List<string> lstRnd = new List<string>();

        //    for (int i = 0; i <= iNbElements - 1; i++)
        //    {

        //        Recom:
        //        int iNumElement = Util.clsUtil.iRandomiser(0, iNbElements - 1);
        //        if (lstIndex.Contains(iNumElement)) goto Recom;

        //        lstIndex.Add(iNumElement);
        //        lstRnd.Add(lst[iNumElement]);
        //    }

        //    lb.Items.Clear();
        //    foreach (string sPref in lstRnd) lb.Items.Add(sPref);

        //}

        #endregion

        #region Tutoriel TextInput

        public ReactElement RenderTuto(
            FieldSetAttributes fsa,
            SelectList selectLstApp, Attributes selectLstAppAtt,
            AnchorAttributes aAttr, string sDoc)
        {
            var msgHist = new MessageHistory(state.MessageHistoryTuto, sHistorique);

            var msgEdAtt = new Attributes
            { ClassName = "MessageEditor", Style = Style.Margin(20).Padding(5) };

            var formIsInvalid =
                string.IsNullOrWhiteSpace(state.Message.Title) ||
                string.IsNullOrWhiteSpace(state.Message.Content);
            var msgEd = new MessageEditor(
                title: state.Message.Title,
                content: state.Message.Content,
                className: "MessageEditor",
                disabled: formIsInvalid,
                onChange: e => SetState(state.With(_ => _.Message, e)),
                onSave: async () =>
                {
                    var tid = new SavedMessageDetails(
                        _nextAvailableIdTuto++,
                        new MessageDetails(state.Message.Title, state.Message.Content));
                    SetState(state.With(_ => _.MessageHistoryTuto, value => value.Add(tid)));
                    await props.MessageApi.SaveMessage(state.Message);
                    SetState(state.With(_ => _.Message, new MessageDetails("", "")));
                }
            );

            return DOM.Div(
                new Attributes { Style = Style.Margin(iMargin).Padding(iPadding).FontSize(iFontSize) },
                DOM.A(aAttr, sDoc),
                DOM.H3(props.Label),
                DOM.Span(selectLstAppAtt, selectLstApp),
                DOM.Legend(null, "Tutoriel simple en Bridge.React"),
                DOM.Span(msgEdAtt, msgEd),
                DOM.Div(null, msgHist)
            );
            
        }

        #endregion

        #region Tutoriel ToDo App

        public ReactElement RenderToDo(FieldSetAttributes fsa,
           SelectList selectLstApp, Attributes selectLstAppAtt,
           AnchorAttributes aAttr, string sDoc)
        {
            //return DOM.FieldSet(fsa, DOM.Legend(null, state.sAppli));

            var iAtt = new InputAttributes
            {
                Value = state.InputValue,
                OnChange = e => SetState(
                    state.With(_ => _.InputValue, e.CurrentTarget.Value))
            };

            var bAtt = new ButtonAttributes
            {
                Disabled = string.IsNullOrWhiteSpace(state.InputValue),
                //OnClick = e => AppendTodo(state.InputValue)
                OnClick = async (e) =>
                {
                    AppendTodo(state.InputValue);
                    await props.MessageApi.SaveMessage(state.InputValue);
                    SetState(state.With(_ => _.InputValue, ""));
                }
            };
            var la = new Attributes { ClassName = "label" };

            return DOM.Div(
                new Attributes { Style = Style.Margin(iMargin).Padding(iPadding).FontSize(iFontSize) },
                DOM.A(aAttr, sDoc),
                DOM.H3(props.Label),
                DOM.Span(selectLstAppAtt, selectLstApp),
                DOM.Legend(null, "Tutoriel ToDo App en Bridge.React"),
                //DOM.Label("Add ToDo :"),
                DOM.Span(la, "Add ToDo : "), DOM.Input(iAtt),
                DOM.Button(bAtt, "Add"),
                DOM.Div(
                   state.Todos.Select((todo, index) => new TaskCard(
                       task: todo,
                       onChange: updatedTask => SetState(
                           state.With(_ => _.Todos, index, updatedTask)),
                       onRemove: () => SetState(
                           state.With(_ => _.Todos, value => value.RemoveAt(index)))
                   )))
                );
        }

        private void AppendTodo(string todoDescription)
        {
            SetState(state.With(
                _ => _.Todos,
                value => value.Add(
                    new TaskDetails(
                        description: todoDescription,
                        done: false,
                        id: _nextAvailableIdToDoApp++
                    )
                )
            ));
        }

        #endregion

        #region Démo ScrollTextBox

        public ReactElement RenderScrollTBDemo(FieldSetAttributes fsa,
           SelectList selectLstApp, Attributes selectLstAppAtt,
           AnchorAttributes aAttr, string sDoc)
        {
            //var selectSTBItems2 = new ItemApi(new List<string> {
            //    "Texte 1", "Texte 2", "Texte 3", "Texte 4", "Texte 5", "Texte 6", "Texte 7" });
            var selectSTBItems2 = new ItemApi(
                state.LstMsg.Map<string>(x => x.Message).ToDynamic()); //.Reverse()

            var selectSTBAtt2 = new Attributes { ClassName = "selectSTB2" };
            var selectSTB2 = new ScrollTextBox(
                className: new NonBlankTrimmedString("selectSTB2"),
                titre: new NonBlankTrimmedString("Exemple ScrollTextBox"),
                disabled: false,
                itemAPI: selectSTBItems2,
                inputValueSTB: state.InputValueSTB,
                onChangeSTB: e => SetState(state.With(_ => _.strSTBLigneSel, e)),
                onChange: e => SetState(state.With(_ => _.InputValueSTB, e)),
                onSave: async () =>
                {
                    AppendMsg(state.InputValueSTB.ToString());
                    await props.MessageApi.SaveMessage(state.InputValueSTB);
                    // Décommenter pour effacer l'entrée
                    //SetState(state.With(_ => _.InputValueSTB, ""));
                }
            );

            // Test SelectList Multiple
            var selectLst1Items = new ItemApi(new List<string> {
                "Option 1", "Option 2", "Option 3" });
            var selectLst1Att = new Attributes { ClassName = "selectLstApp" };
            var selectLst1 = new SelectList(
                className: new NonBlankTrimmedString("ListeAppli"),
                title: "Test SelectList multiple",
                multiple: true,
                itemAPI: selectLst1Items,
                checkBoxArray: state.CheckBoxArrayDemo,
                onChange: sItem => SetState(state.With(
                    _ => _.CheckBoxArrayDemo,
                    SelectList.SetItem(sItem, state.CheckBoxArrayDemo,
                        selectLst1Items.GetItemList())))
            );

            return DOM.Div(
                new Attributes { Style = Style.Margin(iMargin).Padding(iPadding).FontSize(iFontSize) },
                DOM.A(aAttr, sDoc),
                DOM.H3(props.Label),
                DOM.Span(selectLstAppAtt, selectLstApp),
                DOM.Legend(null, "Test composants ScrollTextBox et autres"),
                //DOM.Label("Description"),
                DOM.Span(selectSTBAtt2, selectSTB2),
                DOM.Span(selectLst1Att, selectLst1)
            );
        }

        private void AppendMsg(string sMsg)
        {

            //if (LogotronLib.clsConst.bDebug) Console.WriteLine("AppendMsg : " + sMsg);

            SetState(state.With(
                _ => _.LstMsg,
                value => value.Add(
                    new MgsDetails(
                        sMessage: sMsg,
                        id: _nextAvailableIdSTB++
                    )
                )
            ));

            //SetState(state.With(
            //    _ => _.LstMsg3,
            //    value.Add(
            //        new MgsDetails(
            //            sMessage: sMsg,
            //            id: _nextAvailableId2++
            //        )
            //    )
            //));

        }

        #endregion

    }
}
