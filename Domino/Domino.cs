using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;           // Bitmap
using System.Resources;         // ResourceManager
using System.Windows.Forms;     // PictureBox
using System.Collections;       // ArrayList

namespace Domino
{
    public class Domino
    {



        public Form1 form;                  // Ссылка на главную  форму

        string name;                        // Имя объекта

        public int zn1;                     // Значение 1 половины кости

        public int zn2;                     // Значение 2 половины кости      

        PictureBox pictureBox;              // Контейнер для рисунка

        public ArrayList arrayList;         // Список, в котором расположен домино

        public Panel panel;                 // Панель, на которой расположен объект домино

        public int num;                     // Номер списка/панели

        public bool rotate;                 // перевёрнуто ли изображение объекта



        // Конструктор объекта "Домино"

        public Domino(Form1 f1, ResourceManager resMan, string name)

        {

            form = f1;

            this.name = name;    // Записываю имя

            this.pictureBox = new PictureBox();

            if (name != "verh")
            {

                if (name != "verh1")
                {

                    this.zn1 = int.Parse(name.Substring(1, 1));

                    this.zn2 = int.Parse(name.Substring(3, 1));

                    this.pictureBox.Image = (Bitmap)resMan.GetObject(this.name);

                }
                else
                {
                    this.pictureBox.Image = (Bitmap)resMan.GetObject("verh");
                    this.pictureBox.Size = new Size(47, 87);
                }

            }

            else

            {
                this.pictureBox.BorderStyle = BorderStyle.FixedSingle;

                this.pictureBox.BackColor = Color.DarkSeaGreen;

            }

        }



        // Корректирование положения

        public void PositionControl(bool rotate, Point point, Panel panel)

        {
            //изображение перевернуто
            if (rotate)
            {
                // вертикальные размеры

                this.pictureBox.Size = new Size(47, 87);

                this.pictureBox.Location = point; //координаты верхнего левого угла текущего pictureBox

            }

            else

            {   // горизонтальные размеры

                this.pictureBox.Size = new Size(87, 47);

                // если название изображения равно "verh"

                if (this.name != "verh")

                    this.pictureBox.Image.RotateFlip(RotateFlipType.Rotate270FlipNone); //поворот изображения на 270 градусов

                point.Y = point.Y + 20; //увеличение координаты по Оу на 20

                this.pictureBox.Location = point; //координаты верхнего левого угла текущего pictureBox

            }

            this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom; //отображение изображения с сохранением пропорций

            panel.Controls.Add(this.pictureBox); //добавление кости на панель

        }



        // Добавить  событие Dice_Click

        public void AddEvent()

        {

            this.pictureBox.Click += new System.EventHandler(Dice_Click);

        }



        // Удалить событие Dice_Click

        public void DeleteEvent()

        {

            this.pictureBox.Click -= new System.EventHandler(Dice_Click);

        }



        // Свойство, возвращающее PicturBox

        public PictureBox PicBox
        {
            get
            {
                return this.pictureBox;
            }
        }

        //Метод, выполняющий необходимые манипуляции над костями компьютера

