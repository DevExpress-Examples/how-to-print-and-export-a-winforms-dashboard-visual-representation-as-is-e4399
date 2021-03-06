﻿using DevExpress.DashboardCommon;
using System;

namespace DashboardExport
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        DashboardExporter dashboardExporter;
        DashboardExporter DashboardExporter {
            get {
                if(dashboardExporter == null)
                    dashboardExporter = new DashboardExporter(dashboardViewer1);
                return dashboardExporter;
            }
        }
        public Form1() {
            InitializeComponent();
            
        }
        void Form1_Load(object sender, EventArgs e) {
            Dashboard dashboard = new DevExpress.DashboardCommon.Dashboard();
            dashboard.LoadFromXml(@"Data\CustomerSupport.xml");
            dashboard.DataSources[0].Data = 
                new CustomerSupportData(
                    DataLoader.LoadCustomerSupport(), 
                    DataLoader.LoadEmployees()
                    ).CustomerSupport;
            dashboardViewer1.Dashboard = dashboard;
        }
        void button1_Click_1(object sender, EventArgs e) {
            DashboardExporter.ShowPrintPreview(true);
        }
        private void button2_Click(object sender, EventArgs e) {
            DashboardExporter.ShowPrintPreview(false);
        }
    }
}
