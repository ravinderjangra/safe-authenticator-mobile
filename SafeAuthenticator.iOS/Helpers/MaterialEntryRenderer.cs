﻿using System;
using System.ComponentModel;
using System.Drawing;
using CoreGraphics;
using SafeAuthenticator.Controls;
using SafeAuthenticator.iOS.Extensions;
using SafeAuthenticator.iOS.Helpers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using static Xamarin.Forms.Entry;
using Color = Xamarin.Forms.Color;
using ColorExtensions = SafeAuthenticator.iOS.Extensions.ColorExtensions;

[assembly: ExportRenderer(typeof(MaterialEntry), typeof(MaterialEntryRenderer))]
namespace SafeAuthenticator.iOS.Helpers
{
    public class MaterialEntryRenderer : ViewRenderer<MaterialEntry, FloatLabeledTextField>
    {
        private bool _hasError;
        private bool _hasFocus;
        private UIColor _defaultTextColor;

        protected override void OnElementChanged(ElementChangedEventArgs<MaterialEntry> e)
        {
            base.OnElementChanged(e);

            // unsubscribe
            if (e.OldElement != null)
            {
                Control.EditingDidBegin -= OnEditingDidBegin;
                Control.EditingDidEnd -= OnEditingDidEnd;
                Control.EditingChanged -= ViewOnEditingChanged;
            }

            if (e.NewElement != null)
            {
                var ctrl = CreateNativeControl();
                SetNativeControl(ctrl);

                if (!string.IsNullOrWhiteSpace(Element.AutomationId))
                    SetAutomationId(Element.AutomationId);

                _defaultTextColor = Control.FloatingLabelTextColor;

                SetIsPassword();
                SetText();
                SetHintText();
                SetTextColor();
                SetBackgroundColor();
                SetPlaceholderColor();
                SetKeyboard();
                SetHorizontalTextAlignment();
                SetErrorText();
                SetFont();
                SetFloatingHintEnabled();
                SetErrorDisplay();
                SetFocusedColor();
                SetUnfocusedColor();

                Control.UnderlineErrorTextIsVisible = Element.ErrorDisplay == ErrorDisplay.Underline;
                Control.EditingDidBegin += OnEditingDidBegin;
                Control.EditingDidEnd += OnEditingDidEnd;
                Control.EditingChanged += ViewOnEditingChanged;
            }
        }

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        protected virtual FloatLabeledTextField CreateNativeControl()
        {
            return new FloatLabeledTextField();
        }
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == PlaceholderProperty.PropertyName)
            {
                SetHintText();
            }
            else if (e.PropertyName == MaterialEntry.ErrorTextProperty.PropertyName)
            {
                SetErrorText();
            }
            else if (e.PropertyName == TextColorProperty.PropertyName)
            {
                SetTextColor();
            }
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                SetBackgroundColor();
            }
            else if (e.PropertyName == IsPasswordProperty.PropertyName)
            {
                SetIsPassword();
            }
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
            {
                SetIsPassword();
                SetTextColor();
            }
            else if (e.PropertyName == TextProperty.PropertyName)
            {
                SetText();
            }
            else if (e.PropertyName == PlaceholderColorProperty.PropertyName)
            {
                SetUnfocusedColor();
                SetPlaceholderColor();
            }
            else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
            {
                SetKeyboard();
            }
            else if (e.PropertyName == HorizontalTextAlignmentProperty.PropertyName)
            {
                SetHorizontalTextAlignment();
            }
            else if (e.PropertyName == MaterialEntry.FloatingHintEnabledProperty.PropertyName)
            {
                SetFloatingHintEnabled();
            }
            else if ((e.PropertyName == FontAttributesProperty.PropertyName) ||
                     (e.PropertyName == FontFamilyProperty.PropertyName) ||
                     (e.PropertyName == FontSizeProperty.PropertyName))
            {
                SetFont();
            }
            else if (e.PropertyName == MaterialEntry.ActivePlaceholderColorProperty.PropertyName)
            {
                SetFocusedColor();
            }
        }

        private void OnEditingDidEnd(object sender, EventArgs eventArgs)
        {
            var isFocusedPropertyKey = Element.GetInternalField<BindablePropertyKey>("IsFocusedPropertyKey");
            Element.SetValueFromRenderer(isFocusedPropertyKey, false);
            _hasFocus = false;
            Control.UnderlineColor = GetUnderlineColorForState();
        }

        private void OnEditingDidBegin(object sender, EventArgs eventArgs)
        {
            var isFocusedPropertyKey = Element.GetInternalField<BindablePropertyKey>("IsFocusedPropertyKey");
            Element.SetValueFromRenderer(isFocusedPropertyKey, true);
            _hasFocus = true;
            Control.UnderlineColor = GetUnderlineColorForState();
        }

        private void ViewOnEditingChanged(object sender, EventArgs eventArgs)
        {
            Element?.SetValueFromRenderer(TextProperty, Control.Text);
        }

        private void SetUnfocusedColor()
        {
            Control.FloatingLabelTextColor = Element.PlaceholderColor == Color.Default
                ? Control.TextColor
                : Element.PlaceholderColor.ToUIColor();
        }

        private void SetFocusedColor()
        {
            Control.FloatingLabelActiveTextColor = Element.ActivePlaceholderColor == Color.Accent
                ? Control.TintColor
                : Element.ActivePlaceholderColor.ToUIColor();
        }

        private void SetFloatingHintEnabled()
        {
            Control.FloatingLabelEnabled = Element.FloatingHintEnabled;
        }

        public void SetErrorDisplay()
        {
            switch (Element.ErrorDisplay)
            {
                case ErrorDisplay.None:
                    Control.UnderlineErrorSpaceEnabled = false;
                    Control.UnderlineErrorTextIsVisible = false;
                    break;
                case ErrorDisplay.Underline:
                    Control.UnderlineErrorSpaceEnabled = true;
                    _hasError = !string.IsNullOrEmpty(Element.ErrorText);
                    Control.UnderlineErrorTextIsVisible = _hasError;
                    break;
            }
        }

        private void SetErrorText()
        {
            _hasError = !string.IsNullOrEmpty(Element.ErrorText);
            Control.UnderlineColor = GetUnderlineColorForState();
            Control.UnderlineErrorTextIsVisible = _hasError && (Element.ErrorDisplay == ErrorDisplay.Underline);
            Control.ErrorText = Element.ErrorText;
        }

        private CGColor GetUnderlineColorForState()
        {
            if (_hasError)
                return UIColor.Red.CGColor;

            return _hasFocus
                ? (Element.ActivePlaceholderColor == Color.Accent
                    ? Control.TintColor.CGColor
                    : Element.ActivePlaceholderColor.ToCGColor())
                : (Element.PlaceholderColor == Color.Default
                    ? Control.TextColor.CGColor
                    : Element.PlaceholderColor.ToCGColor());
        }

        private void SetBackgroundColor()
        {
            NativeView.BackgroundColor = Element.BackgroundColor.ToUIColor();
            Control.UnderlineColor = Element.PlaceholderColor.ToCGColor();
        }

        private void SetText()
        {
            if (Control.Text != Element.Text)
                Control.Text = Element.Text;
        }

        private void SetIsPassword()
        {
            if (Element.IsPassword && Control.IsFirstResponder)
            {
                Control.Enabled = false;
                Control.SecureTextEntry = true;
                Control.Enabled = Element.IsEnabled;
                Control.BecomeFirstResponder();
            }
            else
            {
                Control.SecureTextEntry = Element.IsPassword;
            }
        }

        private void SetHintText()
        {
            Control.Placeholder = Element.Placeholder;
        }

        // taken from Xamarin.Forms codebase
        private void SetPlaceholderColor()
        {
            var formatted = (FormattedString)Element.Placeholder;

            if (formatted == null)
                return;

            var targetColor = Element.PlaceholderColor;

            // Placeholder default color is 70% gray
            // https://developer.apple.com/library/prerelease/ios/documentation/UIKit/Reference/UITextField_Class/index.html#//apple_ref/occ/instp/UITextField/placeholder

            var color = Element.IsEnabled && !targetColor.IsDefault ? targetColor : ColorExtensions.SeventyPercentGrey.ToColor();
            Control.AttributedPlaceholder = formatted.ToAttributed(Element, color);
        }

        private void SetTextColor()
        {
            if ((Element.TextColor == Color.Default) || !Element.IsEnabled)
                Control.TextColor = _defaultTextColor;
            else
                Control.TextColor = Element.TextColor.ToUIColor();
        }

        private void SetFont()
        {
            Control.Font = Element.ToUIFont();
        }

        private void SetHorizontalTextAlignment()
        {
            switch (Element.HorizontalTextAlignment)
            {
                case TextAlignment.Center:
                    Control.TextAlignment = UITextAlignment.Center;
                    break;
                case TextAlignment.End:
                    Control.TextAlignment = UITextAlignment.Right;
                    break;
                default:
                    Control.TextAlignment = UITextAlignment.Left;
                    break;
            }
        }

        private void SetKeyboard()
        {
            var kbd = Element.Keyboard.ToNative();
            Control.KeyboardType = kbd;
            Control.InputAccessoryView = kbd == UIKeyboardType.NumberPad ? NumberpadAccessoryView() : null;
            Control.ShouldReturn = InvokeCompleted;
        }

        private UIToolbar NumberpadAccessoryView()
        {
            return new UIToolbar(new RectangleF(0.0f, 0.0f, (float)Control.Frame.Size.Width, 44.0f))
            {
                Items = new[]
                {
                    new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
#pragma warning disable SA1130 // Use lambda syntax
                    new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate { InvokeCompleted(null); })
#pragma warning restore SA1130 // Use lambda syntax
                }
            };
        }

        private bool InvokeCompleted(UITextField textField)
        {
            Control.ResignFirstResponder();
            ((IEntryController)Element).SendCompleted();
            return true;
        }
    }
}
