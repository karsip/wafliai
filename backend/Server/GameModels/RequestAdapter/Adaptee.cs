using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameModels.RequestAdapter
{
    public class Adaptee
    {
        private byte[] buffer;
        
        public Adaptee(byte[] buffer)
        {
            this.buffer = buffer;
        }
        public int [,] BufferToArray()
        {
            int[,] requestArr = new int[1, 1] { { -1 } };
            string query =  Encoding.ASCII.GetString(buffer);
            if (query.Length > 20)
            {
                int[,] mapArr = StringTo2DArray(query);
                return mapArr;
            }
            else return requestArr;
        }

        public int[,] StringTo2DArray(string query)
        {
            int[] array = query.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            int[,] arrayToReturn = new int[64, 64];
            int counter = 0;
            for (int i = 0; i < Math.Sqrt(array.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(array.Length); j++)
                {
                    arrayToReturn[i, j] = array[counter];
                    counter++;
                }
            }
            return arrayToReturn;
        }
    }
}
