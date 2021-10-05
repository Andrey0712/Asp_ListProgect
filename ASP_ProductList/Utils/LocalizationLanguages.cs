using ASP_ProductList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP_ProductList.Utils
{
    public static class LocalizationLanguages
    {
        //private readonly static IList<string> Languages = new List<string> { "uk", "en" };

        //public static IList<string> GetLanguages()
        //{
        //    return Languages;
        //}

        //  Створення і ініціалізація колекції мов
        private static IList<LocalizationViewModel> Languages { get; set; } = new List<LocalizationViewModel> {
            new LocalizationViewModel{
                locCode = "uk",
                locName = "Українська мова"
            },
            new LocalizationViewModel{
                locCode = "en",
                locName = "English language"
            }
        };
        public static List<LocalizationViewModel> GetLanguages()
        {
            //  Повернення колеції мов
            return Languages.ToList();
        }
    }
}
