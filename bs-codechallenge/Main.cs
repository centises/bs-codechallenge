///-------------------------------------------------------------------------------------------------
/// Project:    bs-codechallenge
/// Datei:	    Main.cs
///
/// Task:	    Hilfsklasse für STC0710-Interface. (Um die Übersichtlichkeit zu steigern.) Hier
///             werden Funktionen implementiert, die auf möglichst wenige Ressourcen des Interfaces
///             zugreifen.
///
///			Date        Developer		Modifications
///			2020/03/01	R K			    Started development. Introduced main() and csv path checking 
///			                            function.
///			
///-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace bs_codechallenge
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   XXX efniwef </summary>
    ///
    /// <remarks>   R K, 2020/03/01. </remarks>
    ///-------------------------------------------------------------------------------------------------
    class Main
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Main function. </summary>
        /// 
        /// <remarks>   R K, 2020/03/01 </remarks>
        ///-------------------------------------------------------------------------------------------------
        static void main(string[] args)
        {
            String inputPath = @"./test.csv";

            if (args.Length > 1)
            {
                inputPath = args[0];
            }

            String content = File.ReadAllText(inputPath);

            Console.Write(content);
            int a = Console.Read();
            return;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Main function. </summary>
        /// 
        /// <remarks>   R K, 2020/03/01 </remarks>
        ///-------------------------------------------------------------------------------------------------
        private static bool checkCsvFileOrDirectory(String path)
        {
            if (File.Exists(path))
            {
                String fileEnding = path.Substring(path.Length - 3, 3);
                if (fileEnding.ToLower() == "csv")
                {
                    return true;
                }
            }
            else if (Directory.Exists(path))
            {
                String[] filePaths = Directory.GetFiles(path);
                foreach (String filePath in filePaths)
                {
                    String fileEnding = filePath.Substring(path.Length - 3, 3);
                    if (fileEnding.ToLower() == "csv")
                    {
                        return true;
                    }
                }
            }

            return false;

            //FileAttributes pathAttributes = File.GetAttributes(path);

            //Check if path leads to either a CSV file or a directory with CSV files
            //if (pathAttributes.HasFlag(FileAttributes.Directory))
            //{

            //}
            //else
            //{
            //    String fileEnding = path.Substring(path.Length - 3, 3);
            //    if (fileEnding.ToLower() == "csv")
            //    {
            //        return true;
            //    }
            //}
        }
    }
}
