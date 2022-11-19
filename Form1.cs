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
        int type_road = 1; // 0 - прямые перееды, 1 - с пересадками
        int type_calc = 0; // 0 - занимающие меньшее кол-во пересадок, 1 - самы бюджетный вариант 

        string[,] t = { 
            { "", "", "", "", "", "", "", "", "", "", "", "12000", "", "10000", "", "", "", "", "" }, 
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "9000", "", "", "", "", "" }, 
            { "", "", "", "11500", "", "", "", "", "", "", "", "", "", "", "9750", "", "", "", "" }, 
            { "", "", "", "", "10200", "", "", "", "", "", "", "", "", "", "", "12400", "", "", "" }, 
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "9750", "11300", "" }, 
            { "", "", "", "", "8600", "", "", "", "", "", "", "", "", "", "", "", "", "", "" }, 
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "13000" }, 
            { "", "", "", "", "", "", "10800", "", "12440", "", "", "", "", "", "", "", "", "", "" }, 
            { "", "", "", "", "", "", "", "", "", "", "", "25400", "", "", "", "", "", "", "" }, 
            { "", "", "", "", "", "", "", "", "13500", "", "11300", "", "", "", "", "", "", "", "" }, 
            { "", "", "", "", "", "", "", "", "", "11000", "", "", "", "", "", "", "", "", "" }, 
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "14100", "" }, 
            { "9500", "", "", "", "", "", "", "", "", "", "", "", "", "10400", "", "", "", "", "" }, 
            { "", "", "10600", "", "", "", "", "", "", "", "", "", "10150", "", "", "", "", "", "" }, 
            { "", "", "8700", "11500", "", "", "", "", "", "", "", "10700", "11550", "", "", "", "", "", "" }, 
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "8400", "", "11200", "10700", "" }, 
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" }, 
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "13400" }, 
            { "", "", "", "", "", "14300", "", "15900", "", "", "", "", "", "", "", "", "", "", "" }, 
        };


        public Form1()
        {
            InitializeComponent();
            load_data();

            load_start();
            load_end();
            radioButton2.Select();
        }

        /// <summary>
        /// Загрузка списка городов в первый селектор(Пункт отправления)
        /// </summary>
        private void load_start()
        {
            comboBox1.Items.AddRange(cities);
        }

        /// <summary>
        /// Загрузка списка городов во второй селектор(Пункт прибытия)
        /// </summary>
        private void load_end()
        {
            comboBox2.ResetText();
            if(type_road == 0)
                comboBox2.Items.AddRange(get_cities());
            else
                comboBox2.Items.AddRange(cities);
        }

        /// <summary>
        /// Сбор данных и добавление в таблицу (Доступные билеты)
        /// </summary>
        /// <param name="sender">Стандартный параметр, создается по умолчанию</param>
        /// <param name="e">Стандартный параметр, создается по умолчанию</param>
        private void calculate(object sender, EventArgs e)
        {

            dataGridView1.Rows.Clear();
            string start = (string)comboBox1.SelectedItem;
            string end = (string)comboBox2.SelectedItem;
            
            var index_r = comboBox1.SelectedIndex;
            var index_c = cities.ToList().IndexOf(end) + 1;

            var i = _data.Rows[index_r][index_c];
            if(i != DBNull.Value)
            {
                dataGridView1.Rows.Add(start, end, i);
                result(i);
            }
            calc(index_r, index_c);
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
        /// ЗАгрузка таблицы Excel, для этого создается подключение к excel, котрое выступает в качестве БД
        /// </summary>
        private void load_data()
        {
            /*String name = "Items";
            String constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            "D:\\Users\\84991\\Documents\\Sample.xlsx" + // Здесь указан путь до excel, нужно указать путь к файлу, где он у тебя храниться на ПК, по файлу ПКМ -> Свйоства и там есть пункт "Расположение", только слэши нужно указать как здесь
                            ";Extended Properties='Excel 12.0 XML;HDR=YES;';";

            OleDbConnection con = new OleDbConnection(constr);
            OleDbCommand oconn = new OleDbCommand("Select * From [" + name + "$]", con);
            con.Open();

            OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
            sda.Fill(_data);*/
        }

        /// <summary>
        /// Получение списка городов во второй селектор(Пункт прибытия) в случае, если выбрантип поездки "Прямая"
        /// </summary>
        /// <returns></returns>
        private string[] get_cities()
        {
            comboBox2.Items.Clear();
            var _cities = new List<string>();

            if (comboBox1.SelectedItem != null)
            {
                var ind = comboBox1.SelectedIndex;
               
                for(int i = 1; i < _data.Columns.Count; i++)
                {
                    var n = _data.Rows[ind][i];
                    if(n != DBNull.Value)
                    {
                        _cities.Add(_data.Columns[i].ToString());
                    }
                }
            }

            return _cities.ToArray();
        }

        /// <summary>
        /// Это в работе
        /// </summary>
        /// <param name="start">Откуда начинаются все поездки</param>
        /// <param name="end"> куда по итогу нужно приехать</param>
        private void calc(int start, int end)
        {
            get_path(start, end, new List<int>());
        }

        /// <summary>
        /// Маршруты, по идее суда должны записываться маршруты следования, из них нужно будет выбрать самы короткий и который позваоляет приехать в нужный для нас город
        /// </summary>
        private List<List<int>> path = new List<List<int>>();
        private List<int> _path = new List<int>();
        /// <summary>
        /// Вот именно сейчас у меня с этим проблема, нужно подумать, как определять поездки и добавлять в маршрут
        /// [ [3, 4, 5], [3, 15, 4, 5], [3, 15, 4] ]
        /// 
        /// </summary>
        /// <param name="ind">Начало отправления, если мы куда то приезжаем, суда передается новое место</param>
        /// <param name="end">Самый конечный пункт назначения</param>
        /// <param name="_path">Путь маршрута, который передается из раза в раз, но из-за особенностей C# надо придумывать обходное решение этой проблемы</param>
        private void get_path(int ind, int end, List<int> p)
        {
            p.Add(ind);

            if (ind == end)
            {
                return;
            } else
            {
                path.Add(p);
            }

            var d = _data.Rows[ind];

            for (int i = 1; i < _data.Columns.Count; i++)
            {
                List<int> l = new List<int>();
                l.AddRange(p);

                if (d[i] != DBNull.Value)
                {
                    get_path(i - 1, end, l);
                }
            }
        }

        /// <summary>
        /// Этот метод нужен для загрузки доступных Пунктов прибытия, куда можно приехать, если выбран Прямой тип поездки( чтобы сменить тип поездки нужно type_road = 0 )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void load_end(object sender, EventArgs e)
        {
            load_end();
        }
    }
}