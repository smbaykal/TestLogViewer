#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using Newtonsoft.Json.Linq;

#endregion

namespace TestLogViewer
{
    internal struct ResultCompare
    {
        public int CpuLow { get; set; }
        public int CpuHigh { get; set; }
        public int MemLow { get; set; }
        public int MemHigh { get; set; }
        public string FirmwareVersion { get; set; }
        public string HardwareId { get; set; }
        public string SerialNumber { get; set; }
        public int FpsLow { get; set; }
        public int FpsHigh { get; set; }
        public string Resolution { get; set; }
        public string Io0 { get; set; }
        public string Io1 { get; set; }
        public string Audio { get; set; }
        public string Zoom { get; set; }
    }

    public partial class Form1 : Form
    {
        private static XLWorkbook _workbook;
        private readonly List<Log> _logs = new List<Log>();
        private ResultCompare _compare;
        private DataTable _dataTable;

        public Form1()
        {
            InitializeComponent();
        }

        private void BindData()
        {
            UpdateStatus("Binding data.");
            _dataTable = ToDataTable(_logs); // takes about 100ms for 16k entries
            GetTestResultStatus();
            dataGridViewLogs.DataSource = _dataTable; // 80ms
            //dataGridViewLogs.AutoResizeColumns(); // Takes too long to finish...
            dataGridViewLogs.Refresh();

            comboBoxDate.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBoxDate.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBoxDate.DataSource = _logs.OrderBy(log => log.Date).Select(log => log.Date).Distinct().ToList();
            UpdateStatus("Data binded.");

            _logs.Clear();
            LoadColumnSettings();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var r = new Random();
            var rootJObject = new JObject();
            var array = new JArray();
            for (var i = 1; i <= 16000; i++)
            {
                var obj = new JObject
                {
                    {"Cpu load", r.Next(40, 60).ToString()},
                    {
                        "FPS",
                        (24.8 + r.NextDouble() / 5).ToString("#.######", CultureInfo.CreateSpecificCulture("en-US"))
                    },
                    {"Resolution", "1920x1080"},
                    {"Firmware version", "1.0.26"},
                    {"Free mem", r.Next(15200, 17300).ToString()},
                    {"Hardware Id", "AB-0000-0449-AA"},
                    {"IO0", "1"},
                    {"IO1", "0"},
                    {"Audio input", "1"},
                    {"Audio record", "1"},
                    {"Serial number", i.ToString().PadLeft(8, '0')},
                    {"SupervisorVersion version", "1.2.0-00046-g1e24e9f"},
                    {"Zoom", "1"}
                };
                array.Add(obj);
            }
            rootJObject.Add("", array);
            JsonHelper.SaveJsonFile("GGG.json", rootJObject);
        }

        private void buttonClearFilter_Click(object sender, EventArgs e)
        {
            if (checkBoxShowOnlyPassed.Checked)
            {
                var current = _dataTable;
                if (current.Rows.Count < 1)
                    return;
                var query = current.AsEnumerable()
                    .Where(x => x["Passed"].CastTo<bool>())
                    .Select(x => x);
                if (!query.Any())
                {
                    dataGridViewLogs.DataSource = null;
                    return;
                }
                var dt = query.CopyToDataTable();
                dataGridViewLogs.DataSource = dt;
            }
            _dataTable.DefaultView.RowFilter = null;
            dataGridViewLogs.DataSource = _dataTable;
            dataGridViewLogs.Refresh();
        }

        private void buttonDownloadLogs_Click(object sender, EventArgs e)
        {
            DownloadLogs();
        }

