using System.Text;
using System.IO;

namespace TestAutomation.project.tests
{
    internal static class ReportManager
    {
        private static readonly object _lock = new object();
        private static string _reportPath = Path.Combine(Directory.GetCurrentDirectory(), "TestResults", "Report.html");
        private static List<string> _entries = new List<string>();
        private static string _currentTestName = null!;

        public static void CreateTest(string name)
        {
            lock (_lock)
            {
                _currentTestName = name;
                _entries.Add($"<h2>{Escape(name)}</h2>");
                _entries.Add("<ul>");
            }
        }

        // Allow configuration of the output report path. If a relative path is provided,
        // it's resolved against the current working directory.
        public static void SetReportPath(string? reportPath)
        {
            if (string.IsNullOrWhiteSpace(reportPath))
                return;

            lock (_lock)
            {
                try
                {
                    var resolved = Path.IsPathRooted(reportPath)
                        ? reportPath
                        : Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), reportPath));
                    _reportPath = resolved;
                }
                catch
                {
                    // ignore and keep default
                }
            }
        }

        public static void LogInfo(string message)
        {
            lock (_lock)
            {
                _entries.Add($"<li>[INFO] {Escape(message)}</li>");
            }
        }

        public static void LogPass(string message)
        {
            lock (_lock)
            {
                _entries.Add($"<li style=\"color:green\">[PASS] {Escape(message)}</li>");
            }
        }

        public static void LogFail(string message)
        {
            lock (_lock)
            {
                _entries.Add($"<li style=\"color:red\">[FAIL] {Escape(message)}</li>");
            }
        }

        public static void AddScreenCapture(string path)
        {
            lock (_lock)
            {
                if (File.Exists(path))
                {
                    var relative = Path.GetRelativePath(Directory.GetCurrentDirectory(), path).Replace('\\', '/');
                    _entries.Add($"<li><a href=\"{relative}\" target=\"_blank\">Screenshot</a></li>");
                }
            }
        }

        public static void Flush()
        {
            lock (_lock)
            {
                if (_entries.Count == 0)
                    return;

                // close last test's list if open
                _entries.Add("</ul>");

                var dir = Path.GetDirectoryName(_reportPath)!;
                Directory.CreateDirectory(dir);

                var sb = new StringBuilder();
                sb.AppendLine("<html><head><meta charset=\"utf-8\"><title>Test Report</title></head><body>");
                sb.AppendLine($"<h1>Test Run - {DateTime.Now:yyyy-MM-dd HH:mm:ss}</h1>");
                foreach (var e in _entries)
                {
                    sb.AppendLine(e);
                }
                sb.AppendLine("</body></html>");

                File.WriteAllText(_reportPath, sb.ToString());
                // clear entries so subsequent runs start fresh
                _entries.Clear();
            }
        }

        private static string Escape(string s)
        {
            return System.Net.WebUtility.HtmlEncode(s);
        }
    }
}
