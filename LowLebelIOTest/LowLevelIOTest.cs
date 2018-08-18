using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using VSET2;

namespace LowLevelIOTest
{
    [TestClass]
    public class LowLevelIOTest
    {
        static int MAX_FILENO = 128;

        [TestInitialize]
        public void TestInitialize()
        {
            DirectoryInfo target = new DirectoryInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Vset2\\LUA");
            // ディレクトリごとファイルを消す
            target.Delete(true);

        }

        /// <summary>
        /// 通常書き込み
        /// </summary>
        [TestMethod]
        [TestCategory("Normal")]
        public void NormalWrite()
        {
            LowLevelIO test = new LowLevelIO();

            long fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes("TEST");
            test.write(fileno, buf, 4);
            test.close(fileno);

            Assert.IsTrue(File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "\\Vset2\\LUA\\test.csv"));

        }
        /// <summary>
        /// 通常読み込み
        /// </summary>
        [TestMethod]
        [TestCategory("Normal")]
        [DeploymentItem("ReadTest.txt")]
        public void NormalRead()
        {
            LowLevelIO test = new LowLevelIO();
            string actual = "TEST";

            // 書いて
            long fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(actual);
            test.write(fileno, writebuf, 4);
            test.close(fileno);

            // 読んで
            fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] readbuf = new Byte[4];
            long count = test.read(fileno, out readbuf, 4);
            test.close(fileno);

