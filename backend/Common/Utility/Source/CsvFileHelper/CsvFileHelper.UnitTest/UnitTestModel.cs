namespace Luminator.CsvFileHelper.UnitTest
{
    using System;

    using CsvHelper.Configuration;

    using Random = DimMock.Random;
    
    public class UnitTestModel
    {
        public UnitTestModel()
        {

            this.Created = DateTime.Now;
            this.First = "First=" +  Faker.Name.First();
            this.Last = "Last=" + Faker.Name.Last();
            this.Comment = "Comment=" + Faker.Lorem.Sentence();
            this.Enabled = false;
            this.Started = Random.DateTime();
            this.Stopped = this.Started.Value.AddSeconds(Random.Int());
        }
    
        public DateTime Created { get; set; }
        
        public string First { get; set; }

        public string Last { get; set; }

        public string Comment { get; set; }

        public bool Enabled { get; set; }
        
        public DateTime? Started { get; set; }

        public DateTime? Stopped { get; set; }

        public DateTime? PlayStopped { get; set; }
    }

    public class UnitTestModel2 : UnitTestModel
    {
        public UnitTestModel2()
        {
            this.Description = "Description=" + Faker.Lorem.Sentence();
        }

        public string Description { get; set; }

        public bool IsValid
        {
            get
            {
                return string.IsNullOrEmpty(this.Description) == false;
            }
        }
    }

    public sealed class UnitTestModel2CsvClassMap : ClassMap<UnitTestModel2>
    {
        public UnitTestModel2CsvClassMap()
        {
            this.Map(m => m.Description);
            this.Map(m => m.Created);
            this.Map(m => m.First);
            this.Map(m => m.Last);
            this.Map(m => m.Started);
            this.Map(m => m.Stopped);
            this.Map(m => m.PlayStopped);
            this.Map(m => m.Comment);
            this.Map(m => m.Enabled);
        }
    }
}