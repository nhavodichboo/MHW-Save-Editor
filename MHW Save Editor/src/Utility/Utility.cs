﻿using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace MHW_Save_Editor
{
    static class Utility
    {
        public static string getSteamPath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string steamPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", "");
            if (steamPath != "")
            {
                steamPath = steamPath + "/userdata";
                Console.WriteLine("Found SteamPath " + steamPath);
                bool foundGamePath = false;
                foreach (string userdir in Directory.GetDirectories(steamPath))
                {
                    foreach (string gamedir in Directory.GetDirectories(userdir))
                        if (gamedir.Contains("582010"))
                        {
                            steamPath = (gamedir + "\\remote").Replace('/', '\\');
                            Console.WriteLine("Found GameDir " + steamPath);
                            foundGamePath = true;
                            break;
                        }

                    if (foundGamePath)
                        break;
                }
            }
            return steamPath;
        }

        public static byte[] bswap(this byte[] data)
        {
            var swapped = new byte[data.Length];
            for (var i = 0; i < data.Length; i += 4)
            {
                swapped[i] = data[i + 3];
                swapped[i + 1] = data[i + 2];
                swapped[i + 2] = data[i + 1];
                swapped[i + 3] = data[i];
            }
            return swapped;
        }
        
        public static T[] Slice<T>(this T[] sliceable, int start, int end)
        {
            T[] result = new T[end-start];
            for(int i = start; i<end; i++)result[i-start]=sliceable[i];
            return result;
        }
        
       public static Int32 BMHIndexOf(this Byte[] value, Byte[] pattern)
        {
            BoyerMoore searcher = new BoyerMoore(pattern);
            return searcher.Search(value).First();
        }
        
        public static void AllowUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            //EDIT:
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));
        }
    }
    public static class SortExtensions
    {
        //  Sorts an IList<T> in place.
        public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
        {
            ArrayList.Adapter((IList)list).Sort(new ComparisonComparer<T>(comparison));
        }

        // Convenience method on IEnumerable<T> to allow passing of a
        // Comparison<T> delegate to the OrderBy method.
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, Comparison<T> comparison)
        {
            return list.OrderBy(t => t, new ComparisonComparer<T>(comparison));
        }
    }

    // Wraps a generic Comparison<T> delegate in an IComparer to make it easy
    // to use a lambda expression for methods that take an IComparer or IComparer<T>
    public class ComparisonComparer<T> : IComparer<T>, IComparer
    {
        private readonly Comparison<T> _comparison;

        public ComparisonComparer(Comparison<T> comparison)
        {
            _comparison = comparison;
        }

        public int Compare(T x, T y)
        {
            return _comparison(x, y);
        }

        public int Compare(object o1, object o2)
        {
            return _comparison((T)o1, (T)o2);
        }
    }
}