        public void ControlDice()
        {
            if (form.step == 0) //если счет равен 0
            {
               
                form.left_value = form.right_value = this.zn1;
            }
            else

            {   // Задание координат для выделения (по левую сторону)

                Point point2 = form.point2;

                if (point2.X > 200 && form.turn_left == 0)

                {   // ПО ВЕРХНЕЙ ГОРИЗОНТАЛИ  

                    //изменение крайней левой координаты по Ох

                    if (this.zn1 == this.zn2)

                        point2.X = point2.X - 48; 

                    else

                        point2.X = point2.X - 87;

                    //левое выделение

                    form.highlight2.PositionControl(this.zn1 == this.zn2, point2, form.panel1);  //рисование левого выделения


                    form.puf = false;

                }

                else

                {
                    //изменение крайней левой координаты по Оу

                    if (point2.Y < 300 && form.turn_left < 2)

                    {   // ПО ВЕРТИКАЛИ

                        if (form.turn_left == 0) //если кол-во поворотов влево равно 0

                            pointPNT_300(point2);

                        else if (form.turn_left == 1) //если кол-во поворотов влево равно 1

                            pointPNT_333(point2);

                    }

                    else

                    {   // ПО НИЖНЕЙ ГОРИЗОНТАЛИ

                        if (form.turn_left == 2) //если кол-во поворотов влево равно 2

                            pointPNT_311(point2);

                        else if (form.turn_left == 3) //если кол-во поворотов влево равно 3

                            pointPNT_322(point2);

                    }

                }

            }

            // ---------------------------------------------------------------------------------

            Point point1 = form.point1;

            if (point1.X < 1200 && form.turn_right == 0)

            {   // ПО ВЕРХНЕЙ ГОРИЗОНТАЛИ

                form.highlight1.PositionControl(this.zn1 == this.zn2, point1, form.panel1); //рисование правого выделения

            }

            else

            {

                if (point1.Y < 300 && form.turn_right < 2)

                {   // ПО ВЕРТИКАЛИ

                    if (form.turn_right == 0) //если кол-во поворотов вправо равно 0

                        pointPNT_200(point1);

                    else if (form.turn_right == 1) //если кол-во поворотов вправо равно 1

                        pointPNT_111(point1);

                }

                else

                {   // ПО НИЖНЕЙ ГОРИЗОНТАЛИ

                    if (form.turn_right == 2) //если кол-во поворотов вправо равно 2

                        pointPNT_211(point1);

                    else if (form.turn_right == 3) //если кол-во поворотов вправо равно 3

                        pointPNT_222(point1);

                }

            }

            form.LockMove(this.arrayList); //заблокировать ход


            if (CompareRight() == 1) //если крайнее правое значение равно значению кости
            {
                form.current.pictureBox.Click -= new System.EventHandler(Dice_Click);

                form.current.pictureBox.BackColor = Color.DarkSeaGreen; //цвет pictureBox

                //удалить выделения

                form.panel1.Controls.Remove(form.highlight1.pictureBox);

                form.panel1.Controls.Remove(form.highlight2.pictureBox);

                //Point
                point1 = form.point1; //координата крайней правой точки

                if (point1.X < 1200 && form.turn_right == 0)

                {   // ПО ВЕРХНЕЙ ГОРИЗОНТАЛИ

                    form.current.PositionControl(form.current.zn1 == form.current.zn2, point1, form.panel1); //корректировка положения кости

                    form.point1.X += form.current.pictureBox.Size.Width;

                    form.IsRightDouble = (form.current.zn1 == form.current.zn2);

                }

                else
                {
                    point1 = form.highlight1.pictureBox.Location; //координаты верхнего левого угла pictureBox

                    if (point1.Y < 300 && form.turn_right < 2)

                    {   // ПО ВЕРТИКАЛИ

                        if (form.turn_right == 0)

                            pointPNT_20(point1);

                        else if (form.turn_right == 1)

                            pointPNT_11(point1);


                    }

                    else

                    {   // ПО НИЖНЕЙ ГОРИЗОНТАЛИ

                        if (form.turn_right == 2)

                            pointPNT_21(point1);

                        else if (form.turn_right == 3)

                            pointPNT_22(point1);

                    }

                }

                form.current.arrayList.RemoveAt(form.current.arrayList.IndexOf(form.current)); //удалить текущую кость из списка

                //очистить панель

                form.CleanPanel(form.current.panel); //очистить панель

                form.UpdatePanel(form.current.panel, form.current.arrayList); //добавление домино на панель

                form.LockMove(form.current.arrayList); //заблокировать ход

                form.left_value = form.current.zn2; //крайнее левое значение равно второму значению кости

                if (form.bazar_list.Count == 0) //базар пуст

                    form.CheckEnd(); //проверить возможность завершения игры

                form.ChangePlayer(); //изменить игрока

                form.step++; //увеличить кол-во ходов

            }
            else if (CompareLeft() == 1) //если крайнее левое значение равно значению кости

            {

                form.current.DeleteEvent(); //удалить событие

                form.current.pictureBox.BackColor = Color.DarkSeaGreen; //цвет pictureBox

                //удалить выделения

                form.panel1.Controls.Remove(form.highlight1.pictureBox);

                form.panel1.Controls.Remove(form.highlight2.pictureBox);

                Point point2 = form.highlight2.pictureBox.Location; //координаты верхнего левого угла pictureBox

                if (form.turn_left == 0 && form.puf == false)

                {   // ПО ВЕРХНЕЙ ГОРИЗОНТАЛИ

                    if (form.current.zn1 != form.current.zn2)

                    {

                        point2.Y -= 20;  //изменение крайней левой координаты по Оу

                    }

                    form.current.PositionControl(form.current.zn1 == form.current.zn2, point2, form.panel1); //корректировка положения кости

                    form.point2 = point2; //координата крайней левой точки

                    form.IsLeftDouble = (form.current.zn1 == form.current.zn2);

                }

                else

                {

                    point2 = form.highlight2.pictureBox.Location; //координаты верхнего левого угла pictureBox

                    if (point2.Y < 300 && form.turn_left < 2)

                    {   // ПО ВЕРТИКАЛИ

                        if (form.turn_left == 0)

                            pointPNT_30(point2);

                        else if (form.turn_left == 1)

                            pointPNT_33(point2);

                    }

                    else

                    {   // ПО НИЖНЕЙ ГОРИЗОНТАЛИ

                        if (form.turn_left == 2)

                            pointPNT_31(point2);

                        else if (form.turn_left == 3)

                            pointPNT_32(point2);

                    }

                }



                form.current.arrayList.RemoveAt(form.current.arrayList.IndexOf(form.current)); //удалить текущую кость из списка

                form.CleanPanel(form.current.panel); //очистить панель

                form.UpdatePanel(form.current.panel, form.current.arrayList); //добавление домино на панель

                form.LockMove(form.current.arrayList); //заблокировать ход

                form.right_value = form.current.zn1; //крайнее правое значение равно первому значению текущей кости

                if (form.bazar_list.Count == 0) //базар пуст

                    form.CheckEnd(); //проверить возможность завершения игры

                form.ChangePlayer(); //изменить игрока

                form.step++; //увеличить кол-во ходов
            }

        }

