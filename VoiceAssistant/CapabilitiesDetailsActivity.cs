using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using VoiceAssistant.Capabilities;

namespace VoiceAssistant
{
    [Activity(Label = "Capabilities Details")]
    public class CapabilitiesDetailsActivity:AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.CapabilitiesDetails);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Описание";
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var position = Intent.GetIntExtra("position", -1);
            var capability = CapabilitiesList.Capabilities[position];

            var image = FindViewById<ImageView>(Resource.Id.iconImageView);
            var primaryText = FindViewById<TextView>(Resource.Id.primaryTextView);
            var secondaryText = FindViewById<TextView>(Resource.Id.secondaryTextView);
            var description = FindViewById<TextView>(Resource.Id.descriptionTextView);

            image.SetImageDrawable(ImageAssetManager.Get(this, capability.Icon));
            primaryText.Text = capability.PrimaryText;
            secondaryText.Text = capability.SecondaryText;
            description.Text = capability.Description;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();

            return base.OnOptionsItemSelected(item);
        }
    }
}