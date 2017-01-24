using System;

namespace PCBDiploma
{
    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Gprice { get; set; }
        public int Fprice { get; set; }
        public int Hprice { get; set; }
        public Cell Parent { get; set; }
        public bool Blocked { get; set; }

        public Cell()
        {
        }

        public Cell(int x, int y , bool blocked)
        {
            Parent = null;
            X = x;
            Blocked = blocked;
            Y = y;
            Gprice = 0;
        }

        public Cell(int x, int y)
        {
            Parent = null;
            X = x;
            Blocked = false;
            Y = y;
            Gprice = 0;
        }
        //Эвристическая оценка
        public int HPrice(Cell finalcell, int herouisticmode)
        {
            int xDist = Math.Abs(X - finalcell.X);
            int yDist = Math.Abs(Y - finalcell.Y);
            switch (herouisticmode)
            {
                   
                case 1://быстрое манхеттонское
                    return  (xDist + yDist);
                case 2://медленное, но точное манхеттонское
                    return (int)Math.Sqrt(xDist*xDist + yDist*yDist);
                    
                   // return (xDist + yDist) / 5;
                case 3://диагональный расчет
                    {
                    
                    if (xDist > yDist)
                        return (int)(1.4*yDist) + (xDist - yDist);
                    return (int)(1.4*xDist) + (yDist - xDist);
                    }
            }
            return 0;

        }
        //Стоимость пути(по прямой 10, диагонали 14)
        public int GPrice(Cell secondcell)
        {
            if (X == secondcell.X || Y == secondcell.Y)
                return 10;
            return 14;
        }
        public Cell Clone()
        {
            return new Cell(X, Y); 
        }
        public override bool Equals(object obj)
        {
            var cell = obj as Cell;
            if(cell != null)
                return (X == cell.X && Y == cell.Y);
            return false;
        }
    }
}
