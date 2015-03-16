About PerfCompare
-----------------
PerfCompare is designed to help you micro-benchmark small snippets of C# code.

The most effective performance improvements come under the guidance of a 
Profiler.  Once you've identified a bottleneck, PerfCompare's Performance
testing helps you quickly test and compare ideas.

When you want to dig a little deeper, PerfCompare's ILDasm integration helps
you see exactly how the compiler translates your C# code into MSIL.

Finally, PerfCompare's history management features help you easily track,
compare, and export your performance tests.


Feature Highlights
------------------
- Supports compiler parameters (ie: /unsafe, /reference, etc.)
  via the "Additional Compiler Parameters" text box
- Supports "advanced edit."  This gives you complete
  control over the code in test, provided you follow the guidance
  of the comments at the top of the template.
- "Export to Clipboard" allows you to export tab-delimited history data into
  the clipboard.  Paste into Excel for further data analysis.
- "Restore Values to Form" allows you to restore the settings from a test run 
  back throughout the application.
- "Compare" allows you to compare the results of 2 selected runs.


05/18/2004
----------
Initial implementation complete.
Feature highlights:
    - Enter code in the "Code" tab.
    - Click on the "ILDasm Result" tab to see ILDasm output.
    - Test the performance on the "Performance" tab.
    - History tab auto-updates with results of performance runs.


FAQ
---
Q: I'd like to use a different ILDasm.exe.  How do I change that path?
A: Delete the registry key, 
   HKEY_CURRENT_USER\Software\Microsoft\PerfCompare\ILDasmPath

Q: I accidentally turned off update notification.  How do I turn it back on?
A: Delete the registry key, 
   HKEY_CURRENT_USER\Software\Microsoft\PerfCompare\CheckForUpdatesOnLoad

Q: How do I test unsafe code?
A: In the "Code" tab, add the compiler parameter, "/unsafe"

Q: How do I reference other assemblies?
A: In the "Code" tab, add the compiler parameter, "/r:[the assembly]"
   For example, "/r:System.Windows.Forms.dll"

Q: How do I add some setup or teardown code that executes outside of the loop?
A: In the "Code" tab, enable "Advanced Edit"

Q: How do I add more 'using' statements?
A: In the "Code" tab, enable "Advanced Edit"


Contact Information
-------------------
Do you have questions?  Comments?  Bugs?  Feature requests?
If so, email me at perfcompare@leeholmes.com.

