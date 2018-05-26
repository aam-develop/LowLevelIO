using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowLevelIO
{
    /// <summary>
    /// 低水準インタフェースルーチン
    /// </summary>
    class LowLevelIO
    {
        /// <summary>
        /// 入出力するファイルのパス
        /// </summary>
        String BaseDir;

        /// <summary>
        /// コンストラクタ
        /// <para>デフォルトパス</para>
        /// </summary>
        public LowLevelIO() : this(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "Vset2\\LUA")
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dir"></param>
        public LowLevelIO(String dir)
        {
            this.BaseDir = dir;
        }

        /// <summary>
        /// ファイルを開きます
        /// </summary>
        /// <param name="name">ファイルのファイル名を指す文字</param>
        /// <param name="mode">ファイルをオープンするときの処理の指定</param>
        /// <param name="flg"> ファイルをオープンするときの処理の指定</param>
        /// <returns>
        ///   <para>正常： 正常オープンしたファイルのファイル番号</para>
        ///   <para>異常： -1</para>
        //// </returns>
        public long open(string name, long mode, long flg)
        {
            return -1;
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
            return -1;
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
            buf = new byte[0];
            return -1;
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
            return -1;
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
            return -1;
        }


    }
}
