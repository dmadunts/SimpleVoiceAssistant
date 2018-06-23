using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Speech;
using Android.Views;
using Android.Util;
using Android;
using Android.Content.PM;
using VoiceAssistant.Services;
using System.Linq;
using static Android.Speech.Tts.TextToSpeech;
using Android.Speech.Tts;
using Java.Util;
using Android.Provider;
using Android.Support.V7.App;

namespace VoiceAssistant
{
    [Activity(Label = "Voice Assistant", MainLauncher = true, Icon = "@drawable/icon")]
    public class VoiceRecognitionActivity : AppCompatActivity, IRecognitionListener, IOnInitListener
    {
        #region Fields
        const int REQUEST_CODE = 100;
        const int NEED_LANGUAGE = 101;
        const string LOG_TAG = "VoiceRecognitionActivity";
        const string COMMAND_NOT_FOUND = "Command not found";

        const string Greetings = "greetings";
        const string Flashlight = "flashlight";
        const string Search = "search";
        const string Capabilities = "capabilities";
        const string About = "about";
        const string HowAreYou = "howareyou";
        const string Music = "music";
        const string PhoneDial = "dial";
        const string Alarm = "alarm";

        TextToSpeech tts;
        Locale locale;

        Android.Support.V7.Widget.Toolbar toolbar;

        TextView annotationTextView;
        TextView speechTextView;
        TextView speechStateTextView;
        TextView speechIntentTextView;
        ToggleButton toggleButton;
        ProgressBar progressBar;
        SpeechRecognizer speech;
        Intent recognizerIntent;
        System.Random random;
        #endregion

        #region Answers
        static string[] GREETINGS_ANSWERS =
            {
                "привет-привет",
                "привет", "здравствуйте", "салют", "приветствую вас"
            };

        static string[] EXECUTION_ANSWERS =
            {
                "секунду", "выполняю", "сделано", "выполнено"
            };

        static string[] ABOUT_ANSWERS =
            {
                "я ваш верный помощник", "я голосовой ассистент",
                "я молодой и быстро развивающийся голосовой ассистент",
                "я не Сири, я только учусь", "голосовой ассистент"
            };

        static string[] HOWAREYOU_ANSWERS =
            {
                "Всё отлично, спасибо. Надеюсь у вас тоже всё хорошо.",
                "Теперь, когда вы спросили, намного лучше. Надеюсь у вас тоже всё хорошо.",
                "Вполне неплохо. Надеюсь у вас тоже всё хорошо."
            };

        static string[] SEARCH_ANSWERS =
            {
                "Сейчас найдём...", "Уже ищу...", "Одну секунду...",
                "Давайте поищем", "Сейчас найду", "Ищу ответ"
            };

        static string[] ELUSIVE_ANSWERS =
            {
                "Это непросто, возможно, позже я смогу это выполнить",
                "Немного позже я смогу это сделать, а пока давайте попробуем что-нибудь другое",
                "К сожалению, пока я не могу это сделать",
            };

        static string[] CAPABILITIES_ANSWERS =
            { "Вот что я умею", "Вот что я могу", "Я могу следующее", "Я могу выполнить следующее"};
        #endregion Answers

        #region Permissions
        readonly string[] RequiredPermissions =
            {
                Manifest.Permission.Flashlight,
                Manifest.Permission.RecordAudio
            };

        private bool CheckPermissions(VoiceRecognitionActivity voiceRecognitionActivity)
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                var value =
                    voiceRecognitionActivity.CheckSelfPermission(Manifest.Permission.Flashlight) != Permission.Granted ||
                    voiceRecognitionActivity.CheckSelfPermission(Manifest.Permission.RecordAudio) != Permission.Granted;

