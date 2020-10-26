using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels.Command
{
    public class MovementReceiver
    {
        public void MoveTo(int[,] map, int object_id, int x_pos, int y_pos)
        {
            try
            {
                writeToMap(map, object_id, x_pos, y_pos);
            }
            catch(Exception e)
            {
                String errrMessage = String.Format("Object wchich id is {0} cannot move to position x:{1} and y:{2}. Error occurred -> {3}", object_id, x_pos, y_pos, e.Message);
                Console.WriteLine(errrMessage);
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
        private int [] GetAreaSize(int object_id)
        {
            int[] area = new int[1];
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
