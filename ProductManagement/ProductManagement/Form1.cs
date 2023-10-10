﻿using ProductManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductManagement
{
    public partial class Form1 : Form
    {
        List<PurchaseViewModel> lst;       
        public Form1(List<PurchaseViewModel> list)
        {
            InitializeComponent();
            lst= list;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            PurchaseReport objRpt=new PurchaseReport();
            objRpt.SetDataSource(lst);
            crystalReportViewer1.ReportSource = objRpt;
            crystalReportViewer1.Refresh();
        }
        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }
        public Form1()
        {
            InitializeComponent();
        }
    }
}
