using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Net;
using System.Collections.Specialized;
using System.IO;

namespace TwitchGameBot
{
    class Labirint
    {
        Random rand = new Random();
        private int _Width, _Height;
        private Cell[,] Cells;
        private int targ;
        public Labirint(int w,int h,int targ)
        {
            _Width = w;
            _Height = h;
            this.targ = targ;
        }
   

        public void PathInitialized()
        {

            Random rand = new Random();
            Cells = new Cell[_Width, _Height];
            for (int y = 0; y < _Height; y++)
                for (int x = 0; x < _Width; x++)
                    Cells[x, y] = new Cell(new Point(x, y));
            
            int count = 0;
            int startX = 0;
            int startY = 0;
            Stack<Cell> path = new Stack<Cell>();
            Cells[startX, startY].Visited = true;
            path.Push(Cells[startX, startY]);
            while (path.Count > 0)
            {
                
                Cell _cell = path.Peek();
                if (count == targ)
                {
                    _cell.isTarg = true;
                }

                List<Cell> nextStep = new List<Cell>();
                
                int a = 0;

                if (_cell.Position.X > 0)
                {
                    if (!Cells[_cell.Position.X - 1, _cell.Position.Y].Visited)
                        a++;

                    if (_cell.Position.X - 2 < 0) a++;
                    else
                        if (!Cells[_cell.Position.X - 2, _cell.Position.Y].Visited)
                        a++;
                    if (_cell.Position.Y - 1 < 0) a++;
                    else
                        if (!Cells[_cell.Position.X - 1, _cell.Position.Y - 1].Visited)
                        a++;

                    if (_cell.Position.Y + 1 == _Height) a++;
                    else
                        if (!Cells[_cell.Position.X - 1, _cell.Position.Y + 1].Visited)
                        a++;

                    if (a == 4) nextStep.Add(Cells[_cell.Position.X - 1, _cell.Position.Y]);
                }
                a = 0;
                if (_cell.Position.X < _Width - 1)
                {

                    if (!Cells[_cell.Position.X + 1, _cell.Position.Y].Visited)
                        a++;

                    if (_cell.Position.X + 2 == _Width) a++;
                    else
                        if (!Cells[_cell.Position.X + 2, _cell.Position.Y].Visited)
                        a++;

                    if (_cell.Position.Y - 1 < 0) a++;
                    else
                        if (!Cells[_cell.Position.X + 1, _cell.Position.Y - 1].Visited)
                        a++;

                    if (_cell.Position.Y + 1 == _Height) a++;
                    else
                        if (!Cells[_cell.Position.X + 1, _cell.Position.Y + 1].Visited)
                        a++;
                    if (a == 4) nextStep.Add(Cells[_cell.Position.X + 1, _cell.Position.Y]);
                }
                a = 0;
                if (_cell.Position.Y > 0)
                {

                    if (!Cells[_cell.Position.X, _cell.Position.Y - 1].Visited)
                        a++;


                    if (_cell.Position.Y - 2 < 0) a++;
                    else
                        if (!Cells[_cell.Position.X, _cell.Position.Y - 2].Visited)
                        a++;

                    if (_cell.Position.X - 1 < 0) a++;
                    else
                        if (!Cells[_cell.Position.X - 1, _cell.Position.Y - 1].Visited)
                        a++;

                    if (_cell.Position.X + 1 == _Width) a++;
                    else
                        if (!Cells[_cell.Position.X + 1, _cell.Position.Y - 1].Visited)
                        a++;

                    if (a == 4) nextStep.Add(Cells[_cell.Position.X, _cell.Position.Y - 1]);
                }
                a = 0;
                if (_cell.Position.Y < _Height - 1)
                {
                    if (!Cells[_cell.Position.X, _cell.Position.Y + 1].Visited)
                        a++;

                    if (_cell.Position.Y + 2 == _Height) a++;
                    else
                        if (!Cells[_cell.Position.X, _cell.Position.Y + 2].Visited)
                        a++;

                    if (_cell.Position.X + 1 == _Width) a++;
                    else
                        if (!Cells[_cell.Position.X + 1, _cell.Position.Y + 1].Visited)
                        a++;

                    if (_cell.Position.X - 1 < 0) a++;
                    else
                        if (!Cells[_cell.Position.X - 1, _cell.Position.Y + 1].Visited)
                        a++;




                    if (a == 4) nextStep.Add(Cells[_cell.Position.X, _cell.Position.Y + 1]);
                }
                 

                if (nextStep.Count() > 0)
                {

                    Cell next = nextStep[rand.Next(nextStep.Count())];                   
                    next.Visited = true;
                    count++;
                    path.Push(next);
                }
                else
                {
                    path.Pop();
                }
            }
            
        }

