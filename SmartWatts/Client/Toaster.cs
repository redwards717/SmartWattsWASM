using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SmartWatts.Client
{
    public static class Toaster
    {
        public static string ToastMsg { get; private set; }
        public static string ToastColor { get; private set; }
        public static bool ToastOn { get; private set; }
        public async static void ShowToast(string msg, string color, int time = 15 * 1000)
        {       
            ToastColor = color;
            ToastMsg = msg;
            ToastOn = true;
            await Task.Delay(time);
            ToastOn = false;
        }

        public static void CloseToast()
        {
            ToastOn = false;
        }
    }
}
