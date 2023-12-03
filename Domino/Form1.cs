using System;
using System.Collections;  // ArrayList
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection; // Assembly
using System.Resources; // ResourceManager
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Domino
{
    public partial class Form1 : Form
    {


        public ResourceManager res;                          // Ресурсы

        Domino[] domino = new Domino[28];                       // Массив костей

        Domino[] back_dmn = new Domino[20];                     // Массив перевернутых костей

        public ArrayList bazar_list = new ArrayList();          // Список костяшек, не участвующих на данный момент в процессе игры

        public ArrayList user_list = new ArrayList();           // Список, содержащий перечень всех костяшек принадлежащих первому игроку

        public ArrayList comp_list = new ArrayList();           // Список, содержащий перечень всех костяшек принадлежащих второму (компьютер) игроку

        Random rand = new Random();                             // рэндомайзер

        public int player;                                      // Номер текущего игрока

        public int step;                                        // Номер хода

        public Domino current;                                  //текущий объект домино

        public Domino highlight1;                               //выделенная область для размещения кости справа

        public Domino highlight2;                               //выделенная область для размещения кости слева

        public Point point1;                                    //координаты крайней правой точки

        public Point point2;                                    //координаты крайней левой точки

        public int left_value;                                  //крайнее значение костяшки по левой стороне поля

        public int right_value;                                 //крайнее значение костяшки по правой стороне поля

        public int turn_right;                                       //кол-во поворотов вправо

        public int turn_left;                                       //кол-во поворотов влево

        public bool puf;                                        //флаг

        public bool IsRightDouble;                              //флаг; true - крайняя кость справа - двойная

        public bool IsLeftDouble;                               //флаг; true - крайняя кость слева - двойная

        string message;                                         //сообщение, которое выводится на экран после оканчания игры

        int game_num;                                           //кол-во игр

        int losses_num;                                         //кол-во проигранных пользователем игр

        int wins_num;                                           //кол-во выигранных игр

        bool no_winners = false;                                //ничья

        string path = @"rate.txt";                              //путь к файлу рейтинговой таблицы

        


        public Form1()
        {
            InitializeComponent();

            // Подключение ресурса

            Assembly assembly = Assembly.GetExecutingAssembly();

            res = new ResourceManager("Domino.AppDomino.Pictures", assembly);

            GamePreparing(); //начать игру

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //если файл существует и не пуст
            if (File.Exists(path) && File.ReadAllText(path).Length != 0)
            {
                //считывание значений для кол-ва игр, кол-ва выигранных и проигранных игр из файла

                game_num = Convert.ToInt32(File.ReadAllLines(path).Last().Split('\t')[0]);
                losses_num = Convert.ToInt32(File.ReadAllLines(path).Last().Split('\t')[4]);
                wins_num = Convert.ToInt32(File.ReadAllLines(path).Last().Split('\t')[3]);
            }
            else
            {   //если текущая игра - первая, то устанавляваются значения 0 для всех трех переменных

                game_num = 0;
                losses_num = 0;
                wins_num = 0;
            }


        }

        // подготовка поля и костей к игре

        void GamePreparing()
        {

            // Заполнение массива объектов домино

            int n = 0; //счетчик

            //заполнение базара всеми возможными комбинациями костей
            for (int i = 0; i < 7; i++)
            {
                for (int j = i; j < 7; j++)
                {
                    domino[n] = new Domino(this, res, "d" + i + "_" + j); //создание объекта домино

                    bazar_list.Add(domino[n++]); //добавление кости в базар

                }
            }

            //заполнение массива перевернутых домино

            for (int i = 0; i < 20; i++)

                back_dmn[i] = new Domino(this, res, "verh1"); //создание объекта домино

            highlight1 = new Domino(this, res, "verh");  //создание объекта домино

            //добавление обаботчика события щелчка по крайнему правому выделению

            highlight1.PicBox.Click += new System.EventHandler(highlight1.RightClick);

            highlight2 = new Domino(this, res, "verh");  //создание объекта домино

            //добавление обаботчика события щелчка по крайнему левому выделению

            highlight2.PicBox.Click += new System.EventHandler(highlight2.LeftClick);

            // verh1.PicBox.Click += new ClickHandler(verh1.RightClick);

            AddDomino(7, user_list, panel2, 2);         // формирование списка костей игрока-человека

            AddDomino(7, comp_list, panel3, 3);         // формирование списка костей игрока-компьютера



            // Значения по умолчанию

            step = 0;   //кол-во совершенных ходов

            turn_right = 0;

            turn_left = 0;

            UpdatePanel(panel2, user_list); // Добавление  домино на панель пользователя

            UpdatePanel(panel3, comp_list); // Добавление  домино на панель компьютера

            //блокировка ходов игроков

            LockMove(user_list);

            LockMove(comp_list);

            //разблокировать ход компьютеру

            UnlockMove(comp_list);

            player = 3; //первый ход совершает компьютер

            button1.Enabled = false; //кнопка добавления домино неактивна

            ComputerPlay(); //компьютер делает ход


        }



        // Добавление домино

        void AddDomino(int count, ArrayList list, Panel panel, int num)
        {

            int rnd; //переменная для записи результата генерации

            if (bazar_list.Count != 0) //если базар не пуст
            {

                for (int i = 0; i < count; i++) //n -количество костей, которые нужно добавить
                {

                    rnd = rand.Next(0, bazar_list.Count); // Случайное число

                    list.Add(bazar_list[rnd]);  //добавление кости в список костей, участствующих в игре

                    bazar_list.RemoveAt(rnd); //удаление выбранной кости из исходного списка костей

                    label1.Text = bazar_list.Count.ToString();  //поменять кол-во оставшихся на базаре костей

                    Domino current = (Domino)list[i]; //текущая кость равна выбранной

                    current.arrayList = list; // текущий список

                    current.panel = panel; // текущая панель

                    current.num = num; //номер текущей панели

                    if (bazar_list.Count == 0) //если базар пуст

                        CheckEnd();  //проверить выполнение условий окончания игры
                }

            }
        }



        // Добавление домино на панель

        public void UpdatePanel(Panel panel, ArrayList list)
        {

            int x = 0; //начальная координата

            for (int i = 0; i < list.Count; i++)
            {
                Domino current = (Domino)list[i]; //текущий список объектов

                Point point = new Point(x += 60, 15); //создание точки

                if (current.panel != panel3) //если ходит пользователь

                    current.PositionControl(true, point, panel); //корректирование положения текущей кости пользователя

                else back_dmn[i].PositionControl(true, point, panel); //иначе - корректирование положения текущей кости компьютера (значение кости скрыто)

                current.AddEvent(); //добавление события нажатия на домино на панели игрока

            }

        }



        // Удаление всех PictureBox c панели

        public void CleanPanel(Panel panel)
        {
            //перебор элементов управления в панели
            for (int i = panel.Controls.Count - 1; i >= 0; i--)
            {
                PictureBox pic = panel.Controls[i] as PictureBox; //создание объекта PictureBox

                //если объект не равен null, то удалить

                if (pic != null)

                    panel.Controls.RemoveAt(i);
            }
        }

        //Выбор кости компьютером для осуществления хода
        public void ComputerPlay()
        {
            bool IsContains = false; //флаг, показывает наличие элемента в списке костей

            if (step == 0) //если ходов еще не было
            {
                //выбор случайной кости

                int rnd = rand.Next(0, comp_list.Count); // генерация индекса кости 
                current = (Domino)comp_list[rnd];  //выбор кости для первого хода по сгенерированному индексу 
                current.ControlDice(); //начать игру 

            }
            else
            {
                if (comp_list.Count != 0) //если базар не пуст
                {
                    //перебор костей компьютера

                    foreach (Domino item in comp_list.ToArray())
                    {


                        if (left_value == item.zn1 || left_value == item.zn2 || right_value == item.zn2 || right_value == item.zn1)
                        {
                            IsContains = true; // установить значений true
                            current = item; //использовать найденную подходящую кость для хода
                            current.ControlDice(); //сделать ход
                            break;
                        }

                    }
                    if (!IsContains) //не содержит необходимые кости
                    {
                        TakeFromBazar(); //взять из базара

                    }
                }
                else TakeFromBazar(); //взять из базара
            }
        }
        //взятие кости из базара компьютером
        public void TakeFromBazar()
        {
            if (bazar_list.Count != 0) //базар не пуст
            {
                NewDice(comp_list, panel3, 3); //взятие новой кости
                ComputerPlay(); //компьютер делает ход
            }
            else //базар пуст
            {
                button1.BackColor = SystemColors.GradientInactiveCaption; //изменение цвета кнопки

                button1.Enabled = false;   //кнопка неактивна

                //если нет подходящих костей для хода, пропустить ход (поменять текущего игрока)

                if (NoSuitable(comp_list))
                    ChangePlayer();

            }

        }



        // Нажатие на кнопку "Базар"

        private void button1_Click(object sender, EventArgs e)

        {

            if (bazar_list.Count != 0) //базар не пуст

            {
                NewDice(user_list, panel2, 2);

            }

            else //базар пуст

            {
                MessageBox.Show("Фишек/Костей больше нет", "Базар пуст", MessageBoxButtons.OK, MessageBoxIcon.Warning); //предупреждение о том, что базар пуст

                button1.BackColor = SystemColors.GradientInactiveCaption; //изменение цвета кнопки

                button1.Enabled = false;   //кнопка неактивна

            }


        }



        public void NewDice(ArrayList list, Panel panel, int num)
        {
            Domino tmp; //временный объект класса dominoClass

            int x = 0; //переменная для запоминания координаты по Ох

            if (list.Count != 0) //у текущего игрока есть кости
            {

                tmp = (Domino)list[list.Count - 1]; //tmp присваивается последний элемент списка костей игрока

                x = tmp.PicBox.Location.X; //координата по Ох для последней кости игрока

                CleanPanel(panel); //очистка панели игрока-человека

                tmp = null; //удаление значения временного объекта

            }

            AddDomino(1, list, panel, 2); //добавить 1 кость

            tmp = (Domino)list[list.Count - 1]; //tmp присваивается добавленный элемент списка костей игрока

            Point point = new Point(x += 60, 20); //новая точка, координата по Ох - координата предпоследней кости игрока + 60

            tmp.PositionControl(true, point, panel); //корректирование размера tmp

            tmp.arrayList = list; //списку временного объекта присваивается список костей игрока

            tmp.panel = panel; //панели временного объекта присваивается панель игрока

            tmp.num = num; //номер панели

            UpdatePanel(panel, list); //добавление домино на панель

            ButtonMode(list); //установка режима кнопки базара
        }



        // Блокирование хода игрока

        public void LockMove(ArrayList list)

        {
            //перебор всех костей 

            for (int i = 0; i < list.Count; i++)
            {
                //блокировка каждого PicBox

                Domino current = (Domino)list[i]; //объект dominoClass

                current.PicBox.Click -= new System.EventHandler(current.Dice_Click); //удалить обработчик события нажатия на кость

            }

        }


        // Разблокирование хода игрока

        public void UnlockMove(ArrayList list)
        {
            for (int i = 0; i < list.Count; i++) //перебор элементов списка
            {
                Domino current = (Domino)list[i]; //объект dominoClass

                current.PicBox.Click += new System.EventHandler(current.Dice_Click); //добавление обработчика события нажатия на кость

                current.PicBox.Visible = true; //видимость элемента
            }

        }


        // Нажатие на Panel1

        public void panel1_Click(object sender, EventArgs e)
        {
            if (player == 2 && current != null) //если не было нажатия на пустое пространство и ход делает пользователь
            {
                UnlockMove(current.arrayList); // Разблокирование хода игрока

                current = null; //присвоить null

                //удаление выделений

                panel1.Controls.Remove(highlight1.PicBox);

                panel1.Controls.Remove(highlight2.PicBox);
            }
        }

        // Изменение размеров панелей
        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            point1.X = panel1.Width / 2 - 26; point1.Y = panel1.Height / 2 - 135; //изменение координаты point1

            point2.X = panel1.Width / 2 - 26; point2.Y = panel1.Height / 2 - 135; //изменение координаты point2

            panel1.SizeChanged -= new System.EventHandler(panel1_SizeChanged); //удаление события

        }

        private void panel2_SizeChanged(object sender, EventArgs e)
        {
            point1.X = panel2.Width / 2 - 26; point1.Y = panel1.Height / 2 - 135; //изменение координаты point1

            point2.X = panel2.Width / 2 - 26; point2.Y = panel1.Height / 2 - 135; //изменение координаты point2

            panel2.SizeChanged -= new System.EventHandler(panel2_SizeChanged); //удаление события

        }

        private void panel3_SizeChanged(object sender, EventArgs e)
        {
            point1.X = panel3.Width / 2 - 26; point1.Y = panel1.Height / 2 - 135; //изменение координаты point1

            point2.X = panel3.Width / 2 - 26; point2.Y = panel1.Height / 2 - 135; //изменение координаты point2

            panel3.SizeChanged -= new System.EventHandler(panel3_SizeChanged); //удаление события

        }

        

        // Изменение игрока
        public ArrayList ChangePlayer()
        {
            //если текущий игрок - человек
            if (player == 2)
            {
                UnlockMove(comp_list); // Разблокирование хода игрока

                player = 3; //игрок - компьютер

                label2.Text = "Ход компьютера"; //запись в лейбл

                ButtonMode(comp_list); //установка режима кнопки базара

                if (button1.Enabled == true) //если кпопка базара активна

                    NoSuitable(bazar_list); // Определение наличия в базаре костей для осуществления хода

                //перерисовывание панелей во избежание зависания формы

                panel1.Refresh();
                panel2.Refresh();
                panel3.Refresh();

                Thread.Sleep(1500);  //"задержка" при выполнении хода компьютером

                ComputerPlay(); //компьютер делает ход

                return comp_list; //вернуть список костей компьютера

            }
            //иначе
            else
            {
                UnlockMove(user_list); // Разблокирование хода игрока

                player = 2; //игрок - человек

                label2.Text = "Ваш ход"; //запись в лейбл

                ButtonMode(user_list); //установка режима кнопки базара

                if (button1.Enabled == true) //если кпопка базара активна

                    NoSuitable(bazar_list); // Определение наличия в базаре костей для осуществления хода

                return user_list; //вернуть список костей пользователя
            }

        }

        //проверка выполнение условий окончания игры

        public void CheckEnd()

        {
            if (user_list.Count == 0 || comp_list.Count == 0) //нет подходящих костей для хода
            {
                if (user_list.Count == 0) //у человека не осталось костей
                {
                    wins_num++; //увеличить кол-во выигранных игр
                    message = "Вы победили!"; //победа
                }
                else if (comp_list.Count == 0) //у компьютера не осталось костей
                {
                    message = "Вы проиграли!"; //проигрыш
                    losses_num++; //увеличить кол-во проигранных игр
                }
            }

            else if (NoSuitable(user_list) && NoSuitable(comp_list)) //нет подходящих костей для хода
            {
                //ничья

                message = "Ходов больше нет";

                no_winners = true;
            }
            

            if (message != null) //если есть сообщение
            {
                //очистить панели

                CleanPanel(panel2);

                CleanPanel(panel3);

                //добавление домино на панель

                UpdatePanel(panel2, user_list);

                UpdatePanel(panel3, comp_list);

                game_num++; //увеличить общее число игр

                AddToRate(); //добавить запись в файл

                //вывести на экран сообщение

                //показать рейтинг, если выбрано "Да"

                if (MessageBox.Show($"{ message} '\n' Показать рейтинг?", "Игра окончена", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)

                {
                    ShowRate();
                }
                //иначе - выход из приложения

                else Application.Exit(); //выход
            }
        }


        // Режим кнопки базара

        public void ButtonMode(ArrayList list)
        {
            bool tmp_bool = true; //флаг, с помощью которого устанавливается режим кпопки

            if (list == comp_list) //если играет компьютер, то кпопка неактивна для пользователя
                tmp_bool = false;
            else //иначе
            {
                //перебор костей

                for (int i = 0; i < list.Count; i++)
                {

                    Domino tmp = (Domino)list[i]; //временный объект dominoClass

                    //если верхнее или нижнее значение кости равно крайнему левому или крайнему правому  

                    if (tmp.zn1 == left_value || tmp.zn1 == right_value || tmp.zn2 == left_value || tmp.zn2 == right_value)

                    {
                        //у игрока есть подходящие для осуществления хода костяшки

                        tmp_bool = false; //изменить значение флага

                        button1.BackColor = SystemColors.GradientInactiveCaption; //изменить цвет кнопки

                        break; //выход из цикла

                    }

                }
            }
            button1.Enabled = tmp_bool; // режим кнопки в зависимости от значения флага

        }



        // Определение наличия у игрока костей для осуществления хода

        public bool NoSuitable(ArrayList list)
        {

            bool tmp_bool = true; //флаг
            //если список не пуст
            if (list.Count != 0)
            {
                //перебор элементов

                for (int i = 0; i < list.Count; i++)

                {

                    Domino tmp = (Domino)list[i];//временный объект dominoClass, содержащий значения костей из  базара

                    //если верхнее или нижнее значение кости из базара равно крайнему левому или крайнему правому

                    if (tmp.zn1 == left_value || tmp.zn1 == right_value || tmp.zn2 == left_value || tmp.zn2 == right_value)

                    {

                        // есть подходящие для осуществления хода костяшки

                        tmp_bool = false; // изменение флага

                        break; //выход из цикла

                    }

                }
            }

            return tmp_bool;
            // false - есть кости для осуществления текущего хода
        }



        //запись в файл
        public void AddToRate()
        {
            string text; //текст записи

            if (player == 2) //послейдний игрок - человек
                text = game_num.ToString() + '\t' + "Пользователь" + '\t' + "Компьютер" + '\t' + wins_num.ToString() + '\t' + losses_num.ToString() + '\t' + ';' + "\r\n";
            else if (no_winners) //ничья
                text = game_num.ToString() + '\t' + "-" + '\t' + "-" + '\t' + wins_num.ToString() + '\t' + losses_num.ToString() + '\t' + ';' + "\r\n";
            else //иначе
                text = game_num.ToString() + '\t' + "Компьютер" + '\t' + "Пользователь" + '\t' + wins_num.ToString() + '\t' + losses_num.ToString() + '\t' + ';' + "\r\n";

            // добавление в файл

            if (!File.Exists(path)) //создать файл, если он не существет
                File.Create(path).Close();
            File.AppendAllText(path, text); //добавить запись в файл
        }

        
        //нажатие кнопки выхода
        private void button2_Click(object sender, EventArgs e)
        {
            //если выбрано "Да" - выход
            if (MessageBox.Show("Вы хотите выйти?", "Конец игры", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

            {
                Application.Exit(); //выход
            }
        }

        //нажатие кнопки пропуска хода
        private void button4_Click(object sender, EventArgs e)
        {
            if (bazar_list.Count == 0) //если базар пустой
            {
                if (NoSuitable(user_list)) //если нет нужных костей 
                {
                    ChangePlayer(); //изменить игрока

                }
            }

            else //иначе - показать сообщение
                MessageBox.Show("Возьмите кости из Базара", "Базар не пуст", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        //нажатие кнопки показа правил игры
        private void button3_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(); //ссылка на Form2

            f.ShowDialog(); //показать диалоговое окно
        }

        //показать рейтинг

        public void ShowRate()
        {
            Form3 form = new Form3(); //ссылка на Form3

            StreamReader f = new StreamReader(path); //экземпляр StreamReader

            while (!f.EndOfStream) //пока не достигнут конец потока
            {
                string s = f.ReadLine(); //считывание
                string[] d = s.Split(';'); //разбинение на строки

                //создание массива для каждой строки по разделителю '\t'

                foreach (var i in d)
                {
                    if (i != "") //если строка не пустая
                    {
                        string[] temp = i.Split('\t'); //разбинение строки

                        //Добавляем строку dataGridView1, указывая значения колонок поочереди слева направо

                        form.dataGridView1.Rows.Add(temp[0], temp[1], temp[2]);

                        //Добавление текста в лейблы

                        form.label4.Text = temp[3];
                        form.label5.Text = temp[4];

                    }
                }

            }
            f.Close(); //закрыть поток
            form.ShowDialog(); //показать диалоговое окно
        }



    }

}