        private void buttonExportExcel_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var folderName = folderBrowserDialog.SelectedPath;
                var dt = GetSelectedData();
                ExportToExcel(folderName, dt);
            }
        }

        private void buttonLoadSerials_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            var result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                var serials = File.ReadAllLines(ofd.FileName).ToList();
                var query = _dataTable.AsEnumerable()
                    .Where(x => x["Serial"] != null && serials.Contains(x["Serial"].ToString()))
                    .Select(x => x);

                if (!query.Any())
                {
                    if (serials.Count > 0)
                    {
                        serials.Sort();
                        var s = "";
                        foreach (var serial in serials)
                            s += serial + "\n";
                        if (serials.Count < 10)
                            MessageBox.Show(s, "These are not found in logs");
                        else
                            MessageBox.Show("There are 10+ cameras which couldn't found in logs.\n" +
                                            "They are saved to not_founded_serials.txt",
                                "These are not found in logs.");
                        File.WriteAllText("not_founded_serials.txt", s);
                    }
                    return;
                }

                var dt = query.CopyToDataTable();
                dataGridViewLogs.DataSource = dt;

                foreach (var v in query)
                    serials.RemoveAll(item => item == v["Serial"].ToString());
                if (serials.Count > 0)
                {
                    serials.Sort();
                    var s = "";
                    foreach (var serial in serials)
                        s += serial + "\n";
                    if (serials.Count < 10)
                        MessageBox.Show(s, "These are not found in logs");
                    else
                        MessageBox.Show("There are 10+ cameras which couldn't found in logs.\n" +
                                        "They are saved to not_founded_serials.txt",
                            "These are not found in logs.");
                    File.WriteAllText("not_founded_serials.txt", s);
                }
            }
        }


        private void comboBoxDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
            {
                Filter(checkBoxAfterDate.Checked ? textBoxFilter.Text : comboBoxDate.Text);
            }
            if (!IsValidInput(e))
                e.Handled = true;
        }

        private void comboBoxDate_SelectedValueChanged(object sender, EventArgs e)
        {
            Filter(checkBoxAfterDate.Checked ? textBoxFilter.Text : comboBoxDate.Text);
        }

        private void dataGridViewLogs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridViewLogs.RowCount ||
                e.ColumnIndex < 0 || e.ColumnIndex >= dataGridViewLogs.ColumnCount)
                return;
            textBoxFilter.Text = dataGridViewLogs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        private void dataGridViewLogs_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var context = new ContextMenu();
                for (var i = 0; i < dataGridViewLogs.ColumnCount; i++)
                {
                    var columnName = dataGridViewLogs.Columns[i].Name;
                    context.MenuItems.Add(columnName, (o, args) =>
                    {
                        try
                        {
                            dataGridViewLogs.Columns[columnName].Visible =
                                !dataGridViewLogs.Columns[columnName].Visible;
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    });
                    context.MenuItems[i].Checked = dataGridViewLogs.Columns[i].Visible;
                }
                var headerRect = dataGridViewLogs.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                var p = new Point(headerRect.Left + e.X, headerRect.Top + e.Y);
                context.Show(dataGridViewLogs, p);
            }
        }

        private void dataGridViewLogs_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var columnName = dataGridViewLogs.Columns[e.ColumnIndex].Name;
            var current = GetCurrentData();
            if (current.Rows.Count < 1)
                return;
            var query = current.AsEnumerable()
                .Where(x => x[columnName] != null && x[columnName].ToString() != string.Empty)
                .GroupBy(x => x[columnName])
                .Select(x => x.Last())
                .ToList();
            if (!query.Any())
            {
                dataGridViewLogs.DataSource = null;
                return;
            }
            var dt = query.CopyToDataTable();
            dataGridViewLogs.DataSource = dt;
        }

        private void dataGridViewLogs_Paint(object sender, PaintEventArgs e)
        {
            labelCount.Text = "Item count: " + dataGridViewLogs.RowCount;
        }

        private async void DownloadLogs()
        {
            try
            {
                buttonDownloadLogs.Enabled = false;
                UpdateStatus("Downloading log file.");

                var aws = new AwsHelper();
                var getObjectResponse = await aws.S3GetObject("bilkon-camera-test01", "testLog.json");
                using (var fileStream = File.OpenWrite("testLog.json"))
                {
                    getObjectResponse.ResponseStream.CopyTo(fileStream);
                }
                UpdateStatus("Log file downloaded.");
                ReloadLogsAws();
                LoadLogs();
            }
            catch (Exception exception)
            {
                UpdateStatus("Can't download log file.", exception);
            }
            finally
            {
                buttonDownloadLogs.Enabled = true;
            }
        }

        private async void ExportToExcel(string folder, DataTable data)
        {
            buttonExportExcel.Enabled = false;
            if (!LoadExcelSchema()) return;

            try
            {
                var i = 0;
                foreach (DataRow row in data.Rows)
                {
                    i++;
                    if (row["Serial"].ToString() == string.Empty)
                        continue;
                    var t = Task.Run(() => OutputResult(folder, row));
                    await t;
                    UpdateStatus("File saved: " + row["Serial"] + ".xlsx   " + i + "/" + data.Rows.Count);
                }
                UpdateStatus("Files saved.");
            }
            catch (Exception e)
            {
                UpdateStatus("Can't write file.", e);
            }
            buttonExportExcel.Enabled = true;
        }

        private void Filter(string filter)
        {
            DrawingControl.SuspendDrawing(panelDataGrid);
            if (checkBoxTogether.Checked)
            {
                var filters = new List<string> {comboBoxDate.Text, textBoxFilter.Text};
                var columns = new List<string> {_dataTable.Columns[0].ColumnName};
                FilterData(filters, columns);
            }
            else
            {
                FilterData(filter);
            }
            var current = GetCurrentData();
            if (current.Rows.Count >= 1)
                if (checkBoxShowOnlyPassed.Checked && checkBoxAfterDate.Checked)
                {
                    var query = current.AsEnumerable()
                        .Where(x => x["Passed"].CastTo<bool>() &&
                                    string.Compare(x["Date"].ToString(), comboBoxDate.Text, StringComparison.Ordinal) >=
                                    0)
                        .Select(x => x);
                    if (!query.Any())
                    {
                        dataGridViewLogs.DataSource = null;
                    }
                    else
                    {
                        var dt = query.CopyToDataTable();
                        dataGridViewLogs.DataSource = dt;
                    }
                }
                else if (checkBoxShowOnlyPassed.Checked)
                {
                    var query = current.AsEnumerable()
                        .Where(x => x["Passed"].CastTo<bool>())
                        .Select(x => x);
                    if (!query.Any())
                    {
                        dataGridViewLogs.DataSource = null;
                    }
                    else
                    {
                        var dt = query.CopyToDataTable();
                        dataGridViewLogs.DataSource = dt;
                    }
                }
                else if (checkBoxAfterDate.Checked)
                {
                    var query = current.AsEnumerable()
                        .Where(x => string.Compare(x["Date"].ToString(), comboBoxDate.Text, StringComparison.Ordinal) >=
                                    0)
                        .Select(x => x);
                    if (!query.Any())
                    {
                        dataGridViewLogs.DataSource = null;
                    }
                    else
                    {
                        var dt = query.CopyToDataTable();
                        dataGridViewLogs.DataSource = dt;
                    }
                }
            DrawingControl.ResumeDrawing(panelDataGrid);
            dataGridViewLogs.Refresh();
        }

        private void FilterData(List<string> filters, List<string> columns)
        {
            if (columns.Count + 1 != filters.Count)
                return;
            var sbFilter = new StringBuilder();
            for (var i = 0; i < columns.Count; i++)
            {
                sbFilter.Append(columns[i] + $" LIKE '%{filters[i]}%'");
                if (i < _dataTable.Columns.Count - 1)
                    sbFilter.Append(" AND ");
            }
            if (columns.Count < _dataTable.Columns.Count)
                sbFilter.Append("(");
            var tmp = columns.Count;
            var list = new List<int>();
            for (var i = 0; i < _dataTable.Columns.Count; i++)
                if (!columns.Contains(_dataTable.Columns[i].ColumnName))
                    columns.Add(_dataTable.Columns[i].ColumnName);
                else
                    list.Add(i);
            for (var i = 0; i < columns.Count; i++)
            {
                if (!list.Contains(i))
                {
                    sbFilter.Append(columns[i] + $" LIKE '%{filters[tmp]}%'");
                    if (i < columns.Count - 1)
                        sbFilter.Append(" OR ");
                }
                if (i == columns.Count - 1)
                    sbFilter.Append(")");
            }
            var strFilter = sbFilter.ToString();
            if (strFilter.Substring(strFilter.Length - 4) == " OR ")
                strFilter = strFilter.Remove(strFilter.Length - 4);
            _dataTable.DefaultView.RowFilter = strFilter;
            dataGridViewLogs.DataSource = _dataTable;
            dataGridViewLogs.Refresh();
        }

        private void FilterData(string filter)
        {
            var columns = new List<string>();
            for (var i = 0; i < _dataTable.Columns.Count; i++)
                columns.Add(_dataTable.Columns[i].ColumnName);
            var sbFilter = new StringBuilder();
            for (var i = 0; i < columns.Count; i++)
            {
                sbFilter.Append(columns[i] + $" LIKE '%{filter}%'");
                if (i < columns.Count - 1)
                    sbFilter.Append(" OR ");
            }
            _dataTable.DefaultView.RowFilter = sbFilter.ToString();
            dataGridViewLogs.DataSource = _dataTable;
            dataGridViewLogs.Refresh();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveColumnSettings();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,
                true);

            dataGridViewLogs.DoubleBuffered(true);

            if (File.Exists("testLog.json"))
                LoadLogs();
            else
                DownloadLogs();
            LoadExcelSchema();
            SetToolTips();
        }

        private DataTable GetCurrentData()
        {
            var dt = new DataTable();

            foreach (DataGridViewColumn column in dataGridViewLogs.Columns)
                dt.Columns.Add(column.HeaderText, column.ValueType);

            foreach (DataGridViewRow row in dataGridViewLogs.Rows)
            {
                dt.Rows.Add();
                foreach (DataGridViewCell cell in row.Cells)
                    dt.Rows[dt.Rows.Count - 1][cell.ColumnIndex] = cell.Value.ToString();
            }
            return dt;
        }

        private DataTable GetSelectedData()
        {
            var dt = new DataTable();

            foreach (DataGridViewColumn column in dataGridViewLogs.Columns)
                dt.Columns.Add(column.HeaderText, column.ValueType);

            foreach (DataGridViewRow row in dataGridViewLogs.SelectedRows)
            {
                dt.Rows.Add();
                foreach (DataGridViewCell cell in row.Cells)
                    dt.Rows[dt.Rows.Count - 1][cell.ColumnIndex] = cell.Value.ToString();
            }
            return dt;
        }

        private void GetTestResultStatus()
        {
            _dataTable.Columns.Add("Passed");
            for (var i = 0; i < _dataTable.Rows.Count; i++)
            {
                var passed = true;
                var canParse = true;
                int cpuLoad;
                canParse = int.TryParse(_dataTable.Rows[i]["CpuLoad"].ToString(), out cpuLoad);
                int freeMem;
                canParse = int.TryParse(_dataTable.Rows[i]["FreeMem"].ToString(), out freeMem);
                var firmware = _dataTable.Rows[i]["Firmware"].ToString();
                var hardwareId = _dataTable.Rows[i]["HardwareId"].ToString();
                var serial = _dataTable.Rows[i]["Serial"].ToString();
                int fps;
                canParse = _dataTable.Rows[i]["Fps"].ToString().Contains('.')
                    ? int.TryParse(_dataTable.Rows[i]["Fps"].ToString().Split('.')[0], out fps)
                    : int.TryParse(_dataTable.Rows[i]["Fps"]
                        .ToString().Split(',')[0], out fps);
                var resolution = _dataTable.Rows[i]["Resolution"].ToString();
                var io0 = _dataTable.Rows[i]["Io0"].ToString();
                var io1 = _dataTable.Rows[i]["Io1"].ToString();
                var audioInput = _dataTable.Rows[i]["Audio"].ToString();
                var zoom = _dataTable.Rows[i]["Zoom"].ToString();

                if (cpuLoad < _compare.CpuLow || cpuLoad > _compare.CpuHigh ||
                    freeMem < _compare.MemLow || freeMem > _compare.MemHigh ||
                    firmware != _compare.FirmwareVersion ||
                    hardwareId != _compare.HardwareId ||
                    serial == _compare.SerialNumber ||
                    fps < _compare.FpsLow || fps > _compare.FpsHigh ||
                    resolution != _compare.Resolution ||
                    io0 != _compare.Io0 || io1 != _compare.Io1 ||
                    audioInput != _compare.Audio ||
                    zoom != _compare.Zoom || !canParse)
                    passed = false;
                _dataTable.Rows[i]["Passed"] = passed;
            }
        }

        private static bool IsValidInput(KeyPressEventArgs e)
        {
            return char.IsLetterOrDigit(e.KeyChar) ||
                   char.IsControl(e.KeyChar) ||
                   e.KeyChar == '.' ||
                   e.KeyChar == ':' ||
                   e.KeyChar == '+' ||
                   e.KeyChar == '-';
        }

        private void LoadColumnSettings()
        {
            try
            {
                var token = JsonHelper.GetJsonFile("column_settings.json");
                var obj = (JObject) token;
                foreach (var key in obj.Properties())
                    if (dataGridViewLogs.Columns.Contains(key.Name))
                        dataGridViewLogs.Columns[key.Name].Visible = obj[key.Name].ToObject<bool>();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private bool LoadExcelSchema()
        {
            if (!File.Exists("testRapor.xlsx"))
            {
                MessageBox.Show("Couldn't find testRapor.xlsx, aborting...");
                return false;
            }
            _workbook = new XLWorkbook(Path.GetFullPath("testRapor.xlsx"));

            try
            {
                var ws = _workbook.Worksheet(1);
                _compare.CpuLow = int.Parse(ws.Cell(15, 5).Value.ToString());
                _compare.CpuHigh = int.Parse(ws.Cell(15, 6).Value.ToString());
                _compare.MemLow = int.Parse(ws.Cell(16, 5).Value.ToString());
                _compare.MemHigh = int.Parse(ws.Cell(16, 6).Value.ToString());
                _compare.FirmwareVersion = ws.Cell(17, 7).Value.ToString();
                _compare.HardwareId = ws.Cell(18, 7).Value.ToString();
                _compare.SerialNumber = "00000000";
                _compare.FpsLow = int.Parse(ws.Cell(20, 5).Value.ToString());
                _compare.FpsHigh = int.Parse(ws.Cell(20, 6).Value.ToString());
                _compare.Resolution = ws.Cell(21, 7).Value.ToString();
                _compare.Io0 = "1";
                _compare.Io1 = "1";
                _compare.Audio = "1";
                _compare.Zoom = "1";
            }
            catch (Exception e)
            {
                UpdateStatus("Can't load result comparison values", e);
            }

            return true;
        }

        private async void LoadLogs()
        {
            try
            {
                UpdateStatus("Parsing logs.");

                var jToken = await JsonHelper.GetJsonFileTask("testLog.json");
                var rootJObject = (JObject) jToken;
                //var rootJObject = JsonHelper.GetJsonFile("testLog.json");
                foreach (var dateObject in rootJObject)
                foreach (var arrItem in (JArray) dateObject.Value)
                {
                    var jObject = (JObject) arrItem;
                    var log = new Log
                    {
                        Date = dateObject.Key,
                        ConnectedPort = (string) jObject.SelectToken("['ConnectedPort']"),
                        CpuLoad = (string) jObject.SelectToken("['CpuLoad']"),
                        Firmware = (string) jObject.SelectToken("['Firmware']"),
                        FreeMem = (string) jObject.SelectToken("['FreeMem']"),
                        HardwareId = (string) jObject.SelectToken("['HardwareId']"),
                        Fps = (string) jObject.SelectToken("['Fps']"),
                        Resolution = (string) jObject.SelectToken("['Resolution']"),
                        Zoom = (string) jObject.SelectToken("['Zoom']"),
                        Audio = (string) jObject.SelectToken("['Audio']"),
                        Io0 = (string) jObject.SelectToken("['Io0']"),
                        Io1 = (string) jObject.SelectToken("['Io1']"),
                        Irq0 = (string) jObject.SelectToken("['Irq0']"),
                        Irq10 = (string) jObject.SelectToken("['Irq10']"),
                        Ip = (string) jObject.SelectToken("['Ip']"),
                        Mac = (string) jObject.SelectToken("['Mac']"),
                        Serial = (string) jObject.SelectToken("['Serial']"),
                        SupervisorVersion = (string) jObject.SelectToken("['SupervisorVersion']")
                    };
                    _logs.Add(log);
                }
                UpdateStatus("Logs parsed.");
                BindData();
            }
            catch (Exception exception)
            {
                UpdateStatus("Logs couldn't read.", exception);
            }
        }

        private void OutputResult(string folder, DataRow row)
        {
            var ws = _workbook.Worksheets.First();
            ws.Cell(10, 5).SetValue(row["Serial"].ToString());
            ws.Cell(11, 5).SetValue(row["HardwareId"].ToString());
            var date = DateTime.ParseExact(row["Date"].ToString(), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            ws.Cell(12, 5).SetValue(date.ToString("dd.MM.yyyy"));


            ws.Cell(15, 8).SetValue(row["CpuLoad"].ToString());
            TestResult(ws, 15, row["CpuLoad"], true);

            ws.Cell(16, 8).SetValue(row["FreeMem"].ToString());
            TestResult(ws, 16, row["FreeMem"], true);

            ws.Cell(17, 8).SetValue(row["Firmware"].ToString());
            TestResult(ws, 17, row["Firmware"]);

            ws.Cell(18, 8).SetValue(row["HardwareId"].ToString());
            TestResult(ws, 18, row["HardwareId"]);

            ws.Cell(19, 8).SetValue(row["Serial"].ToString());
            TestResult(ws, 19, row["Serial"], _compare.SerialNumber, false);

            ws.Cell(20, 8).SetValue(row["Fps"].ToString());
            TestResult(ws, 20, row["Fps"], true);

            ws.Cell(21, 8).SetValue(row["Resolution"].ToString());
            TestResult(ws, 21, row["Resolution"]);

            ws.Cell(22, 8).SetValue(row["Zoom"].ToString() == _compare.Zoom ? "Var" : "Yok");
            TestResult(ws, 22, row["Zoom"], _compare.Zoom);

            ws.Cell(23, 8).SetValue(row["Io0"].ToString() == _compare.Io0 ? "Var" : "Yok");
            TestResult(ws, 23, row["Io0"], _compare.Io0);

            ws.Cell(24, 8).SetValue(row["Io1"].ToString() == _compare.Io1 ? "Var" : "Yok");
            TestResult(ws, 24, row["Io1"], _compare.Io1);

            ws.Cell(25, 8).SetValue(row["Audio"].ToString() == _compare.Audio ? "Var" : "Yok");
            TestResult(ws, 25, row["Audio"], _compare.Audio);

            ws.Cell(26, 8).SetValue(row["Audio"].ToString() == _compare.Audio ? "Var" : "Yok");
            TestResult(ws, 26, row["Audio"], _compare.Audio);

            var fileName = row["Serial"] + ".xlsx";
            ws.Workbook.SaveAs(folder + "\\" + fileName); // Takes time to save...
        }

        private async void ReloadLogsAws()
        {
            var aws = new AwsHelper();
            var result = await aws.InvokeLambdaFunction("test-log-save-scheduled-event");
            using (var sr = new StreamReader(result.Payload))
            {
                var str = sr.ReadLine();
                if (str != null && str.ToLower().Contains("test log save event completed"))
                    UpdateStatus("Logs queue in AWS is processed.");
            }
        }

        private void SaveColumnSettings()
        {
            var obj = new JObject();
            for (var i = 0; i < dataGridViewLogs.ColumnCount; i++)
            {
                var columnName = dataGridViewLogs.Columns[i].Name;
                obj.Add(columnName, dataGridViewLogs.Columns[i].Visible);
            }
            JsonHelper.SaveJsonFile("column_settings.json", obj);
        }

        private void SetToolTips()
        {
            new ToolTip().SetToolTip(comboBoxDate, "Select date for filtering");
            new ToolTip().SetToolTip(textBoxFilter, "Filter data");
            new ToolTip().SetToolTip(checkBoxTogether, "Filters date and text filter together");
            new ToolTip().SetToolTip(buttonClearFilter, "Clears all filters");
            new ToolTip().SetToolTip(buttonExportExcel, "Exports selected data to excel");
            new ToolTip().SetToolTip(buttonDownloadLogs, "Download logs from cloud");
            new ToolTip().SetToolTip(labelCount, "Shows current row count");
            new ToolTip().SetToolTip(buttonLoadSerials, "Load serials for filtering");
            new ToolTip().SetToolTip(labelStatus, "Shows last state");
            new ToolTip().SetToolTip(dataGridViewLogs, "Right click anywhere for column visibility settings\n" +
                                                       "One left click one of the headers for sort by respective column\n" +
                                                       "Double left click one of the headers for view distinct data by respective column");
        }

        private static void TestResult(IXLWorksheet w, int row, object cell, string compare, bool equal = true)
        {
            if (cell == null || cell.ToString().Trim() == string.Empty)
            {
                w.Cell(row, 10).SetValue("Başarısız");
                return;
            }
            if (equal)
                w.Cell(row, 10).SetValue(cell.ToString() == compare ? "Başarılı" : "Başarısız");
            else
                w.Cell(row, 10).SetValue(cell.ToString() != compare ? "Başarılı" : "Başarısız");
        }

        private static void TestResult(IXLWorksheet w, int row, object cell, bool range = false)
        {
            if (cell == null || cell.ToString().Trim() == string.Empty)
            {
                w.Cell(row, 10).SetValue("Başarısız");
                return;
            }
            if (range)
            {
                var low = int.Parse(w.Cell(row, 5).Value.ToString());
                var high = int.Parse(w.Cell(row, 6).Value.ToString());
                var value = 0;
                if (cell.ToString().Contains(".") || cell.ToString().Contains(","))
                    value = int.Parse(cell.ToString().Split('.', ',').First());
                else
                    value = int.Parse(cell.ToString());
                w.Cell(row, 10).SetValue(value >= low && value <= high ? "Başarılı" : "Başarısız");
                return;
            }
            w.Cell(row, 10).SetValue(w.Cell(row, 7).Value.ToString() == cell.ToString() ? "Başarılı" : "Başarısız");
        }

        private void textBoxFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
                Filter(textBoxFilter.Text);
            if (!IsValidInput(e))
                e.Handled = true;
        }

        private static DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                //Defining type of data column gives proper data table 
                var type = prop.PropertyType.IsGenericType &&
                           prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Nullable.GetUnderlyingType(prop.PropertyType)
                    : prop.PropertyType;
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                    //inserting property values to datatable rows
                    values[i] = props[i].GetValue(item, null);
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        private void UpdateStatus(string message)
        {
            labelStatus.Text = message;
        }

        private void UpdateStatus(string message, Exception exc)
        {
            labelStatus.Text = message;
            MessageBox.Show(exc.Message + "\n" + exc.StackTrace, exc.Source);
        }
    }
}