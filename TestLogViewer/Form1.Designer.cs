namespace TestLogViewer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelDataGrid = new System.Windows.Forms.Panel();
            this.dataGridViewLogs = new System.Windows.Forms.DataGridView();
            this.flowLayoutPanelStatus = new System.Windows.Forms.FlowLayoutPanel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelCount = new System.Windows.Forms.Label();
            this.comboBoxDate = new System.Windows.Forms.ComboBox();
            this.labelDate = new System.Windows.Forms.Label();
            this.panelControls = new System.Windows.Forms.Panel();
            this.checkBoxShowOnlyPassed = new System.Windows.Forms.CheckBox();
            this.checkBoxAfterDate = new System.Windows.Forms.CheckBox();
            this.buttonLoadSerials = new System.Windows.Forms.Button();
            this.buttonExportExcel = new System.Windows.Forms.Button();
            this.buttonDownloadLogs = new System.Windows.Forms.Button();
            this.buttonClearFilter = new System.Windows.Forms.Button();
            this.checkBoxTogether = new System.Windows.Forms.CheckBox();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.labelFilter = new System.Windows.Forms.Label();
            this.panelDataGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLogs)).BeginInit();
            this.flowLayoutPanelStatus.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDataGrid
            // 
            this.panelDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDataGrid.Controls.Add(this.dataGridViewLogs);
            this.panelDataGrid.Location = new System.Drawing.Point(12, 83);
            this.panelDataGrid.Name = "panelDataGrid";
            this.panelDataGrid.Size = new System.Drawing.Size(910, 506);
            this.panelDataGrid.TabIndex = 0;
            // 
            // dataGridViewLogs
            // 
            this.dataGridViewLogs.AllowUserToAddRows = false;
            this.dataGridViewLogs.AllowUserToDeleteRows = false;
            this.dataGridViewLogs.AllowUserToResizeRows = false;
            this.dataGridViewLogs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewLogs.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewLogs.Name = "dataGridViewLogs";
            this.dataGridViewLogs.ReadOnly = true;
            this.dataGridViewLogs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewLogs.Size = new System.Drawing.Size(910, 506);
            this.dataGridViewLogs.TabIndex = 0;
            this.dataGridViewLogs.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewLogs_CellDoubleClick);
            this.dataGridViewLogs.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewLogs_ColumnHeaderMouseClick);
            this.dataGridViewLogs.ColumnHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewLogs_ColumnHeaderMouseDoubleClick);
            this.dataGridViewLogs.Paint += new System.Windows.Forms.PaintEventHandler(this.dataGridViewLogs_Paint);
            // 
            // flowLayoutPanelStatus
            // 
            this.flowLayoutPanelStatus.Controls.Add(this.labelStatus);
            this.flowLayoutPanelStatus.Controls.Add(this.labelCount);
            this.flowLayoutPanelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanelStatus.Location = new System.Drawing.Point(0, 595);
            this.flowLayoutPanelStatus.Name = "flowLayoutPanelStatus";
            this.flowLayoutPanelStatus.Size = new System.Drawing.Size(934, 26);
            this.flowLayoutPanelStatus.TabIndex = 2;
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatus.Location = new System.Drawing.Point(3, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(418, 29);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Status";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCount
            // 
            this.labelCount.Location = new System.Drawing.Point(427, 0);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(349, 29);
            this.labelCount.TabIndex = 8;
            this.labelCount.Text = "Item count:";
            this.labelCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxDate
            // 
            this.comboBoxDate.DropDownHeight = 120;
            this.comboBoxDate.DropDownWidth = 240;
            this.comboBoxDate.FormattingEnabled = true;
            this.comboBoxDate.IntegralHeight = false;
            this.comboBoxDate.Location = new System.Drawing.Point(36, 9);
            this.comboBoxDate.MaxDropDownItems = 20;
            this.comboBoxDate.Name = "comboBoxDate";
            this.comboBoxDate.Size = new System.Drawing.Size(238, 21);
            this.comboBoxDate.TabIndex = 0;
            this.comboBoxDate.SelectedValueChanged += new System.EventHandler(this.comboBoxDate_SelectedValueChanged);
            this.comboBoxDate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBoxDate_KeyPress);
            // 
            // labelDate
            // 
            this.labelDate.Location = new System.Drawing.Point(3, 0);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(30, 37);
            this.labelDate.TabIndex = 1;
            this.labelDate.Text = "Date";
            this.labelDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelControls
            // 
            this.panelControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelControls.Controls.Add(this.checkBoxShowOnlyPassed);
            this.panelControls.Controls.Add(this.checkBoxAfterDate);
            this.panelControls.Controls.Add(this.buttonLoadSerials);
            this.panelControls.Controls.Add(this.buttonExportExcel);
            this.panelControls.Controls.Add(this.buttonDownloadLogs);
            this.panelControls.Controls.Add(this.buttonClearFilter);
            this.panelControls.Controls.Add(this.checkBoxTogether);
            this.panelControls.Controls.Add(this.textBoxFilter);
            this.panelControls.Controls.Add(this.labelFilter);
            this.panelControls.Controls.Add(this.labelDate);
            this.panelControls.Controls.Add(this.comboBoxDate);
            this.panelControls.Location = new System.Drawing.Point(12, 12);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(910, 65);
            this.panelControls.TabIndex = 3;
            // 
            // checkBoxShowOnlyPassed
            // 
            this.checkBoxShowOnlyPassed.AutoSize = true;
            this.checkBoxShowOnlyPassed.Location = new System.Drawing.Point(338, 36);
            this.checkBoxShowOnlyPassed.Name = "checkBoxShowOnlyPassed";
            this.checkBoxShowOnlyPassed.Size = new System.Drawing.Size(112, 17);
            this.checkBoxShowOnlyPassed.TabIndex = 11;
            this.checkBoxShowOnlyPassed.Text = "Show only passed";
            this.checkBoxShowOnlyPassed.UseVisualStyleBackColor = true;
            // 
            // checkBoxAfterDate
            // 
            this.checkBoxAfterDate.AutoSize = true;
            this.checkBoxAfterDate.Location = new System.Drawing.Point(36, 36);
            this.checkBoxAfterDate.Name = "checkBoxAfterDate";
            this.checkBoxAfterDate.Size = new System.Drawing.Size(142, 17);
            this.checkBoxAfterDate.TabIndex = 10;
            this.checkBoxAfterDate.Text = "Show only after this date";
            this.checkBoxAfterDate.UseVisualStyleBackColor = true;
            // 
            // buttonLoadSerials
            // 
            this.buttonLoadSerials.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoadSerials.Location = new System.Drawing.Point(832, 38);
            this.buttonLoadSerials.Name = "buttonLoadSerials";
            this.buttonLoadSerials.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadSerials.TabIndex = 9;
            this.buttonLoadSerials.Text = "Load serials";
            this.buttonLoadSerials.UseVisualStyleBackColor = true;
            this.buttonLoadSerials.Click += new System.EventHandler(this.buttonLoadSerials_Click);
            // 
            // buttonExportExcel
            // 
            this.buttonExportExcel.Location = new System.Drawing.Point(660, 7);
            this.buttonExportExcel.Name = "buttonExportExcel";
            this.buttonExportExcel.Size = new System.Drawing.Size(104, 23);
            this.buttonExportExcel.TabIndex = 7;
            this.buttonExportExcel.Text = "Export Excel";
            this.buttonExportExcel.UseVisualStyleBackColor = true;
            this.buttonExportExcel.Click += new System.EventHandler(this.buttonExportExcel_Click);
            // 
            // buttonDownloadLogs
            // 
            this.buttonDownloadLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDownloadLogs.Location = new System.Drawing.Point(798, 7);
            this.buttonDownloadLogs.Name = "buttonDownloadLogs";
            this.buttonDownloadLogs.Size = new System.Drawing.Size(109, 23);
            this.buttonDownloadLogs.TabIndex = 6;
            this.buttonDownloadLogs.Text = "Download Logs";
            this.buttonDownloadLogs.UseVisualStyleBackColor = true;
            this.buttonDownloadLogs.Click += new System.EventHandler(this.buttonDownloadLogs_Click);
            // 
            // buttonClearFilter
            // 
            this.buttonClearFilter.Location = new System.Drawing.Point(579, 7);
            this.buttonClearFilter.Name = "buttonClearFilter";
            this.buttonClearFilter.Size = new System.Drawing.Size(75, 23);
            this.buttonClearFilter.TabIndex = 5;
            this.buttonClearFilter.Text = "Clear Filter";
            this.buttonClearFilter.UseVisualStyleBackColor = true;
            this.buttonClearFilter.Click += new System.EventHandler(this.buttonClearFilter_Click);
            // 
            // checkBoxTogether
            // 
            this.checkBoxTogether.AutoSize = true;
            this.checkBoxTogether.Location = new System.Drawing.Point(504, 11);
            this.checkBoxTogether.Name = "checkBoxTogether";
            this.checkBoxTogether.Size = new System.Drawing.Size(69, 17);
            this.checkBoxTogether.TabIndex = 4;
            this.checkBoxTogether.Text = "Together";
            this.checkBoxTogether.UseVisualStyleBackColor = true;
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Location = new System.Drawing.Point(338, 9);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.ShortcutsEnabled = false;
            this.textBoxFilter.Size = new System.Drawing.Size(160, 20);
            this.textBoxFilter.TabIndex = 3;
            this.textBoxFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFilter_KeyPress);
            // 
            // labelFilter
            // 
            this.labelFilter.AutoSize = true;
            this.labelFilter.Location = new System.Drawing.Point(280, 12);
            this.labelFilter.Name = "labelFilter";
            this.labelFilter.Size = new System.Drawing.Size(52, 13);
            this.labelFilter.TabIndex = 2;
            this.labelFilter.Text = "FilterData";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 621);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.flowLayoutPanelStatus);
            this.Controls.Add(this.panelDataGrid);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Log Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelDataGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLogs)).EndInit();
            this.flowLayoutPanelStatus.ResumeLayout(false);
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelDataGrid;
        private System.Windows.Forms.DataGridView dataGridViewLogs;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelStatus;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ComboBox comboBoxDate;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.CheckBox checkBoxTogether;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label labelFilter;
        private System.Windows.Forms.Button buttonClearFilter;
        private System.Windows.Forms.Button buttonDownloadLogs;
        private System.Windows.Forms.Button buttonExportExcel;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.Button buttonLoadSerials;
        private System.Windows.Forms.CheckBox checkBoxShowOnlyPassed;
        private System.Windows.Forms.CheckBox checkBoxAfterDate;
    }
}

