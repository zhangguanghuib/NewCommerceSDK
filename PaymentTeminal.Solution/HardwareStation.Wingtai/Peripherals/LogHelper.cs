using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WTR.HWExt.Peripherals
{
    public class LogHelper
    {
        string fileName;
        

        public LogHelper(string _fileName, object _caller)
        {
            this.checkFolderAndFileName(_fileName, _caller);

           this.PurgingFolder(_fileName.Replace(System.IO.Path.GetFileName(_fileName),""));
        }

        private  void PurgingFolder(string folderpath)
        {

            if (System.IO.Directory.Exists(folderpath))
            {
                string[] subDir = System.IO.Directory.GetDirectories(folderpath);

                foreach (string eachSubDir in subDir)
                {
                    // Use static Path methods to extract only the file name from the path.
                    string[] arrDir = eachSubDir.Split('\\');

                    if (arrDir != null && arrDir.Length > 0)
                    {

                        string eachSubDirName = arrDir[arrDir.Length - 1];

                        if (Convert.ToInt32(eachSubDirName.Substring(0, 6)) <= Convert.ToInt32(DateTime.Today.AddMonths(-3).ToString("yyyyMM")))
                        {
                            try
                            {
                                System.IO.Directory.Delete(folderpath + @"\" + eachSubDirName, true);
                            }

                            catch (System.IO.IOException e)
                            {
                                saveLog(" Purging of folder:" + folderpath + @"\" + eachSubDirName + " is not success. [EXCEPTION] " + e.Message.ToString() +
                                 " [InnerException]: " +   (e.InnerException != null ? e.InnerException.ToString() : "") + "[Stacktrace] "+  e.StackTrace);
                            }
                        }
                    }

                }
            }
        }


        public void saveLog(string _stringToWrite)
        {           
            using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(DateTime.Now.ToString() + "(" + DateTime.Now.Millisecond.ToString() + ")" + " <<:>> " + _stringToWrite);
                sw.WriteLine("");
            }
        }

        private void checkFolderAndFileName(string _fileName, object _caller)
        {
            //get file name
            string fileNameWithoutExtLocal = Path.GetFileNameWithoutExtension(_fileName);
            //get file extension
            string fileExtensionLocal = Path.GetExtension(_fileName);
            //base folder path
            string baseFolderPath = Path.GetDirectoryName(_fileName);
            //working folder path
            string todayFolderPath = baseFolderPath + @"\" + DateTime.Today.ToString("yyyyMMdd");

            //check if directory exist if not then create it
            if (!Directory.Exists(baseFolderPath))
            {
                Directory.CreateDirectory(baseFolderPath);
            }
            
            //check if today's folder already created
            if (!Directory.Exists(todayFolderPath))
            {
                Directory.CreateDirectory(todayFolderPath);
            }

            //construct full file name
            fileName = todayFolderPath + @"\" + _caller.GetType().Name + @"_" + DateTime.Now.ToString("HHmmss") + fileExtensionLocal;
        }

        internal static void Error(Exception ex)
        {
            throw new NotImplementedException();
        }

        internal static void Info(string v)
        {
            throw new NotImplementedException();
        }
    }
}
