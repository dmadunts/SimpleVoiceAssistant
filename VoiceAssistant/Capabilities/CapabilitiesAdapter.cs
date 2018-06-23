using System.Collections.Generic;
using Android.Views;
using Android.Widget;

namespace VoiceAssistant.Capabilities
{
    public class CapabilitiesAdapter : BaseAdapter<Capability>
    {
        List<Capability> capabilities;

        public CapabilitiesAdapter(List<Capability> capabilities)
        {
            this.capabilities = capabilities;
        }

        public override Capability this[int position]
        {
            get
            {
                return capabilities[position];
            }
        }

        public override int Count
        {
            get
            {
                return capabilities.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CapabilityRow, parent, false);

                var icon = view.FindViewById<ImageView>(Resource.Id.iconImageView);
                var primaryText = view.FindViewById<TextView>(Resource.Id.primaryTextView);
                var secondaryText = view.FindViewById<TextView>(Resource.Id.secondaryTextView);

                view.Tag = new ViewHolder() { Icon = icon, PrimaryText = primaryText, SecondaryText = secondaryText };
            }

            var holder = (ViewHolder)view.Tag;

            holder.Icon.SetImageDrawable(ImageAssetManager.Get(parent.Context, capabilities[position].Icon));
            holder.PrimaryText.Text = capabilities[position].PrimaryText;
            holder.SecondaryText.Text = capabilities[position].SecondaryText;

            return view;
        }
    }
}