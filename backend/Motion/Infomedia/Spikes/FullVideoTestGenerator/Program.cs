namespace FullVideoTestGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    class Program
    {
        static void Main()
        {
            var path = @"D:\projects\Video_Test_Gorba\VIDEO_TESTS_IM2";
            var outputDir = Path.Combine(path, "Presentations");

            CreatePresentation(
                path,
                Path.Combine(outputDir, "video_test_Window.im2"),
                new Tuple<string, string>(
                    "Full", @"<Video X=""0"" Y=""0"" Width=""{0}"" Height=""{1}"" VideoUri=""..\{2}"" />"),
                new Tuple<string, string>(
                    "TopMost", 
                    @"
<Text X=""20"" Y=""10"" Width=""600"" Height=""75"" Align=""Left"" Overflow=""Scale"" ZIndex=""0"" Value=""This is some random text"">
  <Font Face=""Arial"" Height=""40"" Weight=""900"" Italic=""false"" Color=""#FF00FF"" />
</Text>
<Text X=""20"" Y=""100"" Width=""600"" Height=""75"" Align=""Left"" Overflow=""Scale"" ZIndex=""0"" Value=""Behind the video"">
  <Font Face=""Arial"" Height=""20"" Weight=""900"" Italic=""false"" Color=""#FF00FF"" />
</Text>
<Image X=""30"" Y=""30"" Width=""100"" Height=""50"" ZIndex=""-1"" Filename=""gorba.jpg"" />
<Video X=""100"" Y=""50"" Width=""640"" Height=""360"" ZIndex=""999"" VideoUri=""..\{2}"" />"));

            CreatePresentation(
                path,
                Path.Combine(outputDir, "video_test_DirectDraw.im2"),
                new Tuple<string, string>(
                    "BelowItems",
                    @"
<Text X=""20"" Y=""10"" Width=""600"" Height=""75"" Align=""Left"" Overflow=""Scale"" ZIndex=""0"" Value=""This is some random text"">
  <Font Face=""Arial"" Height=""40"" Weight=""900"" Italic=""false"" Color=""#FF00FF"" />
</Text>
<Text X=""20"" Y=""100"" Width=""600"" Height=""75"" Align=""Left"" Overflow=""Scale"" ZIndex=""0"" Value=""Behind the video"">
  <Font Face=""Arial"" Height=""20"" Weight=""900"" Italic=""false"" Color=""#FF00FF"" />
</Text>
<Image X=""30"" Y=""30"" Width=""100"" Height=""50"" ZIndex=""-1"" Filename=""gorba.jpg"" />
<Video X=""100"" Y=""50"" Width=""640"" Height=""360"" ZIndex=""-100"" VideoUri=""..\{2}"" />"));
        }

        private static void CreatePresentation(string path, string fileName, params Tuple<string, string>[] layouts)
        {
            var videos = new List<string>();
            videos.AddRange(Directory.GetFiles(path, "*.avi", SearchOption.AllDirectories));
            videos.AddRange(Directory.GetFiles(path, "*.mpg", SearchOption.AllDirectories));
            videos.AddRange(Directory.GetFiles(path, "*.mp4", SearchOption.AllDirectories));
            videos.Sort();

            var videoInfos = videos.ConvertAll(f => new VideoInfo(path, f));

            var screenResolutions = new[] { new Size(1368, 768), new Size(1440, 900), new Size(1920, 1080) };

            File.Delete(fileName);
            using (var writer = File.CreateText(fileName))
            {
                writer.WriteLine(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Infomedia Version=""2.0"">
  <PhysicalScreens>");
                foreach (var resolution in screenResolutions)
                {
                    writer.WriteLine(
                        @"    <PhysicalScreen Name=""TFT_{0}x{1}"" Type=""TFT"" Width=""{0}"" Height=""{1}"" />",
                        resolution.Width,
                        resolution.Height);
                }

                writer.WriteLine(@"  </PhysicalScreens>

  <VirtualDisplays>");
                foreach (var resolution in screenResolutions)
                {
                    writer.WriteLine(
                        @"    <VirtualDisplay Name=""TFT_{0}x{1}"" CyclePackage=""TftPackage"" Width=""{0}"" Height=""{1}"" />",
                        resolution.Width,
                        resolution.Height);
                }

                writer.WriteLine(@"  </VirtualDisplays>

  <MasterPresentation>
    <MasterCycles>
      <MasterCycle Name=""MasterCycle"">
        <MasterSection Duration=""100000"" Layout=""MasterLayout"" />
      </MasterCycle>
    </MasterCycles>

    <MasterEventCycles />

    <MasterLayouts>
      <MasterLayout Name=""MasterLayout"">");
                foreach (var resolution in screenResolutions)
                {
                    writer.WriteLine(
                        @"        <PhysicalScreen Ref=""TFT_{0}x{1}"">
          <VirtualDisplay Ref=""TFT_{0}x{1}"" X=""0"" Y=""0"" ZIndex=""0"" />
        </PhysicalScreen>",
                        resolution.Width,
                        resolution.Height);
                }

                writer.WriteLine(@"      </MasterLayout>
    </MasterLayouts>
  </MasterPresentation>

  <CyclePackages>
    <CyclePackage Name=""TftPackage"">
      <StandardCycles>
        <StandardCycle Ref=""MainCycle"" />
      </StandardCycles>
    </CyclePackage>
  </CyclePackages>

  <Cycles>
    <StandardCycles>
      <StandardCycle Name=""MainCycle"">");

                int total = 0;
                foreach (var pair in layouts)
                {
                    foreach (var info in videoInfos)
                    {
                        writer.WriteLine(
                            "        <StandardSection Duration=\"10\" Layout=\"Title_{0}_{1}_{2}\" />",
                            pair.Item1,
                            info.Codec,
                            info.Resolution);
                        writer.WriteLine(
                            "        <StandardSection Duration=\"40\" Layout=\"Video_{0}_{1}_{2}\" />",
                            pair.Item1,
                            info.Codec,
                            info.Resolution);

                        total++;
                    }
                }

                writer.WriteLine(@"      </StandardCycle>
    </StandardCycles>

    <EventCycles />
  </Cycles>

  <Layouts>");

                int count = 0;
                foreach (var pair in layouts)
                {
                    foreach (var info in videoInfos)
                    {
                        count++;

                        writer.WriteLine(
                            "    <Layout Name=\"Title_{0}_{1}_{2}\">",
                            pair.Item1,
                            info.Codec,
                            info.Resolution);

                        foreach (var resolution in screenResolutions)
                        {
                            writer.WriteLine(
                                @"      <Resolution Width=""{0}"" Height=""{1}"">",
                                resolution.Width,
                                resolution.Height);
                            writer.WriteLine(
                                "        <Text X=\"50\" Y=\"50\" Width=\"800\" Height=\"75\" Align=\"Left\" Overflow=\"Scale\" Value=\"Video {0} {1} {2}  ({3} / {4})\">",
                                pair.Item1,
                                info.Codec,
                                info.Resolution,
                                count,
                                total);
                            writer.WriteLine("          <Font Face=\"Arial\" Height=\"60\" Weight=\"900\" Italic=\"false\" Color=\"#FFFFFF\" />");
                            writer.WriteLine("        </Text>");
                            writer.WriteLine(
                                @"      </Resolution>");
                        }

                        writer.WriteLine("    </Layout>");

                        writer.WriteLine(
                            "    <Layout Name=\"Video_{0}_{1}_{2}\">",
                            pair.Item1,
                            info.Codec,
                            info.Resolution);
                        foreach (var resolution in screenResolutions)
                        {
                            writer.WriteLine(
                                @"      <Resolution Width=""{0}"" Height=""{1}"">",
                                resolution.Width,
                                resolution.Height);
                            writer.WriteLine(pair.Item2, resolution.Width, resolution.Height, info.FileName);
                            writer.WriteLine(
                                @"      </Resolution>");
                        }

                        writer.WriteLine("    </Layout>");
                    }
                }

                writer.WriteLine("  </Layouts>");
                writer.WriteLine("</Infomedia>");
            }
        }

        private class VideoInfo
        {
            public VideoInfo(string path, string fullName)
            {
                var info = new FileInfo(fullName);
                this.FileName = fullName.Substring(path.Length + 1);
                this.Codec = info.Directory.Name;

                var name = Path.GetFileNameWithoutExtension(fullName);
                this.Resolution = name.Substring(name.LastIndexOf('_') + 1);
            }

            public string FileName { get; private set; }

            public string Codec { get; private set; }

            public string Resolution { get; private set; }
        }
    }
}
