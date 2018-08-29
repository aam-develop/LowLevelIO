using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSET2
{


    /// <summary>
    /// 低水準インタフェースルーチン
    /// </summary>
    public class LowLevelIO
    {
        /// <summary>
        /// ファイルを読み込み専用にオープン
        /// </summary>
        public static readonly long O_RDONLY = 0x0001;
        /// <summary>
        /// ファイルを書き出し専用にオープン
        /// </summary>
        public static readonly long O_WRONLY = 0x0002;
        /// <summary>
        /// ファイルを読み込み、書き出し両用にオープン
        /// </summary>
        public static readonly long O_RDWR = 0x0004;
        /// <summary>
        /// ファイル名で示すファイルが存在しない場合にファイルを新規に作成
        /// </summary>
        public static readonly long O_CREAT = 0x0008;
        /// <summary>
        /// ファイル名で示すファイルが存在する場合にファイルの内容を捨て、ファイルのサイズを0に更新
        /// </summary>
        public static readonly long O_TRUNC = 0x0010;
        /// <summary>
        /// 次に読み込み/書き出しを行うファイル内の位置をファイルの最後に設定
        /// </summary>
        public static readonly long O_APPEND = 0x0020;

        private static readonly int MAX_FILENO = 128;

        private class FILE
        {
            public int Id { get; set; }
            public FileStream fs { get; set; }
        }
        FileStream[] list = new FileStream[MAX_FILENO];

        /// <summary>
        /// 入出力するファイルのパス
        /// </summary>
        public String BaseDir;

        /// <summary>
        /// コンストラクタ
        /// <para>デフォルトパス</para>
        /// </summary>
        public LowLevelIO() : this(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Vset2\\LUA")
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dir">デフォルトパス</param>
        public LowLevelIO(String dir)
        {
            this.BaseDir = dir;

            // 読み書きディレクトリを作成
            if (!Directory.Exists(this.BaseDir))
                Directory.CreateDirectory(this.BaseDir);

        }

        /// <summary>
        /// ファイルを開きます
        /// </summary>
        /// <param name="name">ファイルのファイル名を指す文字</param>
        /// <param name="mode">ファイルをオープンするときの処理の指定</param>
        /// <param name="flg"> アクセス許可モード</param>
        /// <returns>
        ///   <para>正常： オープンしたファイルのファイル番号</para>
        ///   <para>異常： -1</para>
        //// </returns>
        public long open(string name, long mode, long flg)
        {
            FileAccess access;
            FileMode filemode;

            if ((mode & O_RDWR) != 0)
            {
                access = FileAccess.ReadWrite;
            }
            else if ((mode & O_RDONLY) != 0)
            {
                access = FileAccess.Read;
            }
            else if ((mode & O_WRONLY) != 0)
            {
                access = FileAccess.Write;
            }
            else
                return -1;

            filemode = FileMode.OpenOrCreate;
            if ((mode & O_APPEND) != 0)
            {
                filemode = FileMode.Append;
            }
            if ((mode & O_TRUNC) != 0)
            {
                filemode = FileMode.Truncate;
            }


            // 空いているファイル番号を検索
            int i;
            for (i = 0; i < MAX_FILENO; i++)
                if (list[i] == null)
                {
                    try
                    {
                        list[i] = new FileStream(BaseDir + "\\" + name, filemode, access);
                    }
                    catch (Exception e)
                    {
                        Debug.Write(e.ToString());
                        return -1;
                    }
                    break;
                }
            if (i == MAX_FILENO)
                return -1;

            return i;
        }

        /// <summary>
        /// ファイルを閉じます。
        /// </summary>
        /// <param name="fileno">クローズするファイル番号</param>
        /// <returns>
        ///   <para>正常： 0</para>
        ///   <para>異常： -1</para>
        //// </returns>
        public long close(long fileno)
        {
            if (0 > fileno || fileno >= MAX_FILENO)
                return -1;
            if (list[fileno] == null)
                return -1;

            list[fileno].Dispose();
            list[fileno] = null;

            return 0;
        }

        /// <summary>
        /// ファイルからデータを読み込みます。
        /// </summary>
        /// <param name="fileno">読み込みの対象となるファイル番号</param>
        /// <param name="buf">読み込んだデータを格納する領域</param>
        /// <param name="count">読み込むバイト数</param>
        /// <returns>
        ///   <para>正常： 実際に読み込んだバイト数</para>
        ///   <para>異常： -1</para>
        /// </returns>
        public long read(long fileno, out byte[] buf, long count)
        {
            buf = new Byte[count];

            if (0 > fileno || fileno >= MAX_FILENO)
                return -1;
            if (list[fileno] == null)
                return -1;

            int readcount = list[fileno].Read(buf, 0, (int)count);

            return readcount;
        }

        /// <summary>
        /// データをファイルに書き込みます。
        /// </summary>
        /// <param name="fileno">書き出しの対象となるファイル番号</param>
        /// <param name="buf">書き出すデータ領域</param>
        /// <param name="count">書き出すバイト数</param>
        /// <returns>
        ///   <para>正常： 実際に書き出されたバイト数</para>
        ///   <para>異常： -1</para>
        /// </returns>
        public long write(long fileno, byte[] buf, long count)
        {
            if (0 > fileno || fileno >= MAX_FILENO)
                return -1;
            if (list[fileno] == null)
                return -1;

            list[fileno].Write(buf, 0, (int)count);
            return count;
        }

        /// <summary>
        /// ファイル ポインターを指定された位置に移動します。
        /// </summary>
        /// <param name="fileno">対象となるファイル番号</param>
        /// <param name="offset">読み込み/書き出しの位置を示すオフセット(バイト単位)</param>
        /// <param name="origin"> オフセットの起点</param>
        /// <returns>
        ///   <para>正常：  新しいファイルの読み込み/書き出し位置のファイルの先頭からのオフセット(バイト単位)</para>
        ///   <para>異常： -1</para>
        /// </returns>
        public long lseek(long fileno, long offset, long origin)
        {
            if (0 > fileno || fileno >= MAX_FILENO)
                return -1;
            if (list[fileno] == null)
                return -1;

            SeekOrigin seekorgine;
            switch (origin)
            {
                case 0:
                    seekorgine = SeekOrigin.Begin;
                    break;
                case 1:
                    seekorgine = SeekOrigin.Current;
                    break;
                case 2:
                    seekorgine = SeekOrigin.End;
                    break;
                default:
                    return -1;
            }


            long ret = list[fileno].Seek(offset, seekorgine);
            return ret;
        }


    }
}
