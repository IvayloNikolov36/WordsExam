using EnglishWordsExam;
using EnglishWordsExam.ConsoleApp;
using System.Text;

Console.OutputEncoding = Encoding.Unicode;
Console.InputEncoding = Encoding.Unicode;

IReader reader = new FileReader(@"../../../assets/words.txt");
AppEngine engine = new(reader);
engine.Run();
