using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
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
using System.Windows.Shapes;
using wUtility.Utils;

namespace wUtility
{
    /// <summary>
    /// Interaction logic for DoctorsPriceGroupsForm.xaml
    /// </summary>
    public partial class DoctorsPriceGroupsForm : Window
    {
       
        public static string DoctorsPriceGroupsTableName = "DoctorsPriceGroups";

        private static DataTable _doctorPriceGroupsView { get; set; }
        public static DataTable DoctorPriceGroupsView
        {
            get
            {
                if (_doctorPriceGroupsView != null)
                    return _doctorPriceGroupsView;
                else
                {
                    _doctorPriceGroupsView.Columns.Add("RowGD");
                    _doctorPriceGroupsView.Columns.Add("DrCode");
                    _doctorPriceGroupsView.Columns.Add("WCode");
                    _doctorPriceGroupsView.Columns.Add("CenterSharePrice");
                    _doctorPriceGroupsView.Columns.Add("Title");
                    _doctorPriceGroupsView.Columns.Add("DrName");
                    _doctorPriceGroupsView.Columns.Add("WName");
                    return _doctorPriceGroupsView;
                }
            }
            set
            {
                _doctorPriceGroupsTable = value;
            }
        }

        private static DataTable _doctorPriceGroupsTable { get; set; }
        public static DataTable DoctorPriceGroupsTable
        {
            get
            {
                if (_doctorPriceGroupsTable != null)
                    return _doctorPriceGroupsTable;
                else
                {
                    _doctorPriceGroupsTable = new DataTable();
                    _doctorPriceGroupsTable.TableName = DoctorsPriceGroupsTableName;
                    _doctorPriceGroupsTable.Columns.Add("RowGD");
                    _doctorPriceGroupsTable.Columns.Add("DrCode");
                    _doctorPriceGroupsTable.Columns.Add("WCode");
                    _doctorPriceGroupsTable.Columns.Add("CenterSharePrice");
                    _doctorPriceGroupsTable.Columns.Add("Title");
                    return _doctorPriceGroupsTable;
                }
            }
            set
            {
                _doctorPriceGroupsTable = value;
            }
        }
        public static DataTable DoctorsTable = new DataTable();
        public static int DoctorCode;
        public static string DoctorFullName;

        public  class Doctors
        {
            
            private  string _drName { get; set; } = "";

            public  string DrName
            {
                get
                {
                    return _drName;
                }
                set
                {
                    _drName = value;
                }
            }

            private  string _doctorCode { get; set; } = "";
            public  string DoctorCode
            {
                get
                {
                    return _doctorCode;
                }
                set
                {
                    _doctorCode = value;
                }
            }
            public  List<string> DoctorsNameList
            {
                get
                {
                    List<string> vs = DoctorsTable.AsEnumerable().Select(o => o["Name"].ToString()).ToList();
                    return vs;
                }
            }

            public  List<string> DoctorsCodeList
            {
                get
                {
                    List<string> vs = DoctorsTable.AsEnumerable().Select(o => o["Code"].ToString()).ToList();
                    return vs;
                }
            }

            public  List<Doctors> DoctorsList
            {
                get
                {
                    List<Doctors> doctors = new List<Doctors>();
                    
                    foreach(DataRow item in DoctorsTable.Rows)
                    {
                        Doctors doctor = new Doctors();
                        doctor._drName = item["Name"].ToString();
                        doctor._doctorCode = item["Code"].ToString();
                        doctors.Add(doctor);
                    }
                    return doctors;
                }
                set
                {
                    DoctorsList = value;
                }
            }
        }

        public static DoctorsPriceGroup CurrentItem
        {
            get
            {
                DoctorsPriceGroupsForm doctorsPriceGroupsForm = new DoctorsPriceGroupsForm();
                if (doctorsPriceGroupsForm.DoctorsPriceGroupCurrentRow != null)
                {
                    DoctorsPriceGroup doctorsPriceGroup = new DoctorsPriceGroup(doctorsPriceGroupsForm.DoctorsPriceGroupCurrentRow, doctorsPriceGroupsForm);
                    return doctorsPriceGroup;
                }
                else
                    return null;
            }
            set
            {
                CurrentItem = value;
            }
        }
        public class DoctorsPriceGroup : INotifyPropertyChanged
        {

