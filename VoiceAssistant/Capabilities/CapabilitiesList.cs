using System.Collections.Generic;
using System.Linq;
using VoiceAssistant.Capabilities;

namespace VoiceAssistant
{
    public static class CapabilitiesList
    {
        public static List<Capability> Capabilities { get; private set; }

        static CapabilitiesList()
        {
            var temp = new List<Capability>();

            AddCapabilities(temp);

            Capabilities = temp.OrderBy(i => i.PrimaryText).ToList();
        }


        static void AddCapabilities(List<Capability> capabilities)
        {
            capabilities.Add(new Capability()
            {
                PrimaryText = "Музыка",
                SecondaryText = "Включает музыку",
                Icon = "Resources/drawable/music.png",
                Description = "Включает случайный музыкальный трек с Вашего устройства по голосовому запросу (пр. \"Включи музыку\")."
            });

            capabilities.Add(new Capability()
            {
                PrimaryText = "Фонарик",
                SecondaryText = "Включает фонарик",
                Icon = "Resources/drawable/flashlight.png",
                Description = "Включает фонарик по голосовому запросу (пр. \"Включи фонарик\")."
            });

            capabilities.Add(new Capability()
            {
                PrimaryText = "Поиск",
                SecondaryText = "Производит поисковый запрос",
                Icon = "Resources/drawable/search.png",
                Description = "Производит поисковый запрос по голосовому запросу (пр. \"Что такое оперативная память?\")."
            });

            capabilities.Add(new Capability()
            {
                PrimaryText = "Будильник",
                SecondaryText = "Устанавливает будильник",
                Icon = "Resources/drawable/alarm.png",
                Description = "Устанавливает будильник по голосовому запросу (пр. \"Установи будильник на 12:45\")."
            });

            capabilities.Add(new Capability()
            {
                PrimaryText = "Звонок",
                SecondaryText = "Производит вызов",
                Icon = "Resources/drawable/phone.png",
                Description = "Производит вызов по номеру телефона (пр. \"Позвони на номер 555-55-55\")."
            });


        }
    }
}