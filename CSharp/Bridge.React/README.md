**Le Logotron : jouer avec les préfixes et les suffixes de la langue
française**

[*https://github.com/PatriceDargenton/Logotron*](https://github.com/PatriceDargenton/Logotron)

*Documentation* :
[*Logotron.html*](http://patrice.dargenton.free.fr/CodesSources/Logotron/index.html)
[*.epub*](http://patrice.dargenton.free.fr/CodesSources/Logotron/Logotron.epub)
[*.mobi*](http://patrice.dargenton.free.fr/CodesSources/Logotron/Logotron.mobi)
[*.pdf*](http://patrice.dargenton.free.fr/CodesSources/Logotron/Logotron.pdf)

*Application en ligne* :
[*Logotron/App*](http://patrice.dargenton.free.fr/CodesSources/Logotron/App/)

*Code source* :
[*Logotron.vbproj.html*](http://patrice.dargenton.free.fr/CodesSources/Logotron/Logotron.vbproj.html)

Par Patrice Dargenton :
[*patrice.dargenton@free.fr*](mailto:patrice.dargenton@free.fr)

[*http://patrice.dargenton.free.fr/CodesSources/index.html*](http://patrice.dargenton.free.fr/CodesSources/index.html)

Version 1.07 du 04/08/2020

Le Logotron en Bridge React
===========================

La [*version en ligne du
Logotron*](http://patrice.dargenton.free.fr/CodesSources/Logotron/App/)
est codée en Bridge React, avec Visual Studio 2017.
[*React*](https://fr.wikipedia.org/wiki/React_(JavaScript)) est le
langage de développement popularisé par Facebook pour concevoir des
applications web monopage. [*Bridge*](https://bridge.net/) est une
technologie permettant de convertir du code C\# en JavaScript, l'idée
étant de récupérer les compétences acquises en [*DotNet pour programmer
pour le web*](http://object.net/). Il existe déjà [*des
milliers*](https://retyped.com/) de librairies JavaScript adaptées en
.Net.

Je suis parti de l'excellent [*tutoriel en trois parties de Dan
Roberts*](https://www.productiverage.com/writing-react-apps-using-bridgenet-the-dan-way-from-first-principles),
et j'ai mixé mon code avec le [*tutoriel
officiel*](https://github.com/ProductiveRage/Bridge.React) afin de
pouvoir bénéficier de toutes les options de débogage en C\# dans le
navigateur : pause, pause sur exception, pas à pas, état des variables,
de la pile d'appel, ... (options qui ne marchaient pas dans le tuto de
départ)

J'ai fait une version Release et une version Debug, car en mode Release
ça prend du temps à compiler (il faut cependant compiler une première fois en mode Release pour activer la conversion du code en Javascript). En effet, comme je n'ai pas réussi à lire
un fichier csv ou json en Bridge React, j'ai inséré directement la liste
des mots du dictionnaire pour le quiz via le code, la liste étant
générée depuis la version en VB .Net.

Si on met à jour toutes les librairies NuGet, ou partiellement, on
obtient des erreurs, soit à la compilation, soit à l'exécution, selon
les cas. Pour le moment, on ne peut donc pas utiliser les dernières
librairies disponibles.

Si le fichier JavaScript n'est plus généré à l'issue de la compilation,
c'est parce que le fichier
CSharp\\Bridge.React\\Bridge.React\\bridge.json a été remplacé lors de
l'installation d'une mise à jour, et que la ligne suivante ne fonctionne
pas : "output": "\$(OutDir)/bridge/",

Il faut rétablir le json qui marche dans ce cas : "output": "js",

J'ai pas mal galéré concernant le cycle de vie des variables selon le
paradigme de React, lequel consiste à virtualiser le
[*DOM*](https://fr.wikipedia.org/wiki/Document_Object_Model) du
navigateur, je ne garantis pas que mon code suit l'état de l'art de la
programmation en React, mais j'ai suivi le plus possible les
recommandations des tutoriels, et cela fonctionne plutôt pas mal (si on
clique trop vite sur le quiz, les préfixes et suffixes s'ajoutent dans
les listes si on augmente un peu la temporisation de la tâche
d'attente !). Toutefois, je n'en suis pas encore à être complètement
satisfait du code : par exemple, je n'ai pas réussi à faire un composant
[*ScrollTextBox*](http://patrice.dargenton.free.fr/CodesSources/Logotron/App/Debug/)
valable en Bridge React, alors que le [*même
composant*](http://patrice.dargenton.free.fr/CodesSources/Logotron/App1/)
réalisé en React standard par un développeur compétent n'a pris que
quelques minutes à le réaliser, et il est parfait (excepté que sur iOS,
la sélection du dernier mot ne fonctionne pas). Et je n'ai pas réussi
non plus à enchainer la question suivante du quiz lorsque le choix est
correct ! Mais bon, c'est ma première application web, et c'est déjà pas
mal. Pour le moment, l'intérêt de faire du .Net pour le web n'est pas
flagrant, sauf pour la partie récupération du code métier en dehors de
l'interface (ici le moteur du Logotron). Par contre, pour la partie
interface, ce n'est pas franchement plus simple de coder en C\#, et il
est difficile de trouver de l'aide sur le web, car c'est une technologie
avant-gardiste pour le moment. Mais l'avantage c'est que le typage fort
du C\# va quand même beaucoup plus loin que celui de
[*TypeScript*](https://fr.wikipedia.org/wiki/TypeScript) : au lieu
d'ajouter une surcouche de typage à JavaScript, autant ajouter une étape
de
[*transpilation*](https://fr.wikipedia.org/wiki/Compilateur_source_%C3%A0_source)
de C\# vers JavaScript, c'est quand même plus classe, surtout pour
détecter un maximum de problème lors de la compilation, et non lors de
l'exécution. Il me reste à tester d'autres librairies pour voir tout ce
qu'on peut faire avec cette techno Bridge.