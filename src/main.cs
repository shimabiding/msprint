using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Npgsql;

namespace msprint.src
{
    class main
    {
        [STAThread]
        static void Main()
        {
            var useGUI = false; // Change to true to use GUI
            if (useGUI)
            {
                Application.Run(new wrapperForm());
                return;
            }
            while (!useGUI)
            {
                try
                {
                    var input = GetValidatedInt();
                    var data = GetDataListFromInt(input, WPQuery());
                    var result = FilteredID(data);
                    CallSetResult(result);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Record not found"))
                    {
                        Console.WriteLine(e);
                        continue;
                    }
                    Console.WriteLine(e);
                    break;
                }
            }
        }

        internal static int GetValidatedInt()
        {
            while (true)
            {
                try
                {
                    Console.Write("Enter a WP ID: ");
                    var input = Console.ReadLine();
                    return ValidateAndParse(input);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        internal static int ValidateAndParse(string inputStr)
        {
            if (inputStr.Length != 16)
            {
                throw new ArgumentException("Missing Length");
            }
            var match = Regex.Match(inputStr, @"^WP(\d{11})\d{3}$", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                throw new ArgumentException("Invalid format");
            }

            return int.Parse(match.Groups[1].Value);
        }

        internal static List<Dictionary<string, object>> GetDataListFromInt(int id, string query)
        {
            // order_idからdetailを取得
            var connString = "Host=localhost;Username=postgres;Password=postgres;Database=mng";
            var dataList = new List<Dictionary<string, object>>();
            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            dataList.Add(row);
                        }
                    }
                }
                catch (Exception)
                {
                    throw; //DB処理でのエラーはExceptionの内容を問わずmainループに投げています
                }
            }
            return dataList;
        }

        internal static string FilteredID(List<Dictionary<string, object>> data)
        {
            // listの中で進捗が最も進んでいるidを返す
            if (data.Count == 0) throw new ArgumentException("Record not found");
            var previousDept = "";
            var previousID = "";
            foreach (var row in data)
            {
                var workStatus = row["work_status"].ToString();
                if (workStatus == "1")
                {
                    var dept = row["dept_id"].ToString();
                    var id = row["order_detail_id"].ToString();
                    if (dept == previousDept || string.IsNullOrEmpty(previousDept))
                    {
                        previousDept = dept;
                        previousID = id;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return previousID;
        }

        internal static void CallSetResult(string result)
        {
            AutomationElement focus = AutomationElement.FocusedElement; // SetValue時にフォーカスが入力先に奪われるため、コンソールないしGUIへフォーカスを戻すために取得
            try
            {
                SetResult(result);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Not found print app. Please run the app");
                Console.WriteLine(e);
            }
            finally {
                focus.SetFocus();
            }
        }

        internal static void SetResult(string result)
        {
            var NAME = "beInput";// デスクトップ直下の子要素を取得するための名前
            var ID = "textBox111";// 子要素内のテキストボックスのAutomationIDを指定 事前に調査要

            var window = AutomationElement.RootElement.FindFirst(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.NameProperty, NAME));
            var element = window.FindFirst(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, ID));
            var pattern = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            pattern.SetValue(result);
        }
        
        internal static string WPQuery()
        {
            return @"
SELECT *
FROM abcd
WHERE order_id = @id
ORDER BY job_seq ASC
";
        }
    }
}

