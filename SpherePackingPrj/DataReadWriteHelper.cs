using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Newtonsoft.Json;

namespace SpherePacking.MainWindow
{
    /// <summary>
    /// 提供读写文件的静态类
    /// </summary>
    class DataReadWriteHelper
    {

        /// <summary>
        /// 保存矩阵数据
        /// </summary>
        /// <param name="fn">文件名</param>
        /// <param name="info">矩阵</param>
        /// <returns></returns>
        public static bool RecordInfo(string fn, string dirForSaveInfo, Matrix<double> info, bool addComment = true)
        {
            bool res = true;

            if (info.Rows == 0 || info.Cols == 0)
            {
                Console.WriteLine("Matrix is empty !");
                res = false;
            }
            else
            {
                if (!Directory.Exists(dirForSaveInfo))
                {
                    Directory.CreateDirectory(dirForSaveInfo);
                }
                FileStream fs = new FileStream(dirForSaveInfo + fn, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                if( addComment )
                {
                    string head = String.Format("The Matrix has {0} Rows, {1} Cols", info.Rows, info.Cols);
                    sw.WriteLine(head);
                }
                string str = "";
                for (int i = 0; i < info.Rows; i++)
                {
                    str = "";
                    for (int j = 0; j < info.Cols - 1; j++)
                    {
                        str += info[i, j].ToString("0.00000000") + ", ";
                    }
                    str += info[i, info.Cols - 1].ToString("0.00000000");
                    sw.WriteLine(str);
                }
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            return res;
        }

        /// <summary>
        /// 将DEM模型保存到文件中
        /// 会存储模型中的小球的半径和当前的位置、速度和加速度
        /// example :
        ///             DataReadWriteHelper.SaveModelDemAsSimple(modelDem3D, "result.json");
        /// </summary>
        /// <param name="model">MddelDem3D</param>
        /// <param name="fn">file name to save</param>
        /// <returns></returns>
        public static bool SaveObjAsJsonFile( object obj, string fn )
        {
            bool res = true;

            try
            {
                string str = JsonConvert.SerializeObject(obj);

                using (StreamWriter sw = new StreamWriter(fn))
                {
                    using (JsonWriter js = new JsonTextWriter(sw))
                    {
                        (new JsonSerializer()).Serialize(js, obj);
                    }
                    sw.Close();
                }
            }
            catch(Exception ex)
            {
                res = false;
                Console.WriteLine(ex);
            }
            return res;
        }

        /// <summary>
        /// 从json文件中导入SimpleModelForSave中
        /// example :
        ///             SimpleModelForSave sModel;
        ///             DataReadWriteHelper.LoadSimpleModelFromFile("result.json", out sModel);
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="sModel"></param>
        /// <returns></returns>
        public static T LoadSimpleModelFromFile<T>( string fn )
        {
            T obj;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(fn);
                string str = sr.ReadToEnd();
                obj = JsonConvert.DeserializeObject<T>(str);
            }
            catch(Exception ex)
            {
                obj = default(T);
                Console.WriteLine( ex );
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }

            
            return obj;
        }
    }
}
