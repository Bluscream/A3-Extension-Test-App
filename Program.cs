using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Maca134.Arma.Serializer;

namespace testapp
{
    internal class Program
    {
        private const string dllPath_x86 = @"S:\Steam\steamapps\common\Arma 3\Mods\@LocalTimeSync\LocalTimeSync.dll";
        private const string dllPath_x64 = @"S:\Steam\steamapps\common\Arma 3\Mods\@LocalTimeSync\LocalTimeSync_x64.dll";

        private static bool is64BitProcess = (IntPtr.Size == 8);

        private static void Main(string[] args)
        {
            Console.WriteLine("{0} (UTC: {1})", DateTime.Now, DateTime.UtcNow);
            Console.WriteLine("is64BitProcess: {0}", is64BitProcess);
            TestDateTime();
            Console.ReadLine();
        }

        private static void TestCommand(Arma.SQFCommand command)
        {
            var command_parsed = Converter.SerializeObject(command);
            foreach (var dllPath in new string[] { dllPath_x86, dllPath_x64 })
            {
                var dllName = new FileInfo(dllPath).FileNameWithoutExtension();
                Console.WriteLine("Sending Command to {0}: {1}", dllName, command_parsed);
                StringBuilder result = new StringBuilder();
                if (dllName.ToLower().EndsWith("_x64"))
                {
                    Invoke_x64(result, int.MaxValue, command_parsed);
                }
                else
                {
                    Invoke_x86(result, int.MaxValue, command_parsed);
                }
                Console.WriteLine("Got Result from {0}: {1}", dllName, result.ToString().ToJson(indented: true));
            }
        }

        #region Tests

        private static void TestFileList()
        {
            TestCommand(new Arma.SQFCommand(@"blu_fnc", @"S:\Steam\steamapps\common\Arma 3"));
        }

        private static void TestDateTime()
        {
            TestCommand(new Arma.SQFCommand("blu_fnc_getCurrentDateTime"));
            TestCommand(new Arma.SQFCommand("blu_fnc_getCurrentDateTimeFull"));
        }

        #endregion Tests

        #region Imported Methods

        [DllImport(dllPath_x64, CharSet = CharSet.Unicode, EntryPoint = "RVExtension")]
        private static extern void Invoke_x64(StringBuilder output, int size, [MarshalAs(UnmanagedType.LPStr)] string input);

        [DllImport(dllPath_x86, CharSet = CharSet.Ansi, EntryPoint = "_RVExtension@12")]
        private static extern void Invoke_x86(StringBuilder output, int size, [MarshalAs(UnmanagedType.LPStr)] string input);
    }

    #endregion Imported Methods
}