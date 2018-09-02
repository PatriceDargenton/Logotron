
using System; // Single
using Util; // clsUtil

namespace Bridge.React.Logotron.UtilWeb
{
    public static class clsUtilWeb
    {

        // En mode web, il faut passer par la méthode générale
        //  avec les réels, car la méthode via le
        //  format "n" (numérique général) ne marche pas

        public static string sFormaterNumeriqueWeb(int iVal)
        {
            
            Single rVal = (Single)iVal;
            string sVal = clsUtil.sFormaterNumerique(rVal, 
                bSupprimerPt0: true, iNbDecimales: 0);
            return sVal;
        }

        public static string sFormaterNumeriqueLongWeb(long lVal)
        {
            Single rVal = (Single)lVal;
            string sVal = clsUtil.sFormaterNumerique(rVal, 
                bSupprimerPt0: true, iNbDecimales: 0);
            return sVal;
        }
    }
}
