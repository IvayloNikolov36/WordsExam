using System;
using System.Text;

namespace EnglishWordsExam;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.Unicode;
        Console.InputEncoding = Encoding.Unicode;

        IReader reader = new FileReader(Constants.FilePath);
        AppEngine engine = new(reader);
        engine.Run();
    }
}
