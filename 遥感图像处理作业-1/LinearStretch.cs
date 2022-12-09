using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace 遥感图像处理作业_1
{
    class LinearStretch
    {
        /** 
2%-98%最大最小值拉伸，小于最小值的设为0，大于最大值的设为255
@param pBuf 保存16位影像数据的数组，该数组一般直接由Gdal的RasterIO函数得到
@param dstBuf 保存8位影像数据的数组，该数组一般直接由Gdal的RasterIO函数得到 
@param width 图像的列数
@param height 图像的行数
@param minVal 用于保存计算得到的最小值
@param maxVal 用于保存计算得到的最大值
*/
        void MinMaxStretch(ref ushort pBuf,ref char dstBuf, int bufWidth, int bufHeight, double minVal, double maxVal)
        {
            ushort data;
            char result;
            for (int x = 0; x < bufWidth; x++)
            {
                for (int y = 0; y < bufHeight; y++)
                {
                    data = pBuf[x * bufHeight + y];
                    result = (data - minVal) / (maxVal - minVal) * 255;
                    dstBuf[x * bufHeight + y] = result;
                }
            }
        }

    }
}
