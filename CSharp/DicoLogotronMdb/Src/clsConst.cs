
using System; // Environment

namespace DicoLogotronMdb
{
    public static class clsConstMdb
    {
        public const string sNomAppli = "LogotronMdb";
        public const string sDateAppli = "03/05/2020";
        public const string sVersionAppli = "1.05";
        public const string sBaseLogotron = "Logotron";
        public const string sBaseLogotronVide = "LogotronVide";

#if DEBUG
        public static bool bDebug = true;
        public static bool bDebugDB = false;
        public static bool bRelease = false;
#else
        public static bool bDebug = false;
        public static bool bDebugDB = false;
        public static bool bRelease = true;
#endif

        public static readonly string sCrLf = Environment.NewLine; // "\r\n"
        public const int iMaxCar255 = 255;
        public const string sLang = enumLangue.Fr;
        //public const string sLang = enumLangue.En;
        public const string sExtMdb = ".mdb";
        public const string sExtLdb = ".ldb";
    }
    
    static class enumLangue
    {
        public const string Fr = "_fr"; // Français
        public const string En = "_en"; // English
    }
}
