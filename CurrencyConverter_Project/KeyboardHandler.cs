using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter_Project
{
    public static class KeyboardHandler
    {
        public static void ShowKeyboard(bool keyboardVisibility)
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("ShowHideKeyboard", (handler, view) =>
            {
#if ANDROID
                    handler.PlatformView.ShowSoftInputOnFocus=keyboardVisibility;
#endif
            });
        }

    }
}