                return value;
            }
            else return false; 
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
                switch (requestCode)
                {
                    case REQUEST_CODE:
                        {
                            bool hasAllPermissions = grantResults.Where(r => r == Permission.Denied).Count() == 0;

                            if (!hasAllPermissions)
                                Toast.MakeText(this, "Unable to get all required permissions", ToastLength.Short).Show();
                        }
                        break;
                }
        }
        #endregion

        #region Lifecycle
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Voice Assistant";


            annotationTextView = FindViewById<TextView>(Resource.Id.annotationTextView);
            speechTextView = FindViewById<TextView>(Resource.Id.speechTextView);
            speechIntentTextView = FindViewById<TextView>(Resource.Id.speechIntentTextView);
            speechStateTextView = FindViewById<TextView>(Resource.Id.speechActionTextView);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            toggleButton = FindViewById<ToggleButton>(Resource.Id.toggleButton);

            progressBar.Visibility = ViewStates.Invisible;

            random = new System.Random();

            speech = SpeechRecognizer.CreateSpeechRecognizer(this);
            Log.Debug(LOG_TAG, "IsRecognitionAvailable: " + SpeechRecognizer.IsRecognitionAvailable(this));
            speech.SetRecognitionListener(this);

            tts = new TextToSpeech(this, this, "com.google.android.com");
            locale = new Locale("ru");

            recognizerIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            recognizerIntent.PutExtra(RecognizerIntent.ExtraLanguagePreference, "ru");
            recognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            recognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            recognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 5000);
            recognizerIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            recognizerIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 3);

            toggleButton.CheckedChange += ToggleButton_CheckedChange;
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug(LOG_TAG, "OnResume");
        }

        protected override void OnPause()
        {
            base.OnPause();
            Log.Debug(LOG_TAG, "OnPause");
        }

        protected override void OnStop()
        {
            base.OnStop();
            Log.Debug(LOG_TAG, "OnStop");
        }
        #endregion Lifecycle

        #region Events
        private void ToggleButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                if (CheckPermissions(this))
                {
                    toggleButton.Checked = false;
                    RequestPermissions(RequiredPermissions, REQUEST_CODE);
                }
                else
                {
                    progressBar.Visibility = ViewStates.Visible;
                    progressBar.Indeterminate = true;
                    speech.StartListening(recognizerIntent);
                }
            }
            else
            {
                progressBar.Indeterminate = false;
                progressBar.Visibility = ViewStates.Invisible;
                speech.StopListening();
            }
        }
        #endregion Events

        #region Toolbar menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_capabilities)
            {
                StartActivity(new Intent(this, typeof(CapabilitiesActivity)));
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion Toolbar menu

        #region IRecognitionListener
        void IRecognitionListener.OnBeginningOfSpeech()
        {
            Log.Debug(LOG_TAG, "OnBeginningOfSpeech");
            progressBar.Indeterminate = false;
            progressBar.Max = 10;
        }

        void IRecognitionListener.OnBufferReceived(byte[] buffer)
        {
            Log.Debug(LOG_TAG, "OnBufferReceived: " + buffer);
        }

        void IRecognitionListener.OnEndOfSpeech()
        {
            Log.Debug(LOG_TAG, "OnEndOfSpeech");
            progressBar.Indeterminate = true;
            toggleButton.Checked = false;
        }

        void IRecognitionListener.OnError(SpeechRecognizerError error)
        {
            string errorMessage = GetErrorText((int)error);
            Log.Debug(LOG_TAG, "FAILED " + errorMessage);
            Toast.MakeText(this, errorMessage, ToastLength.Short).Show();
            toggleButton.Checked = false;
        }

        private static string GetErrorText(int errorCode)
        {
            string message;
            switch (errorCode)
            {
                case (int)SpeechRecognizerError.Audio:
                    message = "Ошибка записи аудио";
                    break;
                case (int)SpeechRecognizerError.Client:
                    message = "Ошибка на стороне клиента";
                    break;
                case (int)SpeechRecognizerError.InsufficientPermissions:
                    message = "Нет разрешений";
                    break;
                case (int)SpeechRecognizerError.Network:
                    message = "Ошибка сети";
                    break;
                case (int)SpeechRecognizerError.NetworkTimeout:
                    message = "Задержка сети";
                    break;
                case (int)SpeechRecognizerError.NoMatch:
                    message = "Нет совпадений";
                    break;
                case (int)SpeechRecognizerError.RecognizerBusy:
                    message = "RecognitionService занят";
                    break;
                case (int)SpeechRecognizerError.Server:
                    message = "Ошибка на стороне сервера";
                    break;
                case (int)SpeechRecognizerError.SpeechTimeout:
                    message = "Нет входящего речевого сигнала";
                    break;
                default:
                    message = "Ошибка распознавания. Попробуйте еще раз";
                    break;
            }
            return message;
        }

        void IRecognitionListener.OnEvent(int eventType, Bundle @params)
        {
            Log.Debug(LOG_TAG, "OnEvent");
        }

        void IRecognitionListener.OnPartialResults(Bundle partialResults)
        {
            Log.Debug(LOG_TAG, "OnPartialResults");
        }

        void IRecognitionListener.OnReadyForSpeech(Bundle @params)
        {
            Log.Debug(LOG_TAG, "OnReadyForSpeech");
        }

        void IRecognitionListener.OnRmsChanged(float rmsdB)
        {
            progressBar.Progress = (int)rmsdB;
        }

        void IRecognitionListener.OnResults(Bundle results)
        {
            Log.Debug(LOG_TAG, "OnResults");
            var matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            if (matches.Count != 0)
            {
                string inputRecord = string.Empty + matches[0].ToLower();

#if DEBUG
                speechTextView.Text = "Вы сказали: " + inputRecord;
#endif
                string speechIntent = IntentManager.SearchRelevantIntent(inputRecord);
                string possibleState = IntentManager.DetermineState(inputRecord);

                if (!string.IsNullOrEmpty(speechIntent))
                {
                    switch (speechIntent)
                    {
                        case Flashlight:
                            ExecuteFlashlight(possibleState); break;
                        case Search:
                            Speak(SEARCH_ANSWERS);
                            ExecuteSearch(inputRecord); break;
                        case Capabilities:
                            Speak(CAPABILITIES_ANSWERS);
                            StartActivity(new Intent(this, typeof(CapabilitiesActivity))); break;
                        case About:
                            Speak(ABOUT_ANSWERS);
                            StartActivity(new Intent(this, typeof(AboutActivity))); break;
                        case Greetings:
                            Speak(GREETINGS_ANSWERS); break;
                        case HowAreYou:
                            Speak(HOWAREYOU_ANSWERS); break;
                        case Music:
                            Speak(EXECUTION_ANSWERS);
                            ExecutePlayMusic(); break;
                        case PhoneDial:
                            Speak(EXECUTION_ANSWERS);
                            ExecutePhoneDial(inputRecord); break;
                        case Alarm:
                            Speak(EXECUTION_ANSWERS);
                            ExecuteSetAlarm(inputRecord); break;
                        default: Speak(ELUSIVE_ANSWERS); break;
                    }
                }
#if DEBUG
                speechIntentTextView.Text = "Intent: " + speechIntent;
                speechStateTextView.Text = "State: " + possibleState;
#endif
            }
        }
        #endregion IRecognitionListener

        #region Executable services 
        private void ExecuteFlashlight(string state)
        {
            if (state == "false")
            {
                StartService(new Intent(this, typeof(FlashlightService)).PutExtra("state", "off"));
            }
            else
            {
                StartService(new Intent(this, typeof(FlashlightService)).PutExtra("state", "on"));
            }
        }

        private void ExecuteSearch(string searchQuery)
        {
            var wordsToMatch = new string[] { "что такое", "поиск", "найди" };
            if (searchQuery != null)
            {
                foreach (var value in wordsToMatch)
                {
                    if (searchQuery.Contains(value))
                    {
                        searchQuery = searchQuery.Replace(value, "");
                    }
                }

                Intent intent = new Intent(Intent.ActionWebSearch);
                intent.PutExtra(SearchManager.Query, searchQuery);
                if (intent.ResolveActivity(PackageManager) != null)
                {
                    StartActivity(intent);
                }
            }
        }

        private void ExecutePlayMusic()
        {
            Intent intent = new Intent(MediaStore.IntentActionMediaPlayFromSearch);
            intent.PutExtra(SearchManager.Query, string.Empty);
            if (intent.ResolveActivity(PackageManager) != null)
            {
                StartActivity(intent);
            }
        }

        private void ExecutePhoneDial(string inputSpeech)
        {
            var phoneNumber = string.Empty;
            foreach (var c in inputSpeech)
            {
                if (char.IsDigit(c))
                {
                    phoneNumber = phoneNumber + c;
                }
            }

            Intent intent = new Intent(Intent.ActionDial);
            intent.SetData(Android.Net.Uri.Parse("tel:" + phoneNumber));
            if (intent.ResolveActivity(PackageManager) != null)
            {
                StartActivity(intent);
            }
        }

        private void ExecuteSetAlarm(string inputRecord)
        {
            //listBox1.DataSource = value.Where(x => Char.IsDigit(x)).ToList();
            string[] time = inputRecord.Split(':');
            int hour;
            int minutes;
            var temp = string.Empty;

            foreach (var c in time[0].Where(x => char.IsDigit(x)))
            {
                temp = temp + c;
            }
            hour = int.Parse(temp);
            temp = "";

            foreach (var c in time[1].Where(x => char.IsDigit(x)))
            {
                temp = temp + c;
            }
            minutes = int.Parse(temp);

            Intent intent = new Intent(AlarmClock.ActionSetAlarm)
                    .PutExtra(AlarmClock.ExtraHour, hour)
                    .PutExtra(AlarmClock.ExtraMinutes, minutes)
                    .PutExtra(AlarmClock.ExtraSkipUi, true);
            if (intent.ResolveActivity(PackageManager) != null)
            {
                StartActivity(intent);
            }
        }
        private void Speak(string[] answers)
        {
            tts.Speak(answers[random.Next(answers.Length)], QueueMode.Flush, null);
        }
        #endregion

        void IOnInitListener.OnInit(OperationResult status)
        {
            if (status == OperationResult.Success)
            {
                Locale defaultOrPassedIn = locale;
                if (locale == null)
                {
                    defaultOrPassedIn = Locale.Default;
                }

                switch (tts.IsLanguageAvailable(defaultOrPassedIn))
                {
                    case LanguageAvailableResult.Available:
                    case LanguageAvailableResult.CountryAvailable:
                    case LanguageAvailableResult.CountryVarAvailable:
                        Log.Debug(LOG_TAG, "SUPPORTED");
                        tts.SetLanguage(locale);
                        tts.SetSpeechRate(1.0f);
                        tts.SetPitch(1.2f);
                        break;

                    case LanguageAvailableResult.MissingData:
                        Log.Debug(LOG_TAG, "MISSING_DATA");
                        Log.Debug(LOG_TAG, "require data...");
                        Intent installIntent = new Intent();
                        installIntent.SetAction(TextToSpeech.Engine.ActionInstallTtsData);
                        StartActivity(installIntent);
                        break;
                    case LanguageAvailableResult.NotSupported:
                        Log.Debug(LOG_TAG, "NOT SUPPORTED");
                        break;
                    default:
                        Speak(ELUSIVE_ANSWERS);
                        break;
                }
            }
        }
    }
}

