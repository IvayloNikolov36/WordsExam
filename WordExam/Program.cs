using EnglishWordsExam;

namespace WordExam
{
    class Program
    {
        static void Main()
        {
            AppEngine engine = new AppEngine(new FileReader());
            engine.Run();
        }
    }
}
