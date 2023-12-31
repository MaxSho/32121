﻿using DesARMA.Models;
using NPOI.SS.Formula.Functions;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesARMA.CombinedResponseWindows
{
    /// <summary>
    /// Interaction logic for SelectionOfCombinedQueryFieldsWindow.xaml
    /// </summary>
    public partial class SelectionOfCombinedQueryFieldsWindow : Window
    {
        public List<string> listNumIn;
        ModelContext modelContext;
        Main main;
        private System.Windows.Forms.Timer inactivityTimer = new System.Windows.Forms.Timer();
        public SelectionOfCombinedQueryFieldsWindow(ModelContext modelContext, Main main, List<string> listNumIn,
            System.Windows.Forms.Timer inactivityTimer)
        {
            try
            {
                InitializeComponent();

                this.listNumIn = listNumIn;
                this.modelContext = modelContext;
                this.main = main;
                this.inactivityTimer = inactivityTimer;

                inactivityTimer.Start();

                DounloadReestr();
                DateSet();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);

            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            inactivityTimer.Stop();
            try
            {
                this.DialogResult = true;

            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            inactivityTimer.Start();
        }
        private void DounloadReestr()
        {
            this.idAcc.ItemsSource = (from c in modelContext.DictCommons where c.Domain == "ACCOST" && c.Code != "МІНЮСТ" select c.Code).ToList();
        }
        private void DateSet()
        {
            var list = (from m in modelContext.Mains where listNumIn.Contains(m.NumbInput) select new
            { 
                idAcc = m.IdAcc, 
                agencyDep = m.AgencyDep,
                addr = m.Addr, 
                work = m.Work, 
                executorInit = m.ExecutorInit,
                art = m.Art
            }
            ).ToList();
            List<bool> listBoolIsAllEqually = new List<bool>();
            for (int i = 0; i < 6; i++)
            {
                listBoolIsAllEqually.Add(true);
            }
            foreach (var item in list)
            {
                if(item.idAcc != main.IdAcc)
                {
                    listBoolIsAllEqually[0] = false;
                }
                if (item.agencyDep != main.AgencyDep)
                {
                    listBoolIsAllEqually[1] = false;
                }
                if (item.addr != main.Addr)
                {
                    listBoolIsAllEqually[2] = false;
                }
                if (item.work != main.Work)
                {
                    listBoolIsAllEqually[3] = false;
                }
                if (item.executorInit != main.ExecutorInit)
                {
                    listBoolIsAllEqually[4] = false;
                }
                if (item.art != main.Art)
                {
                    listBoolIsAllEqually[5] = false;
                }
            }

            if (listBoolIsAllEqually[0])
            {
                //this.idAcc.Te = 
                var t = (from d in modelContext.DictCommons where d.Id == main.IdAcc select d.Code).FirstOrDefault();
                if (t != "МІНЮСТ")
                {
                    this.idAcc.SelectedValue = t;
                }
            }
            if (listBoolIsAllEqually[1])
            {
                this.agencyDep.Text = main.AgencyDep;
            }
            if (listBoolIsAllEqually[2])
            {
                this.addr.Text = main.Addr;
            }
            if (listBoolIsAllEqually[3])
            {
                this.work.Text = main.Work;
            }
            if (listBoolIsAllEqually[4])
            {
                this.executorInit.Text = main.ExecutorInit;
            }
            if (listBoolIsAllEqually[5])
            {
                this.art.Text = main.Art;
            }
        }

        public void SaveAllInReq()
        {
            (from m in modelContext.Mains where listNumIn.Contains(m.NumbInput) select m).ToList()
                .ForEach(m =>
                    {
                        m.IdAcc = (from dc in modelContext.DictCommons where dc.Code == this.idAcc.Text && dc.Domain == "ACCOST" select dc.Id).FirstOrDefault();
                        m.AgencyDep = this.agencyDep.Text;
                        m.Addr = this.addr.Text;
                        m.Work = this.work.Text;
                        m.ExecutorInit = this.executorInit.Text;
                        m.Art = this.art.Text;
                    }
                );
            modelContext.SaveChanges();
        }
        
        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }

        private void Window_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            inactivityTimer.Stop();
        }
    }
}