            // 正しいこと
            CollectionAssert.AreEqual(writebuf, readbuf);

        }

        /// <summary>
        /// 複数ファイルを同時に開く
        /// </summary>
        [TestMethod]
        [TestCategory("Open")]
        public void DoubleOpen()
        {
            LowLevelIO test = new LowLevelIO();

            long fileno1 = test.open("test1.csv", LowLevelIO.O_RDWR, 0x777);
            long fileno2 = test.open("test2.csv", LowLevelIO.O_RDWR, 0x777);

            test.close(fileno1);
            test.close(fileno2);

            // ファイル番号が異なること
            Assert.AreNotEqual(fileno1, fileno2);
        }

        /// <summary>
        /// 127ファイルまで同時に開く
        /// </summary>
        [TestMethod]
        [TestCategory("Open")]
        public void Open127Files()
        {
            LowLevelIO test = new LowLevelIO();

            long[] fileno = new long[MAX_FILENO];

            for (int i = 0; i < MAX_FILENO; i++)
            {
                fileno[i] = test.open("test" + i + ".csv", LowLevelIO.O_RDWR, 0x777);
                // 失敗しないこと
                Assert.AreNotEqual(-1, fileno[i]);
            }

            for (int i = 0; i < MAX_FILENO; i++)
            {
                test.close(fileno[i]);
            }
        }

        /// <summary>
        /// 128ファイル目はエラー
        /// </summary>
        [TestMethod]
        [TestCategory("Open")]
        public void Open128Files()
        {
            LowLevelIO test = new LowLevelIO();

            long[] fileno = new long[MAX_FILENO];

            for (int i = 0; i < MAX_FILENO; i++)
            {
                fileno[i] = test.open("test" + i + ".csv", LowLevelIO.O_RDWR, 0x777);
                // 失敗しないこと
                Assert.AreNotEqual(-1, fileno[i]);
            }

            long errorFileNo = test.open("test128.csv", LowLevelIO.O_RDWR, 0x777);
            // 128番目はエラー
            Assert.AreEqual(-1, errorFileNo);

            for (int i = 0; i < MAX_FILENO; i++)
            {
                test.close(fileno[i]);
            }

        }

        /// <summary>
        /// ReadOnly
        /// </summary>
        [TestMethod]
        [TestCategory("Open")]
        public void OpenReadOnly()
        {
            LowLevelIO test = new LowLevelIO();

            long[] fileno = new long[MAX_FILENO];

            for (int i = 0; i < MAX_FILENO; i++)
            {
                fileno[i] = test.open("test" + i + ".csv", LowLevelIO.O_RDONLY, 0x777);
                // 失敗しないこと
                Assert.AreNotEqual(-1, fileno[i]);
            }

            for (int i = 0; i < MAX_FILENO; i++)
            {
                test.close(fileno[i]);
            }

        }
        /// <summary>
        /// WriteOnly
        /// </summary>
        [TestMethod]
        [TestCategory("Open")]
        public void OpenWriteOnly()
        {
            LowLevelIO test = new LowLevelIO();

            long[] fileno = new long[MAX_FILENO];

            for (int i = 0; i < MAX_FILENO; i++)
            {
                fileno[i] = test.open("test" + i + ".csv", LowLevelIO.O_WRONLY, 0x777);
                // 失敗しないこと
                Assert.AreNotEqual(-1, fileno[i]);
            }

            for (int i = 0; i < MAX_FILENO; i++)
            {
                test.close(fileno[i]);
            }

        }
        /// <summary>
        /// ReadでもWriteでもない
        /// </summary>
        [TestMethod]
        [TestCategory("Open")]
        public void OpenOhter()
        {
            LowLevelIO test = new LowLevelIO();

            long[] fileno = new long[MAX_FILENO];

            for (int i = 0; i < MAX_FILENO; i++)
            {
                fileno[i] = test.open("test" + i + ".csv", 0x0000, 0x777);
                // 失敗すること
                Assert.AreEqual(-1, fileno[i]);
            }

            for (int i = 0; i < MAX_FILENO; i++)
            {
                test.close(fileno[i]);
            }

        }
        /// <summary>
        /// Appendテスト
        /// </summary>
        [TestMethod]
        [TestCategory("Open")]
        public void OpenAppend()
        {
            LowLevelIO test = new LowLevelIO();

            string actual1 = "ABCD";
            string actual2 = "EFG";
            byte[] actualbuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(actual1 + actual2);

            // 普通に書いて
            long fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(actual1);
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);

            // 追記する
            fileno = test.open("test.csv", LowLevelIO.O_WRONLY | LowLevelIO.O_APPEND, 0x777);
            Assert.AreNotEqual(-1, fileno);
            writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(actual2);
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);

            // 読んで
            fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] readbuf = new Byte[actual1.Length + actual2.Length];
            long count = test.read(fileno, out readbuf, actual1.Length + actual2.Length);
            test.close(fileno);

            // 正しいこと
            CollectionAssert.AreEqual(actualbuf, readbuf);

            test.close(fileno);
        }

        /// <summary>
        /// Truncateテスト
        /// </summary>
        [TestMethod]
        [TestCategory("Open")]
        public void OpenTrancate()
        {
            LowLevelIO test = new LowLevelIO();

            string actual1 = "ABCD";
            string actual2 = "EFG";
            byte[] actualbuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(actual2);

            // 普通に書いて
            long fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(actual1);
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);

            // 初めから書き直す
            fileno = test.open("test.csv", LowLevelIO.O_WRONLY | LowLevelIO.O_TRUNC, 0x777);
            Assert.AreNotEqual(-1, fileno);
            writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(actual2);
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);

            // 読んで
            fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] readbuf = new Byte[actual2.Length];
            long count = test.read(fileno, out readbuf, actual2.Length);
            test.close(fileno);

            // 最後に書いた文字列のみであること
            CollectionAssert.AreEqual(actualbuf, readbuf);

            test.close(fileno);
        }

        /// <summary>
        /// Read時にAppendされたら、エラー
        /// </summary>
        [TestMethod]
        [TestCategory("Open")]
        public void OpenAppendError()
        {
            LowLevelIO test = new LowLevelIO();

            // ReadでAppend
            long fileno = test.open("test.csv", LowLevelIO.O_RDONLY | LowLevelIO.O_APPEND, 0x777);
            Assert.AreEqual(-1, fileno);

            test.close(fileno);
        }

        /// <summary>
        /// ファイルをクローズしたら、ファイル番号は再利用する
        /// </summary>
        [TestMethod]
        [TestCategory("Close")]
        public void ファイル番号再利用()
        {
            LowLevelIO test = new LowLevelIO();

            long[] fileno = new long[MAX_FILENO];

            // すべてのファイル番号を開いて
            for (int i = 0; i < MAX_FILENO; i++)
                fileno[i] = test.open("test" + i + ".csv", LowLevelIO.O_RDWR, 0x777);

            // 5番をクローズ
            long no5 = fileno[5];
            test.close(no5);
            long newFileNo = test.open("test5.csv", LowLevelIO.O_RDWR, 0x777);

            // 廃棄した番号が使用されること
            Assert.AreEqual(no5, newFileNo);

            // ５番も含めてすべてクローズ
            for (int i = 0; i < MAX_FILENO; i++)
                test.close(fileno[i]);

        }

        /// <summary>
        /// 開いていないファイル番号をクローズ
        /// </summary>
        [TestMethod]
        [TestCategory("Close")]
        public void Close存在しないFileNo()
        {
            LowLevelIO test = new LowLevelIO();

            // 一つ開いて
            long fileno = test.open("test0.csv", LowLevelIO.O_RDWR, 0x777);

            // 別の番号をクローズ
            long ret = test.close(10);
            Assert.AreEqual(-1, ret); // 失敗すること

            // ファイル番号の範囲外をclose(最小)
            ret = test.close(-1);
            Assert.AreEqual(-1, ret); // 失敗すること

            // ファイル番号の範囲外をclose(最大)
            ret = test.close(MAX_FILENO);
            Assert.AreEqual(-1, ret); // 失敗すること

            test.close(fileno);
        }

        /// <summary>
        /// ファイル番号の範囲外をRead(最大)
        /// </summary>
        [TestMethod]
        [TestCategory("Read")]
        public void ReadMaxFileno()
        {
            LowLevelIO test = new LowLevelIO();

            byte[] readbuf = new Byte[4];

            // ファイル番号の範囲外をRead(最大)
            long ret = test.read(MAX_FILENO, out readbuf, 4);
            Assert.AreEqual(-1, ret); // 失敗すること

        }
        /// <summary>
        /// ファイル番号の範囲外をRead(最小)
        /// </summary>
        [TestMethod]
        [TestCategory("Read")]
        public void ReadMinFileno()
        {
            LowLevelIO test = new LowLevelIO();

            byte[] readbuf = new Byte[4];

            // ファイル番号の範囲外をRead(最小)
            long ret = test.read(-1, out readbuf, 4);
            Assert.AreEqual(-1, ret); // 失敗すること

        }
        /// <summary>
        /// オープンしていないファイル番号をRead
        /// </summary>
        [TestMethod]
        [TestCategory("Read")]
        public void ReadBadFileno()
        {
            LowLevelIO test = new LowLevelIO();

            byte[] readbuf = new Byte[4];

            // 開いていないファイル番号をRead
            long ret = test.read(10, out readbuf, 4);
            Assert.AreEqual(-1, ret); // 失敗すること

        }

        /// <summary>
        /// ファイル番号の範囲外をWrite(最大)
        /// </summary>
        [TestMethod]
        [TestCategory("Write")]
        public void WriteMaxFileno()
        {
            LowLevelIO test = new LowLevelIO();

            byte[] Writebuf = new Byte[4];

            // ファイル番号の範囲外をWrite(最大)
            long ret = test.write(MAX_FILENO, Writebuf, 4);
            Assert.AreEqual(-1, ret); // 失敗すること

        }
        /// <summary>
        /// ファイル番号の範囲外をWrite(最小)
        /// </summary>
        [TestMethod]
        [TestCategory("Write")]
        public void WriteMinFileno()
        {
            LowLevelIO test = new LowLevelIO();

            byte[] Writebuf = new Byte[4];

            // ファイル番号の範囲外をWrite(最小)
            long ret = test.write(-1, Writebuf, 4);
            Assert.AreEqual(-1, ret); // 失敗すること

        }
        /// <summary>
        /// オープンしていないファイル番号をWrite
        /// </summary>
        [TestMethod]
        [TestCategory("Write")]
        public void WriteBadFileno()
        {
            LowLevelIO test = new LowLevelIO();

            byte[] Writebuf = new Byte[4];

            // 開いていないファイル番号をWrite
            long ret = test.write(10, Writebuf, 4);
            Assert.AreEqual(-1, ret); // 失敗すること

        }

        /// <summary>
        /// ファイル番号の範囲外をLseek(最大)
        /// </summary>
        [TestMethod]
        [TestCategory("Lseek")]
        public void LseekMaxFileno()
        {
            LowLevelIO test = new LowLevelIO();

            // ファイル番号の範囲外をLseek(最大)
            long ret = test.lseek(MAX_FILENO, 0, 0);
            Assert.AreEqual(-1, ret); // 失敗すること

        }
        /// <summary>
        /// ファイル番号の範囲外をLseek(最小)
        /// </summary>
        [TestMethod]
        [TestCategory("Lseek")]
        public void LseekMinFileno()
        {
            LowLevelIO test = new LowLevelIO();

            // ファイル番号の範囲外をLseek(最小)
            long ret = test.lseek(-1, 0, 0);
            Assert.AreEqual(-1, ret); // 失敗すること

        }
        /// <summary>
        /// オープンしていないファイル番号をLseek
        /// </summary>
        [TestMethod]
        [TestCategory("Lseek")]
        public void LseekBadFileno()
        {
            LowLevelIO test = new LowLevelIO();

            // 開いていないファイル番号をLseek
            long ret = test.lseek(10, 0, 0);
            Assert.AreEqual(-1, ret); // 失敗すること

        }
        /// <summary>
        /// オフセットの起点がエラー
        /// </summary>
        [TestMethod]
        [TestCategory("Lseek")]
        public void LseekBadOrigin()
        {
            LowLevelIO test = new LowLevelIO();

            // 一つ開いて
            long fileno = test.open("test0.csv", LowLevelIO.O_RDWR, 0x777);
            // Originをエラーにする
            long ret = test.lseek(fileno, 0, -1);
            Assert.AreEqual(-1, ret); // 失敗すること
            // Originをエラーにする
            ret = test.lseek(fileno, 0, 3);
            Assert.AreEqual(-1, ret); // 失敗すること

            test.close(fileno);

        }

        /// <summary>
        /// オフセットの起点 Begin
        /// </summary>
        [TestMethod]
        [TestCategory("Lseek")]
        public void LseekOriginBegin0()
        {
            LowLevelIO test = new LowLevelIO();

            // 検証用ファイルを準備
            long fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes("ABCDEFG");
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);


            fileno = test.open("test.csv", LowLevelIO.O_WRONLY, 0x777);

            // OriginをBeginから0byteに設定
            long ret = test.lseek(fileno, 0, 0);
            Assert.AreEqual(0, ret); // 先頭に設定されたこと

            // 先頭を000に書き換え
            writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes("000");
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);

            // 読んで
            fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] readbuf = new Byte[7];
            long count = test.read(fileno, out readbuf, 7);
            test.close(fileno);

            // 書き変わっていること
            CollectionAssert.AreEqual(System.Text.Encoding.GetEncoding("UTF-8").GetBytes("000DEFG"), readbuf);
        }

        /// <summary>
        /// オフセットの起点 Begin
        /// </summary>
        [TestMethod]
        [TestCategory("Lseek")]
        public void LseekOriginBegin1()
        {
            LowLevelIO test = new LowLevelIO();

            // 検証用ファイルを準備
            long fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes("ABCDEFG");
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);


            fileno = test.open("test.csv", LowLevelIO.O_WRONLY, 0x777);

            // OriginをBeginから1byteに設定
            long ret = test.lseek(fileno, 1, 0);
            Assert.AreEqual(1, ret); // 1Byte目に設定されたこと

            // 1byte目以降を000に書き換え
            writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes("000");
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);

            // 読んで
            fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] readbuf = new Byte[7];
            long count = test.read(fileno, out readbuf, 7);
            test.close(fileno);

            // 書き変わっていること
            CollectionAssert.AreEqual(System.Text.Encoding.GetEncoding("UTF-8").GetBytes("A000EFG"), readbuf);
        }

        /// <summary>
        /// オフセットの起点 End
        /// </summary>
        [TestMethod]
        [TestCategory("Lseek")]
        public void LseekOriginEnd()
        {
            LowLevelIO test = new LowLevelIO();

            // 検証用ファイルを準備
            long fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes("ABCDEFG");
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);


            fileno = test.open("test.csv", LowLevelIO.O_WRONLY, 0x777);

            // OriginをEndに設定
            long ret = test.lseek(fileno, 0, 2);
            Assert.AreEqual(writebuf.Length, ret); // 最後に設定されていること

            // 最後に000を追記
            writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes("000");
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);

            // 読んで
            fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] readbuf = new Byte[10];
            long count = test.read(fileno, out readbuf, readbuf.Length);
            test.close(fileno);

            // 追記されていること
            CollectionAssert.AreEqual(System.Text.Encoding.GetEncoding("UTF-8").GetBytes("ABCDEFG000"), readbuf);
        }

        /// <summary>
        /// オフセットの起点 Current
        /// </summary>
        [TestMethod]
        [TestCategory("Lseek")]
        public void LseekOriginCurrent()
        {
            LowLevelIO test = new LowLevelIO();

            // 検証用ファイルを準備
            long fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes("ABCDEFG");
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);


            fileno = test.open("test.csv", LowLevelIO.O_WRONLY, 0x777);

            // OriginをCurrentに設定(先頭+1byte)
            long ret = test.lseek(fileno, 1, 1);
            Assert.AreEqual(1, ret); // 1byteに設定されていること

            // 1byteから000に書き換え
            writebuf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes("000");
            test.write(fileno, writebuf, writebuf.Length);
            test.close(fileno);

            // 読んで
            fileno = test.open("test.csv", LowLevelIO.O_RDWR, 0x777);
            byte[] readbuf = new Byte[10];
            long count = test.read(fileno, out readbuf, readbuf.Length);
            Array.Resize(ref readbuf, (int)count); // 読み取れたサイズに配列を変更
            test.close(fileno);

            // 追記されていること
            CollectionAssert.AreEqual(System.Text.Encoding.GetEncoding("UTF-8").GetBytes("A000EFG"), readbuf);
        }

    }
}
