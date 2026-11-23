using System.Xml.Linq;

namespace Module_XML_reader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        // Добавляем переменную для отслеживания состояния поворота
        public bool isRotatedLeft = false;


        //Открываем файл с модулей
        // Добавляем поле для хранения пути к открытому файлу
        private string openedFilePath = string.Empty;

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            // Открытие диалогового окна для выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Module files (*.module)|*.module",
                Title = "Open Module File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Сбрасываем флаг поворота при загрузке нового файла
                    isRotatedLeft = false;

                    // Сохраняем путь к открытому файлу
                    openedFilePath = openFileDialog.FileName;

                    // Загрузка XML файла
                    XDocument xmlDoc = XDocument.Load(openedFilePath);
                    var module = xmlDoc.Element("Config")?.Element("Module");

                    if (module != null)
                    {
                        // Загрузка данных в TextBox
                        textBoxPointTableData.Text = module.Element("PointTableData")?.Value ?? "Не указано";
                        textBoxNewScanABCDCode.Text = module.Element("NewScanABCDCode")?.Value ?? "Не указано";

                        // Получение и отображение данных из XML
                        lblName.Text = module.Element("Name")?.Value;
                        lblPixelCols.Text = module.Element("PixelCols")?.Value;
                        lblPixelRows.Text = module.Element("PixelRows")?.Value;
                        lblDataDirectType.Text = module.Element("DataDirectType")?.Value;
                        lblDriverChipType.Text = module.Element("DriverChipType")?.Value;
                        lblDriverChipTypeExtend.Text = module.Element("DriverChipTypeExtend")?.Value;
                        lblDriverTypePro.Text = module.Element("DriverTypePro")?.Value;
                        lblDecType.Text = module.Element("DecType")?.Value;
                        lblOEPolarity.Text = module.Element("OEPolarity")?.Value;
                        lblScanType.Text = module.Element("ScanType")?.Value;
                        lblTotalPointInTable.Text = module.Element("TotalPointInTable")?.Value;
                        lblDataGroup.Text = module.Element("DataGroup")?.Value;

                        lblDataGroupSequence.Text = module.Element("DataGroupSequence")?.Value;

                        //это старый код выводит в лейбл
                        lblScanABCDCode.Text = module.Element("ScanABCDCode")?.Value;
                        //это новый в текстовое поле
                        textBoxScanABCDCode.Text = module.Element("ScanABCDCode")?.Value ?? "Не указано";

                        lblRGBCode.Text = module.Element("RGBCode")?.Value;
                        lblRowsCtrlByDataGroup.Text = module.Element("RowsCtrlByDataGroup")?.Value;
                        lblLineBias.Text = module.Element("LineBias")?.Value;
                        lblScreenDriveType.Text = module.Element("ScreenDriveType")?.Value;
                        lblSerialColorNum.Text = module.Element("SerialColorNum")?.Value;
                        lblSerialDotsNumPerColor.Text = module.Element("SerialDotsNumPerColor")?.Value;
                        lblSerialRGBCode.Text = module.Element("SerialRGBCode")?.Value;
                        lblChipMinLawRepeatNumber.Text = module.Element("ChipMinLawRepeatNumber")?.Value;
                        lblChannelEnableData.Text = module.Element("ChannelEnableData")?.Value;
                        lblChannelData.Text = module.Element("ChannelData")?.Value;
                        lblChipNumber.Text = module.Element("ChipNumber")?.Value;
                        lblStartPositionOfDataGroup.Text = module.Element("StartPositionOfDataGroup")?.Value;
                        lblModuleVersion.Text = module.Element("ModuleVersion")?.Value;
                        lblIsIrregular.Text = module.Element("IsIrregular")?.Value;

                        // Вычисление пропущенных пикселей
                        string pointTableData = module.Element("PointTableData")?.Value;
                        if (!string.IsNullOrEmpty(pointTableData))
                        {
                            CalculateMissingPixels(pointTableData);

                            // Рисуем модуль в PictureBox
                            int pixelCols = int.Parse(module.Element("PixelCols")?.Value ?? "0");
                            int pixelRows = int.Parse(module.Element("PixelRows")?.Value ?? "0");

                            if (pixelCols > 0 && pixelRows > 0)
                            {
                                DrawModule(pictureBoxModule, pixelCols, pixelRows, pointTableData);
                            }
                            else
                            {
                                MessageBox.Show("Некорректные значения PixelCols или PixelRows.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            lblMissingPixels.Text = "Ошибка: PointTableData пуст.";
                        }
                    }
                    else
                    {
                        lblMissingPixels.Text = "Ошибка: Не удалось найти элемент Module.";
                    }
                }
                catch (Exception ex)
                {
                    lblMissingPixels.Text = "Ошибка при загрузке файла: " + ex.Message;
                }
            }
        }




        // Метод для вычисления пропущенных пикселей
        private void CalculateMissingPixels(string pointTableData)
        {
            // Разбиваем строку на массив значений
            var dataValues = pointTableData.Split('-').Select(int.Parse).ToArray();

            List<int> missingPixels = new List<int>();
            int expectedPixel = 1;  // Начинаем счёт пикселей с 1

            for (int i = 0; i < dataValues.Length - 1; i += 2)
            {
                // Проверяем, что пара значений равна 255-255
                if (i + 1 < dataValues.Length && dataValues[i] == 255 && dataValues[i + 1] == 255)
                {
                    // Если встречаем 255-255, то текущий ожидаемый пиксель пропущен
                    missingPixels.Add(expectedPixel);

                    // Продвигаемся к следующему ожидаемому пикселю, не учитывая сдвиг
                    expectedPixel++;
                }
                else
                {
                    // Иначе продвигаемся к следующему ожидаемому пикселю
                    expectedPixel++;
                }
            }

            // Выводим пропущенные пиксели в Label или сообщение, если пропущенные пиксели не найдены
            if (missingPixels.Count > 0)
            {
                lblMissingPixels.Text = string.Join(", ", missingPixels);
            }
            else
            {
                lblMissingPixels.Text = "Не найдено пропущенных пикселей.";
            }
        }

        private void lblPointTableData_Click(object sender, EventArgs e)
        {

        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.ShowDialog();  // Открываем окно с информацией "О разработчике" как модальное
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            SaveModuleData();
        }

        // Метод для сохранения изменённых данных с диалогом сохранения
        private void SaveModuleData()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Module files (*.module)|*.module",
                Title = "Сохранить файл как",
                DefaultExt = "module",
                AddExtension = true,
                FileName = "module_file" // Имя файла по умолчанию
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    // Загружаем XML-документ с использованием текущего пути к файлу
                    XDocument xmlDoc = XDocument.Load(openedFilePath); // Загружаем текущий файл

                    var module = xmlDoc.Element("Config")?.Element("Module");

                    if (module != null)
                    {
                        // Обновляем данные в XML из текстовых полей
                        module.Element("PointTableData")?.SetValue(textBoxPointTableData.Text);
                        module.Element("NewScanABCDCode")?.SetValue(textBoxNewScanABCDCode.Text);
                        module.Element("ScanABCDCode")?.SetValue(textBoxScanABCDCode.Text);  // Обновляем ScanABCDCode
                        module.Element("PixelCols")?.SetValue(lblPixelCols.Text);
                        module.Element("PixelRows")?.SetValue(lblPixelRows.Text);
                        module.Element("DataDirectType")?.SetValue(lblDataDirectType.Text);

                        // Сохраняем LineBias из поля lblLineBias
                        module.Element("LineBias")?.SetValue(lblLineBias.Text);

                        // Сохраняем файл с новым именем и путем
                        xmlDoc.Save(filePath);

                        // Сообщение об успешном сохранении
                        MessageBox.Show("Файл успешно сохранён как " + filePath, "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка: Не удалось найти элемент Module.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }










        private void buttonRotateToLeft_Click(object sender, EventArgs e)
        {
            RotateModuleToLeft();
        }

        // Метод для поворота модуля на 90 градусов влево с проверкой состояния
        private void RotateModuleToLeft()
        {
            if (string.IsNullOrEmpty(openedFilePath))
            {
                MessageBox.Show("Файл не был загружен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                XDocument xmlDoc = XDocument.Load(openedFilePath);
                var module = xmlDoc.Element("Config")?.Element("Module");

                if (module != null)
                {
                    // Проверка на состояние поворота и DataDirectType
                    string dataDirectType = module.Element("DataDirectType")?.Value;
                    if (isRotatedLeft || dataDirectType == "Vertical")
                    {
                        MessageBox.Show("Модуль уже повёрнут на 90 градусов влево.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Меняем значения PixelCols и PixelRows местами
                    int pixelCols = int.Parse(module.Element("PixelCols")?.Value ?? "0");
                    int pixelRows = int.Parse(module.Element("PixelRows")?.Value ?? "0");
                    module.Element("PixelCols")?.SetValue(pixelRows);
                    module.Element("PixelRows")?.SetValue(pixelCols);

                    // Устанавливаем DataDirectType в "Vertical"
                    module.Element("DataDirectType")?.SetValue("Vertical");

                    // Изменение таблицы пикселей (PointTableData) для поворота на 90 градусов влево
                    string pointTableData = module.Element("PointTableData")?.Value ?? string.Empty;
                    string newPointTableData = RotatePointTableDataToLeft(pointTableData);
                    module.Element("PointTableData")?.SetValue(newPointTableData);

                    // Обновление данных в интерфейсе
                    textBoxPointTableData.Text = newPointTableData;
                    lblPixelCols.Text = pixelRows.ToString();
                    lblPixelRows.Text = pixelCols.ToString();
                    lblDataDirectType.Text = "Vertical"; // Обновляем DataDirectType в интерфейсе

                    // Устанавливаем флаг поворота
                    isRotatedLeft = true;

                    // Сообщение об успешном повороте
                    MessageBox.Show("Модуль успешно повернут на 90 градусов влево.", "Поворот", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка: Не удалось найти элемент Module.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при повороте файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Метод для поворота таблицы пикселей на 90 градусов влево
        private string RotatePointTableDataToLeft(string pointTableData)
        {
            // Создаём список для хранения инвертированных данных
            var invertedData = new List<int>();

            // Начинаем с 0 и убываем до 63, заполняя "0-X"
            for (int i = 63; i >= 0; i--)
            {
                invertedData.Add(0);  // Первый элемент всегда 0
                invertedData.Add(i);  // Второй элемент убывает от 63 до 0
            }

            return string.Join("-", invertedData);
        }








        // Метод для поворота модуля на 90 градусов вправо
        private void RotateModuleToRight()
        {
            if (string.IsNullOrEmpty(openedFilePath))
            {
                MessageBox.Show("Файл не был загружен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                XDocument xmlDoc = XDocument.Load(openedFilePath);
                var module = xmlDoc.Element("Config")?.Element("Module");

                if (module != null)
                {
                    // Проверка на состояние поворота и DataDirectType
                    string dataDirectType = module.Element("DataDirectType")?.Value;
                    if (dataDirectType == "Vertical")
                    {
                        MessageBox.Show("Модуль уже повёрнут на 90 градусов вправо.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Меняем значения PixelCols и PixelRows местами
                    int pixelCols = int.Parse(module.Element("PixelCols")?.Value ?? "0");
                    int pixelRows = int.Parse(module.Element("PixelRows")?.Value ?? "0");
                    module.Element("PixelCols")?.SetValue(pixelRows);
                    module.Element("PixelRows")?.SetValue(pixelCols);

                    // Устанавливаем DataDirectType в "Vertical"
                    module.Element("DataDirectType")?.SetValue("Vertical");

                    // Изменение таблицы пикселей (PointTableData) для поворота на 90 градусов вправо
                    string pointTableData = module.Element("PointTableData")?.Value ?? string.Empty;
                    string newPointTableData = RotatePointTableDataToRight(pointTableData);
                    module.Element("PointTableData")?.SetValue(newPointTableData);

                    // Инвертируем ScanABCDCode при повороте вправо
                    string scanABCDCode = module.Element("ScanABCDCode")?.Value ?? string.Empty;
                    int scanLength = scanABCDCode.Split('-').Length;  // Динамически определяем длину
                    string newScanABCDCode = ReverseScanCode(scanABCDCode, scanLength);  // Учитываем реальную длину
                    module.Element("ScanABCDCode")?.SetValue(newScanABCDCode);

                    // Длина строки NewScanABCDCode должна быть равна PixelRows * 2
                    string newNewScanABCDCode = ReverseScanCode(module.Element("NewScanABCDCode")?.Value ?? string.Empty, pixelRows * 2);
                    module.Element("NewScanABCDCode")?.SetValue(newNewScanABCDCode);

                    // Вычисляем LineBias как максимальное значение из NewScanABCDCode
                    int newLineBias = CalculateLineBias(newNewScanABCDCode);
                    module.Element("LineBias")?.SetValue(newLineBias.ToString());

                    // Обновление данных в интерфейсе
                    textBoxPointTableData.Text = newPointTableData;
                    lblPixelCols.Text = pixelRows.ToString();
                    lblPixelRows.Text = pixelCols.ToString();
                    lblDataDirectType.Text = "Vertical"; // Обновляем DataDirectType в интерфейсе

                    // Обновляем textBox для ScanABCDCode и NewScanABCDCode
                    textBoxScanABCDCode.Text = newScanABCDCode;
                    textBoxNewScanABCDCode.Text = newNewScanABCDCode;

                    // Обновляем LineBias в интерфейсе
                    lblLineBias.Text = newLineBias.ToString();  // Добавляем обновление LineBias

                    // Сообщение об успешном повороте
                    MessageBox.Show("Модуль успешно повернут на 90 градусов вправо.", "Поворот", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка: Не удалось найти элемент Module.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при повороте файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // Метод для инверсии значимых значений строки и добавления нулей до нужной длины
        private string ReverseScanCode(string scanCode, int totalLength)
        {
            var scanValues = scanCode.Split('-').ToList();

            // Находим количество значимых элементов (все, которые не равны "0")
            var relevantValues = scanValues.Where(v => v != "0").Reverse().ToList();

            // Добавляем нули в конец до нужной длины
            int missingZeros = totalLength - relevantValues.Count;
            relevantValues.AddRange(Enumerable.Repeat("0", missingZeros));

            // Возвращаем строку в правильном формате
            return string.Join("-", relevantValues);
        }








        // Метод для поворота PointTableData на 90 градусов вправо
        private string RotatePointTableDataToRight(string pointTableData)
        {
            var dataValues = pointTableData.Split('-').ToList();

            // Логика поворота вправо
            var rotatedData = new List<string>();

            // Логика переворота: меняем местами элементы и инвертируем их порядок
            for (int i = 0; i < dataValues.Count; i += 2)
            {
                rotatedData.Add("15"); // Здесь используем "15" как поворот на 90 градусов вправо
                rotatedData.Add(dataValues[i]); // Переносим данные
            }

            return string.Join("-", rotatedData);
        }

        // Метод для вычисления LineBias из максимального значения NewScanABCDCode
        // Метод для вычисления LineBias из максимального значения NewScanABCDCode
        // Метод для вычисления LineBias из максимального значения NewScanABCDCode
        private int CalculateLineBias(string newScanABCDCode)
        {
            var scanValues = newScanABCDCode.Split('-')
                .Where(value => int.TryParse(value, out _)) // Фильтруем только корректные числовые значения
                .Select(int.Parse)
                .ToList();
            return scanValues.Max();  // Возвращаем максимальное значение
        }


        // Метод для рисования модуля в PictureBox
        private void DrawModule(PictureBox pictureBox, int pixelCols, int pixelRows, string pointTableData)
        {
            // Настраиваем размеры PictureBox
            pictureBox.Width = pixelCols; // Размер одного пикселя — 10x10
            pictureBox.Height = pixelRows;

            // Создаём Bitmap для рисования
            Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black); // Фон чёрный

                // Разбиваем строку PointTableData на массив чисел
                var pixelData = pointTableData.Split('-').Select(int.Parse).ToArray();

                int index = 0; // Индекс в массиве данных
                for (int y = 0; y < pixelRows; y++)
                {
                    for (int x = 0; x < pixelCols; x++)
                    {
                        // Если данных не хватает, выходим из цикла
                        if (index >= pixelData.Length) break;

                        // Цвет пикселя (255 означает пустой пиксель)
                        Color pixelColor = pixelData[index] == 255 ? Color.Gray : Color.Green;

                        // Рисуем квадрат, представляющий пиксель
                        g.FillRectangle(new SolidBrush(pixelColor), x * 10, y * 10, 10, 10);

                        index++;
                    }
                }
            }

            // Устанавливаем изображение в PictureBox
            pictureBox.Image = bmp;
        }




        private void buttonRotateToRight_Click_1(object sender, EventArgs e)
        {
            RotateModuleToRight();
        }
    }
}
