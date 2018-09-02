
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms; // Pour Form et ListBox
using System.Diagnostics; // Pour Debugger.Break();

namespace UtilWinForm
{
    public static class clsUtil
    {

        public const string sCarSautDeLigne = "↲";

        public static void AfficherTexteListBox(string sTxtOrig,
            ref int iIndexTxtLb, ListBox lb)
        {
            //if (frm == null) throw new ArgumentNullException("frm");
            if (lb == null) throw new ArgumentNullException("lb");
            //if (string.IsNullOrEmpty(sTxtOrig)) goto Fin;
            lb.Items.Add(sTxtOrig);
            lb.SelectedIndex = iIndexTxtLb;
            iIndexTxtLb++;
        }

        public static void AfficherTexteListBox(string sTxtOrig,
            ref int iIndexTxtLb, Form frm, ListBox lb)
        {
            if (frm == null) throw new ArgumentNullException("frm");
            if (lb == null) throw new ArgumentNullException("lb");
            if (string.IsNullOrEmpty(sTxtOrig)) goto Fin;

            System.Drawing.Graphics graphics = frm.CreateGraphics();
            float rLargeurTxtOrig =
                graphics.MeasureString(sTxtOrig, lb.Font).Width * 1.04f;
            float rLargeurDispo = (float)lb.Width;
            float rDiv = (float)lb.Width / rLargeurTxtOrig;
            if (rDiv < 1f)
            {
                int iLongTot = sTxtOrig.Length + 1;
                float rNbTroncons = rLargeurTxtOrig / (float)lb.Width;
                int iLongMoyTroncon = (int)Math.Round(
                    Math.Ceiling((double)((float)iLongTot / rNbTroncons)));
                int iTxtAff = 0;
                string sTxtFinVerif = "";
                int iNumTroncon = 0;
                while (true)
                {
                    int iNbCarEnTrop = 1;
                    int iLongTroncon = iLongMoyTroncon;
                    string sTxtTroncon2 = "";
                    string sTxtTronconVerif2 = "";
                    bool bFin = false;
                    while (true)
                    {
                        bool bAjoutCarSautDeLigne = true;
                        int iLongRest = iLongTroncon - iNbCarEnTrop;
                        if (iLongRest + iTxtAff >= iLongTot)
                        {
                            iLongRest = iLongTot - iTxtAff - 1;
                            bFin = true;
                            bAjoutCarSautDeLigne = false;
                        }
                        sTxtTroncon2 = sTxtOrig.Substring(iTxtAff, iLongRest);
                        sTxtTronconVerif2 = sTxtTroncon2;
                        if (bAjoutCarSautDeLigne) sTxtTroncon2 += sCarSautDeLigne; 
                        float rSubTxtLarg0 =
                            graphics.MeasureString(sTxtTroncon2, lb.Font).Width * 1.04f;
                        if (!(rSubTxtLarg0 <= rLargeurDispo))
                        {
                            iNbCarEnTrop++;
                            if (iNbCarEnTrop <= iLongTroncon && true)
                                continue;
                        }
                        break;
                    }
                    lb.Items.Add(sTxtTroncon2);
                    sTxtFinVerif += sTxtTronconVerif2;
                    iIndexTxtLb++;
                    iTxtAff += iLongTroncon - iNbCarEnTrop;
                    iNumTroncon++;
                    if (((!bFin && sTxtFinVerif.Length < iLongTot) || 1 == 0) && true)
                        continue;
                    break;
                }
                lb.SelectedIndex = iIndexTxtLb - 1;
                //if (sTxtOrig != sTxtFinVerif && clsConst.bDebug)
                //    Debugger.Break();
                return;
            }

            Fin:
            lb.Items.Add(sTxtOrig);
            lb.SelectedIndex = iIndexTxtLb;
            iIndexTxtLb++;
        }
    }
}
