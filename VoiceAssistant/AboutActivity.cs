using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;


namespace VoiceAssistant
{
    [Activity(Label = "About")]
    public class AboutActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar toolbar;
        Button gitButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.About);

            gitButton = FindViewById<Button>(Resource.Id.gitButton);
            gitButton.Click += (sender, e) =>
              {
                  Uri uri = Uri.Parse("https://github.com/dmadunts");
                  Intent intent = new Intent(Intent.ActionView, uri);
                  StartActivity(intent);
              };

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "О приложении";
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();

            return base.OnOptionsItemSelected(item);
        }
    }
}