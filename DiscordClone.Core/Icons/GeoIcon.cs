using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiscordClone.Core.Icons
{
    public class GeoIcon : ContentControl
    {
        private static string D = "M9,7V17H13A2,2 0 0,0 15,15V9A2,2 0 0,0 13,7H9M11,9H13V15H11V9Z";
        public static readonly DependencyProperty IconTypeProperty =
            DependencyProperty.Register("IconType", typeof(IconType), typeof(GeoIcon), new PropertyMetadata(IconType.None, IconTypeChanged));

        private static void IconTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GeoIcon control = d as GeoIcon;

            if (control != null)
            {
                switch (control.IconType)
                {
                    case IconType.D: control.Data = Geometry.Parse(D); break;
                }
            }
        }

        public IconType IconType
        {
            get { return (IconType)GetValue(IconTypeProperty); }
            set { SetValue(IconTypeProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Geometry), typeof(GeoIcon), new PropertyMetadata(null));

        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        static GeoIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeoIcon), new FrameworkPropertyMetadata(typeof(GeoIcon)));
        }
    }
}
