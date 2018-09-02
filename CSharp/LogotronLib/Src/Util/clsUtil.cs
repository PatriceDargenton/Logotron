
using System;
using System.Text;

namespace Util
{
    public static class clsAppUtil
    {
        public static string AppTitle { get; set; }
    }

    public static class clsUtil
    {

        public const string sCarSautDeLigne = "↲";

        static int m_iCompteurRnd;
        static Random m_rRndGenerateur;
        public static int iRandomiser(int iMin, int iMax)
        {
            if (iMin == iMax) return iMin;

            if (m_rRndGenerateur ==null) m_rRndGenerateur = new Random();
            double rRndDouble = m_rRndGenerateur.NextDouble();
            double rVal = iMin + rRndDouble * (iMax + 1 - iMin);
            //int iRes = (int)(Math.Ceiling(rVal));
            int iRes = iFix(rVal);
            // Au cas où Rnd() renverrait 1.0 et qq
            if (iRes > iMax) iRes = iMax;

            m_iCompteurRnd += 1;
            //if (LogotronLib.clsConst.bDebug)
            //    Console.WriteLine("iRandomiser : " + m_iCompteurRnd + 
            //        " : [" + iMin + ", " + iMax + "] -> " + iRes);

            return iRes;
        }

        public static int iFix(double rVal)
        {
            // Fix : Partie entière sans arrondir à l'entier le plus proche
            // Pour les nombres négatifs, on enlève la partie décimale aussi
            // Floor   arrondi les négatifs à l'entier le plus petit, tandis que
            // Ceiling arrondi les négatifs à l'entier le plus grand (le plus petit en valeur absolu)
            // Fix arrondi toujours à l'entier le plus petit en valeur absolu
            if (rVal > 0) return Convert.ToInt32(Math.Floor(rVal));
            return Convert.ToInt32(Math.Ceiling(rVal));
        }

        public static float rRandomiser()
        {
            if (m_rRndGenerateur == null) m_rRndGenerateur = new Random();
            double rRndDouble = m_rRndGenerateur.NextDouble();
            float rRes = (float)(rRndDouble);
            return rRes;
        }
        
        public static string sFormaterNumerique(float rVal,
            bool bSupprimerPt0 = true, int iNbDecimales = 1)
        {
            // Formater un numérique avec une précision d'une décimale

            // Le format numérique standard est correct(séparation des milliers et plus), 
            //  il suffit juste d'enlever la décimale inutile si 0

            // NumberGroupSeparator: Séparateur des milliers, millions...
            // NumberDecimalSeparator : Séparateur décimal
            // NumberGroupSizes: 3 groupes pour milliard, million et millier
            //  (on pourrait en ajouter un 4ème pour les To: 1000 Go)
            // NumberDecimalDigits: 1 décimale de précision

            System.Globalization.NumberFormatInfo nfi =
                new System.Globalization.NumberFormatInfo()
                {
                    NumberGroupSeparator = " ",
                    NumberDecimalSeparator = ".",
                    NumberGroupSizes = new int[] { 3, 3, 3 },
                    NumberDecimalDigits = iNbDecimales
                };

            string sFormatage = rVal.ToString("n", nfi); // n : numérique général

            // Enlever la décimale si 0
            if (bSupprimerPt0)
            {
                if (iNbDecimales == 1)
                    sFormatage = sFormatage.Replace(".0", "");
                else if (iNbDecimales > 1)
                {
                    int i;
                    StringBuilder sb = new StringBuilder(".");
                    for (i = 1; i <= iNbDecimales; i++) sb.Append("0");
                    sFormatage = sFormatage.Replace(sb.ToString(), "");
                }
            }
            return sFormatage;
        }

        // Formater un numérique selon le format choisi dans le panneau de config.
        // Le format numérique standard "n" est correct (séparation des milliers et plus), 
        //  il suffit juste d'enlever la décimale inutile si 0
        // Vérifier , et . :

        public static string sFormaterNumerique(int iVal)
        {
            string sVal = iVal.ToString("n"); // n : numérique général
            string sVal2 = sVal.Replace(",00", "").Replace(".00", "");
            return sVal2;
        }

        public static string sFormaterNumeriqueLong(long lVal)
        {
            string sVal = lVal.ToString("n"); // n : numérique général
            string sVal2 = sVal.Replace(",00", "").Replace(".00", "");
            return sVal2;
        }
    }
}