        // Событие нажатия на домино на панели игрока

        public void Dice_Click(object sender, EventArgs e)
        {
            if (form.player == 2) //если игрок - пользователь
            {
                form.current = this; //текущий объект

                form.puf = true;

                form.current.PicBox.Visible = true; //видимость PicBox

                if (form.step == 0) //если счет равен 0
                {
                    form.left_value = form.right_value = this.zn1;
                }

                else

                {   // Задание координат для выделения (по левую сторону)

                    Point point2 = form.point2;

                    if (point2.X > 200 && form.turn_left == 0)

                    {   // ПО ВЕРХНЕЙ ГОРИЗОНТАЛИ  

                        if (this.zn1 == this.zn2)

                            point2.X = point2.X - 48;

                        else

                            point2.X = point2.X - 87;

                        form.highlight2.PositionControl(this.zn1 == this.zn2, point2, form.panel1); //рисование левого выделения

                        form.puf = false; 

                    }

                    else

                    {

                        if (point2.Y < 300 && form.turn_left < 2)

                        {   // ПО ВЕРТИКАЛИ

                            if (form.turn_left == 0)

                                pointPNT_300(point2);

                            else if (form.turn_left == 1)

                                pointPNT_333(point2);

                        }

                        else

                        {   // ПО НИЖНЕЙ ГОРИЗОНТАЛИ

                            if (form.turn_left == 2)

                                pointPNT_311(point2);

                            else if (form.turn_left == 3)

                                pointPNT_322(point2);

                        }

                    }

                }

                // ---------------------------------------------------------------------------------

                Point point1 = form.point1;

                if (point1.X < 1200 && form.turn_right == 0)

                {   // ПО ВЕРХНЕЙ ГОРИЗОНТАЛИ

                    form.highlight1.PositionControl(this.zn1 == this.zn2, point1, form.panel1); //рисование правого выделения

                }

                else

                {

                    if (point1.Y < 300 && form.turn_right < 2)

                    {   // ПО ВЕРТИКАЛИ

                        if (form.turn_right == 0)

                            pointPNT_200(point1);

                        else if (form.turn_right == 1)

                            pointPNT_111(point1);

                    }

                    else

                    {   // ПО НИЖНЕЙ ГОРИЗОНТАЛИ

                        if (form.turn_right == 2)

                            pointPNT_211(point1);

                        else if (form.turn_right == 3)

                            pointPNT_222(point1);

                    }

                }




                form.LockMove(this.arrayList); //заблокировать ход

            }
        }


