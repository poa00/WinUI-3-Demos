using Microsoft.UI.Xaml.Markup;
using VisualSorting;

namespace VisualSorting
{
    // This idea was copied from:
    // https://devblogs.microsoft.com/ifdef-windows/use-a-custom-resource-markup-extension-to-succeed-at-ui-string-globalization/

    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class ResourceString : MarkupExtension
    {
        public string Name
        {
            get; set;
        }

        protected override object ProvideValue()
        {
            string value = AppResourceManager.GetInstance.GetString(Name);
            return value;
        }
    }
}
