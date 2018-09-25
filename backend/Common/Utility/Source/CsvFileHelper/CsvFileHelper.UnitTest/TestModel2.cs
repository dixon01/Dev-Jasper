namespace Luminator.CsvFileHelper.UnitTest
{
    internal class TestModel2 : TestModel1
    {
        public TestModel2()
        {
         
        }
        
        public string MyReadOnlyString { get; }
    }

    internal class TestModel3 
    {
        public TestModel3()
        {

        }

        public string FileName { get; set; }
        public string ResourceId { get; set; }
    }
}