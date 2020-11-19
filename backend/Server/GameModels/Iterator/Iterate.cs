using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Iterator
{
    public class Iterate: IAbstractIterate
    {
        private Matrix _matrix;
        private int[] _current = {0, 0};
        private int _step = 1;
        public Iterate(Matrix matrix) 
        {
            this._matrix = matrix;
        }
        public object First() 
        {
            _current[0] = 0;
            _current[1] = 0;
            return _matrix[_current[0],_current[1]];
        }
        public object CurrentItem 
        {
            get { return _matrix[_current[0], _current[1]]; }
            set { _matrix[_current[0], _current[1]] = value; }
        }
        public object Next() 
        {
            if (_current[1] < _matrix.ColumnCount-_step)
                _current[1] += _step;
            else 
            {
                _current[0] += _step;
                _current[1] = 0;
            }
            if (!IsDone)
            {
                return _matrix[_current[0], _current[1]] as object;
            }
            else
                return null;
        }
        public int Row 
        {
            get { return _current[0]; }
        }
        public bool IsDone
        {
            get{ return !(_current[0] < _matrix.RowCount && _current[1] < _matrix.ColumnCount); }
        }
        public int Step
        {
            get { return _step; }
            set { _step = value; }
        }
    }
}
