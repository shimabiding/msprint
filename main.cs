
/*using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Text.RegularExpressions;
using Npgsql;

namespace msprint.src
{
    class main
    {
        [STAThread]
        static void Main(string[] args)
        {
            var useConsole = true; // Change to false to use GUI
            if (useConsole)
            {
                var extSetIn = new System.IO.StreamReader("stdIn.txt");
                System.Console.SetIn(extSetIn);
            }
            else
            {
                Application.Run(new wrapperForm());
            }

            while (useConsole)
            {
                System.Console.Write("Enter a WP ID: ");
                var input = 0;
                try
                {
                    input = ValidateAndParseInput(Console.ReadLine());
                }
                catch (ArgumentException e)
                {
                    System.Console.WriteLine(e);
                    continue;
                }

                var result = "";
                using (var conn = CreateConnection())
                {
                    if (conn == null)
                    {
                        System.Console.WriteLine("Connection failed");
                        continue;
                    }
                    var reader = default(NpgsqlDataReader);
                    try
                    {
                        reader = GetData(conn, WPQuery(), input);
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e); //DB処理でのエラーはすべて握りつぶしています
                        continue;
                    }
                    try
                    {
                        result = filter(reader);
                    }
                    catch (ArgumentNullException e)
                    {
                        System.Console.WriteLine("Record not found");
                        System.Console.WriteLine(e);
                    }
                }

                try
                {
                    SetResult(result);
                }
                catch (ArgumentNullException e)
                {
                    System.Console.WriteLine(e);
                }
                catch (NullReferenceException e)
                {
                    System.Console.WriteLine("Please run print app");
                    System.Console.WriteLine(e);
                }
            }
        }

        public static int ValidateAndParseInput(string inputStr)
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

        public static NpgsqlConnection CreateConnection()
        {
            var connString = "Host=localhost;Username=postgres;Password=postgres;Database=mng";
            var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                return null;
            }
        }

        public static NpgsqlDataReader GetData(NpgsqlConnection conn, string query, int id)
        {
            var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("id", id);
            return cmd.ExecuteReader();
        }

        public static string filter(NpgsqlDataReader data)
        {
            if (data == null) throw new ArgumentNullException("Record not found");
            var previousDept = "";
            var previousID = "";
            while (data.Read())
            {
                var workStatus = data["work_status"].ToString();
                if (workStatus == "1")
                {
                    var Dept = data["dept_id"].ToString();
                    var id = data["order_detail_id"].ToString();
                    if (Dept == previousDept || string.IsNullOrEmpty(previousDept))
                    {
                        previousDept = Dept;
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

        public static void SetResult(string result)
        {
            var NAME = "beInput";
            var ID = "textBox111";

            var window = AutomationElement.RootElement.FindFirst(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.NameProperty, NAME));
            var element = window.FindFirst(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, ID));
            var pattern = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            var focus = AutomationElement.FocusedElement;
            pattern.SetValue(result);
            focus.SetFocus();
        }

        public static string WPQuery()
        {
            return @"
SELECT *
FROM abcd
WHERE order_id = @id
ORDER BY job_seq ASC
";
        }
    }
}*/