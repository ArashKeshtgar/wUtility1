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
    /// Interaction logic for AmvalFrm.xaml
    /// </summary>
    public partial class AmvalFrm : Window
    {
        public static string AmvalIT_TableName = "AmvalIT_Tbl";

        private static DataTable _amvalPartHis_View 
        {
            get;
            set;
        }
        public static DataTable AmvalPartHis_View
        {
            get
            {
                if (_amvalPartHis_View.Columns.Count != 0)
                    return _amvalPartHis_View;
                else
                {
                    _amvalPartHis_View.Columns.Add("AmvalIT_ID");
                    _amvalPartHis_View.Columns.Add("PartHISCode");
                    _amvalPartHis_View.Columns.Add("AmvalCode");
                    _amvalPartHis_View.Columns.Add("Radif");
                    _amvalPartHis_View.Columns.Add("KalaName");
                    _amvalPartHis_View.Columns.Add("PartsName");
                    _amvalPartHis_View.Columns.Add("KindKala");
                    _amvalPartHis_View.Columns.Add("FaktorNum");
                    _amvalPartHis_View.Columns.Add("DescriptionKala");
                    return _amvalPartHis_View;
                }
            }
            set
            {
                _amvalPartHis_View = value;
            }
        }

        private static DataTable _amvalITTable { get; set; }
        public static DataTable AmvalTable
        {
            get
            {
                if (_amvalITTable != null)
                    return _amvalITTable;
                else
                {
                    _amvalITTable = new DataTable();
                    _amvalITTable.TableName = AmvalIT_TableName;
                    _amvalITTable.Columns.Add("ID");
                    _amvalITTable.Columns.Add("AmvalCode");
                    _amvalITTable.Columns.Add("Radif");
                    _amvalITTable.Columns.Add("KalaName");
                    _amvalITTable.Columns.Add("PartName");
                    _amvalITTable.Columns.Add("PartHISCode");
                    _amvalITTable.Columns.Add("KindKala");
                    _amvalITTable.Columns.Add("FaktorNum");
                    _amvalITTable.Columns.Add("DescriptionKala");
                    return _amvalITTable;
                }
            }
            set
            {
                _amvalITTable = value;
            }
        }
        public static DataTable PartTable = new DataTable();
        public static int CurretntPartCode;
        public static string CurrentPartName;

        public class Part
        {

            private string _partName { get; set; } = "";

            public string PartName
            {
                get
                {
                    return _partName;
                }
                set
                {
                    _partName = value;
                }
            }

            private int _partCode { get; set; } 
            public int PartCode
            {
                get
                {
                    return CurretntPartCode;
                }
                set
                {
                    _partCode = value;
                }
            }
            public List<string> PartsNameList
            {
                get
                {
                    List<string> vs = PartTable.AsEnumerable().Select(o => o["Name"].ToString()).ToList();
                    return vs;
                }
            }

            public List<string> PartsCodeList
            {
                get
                {
                    List<string> vs = PartTable.AsEnumerable().Select(o => o["Code"].ToString()).ToList();
                    return vs;
                }
            }

            public List<Part> PartsList
            {
                get
                {
                    List<Part> parts = new List<Part>();

                    foreach (DataRow item in PartTable.Rows)
                    {
                        Part part = new Part();
                        part._partName = item["Name"].ToString();
                        part._partCode =int.Parse(item["Code"].ToString());
                        parts.Add(part);
                    }
                    return parts;
                }
                set
                {
                    PartsList = value;
                }
            }
        }

        public static AmvalIT CurrentItem
        {
            get
            {
                AmvalFrm amvalFrm = new AmvalFrm();
                if (amvalFrm.AmvalPartsCurrentRow != null)
                {
                    AmvalIT amvalIt = new AmvalIT(amvalFrm.AmvalPartsCurrentRow, amvalFrm);
                    return amvalIt;
                }
                else
                    return null;
            }
            set
            {
                CurrentItem = value;
            }
        }
        public class AmvalIT : INotifyPropertyChanged
        {

            public static void BindingRow()
            {
                _amvalITBindRow = _amvalITRow;
            }

            string specifier = "F";
            CultureInfo culture = CultureInfo.CreateSpecificCulture("fa-IR");

            private static DataRow _amvalITRow;
            private static AmvalFrm _amvalFrm;
            public AmvalIT(DataRow amvalITRow, AmvalFrm amvalFrm)
            {
                _amvalITRow = amvalITRow;
                _amvalFrm = amvalFrm;
            }
            private int _partHISCode { get; set; }
            public int PartHISCode
            {
                get
                {
                    if (!string.IsNullOrEmpty(_amvalITRow["PartHISCode"].ToString()))
                        _partHISCode =int.Parse(_amvalITRow["PartHISCode"].ToString());
                    else
                        _partHISCode = 0;
                    return _partHISCode;
                }
                set
                {
                    _partHISCode = value;
                    _amvalITRow["PartHISCode"] = _partHISCode;
                    OnPropertyChanged("PartHISCode");
                }
            }

            private string _amvalCode { get; set; }
            public string AmvalCode
            {
                get
                {
                    if (!string.IsNullOrEmpty(_amvalITRow["AmvalCode"].ToString()))
                        _amvalCode = _amvalITRow["AmvalCode"].ToString();
                    else
                        _amvalCode = null;

                    return _amvalCode;
                }
                set
                {
                    _amvalCode = value;
                    _amvalITRow["AmvalCode"] = _amvalCode;
                    OnPropertyChanged("AmvalCode");
                }
            }

            private float _radif { get; set; }
            public float Radif
            {
                get
                {
                    if (!string.IsNullOrEmpty(_amvalITRow["Radif"].ToString()))
                    {
                        _radif = float.Parse(_amvalITRow["Radif"].ToString());
                        return _radif;
                    }
                    else
                        return 0;
                }
                set
                {
                    _radif = value;
                    _amvalITRow["Radif"] = _radif;
                    OnPropertyChanged("Radif");
                }
            }

            private string _kalaName { get; set; }
            public string KalaName
            {
                get
                {
                    if (!string.IsNullOrEmpty(_amvalITRow["KalaName"].ToString()))
                    {
                        //float _centerSharePriceNumeric = float.Parse(_amvalITRow["KalaName"].ToString());

                        _kalaName = _amvalITRow["KalaName"].ToString();
                    }
                    else
                        _kalaName = null;

                    return _kalaName;
                }
                set
                {
                    _kalaName = value;
                    _amvalITRow["KalaName"] = _kalaName;
                    OnPropertyChanged("KalaName");
                }
            }

            private string _partName { get; set; }
            public string PartName
            {
                get
                {
                    if (!string.IsNullOrEmpty(_amvalITRow["PartName"].ToString()))
                        _partName = _amvalITRow["PartName"].ToString();
                    else
                        _partName = null;
                    return _partName;
                }
                set
                {
                    _partName = value;
                    _amvalITRow["PartName"] = _partName;
                    OnPropertyChanged("PartName");
                }
            }


            private string _kindKala { get; set; }
            public string KindKala
            {
                get
                {
                    //if (AmvalITRowStatus.RowStatus != AmvalITStatus.Add
                    //        && AmvalITRowStatus.RowStatus != AmvalITStatus.Modify)
                    {
                        if (!string.IsNullOrEmpty(_amvalITRow["KindKala"].ToString()))
                            _kindKala = _amvalITRow["KindKala"].ToString();
                        else
                            _kindKala = null;
                    }
                    //else
                    //{
                    //    if (_amvalFrm._amvalPartsCurrentRow != null)
                    //        _kindKala = _amvalFrm._amvalPartsCurrentRow["KindKala"].ToString();
                    //    else
                    //        _kindKala = null;
                    //}
                    return _kindKala;
                }
                set
                {
                    _kindKala = value;
                    _amvalITRow["KindKala"] = _kindKala;
                    OnPropertyChanged("KindKala");
                }
            }
            private string _faktorNum { get; set; }
            public string FaktorNum
            {
                get
                {
                    //if (AmvalITRowStatus.RowStatus != AmvalITStatus.Add
                    //        && AmvalITRowStatus.RowStatus != AmvalITStatus.Modify)
                    {
                        if (!string.IsNullOrEmpty(_amvalITRow["FaktorNum"].ToString()))
                            _faktorNum = _amvalITRow["FaktorNum"].ToString();
                        else
                            _faktorNum = null;
                    }
                    //else
                    //     if (_amvalFrm._amvalPartsCurrentRow != null)
                    //    _kindKala = _amvalFrm._amvalPartsCurrentRow["FaktorNum"].ToString();

                    return _faktorNum;
                }
                set
                {
                    _faktorNum = value;
                    _amvalITRow["FaktorNum"] = _faktorNum;
                    OnPropertyChanged("FaktorNum");
                }
            }

            private string _descriptionKala { get; set; }
            public string DescriptionKala
            {
                get
                {
                    //if (AmvalITRowStatus.RowStatus != AmvalITStatus.Add
                    //        && AmvalITRowStatus.RowStatus != AmvalITStatus.Modify)
                    {
                        if (!string.IsNullOrEmpty(_amvalITRow["DescriptionKala"].ToString()))
                            _descriptionKala = _amvalITRow["DescriptionKala"].ToString();
                        else
                            _descriptionKala = null;
                    }
                    //else
                    //     if (_amvalFrm._amvalPartsCurrentRow != null)
                    //    _descriptionKala = _amvalFrm._amvalPartsCurrentRow["DescriptionKala"].ToString();

                    return _descriptionKala;
                }
                set
                {
                    _descriptionKala = value;
                    _amvalITRow["DescriptionKala"] = _descriptionKala;
                    OnPropertyChanged("DescriptionKala");
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
        private static DataRow _amvalITBindRow  = null;
        public static DataRow AmvalITBindRow
        {
            get
            {
                if (_amvalITBindRow == null)
                {
                    _amvalITBindRow.Table.TableName = AmvalIT_TableName;
                    _amvalITBindRow = AmvalTable.NewRow();
                    return _amvalITBindRow;
                }
                else
                    return _amvalITBindRow;
            }
            set 
            {
                _amvalITBindRow = value;
            } 

        }

        private DataRow _amvalPartsCurrentRow = null;
        public DataRow AmvalPartsCurrentRow
        {
            get
            {
                if (_amvalPartsCurrentRow == null)
                    _amvalPartsCurrentRow = _amvalPartHis_View.NewRow();

                _amvalPartsCurrentRow = uiAmalItGrid.SelectedItem as DataRow;
                return _amvalPartsCurrentRow;
                //else
                //    return DoctorPriceGroupsView.NewRow();
            }

            set
            {
                _amvalPartsCurrentRow = value;
            }
        }

        private static string _rowStatus = "None";
        public class AmvalITRowStatus
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

        public class AmvalITStatus
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

        public DataRow AmvalPartsSelectedRow
        {
            get
            {
                using (DBHelper dbh = new DBHelper(Amval.CurrentAmval.AmvalConnectionString))
                {
                    if (uiAmalItGrid.CurrentCell != null)
                    {
                        DataRow dataRow = dbh.SelectSingle(AmvalIT_TableName, string.Format("AmvalCode = {0}",
                                                                ((DataRowView)uiAmalItGrid.SelectedItem).Row["AmvalCode"].ToString()));
                        return dataRow;
                    }
                    return null;
                }
            }
        }

        //public DataRow SelectedProceedingsRow
        //{
        //    get
        //    {
        //        if (uiAmalItGrid.SelectedItem == null)
        //            return null;
        //        return ((System.Data.DataRowView)(uiAmalItGrid.SelectedItem)).Row;
        //    }
        //}





        public AmvalFrm()
        {
            InitializeComponent();
            FillAmvalITPartsGrid();
            ControlsShowMode();
            FillPartsControl();
        }


        public void FillAmvalITPartsGrid()
        {
            try
            {
                using (DBHelper dbh = new DBHelper(Amval.CurrentAmval.AmvalConnectionString))
                {
                    string query = @"
                                    Select * 
                                    from AmvalPartHis_Vw";
                    AmvalPartHis_View = dbh.ExecuteQuery(query);

                    query = @"      
                                SELECT  ID,AmvalCode, Radif, KalaName, PartName, PartHISCode, KindKala, FaktorNum, DescriptionKala
                                FROM    AmvalIT_Tbl";
                    _amvalITTable = dbh.ExecuteQuery(query);
                    uiNavigator.Source = AmvalPartHis_View.AsEnumerable().ToList();
                    uiAmalItGrid.ItemsSource = AmvalPartHis_View;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void FillPartsControl()
        {
            try
            {
                using (DBHelper dbh = new DBHelper(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString))
                {
                    string query = @"select * 
                                    from Parts";
                    PartTable = dbh.ExecuteQuery(query);

                    Part part = new Part();
                    part.PartName = CurrentPartName;
                    part.PartCode = CurretntPartCode;
                    
                    uiPartNameTxt.SelectedItem = part; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ControlsShowMode()
        {
            uiTabControl.SelectedIndex = 1;
            uiAmvalCodeText.IsEnabled = false;
            uiPartNameTxt.IsEnabled = false;
            uiPartCodeText.IsEnabled = false;
            //uiRowText1.IsEnabled = false;
            uiTabagheText.IsEnabled = false;
            uiDescriptionKalaText.IsEnabled = false;
            uiKalaNameText.IsEnabled = false;
            uiKindKalaText.IsEnabled = false;
        }
        private void ControlsEditMode()
        {
            uiTabControl.SelectedIndex = 0;
            uiAmvalCodeText.IsEnabled = true;
            uiPartNameTxt.IsEnabled = true;
            uiPartCodeText.IsEnabled = true;
            //uiRowText1.IsEnabled = true;
            uiTabagheText.IsEnabled = true;
            uiDescriptionKalaText.IsEnabled = true;
            uiKalaNameText.IsEnabled = true;
            uiKindKalaText.IsEnabled = true;
            uiKalaNameText.Focus();
        }


        private void uiDoctorPriceGroupsGrid_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {

        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void uiDoctorPriceGroupsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void uiDoctorsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PartsForm partsForm = new PartsForm();
                partsForm.ShowDialog();
                //DataRow dataRow;

                //dataRow = AmvalTable.AsEnumerable().Where(o => o["ID"].ToString() == AmvalPartsCurrentRow["AmvalIT_ID"].ToString()
                //                                                        && o["AmvalCode"].ToString() == AmvalPartsCurrentRow["AmvalCode"].ToString()).ToList()[0];
                _amvalITBindRow["PartHISCode"] = CurretntPartCode;
                _amvalITBindRow["PartName"] = CurrentPartName;

                AmvalIT amvalIT = new AmvalIT(AmvalITBindRow, this);
                DataContext = amvalIT;
                FillPartsControl();
                //uiDoctorText.Text = DoctorFullName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiAutocom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void uiNewMenuItem_Click_2(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                ControlsEditMode();

                AmvalITRowStatus.RowStatus = AmvalITStatus.Add;

                DataRow dataRow = AmvalTable.NewRow();

                Part parts = new Part();
                AmvalIT amvalIT = new AmvalIT(dataRow, this);

                //AmvalITBindRow = dataRow;
                AmvalIT.BindingRow();

                uiPartNameTxt.SelectedItem = parts;

                DataContext = amvalIT;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void EditActionMode()
        {
            try
            {

                ControlsEditMode();

                AmvalITRowStatus.RowStatus = AmvalITStatus.Modify;

                DataRow dataRow = AmvalTable.AsEnumerable().Where(o =>  //o["ID"].ToString() == AmvalPartsCurrentRow["AmvalIT_ID"].ToString() && 
                                                                        o["AmvalCode"].ToString() == AmvalPartsCurrentRow["AmvalCode"].ToString()).ToList()[0];
                AmvalIT amvalIT = new AmvalIT(dataRow, this);


                AmvalIT.BindingRow();

                DataContext = amvalIT;
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

        private void uiSaveItemMenu_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                using (DBHelper dbh = new DBHelper(Amval.CurrentAmval.AmvalConnectionString))
                {
                    string[] columnsNameArray = {"AmvalCode" };
                    AmvalIT.BindingRow();
                    if (AmvalITRowStatus.RowStatus == AmvalITStatus.Add)
                        dbh.Insert1(AmvalITBindRow, AmvalIT_TableName);
                    if (AmvalITRowStatus.RowStatus == AmvalITStatus.Modify)
                        dbh.UpdateForDebug(AmvalITBindRow, AmvalIT_TableName, columnsNameArray, AmvalPartsCurrentRow);

                    AmvalITRowStatus.RowStatus = AmvalITStatus.None;
                    FillAmvalITPartsGrid();

                    ControlsShowMode();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
       
        }

        private void uiCancelItemMenu_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            ControlsShowMode();
            uiTabControl.SelectedIndex = 0;
        }

        private void uiDeleteItemMenu_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                using (DBHelper dbh = new DBHelper(Amval.CurrentAmval.AmvalConnectionString))
                {
                    MessageBoxResult messageBoxResult = new MessageBoxResult();
                    string[] columnsNameArray = { "AmvalCode" };
                    messageBoxResult = MessageBox.Show("آیا از حذف مطمئن هستید؟", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        dbh.Delete(AmvalIT_TableName, null, columnsNameArray, AmvalPartsCurrentRow);
                        MessageBox.Show("رکورد مورد نظر حذف شد", "Message");
                        FillAmvalITPartsGrid();
                    }
                    else
                        MessageBox.Show("حذف ناموفق", "Message");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiAmalItGrid_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            try
            {
                Part part = new Part();
                AmvalIT amvalIT = new AmvalIT(AmvalPartsCurrentRow, this);
                part.PartName = amvalIT.PartName.ToString();
                part.PartCode = amvalIT.PartHISCode;
                uiPartNameTxt.SelectedItem = part;
                DataContext = amvalIT;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiAmalItGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditActionMode();
        }
    }
}
