namespace Luminator.CsvFileHelper.UnitTest
{
    using System;
    using System.IO;

    internal class TestModel1
    {
        public TestModel1()
        {
            this.TimeStamp = DateTime.Now;
            this.Enabled = true;
            this.FileName = Path.GetRandomFileName();
            this.Enabled = true;
            this.Started = DateTime.Now;
            this.Stopped = this.Started.Value.AddMinutes(1);
        }

        public string FileName { get; set; }
        public bool Enabled { get; set; }
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Stopped { get; set; }
        public DateTime? NullableTimeStamp { get; set; }
        public bool IgnoreMeValue { get; }
    }
}