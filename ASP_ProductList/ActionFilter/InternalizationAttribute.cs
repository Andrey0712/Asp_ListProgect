using ASP_ProductList.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASP_ProductList.ActionFilter
{
    //  Створення атрибуту, який спрацьовуватиме на етапі запиту до сервера
    public class InternalizationAttribute : ActionFilterAttribute
    {
        //  Колеція підтримуваних мов
        private List<string> _locales { get; set; }
        //private readonly IList<string> _locales;
        //  Мова за замовчанням
        private string _defaultLocale { get; set; }

        public InternalizationAttribute()
        {
            //  Ініціаліація колекції мов
            //_locales = LocalizationLanguages.GetLanguages();
            _locales = LocalizationLanguages.GetLanguages().Select(x => x.locCode).ToList();
            //  Ініціалізація мови за замовчанням
            _defaultLocale = _locales[0];
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //  Отримання мови з шляху і приведення її у строку
            string lang = context.RouteData.Values["lang"].ToString();

            //  Перевірка чи мова не пуста
            if (lang == null)
            {
                //  Привоєння для змінної мови стандартного значення
                lang = _defaultLocale;
            }
            //  Перевірка чи колекція не містить прийдену мову
            if (!_locales.Contains(lang))
            {
                //  Присвоєння для змінної мови стандартного значення
                lang = _defaultLocale;
            }

            //  Додає для куки з ключом CookieRequestCultureProvider.DefaultCookieName значення,
            //  CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang))
            context.HttpContext.Response.Cookies.Append(
                // CookieRequestCultureProvider - визначає налаштування для мови через файли Cookie
                //  Надає назву файла Cookie за замовчанням, який використовується для контролю
                //  локалізації користувача
                CookieRequestCultureProvider.DefaultCookieName,
                //  Метод переводить тип RequestCulture у строку для задачі значення у куку
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                //  Задання налаштувань для кукі, а саме встановлення термін життя кукі
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            //  Отримання назви активної культури
            string currentLang = Thread.CurrentThread.CurrentCulture.Name;

            //  Перевірка чи мова додатку 
            if (currentLang != lang)
            {
                //  Отримання обєкту запиту на сервер
                var request = context.HttpContext.Request;
                //  Формування шляху на який буде перевантажуватися обєкт,
                //  якщо мову було змінено
                var returnUrl = string.IsNullOrEmpty(request.Path) ? "~/" : $"~{request.Path.Value}";
                //  Встанолвення значення для результата роботи контекста фільтра,
                //  а саме виконується перевантаження сторінки
                context.Result = new RedirectResult(returnUrl);
                //  Вихід з функції
                return;
            }
        }
    }
}
