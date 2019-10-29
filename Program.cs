using Maca134.Arma.Serializer;
using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace testapp
{
    internal class Program
    {
#if _WIN64
        private const string dllPath = @"S:\Steam\steamapps\common\Arma 3\Mods\@LocalTimeSync\LocalTimeSync_x64.dll";
#else
        private const string dllPath = @"S:\Steam\steamapps\common\Arma 3\Mods\@LocalTimeSync\LocalTimeSync.dll";
#endif

        private static readonly bool is64BitProcess = IntPtr.Size == 8;

        private static void Main(string[] args)
        {
            Console.WriteLine("{0} (UTC: {1})", DateTime.Now, DateTime.UtcNow);
            Console.WriteLine("is64BitProcess: {0}", is64BitProcess);
            Console.WriteLine("dllPath: {0}", dllPath);
            TestDateTime();
            Console.ReadLine();
        }

        [HandleProcessCorruptedStateExceptions]
        private static void TestCommand(Arma.SQFCommand command)
        {
            var command_parsed = Converter.SerializeObject(command);
            Console.WriteLine("Sending Command: {0}", command_parsed);
            StringBuilder result = new StringBuilder();
            try
            {
                Invoke(result, int.MaxValue, command_parsed);
            }
            catch (Exception ex) { Console.WriteLine("Error while invoking command: {0}", ex.Message); }
            Console.WriteLine("Got Result: {0}", result.ToString().ToJson(indented: true));
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

#if _WIN64

        [DllImport(dllPath, CharSet = CharSet.Ansi, EntryPoint = "RVExtension")]
#else

        [DllImport(dllPath, CharSet = CharSet.Ansi, EntryPoint = "_RVExtension@12")]
#endif

        private static extern void Invoke(StringBuilder output, int size,
            [MarshalAs(UnmanagedType.LPStr)] string input);
    }

    #endregion Imported Methods
}