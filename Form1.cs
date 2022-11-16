using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace app_road
{
    public partial class Form1 : Form
    {
        string[] cities = { "Город1", "Город2", "Город3", "Город4", "Город5", "Город6", "Город7", "Город8", "Город9", "Город10", "Город11", "Город12", "Город13", "Город14", "Город15", "Город16", "Город17", "Город18", "Город19" };
        DataTable _data = new DataTable(); // данные из таблицы excel

        public Form1()
        {
            InitializeComponent();
            load_data();
            load_start();
            load_end();
        }

        /// <summary>
        /// Загрузка списка городов в первый селектор(Пункт отправления)
        /// </summary>
        private void load_start()
        {
            comboBox1.Items.AddRange(cities); // Добавление строк(городов) в раскрывающийся список
        }

        /// <summary>
        /// Загрузка списка городов во второй селектор(Пункт прибытия)
        /// </summary>
        private void load_end()
        {
            comboBox2.ResetText(); // Сброс текста пункта прибытия, происходит когда меняется пункт отправления
            comboBox2.Items.AddRange(get_cities()); // Добавление строк(городов) в раскрывающийся список
        }

        /// <summary>
        /// Сбор данных и добавление в таблицу (Доступные билеты)
        /// </summary>
        /// <param name="sender">Стандартный параметр, создается по умолчанию</param>
        /// <param name="e">Стандартный параметр, создается по умолчанию</param>
        private void calculate(object sender, EventArgs e)
        {

            dataGridView1.Rows.Clear();   // Очищение таблицы "Доступные билеты"
            string start = (string)comboBox1.SelectedItem;   // Получение города из поля "Пуенкт отправления"
            string end = (string)comboBox2.SelectedItem;   // Получение города из поля "Пуенкт прибытия"

            var index_r = comboBox1.SelectedIndex; // Получаем индекс выбраного города отправления
            var index_c = cities.ToList().IndexOf(end) + 1; // Находим индекс города прибытия 

            var i = _data.Rows[index_r][index_c]; // получение суммы билета
            if(i != DBNull.Value)
            {
                dataGridView1.Rows.Add(start, end, i); // Добавление записи в таблицу "Доступные билеты"
                result(i); // Запись данных в Итого
            }
        }

        /// <summary>
        /// Запись данных в Итого
        /// </summary>
        /// <param name="result"></param>
        private void result(object result)
        {
            textBox2.Text = result.ToString();
        }

        /// <summary>
        /// Загрузка таблицы Excel, для этого создается подключение к excel, котрое выступает в качестве БД
        /// </summary>
        private void load_data()
        {
            string name = "Items"; // Название "Листа" в таблице с городами
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            "D:\\Users\\84991\\Documents\\Sample.xlsx" + // Здесь указан путь до excel, нужно указать путь к файлу, где он у тебя храниться на ПК, по файлу ПКМ -> Свйоства и там есть пункт "Расположение", только слэши нужно указать как здесь
                            ";Extended Properties='Excel 12.0 XML;HDR=YES;';"; // connectionString строка подключения к БД

            OleDbConnection connect = new OleDbConnection(connectionString); // подключение к БД
            OleDbCommand cmd = new OleDbCommand("Select * From [" + name + "$]", connect); // команда(запрос) для получения данных из БД(excel)
            connect.Open(); // открытие подключения

            OleDbDataAdapter d = new OleDbDataAdapter(cmd); // обработка выполнения команды
            d.Fill(_data); // добавление данных в DataTable, в нашу глобальную переменную
        }

        /// <summary>
        /// Получение списка городов во второй селектор(Пункт прибытия) в случае, если выбран тип поездки "Прямая"
        /// </summary>
        /// <returns></returns>
        private string[] get_cities()
        {
            comboBox2.Items.Clear(); // очищается поле "Пунк прибятия"
            var _cities = new List<string>(); // создается новый пустой строковый список

            if (comboBox1.SelectedItem != null) // проверяем, что в поле "Пункт отправления" выбран город 
            {
                var ind = comboBox1.SelectedIndex; // получение индекса города
               
                for(int i = 1; i < _data.Columns.Count; i++)  // цикл по колонкам, i начинается с 1 из-за того, что он первую колонку(под буквой A) считает тоже, и соответственно у нее индекс 0, в последствии мы проверяем на пустая ли колонка или нет, если не пустая то выводим этот город в возможный вариант прибытия
                {
                    var n = _data.Rows[ind][i]; // получаем значение сторки(города отправления), и i каждый столбец
                    if(n != DBNull.Value) // вместо null проверяем на DBNull.Value, поскольку используем DataTable(без него никак), а там значение null как такового нет, но по сути DBNull.Value обозначает пустую ячейку
                    {
                        _cities.Add(_data.Columns[i].ToString()); // Если ячейка не пустая, значит есть билет до этого города, здесь происходит добавление в возможные города прибытия
                    }
                }
            }

            return _cities.ToArray(); // возвращаем города, в которые можно отправиться
        }

        /// <summary>
        /// Этот метод нужен для загрузки доступных Пунктов прибытия, куда можно приехать, если выбран Прямой тип поездки( чтобы сменить тип поездки нужно type_road = 0 )
        /// Этот метод срабатывает при изменении "Пункта отправления"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void load_end(object sender, EventArgs e)
        {
            load_end(); 
        }
    }
}
