using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PCBDiploma
{
    //класс вывода информации на эжкран
    public class View : FrameworkElement
    {
        private Logic _search;
        private DrawingVisual _grid, _wall, _ball;
        public int Num { get; set; }
        VisualCollection _visuals;
        private int _x, _y;
        private readonly DispatcherTimer _timer;
        public List<Cell> Cells;
        public Cell Start;
        public Cell Finish;
        public VisualCollection Visuals
        {
            get { return _visuals; }
            set { _visuals = value; }
        }
        
        private bool _flag;//, _whiteflag;

        private List<Cell> LastStartStop;

        public MainWindow Main;

        private int ticks;

       // private string timespan;

        private int traceLength;

        private int notFoundTraces;

        private int izgib;
        public int X { get; set; }
        public int Y { get; set; }

        private List<Cell> ElementsCell;
        public List<int> Criteria;
        public Saving save;
        public int fulltime { get; set; }
        public int fullIzgib { get; set; }


        public View(int X, int Y, List<int> criteria,  List<Cell> points, List<Cell> walls)
        {
            Criteria = new List<int>();
            Criteria=criteria;
          
            Start=points[0];
            Finish=points[1];
      
        
            save = new Saving();
            _search = new Logic();
            ticks = 0;
            traceLength = 0;
            izgib = 0;
            Criteria = new List<int>();
            // timespan = string.Empty;
            _timer = new DispatcherTimer();
            Cells = new List<Cell>();
            notFoundTraces = 0;
            LastStartStop = new List<Cell>();

            _visuals = new VisualCollection(this);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _timer.Tick += timer_Tick;

            ElementsCell = new List<Cell>();
            

        }
        public View()
        {
            save = new Saving();
            _search=new Logic();
            ticks = 0;
            traceLength = 0;
            izgib = 0;
            Criteria = new List<int>();
           // timespan = string.Empty;
            _timer = new DispatcherTimer();
            Cells = new List<Cell>();
            notFoundTraces = 0; 
            LastStartStop = new List<Cell>();
            
            _visuals = new VisualCollection(this);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _timer.Tick += timer_Tick;

            ElementsCell = new List<Cell>();
        }

        public void BulkImport(List<StartFinish> startFinishList)
        {
           
            
            Main.timespan.Content = string.Empty;
            Main.traceLength.Content = string.Empty;
            Main.notFoundtraces.Content = string.Empty;
            Main.lblfulltime.Content = string.Empty;
            Main.izgibLabel.Content = string.Empty;

            foreach (var item in startFinishList)
            {
                var start = GetCellFromPoint(item.Start);
                var finish = GetCellFromPoint(item.Finish);

                if (Cells.Contains(start) && !LastStartStop.Contains(start)) continue;
                if (Cells.Contains(finish) && !LastStartStop.Contains(finish)) continue;

                DrawStartFinish(item.Start, Brushes.WhiteSmoke);
                LastStartStop.Add(start);
                DrawStartFinish(item.Finish, Brushes.WhiteSmoke);
                LastStartStop.Add(finish);

                _search = new Logic(Cells, start, finish, X,Y);
                var result = _search.Search(Main._check, out izgib);

                if (result)
                {
                    Cells.AddRange(new List<Cell> { start, finish });
                    LastStartStop.AddRange(new List<Cell> { Start, Finish });

                    foreach (var trace in _search.FinalPath)
                    {
                        DrawRect(new Point(trace.X, trace.Y), Brushes.WhiteSmoke);
                    }

                    foreach (Cell cell in _search.LastTrace)
                    {
                        var neighbours = GetNeighbours(cell);
                        foreach (Cell neighbour in neighbours)
                        {
                            if (!Cells.Contains(neighbour))
                            {
                                Cells.Add(neighbour);
                            }
                        }
                    }

                    fulltime += (int)_search.StopWatch.ElapsedTicks;
                    fullIzgib += izgib;
                    traceLength += _search.LastTrace.Count;

                  
                    
                    
                }
                else
                {
                    if (!LastStartStop.Contains(start)) RemoveBall(start);
                    if (!LastStartStop.Contains(finish)) RemoveBall(finish);

                    notFoundTraces++;
                   // Main.lblfulltime.Content = "";
                  //  Main.izgibLabel.Content = "";
                    Main.notFoundtraces.Content = string.Format("Не найдено решений: {0}", notFoundTraces);
                    
                }
            }
                int traceTime = Convert.ToInt32(_search.StopWatch.ElapsedTicks);
                fulltime += fulltime;
              fullIzgib += Convert.ToInt32(izgib);
                traceLength += _search.LastTrace.Count;
                Main.izgibLabel.Content += izgib.ToString();
               // Main.timespan.Content = string.Format("Решение найдено за {0} тактов",traceTime);
                Main.izgibLabel.Content = string.Format("Количество изгибов: {0}", fullIzgib);
              //  Main.lblfulltime.Content = string.Format("Общее время: {0}", fulltime);


                Main.timespan.Content = string.Format("Решение найдено за {0}", fulltime);
            Main.traceLength.Content = string.Format("Длинна проводников: {0}", traceLength - _search.LastTrace.Count);

        }
        public Cell DrawStartFinish(Point point, Brush brush, bool addToCells = true)
        {
            XyCoords(point, out _x, out _y);
            _ball = new DrawingVisual();
            using (DrawingContext dc = _ball.RenderOpen())
            {
                int sizeCell = 20 / 2, valueOfZoomBool = 9, sizeRect = 20;
                dc.DrawRectangle(Brushes.Gray, new Pen(Brushes.Gray, 0.1), new Rect(new Point(_x, _y), new Point(_x + sizeRect, _y + sizeRect)));
                dc.DrawEllipse(Brushes.WhiteSmoke, new Pen(brush, 0.1), new Point(_x + sizeCell, _y + sizeCell), valueOfZoomBool, valueOfZoomBool);

            }
            _grid.Children.Add(_ball);

            var cell = new Cell(_x, _y);

            if (addToCells) Cells.Add(cell);

            return cell;
        }
        public void StartStop(int check)
        {
            Main.timespan.Content = string.Empty;

            if (_timer.IsEnabled == false && Start != null && Finish != null)
            {
                _timer.Start();
            }

            if (Start != null && Finish != null)
            {
                _search = new Logic(Cells, Start, Finish, X,Y);
                save.Points.Add(Start);
                save.Points.Add(Finish);
              _search.Search(check, out izgib);

              //  Main.izgibLabel.Content += izgib.ToString();
               
               

            }
        }
        //очистка экрана
        public void Clear()
        {

            _timer.Stop();
            Cells.Clear();
            fullIzgib = 0;
            fulltime = 0;
            this.ElementsCell.Clear();
            LastStartStop.Clear();
            _grid.Children.Clear();
            Start = null;
            Finish = null;
            notFoundTraces = 0;
            traceLength = 0;
            Main.notFoundtraces.Content = string.Empty;
            Main.timespan.Content = string.Empty;
            Main.traceLength.Content = string.Empty;

        }

        void timer_Tick(object sender, EventArgs e)
        {
            ticks++;
            int tickval = 200;
            if (ticks > tickval)
            {
                ticks = 0;
                _timer.Stop();

                if (!LastStartStop.Contains(Start))RemoveBall(Start);
                if (!LastStartStop.Contains(Finish))RemoveBall(Finish);

                Start = null;
                Finish = null;
                Main.izgibLabel.Content = "";
                Main.lblfulltime.Content = "";
                Main.traceLength.Content = "";
                notFoundTraces++;
                Main.notFoundtraces.Content = string.Format("Не найдено решений: {0}", notFoundTraces);
                MessageBox.Show("Путь не найден!");
              
            }
            if (_search.FinalPath.Count != 0)
            {
                var cell = _search.FinalPath.Pop();
                save.Walls.AddRange(_search.FinalPath);
                DrawRect(new Point(cell.X, cell.Y), Brushes.WhiteSmoke);

                if (cell.Equals(_search.LastTrace[2])) // start, finish, then last trace => index == 2
                {
                    _timer.Stop();
                    ticks = 0;
                }
            }

            if (_search.HasReachedFinish)
            {
                Cells.AddRange(new List<Cell> { Start, Finish });
                LastStartStop.AddRange(new List<Cell> { Start, Finish });
               
                int traceTime = Convert.ToInt32(_search.StopWatch.ElapsedTicks);
                fulltime+=traceTime;
                int traceLengthL;
                traceLength -= LastStartStop.Count;
                
                fullIzgib += Convert.ToInt32(izgib);
                traceLength += _search.LastTrace.Count;
                traceLengthL = traceLength;
                Main.izgibLabel.Content += izgib.ToString();
                Main.timespan.Content = string.Format("Решение найдено за {0} тактов",traceTime);
                Main.traceLength.Content = string.Format("Длинна проводников: {0}",traceLengthL);
                Main.izgibLabel.Content = string.Format("Количество изгибов: {0} из {1}", izgib, fullIzgib);
                Main.lblfulltime.Content = string.Format("Общее время: {0}", fulltime);
       //////////////////////////////////////////////////////////////////////////////////////////////////////SERRRRRRRRRRRRRRRRRRRRRRRRrr                    
           
            save.Criteria.Add(traceTime);
            save.Criteria.Add(traceLengthL);
            save.Criteria.Add(izgib);

                Start = null;
                Finish = null;
                _search.HasReachedFinish = false;

                foreach (Cell cell in _search.LastTrace)
                {
                    var neighbours = GetNeighbours(cell);
                    foreach (Cell neighbour in neighbours)
                    {
                        if (!Cells.Contains(neighbour))
                        {
                            Cells.Add(neighbour);
                        }
                    }
                }
            }


        }

        private List<Cell> GetNeighbours(Cell cell)
        {
            int sizeCell = 20;
            var neibrCells = new List<Cell>();
            neibrCells.Add(new Cell(cell.X - sizeCell, cell.Y));
            neibrCells.Add(new Cell(cell.X + sizeCell, cell.Y));
            neibrCells.Add(new Cell(cell.X, cell.Y - sizeCell));
            neibrCells.Add(new Cell(cell.X, cell.Y + sizeCell));

            neibrCells.Add(new Cell(cell.X - sizeCell, cell.Y - sizeCell));
            neibrCells.Add(new Cell(cell.X + sizeCell, cell.Y + sizeCell));
            neibrCells.Add(new Cell(cell.X + sizeCell, cell.Y - sizeCell));
            neibrCells.Add(new Cell(cell.X - sizeCell, cell.Y + sizeCell));

            return neibrCells;
        }
        //отрисовка шаров
        public Cell DrawBall(Point point, Brush brush, bool addToCells = true)
        {
            XyCoords(point, out _x, out _y);
            _ball = new DrawingVisual();
            using (DrawingContext dc = _ball.RenderOpen())
            {
                int sizeCell = 20/2, valueOfZoomBool=9;

                dc.DrawEllipse(brush, new Pen(Brushes.LimeGreen, 0.1), new Point(_x + sizeCell, _y + sizeCell), valueOfZoomBool, valueOfZoomBool);

            }
            _grid.Children.Add(_ball);

            var cell = new Cell(_x, _y);

            if (addToCells) Cells.Add(cell);

            return cell;
        }
        public void DrawRect(Point point, Brush brushes)
        {
            XyCoords(point, out _x, out _y);
            _wall = new DrawingVisual();

            using (DrawingContext dc = _wall.RenderOpen())
            {
                int sizeCell = 20;
                dc.DrawRectangle(brushes, new Pen(brushes, 0.1), new Rect(new Point(_x, _y), new Point(_x + sizeCell, _y + sizeCell)));
            }
            //что бы не было повторения координат

            _grid.Children.Add(_wall);

            if (!Cells.Contains(new Cell(_x, _y)))
            {
                Cells.Add(new Cell(_x, _y, true));
            }
        }
        //закраска ошибочных стенок
        public void RemoveBall(Cell cell)
        {
            if (cell == null)
            {
                return;
            }

            _wall = new DrawingVisual();
            using (DrawingContext dc = _wall.RenderOpen())
            {
                int sizeCell = 20;
                dc.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 0.4), new Rect(new Point(cell.X, cell.Y), new Point(cell.X + sizeCell, cell.Y + sizeCell)));
            }
            //проверка на существование стенки 
            if (Cells.Contains(cell))
            {
                _grid.Children.Add(_wall);
                Cells.Remove(cell);
            }
        }
       
        public void RemoveBal(Cell cel)
        {
           
            if (cel == null)
            {
                return;
            }
           List<Cell> cell1= GetNeighbours(cel);
            _wall = new DrawingVisual();
            using (DrawingContext dc = _wall.RenderOpen())
            {
                int sizeCell = 20;
                foreach (var cell in cell1)
                {
                    dc.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 1), new Rect(new Point(cell.X, cell.Y), new Point(cell.X + sizeCell, cell.Y + sizeCell)));
                    //проверка на существование стенки 
                    if (Cells.Contains(cell))
                    {
                        Cells.Remove(cell);
                    }
                }
            }
           
           
        }
        //приведение полученных координат в нужный вид
        public void XyCoords(Point point, out int x, out int y)
        {
            // округление до 10 координат
            x = (int)Math.Round(point.X / 10);
            if (x % 2 != 0)
                x = (x - 1) * 10;
            else if (x * 10 > point.X)
                x = (x - 2) * 10;
            else
                x = x * 10;
            y = (int)Math.Round(point.Y / 10);
            if (y % 2 != 0)
                y = (y - 1) * 10;
            else if (y * 10 > point.Y)
                y = (y - 2) * 10;
            else
                y = y * 10;
        }

        public Cell GetCellFromPoint(Point point)
        {
            int x, y;
            XyCoords(point, out x, out y);

            return new Cell(x, y);
        }

        ////отрисовка грида и задней стенки
        //public void DrawGrid()
        //{
        //    _grid = new DrawingVisual();
        //    using (DrawingContext dc = _grid.RenderOpen())
        //    {
        //        dc.DrawRectangle(Brushes.DarkGreen, new Pen(Brushes.DarkGreen, 0), new Rect(new Point(0, 0), new Point(2000, 2000)));
        //    }
        //    _visuals.Add(_grid);

        public void DrawGrid()
        {
           int[] qwe = {X, Y};
            int sizeCell = 20;
        
            _grid = new DrawingVisual();

            using (DrawingContext dc = _grid.RenderOpen())
            {
                int sizeX = qwe[0], sizeY = qwe[1];
                dc.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 0), new Rect(new Point(0, 0), new Point(sizeX, sizeY)));
                for (int i = 0; i <= sizeX/sizeCell; i++)
                {
                    dc.DrawLine(new Pen(Brushes.DimGray, 0.5), new Point(_x, 0), new Point(_x, sizeY));
                    _x += sizeCell;
                }
                for (int i = 0; i <= sizeY/sizeCell; i++)
                {
                    
                    dc.DrawLine(new Pen(Brushes.DimGray, 0.5), new Point(0, _y), new Point(sizeX, _y));
                    _y += sizeCell;
                }
            }
            _visuals.Add(_grid);
            
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (Start != null && Finish != null) return;

            XyCoords(e.GetPosition(this), out _x, out _y);
            var cell = new Cell(_x, _y);

            if (!Cells.Contains(cell) || LastStartStop.Contains(cell))
            {
                if (Start == null)
                {
                   // Start = DrawBall(e.GetPosition(this), Brushes.White);
                    Start = DrawStartFinish(e.GetPosition(this), Brushes.Black);
                }
                else
                {
                    Finish = DrawStartFinish(e.GetPosition(this), Brushes.Black);
                }


                _flag = true;
            }
        }

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);

        //  //  if(_flag && Num > 2)
        //      //  DrawWall(e.GetPosition(this));
        //  //  if(_whiteflag && Num > 2)
        //      //  DrawWhitewall(e.GetPosition(this));
        //}
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            _flag = false;
        }

    
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
           
            //    base.OnMouseRightButtonDown(e);
            //    DrawWhitewall(e.GetPosition(this));
            //    _whiteflag = true;
            base.OnMouseRightButtonDown(e);
            if (Start == null)
            {
                return;
            }

                if (!(ElementsCell.Contains(Start)) && Start != null)
                    ElementsCell.Add(Start);
                if (!(ElementsCell.Contains(Finish)) && Finish != null)
                    ElementsCell.Add(Finish);
            
          //  if (Start != null && Finish != null && ) return;
            XyCoords(e.GetPosition(this), out _x, out _y);
            var cell = new Cell(_x, _y);

            var neighbours = _search.GetListNeighbours(ElementsCell);

            if (Start != null)
            {
                foreach (var ners in neighbours)
                {
                    foreach (var el in ElementsCell)
                    {
                        if (ners.X == cell.X && ners.Y == cell.Y)
                        {
                            ElementsCell.Add(ners);
                            DrawRect(e.GetPosition(this), Brushes.Gray);

                            return;
                        }
                    }
                }
            }
            if (Finish != null)
            {
                foreach (var ners in neighbours)
                {
                    foreach (var el in ElementsCell)
                    {
                        if (ners.X == cell.X && ners.Y == cell.Y)
                        {
                            ElementsCell.Add(ners);
                            DrawRect(e.GetPosition(this), Brushes.Gray);

                            return;
                        }
                    }
                }
            }

            //if (!Cells.Contains(cell) || LastStartStop.Contains(cell))
            //{
            //    if (Start == null)
            //    {
            //        Start = DrawBall(e.GetPosition(this), Brushes.White);
            //    }
            //    else
            //    {
            //        Finish = DrawBall(e.GetPosition(this), Brushes.White);
            //    }


              //  _flag = true;
            }
        

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
           // _flag = false;
        }
        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }
    }
}
