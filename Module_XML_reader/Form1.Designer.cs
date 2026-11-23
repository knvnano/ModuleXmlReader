namespace Module_XML_reader
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnOpenFile = new Button();
            lblName = new Label();
            lblIsIrregular = new Label();
            lblModuleVersion = new Label();
            lblStartPositionOfDataGroup = new Label();
            lblChipNumber = new Label();
            lblChannelData = new Label();
            lblChannelEnableData = new Label();
            lblChipMinLawRepeatNumber = new Label();
            lblSerialRGBCode = new Label();
            lblSerialDotsNumPerColor = new Label();
            lblSerialColorNum = new Label();
            lblScreenDriveType = new Label();
            lblLineBias = new Label();
            lblRowsCtrlByDataGroup = new Label();
            lblRGBCode = new Label();
            lblScanABCDCode = new Label();
            lblDataGroupSequence = new Label();
            lblPointTableData = new Label();
            lblDataGroup = new Label();
            lblTotalPointInTable = new Label();
            lblScanType = new Label();
            lblOEPolarity = new Label();
            lblDecType = new Label();
            lblDriverTypePro = new Label();
            lblDriverChipTypeExtend = new Label();
            lblDriverChipType = new Label();
            lblDataDirectType = new Label();
            lblPixelRows = new Label();
            lblPixelCols = new Label();
            lblMissingPixels = new Label();
            textBoxPointTableData = new TextBox();
            textBoxNewScanABCDCode = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            label14 = new Label();
            label15 = new Label();
            label16 = new Label();
            label17 = new Label();
            label18 = new Label();
            label19 = new Label();
            label20 = new Label();
            label21 = new Label();
            label22 = new Label();
            label23 = new Label();
            label24 = new Label();
            label25 = new Label();
            label26 = new Label();
            label27 = new Label();
            label28 = new Label();
            label29 = new Label();
            label30 = new Label();
            label31 = new Label();
            label32 = new Label();
            buttonAbout = new Button();
            btnSaveFile = new Button();
            buttonRotateToLeft = new Button();
            buttonRotateToRight = new Button();
            buttonRotate180 = new Button();
            textBoxScanABCDCode = new TextBox();
            pictureBoxModule = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxModule).BeginInit();
            SuspendLayout();
            // 
            // btnOpenFile
            // 
            btnOpenFile.BackColor = Color.Black;
            btnOpenFile.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            btnOpenFile.ForeColor = Color.DarkOrange;
            btnOpenFile.Location = new Point(1182, 481);
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(222, 86);
            btnOpenFile.TabIndex = 0;
            btnOpenFile.Text = "Открыть файл .module";
            btnOpenFile.UseVisualStyleBackColor = false;
            btnOpenFile.Click += btnOpenFile_Click;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblName.ForeColor = Color.DarkOrange;
            lblName.Location = new Point(276, 19);
            lblName.Name = "lblName";
            lblName.Size = new Size(88, 32);
            lblName.TabIndex = 1;
            lblName.Text = "Name:";
            // 
            // lblIsIrregular
            // 
            lblIsIrregular.AutoSize = true;
            lblIsIrregular.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblIsIrregular.ForeColor = Color.DarkOrange;
            lblIsIrregular.Location = new Point(845, 489);
            lblIsIrregular.Name = "lblIsIrregular";
            lblIsIrregular.Size = new Size(142, 32);
            lblIsIrregular.TabIndex = 2;
            lblIsIrregular.Text = "IsIrregular:";
            // 
            // lblModuleVersion
            // 
            lblModuleVersion.AutoSize = true;
            lblModuleVersion.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblModuleVersion.ForeColor = Color.DarkOrange;
            lblModuleVersion.Location = new Point(839, 438);
            lblModuleVersion.Name = "lblModuleVersion";
            lblModuleVersion.Size = new Size(194, 32);
            lblModuleVersion.TabIndex = 3;
            lblModuleVersion.Text = "ModuleVersion:";
            // 
            // lblStartPositionOfDataGroup
            // 
            lblStartPositionOfDataGroup.AutoSize = true;
            lblStartPositionOfDataGroup.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblStartPositionOfDataGroup.ForeColor = Color.DarkOrange;
            lblStartPositionOfDataGroup.Location = new Point(832, 392);
            lblStartPositionOfDataGroup.Name = "lblStartPositionOfDataGroup";
            lblStartPositionOfDataGroup.Size = new Size(320, 32);
            lblStartPositionOfDataGroup.TabIndex = 4;
            lblStartPositionOfDataGroup.Text = "StartPositionOfDataGroup:";
            // 
            // lblChipNumber
            // 
            lblChipNumber.AutoSize = true;
            lblChipNumber.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblChipNumber.ForeColor = Color.DarkOrange;
            lblChipNumber.Location = new Point(833, 349);
            lblChipNumber.Name = "lblChipNumber";
            lblChipNumber.Size = new Size(167, 32);
            lblChipNumber.TabIndex = 5;
            lblChipNumber.Text = "ChipNumber:";
            // 
            // lblChannelData
            // 
            lblChannelData.AutoSize = true;
            lblChannelData.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblChannelData.ForeColor = Color.DarkOrange;
            lblChannelData.Location = new Point(832, 296);
            lblChannelData.Name = "lblChannelData";
            lblChannelData.Size = new Size(166, 32);
            lblChannelData.TabIndex = 6;
            lblChannelData.Text = "ChannelData:";
            // 
            // lblChannelEnableData
            // 
            lblChannelEnableData.AutoSize = true;
            lblChannelEnableData.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblChannelEnableData.ForeColor = Color.DarkOrange;
            lblChannelEnableData.Location = new Point(832, 202);
            lblChannelEnableData.Name = "lblChannelEnableData";
            lblChannelEnableData.Size = new Size(242, 32);
            lblChannelEnableData.TabIndex = 7;
            lblChannelEnableData.Text = "ChannelEnableData:";
            // 
            // lblChipMinLawRepeatNumber
            // 
            lblChipMinLawRepeatNumber.AutoSize = true;
            lblChipMinLawRepeatNumber.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblChipMinLawRepeatNumber.ForeColor = Color.DarkOrange;
            lblChipMinLawRepeatNumber.Location = new Point(832, 148);
            lblChipMinLawRepeatNumber.Name = "lblChipMinLawRepeatNumber";
            lblChipMinLawRepeatNumber.Size = new Size(341, 32);
            lblChipMinLawRepeatNumber.TabIndex = 8;
            lblChipMinLawRepeatNumber.Text = "ChipMinLawRepeatNumber: ";
            // 
            // lblSerialRGBCode
            // 
            lblSerialRGBCode.AutoSize = true;
            lblSerialRGBCode.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSerialRGBCode.ForeColor = Color.DarkOrange;
            lblSerialRGBCode.Location = new Point(832, 106);
            lblSerialRGBCode.Name = "lblSerialRGBCode";
            lblSerialRGBCode.Size = new Size(190, 32);
            lblSerialRGBCode.TabIndex = 9;
            lblSerialRGBCode.Text = "SerialRGBCode:";
            // 
            // lblSerialDotsNumPerColor
            // 
            lblSerialDotsNumPerColor.AutoSize = true;
            lblSerialDotsNumPerColor.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSerialDotsNumPerColor.ForeColor = Color.DarkOrange;
            lblSerialDotsNumPerColor.Location = new Point(832, 61);
            lblSerialDotsNumPerColor.Name = "lblSerialDotsNumPerColor";
            lblSerialDotsNumPerColor.Size = new Size(293, 32);
            lblSerialDotsNumPerColor.TabIndex = 10;
            lblSerialDotsNumPerColor.Text = "SerialDotsNumPerColor:";
            // 
            // lblSerialColorNum
            // 
            lblSerialColorNum.AutoSize = true;
            lblSerialColorNum.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSerialColorNum.ForeColor = Color.DarkOrange;
            lblSerialColorNum.Location = new Point(832, 19);
            lblSerialColorNum.Name = "lblSerialColorNum";
            lblSerialColorNum.Size = new Size(202, 32);
            lblSerialColorNum.TabIndex = 11;
            lblSerialColorNum.Text = "SerialColorNum:";
            // 
            // lblScreenDriveType
            // 
            lblScreenDriveType.AutoSize = true;
            lblScreenDriveType.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblScreenDriveType.ForeColor = Color.DarkOrange;
            lblScreenDriveType.Location = new Point(276, 187);
            lblScreenDriveType.Name = "lblScreenDriveType";
            lblScreenDriveType.Size = new Size(212, 32);
            lblScreenDriveType.TabIndex = 12;
            lblScreenDriveType.Text = "ScreenDriveType:";
            // 
            // lblLineBias
            // 
            lblLineBias.AutoSize = true;
            lblLineBias.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblLineBias.ForeColor = Color.DarkOrange;
            lblLineBias.Location = new Point(274, 148);
            lblLineBias.Name = "lblLineBias";
            lblLineBias.Size = new Size(114, 32);
            lblLineBias.TabIndex = 13;
            lblLineBias.Text = "LineBias:";
            // 
            // lblRowsCtrlByDataGroup
            // 
            lblRowsCtrlByDataGroup.AutoSize = true;
            lblRowsCtrlByDataGroup.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblRowsCtrlByDataGroup.ForeColor = Color.DarkOrange;
            lblRowsCtrlByDataGroup.Location = new Point(832, 247);
            lblRowsCtrlByDataGroup.Name = "lblRowsCtrlByDataGroup";
            lblRowsCtrlByDataGroup.Size = new Size(275, 32);
            lblRowsCtrlByDataGroup.TabIndex = 14;
            lblRowsCtrlByDataGroup.Text = "RowsCtrlByDataGroup:";
            // 
            // lblRGBCode
            // 
            lblRGBCode.AutoSize = true;
            lblRGBCode.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblRGBCode.ForeColor = Color.DarkOrange;
            lblRGBCode.Location = new Point(275, 583);
            lblRGBCode.Name = "lblRGBCode";
            lblRGBCode.Size = new Size(127, 32);
            lblRGBCode.TabIndex = 15;
            lblRGBCode.Text = "RGBCode:";
            // 
            // lblScanABCDCode
            // 
            lblScanABCDCode.AutoSize = true;
            lblScanABCDCode.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblScanABCDCode.ForeColor = Color.DarkOrange;
            lblScanABCDCode.Location = new Point(12, 721);
            lblScanABCDCode.Name = "lblScanABCDCode";
            lblScanABCDCode.Size = new Size(197, 32);
            lblScanABCDCode.TabIndex = 17;
            lblScanABCDCode.Text = "ScanABCDCode:";
            // 
            // lblDataGroupSequence
            // 
            lblDataGroupSequence.AutoSize = true;
            lblDataGroupSequence.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblDataGroupSequence.ForeColor = Color.DarkOrange;
            lblDataGroupSequence.Location = new Point(839, 539);
            lblDataGroupSequence.Name = "lblDataGroupSequence";
            lblDataGroupSequence.Size = new Size(255, 32);
            lblDataGroupSequence.TabIndex = 18;
            lblDataGroupSequence.Text = "DataGroupSequence:";
            // 
            // lblPointTableData
            // 
            lblPointTableData.AutoSize = true;
            lblPointTableData.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPointTableData.ForeColor = Color.OldLace;
            lblPointTableData.Location = new Point(436, 164);
            lblPointTableData.Name = "lblPointTableData";
            lblPointTableData.Size = new Size(0, 32);
            lblPointTableData.TabIndex = 19;
            lblPointTableData.Click += lblPointTableData_Click;
            // 
            // lblDataGroup
            // 
            lblDataGroup.AutoSize = true;
            lblDataGroup.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblDataGroup.ForeColor = Color.DarkOrange;
            lblDataGroup.Location = new Point(276, 539);
            lblDataGroup.Name = "lblDataGroup";
            lblDataGroup.Size = new Size(146, 32);
            lblDataGroup.TabIndex = 20;
            lblDataGroup.Text = "DataGroup:";
            // 
            // lblTotalPointInTable
            // 
            lblTotalPointInTable.AutoSize = true;
            lblTotalPointInTable.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTotalPointInTable.ForeColor = Color.DarkOrange;
            lblTotalPointInTable.Location = new Point(276, 501);
            lblTotalPointInTable.Name = "lblTotalPointInTable";
            lblTotalPointInTable.Size = new Size(220, 32);
            lblTotalPointInTable.TabIndex = 21;
            lblTotalPointInTable.Text = "TotalPointInTable:";
            // 
            // lblScanType
            // 
            lblScanType.AutoSize = true;
            lblScanType.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblScanType.ForeColor = Color.DarkOrange;
            lblScanType.Location = new Point(276, 462);
            lblScanType.Name = "lblScanType";
            lblScanType.Size = new Size(128, 32);
            lblScanType.TabIndex = 22;
            lblScanType.Text = "ScanType:";
            // 
            // lblOEPolarity
            // 
            lblOEPolarity.AutoSize = true;
            lblOEPolarity.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblOEPolarity.ForeColor = Color.DarkOrange;
            lblOEPolarity.Location = new Point(276, 424);
            lblOEPolarity.Name = "lblOEPolarity";
            lblOEPolarity.Size = new Size(140, 32);
            lblOEPolarity.TabIndex = 23;
            lblOEPolarity.Text = "OEPolarity:";
            // 
            // lblDecType
            // 
            lblDecType.AutoSize = true;
            lblDecType.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblDecType.ForeColor = Color.DarkOrange;
            lblDecType.Location = new Point(276, 389);
            lblDecType.Name = "lblDecType";
            lblDecType.Size = new Size(117, 32);
            lblDecType.TabIndex = 24;
            lblDecType.Text = "DecType:";
            // 
            // lblDriverTypePro
            // 
            lblDriverTypePro.AutoSize = true;
            lblDriverTypePro.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblDriverTypePro.ForeColor = Color.DarkOrange;
            lblDriverTypePro.Location = new Point(276, 349);
            lblDriverTypePro.Name = "lblDriverTypePro";
            lblDriverTypePro.Size = new Size(186, 32);
            lblDriverTypePro.TabIndex = 25;
            lblDriverTypePro.Text = "DriverTypePro:";
            // 
            // lblDriverChipTypeExtend
            // 
            lblDriverChipTypeExtend.AutoSize = true;
            lblDriverChipTypeExtend.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblDriverChipTypeExtend.ForeColor = Color.DarkOrange;
            lblDriverChipTypeExtend.Location = new Point(276, 311);
            lblDriverChipTypeExtend.Name = "lblDriverChipTypeExtend";
            lblDriverChipTypeExtend.Size = new Size(275, 32);
            lblDriverChipTypeExtend.TabIndex = 26;
            lblDriverChipTypeExtend.Text = "DriverChipTypeExtend:";
            // 
            // lblDriverChipType
            // 
            lblDriverChipType.AutoSize = true;
            lblDriverChipType.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblDriverChipType.ForeColor = Color.DarkOrange;
            lblDriverChipType.Location = new Point(276, 268);
            lblDriverChipType.Name = "lblDriverChipType";
            lblDriverChipType.Size = new Size(197, 32);
            lblDriverChipType.TabIndex = 27;
            lblDriverChipType.Text = "DriverChipType:";
            // 
            // lblDataDirectType
            // 
            lblDataDirectType.AutoSize = true;
            lblDataDirectType.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblDataDirectType.ForeColor = Color.DarkOrange;
            lblDataDirectType.Location = new Point(276, 226);
            lblDataDirectType.Name = "lblDataDirectType";
            lblDataDirectType.Size = new Size(197, 32);
            lblDataDirectType.TabIndex = 28;
            lblDataDirectType.Text = "DataDirectType:";
            // 
            // lblPixelRows
            // 
            lblPixelRows.AutoSize = true;
            lblPixelRows.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPixelRows.ForeColor = Color.DarkOrange;
            lblPixelRows.Location = new Point(274, 106);
            lblPixelRows.Name = "lblPixelRows";
            lblPixelRows.Size = new Size(129, 32);
            lblPixelRows.TabIndex = 29;
            lblPixelRows.Text = "PixelRows";
            // 
            // lblPixelCols
            // 
            lblPixelCols.AutoSize = true;
            lblPixelCols.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPixelCols.ForeColor = Color.DarkOrange;
            lblPixelCols.Location = new Point(274, 65);
            lblPixelCols.Name = "lblPixelCols";
            lblPixelCols.Size = new Size(117, 32);
            lblPixelCols.TabIndex = 30;
            lblPixelCols.Text = "PixelCols";
            // 
            // lblMissingPixels
            // 
            lblMissingPixels.AutoSize = true;
            lblMissingPixels.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblMissingPixels.ForeColor = Color.GreenYellow;
            lblMissingPixels.Location = new Point(272, 661);
            lblMissingPixels.Name = "lblMissingPixels";
            lblMissingPixels.Size = new Size(293, 32);
            lblMissingPixels.TabIndex = 31;
            lblMissingPixels.Text = "Загрузите файл модуля";
            // 
            // textBoxPointTableData
            // 
            textBoxPointTableData.Location = new Point(1182, 61);
            textBoxPointTableData.Multiline = true;
            textBoxPointTableData.Name = "textBoxPointTableData";
            textBoxPointTableData.ScrollBars = ScrollBars.Vertical;
            textBoxPointTableData.Size = new Size(564, 46);
            textBoxPointTableData.TabIndex = 33;
            // 
            // textBoxNewScanABCDCode
            // 
            textBoxNewScanABCDCode.Location = new Point(1182, 167);
            textBoxNewScanABCDCode.Multiline = true;
            textBoxNewScanABCDCode.Name = "textBoxNewScanABCDCode";
            textBoxNewScanABCDCode.ScrollBars = ScrollBars.Vertical;
            textBoxNewScanABCDCode.Size = new Size(566, 52);
            textBoxNewScanABCDCode.TabIndex = 34;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.ForeColor = Color.OldLace;
            label1.Location = new Point(1371, 19);
            label1.Name = "label1";
            label1.Size = new Size(187, 32);
            label1.TabIndex = 35;
            label1.Text = "PointTableData";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label2.ForeColor = Color.OldLace;
            label2.Location = new Point(1358, 128);
            label2.Name = "label2";
            label2.Size = new Size(241, 32);
            label2.TabIndex = 36;
            label2.Text = "NewScanABCDCode";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label3.ForeColor = Color.OldLace;
            label3.Location = new Point(7, 61);
            label3.Name = "label3";
            label3.Size = new Size(117, 32);
            label3.TabIndex = 53;
            label3.Text = "PixelCols";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label4.ForeColor = Color.OldLace;
            label4.Location = new Point(5, 106);
            label4.Name = "label4";
            label4.Size = new Size(129, 32);
            label4.TabIndex = 52;
            label4.Text = "PixelRows";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label5.ForeColor = Color.OldLace;
            label5.Location = new Point(7, 226);
            label5.Name = "label5";
            label5.Size = new Size(197, 32);
            label5.TabIndex = 51;
            label5.Text = "DataDirectType:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label6.ForeColor = Color.OldLace;
            label6.Location = new Point(7, 268);
            label6.Name = "label6";
            label6.Size = new Size(197, 32);
            label6.TabIndex = 50;
            label6.Text = "DriverChipType:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label7.ForeColor = Color.OldLace;
            label7.Location = new Point(7, 311);
            label7.Name = "label7";
            label7.Size = new Size(275, 32);
            label7.TabIndex = 49;
            label7.Text = "DriverChipTypeExtend:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label8.ForeColor = Color.OldLace;
            label8.Location = new Point(7, 349);
            label8.Name = "label8";
            label8.Size = new Size(186, 32);
            label8.TabIndex = 48;
            label8.Text = "DriverTypePro:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label9.ForeColor = Color.OldLace;
            label9.Location = new Point(7, 389);
            label9.Name = "label9";
            label9.Size = new Size(117, 32);
            label9.TabIndex = 47;
            label9.Text = "DecType:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label10.ForeColor = Color.OldLace;
            label10.Location = new Point(7, 424);
            label10.Name = "label10";
            label10.Size = new Size(140, 32);
            label10.TabIndex = 46;
            label10.Text = "OEPolarity:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label11.ForeColor = Color.OldLace;
            label11.Location = new Point(7, 462);
            label11.Name = "label11";
            label11.Size = new Size(128, 32);
            label11.TabIndex = 45;
            label11.Text = "ScanType:";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label12.ForeColor = Color.OldLace;
            label12.Location = new Point(7, 501);
            label12.Name = "label12";
            label12.Size = new Size(220, 32);
            label12.TabIndex = 44;
            label12.Text = "TotalPointInTable:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label13.ForeColor = Color.OldLace;
            label13.Location = new Point(5, 539);
            label13.Name = "label13";
            label13.Size = new Size(146, 32);
            label13.TabIndex = 43;
            label13.Text = "DataGroup:";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label14.ForeColor = Color.OldLace;
            label14.Location = new Point(194, 164);
            label14.Name = "label14";
            label14.Size = new Size(0, 32);
            label14.TabIndex = 42;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label15.ForeColor = Color.OldLace;
            label15.Location = new Point(5, 623);
            label15.Name = "label15";
            label15.Size = new Size(197, 32);
            label15.TabIndex = 41;
            label15.Text = "ScanABCDCode:";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label16.ForeColor = Color.OldLace;
            label16.Location = new Point(7, 583);
            label16.Name = "label16";
            label16.Size = new Size(127, 32);
            label16.TabIndex = 40;
            label16.Text = "RGBCode:";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label17.ForeColor = Color.OldLace;
            label17.Location = new Point(5, 148);
            label17.Name = "label17";
            label17.Size = new Size(114, 32);
            label17.TabIndex = 39;
            label17.Text = "LineBias:";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label18.ForeColor = Color.OldLace;
            label18.Location = new Point(7, 187);
            label18.Name = "label18";
            label18.Size = new Size(212, 32);
            label18.TabIndex = 38;
            label18.Text = "ScreenDriveType:";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label19.ForeColor = Color.OldLace;
            label19.Location = new Point(7, 19);
            label19.Name = "label19";
            label19.Size = new Size(88, 32);
            label19.TabIndex = 37;
            label19.Text = "Name:";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label20.ForeColor = Color.OldLace;
            label20.Location = new Point(531, 539);
            label20.Name = "label20";
            label20.Size = new Size(255, 32);
            label20.TabIndex = 65;
            label20.Text = "DataGroupSequence:";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label21.ForeColor = Color.OldLace;
            label21.Location = new Point(531, 247);
            label21.Name = "label21";
            label21.Size = new Size(275, 32);
            label21.TabIndex = 64;
            label21.Text = "RowsCtrlByDataGroup:";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label22.ForeColor = Color.OldLace;
            label22.Location = new Point(531, 19);
            label22.Name = "label22";
            label22.Size = new Size(202, 32);
            label22.TabIndex = 63;
            label22.Text = "SerialColorNum:";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label23.ForeColor = Color.OldLace;
            label23.Location = new Point(531, 61);
            label23.Name = "label23";
            label23.Size = new Size(293, 32);
            label23.TabIndex = 62;
            label23.Text = "SerialDotsNumPerColor:";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label24.ForeColor = Color.OldLace;
            label24.Location = new Point(531, 100);
            label24.Name = "label24";
            label24.Size = new Size(190, 32);
            label24.TabIndex = 61;
            label24.Text = "SerialRGBCode:";
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label25.ForeColor = Color.OldLace;
            label25.Location = new Point(531, 148);
            label25.Name = "label25";
            label25.Size = new Size(341, 32);
            label25.TabIndex = 60;
            label25.Text = "ChipMinLawRepeatNumber: ";
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label26.ForeColor = Color.OldLace;
            label26.Location = new Point(531, 202);
            label26.Name = "label26";
            label26.Size = new Size(242, 32);
            label26.TabIndex = 59;
            label26.Text = "ChannelEnableData:";
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label27.ForeColor = Color.OldLace;
            label27.Location = new Point(531, 296);
            label27.Name = "label27";
            label27.Size = new Size(166, 32);
            label27.TabIndex = 58;
            label27.Text = "ChannelData:";
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label28.ForeColor = Color.OldLace;
            label28.Location = new Point(532, 349);
            label28.Name = "label28";
            label28.Size = new Size(167, 32);
            label28.TabIndex = 57;
            label28.Text = "ChipNumber:";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label29.ForeColor = Color.OldLace;
            label29.Location = new Point(532, 392);
            label29.Name = "label29";
            label29.Size = new Size(320, 32);
            label29.TabIndex = 56;
            label29.Text = "StartPositionOfDataGroup:";
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label30.ForeColor = Color.OldLace;
            label30.Location = new Point(532, 438);
            label30.Name = "label30";
            label30.Size = new Size(194, 32);
            label30.TabIndex = 55;
            label30.Text = "ModuleVersion:";
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label31.ForeColor = Color.OldLace;
            label31.Location = new Point(532, 489);
            label31.Name = "label31";
            label31.Size = new Size(142, 32);
            label31.TabIndex = 54;
            label31.Text = "IsIrregular:";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label32.ForeColor = Color.OldLace;
            label32.Location = new Point(5, 661);
            label32.Name = "label32";
            label32.Size = new Size(299, 32);
            label32.TabIndex = 66;
            label32.Text = "Пропущенные пиксели:";
            // 
            // buttonAbout
            // 
            buttonAbout.BackColor = Color.Black;
            buttonAbout.Font = new Font("Segoe UI", 25.8000011F, FontStyle.Bold, GraphicsUnit.Point, 204);
            buttonAbout.ForeColor = Color.Fuchsia;
            buttonAbout.Location = new Point(1669, 481);
            buttonAbout.Name = "buttonAbout";
            buttonAbout.Size = new Size(88, 86);
            buttonAbout.TabIndex = 67;
            buttonAbout.Text = "?";
            buttonAbout.UseVisualStyleBackColor = false;
            buttonAbout.Click += buttonAbout_Click;
            // 
            // btnSaveFile
            // 
            btnSaveFile.BackColor = Color.Black;
            btnSaveFile.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            btnSaveFile.ForeColor = Color.DarkOrange;
            btnSaveFile.Location = new Point(1435, 481);
            btnSaveFile.Name = "btnSaveFile";
            btnSaveFile.Size = new Size(213, 86);
            btnSaveFile.TabIndex = 68;
            btnSaveFile.Text = "Сохранить файл .module";
            btnSaveFile.UseVisualStyleBackColor = false;
            btnSaveFile.Click += btnSaveFile_Click;
            // 
            // buttonRotateToLeft
            // 
            buttonRotateToLeft.BackColor = Color.Black;
            buttonRotateToLeft.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            buttonRotateToLeft.ForeColor = Color.OrangeRed;
            buttonRotateToLeft.Location = new Point(1182, 583);
            buttonRotateToLeft.Name = "buttonRotateToLeft";
            buttonRotateToLeft.Size = new Size(167, 86);
            buttonRotateToLeft.TabIndex = 69;
            buttonRotateToLeft.Text = "Rotate 90 Left";
            buttonRotateToLeft.UseVisualStyleBackColor = false;
            buttonRotateToLeft.Click += buttonRotateToLeft_Click;
            // 
            // buttonRotateToRight
            // 
            buttonRotateToRight.BackColor = Color.Black;
            buttonRotateToRight.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            buttonRotateToRight.ForeColor = Color.OrangeRed;
            buttonRotateToRight.Location = new Point(1371, 583);
            buttonRotateToRight.Name = "buttonRotateToRight";
            buttonRotateToRight.Size = new Size(186, 86);
            buttonRotateToRight.TabIndex = 70;
            buttonRotateToRight.Text = "Rotate 90 Right";
            buttonRotateToRight.UseVisualStyleBackColor = false;
            buttonRotateToRight.Click += buttonRotateToRight_Click_1;
            // 
            // buttonRotate180
            // 
            buttonRotate180.BackColor = Color.Black;
            buttonRotate180.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            buttonRotate180.ForeColor = Color.OrangeRed;
            buttonRotate180.Location = new Point(1595, 583);
            buttonRotate180.Name = "buttonRotate180";
            buttonRotate180.Size = new Size(162, 86);
            buttonRotate180.TabIndex = 71;
            buttonRotate180.Text = "Rotate 180";
            buttonRotate180.UseVisualStyleBackColor = false;
            // 
            // textBoxScanABCDCode
            // 
            textBoxScanABCDCode.Location = new Point(276, 623);
            textBoxScanABCDCode.Multiline = true;
            textBoxScanABCDCode.Name = "textBoxScanABCDCode";
            textBoxScanABCDCode.ScrollBars = ScrollBars.Vertical;
            textBoxScanABCDCode.Size = new Size(871, 28);
            textBoxScanABCDCode.TabIndex = 72;
            // 
            // pictureBoxModule
            // 
            pictureBoxModule.Location = new Point(1358, 239);
            pictureBoxModule.Name = "pictureBoxModule";
            pictureBoxModule.Size = new Size(226, 217);
            pictureBoxModule.TabIndex = 73;
            pictureBoxModule.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 26);
            ClientSize = new Size(1779, 758);
            Controls.Add(pictureBoxModule);
            Controls.Add(textBoxScanABCDCode);
            Controls.Add(buttonRotate180);
            Controls.Add(buttonRotateToRight);
            Controls.Add(buttonRotateToLeft);
            Controls.Add(btnSaveFile);
            Controls.Add(buttonAbout);
            Controls.Add(label32);
            Controls.Add(label20);
            Controls.Add(label21);
            Controls.Add(label22);
            Controls.Add(label23);
            Controls.Add(label24);
            Controls.Add(label25);
            Controls.Add(label26);
            Controls.Add(label27);
            Controls.Add(label28);
            Controls.Add(label29);
            Controls.Add(label30);
            Controls.Add(label31);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(label5);
            Controls.Add(label6);
            Controls.Add(label7);
            Controls.Add(label8);
            Controls.Add(label9);
            Controls.Add(label10);
            Controls.Add(label11);
            Controls.Add(label12);
            Controls.Add(label13);
            Controls.Add(label14);
            Controls.Add(label15);
            Controls.Add(label16);
            Controls.Add(label17);
            Controls.Add(label18);
            Controls.Add(label19);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBoxNewScanABCDCode);
            Controls.Add(textBoxPointTableData);
            Controls.Add(lblMissingPixels);
            Controls.Add(lblPixelCols);
            Controls.Add(lblPixelRows);
            Controls.Add(lblDataDirectType);
            Controls.Add(lblDriverChipType);
            Controls.Add(lblDriverChipTypeExtend);
            Controls.Add(lblDriverTypePro);
            Controls.Add(lblDecType);
            Controls.Add(lblOEPolarity);
            Controls.Add(lblScanType);
            Controls.Add(lblTotalPointInTable);
            Controls.Add(lblDataGroup);
            Controls.Add(lblPointTableData);
            Controls.Add(lblDataGroupSequence);
            Controls.Add(lblScanABCDCode);
            Controls.Add(lblRGBCode);
            Controls.Add(lblRowsCtrlByDataGroup);
            Controls.Add(lblLineBias);
            Controls.Add(lblScreenDriveType);
            Controls.Add(lblSerialColorNum);
            Controls.Add(lblSerialDotsNumPerColor);
            Controls.Add(lblSerialRGBCode);
            Controls.Add(lblChipMinLawRepeatNumber);
            Controls.Add(lblChannelEnableData);
            Controls.Add(lblChannelData);
            Controls.Add(lblChipNumber);
            Controls.Add(lblStartPositionOfDataGroup);
            Controls.Add(lblModuleVersion);
            Controls.Add(lblIsIrregular);
            Controls.Add(lblName);
            Controls.Add(btnOpenFile);
            Font = new Font("Segoe UI Light", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            ForeColor = Color.OldLace;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Nova XML Reader Калькулятор пропущенных пикселей для файлов .module от NovaLCT";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxModule).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnOpenFile;
        private Label lblName;
        private Label lblIsIrregular;
        private Label lblModuleVersion;
        private Label lblStartPositionOfDataGroup;
        private Label lblChipNumber;
        private Label lblChannelData;
        private Label lblChannelEnableData;
        private Label lblChipMinLawRepeatNumber;
        private Label lblSerialRGBCode;
        private Label lblSerialDotsNumPerColor;
        private Label lblSerialColorNum;
        private Label lblScreenDriveType;
        private Label lblLineBias;
        private Label lblRowsCtrlByDataGroup;
        private Label lblRGBCode;
        private Label lblScanABCDCode;
        private Label lblDataGroupSequence;
        private Label lblPointTableData;
        private Label lblDataGroup;
        private Label lblTotalPointInTable;
        private Label lblScanType;
        private Label lblOEPolarity;
        private Label lblDecType;
        private Label lblDriverTypePro;
        private Label lblDriverChipTypeExtend;
        private Label lblDriverChipType;
        private Label lblDataDirectType;
        private Label lblPixelRows;
        private Label lblPixelCols;
        private Label lblMissingPixels;
        private TextBox textBoxPointTableData;
        private TextBox textBoxNewScanABCDCode;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label17;
        private Label label18;
        private Label label19;
        private Label label20;
        private Label label21;
        private Label label22;
        private Label label23;
        private Label label24;
        private Label label25;
        private Label label26;
        private Label label27;
        private Label label28;
        private Label label29;
        private Label label30;
        private Label label31;
        private Label label32;
        private Button buttonAbout;
        private Button btnSaveFile;
        private Button buttonRotateToLeft;
        private Button buttonRotateToRight;
        private Button buttonRotate180;
        private TextBox textBoxScanABCDCode;
        private PictureBox pictureBoxModule;
    }
}
