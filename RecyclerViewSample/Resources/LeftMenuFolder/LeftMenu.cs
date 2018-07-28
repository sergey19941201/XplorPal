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

namespace RecyclerViewSample.Resources.LeftMenuFolder
{
    public class LeftMenu
    {
        /*VARIABLES*/
        string _item;

        /*CONSTRUCTOR*/
        public LeftMenu(string item)
        {
            this._item = item;
        }
        /*PROPIEDADES*/

        public string Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
            }
        }
    }
}