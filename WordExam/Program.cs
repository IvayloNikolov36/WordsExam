using EnglishWordsExam;
using System.Text;
using System;

namespace WordExam;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.Unicode;
        Console.InputEncoding = Encoding.Unicode;

        AppEngine engine = new AppEngine(new FileReader(Constants.FilePath));
        engine.Run();
    }
}
