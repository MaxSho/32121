﻿using DesARMA.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Security.Policy;
using NPOI.SS.Formula.Functions;
using static NPOI.HSSF.Util.HSSFColor;
using System.Windows.Documents;
using System.Diagnostics;
using SixLabors.Fonts.Tables.AdvancedTypographic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DesARMA
{
    public enum FigYesNo
    {
        Yes,
        No
    }
    public class AllDirectories
    {
        public MainConfig mainConfig { get; private set; } = null!;
        public Main main { get; private set; } = null!;
        public List<Figurant>? figurants { get; private set; } = null!; 
        ModelContext modelContext = null!;  
        RoutedEventHandler routedEventHandler = null!;
        System.Windows.Controls.TreeView treeView1 = null!;
        List<bool> listContr;
        List<bool> listSh;
        List<List<bool>?> listFigurantCheckListSHEMA;
        List<List<bool>?> listFigurantCheckListCONTROL;

        SolidColorBrush RedSolidColorBrush = null!;
        SolidColorBrush WhiteSolidColorBrush = null!;
        SolidColorBrush GreenSolidColorBrush = null!;

        //public List<bool> GetListContr 
        //{ 
        //    get 
        //    { 
        //        return GetBoolsFromString(mainConfig.Control); 
        //    } 
        //    private set 
        //    {
        //        mainConfig.Control = GetStringFromBools()
        //    } 
        //}

        public AllDirectories(Main main, MainConfig mainConfig, RoutedEventHandler routedEventHandler,
            SolidColorBrush RedSolidColorBrush, SolidColorBrush WhiteSolidColorBrush, SolidColorBrush GreenSolidColorBrush,
            System.Windows.Controls.TreeView treeView1, ModelContext modelContext
            )
        {
            this.mainConfig = mainConfig;
            this.main = main;
            this.RedSolidColorBrush = RedSolidColorBrush;
            this.WhiteSolidColorBrush = WhiteSolidColorBrush;
            this.GreenSolidColorBrush = GreenSolidColorBrush;
            this.treeView1 = treeView1;
            this.modelContext = modelContext;
            figurants = (from f in modelContext.Figurants where f.NumbInput == main.NumbInput && f.Status == 1 select f).ToList();


            listContr = GetBoolsFromString(mainConfig.Control);
            listSh = GetBoolsFromString(mainConfig.Shema);

            listFigurantCheckListSHEMA = GetFigurantCheckListNO();
            listFigurantCheckListCONTROL = GetFigurantCheckListYES();
            this.routedEventHandler = routedEventHandler;
        }
        public System.Windows.Controls.TreeView CreateNewTree()
        {
            treeView1!.Items!.Clear();

            listContr = GetBoolsFromString(mainConfig.Control);
            listSh = GetBoolsFromString(mainConfig.Shema);

            for (int i = 0; i < Reest.abbreviatedName.Count; i++)
            {
                treeView1.Items.Add(СreatePosition(i + 1, Reest.abbreviatedName[i]));
            }
            treeView1.Items.Add(СreatePosition(Reest.abbreviatedName.Count + 1, "Схеми"));
            Update();

            return treeView1;
        }
        public StackPanel СreatePosition(int idNum, string name)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;


            TreeViewItem tree = new TreeViewItem();

                var checkBoxIn = new System.Windows.Controls.CheckBox();
                //checkBoxIn.Checked += routedEventHandler;
                if (listSh != null)
                if (listSh.Count >= idNum)
                    checkBoxIn.IsChecked = listSh[idNum - 1];
                //checkBoxIn.Content = tree;
                //checkBoxIn.Margin = new Thickness(0, 1, 0, 0);

                var checkBox = new System.Windows.Controls.CheckBox();
                //checkBox.Checked += routedEventHandler;
                checkBox.Tag = false;
                checkBox.IsHitTestVisible = false;
                if(listContr!=null)
                if(listContr.Count >= idNum)
                checkBox.IsChecked = listContr[idNum - 1];
                //checkBox.Content = checkBoxIn;

                // на самому початку задано червоний колір
                
                tree.Header = $"{idNum}. " + name;

            if (IsAvailableDirectory(idNum, name))
            {
                tree.Foreground = GreenSolidColorBrush;

                if (IsEmptyDirectory(idNum, name))
                {
                    tree.Foreground = WhiteSolidColorBrush;
                }
            }
            else
            {
                tree.Foreground = RedSolidColorBrush;
            }

            //var contextmenu = new ContextMenu();
            //checkBox.ContextMenu = contextmenu;

            //var mi = new MenuItem();
            //mi.Header = "Видалити";
            //mi.Tag = $"{idNum}. {name}";
            //mi.Click += DeleteDir;
            //contextmenu.Items.Add(mi);

            if (figurants != null)
            {
                TextBlock textB = new TextBlock();
                textB.Text = "так ні";
                tree.Items.Add(textB);
            }

            if (idNum < Reest.abbreviatedName.Count)
            {
                var contextmenu = new ContextMenu();
                tree.ContextMenu = contextmenu;

                var mi = new MenuItem();
                mi.Header = "Видалити";
                mi.Tag = $"{idNum}. {Reest.abbreviatedName[idNum - 1]}";
                mi.Click += DeleteDir;
                contextmenu.Items.Add(mi);
            }


            for (int i = 0; i < figurants!.Count; i++)
            {
                    // var sdhjgf = СreatePositionFigurantCheckBoxYes(idNum, i + 1, tree.Foreground as SolidColorBrush);
                    var sdhjgf = CreatePozFig(idNum, i + 1, tree.Foreground as SolidColorBrush);
                    if (sdhjgf != null)
                    {
                        tree.Items.Add(sdhjgf);
                    }
            }
            stackPanel.Children.Add(checkBox);
            stackPanel.Children.Add(checkBoxIn);
            stackPanel.Children.Add(tree);
            return stackPanel;
        }
        public string? СreatePositionFigurantText(int idNumFig)
        {
            var treeIn = new TreeViewItem();

            if (figurants != null)
            {
                var w1 = figurants[idNumFig - 1];

                if (w1 != null)
                {
                    return GetDefInString(w1);
                }
            }
            return null;
        }
        public TreeViewItem СreatePositionFigurant(int idNumReestr, int idNumFig, SolidColorBrush color)
        {
            var treeIn = new TreeViewItem();

            if (figurants != null)
            {
                var w1 = figurants[idNumFig - 1];
            
                if (w1 != null)
                {
                    treeIn.Header = GetDefInString(w1);
                    treeIn.Foreground = color;
                    treeIn.Margin = new Thickness(-10, -3, 0, 0);

                    if (idNumReestr < Reest.abbreviatedName.Count)
                    {
                        var contextmenu = new ContextMenu();
                        treeIn.ContextMenu = contextmenu;

                        var mi = new MenuItem();
                        mi.Header = "Видалити";
                        mi.Tag = $"{idNumReestr}. {Reest.abbreviatedName[idNumReestr - 1]}";
                        mi.Click += DeleteDir;
                        contextmenu.Items.Add(mi);
                    }
                }
            }
            
            return treeIn;
        }
        //public CheckBox? СreatePositionFigurantCheckBoxYes(int idNumReestr, int idNumFig, SolidColorBrush color)
        //{
        //    var checkBox = new CheckBox();

        //    if (figurants != null)
        //    {
        //        var w1 = figurants[idNumFig - 1];

        //        if (w1 != null)
        //        {
        //            if (w1.Control != null)
        //            {
        //                var w2 = GetBoolsFromString(w1.Control);
        //                if (w2 != null)
        //                {
        //                    checkBox.IsChecked = w2[idNumReestr - 1];
        //                }
        //                checkBox.Content = СreatePositionFigurantCheckBoxNo(idNumReestr, idNumFig, color);
        //                checkBox.Click += ClickCheckBoxFigurantYES;
        //                checkBox.Tag = new Tuple<int, int, FigYesNo>(idNumFig, idNumReestr, FigYesNo.Yes);
        //                checkBox.Foreground = WhiteSolidColorBrush;
        //                checkBox.Margin = new Thickness(-20, 3, 0, 0);
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    return checkBox;
        //}
        //public CheckBox? СreatePositionFigurantCheckBoxNo(int idNumReestr, int idNumFig, SolidColorBrush color)
        //{
        //    var checkBox = new CheckBox();

        //    if (figurants != null)
        //    {
        //        var w1 = figurants[idNumFig - 1];

        //        if (w1 != null)
        //        {
        //            if (w1.Shema != null)
        //            {
        //                var w2 = GetBoolsFromString(w1.Shema);
        //                if (w2 != null)
        //                {
        //                    checkBox.IsChecked = w2[idNumReestr - 1];
        //                }
        //                checkBox.Content = СreatePositionFigurant(idNumReestr, idNumFig, color);
        //                checkBox.Click += ClickCheckBoxFigurantNO;
        //                checkBox.Tag = new Tuple<int, int, FigYesNo>(idNumFig, idNumReestr, FigYesNo.No);
        //                checkBox.Foreground = WhiteSolidColorBrush;
        //                checkBox.Margin = new Thickness(0, 1, 0, 0);
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }

        //    return checkBox;
        //}
        public StackPanel CreatePozFig(int idNumReestr, int idNumFig, SolidColorBrush color)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            var checkBox = new CheckBox();

            if (figurants != null)
            {
                var w1 = figurants[idNumFig - 1];

                if (w1 != null)
                {
                    if (w1.Control != null)
                    {
                        var w2 = GetBoolsFromString(w1.Control);
                        if (w2 != null)
                        {
                            checkBox.IsChecked = w2[idNumReestr - 1];
                        }
                        checkBox.Click += ClickCheckBoxFigurantYES;
                        checkBox.Tag = new Tuple<int, int, FigYesNo>(idNumFig, idNumReestr, FigYesNo.Yes);
                        checkBox.Foreground = WhiteSolidColorBrush;
                        checkBox.Padding = new Thickness(0, 0, 10, 0);
                    }
                    else
                    {
                        checkBox.IsChecked = false;
                        checkBox.Click += ClickCheckBoxFigurantYES;
                        checkBox.Tag = new Tuple<int, int, FigYesNo>(idNumFig, idNumReestr, FigYesNo.Yes);
                        checkBox.Foreground = WhiteSolidColorBrush;
                        checkBox.Padding = new Thickness(0, 0, 10, 0);
                    }
                }
            }
            var checkBox2 = new CheckBox();

            if (figurants != null)
            {
                var w1 = figurants[idNumFig - 1];

                if (w1 != null)
                {
                    if (w1.Shema != null)
                    {
                        var w2 = GetBoolsFromString(w1.Shema);
                        if (w2 != null)
                        {
                            checkBox2.IsChecked = w2[idNumReestr - 1];
                        }
                        checkBox2.Click += ClickCheckBoxFigurantNO;
                        checkBox2.Tag = new Tuple<int, int, FigYesNo>(idNumFig, idNumReestr, FigYesNo.No);
                        checkBox2.Foreground = WhiteSolidColorBrush;
                        //checkBox2.Padding = new Thickness(100, 0, 0, 0);
                    }
                    else
                    {
                        checkBox2.IsChecked = false;
                        checkBox2.Click += ClickCheckBoxFigurantNO;
                        checkBox2.Tag = new Tuple<int, int, FigYesNo>(idNumFig, idNumReestr, FigYesNo.No);
                        checkBox2.Foreground = WhiteSolidColorBrush;
                        checkBox2.Padding = new Thickness(0, 0, 10, 0);
                    }
                }
            }

            TextBlock textBlock = new TextBlock();
            textBlock.Text = СreatePositionFigurantText(idNumFig);
            textBlock.FontSize = 14;
            textBlock.Margin = new Thickness(+20, 0, 0, 0);
            
            stackPanel.Children.Add(checkBox);
            stackPanel.Children.Add(checkBox2);
            stackPanel.Children.Add(textBlock);

            return stackPanel;
        }
        private void DeleteDir(object sender, RoutedEventArgs e)
        {
            try
            {
                var mi = sender as MenuItem;
                if (mi != null)
                {
                    object tag = mi.Tag;
                    if (tag != null)
                    {
                        var nameD = tag as string;
                        if (nameD != null)
                        {
                            if (Directory.Exists(mainConfig.Folder + $"\\{nameD}"))
                            {
                                Directory.Delete(mainConfig.Folder + $"\\{nameD}", true);

                                int index = Convert.ToInt32(nameD.Split('.').First()) - 1;


                                var itemCh = treeView1.Items[index] as StackPanel;
                                if (itemCh != null)
                                {
                                    var che = itemCh.Children[0] as CheckBox;
                                    if (che != null)
                                    {
                                        che.IsChecked = false;
                                    }
                                    var che2 = itemCh.Children[1] as CheckBox;
                                    if (che2 != null)
                                    {
                                        che2.IsChecked = false;
                                    }
                                }

                                CreateNewTree();
                            }
                        }
                    }

                }
                Update();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private List<bool> GetBoolsFromString(string? str)
        {
            if (str == null) return null!;

            List<bool> ret = new List<bool>();
            foreach (var item in str)
            {
                if (item == '1')
                {
                    ret.Add(true);
                }
                else
                {
                    ret.Add(false);
                }
            }
            return ret;
        }
        private string? GetStringFromBools(List<bool> list)
        {
            if(list == null) return null!;
            var st = "";

            foreach (var item in list)
            {
                st += item ? "1" : "0";
            }
            if(st == "")
                return null!;
            return st;
        }
        public List<string> GetAllAvailableDirectories()
        {
            var newlist = new List<string>();
            for (int i = 0; i < Reest.abbreviatedName.Count; i++)
            {
                if (IsAvailableDirectory(i+1, Reest.abbreviatedName[i]))
                {
                    newlist.Add($"{i + 1}. {Reest.abbreviatedName[i]}");
                }
            }
            return newlist;
        }
        public List<string> GetAllNotAvailableDirectories()
        {
            var newlist = new List<string>();
            for (int i = 0; i < Reest.abbreviatedName.Count; i++)
            {
                if (!IsAvailableDirectory(i + 1, Reest.abbreviatedName[i]))
                {
                    newlist.Add($"{i + 1}. {Reest.abbreviatedName[i]}");
                }
            }
            return newlist;
        }
        public List<List<bool>?> GetFigurantCheckListNO()
        {
            List<List<bool>?> ret = new List<List<bool>?>();

            List<string>? listStringsFig = null;

            if(figurants!=null)
            listStringsFig = (from f in figurants select f.Shema).ToList();

            if (listStringsFig != null)
            {
                foreach (var itemStringFig in listStringsFig)
                {
                    var itemBoolsFig = GetBoolsFromString(itemStringFig);
                    ret.Add(itemBoolsFig);
                }
            }

            return ret;
        }
        public List<List<bool>?> GetFigurantCheckListYES()
        {
            List<List<bool>?> ret = new List<List<bool>?>();

            List<string>? listStringsFig = null;

            if (figurants != null)
                listStringsFig = (from f in figurants select f.Control).ToList();

            if (listStringsFig != null)
            {
                foreach (var itemStringFig in listStringsFig)
                {
                    var itemBoolsFig = GetBoolsFromString(itemStringFig);
                    ret.Add(itemBoolsFig);
                }
            }
            return ret;
        }
        public bool IsAvailableDirectory(int id, string abbreviatedName)
        {
            return Directory.Exists($"{mainConfig.Folder}\\{id}. {abbreviatedName}");
        }
        public bool IsEmptyDirectory(int id, string abbreviatedName)
        {
            return Directory.GetDirectories($"{mainConfig.Folder}\\{id}. {abbreviatedName}").Length == 0 &&
                Directory.GetFiles($"{mainConfig.Folder}\\{id}. {abbreviatedName}").Length == 0;
        }
        static public string? GetDefInString(Figurant d)
        {
            if (d.ResFiz != null)
            {
                string strDt = "";
                var birth = d.DtBirth;
                if (birth != null)
                {
                    return $"{d.Fio}, {birth.ToString().Substring(0, 10)} р.н., РНОКПП {d.Ipn}";
                }
                return $"{d.Fio}, РНОКПП {d.Ipn}";
            }
            else
            {
                if (d.ResUr == 2)
                    return $"{d.Name}";
                return $"{d.Name} (ЄДРПОУ {d.Code})";
            }
            return null;
        }
        private void ClickCheckBoxFigurantYES(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("ClickCheckBoxFigurantYes");

            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                var idNumReestr = checkBox.Tag;
                if (idNumReestr != null)
                {
                    Tuple<int, int, FigYesNo> tag = (Tuple<int, int, FigYesNo>)idNumReestr;

                    if(tag.Item2 - 1 < Reest.abbreviatedName.Count)
                    {
                        if (!Directory.Exists(mainConfig.Folder + $"\\{tag.Item2}. {Reest.abbreviatedName[tag.Item2 - 1]}"))
                        {
                            //MessageBox.Show("Yeeees");
                            checkBox.IsChecked = false;
                        }
                        else
                        {
                            var list = GetFigListTupleCheckBoxes(tag.Item2);
                            if (list != null)
                            {
                                var tr = list[tag.Item1 - 1];
                                if (tr != null)
                                {
                                    if (tr.Item1!.IsChecked!.Value && tr.Item2!.IsChecked!.Value)
                                    {
                                        tr.Item2!.IsChecked = false;
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        if (!Directory.Exists(mainConfig.Folder + $"\\{Reest.abbreviatedName.Count + 1}. Схеми"))
                        {
                            //MessageBox.Show("Yeeees");
                            checkBox.IsChecked = false;
                        }
                        else
                        {
                            var list = GetFigListTupleCheckBoxes(tag.Item2);
                            if (list != null)
                            {
                                var tr = list[tag.Item1 - 1];
                                if (tr != null)
                                {
                                    if (tr.Item1!.IsChecked!.Value && tr.Item2!.IsChecked!.Value)
                                    {
                                        tr.Item2!.IsChecked = false;
                                    }
                                }
                            }

                        }
                    }
                }
            }
            Update();

        }
        private void ClickCheckBoxFigurantNO(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("ClickCheckBoxNo");

            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                var idNumReestr = checkBox.Tag;
                if(idNumReestr != null)
                {
                    Tuple<int, int, FigYesNo> tag = (Tuple<int, int, FigYesNo>)idNumReestr;
                    if (!Directory.Exists(mainConfig.Folder + $"\\{tag.Item2}. {Reest.abbreviatedName[tag.Item2 - 1]}"))
                    {
                        checkBox.IsChecked = false;
                    }
                    else
                    {
                        var list = GetFigListTupleCheckBoxes(tag.Item2);
                        if(list != null)
                        {
                            var tr = list[tag.Item1 - 1];
                            if (tr != null)
                            {
                                if (tr.Item1!.IsChecked!.Value && tr.Item2!.IsChecked!.Value)
                                {
                                    tr.Item1!.IsChecked = false;
                                }
                            }
                        }
                        
                        //var st = treeView1.Items[tag.Item2 - 1] as StackPanel;
                        //if(st != null)
                        //{
                        //    var ch = st.Children[0] as CheckBox;
                        //    if (ch != null)
                        //    {
                        //        ch.Tag = false;
                        //        ch.IsChecked = isAllFigChecked(tag);
                        //    }
                        //}
                    }
                }
            }
            Update();
            
        }
        public void SaveToDB()
        {
            try
            {
                Update();

                if (figurants!=null && listFigurantCheckListCONTROL.Count != 0 && listFigurantCheckListSHEMA.Count != 0)
                for (int i = 0; i < figurants.Count; i++)
                {
                   
                        if(listFigurantCheckListCONTROL.Count > i)
                        {
                            var listCONTROL = listFigurantCheckListCONTROL[i];
                            if (listCONTROL != null)
                            {
                                figurants[i].Control = GetStringFromBools(listCONTROL);
                            }
                        }
                        else
                        {

                        }

                        if(listFigurantCheckListSHEMA.Count > i)
                        {
                            var listShema = listFigurantCheckListSHEMA[i];
                            if (listShema != null)
                            {
                                figurants[i].Shema = GetStringFromBools(listShema);
                            }
                            else
                            {

                            }
                        }
                    
                }
                mainConfig.Control = GetStringFromBools(listContr);
                mainConfig.Shema = GetStringFromBools(listSh);
                modelContext.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Update()
        {
            try
            {
                if (figurants != null)
                {
                    var listBoolControl = new List<List<bool>?>();
                    var listBoolShema = new List<List<bool>?>();
                    foreach (var item in figurants)
                    {
                        listBoolControl.Add(new List<bool>());
                        listBoolShema.Add(new List<bool>());
                    }
                    var listControl = new List<bool>();
                    var listShema = new List<bool>();
                    for (int i = 0; i < treeView1.Items.Count; i++)
                    {
                        var stackPanel1 = treeView1.Items[i] as StackPanel;

                        if (stackPanel1 != null)
                        {
                            var treeViewItem = stackPanel1.Children[2] as TreeViewItem;
                            bool isControlNice = true;
                            for (int iTVI = 0; iTVI < treeViewItem!.Items.Count; iTVI++)
                            {

                                var treeViewFigStackPanel = treeViewItem.Items[iTVI] as StackPanel;
                                if (treeViewFigStackPanel != null)
                                {
                                    var treeViewFigControl = treeViewFigStackPanel.Children[0] as CheckBox;
                                    var treeViewFigShema = treeViewFigStackPanel.Children[1] as CheckBox;

                                    if (treeViewFigShema != null && treeViewFigControl != null)
                                    {
                                        var IsCheckedTreeViewFigControl = treeViewFigControl.IsChecked;
                                        if (IsCheckedTreeViewFigControl != null)
                                        {
                                            if (listBoolControl.Count < iTVI)
                                            {
                                                listBoolControl.Add(new List<bool>());
                                            }
                                            listBoolControl[iTVI - 1]!.Add(IsCheckedTreeViewFigControl.Value);
                                        }
                                        else
                                        {
                                            if (listBoolControl.Count < iTVI)
                                            {
                                                listBoolControl.Add(new List<bool>());
                                            }
                                            listBoolControl[iTVI - 1]!.Add(false);
                                        }

                                        var IsCheckedTreeViewFigShema = treeViewFigShema.IsChecked;
                                        if (IsCheckedTreeViewFigShema != null)
                                        {
                                            if (listBoolShema.Count < iTVI)
                                            {
                                                listBoolShema.Add(new List<bool>());
                                            }
                                            listBoolShema[iTVI - 1]!.Add(IsCheckedTreeViewFigShema.Value);
                                        }
                                        else
                                        {
                                            if (listBoolShema.Count < iTVI)
                                            {
                                                listBoolShema.Add(new List<bool>());
                                            }
                                            listBoolShema[iTVI - 1]!.Add(false);
                                        }

                                        if (!listBoolShema[iTVI - 1]!.Last() && !listBoolControl[iTVI - 1]!.Last())
                                        {
                                            isControlNice = false;
                                        }

                                    }
                                }
                                else
                                {
                                    if(iTVI > 0)
                                    {
                                        listBoolControl[iTVI - 1]!.Add(false);
                                        listBoolShema[iTVI - 1]!.Add(false);
                                        isControlNice = false;
                                    }
                                    
                                }

                            }
                            var checkContr = stackPanel1.Children[0] as CheckBox;
                            var checkShema = stackPanel1.Children[1] as CheckBox;
                            if (checkContr != null && checkShema != null)
                            {
                                if (i < Reest.abbreviatedName.Count)
                                {
                                    if (!Directory.Exists(mainConfig.Folder + "\\" + $"{i + 1}. " + Reest.abbreviatedName[i]))
                                    {
                                        checkContr.IsChecked = false;
                                        checkShema.IsChecked = false;
                                    }
                                    else
                                        checkContr.IsChecked = isControlNice;
                                }
                                else
                                {
                                    if (!Directory.Exists(mainConfig.Folder + "\\" + $"{i + 1}. Схеми"))
                                    {
                                        checkContr.IsChecked = false;
                                        checkShema.IsChecked = false;
                                    }
                                    else
                                        checkContr.IsChecked = isControlNice;
                                }
                                
                                listControl.Add(checkContr.IsChecked!.Value);
                            }
                            var checkShem = stackPanel1.Children[1] as CheckBox;
                            if (checkShem != null)
                            {
                                listShema.Add(checkShem.IsChecked!.Value);
                            }
                        }
                    }
                    listFigurantCheckListCONTROL = listBoolControl;
                    listFigurantCheckListSHEMA = listBoolShema;
                    listContr = listControl;
                    listSh = listShema;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine($"StackTrace: {ex.StackTrace}\n\n");

                var trace = new StackTrace(ex, true);

                foreach (var frame in trace.GetFrames())
                {
                    var sb = new StringBuilder();

                    sb.AppendLine($"Файл: {frame.GetFileName()}");
                    sb.AppendLine($"Строка: {frame.GetFileLineNumber()}");
                    sb.AppendLine($"Столбец: {frame.GetFileColumnNumber()}");
                    sb.AppendLine($"Метод: {frame.GetMethod()}");

                    Console.WriteLine(sb);
                }
            }
        }
        public Tuple<bool, bool> isHaveEmptyCheckBoes(int idNumReestr)
        {
            bool isEmpty = true;
            bool isEmpty2 = true;
            if (treeView1.Items.Count > idNumReestr - 1)
            {
                var chCONTROL = treeView1.Items[idNumReestr - 1] as CheckBox;
                if (chCONTROL != null)
                {
                    var chSHEMA = chCONTROL.Content as CheckBox;
                    if (chSHEMA != null)
                    {
                        var viewItem = chSHEMA.Content as TreeViewItem;
                        if (viewItem != null)
                        {
                            var listFig = viewItem.Items;
                            if (listFig != null)
                            {
                                foreach (var item in listFig)
                                {
                                    var chListFig = item as CheckBox;
                                    if(chListFig != null)
                                    {
                                        var isChListFig = chListFig.IsChecked;
                                        if(isChListFig != null)
                                        {
                                            if (isChListFig.Value)
                                            {
                                                isEmpty = false;
                                            }
                                        }
                                        var chIn = chListFig.Content as CheckBox;
                                        if(chIn != null)
                                        {
                                            var isChInListFig = chIn.IsChecked;
                                            if( isChInListFig != null)
                                            {
                                                if (isChInListFig.Value)
                                                {
                                                    isEmpty2 = false;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new Tuple<bool, bool>(isEmpty, isEmpty2);
        }
        public bool isAllFigChecked(Tuple<int, int, FigYesNo> tag)
        {
            if(tag != null)
            {
                var tList = GetFigListTupleCheckBoxes(tag.Item2);

                foreach (var item in tList!)
                {
                    if(!item.Item1!.IsChecked!.Value && !item.Item2!.IsChecked!.Value)
                    {
                        return false;
                    }

                }

                return true;
            }
            return false;
        }
        public CheckBox? GetControlCheckBox(int id)
        {
            return (treeView1.Items[id - 1] as StackPanel).Children[0] as CheckBox;
        }
        public CheckBox? GetShemaCheckBox(int id)
        {
            return (treeView1.Items[id - 1] as StackPanel).Children[1] as CheckBox;
        }
        public TreeViewItem? GetTreeViewItem(int id)
        {
            return (treeView1.Items[id - 1] as StackPanel).Children[2] as TreeViewItem;
        }
        //public Tuple<List<CheckBox?>, List<CheckBox?>>? GetFigTupleListCheckBoxes(int id)
        //{
        //    List<CheckBox?> checkBoxesYes = new List<CheckBox?>();
        //    List<CheckBox?> checkBoxesNo = new List<CheckBox?>();
        //    TreeViewItem? treeViewItem = GetTreeViewItem(id);

        //    if(treeViewItem != null)
        //    {
        //        if (treeViewItem.Items != null)
        //        for (int i = 0; i < treeViewItem.Items!.Count; i++)
        //        {
        //            var stackPanel = treeViewItem.Items[i] as StackPanel;

                    

        //            checkBoxesYes.Add(stackPanel!.Children[0] as CheckBox);
        //            checkBoxesNo.Add(stackPanel!.Children[1] as CheckBox);
        //        }
        //        return Tuple.Create(checkBoxesYes, checkBoxesNo);
        //    }
        //    return null;
        //}
        public List<Tuple<CheckBox?, CheckBox?>>? GetFigListTupleCheckBoxes(int id)
        {
            TreeViewItem? treeViewItem = GetTreeViewItem(id);
            List<Tuple<CheckBox?, CheckBox?>> listRet = new List<Tuple<CheckBox?, CheckBox?>>();

            if (treeViewItem != null)
            {
                foreach (var item in treeViewItem.Items)
                {
                    var stack = item as StackPanel;
                    if(stack != null)
                    {
                        listRet.Add(Tuple.Create(stack.Children[0] as CheckBox, stack.Children[1] as CheckBox));
                    }
                }
                return listRet;
            }
            return null;
        }
    }
}