namespace Luminator.AdhocMessaging
{
    using System.Collections.Generic;

    public static class AdhocConstants
    {
        public const string DefaultTenentId = "7141d8e5-b8e1-425a-21e5-08d541a9fc53";

        public const string DefaultMessageApiUrl = "http://swdevicntrweb.luminatorusa.com";

        public const string DefaultDestinationsApiUrl = "http://swdevicntrapp.luminatorusa.com";

        public const int DefaultHttpClientTimeoutInMilliseconds = 15 * 1000; // 15 seconds

        public const string DefaultHttpPort = "80";

        public const string DefaultUnitRegistrationName = "New Unit Registration Name";
        public const string DefaultVehicleRegistrationName = "New Vehicle Registration Name";

        public const string DefaultUnitRegistrationDescription = "New Unit Registration Description";
        public const string DefaultVehicleRegistrationDescription = "New Vehicle Registration Name";

        public static List<string> DefaultDbConnectionStringsList = new List<string>
                                                                    {
                                                                        @"Connect Timeout=10;Pooling=false;Server = swnycticntr; Database = Infotransit.Destinations; user = lumuser; password = Lum2014;",
                                                                        @"Data Source=.\SQLEXPRESS;Integrated Security=True;Database = Infotransit.Destinations;MultipleActiveResultSets=True;",
                                                                        @"Data Source=(local);Integrated Security=True;Database = Infotransit.Destinations;MultipleActiveResultSets=True;",
                                                                        @"Connect Timeout=10;Pooling=false;Server = swdevicntr; Database = Infotransit.Destinations; user = lumuser; password = Lum2014;",
                                                                        @"Connect Timeout=10;Pooling=false;Server = swdevsql; Database = Infotransit.Destinations; user = lumuser; password = Lum2014;MultipleActiveResultSets=True;",
                                                                    };

    }
}