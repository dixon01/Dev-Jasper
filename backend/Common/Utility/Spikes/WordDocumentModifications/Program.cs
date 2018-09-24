namespace WordDocumentModifications
{
    class Program
    {
        private const string FileName = "Test.docx";

        static void Main(string[] args)
        {
            OpenXmlTest.Test(FileName);
            //DocXTest.Test(FileName);
        }
    }
}
