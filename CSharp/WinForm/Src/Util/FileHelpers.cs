
using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;
using System.Diagnostics; // For Process in startAssociateApp()

using UtilMsg = UtilWinForm.clsMessageUtil;

namespace UtilWinForm
{
    public static class FileHelpers // ToDo : Français ?
    {
        //const string sCrLf = "\r\n";
        public static readonly string sCrLf = Environment.NewLine;

        const string sPossibleErrCause = "The file may be write-protected or locked by another software";

        public static bool FileExistsPrompt(string sFilePath)
        {
            if (!File.Exists(sFilePath))
            { UtilMsg.ShowErrorMsg("Can't find file: " + sFilePath); return false; }
            return true;
        }

        public static bool DeleteFile(string sFilePath, bool bPromptIfErr = false)
        {
            if (!File.Exists(sFilePath)) return true;

            // ToDo
            //if (bFileLocked(sFilePath, bPromptClose: bPromptIfErr, bPromptRetry: bPromptIfErr))
            //    return false;

            try
            {
                File.Delete(sFilePath);
                return true;
            }
            catch (Exception ex)
            {
                if (bPromptIfErr) UtilMsg.ShowErrorMsg(ex, "DeleteFile",
                    "Can't delete file: " + System.IO.Path.GetFileName(sFilePath) + sCrLf + sFilePath,
                    sPossibleErrCause);
                return false;
            }
        }

        public static bool DeleteFiles(string sPath, string sFilter)
        {
            // Supprimer tous les fichiers correspondants au filtre, par exemple : C:\ avec *.txt
            // Si le dossier n'existe pas, on considère que c'est un succès
            if (!Directory.Exists(sPath)) return true;
            foreach (string sFile in Directory.GetFileSystemEntries(sPath, sFilter))
            {
                if (Directory.Exists(sFile)) continue; // Ne pas supprimer les sous-dossiers
                if (!DeleteFile(sFile)) return false;
            }
            return true;
        }

        public static bool MoveFile(string sSrc, string sDest, bool bPreserveDest = false)
        {
	        // Renommer ou déplacer un et un seul fichier

            if (!FileExistsPrompt(sSrc)) return false;

	        if (bPreserveDest) {
		        // Cette option permet de conserver le fichier de destination s'il existe
                if (File.Exists(sDest)) {
			        // Dans ce cas on supprime la source
                    if (!DeleteFile(sSrc, bPromptIfErr: true)) return false;
			        return true;
		        }
	        } else {if (!DeleteFile(sDest, bPromptIfErr: true)) return false; }

	        try {
		        System.IO.File.Move(sSrc, sDest);
		        return true;
	        } catch (Exception ex) {
                UtilMsg.ShowErrorMsg(ex, "MoveFile",
                    "Can't move file from:" + sCrLf +
                    Path.GetDirectoryName(sSrc) + sCrLf + Path.GetFileName(sSrc) + sCrLf +
                    "to:" + sCrLf + Path.GetDirectoryName(sDest) + sCrLf + Path.GetFileName(sDest),
                    sPossibleErrCause);
                    //"Can't move file from:" + sCrLf + sSrc + sCrLf +
                    //"to:" + sCrLf + sDest, sPossibleErrCause);
		        return false;
	        }
        }

        public static StringBuilder sbReadFile(string sFilePath, Encoding encod, bool bLectureSeule = false)
        {
            // Lire et renvoyer le contenu d'un fichier
            
            if (!FileExistsPrompt(sFilePath)) return null;

            var sb = new StringBuilder();
            FileStream fs = null;
            try
            {
                // Si Excel a verrouillé le fichier, une simple ouverture en lecture
                //  est permise à condition de préciser aussi IO.FileShare.ReadWrite
                FileShare share = FileShare.Read; // Valeur par défaut
                if (bLectureSeule) share = FileShare.ReadWrite;
                fs = new FileStream(sFilePath, FileMode.Open, FileAccess.Read, share);
                // Encoding.UTF8 fonctionne dans le bloc-notes, mais pas avec Excel via csv
                using (StreamReader sr = new StreamReader(fs, encod))
                {
                    fs = null; // CA2202
                    bool bDebut = false;
                    do
                    {
                        string sLigne = sr.ReadLine();
                        if (sLigne == null) break;
                        if (bDebut) sb.Append(Environment.NewLine);
                        bDebut = true;
                        sb.Append(sLigne);
                    } while (true);
                }
                return sb;
            }
            catch (Exception ex)
            { 
                UtilMsg.ShowErrorMsg(ex, "sbReadFile"); 
                return null; 
            }
            finally
            {
                if (fs != null) fs.Dispose(); // CA2202
            }
        }

