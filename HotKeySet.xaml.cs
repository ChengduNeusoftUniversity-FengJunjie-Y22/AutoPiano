﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FastHotKeyForWPF;

namespace AutoPiano
{
    /// <summary>
    /// HotKeySet.xaml 的交互逻辑
    /// </summary>
    public partial class HotKeySet : Page
    {
        private static ComponentInfo Info = new ComponentInfo(40, Brushes.Cyan, Brushes.Transparent, new Thickness());

        KeySelectBox k1 = PrefabComponent.GetComponent<KeySelectBox>();
        KeySelectBox k2 = PrefabComponent.GetComponent<KeySelectBox>();

        KeySelectBox k3 = PrefabComponent.GetComponent<KeySelectBox>();
        KeySelectBox k4 = PrefabComponent.GetComponent<KeySelectBox>();

        KeySelectBox k5 = PrefabComponent.GetComponent<KeySelectBox>();
        KeySelectBox k6 = PrefabComponent.GetComponent<KeySelectBox>();

        KeySelectBox k7 = PrefabComponent.GetComponent<KeySelectBox>();
        KeySelectBox k8 = PrefabComponent.GetComponent<KeySelectBox>();

        KeySelectBox k9 = PrefabComponent.GetComponent<KeySelectBox>();
        KeySelectBox k10 = PrefabComponent.GetComponent<KeySelectBox>();


        public HotKeySet()
        {
            InitializeComponent();
            LoadHotKeySetPage();
        }

        public void LoadHotKeySetPage()
        {
            b1.Child = k1;
            b2.Child = k2;
            b3.Child = k3;
            b4.Child = k4;
            b5.Child = k5;
            b6.Child = k6;
            b7.Child = k7;
            b8.Child = k8;
            b9.Child = k9;
            b10.Child = k10;

            k1.IsDefaultColorChange = false;
            k2.IsDefaultColorChange = false;
            k3.IsDefaultColorChange = false;
            k4.IsDefaultColorChange = false;
            k5.IsDefaultColorChange = false;
            k6.IsDefaultColorChange = false;
            k7.IsDefaultColorChange = false;
            k8.IsDefaultColorChange = false;
            k9.IsDefaultColorChange = false;
            k10.IsDefaultColorChange = false;

            k1.UseFatherSize<Border>();
            k2.UseFatherSize<Border>();
            k3.UseFatherSize<Border>();
            k4.UseFatherSize<Border>();
            k5.UseFatherSize<Border>();
            k6.UseFatherSize<Border>();
            k7.UseFatherSize<Border>();
            k8.UseFatherSize<Border>();
            k9.UseFatherSize<Border>();
            k10.UseFatherSize<Border>();

            k1.UseStyleProperty("MyBox");
            k2.UseStyleProperty("MyBox");
            k3.UseStyleProperty("MyBox");
            k4.UseStyleProperty("MyBox");
            k5.UseStyleProperty("MyBox");
            k6.UseStyleProperty("MyBox");
            k7.UseStyleProperty("MyBox");
            k8.UseStyleProperty("MyBox");
            k9.UseStyleProperty("MyBox");
            k10.UseStyleProperty("MyBox");

            BindingRef.Connect(k1, k6, PlayInEdit);
            BindingRef.Connect(k2, k7, PlayOutEdit);
            BindingRef.Connect(k3, k8, Pause);
            BindingRef.Connect(k4, k9, Stop);
            BindingRef.Connect(k5, k10, Visual);
        }

        public static void PlayInEdit()
        {

        }
        public static void PlayOutEdit()
        {

        }
        public static void Pause()
        {

        }
        public static void Stop()
        {

        }
        public static void Visual()
        {

        }
    }
}