        // Событие нажатия правого выделения

        public void RightClick(object sender, EventArgs e)
        {
            if (form.player == 2)
            {
                if (CompareRight() == 1) //если крайнее правое значение равно значению кости
                {
                    form.current.pictureBox.Click -= new System.EventHandler(Dice_Click); //удаление обработчика события нажатия на домино

                    form.current.pictureBox.BackColor = Color.DarkSeaGreen; //цвет pictureBox

                    //удаление выделений на игровом поле

                    form.panel1.Controls.Remove(form.highlight1.pictureBox);

                    form.panel1.Controls.Remove(form.highlight2.pictureBox);

                    Point point1 = form.point1;

                    if (point1.X < 1200 && form.turn_right == 0)

                    {   // ПО ВЕРХНЕЙ ГОРИЗОНТАЛИ

                        form.current.PositionControl(form.current.zn1 == form.current.zn2, point1, form.panel1); //корректировка положения кости

                        form.point1.X += form.current.pictureBox.Size.Width;

                        form.IsRightDouble = (form.current.zn1 == form.current.zn2);

                    }

                    else
                    {
                        point1 = form.highlight1.pictureBox.Location; //координаты верхнего левого угла pictureBox

                        if (point1.Y < 300 && form.turn_right < 2)

                        {   // ПО ВЕРТИКАЛИ

                            if (form.turn_right == 0)

                                pointPNT_20(point1);

                            else if (form.turn_right == 1)

                                pointPNT_11(point1);

                        }

                        else

                        {   // ПО НИЖНЕЙ ГОРИЗОНТАЛИ

                            if (form.turn_right == 2)

                                pointPNT_21(point1);

                            else if (form.turn_right == 3)

                                pointPNT_22(point1);

                        }

                    }

                    form.current.arrayList.RemoveAt(form.current.arrayList.IndexOf(form.current)); //удалить текущую кость из списка

                    form.CleanPanel(form.current.panel); //очистить панель

                    form.UpdatePanel(form.current.panel, form.current.arrayList); //добавление домино на панель

                    form.LockMove(form.current.arrayList); //заблокировать ход

                    form.left_value = form.current.zn2; //крайнее левое значение равно второму значению кости

                    if (form.bazar_list.Count == 0) //базар пуст 

                        form.CheckEnd(); //проверить возможность завершения игры

                    form.ChangePlayer(); //изменить игрока

                    form.step++; //увеличить кол-во ходов
                }

                else

                    form.panel1_Click(null, null);
            }

        }



        // Событие  нажатия выделения слева

        public void LeftClick(object sender, EventArgs e)
        {
            if (form.player == 2) //если игрок- пользователь
            {

                if (CompareLeft() == 1) //если крайнее левое значение равно значению кости 

                {
                    form.current.DeleteEvent(); //удалить событие

                    form.current.pictureBox.BackColor = Color.DarkSeaGreen; //цвет pictureBox

                    //удалить выделения

                    form.panel1.Controls.Remove(form.highlight1.pictureBox);

                    form.panel1.Controls.Remove(form.highlight2.pictureBox);

                    Point point2 = form.highlight2.pictureBox.Location; //координаты верхнего левого угла pictureBox

                    if (form.turn_left == 0 && form.puf == false)

                    {   // ПО ВЕРХНЕЙ ГОРИЗОНТАЛИ

                        if (form.current.zn1 != form.current.zn2)
                        {
                            point2.Y -= 20;  //изменение крайней левой координаты по Оу
                        }

                        form.current.PositionControl(form.current.zn1 == form.current.zn2, point2, form.panel1); //корректировка положения кости

                        form.point2 = point2;

                        form.IsLeftDouble = (form.current.zn1 == form.current.zn2); //задание значения IsLeftDouble

                    }

                    else

                    {

                        point2 = form.highlight2.pictureBox.Location; //координаты верхнего левого угла pictureBox

                        if (point2.Y < 300 && form.turn_left < 2)

                        {   // ПО ВЕРТИКАЛИ

                            if (form.turn_left == 0)

                                pointPNT_30(point2);

                            else if (form.turn_left == 1)

                                pointPNT_33(point2);

                        }

                        else

                        {   // ПО НИЖНЕЙ ГОРИЗОНТАЛИ

                            if (form.turn_left == 2)

                                pointPNT_31(point2);

                            else if (form.turn_left == 3)

                                pointPNT_32(point2);

                        }

                    }

                    form.current.arrayList.RemoveAt(form.current.arrayList.IndexOf(form.current)); //удалить текущую кость из списка

                    form.CleanPanel(form.current.panel); //очистить панель

                    form.UpdatePanel(form.current.panel, form.current.arrayList); //добавление домино на панель

                    form.LockMove(form.current.arrayList); //заблокировать ход

                    form.right_value = form.current.zn1; //крайнее правое значение равно первому значению текущей кости

                    if (form.bazar_list.Count == 0) //базар пуст

                        form.CheckEnd();  //проверить возможность завершения игры

                    form.ChangePlayer(); //изменить игрока

                    form.step++; //увеличить кол-во ходов

                }

                else

                    form.panel1_Click(null, null);

            }
        }



