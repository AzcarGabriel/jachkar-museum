using UnityEngine.UIElements;

namespace UI.UIScripts
{
    public static class UIExtension
    {
        public static void Display(this VisualElement element, bool enabled)
        {
            if (element == null) return;
            element.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
