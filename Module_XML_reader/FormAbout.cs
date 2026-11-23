using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace Module_XML_reader
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();

            // Настройка текста в Label
            label1.Text = " Nova XML Reader\n Версия 1.0 Beta. 2024г.\n Разработка приложения: @digitalbuddha\n Почта: knv_nano@bk.ru";

        }

        private void FormAbout_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Открытие ссылки на Telegram в браузере
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://t.me/digitalbudda",
                    UseShellExecute = true  // Это позволяет системе выбрать, чем открыть URL
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось открыть ссылку: " + ex.Message);
            }
        }
    }
}