        public static string[] asReadFile(string sFilePath, Encoding encod) //, bool bLectureSeule = false)
        {
            // Lire et renvoyer le contenu d'un fichier

            if (!FileExistsPrompt(sFilePath)) return null;

            try
            {
                return System.IO.File.ReadAllLines(sFilePath, encod);
            }
            catch (Exception ex)
            { UtilMsg.ShowErrorMsg(ex, "asReadFile"); return null; }
        }

        public static bool WriteFile(string sFilePath, StringBuilder sbContent, bool bDefaultEncoding = true,
            Encoding encode = null, bool bPromptIfErr = true)
        {
            if (!DeleteFile(sFilePath, bPromptIfErr: true)) return false;

            //StreamWriter sw = null;
            try
            {
                if (bDefaultEncoding) encode = Encoding.Default;
                bool bAppend = false;
                //sw = new StreamWriter(sFilePath, bAppend, encode);
                //sw.Write(sbContent.ToString());
                //sw.Close();
                using (StreamWriter sw = new StreamWriter(sFilePath, bAppend, encode))
                { sw.Write(sbContent.ToString()); }
                return true;
            }
            catch (Exception ex)
            {
                string sMsg = "Can't write file : " + System.IO.Path.GetFileName(sFilePath) +
                    sCrLf + sFilePath + sCrLf + sPossibleErrCause;
                if (bPromptIfErr) UtilMsg.ShowErrorMsg(ex, "WriteFile", sMsg);
                return false;
            }
            //finally { if ((sw != null)) sw.Close(); }
        }
        
        public static bool WriteFile(string sFilePath, StringBuilder sbContent, out string sMsgErr,
            bool bDefaultEncoding = true, Encoding encode = null, bool bPromptIfErr = true)
        {
            sMsgErr = "";

            if (!DeleteFile(sFilePath, bPromptIfErr: true)) return false;

            try
            {
                if (bDefaultEncoding) encode = Encoding.Default;
                bool bAppend = false;
                using (StreamWriter sw = new StreamWriter(sFilePath, bAppend, encode))
                { sw.Write(sbContent.ToString()); }
                return true;
            }
            catch (Exception ex)
            {
                string sMsg = "Can't write file : " + System.IO.Path.GetFileName(sFilePath) +
                    sCrLf + sFilePath + sCrLf + sPossibleErrCause;
                sMsgErr = sMsg + sCrLf + ex.Message;
                if (bPromptIfErr) UtilMsg.ShowErrorMsg(ex, out sMsgErr, "WriteFile", sMsg);
                return false;
            }
        }

        public static bool DirectoryExistsPrompt(string sPath)
        {
            if (!Directory.Exists(sPath))
            { UtilMsg.ShowErrorMsg("Can't find directory: " + sPath); return false; }
            return true;
        }

        public static void startAssociateApp(string sFilePath, bool bMaximized = false,
            bool bCheckFile = true, bool bWait = false, string sArguments = "")
        {
            // Don't check file if it is a URL to browse
            if (bCheckFile) { if (!FileExistsPrompt(sFilePath)) return; }
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(sFilePath);
            p.StartInfo.Arguments = sArguments;
            if (bMaximized)
                p.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            p.Start();
            if (bWait) p.WaitForExit();
        }
    }
}
