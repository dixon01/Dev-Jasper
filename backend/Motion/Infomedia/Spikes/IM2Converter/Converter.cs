namespace IM2Converter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Motion.Infomedia.Entities.Layout;

    using DynamicProperty = Gorba.Common.Configuration.Infomedia.Common.DynamicProperty;
    using ElementBase = Gorba.Common.Configuration.Infomedia.Layout.ElementBase;
    using EventCycleConfig = Gorba.Common.Configuration.Infomedia.Presentation.Cycle.EventCycleConfig;
    using LayoutConfig = Gorba.Common.Configuration.Infomedia.Presentation.LayoutConfig;
    using SectionConfigBase = Gorba.Common.Configuration.Infomedia.Presentation.Section.SectionConfigBase;
    using StandardCycleConfig = Gorba.Common.Configuration.Infomedia.Presentation.Cycle.StandardCycleConfig;

    public class Converter
    {
        private const string PhysicalScreenName = "PS1";
        private const string CyclePackageName = "CP1";
        private const string VirtualDisplayName = "VD1";
        private const string MasterCycleName = "MC1";
        private const string MasterLayoutName = "ML1";

        private readonly string filename;

        public Converter(string filename)
        {
            this.filename = filename;
        }

        public bool CreateBackup { get; set; }

        public void Convert()
        {
            var backupFilename = this.filename + ".bak";
            if (this.CreateBackup)
            {
                File.Copy(this.filename, backupFilename);
            }

            bool writing = false;

            try
            {
                var oldSerializer = new XmlSerializer(typeof(PresentationConfig));
                PresentationConfig oldConfig;
                using (var reader = File.OpenText(this.filename))
                {
                    oldConfig = (PresentationConfig)oldSerializer.Deserialize(reader);
                }

                var newConfig = this.ConvertPresentation(oldConfig);
                var newSerializer = new XmlSerializer(typeof(InfomediaConfig));
                var memory = new MemoryStream();
                newSerializer.Serialize(memory, newConfig);
                memory.Flush();
                
                writing = true;
                using (var output = File.Create(this.filename))
                {
                    memory.WriteTo(output);
                }
            }
            finally
            {
                if (!writing && this.CreateBackup)
                {
                    File.Delete(backupFilename);
                }
            }
        }

        private InfomediaConfig ConvertPresentation(PresentationConfig oldConfig)
        {
            var newConfig = new InfomediaConfig { CreationDate = DateTime.Now };

            newConfig.PhysicalScreens.Add(
                new PhysicalScreenConfig
                    {
                        Name = PhysicalScreenName,
                        Type = PhysicalScreenType.TFT,
                        Width = oldConfig.Display.Width,
                        Height = oldConfig.Display.Height
                    });

            newConfig.VirtualDisplays.Add(
                new VirtualDisplayConfig
                    {
                        Name = VirtualDisplayName,
                        CyclePackage = CyclePackageName,
                        Width = oldConfig.Display.Width,
                        Height = oldConfig.Display.Height
                    });

            newConfig.MasterPresentation = this.CreateMasterPresentation();

            this.ConvertCycles(newConfig, oldConfig);

            foreach (var layout in oldConfig.Layouts)
            {
                newConfig.Layouts.Add(this.ConvertLayout(layout, oldConfig.Display));
            }

            return newConfig;
        }

        private MasterPresentationConfig CreateMasterPresentation()
        {
            var config = new MasterPresentationConfig();
            config.MasterCycles.Add(
                new MasterCycleConfig
                    {
                        Name = MasterCycleName,
                        Sections =
                            {
                                new MasterSectionConfig
                                    {
                                        Duration = TimeSpan.FromDays(1),
                                        Layout = MasterLayoutName
                                    }
                            }
                    });
            config.MasterLayouts.Add(
                new MasterLayoutConfig
                    {
                        Name = MasterLayoutName,
                        PhysicalScreens =
                            {
                                new PhysicalScreenRefConfig
                                    {
                                        Reference = PhysicalScreenName,
                                        VirtualDisplays =
                                            {
                                                new VirtualDisplayRefConfig
                                                    {
                                                        Reference = VirtualDisplayName,
                                                        X = 0,
                                                        Y = 0,
                                                        ZIndex = 0
                                                    }
                                            }
                                    }
                            }
                    });
            return config;
        }

        private void ConvertCycles(InfomediaConfig newConfig, PresentationConfig oldConfig)
        {
            var packageConfig = new CyclePackageConfig { Name = CyclePackageName };
            foreach (var oldCycle in oldConfig.StandardCycles)
            {
                var cycle = this.ConvertStandardCycle(oldCycle);
                newConfig.Cycles.StandardCycles.Add(cycle);
                packageConfig.StandardCycles.Add(new StandardCycleRefConfig { Reference = cycle.Name });
            }

            foreach (var oldCycle in oldConfig.EventCycles)
            {
                var cycle = this.ConvertEventCycle(oldCycle);
                newConfig.Cycles.EventCycles.Add(cycle);
                packageConfig.EventCycles.Add(new EventCycleRefConfig { Reference = cycle.Name });
            }

            newConfig.CyclePackages.Add(packageConfig);
        }

        private EventCycleConfig ConvertEventCycle(Gorba.Motion.Infomedia.Entities.Layout.EventCycleConfig oldCycle)
        {
            var newCycle = new EventCycleConfig { Name = oldCycle.Name };
            var triggerProperty = this.Convert<DynamicProperty>(oldCycle.TriggerProperty);
            if (triggerProperty != null && triggerProperty.Evaluation is GenericEval)
            {
                newCycle.Trigger = new GenericTriggerConfig { Coordinates = { (GenericEval)triggerProperty.Evaluation } };
            }

            foreach (var step in oldCycle.Steps)
            {
                newCycle.Sections.Add(this.Convert<SectionConfigBase>(step));
            }

            return newCycle;
        }

        private StandardCycleConfig ConvertStandardCycle(Gorba.Motion.Infomedia.Entities.Layout.StandardCycleConfig oldCycle)
        {
            var newCycle = new StandardCycleConfig
                {
                    Name = oldCycle.Name,
                    Enabled = oldCycle.Enabled,
                    EnabledProperty = this.Convert<DynamicProperty>(oldCycle.DynamicProperty)
                };

            foreach (var step in oldCycle.Steps)
            {
                newCycle.Sections.Add(this.Convert<SectionConfigBase>(step));
            }

            return newCycle;
        }

        private LayoutConfig ConvertLayout(Gorba.Motion.Infomedia.Entities.Layout.LayoutConfig oldLayout, DisplayConfig display)
        {
            var layout = new LayoutConfig { Name = oldLayout.Name, BaseLayoutName = oldLayout.BaseLayoutName };
            var resolution = new ResolutionConfig { Width = display.Width, Height = display.Height };
            layout.Resolutions.Add(resolution);

            foreach (var oldElement in oldLayout.Elements)
            {
                var newElement = this.ConvertElement(oldElement);
                if (newElement != null)
                {
                    resolution.Elements.Add(newElement);
                }
            }

            return layout;
        }

        private ElementBase ConvertElement(Gorba.Motion.Infomedia.Entities.Layout.ElementBase element)
        {
            if (element is PresentationElement)
            {
                // this is no more supported
                return null;
            }

            return this.Convert<ElementBase>(element);
        }

        private T Convert<T>(object input)
        {
            if (input == null)
            {
                return default(T);
            }

            return (T)this.Convert(input, typeof(T));
        }

        private object Convert(object input, Type baseType)
        {
            if (input == null)
            {
                return null;
            }

            if (input.GetType() == baseType)
            {
                return input;
            }

            if (input.GetType().IsPrimitive && baseType == typeof(TimeSpan))
            {
                return TimeSpan.FromSeconds(System.Convert.ToDouble(input));
            }

            if (input.GetType().IsEnum && baseType.IsEnum)
            {
                return Enum.Parse(baseType, input.ToString());
            }

            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var list = (IList)Activator.CreateInstance(baseType);
                var itemType = baseType.GetGenericArguments()[0];
                foreach (var item in (IList)input)
                {
                    list.Add(this.Convert(item, itemType));
                }

                return list;
            }

            object output;
            if (input.GetType().FullName == baseType.FullName)
            {
                output = Activator.CreateInstance(baseType);
            }
            else
            {
                var type = baseType.Assembly.GetType(baseType.Namespace + "." + input.GetType().Name, true, false);
                Debug.Assert(type != null, "type != null");
                output = Activator.CreateInstance(type);
            }

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(input))
            {
                var prop = output.GetType().GetProperty(property.Name);
                prop.SetValue(output, this.Convert(property.GetValue(input), prop.PropertyType), null);
            }

            return output;
        }
    }
}
