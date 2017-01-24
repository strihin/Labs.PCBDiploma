using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace PCBDiploma
{
    class Logic
    {

        private double ClientHeight;
        private double ClientWidth;

        private List<Cell> _openCell, _closeCell, _neibrCells;
        public Stack<Cell> FinalPath;
        private readonly Cell _startCell, _finalCell;
        private Cell _currentCell;
        public List<Cell> LastTrace;
        public Stopwatch StopWatch;

        private int previouslyOpenedByWave;
        private bool breakWaving;

        public bool HasReachedFinish { get; set; }

        private List<Cell> listNeighbours = new List<Cell>();

        public Logic() { }

        public Logic(List<Cell> cells, Cell start, Cell finish, double height, double width)
        {
            ClientHeight = height;
            ClientWidth = width;
            LastTrace = new List<Cell>() { start, finish };
            _openCell = new List<Cell>();
            _closeCell = new List<Cell>();
            _neibrCells = new List<Cell>();
            FinalPath = new Stack<Cell>();
            _startCell = start;
            _finalCell = finish;
            HasReachedFinish = false;

            previouslyOpenedByWave = 0;
            breakWaving = false;

            StopWatch = new Stopwatch();

            _closeCell.AddRange(cells);
            _closeCell.Remove(start);

            _closeCell.Remove(finish);

            foreach (var cell in GetNeighbours(_startCell))
            {
                _closeCell.Remove(cell);
            }
            foreach (var cell in GetNeighbours(_finalCell))
            {
                _closeCell.Remove(cell);
            }
           // listNeighbours = new List<Cell>();
        }

        public bool Search(int check, out int izgib)
        {
            if (check == 4)
            {
                WaveSearching();
            }
            else if (check == 5)
            {
                RaySearching();
            }
            else
            {
                Searching(check);
            }

            izgib = 0;

            if (HasReachedFinish)
            {
                Cell[] array = FinalPath.ToArray();
                for (int i = 1; i < array.Length - 1; i++)
                {
                    if (array[i - 1].X != array[i + 1].X && array[i - 1].Y != array[i + 1].Y)
                    {
                        izgib++;
                    }
                }
            }

            return HasReachedFinish;
        }

        //метод реализующий поиск пути
        public void Searching(int check)
        {
            StopWatch = Stopwatch.StartNew();

            _startCell.Gprice = 0;
            _startCell.Hprice = _startCell.HPrice(_finalCell, check);
            _startCell.Fprice = _startCell.Hprice;
            _startCell.Parent = _startCell;
            _openCell.Add(_startCell);

            var previousCell = new Cell();

            while (_openCell != null)
            {
                LowestFPriceCell();

                if (previousCell.Equals(_currentCell))
                {
                    return;
                }

                if (_currentCell.Equals(_finalCell))
                {
                    _finalCell.Parent = _currentCell;
                    Finalpath();
                    break;
                }
                _openCell.Remove(_currentCell);


                AddNeighbours();
                foreach (var neibrHood in _neibrCells)
                {
                    if (_closeCell.Contains(neibrHood) || neibrHood.Blocked || neibrHood.X < 0 || neibrHood.Y < 0 || neibrHood.X > ClientWidth || neibrHood.Y > ClientHeight)
                        continue;
                    int g = _currentCell.Gprice + neibrHood.GPrice(_currentCell);

                    bool flag = false;
                    if (!_openCell.Contains(neibrHood))
                    {
                        _openCell.Add(neibrHood);
                        flag = true;
                    }
                    else if (g < neibrHood.GPrice(_currentCell.Parent) + _currentCell.Gprice)
                        flag = true;
                    if (flag)
                    {
                        neibrHood.Parent = _currentCell;
                        neibrHood.Gprice = g;
                        neibrHood.Hprice = neibrHood.HPrice(_finalCell, check);
                        neibrHood.Fprice = neibrHood.Gprice + neibrHood.Hprice;
                    }
                }
                _closeCell.Add(_currentCell);
                previousCell = _currentCell.Clone();
            }

            StopWatch.Stop();
        }

        //метод реализующий волновой поиск пути
        public void WaveSearching()
        {
            StopWatch = Stopwatch.StartNew();
            
            _openCell.Add(_startCell);

            PerformWave(GetNeighbours(_startCell));

            if (!breakWaving)
            {
                WaveFinalpath();  
            } 

            StopWatch.Stop();
        }

        private void PerformWave(List<Cell> neighbours, int step = 0)
        {
            step++;

            var nextStepCells = new List<Cell>();

            foreach (var neibrHood in neighbours)
            {
                if (neibrHood.Equals(_finalCell))
                {
                    _finalCell.Gprice = step;
                    return;
                }

                if (_openCell.Contains(neibrHood) || _closeCell.Contains(neibrHood) || neibrHood.Blocked || neibrHood.X < 0 || neibrHood.Y < 0 || neibrHood.X > ClientWidth || neibrHood.Y > ClientHeight)
                    continue;

                neibrHood.Gprice = step;

                _openCell.Add(neibrHood);

                nextStepCells.AddRange(GetNeighbours(neibrHood));
            }

            if (_openCell.Count == previouslyOpenedByWave)
            {
                breakWaving = true;
                return;
            }

            previouslyOpenedByWave = _openCell.Count;

            PerformWave(nextStepCells, step);
        }

        //метод реализующий лучевой поиск пути
        public void RaySearching()
        {
            StopWatch = Stopwatch.StartNew();

            _openCell.Add(_startCell);

            while (_openCell != null)
            {
                LowestHPriceCell();

                if (_currentCell.Equals(_finalCell))
                {
                    _finalCell.Parent = _currentCell;
                    Finalpath();
                    break;
                }
                _openCell.Remove(_currentCell);

                int minDistance = Distance(_currentCell, _finalCell);

                Cell newCell = _currentCell;

                AddNeighbours();
                foreach (var neibrHood in _neibrCells)
                {
                    if (_openCell.Contains(neibrHood) || _closeCell.Contains(neibrHood) || neibrHood.Blocked || neibrHood.X < 0 || neibrHood.Y < 0 || neibrHood.X > ClientWidth || neibrHood.Y > ClientHeight)
                        continue;

                    var dist = Distance(neibrHood, _finalCell);

                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        newCell = neibrHood;
                    }
                }

                if (newCell.Equals(_currentCell))
                {
                    break;
                }

                newCell.Parent = _currentCell;
                _openCell.Add(newCell);
            }

            StopWatch.Stop();
        }

        //запись окончательного пути в массив
        private void Finalpath()
        {
            var temp = _finalCell.Parent;

            while (temp != _startCell)
            {
                temp = temp.Parent;
                if (temp == _startCell)
                    break;
                if (!FinalPath.Contains(temp))
                {
                    FinalPath.Push(temp);
                    LastTrace.Add(temp);
                }
            }

            HasReachedFinish = true;
        }

        private void WaveFinalpath()
        {
            _currentCell = _finalCell;

            while (_currentCell != _startCell)
            {
                AddNeighbours();

                foreach (var neighbour in _neibrCells)
                {
                    var openedNeighbour = _openCell.Find(cell => cell.X == neighbour.X && cell.Y == neighbour.Y);

                    if (openedNeighbour != null && openedNeighbour.Gprice < _currentCell.Gprice)
                    {
                        _currentCell = openedNeighbour;
                    }
                }

                if (_currentCell == _startCell)
                    break;

                if (!FinalPath.Contains(_currentCell))
                {
                    FinalPath.Push(_currentCell);
                    LastTrace.Add(_currentCell);
                }
            }

            HasReachedFinish = true;
        }
        //нахождение наименьшего F(для установки его родителем)
        private void LowestFPriceCell()
        {
            int F = 10000;
            foreach (var cell in _openCell)
            {
                if (F > cell.Fprice)
                {
                    _currentCell = cell;
                    F = cell.Fprice;
                }
            }
        }

        private void LowestHPriceCell()
        {
            var min = 10000;

            foreach (var cell in _openCell)
            {
                if (Distance(cell, _finalCell) < min)
                {
                    _currentCell = cell;
                    min = cell.Fprice;
                }
            }
        }

        //добавление соседей точки
        private void AddNeighbours()
        {
            _neibrCells.Clear();
            _neibrCells.AddRange(GetNeighbours());
        }
#region boroda double list
        public List<Cell> GetListNeighbours(List<Cell> list)
        {
            if (list.Count == 0)
                return list;
            foreach (var current in list)
            {
               listNeighbours.AddRange(GetNeighbours(current));
            }

  
            listNeighbours=listNeighbours.Distinct().ToList();

            return listNeighbours;
        }
        public List<Cell> RemoveDoubles(List<Cell> list)
        {
            HashSet<Cell> set = new HashSet<Cell>(list);
            if (list.Count != 0)
            {
                foreach (var value in list)
                    set.Add(value);
            }
            else
            {
                return list;
            }
            return set.ToList();
        }

#endregion
        public List<Cell> GetNeighbours(Cell cell = null)
        {
            int sizeCell = 20;
            var temp = cell ?? _currentCell;
            ////////////////////////////////////////////////////////////////////////////////////////////
            var neighbours = new List<Cell>();
            //if (temp.X + sizeCell >= ClientWidth || temp.Y - sizeCell < 0 || temp.Y + sizeCell >= ClientHeight ||
            //    temp.Y + sizeCell < 0)
            //{
            //    return neighbours;
            //}
            neighbours.Add(new Cell(temp.X + sizeCell, temp.Y));
            neighbours.Add(new Cell(temp.X, temp.Y - sizeCell));
            neighbours.Add(new Cell(temp.X - sizeCell, temp.Y));
            neighbours.Add(new Cell(temp.X, temp.Y + sizeCell));

            return neighbours;
        }

        private int Distance(Cell cell1, Cell cell2)
        {
            return Math.Abs(cell1.X - cell2.X) + Math.Abs(cell1.Y - cell2.Y);
        }

    }

}
