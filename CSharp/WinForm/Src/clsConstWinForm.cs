
using System;

namespace Logotron
{
    static class clsConstWinForm
    {

        public const string sNomAppli = "Logotron";
        public const string sDateAppli = "14/09/2018";
        public const string sVersionAppli = "1.02";

#if DEBUG
        public static bool bDebug = true;
        public static bool bRelease = false;
#else
        public static bool bDebug = false;
        public static bool bRelease = true;
#endif

        public const string sLang = enumLangue.Fr; 
        //public const string sModeLecture = enumModeLecture.Code; // Seul dispo.
        
        // Pour la version WinForm, on peut choisir le csv ou le code
        //public const string sModeLectureMotsExistants = enumModeLectureMotExistant.Code;
        public const string sModeLectureMotsExistants = enumModeLectureMotExistant.Csv;

        //public const string sCarSautDeLigne = "↲";
        public static readonly string sCrLf = Environment.NewLine; // "\r\n"

    }
    
    static class enumLangue
    {
        public const string Fr = "_fr"; // Français
        public const string En = "_en"; // English
    }

    //static class enumModeLecture
    //{
    //    //public const string Csv = "Csv"; Pas encore fait
    //    public const string Code = "Code"; Fait
    //    //public const string JSon = "JSon"; Pas encore fait
    //}

    static class enumModeLectureMotExistant
    {
        public const string Csv = "Csv"; 
        public const string Code = "Code";
        //public const string JSon = "JSon"; Pas encore fait
    }
}
