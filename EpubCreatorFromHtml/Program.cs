using System;

namespace EpubCreatorFromHtml
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Creating EPubWithGivenArguments: ");
            EpubCreator.CreateEpub("DummyTitle", "DummyCreator");
        }
    }
}
