//-------------------------------------------------------------------------------------------------
// Project:    bs-codechallenge
// File:       Main.cs
//
// Task:        Transforms one or more given CSV files into an SQL data base.
//
//     Date        Developer		Modifications
//		2020/03/01	R K			    Started development. Introduced Main() and csv path checking 
//			                        function.
//      2020/03/03  R K             Finished csv path checking and started with file parsing
//      2020/03/04  R K             Continued with file parsing and started with SQL commands
//
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
//using Microsoft.VisualBasic.FileIO;

using System.IO;

namespace bs_codechallenge
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Class for main program parts. Only class so far. </summary>
    ///
    /// <remarks>   R K, 2020/03/01. </remarks>
    ///-------------------------------------------------------------------------------------------------
    class Program
    {
        private static SqlConnectionStringBuilder sqlConString;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Main function. </summary>
        /// 
        /// <remarks>   R K, 2020/03/01 </remarks>
        ///
        /// <param name="args">    Command line input for the function. </param>
        ///-------------------------------------------------------------------------------------------------
        public static void Main(string[] args)
        {
            // Initialize SQL connection string
            sqlConString = new SqlConnectionStringBuilder
            {
                DataSource = "localhost",
                UserID = "SA",
                Password = "k28nJTvof3UTL5E",
                InitialCatalog = "MainDB"
            };

            // Set default input path
            String inputPath = @"../../test2.csv";

            if (args.Length > 1)
            {
                inputPath = args[0];
            }

            if (PathContainsCsv(inputPath))
            {
                ConvertCsvToSql(inputPath);
            }
            else
            {
                Console.WriteLine("ERROR: The specified path does not lead to (a) CSV file(s). Please enter a valid path!");
            }

            return;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Function for checking if a specified <paramref name="path"/> leads to either a CSV 
        ///             file or a directory with one or more CSV files. </summary>
        /// 
        /// <remarks>   R K, 2020/03/01 </remarks>
        /// 
        /// <param name="path">    Path in question. </param>
        ///
        /// <returns>   Returns <see langword="true"/> if <paramref name="path"/> leads to CSV file(s), 
        ///             returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        private static bool PathContainsCsv(String path)
        {
            if (File.Exists(path) && IsCsvFile(path))
            {
                return true;
            }
            else if (Directory.Exists(path))
            {
                String[] filePaths = Directory.GetFiles(path);
                foreach (String filePath in filePaths)
                {
                    if (IsCsvFile(filePath))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Function for checking if a file, specified by <paramref name="path"/> is a CSV file. 
        ///             </summary>
        /// 
        /// <remarks>   R K, 2020/03/03 </remarks>
        /// 
        /// <param name="path">    Path leading to the file in question. </param>
        ///
        /// <returns>   Returns <see langword="true"/> if <paramref name="path"/> leads to CSV file, 
        ///             returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        private static bool IsCsvFile(String path)
        {
            String fileEnding = path.Substring(path.Length - 3, 3);
            if (fileEnding.ToLower() == "csv")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   XXX Converts a given CSV file into an SQL data table. </summary>
        /// 
        /// <remarks>   R K, 2020/03/03 </remarks>
        /// 
        /// <param name="csvPath">    Path leading to the CSV file. </param>
        ///-----------------------------------------------  --------------------------------------------------
        private static void ConvertCsvToSql(String csvPath)
        {
            String tableName = csvPath.Split('/').ToArray().Last();
            tableName = tableName.Split('.')[0].ToUpper();
            // Console.WriteLine("Name: " + tableName + "\r\n");

            using (StreamReader csvFile = new StreamReader(csvPath))
            {
                String columnLine = csvFile.ReadLine();
                String[] columns = columnLine.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                // Abort if data table cannot be created
                if (!CreateSqlTable(tableName, columns))
                {
                    Console.WriteLine("ERROR: Could not create SQL data table.");
                }

                while(!csvFile.EndOfStream)
                {
                    String newLine = csvFile.ReadLine();
                    String[] newRecord = newLine.Split(new char[] { ';' });
                    for (int i = 0; i < newRecord.Length; i++)
                    {
                        if (newRecord[i] == "")
                        {
                            newRecord[i] = "NULL";
                        }
                    }

                    if (!InsertRecord(tableName, columns, newRecord))
                    {
                        Console.WriteLine("ERROR: Could not insert record {0}.", newRecord[0]);
                    }
                }
            }
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   XXX Creates an SQL data table based on the column descriptions given in 
        ///             <paramref name="columns"/>. </summary>
        /// 
        /// <remarks>   R K, 2020/03/03 </remarks>
        /// 
        /// <param name="tableName">    Path leading to the CSV file. </param>
        /// <param name="columns">    Path leading to the CSV file. </param>
        /// <returns>   Returns <see langword="true"/> if table has been created successfully, returns 
        ///             <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        private static bool CreateSqlTable(String tableName, String[] columns)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(sqlConString.ConnectionString))
                {
                    dbConnection.Open();

                    StringBuilder sqlCmdText = new StringBuilder("CREATE TABLE " + tableName + " (");
                    foreach (String column in columns)
                    {
                        sqlCmdText.Append(column.Replace("$$", " ") + ", ");
                    }
                    sqlCmdText.Remove(sqlCmdText.Length - 2, 2);
                    sqlCmdText.Append(");");

                    Console.WriteLine(sqlCmdText.ToString());

                    using (SqlCommand sqlCmd = new SqlCommand(sqlCmdText.ToString(), dbConnection))
                    {
                        sqlCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }

            return true;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   XXX. </summary>
        /// 
        /// <remarks>   R K, 2020/03/03 </remarks>
        /// 
        /// <param name="data">    Path leading to the CSV file. </param>
        /// 
        /// <returns>   Returns <see langword="true"/> if record has been inserted successfully, returns 
        ///             <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        private static bool InsertRecord(String tableName, String[] columns, String[] data)
        {
            //try
            //{
            using (SqlConnection dbConnection = new SqlConnection(sqlConString.ConnectionString))
            {
                dbConnection.Open();

                //Console.WriteLine("SET IDENTITY_INSERT " + tableName + " ON;");

                //// Activate identity insert for table
                //using (SqlCommand sqlCmd = new SqlCommand("SET IDENTITY_INSERT " + tableName + " ON;", dbConnection))
                //{
                //    sqlCmd.ExecuteNonQuery();
                //}

                StringBuilder sqlCmdText = new StringBuilder("INSERT INTO " + tableName + " VALUES (");
                //StringBuilder sqlCmdText = new StringBuilder("INSERT INTO " + tableName + " (");
                //foreach (String column in columns)
                //{
                //    sqlCmdText.Append(column.Split(new String[] { "$$" }, StringSplitOptions.None).First() + ", ");
                //}

                //sqlCmdText.Remove(sqlCmdText.Length - 2, 2);
                //sqlCmdText.Append(") VALUES (");

                for (int i = 1; i < data.Length; i++)
                {
                    if (columns[i].ToUpper().Contains("CHAR"))
                    {
                        sqlCmdText.Append(@"'" + data[i] + @"', ");
                    }
                    else
                    {
                        sqlCmdText.Append(data[i] + ", ");
                    }
                    //sqlCmdText.Append(data[i] + ", ");
                }
                sqlCmdText.Remove(sqlCmdText.Length - 2, 2);
                sqlCmdText.Append(");");

                Console.WriteLine(sqlCmdText.ToString());

                using (SqlCommand sqlCmd = new SqlCommand(sqlCmdText.ToString(), dbConnection))
                {
                    sqlCmd.ExecuteNonQuery();
                }
            }
            //}
            //catch (SqlException)
            //{
            //    return false;
            //}

            return true;
        }
    }
}
