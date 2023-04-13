using System;
using System.Windows;

namespace RemoteDebuggingToolsWpf
{
   public static class ShowMsgHandle
    {

       public static void ShowMsg(string s)
       {
           MessageBox.Show(s, "温馨提示", MessageBoxButton.OK, MessageBoxImage.Warning);
       }
    }
}
