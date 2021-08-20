using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DialogController : ControllerBase
    {

        /// <summary>
        /// Возвращает GUID диалога, на основе введенных GUID пользователей. Если такого диалога нет - возвращает пустой GUID. Введите GUID пользователя в "кавычках" через запятую.
        /// </summary>
        [HttpPost]
        public Guid GetDialog(List<Guid> guids)
        {
            RGDialogsClients rg = new RGDialogsClients();

            // Получаем данные по клиентам и чатам.
            var dialogsClients = rg.Init();

            // Группируем, получаем Key это IDRGDialog.
            var groupedDialogs = dialogsClients.GroupBy(d => d.IDRGDialog); 

            // Создаем кортеж с типами - Гуид : List гуидов.
            List<(Guid, List<Guid>)> listOfDialogs = new List<(Guid, List<Guid>)>();

            // С помощью цикла добавляем данные в созданный кортеж, присваиваем ключу Key (Гуид диалога) - отсортированный лист данных клиентов. 
            foreach (var dialog in groupedDialogs)
            {
                listOfDialogs.Add((dialog.Key, dialogsClients.Where(d => d.IDRGDialog == dialog.Key).Select(d => d.IDClient).ToList()));
            }

            // В результате получаем выборку - получить Guid чата c условиями:
            // 1) Кол-во гуидов из листа Item2 (лист данных клиентов из кортежа) должно быть эквивалентно кол-ву введенных пользователем гуидов
            // 2) |Полученное кол-во данных из пересечения листа Item2 и листа введенных гуидов| должны совпадать с |кол-вом данных листа Item2|
            var result = listOfDialogs.FirstOrDefault(d => d.Item2.Count() == guids.Count() && d.Item2.Intersect(guids).Count() == d.Item2.Count()).Item1;

            // В результате получаем Guid чата если такой существует.
            // Если данных нет, тогда автоматически будет выводиться пустой Guid - "00000000-0000-0000-0000-000000000000"
            return result;

        }

    }
}
