using ProgramNameSpace.Session;
namespace ProgramNameSpace
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: programm.exe <path-to-csv>");
                return;
            }
            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"file {args[0]} not exists");
                return;
            }

            var targetPath = args[0];


        }
    }
}