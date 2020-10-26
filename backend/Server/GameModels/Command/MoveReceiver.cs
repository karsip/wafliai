using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Command
{
    public class MoveReceiver
    {
        private int[,] map;
        private int object_id;
        private int x_pos;
        private int y_pos;
        private int[,] selectedArea;

        public MoveReceiver(int [,] map, int object_id, int x_pos, int y_pos, int[,] selectedArea)
        {
            this.map = map;
            this.object_id = object_id;
            this.x_pos = x_pos;
            this.y_pos = y_pos;
            this.selectedArea = selectedArea;
        }
        public int [,] Undo()
        {
            int[] objectSize = GetAreaSize(object_id);
            for (int i = 0; i < objectSize[1]; i++)
            {
                for (int j = 0; j < objectSize[0]; j++)
                {
                    // clear the updated object place 
                    map[x_pos + i, y_pos + j] = 0;
                }
            }
            for (int i = 0; i < selectedArea.GetLength(0); i++)
            {
                map[selectedArea[i, 0], selectedArea[i, 1]] = object_id;
            }
            return map;
        }
        public int [,] Move()
        {
            try
            {
                ClearNowEmptySpace(map, selectedArea);
                writeToMap(map, object_id, x_pos, y_pos);  
            }
            catch (Exception e)
            {
                String errrMessage = String.Format("Object wchich id is {0} cannot move to position x:{1} and y:{2}. Error occurred -> {3}", object_id, x_pos, y_pos, e.Message);
                Console.WriteLine(errrMessage);
            }
            return map;
        }
        private void ClearNowEmptySpace(int[,] map, int[,] selectedArea)
        {
            for (int i = 0; i < selectedArea.GetLength(0); i++)
            {
                map[selectedArea[i, 0], selectedArea[i, 1]] = 0;
            }
        }
        private void writeToMap(int[,] map, int object_id, int x_pos, int y_pos)
        {
            int[] areaSize = GetAreaSize(object_id);
            for (int k = 0; k < areaSize[0]; k++)
            {
                map[x_pos + k, y_pos] = object_id;
            }
            for (int m = 0; m < areaSize[0]; m++)
            {
                map[x_pos, y_pos + m] = object_id;
            }
        }
        private int[] GetAreaSize(int object_id)
        {
            int[] area = new int[2];
            switch (object_id)
            {
                case 1:
                    area[0] = 2;
                    area[1] = 4;
                    return area;
                case 2:
                    area[0] = 1;
                    area[1] = 4;
                    return area;
                case 3:
                    area[0] = 1;
                    area[1] = 5;
                    return area;
                case 4:
                    area[0] = 3;
                    area[1] = 2;
                    return area;
                case 5:
                    area[0] = 2;
                    area[1] = 2;
                    return area;
                case 6:
                    area[0] = 1;
                    area[1] = 4;
                    return area;
                default:
                    area[0] = 1;
                    area[1] = 1;
                    return area;
            }
        }
    }
}
