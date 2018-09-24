using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication.ApiControllers
{
    public static class Extensions
    {
        private static readonly PluralizationService PluralizationService =
            PluralizationService.CreateService(CultureInfo.GetCultureInfoByIetfLanguageTag("en-US"));

        public static string Pluralize(this string word)
        {
            return PluralizationService.Pluralize(word);
        }
    }
}