        // Сравнение  сторон (по правой)

        public int CompareRight()

        {

            if (form.left_value == form.current.zn1)

            {

                if (form.turn_right > 1) //если кол-во поворотов больше 1

                {

                    form.current.pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);

                }

                return 1;

            }

            if (form.left_value == form.current.zn2)

            {

                if (form.turn_right < 2) //если кол-во поворотов меньше 2

                {

                    form.current.pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);

                }

                int current = form.current.zn1;

                form.current.zn1 = form.current.zn2;

                form.current.zn2 = current;

                return 1;

            }

            return -1;

        }



        // Сравнение  сторон (по левой)

        public int CompareLeft()

        {

            if (form.right_value == form.current.zn2) //значение крайней правой костяшки на игровом поле равно значению второй половины текущей

            {

                return 1;

            }

            if (form.right_value == form.current.zn1) //значение крайней правой костяшки на игровом поле равно значению первой половины текущей

            {

                form.current.pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipNone); //поворот кости на 180 градусов

                //поменять местами значения строн кости

                int current = form.current.zn1;

                form.current.zn1 = form.current.zn2;

                form.current.zn2 = current;

                return 1;

            }

            return -1;

        }

        

        // ПОВОРОТ: выделение по правой  стороне, поворот вниз

        void pointPNT_200(Point point1)

        {
            if (form.IsRightDouble && !(this.zn1 == this.zn2))

            {
                //изменение крайней правой координаты

                point1.X -= 48;

                point1.Y += 87;

                form.highlight1.rotate = true;

                form.highlight1.PositionControl(true, point1, form.panel1); //рисование правого выделения

            }

            if (!form.IsRightDouble && !(this.zn1 == this.zn2))

            {
                //изменение крайней правой координаты

                point1.X -= 48;

                point1.Y += 48 + 20;

                form.highlight1.rotate = true;

                form.highlight1.PositionControl(true, point1, form.panel1); //рисование правого выделения

            }

            if (!form.IsRightDouble && (this.zn1 == this.zn2))

            {
                //изменение крайней правой координаты

                point1.X -= 48 + 20;

                point1.Y += 48;

                form.highlight1.rotate = false;

                form.highlight1.PositionControl(false, point1, form.panel1); //рисование правого выделения

            }

        }



        // ПОВОРОТ:  выделение по правой стороне

        void pointPNT_111(Point point1)

        {
            if (form.IsRightDouble && !(this.zn1 == this.zn2))

            {

                form.highlight1.PositionControl(true, point1, form.panel1); //рисование правого выделения

                form.highlight1.rotate = true;

            }

            if (form.IsRightDouble && (this.zn1 == this.zn2))

            {
                //изменение крайней правой координаты

                point1.X -= 20;

                point1.Y -= 20;

                form.highlight1.PositionControl(false, point1, form.panel1); //рисование правого выделения

                form.highlight1.rotate = false;

            }

            if (!form.IsRightDouble && !(this.zn1 == this.zn2))

            {

                form.highlight1.PositionControl(true, point1, form.panel1); //рисование правого выделения

                form.highlight1.rotate = true;

            }

        }



        // ПОВОРОТ:  выделение по правой стороне,  поворот ОБРАТНО

        void pointPNT_211(Point point1)
        {
            if (form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {
                //изменение крайней правой координаты

                point1.X -= 87;

                point1.Y -= 48 + 20;

                form.highlight1.PositionControl(false, point1, form.panel1); //рисование правого выделения

                form.highlight1.rotate = false;

                point1.Y -= 20;
            }

            else if (!form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {

                //изменение крайней правой координаты

                point1.X -= 87 + 20;

                point1.Y -= 48 + 20;

                form.highlight1.PositionControl(false, point1, form.panel1); //рисование правого выделения

                form.highlight1.rotate = false;


            }

            else if (!form.IsRightDouble && (form.current.zn1 == form.current.zn2))

            {

                //изменение крайней правой координаты

                point1.X -= 48;

                point1.Y -= 48 - 20;

                form.highlight1.PositionControl(true, point1, form.panel1); //рисование правого выделения

                form.highlight1.rotate = true;

            }

            else if (form.IsRightDouble && (form.current.zn1 == form.current.zn2))

            {

                //изменение крайней правой координаты

                point1.X -= 48;

                point1.Y -= 48 + 20;

                form.highlight1.PositionControl(true, point1, form.panel1); //рисование правого выделения

                form.highlight1.rotate = true;


            }

        }



        // ПОВОРОТ:  выделение по правой стороне  ОБРАТНО

        void pointPNT_222(Point point1)
        {
            if (form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {
                //изменение крайней правой координаты по Ох

                point1.X -= 87;

                form.highlight1.PositionControl(false, point1, form.panel1); //рисование правого выделения

                form.highlight1.rotate = false;

            }

            else if (!form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {
                //изменение крайней правой координаты

                point1.X -= 87;

                point1.Y += 20;

                form.highlight1.PositionControl(false, point1, form.panel1); //рисование правого выделения

                form.highlight1.rotate = false;

            }

            else if (!form.IsRightDouble && (form.current.zn1 == form.current.zn2))

            {
                //изменение крайней правой координаты

                point1.X -= 48;

                point1.Y += 20;

                form.highlight1.PositionControl(true, point1, form.panel1); //Рисование правого выделения

                form.highlight1.rotate = true;

            }

        }



        // ПОВОРОТ: кость по правой  стороне, поворот вниз

        void pointPNT_20(Point point1)

        {
            if (form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {

                form.current.PositionControl(true, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = true;

                //изменение крайней правой координаты по ОУ

                point1.Y += 87;

            }

            if (!form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {

                form.current.PositionControl(true, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = true;

                point1.Y += 87; //изменение крайней правой координаты по Оу

            }

            if (!form.IsRightDouble && (form.current.zn1 == form.current.zn2))

            {

                point1.Y -= 20;

                form.current.PositionControl(false, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = false;

                //изменение крайней правой координаты

                point1.X += 20;

                point1.Y += 48 + 20;

            }

            form.point1 = point1;

            form.turn_right++;

        }



        // ПОВОРОТ:  кость по правой стороне

        void pointPNT_11(Point point1)
        {

            if (form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {

                form.current.PositionControl(true, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = true;

                //изменение крайней правой координаты по Оу

                point1.Y += 87;

            }

            if (form.IsRightDouble && (form.current.zn1 == form.current.zn2))

            {

                point1.Y -= 20;

                form.current.PositionControl(false, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = false;

                //изменение крайней правой координаты

                point1.Y += 48 + 20;

                point1.X += 20;

            }

            if (!form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {

                form.current.PositionControl(true, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = true;

                //изменение крайней правой координаты по Оу

                point1.Y += 87;

            }

            form.point1 = point1;

            if (point1.Y >= 300)

                form.turn_right++;

        }



        // ПОВОРОТ:  кость по правой стороне,  поворот обратно

        void pointPNT_21(Point point1)

        {

            if (form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {

                point1.Y -= 20;

                form.current.PositionControl(false, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = false;

                //изменение крайней правой координаты по Оу

                point1.Y -= 20;

            }

            else if (!form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {

                point1.Y -= 20;

                form.current.PositionControl(false, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = false;

                //изменение крайней правой координаты по Оу

                point1.Y -= 20;

            }

            else if (!form.IsRightDouble && (form.current.zn1 == form.current.zn2))

            {

                form.current.PositionControl(true, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = true;

            }

            else if (form.IsRightDouble && (form.current.zn1 == form.current.zn2))

            {

                form.current.PositionControl(true, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = true;

            }

            form.point1 = point1;

            form.turn_right++;

        }



        // ПОВОРОТ: кость по правой стороне, обратно

        void pointPNT_22(Point point1)
        {
            if (form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {

                point1.Y -= 20;

                form.current.PositionControl(false, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = false;

                //изменение крайней правой координаты по оу

                point1.Y -= 20;

            }



            else if (!form.IsRightDouble && (form.current.zn1 == form.current.zn2))

            {

                form.current.PositionControl(true, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = true;

            }

            else if (!form.IsRightDouble && !(form.current.zn1 == form.current.zn2))

            {

                point1.Y -= 20;

                form.current.PositionControl(false, point1, form.panel1); //корректировка положения кости

                form.IsRightDouble = false;

                point1.Y -= 20;

            }

            form.point1 = point1;

        }



        // --------------------------------------------------------------------------------



        // ПОВОРОТ:  выделение по правой стороне,  поворот вниз

        void pointPNT_300(Point point2)

        {
            if (form.IsLeftDouble && !(this.zn1 == this.zn2))

            {

                point2.X += 87; //изменение крайней левой координаты по Ох

                form.highlight2.rotate = true;

                form.highlight2.PositionControl(true, point2, form.panel1);  //рисование левого выделения


            }

            if (!form.IsLeftDouble && !(this.zn1 == this.zn2))

            {

                point2.Y += 48 + 20;  //изменение крайней левой координаты по Оу

                form.highlight2.rotate = true;

                form.highlight2.PositionControl(true, point2, form.panel1);  //рисование левого выделения


            }

            if (!form.IsLeftDouble && (this.zn1 == this.zn2))

            {
                //изменение крайней левой координаты

                point2.X -= 20;  

                point2.Y += 48;

                form.highlight2.rotate = false;

                form.highlight2.PositionControl(false, point2, form.panel1);  //рисование левого выделения

            }

        }



        // ПОВОРОТ:  выделение по правой стороне

        void pointPNT_333(Point point2)

        {
            if (form.IsLeftDouble && !(this.zn1 == this.zn2))

            {

                form.highlight2.PositionControl(true, point2, form.panel1);  //рисование левого выделения


                form.highlight2.rotate = true;

            }

            if (form.IsLeftDouble && (this.zn1 == this.zn2))    //****

            {
                //изменение крайней левой координаты

                point2.X -= 20;

                point2.Y -= 20;

                form.highlight2.PositionControl(false, point2, form.panel1);  //рисование левого выделения


                form.highlight2.rotate = false;

            }

            if (!form.IsLeftDouble && !(this.zn1 == this.zn2))

            {

                point2.Y += 20;  //изменение крайней левой координаты по Оу

                form.highlight2.PositionControl(true, point2, form.panel1);  //рисование левого выделения


                form.highlight2.rotate = true;

            }

        }



        // ПОВОРОТ:  выделение по правой стороне,  поворот ОБРАТНО

        void pointPNT_311(Point point2)

        {
            if (form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))

            {
                //изменение крайней левой координаты
                
                point2.X += 48;

                point2.Y -= 48 + 20;

                form.highlight2.PositionControl(false, point2, form.panel1);  //рисование левого выделения


                form.highlight2.rotate = false;

            }

            else if (!form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))

            {
                //изменение крайней левой координаты

                point2.X += 48 + 20;

                point2.Y -= 48 + 20;

                form.highlight2.PositionControl(false, point2, form.panel1);  //рисование левого выделения


                form.highlight2.rotate = false;

            }

            else if (form.IsLeftDouble && (form.current.zn1 == form.current.zn2))

            {
                //изменение крайней левой координаты

                point2.X += 48;

                point2.Y -= 48 + 20;

                form.highlight2.PositionControl(true, point2, form.panel1);  //рисование левого выделения

                form.highlight2.rotate = true;

            }

        }



        // ПОВОРОТ:  выделение по правой стороне  ОБРАТНО

        void pointPNT_322(Point point2)

        {
            if (form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))

            {

                form.highlight2.PositionControl(false, point2, form.panel1); //рисование левого выделения

                form.highlight2.rotate = false;

                point2.Y -= 20;  //изменение крайней левой координаты по Оу

            }

            else if (!form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))

            {

                point2.Y += 20;  //изменение крайней левой координаты по Оу

                form.highlight2.PositionControl(false, point2, form.panel1);  //рисование левого выделения

                form.highlight2.rotate = false;

            }

            else if (!form.IsLeftDouble && (form.current.zn1 == form.current.zn2))

            {

                point2.Y += 20;  //изменение крайней левой координаты по Оу

                form.highlight2.PositionControl(true, point2, form.panel1);  //рисование левого выделения

                form.highlight2.rotate = true;

            }

        }



        // ПОВОРОТ: кость по правой  стороне, поворот вниз

        void pointPNT_30(Point point2)

        {

            if (form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))

            {

                form.current.PositionControl(true, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = true;

                point2.Y += 87;  //изменение крайней левой координаты по Оу

            }

            if (!form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))

            {

                form.current.pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);

                form.current.PositionControl(true, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = true;

                point2.Y += 87;  //изменение крайней левой координаты по Оу

            }

            if (!form.IsLeftDouble && (form.current.zn1 == form.current.zn2))

            {
                //изменение крайней левой координаты по Оу

                point2.Y -= 20;

                form.current.PositionControl(false, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = false;

                //изменение крайней левой координаты

                point2.X += 20;

                point2.Y += 48;

            }

            form.point2 = point2;

            form.turn_left++;

        }



        // ПОВОРОТ: кость по правой стороне

        void pointPNT_33(Point point2)

        {
            if (form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))

            {

                form.current.pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);

                form.current.PositionControl(true, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = true;

                point2.Y += 87;  //изменение крайней левой координаты по Оу

            }

            if (form.IsLeftDouble && (form.current.zn1 == form.current.zn2))

            {

                point2.Y -= 20;  //изменение крайней левой координаты по Оу

                form.current.PositionControl(false, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = false;

                //изменение крайней левой координаты

                point2.X += 20;

                point2.Y += 48;

            }

            if (!form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))

            {

                form.current.pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);

                form.current.PositionControl(true, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = true;

                point2.Y += 87;  //изменение крайней левой координаты по Оу

            }

            form.point2 = point2;

            if (point2.Y >= 300)

                form.turn_left++;

        }



        // ПОВОРОТ:  кость по правой стороне,  поворот обратно

        void pointPNT_31(Point point2)
        {
            if (form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))

            {
                form.current.pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);

                point2.Y -= 20;  //изменение крайней левой координаты по Оу

                form.current.PositionControl(false, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = false;

                //изменение крайней левой координаты

                point2.X += 87;

                point2.Y -= 20;

            }

            else if (!form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))
            {

                point2.Y -= 20;  //изменение крайней левой координаты по Оу

                form.current.PositionControl(false, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = false;

                //изменение крайней левой координаты

                point2.X += 87;

                point2.Y -= 20;

            }

            else if (form.IsLeftDouble && (form.current.zn1 == form.current.zn2))

            {

                form.current.PositionControl(true, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = true;

                //изменение крайней левой координаты

                point2.X += 48;

                point2.Y += 20;

            }

            form.point2 = point2; //новое значение крайнего левого положения

            form.turn_left++; //увеличение кол-ва поворотов

        }

        // ПОВОРОТ: кость по правой стороне, обратно

        void pointPNT_32(Point point2)
        {
            if (form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))
            {
                point2.Y -= 20;  //изменение крайней левой координаты по Оу

                form.current.pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);

                form.current.PositionControl(false, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = false;

                //изменение крайней левой координаты

                point2.Y -= 20;

                point2.X += 87;
            }

            else if (!form.IsLeftDouble && (form.current.zn1 == form.current.zn2))
            {
                form.current.PositionControl(true, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = true;

                //изменение крайней левой координаты по Ох

                point2.X += 48;
            }

            else if (!form.IsLeftDouble && !(form.current.zn1 == form.current.zn2))
            {
                point2.Y -= 20;  //изменение крайней левой координаты по Оу

                form.current.pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);

                form.current.PositionControl(false, point2, form.panel1); //корректировка положения кости

                form.IsLeftDouble = false;

                //изменение крайней левой координаты

                point2.Y -= 20;

                point2.X += 87;
            }

            form.point2 = point2;

        }

    }
}