            public static void BindingRow()
            {
                _doctorsPriceGroupBindRow = _doctorsPriceGroupsRow;
            }

            string specifier = "F";
            CultureInfo culture = CultureInfo.CreateSpecificCulture("fa-IR");

            private static DataRow _doctorsPriceGroupsRow;
            private static DoctorsPriceGroupsForm _doctorsPriceGroupsForm;
            public DoctorsPriceGroup(DataRow doctorsPriceGroupsRow,DoctorsPriceGroupsForm doctorsPriceGroupsForm)
            {
                _doctorsPriceGroupsRow = doctorsPriceGroupsRow;
                _doctorsPriceGroupsForm = doctorsPriceGroupsForm;
            }
            private string _rowGD { get; set; }
            public string RowGD
            {
                get
                {
                    if (!string.IsNullOrEmpty(_doctorsPriceGroupsRow["RowGD"].ToString()))
                        _rowGD = _doctorsPriceGroupsRow["RowGD"].ToString();
                    else
                        _rowGD = null;
                    return _rowGD;
            }
                set
                {
                    _rowGD = value;
                    _doctorsPriceGroupsRow["RowGD"] = _rowGD;
                    OnPropertyChanged("RowGD");
                }
            }

            private string _drCode { get; set; }
            public string DrCode
            {
                get
                {
                    if (!string.IsNullOrEmpty(_doctorsPriceGroupsRow["DrCode"].ToString()))
                        _drCode = _doctorsPriceGroupsRow["DrCode"].ToString();
                    else
                    _drCode = null;

                    return _drCode;
                }
                set
                {
                    _drCode = value;
                    _doctorsPriceGroupsRow["DrCode"] = _drCode;
                    OnPropertyChanged("DrCode");
                }
            }

            private float _wCode { get; set; }
            public float WCode
            {
                get
                {
                    if (!string.IsNullOrEmpty(_doctorsPriceGroupsRow["WCode"].ToString()))
                    {
                        _wCode = float.Parse(_doctorsPriceGroupsRow["WCode"].ToString());
                        return _wCode;
                    }
                    else
                        return 0;
                }
                set
                {
                    _wCode = value;
                    _doctorsPriceGroupsRow["WCode"] = _drCode;
                    OnPropertyChanged("WCode");
                }
            }

            private string _centerSharePrice { get; set; }
            public string CenterSharePrice
            {
                get
                {
                    if (!string.IsNullOrEmpty(_doctorsPriceGroupsRow["CenterSharePrice"].ToString()))
                    {
                        float _centerSharePriceNumeric = float.Parse(_doctorsPriceGroupsRow["CenterSharePrice"].ToString());
                        
                        _centerSharePrice = _centerSharePriceNumeric.ToString(specifier, culture);
                    }
                    else
                        _centerSharePrice = null;

                    return _centerSharePrice;
                }
                set
                {
                    _centerSharePrice = value;
                    _doctorsPriceGroupsRow["CenterSharePrice"] = _centerSharePrice;
                    OnPropertyChanged("CenterSharePrice");
                }
            }

            private string _title { get; set; }
            public string Title
            {
                get
                {
                    if (!string.IsNullOrEmpty(_doctorsPriceGroupsRow["CenterSharePrice"].ToString()))
                        _title = _doctorsPriceGroupsRow["Title"].ToString();
                    else
                        _title = null;
                    return _title;
                }
                set
                {
                    _title = value;
                    _doctorsPriceGroupsRow["Title"] = _title;
                    OnPropertyChanged("Title");
                }
            }


