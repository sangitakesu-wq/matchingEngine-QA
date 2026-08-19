How to generate the human-friendly HTML report (POC)

1. Run tests as usual (example using dotnet):

   dotnet test or through VS

2. After the run, a minimal HTML report will be generated at:

   <repo-root>/ConsoleApp1/bin/Debug/net10.0/TestResults/Report.html

3. Screenshots for failed scenarios (if any) are saved under:

   <repo-root>/ConsoleApp1/bin/Debug/net10.0/TestResults/Screenshots/

4. Open Report.html in a browser to view the run summary and click any attached screenshots.

