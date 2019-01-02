using CoreAnimation;
using CoreGraphics;
using SafeAuthenticator.Controls;
using SafeAuthenticator.iOS.Helpers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(typeof(MaterialEntry), typeof(BorderlessEntryRenderer))]
namespace SafeAuthenticator.iOS.Helpers
{
    public class BorderlessEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || e.NewElement == null)
            {
                return;
            }

            var buttonRect = UIButton.FromType(UIButtonType.Custom);
            buttonRect.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            buttonRect.SetImage(new UIImage("music"), UIControlState.Normal);
            buttonRect.TouchUpInside += (sender, e1) =>
            {
                if (Control.SecureTextEntry)
                {
                    Control.SecureTextEntry = false;
                    buttonRect.SetImage(new UIImage("HidePasswordIcon"), UIControlState.Normal);
                }
                else
                {
                    Control.SecureTextEntry = true;
                    buttonRect.SetImage(new UIImage("ShowPasswordIcon"), UIControlState.Normal);
                }
            };

            Control.ShouldChangeCharacters += (textField, range, replacementString) =>
            {
                string text = Control.Text;
                var result = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                Control.Text = result;
                return false;
            };

            Control.RightViewMode = UITextFieldViewMode.Always;
            Control.RightView = buttonRect;

            Control.BorderStyle = UITextBorderStyle.None;
            var borderLayer = new CALayer
            {
                MasksToBounds = true,
                Frame = new CGRect(0f, Frame.Height - 1, Frame.Width, 1f),
                BorderColor = Color.FromHex("#0074e4").ToCGColor(),
                BorderWidth = 2.0f
            };

            Control.Layer.AddSublayer(borderLayer);
            Control.Layer.MasksToBounds = true;
        }
    }
}