            private string _drName { get; set; }
            public string DrName
            {
                get
                {
                    if (DoctorsPriceGroupRowStatus.RowStatus != DoctorsPriceGroupStatus.Add
                            && DoctorsPriceGroupRowStatus.RowStatus != DoctorsPriceGroupStatus.Modify)
                    {
                        if (!string.IsNullOrEmpty(_doctorsPriceGroupsRow["DrName"].ToString()))
                            _drName = _doctorsPriceGroupsRow["DrName"].ToString();
                        else
                            _drName = null;
                    }
                    else
                    {
                        if (_doctorsPriceGroupsForm.DoctorsPriceGroupCurrentRow != null)
                            _drName = _doctorsPriceGroupsForm.DoctorsPriceGroupCurrentRow["DrName"].ToString();
                        else
                            _drName = null;
                    }
                    return _drName;
                }
                set
                {
                    _drName = value;
                    _doctorsPriceGroupsRow["DrName"] = _drName;
                    OnPropertyChanged("DrName");
                }
            }
            private string _wName { get; set; }
            public string WName
            {
                get
                {
                    if (DoctorsPriceGroupRowStatus.RowStatus != DoctorsPriceGroupStatus.Add
                            && DoctorsPriceGroupRowStatus.RowStatus != DoctorsPriceGroupStatus.Modify)
                    {
                        if (!string.IsNullOrEmpty(_doctorsPriceGroupsRow["WName"].ToString()))
                            _wName = _doctorsPriceGroupsRow["WName"].ToString();
                        else
                            _wName = null;
                    }
                    else
                         if (_doctorsPriceGroupsForm.DoctorsPriceGroupCurrentRow != null)
                            _drName = _doctorsPriceGroupsForm.DoctorsPriceGroupCurrentRow["WName"].ToString();

                    return _wName;
                }
                set
                {
                    _wName = value;
                    _doctorsPriceGroupsRow["WName"] = _wName;
                    OnPropertyChanged("WName");
                }
            }

            
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }
        private static DataRow _doctorsPriceGroupBindRow = null;  
        public static  DataRow DoctorsPriceGroupBindRow
        {
            get
            {
                using (DBHelper dbh = new DBHelper(ConnectionStrings.DrOfficeDB))
                {
                    if (_doctorsPriceGroupBindRow == null)
                    {
                        _doctorsPriceGroupBindRow.Table.TableName = DoctorsPriceGroupsTableName;
                        _doctorsPriceGroupBindRow = DoctorPriceGroupsTable.NewRow();
                        return _doctorsPriceGroupBindRow;
                    }
                    else
                        return _doctorsPriceGroupBindRow;
                }
            }
            set
            {
                _doctorsPriceGroupBindRow = value;
            }

        }

        private DataRow _doctorsPriceGroupCurrentRow = null; 
        public DataRow DoctorsPriceGroupCurrentRow
        {
            get
            {
                if (_doctorsPriceGroupCurrentRow == null)
                    _doctorsPriceGroupCurrentRow = DoctorPriceGroupsView.NewRow();

                _doctorsPriceGroupCurrentRow = uiDoctorPriceGroupsGrid.SelectedItem as DataRow;
                return _doctorsPriceGroupCurrentRow;
                //else
                //    return DoctorPriceGroupsView.NewRow();
            }

            set
            {
                _doctorsPriceGroupCurrentRow = value;
            }
        }

        private static string _rowStatus = "None";
        public class DoctorsPriceGroupRowStatus
        {
            public static string RowStatus
            {
                get
                {
                    return _rowStatus;
                }
                set
                {
                    _rowStatus = value;
                }
            }
        }

        public class DoctorsPriceGroupStatus
        {
            public static string Add
            {
                get
                {
                    return "Add";
                }
            }

            public static string Modify
            {
                get
                {
                    return "Modify";
                }
            }

            public static string Delete
            {
                get
                {
                    return "Delete";
                }
            }

            public static string None
            {
                get
                {
                    return "None";
                }
            }
        }

        public DataRow DoctorsPriceGroupSelectedRow
        {
            get
            {
                using (DBHelper dbh = new DBHelper(ConnectionStrings.DrOfficeDB))
                {
                    if (uiDoctorPriceGroupsGrid.CurrentCell != null)
                    {
                        DataRow dataRow = dbh.SelectSingle(DoctorsPriceGroupsTableName, string.Format("RowGD = {0}",
                                                                ((DataRowView)uiDoctorPriceGroupsGrid.SelectedItem).Row["RowGD"].ToString()));
                        return dataRow;
                    }
                    return null;
                }
            }
        }

