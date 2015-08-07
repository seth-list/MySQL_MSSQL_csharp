using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SQL db; //abstract db
        //private int indDb = 0;


        public MainWindow()
        {
            InitializeComponent();
            cb1.Items.Add("MySQL");
            cb1.Items.Add("MSSQL");

            
            cb1.SelectedIndex = 0;
            cb2.SelectedIndex = 0;
            cb3.SelectedIndex = 0;
            //инициализация раскрывающегося списка с базами данных
            cb1_init();
            //инициализация раскрывающегося списка с таблицами
            cb2_init();
        }
       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cb2.Items.Count == 0 || cb3.Items.Count == 0)
                return;

            String sdb = cb2.Text;
            String stb = cb3.Text;
            
            List<string> cols = db.getAllColumns(sdb, stb);
            DataTable dt = new DataTable();

            //очистка данных
            lstw.Columns.Clear();

            //TODO доделать
            foreach (string data in cols)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Header = data;                
                textColumn.Binding = new Binding(string.Format("[{0}]", data));
                lstw.Columns.Add(textColumn);
                //data table
                dt.Columns.Add(data);
            }
            
            List<List<string>> sds = db.getDataOfTable(sdb, stb);
            foreach (List<string> sd in sds)
            {
                dt.Rows.Add(sd.ToArray());
            }

            //context
            lstw.DataContext = dt;


        }


        private void cb1_DropDownClosed(object sender, EventArgs e)
        {
            cb1_init();
            cb2_init();
        }

        private void cb2_DropDownClosed(object sender, EventArgs e)
        {
            cb2_init();
        }



        private void cb1_init()
        {
            String sdb = cb1.Text;
            List<string> databs;
            switch (sdb)
            {
                case "MySQL":
                    db = new MySQL();
                    databs = db.getAllDataBases();
                    cb2.Items.Clear();

                    foreach (string data in databs)
                    {
                        cb2.Items.Add(data);
                    }
                    cb2.SelectedIndex = 0;

                    break;

                case "MSSQL":
                    db = new MSSQL();
                    databs = db.getAllDataBases();

                    cb2.Items.Clear();

                    foreach (string data in databs)
                    {
                        cb2.Items.Add(data);
                    }
                    cb2.SelectedIndex = 0;

                    break;
            }
        }

        private void cb2_init()
        {
            
            String sdb = cb2.Text;
            List<string> tables = db.getAllTables(sdb);

            cb3.Items.Clear();
            foreach (string data in tables)
            {
                cb3.Items.Add(data);
            }
            cb3.SelectedIndex = 0;

        }

        


    }
}

