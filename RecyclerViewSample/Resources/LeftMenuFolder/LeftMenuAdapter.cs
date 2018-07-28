using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace RecyclerViewSample.Resources.LeftMenuFolder
{
    public class LeftMenuAdapter : BaseAdapter<LeftMenuFolder.LeftMenu>
    {
        List<LeftMenuFolder.LeftMenu> items;
        Activity context;

        public LeftMenuAdapter(Activity context, List<LeftMenuFolder.LeftMenu> items) : base()
        {
            this.context = context;
            this.items = items;
        }

        #region implemented abstract members of BaseAdapter
        public override long GetItemId(int position)
        {
            return position;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            //if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.custom_item_left_drawer, null);

            var view1 = view.FindViewById<TextView>(Resource.Id.item);

            view1.Text = item.Item;

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(context.Assets, path);
            view1.Typeface = tf;

            return view;
        }
        public override int Count
        {
            get { return items.Count; }
        }

        public override LeftMenuFolder.LeftMenu this[int position]
        {
            get
            {
                return items[position];
            }
        }

        #endregion
    }
}