        public DataRow SelectedProceedingsRow
        {
            get
            {
                if (uiDoctorPriceGroupsGrid.SelectedItem == null)
                    return null;
                return ((System.Data.DataRowView)(uiDoctorPriceGroupsGrid.SelectedItem)).Row;
            }
        }

        //public BindingSource ProceedingBindingSource = new BindingSource();


        public DoctorsPriceGroupsForm()
        {
            InitializeComponent();
            FillDoctorPriceGroupsGrid();
            FillDoctorsControl();
            ControlsShowMode();
        }




        public void FillDoctorPriceGroupsGrid()
        {
            try
            {
                using (DBHelper dbh = new DBHelper(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString))
                {
                    string query = @"
                                    select Dp.*, D.Name DrName,W.Name WName
                                    from DoctorsPriceGroups Dp 
                                    inner join Doctors D on D.Code = Dp.DrCode
                                    left join Works W on W.Code = Dp.WCode";
                    _doctorPriceGroupsView = dbh.ExecuteQuery(query);

                    query = @"      select Dp.*
                                    from DoctorsPriceGroups Dp";
                    _doctorPriceGroupsTable = dbh.ExecuteQuery(query);
                    uiNavigator.Source = DoctorPriceGroupsView.AsEnumerable().ToList();
                    uiDoctorPriceGroupsGrid.ItemsSource = DoctorPriceGroupsView;
                    //this.DataContext = DoctorsNameList;
                    //uiNavigator.DataContext = _doctorPriceGroupsTable;Source="{Binding DoctorPriceGroupsView}" 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                //RadMessageBox.ThemeName = "MaterialTeal";
                //RadMessageBox.Show(ex.Message, "خطا", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void FillDoctorsControl()
        {
            try
            {
                using (DBHelper dbh = new DBHelper(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString))
                {
                    string query = @"
                                    Select * from doctors ";
                    DoctorsTable = dbh.ExecuteQuery(query);
                    Doctors doctors = new Doctors();
                    
                    uiDoctorsNameTxt.ItemsSource = doctors.DoctorsList; //DoctorsTable.AsEnumerable().ToList();//Doctros.DoctorsNameList;
                    //uiAutocom.DisplayMemberPath = doctors.DoctorName;
                    //uiAutocom.DataContext = DoctorsTable;
                    //uiAutocom.DisplayMemberPath = "Code";
                    //uiAutocom. = Doctros.DoctorsCodeList;
                    //uiDoctorsAutoComplex.DisplayMemberPath = "Name";
                    //uiDoctorPriceGroupsGrid.ItemsSource = DoctorPriceGroups;
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                //RadMessageBox.ThemeName = "MaterialTeal";
                //RadMessageBox.Show(ex.Message, "خطا", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        


        private void uiTotalsystemBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void uiTotalsystemBtn_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void uiDoctorsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoctorsForm doctorsForm = new DoctorsForm();
                doctorsForm.ShowDialog();
                //uiDoctorText.Text = DoctorFullName;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
        }

        private void uiNewMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {

        }

        private void uiNewMenuItem_Click_1(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {

        }

        private void uiNewMenuItem_Click_2(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                ControlsEditMode();

                DoctorsPriceGroupRowStatus.RowStatus = DoctorsPriceGroupStatus.Add;

                DataRow dataRow = DoctorPriceGroupsTable.NewRow();
                Doctors doctor = new Doctors();
                DoctorsPriceGroup doctorsPriceGroup = new DoctorsPriceGroup(dataRow,this);

                uiDoctorsNameTxt.SelectedItem = doctor;

                DataContext = doctorsPriceGroup;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiEditItemMenu_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            EditActionMode();
        }

         private void EditActionMode()
        {
            try
            {

                ControlsEditMode();

                DoctorsPriceGroupRowStatus.RowStatus = DoctorsPriceGroupStatus.Modify;

                DataRow dataRow = DoctorPriceGroupsTable.AsEnumerable().Where(o => o["RowGD"].ToString() == DoctorsPriceGroupCurrentRow["RowGD"].ToString()
                                                                        && o["DrCode"].ToString() == DoctorsPriceGroupCurrentRow["DrCode"].ToString()).ToList()[0];
                DoctorsPriceGroup doctorsPriceGroup = new DoctorsPriceGroup(dataRow, this);
                DataContext = doctorsPriceGroup;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiDoctorPriceGroupsGrid_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                Doctors doctors = new Doctors();
                DoctorsPriceGroup doctorsPriceGroup = new DoctorsPriceGroup(DoctorsPriceGroupCurrentRow,this);
                doctors.DrName = doctorsPriceGroup.DrName;
                doctors.DoctorCode = doctorsPriceGroup.DrCode;
                uiDoctorsNameTxt.SelectedItem = doctors;
                DataContext = doctorsPriceGroup;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
        }

        private void uiSaveItemMenu_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                using (DBHelper dbh = new DBHelper(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString))
                {
                    // DataContext = DoctorsPriceGroupBindRow;
                    //DataRow dataRow = DataContext as DataRow;
                    //string s = DoctorsPriceGroupCurrentRow["RowGD"].ToString();

                    string[] columnsNameArray = { "RowGD", "DrCode" };
                    DoctorsPriceGroup.BindingRow();
                    if (DoctorsPriceGroupRowStatus.RowStatus == DoctorsPriceGroupStatus.Add)
                        dbh.Insert1(DoctorsPriceGroupBindRow,DoctorsPriceGroupsTableName);
                    if (DoctorsPriceGroupRowStatus.RowStatus == DoctorsPriceGroupStatus.Modify)
                        dbh.UpdateForDebug(DoctorsPriceGroupBindRow,DoctorsPriceGroupsTableName,columnsNameArray, DoctorsPriceGroupCurrentRow);

                    DoctorsPriceGroupRowStatus.RowStatus = DoctorsPriceGroupStatus.None;
                    FillDoctorPriceGroupsGrid();

                    ControlsShowMode();
                  
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //DataTable dataTable = DataContext as DataTable;
            //DataRow dataRow = DoctorsPriceGroupBindRow;
            //DataContext = DoctorsPriceGroupCurrentRow;   <!--ItemsSource="{Binding DoctorsNameList1}"-->
        }

        private void uiCancelItemMenu_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            ControlsShowMode();
            uiTabControl.SelectedIndex = 0;
        }
        private void ControlsShowMode()
        {
            uiTabControl.SelectedIndex = 0;
            uiRowText.IsEnabled = false;
            uiDoctorsNameTxt.IsEnabled = false;
            uiDoctorCodeText.IsEnabled = false;
            //uiRowText1.IsEnabled = false;
            uiPriceText.IsEnabled = false;
            uiTitleText.IsEnabled = false;
            uiDoctorPriceGroupsGrid.IsEnabled = true;
        }
        private void ControlsEditMode()
        {
            uiTabControl.SelectedIndex = 1;
            uiDoctorsNameTxt.IsEnabled = true;
            uiRowText.IsEnabled = true;
            uiDoctorCodeText.IsEnabled = true;
            //uiRowText1.IsEnabled = true;
            uiPriceText.IsEnabled = true;
            uiTitleText.IsEnabled = true;
            uiDoctorPriceGroupsGrid.IsEnabled = false;
            uiRowText.Focus();
        }

        private void uiDeleteItemMenu_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                using (DBHelper dbh = new DBHelper(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString))
                {
                    MessageBoxResult messageBoxResult = new MessageBoxResult();
                    string[] columnsNameArray = { "RowGD", "DrCode" };
                    messageBoxResult = MessageBox.Show("آیا از حذف مطمئن هستید؟", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        dbh.Delete(DoctorsPriceGroupsTableName, null, columnsNameArray, DoctorsPriceGroupCurrentRow);
                        MessageBox.Show("رکورد مورد نظر حذف شد", "Message");
                        FillDoctorPriceGroupsGrid();
                    }
                    else
                        MessageBox.Show("حذف ناموفق", "Message");

                }
                }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
        }

        private void uiAutocom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Doctors doctor = new Doctors();
            var item = uiDoctorsNameTxt.SelectedItem;
            doctor = item as Doctors;
            if (doctor != null)
            {
                if (!string.IsNullOrEmpty(doctor.DoctorCode))
                    uiDoctorCodeText.Text = doctor.DoctorCode;
            }
        }

        private void uiDoctorPriceGroupsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditActionMode();
        }
    }
}