        public string PathToImg()
        {
           
            Bitmap bmp1 = new Bitmap(650, 750, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(bmp1))
            {
                g.Clear(Color.White);
                
                for (int y = 0; y < _Height + 2; y++)
                {
                    
                    for (int x = 0; x < _Width + 2; x++)
                    {

                        if (x == 1 && y==0)
                        {
                            g.FillRectangle(Brushes.White, new Rectangle(x * 50, y * 50, 50, 50));
                            continue;
                        }

                        if (y == 0 || x == 0 || y == _Height + 1 || x == _Width + 1)
                        {
                            g.FillRectangle(Brushes.Black, new Rectangle(x * 50, y * 50, 50, 50));
                            continue;
                        }
                        

                        if (Cells[x - 1, y - 1].isTarg)
                        {
                            g.FillRectangle(Brushes.White, new Rectangle(x * 50, y * 50, 50, 50));
                            g.FillRectangle(Brushes.Red, new Rectangle(x * 50 + 10, y * 50 + 10, 30, 30));
                            continue;
                        }


                        if (Cells[x - 1, y - 1].Visited == true)
                        {
                            g.FillRectangle(Brushes.White, new Rectangle(x * 50, y * 50, 50, 50));
                            continue;
                        }
                        else
                        {
                            g.FillRectangle(Brushes.Black, new Rectangle(x * 50, y * 50, 50, 50));
                            continue;
                        }

                    }
                    
                }
                for (int y = 0; y < _Height + 2; y++)
                {
                    for (int x = 0; x < _Width + 2; x++)
                    {
                        g.DrawLine(new Pen(Brushes.Black, 1), x * 50, y * 50, x * 50+50, y * 50);
                        g.DrawLine(new Pen(Brushes.Black, 1), x * 50, y * 50, x * 50, y * 50+50);
                        g.DrawLine(new Pen(Brushes.Black, 1), x * 50, y * 50+50, x * 50+50, y * 50+50);
                        g.DrawLine(new Pen(Brushes.Black, 1), x * 50+50, y * 50, x * 50+50, y * 50+50);


                    }

                }
            }

          
            bmp1.Save("lab.png", ImageFormat.Png);
            return sendImg("lab.png");

            
                
        }

        public string sendImg(string path)
        {
            string url = "https://api.imgbb.com/1/upload";
            using (var webClient = new WebClient())
            {
                var pars = new NameValueCollection();
                pars.Add("key", Setting.ibbApiKey);
                pars.Add("image", ImgToString(path));
                pars.Add("expiration", "10000");
                var response = webClient.UploadValues(url, pars);
                string str = Encoding.UTF8.GetString(response);
                int a = str.IndexOf("ibb.co") + 8;
                string res = "";
                char b = str[a];
                while (b != '"')
                {
                    res += b;
                    a++;
                    b = str[a];

                }

                return "https://ibb.co/" + res;

            }
        }

        public string ImgToString(string Path)
        {
            using (Image image = Image.FromFile(Path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }


    }

    class Cell
    {
        public Cell(Point currentPosition)
        {
            Visited = false;
            Position = currentPosition;
        }
        public Boolean Visited { get; set; }
        public Point Position { get; set; }
        public bool isTarg { get; set; }

    }

    class AppleAtPath
    {
        public Stack<Cell> path = new Stack<Cell>();
        public int count =0;

    }


}
