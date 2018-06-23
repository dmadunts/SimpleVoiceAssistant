using System.Collections.Generic;
using System.Linq;
namespace VoiceAssistant
{
    public static class IntentManager
    {
        const string SPEECH_INTENT_NOT_FOUND = "No intent to handle";
        const string SPEECH_ACTION_NOT_FOUND = "State is not defined";

        private static Dictionary<string, string[]> Intents = new Dictionary<string, string[]>()
        {
            {
                "greetings", new string[]
                    {
                        "привет", "здар", "здор", "здрав",
                        "приветствую", "хай", "хэй", "хей", "эй", "приветик", "приветули", "куку",
                        "ку-ку", "салют", "салом", "салам", "асалом", "ассалом", "асалам",
                        "ассалам", "асолом", "ассолом", "ё", "йо", "йоу", "ёу",
                    }
            },

            {
                "howareyou", new string[]
                    {"как дела","как чувст","как чуст", "как настроение",
                        "как ты", "как жизнь", "че как", "чё как",
                        "чо как", "что как", "как самочувствие", "как оно"}
            },

            {
                "alarm", new string[] {"буди"}
            },

            {
                "music", new string[]
                    {"включи музыку", "музык", "песн" }
            },

            {
                "dial", new string[]
                    {"звонок", "позвони", "вызов", "набери", "номер", "телефон" }
            },

            {
                "about", new string[]
                {"кто ты", "что ты", "о себе"}
            },

            {
                "flashlight", new string[]
                {"фонар", "свет", "ламп"}
            },

            {
                "search", new string[]
                {
                    "что такое", "поиск", "найди", "как", "где",
                    "когда", "почему", "сколько", "кто так", "что",
                    "чем", "зачем", "кого", "от чего", "чего", "о чём",
                    "о ком", "чей", "чья", "чьи"
                }
            },

            {
                "capabilities", new string[]
                {
                    "что можешь", "что ты можешь", "что умеешь", "что ты умеешь",
                    "список команд", "список возможно", "доступные команды",
                    "что может", "команды", "возмож", "навык", "способност"
                }
            }
        };

        private static Dictionary<bool, string[]> Actions = new Dictionary<bool, string[]>()
        {
            { true, new string[] { "включ", "вруб" } },
            { false, new string [] { "выключ", "выруб", "отключ" } },
        };

        public static string SearchRelevantIntent(string speechIntent)
        {
            string tempKey = null;
            string matchedString = "";
            if (!string.IsNullOrEmpty(speechIntent))
            {
                foreach (KeyValuePair<string, string[]> pair in Intents)
                {
                    foreach (var value in pair.Value)
                    {
                        if (speechIntent.Contains(value))
                        {
                            if (value.Length > matchedString.Length)
                            {
                                matchedString = value;
                                tempKey = pair.Key.ToString();
                            }
                        }
                    }
                }
                if (tempKey != null)
                {
                    return tempKey;
                }

                else
                {
                    return SPEECH_INTENT_NOT_FOUND;
                }
            }
            else return "No input speech";
        }

        public static string DetermineState(string possibleState)
        {
            if (possibleState != null)
            {
                foreach (KeyValuePair<bool, string[]> pair in Actions)
                {
                    foreach (var value in pair.Value)
                    {
                        if (possibleState.Contains(value))
                        {
                            return pair.Key.ToString().ToLower();
                        }
                    }
                }
            }
            return SPEECH_ACTION_NOT_FOUND;
        }

    }
}