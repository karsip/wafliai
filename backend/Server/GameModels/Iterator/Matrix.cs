using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Iterator
{
    public  class Matrix : IAbstractMatrix
    {

        public List<List<object>> _items = new List<List<object>>();
        public Matrix() { }
        public Matrix(int numberOfRows, int numberOfColumns)
        {
            var cell = new MapCell();
            for (int i = 0; i < numberOfRows; i++)
            {
                List<object> row = new List<object>();
                for (int j = 0; j < numberOfColumns; j++)
                {
                    row.Add(cell);
                }
                _items.Add(row);
            }
        }
        public Iterate CreateIterator() 
        {
            return new Iterate(this);
        }
        public int RowCount
        {
            get { return _items.Count; }
        }
        public int ColumnCount 
        {
            get { return _items[0].Count; }
        }
        public object this[int row, int column] 
        {
            get { return _items[row][column]; }
            set { _items[row][column] = value; }
        }
    }
}
