using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace TestSTC
{
    /// <sumary>
    /// This class describe reading from source file
    /// </sumary>
    class Reader
    {
        private Thread ThreadRead;
        private string sourceFilePath;
        private Buffer buffer;
        private byte[] internalbuffer;
        private bool active = true;
        public delegate void SetLabelRead(string str);
        public event SetLabelRead LabelRead;

        /// <summary>
        /// This is Constructor
        /// </summary>
        /// <param name="sourceFilePath">Path of source file</param>
        /// <param name="buffer">Variable of class Buffer </param>
        /// <param name="sizeBuffer">value is size Buffer from TextBox</param>
       
        public Reader(string sourceFilePath, Buffer buffer, int sizeBuffer)
        {
            this.sourceFilePath = sourceFilePath;
            this.buffer = buffer;
            this.internalbuffer = new byte[sizeBuffer];
        }


        /// <summary>
        /// This method start reader thread
        /// </summary>
        public void Start()
        {
            ThreadRead = new Thread(Run);
            ThreadRead.Start();
        }
        /// <summary>
        /// This method stops reader thread
        /// </summary>
        public void pause()
        {
            active = false;
        }

        /// <summary>
        /// This method resume reader thread after stopping
        /// </summary>
        public void resume()
        {
            active = true;
        }

        /// <summary>
        /// This method describe work reader thread
        /// </summary>
        public void Run()
        {
            using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
            {
                while (true)
                {
                    
                    if (active)
                    {
                        int readbyte = sourceStream.Read(internalbuffer, 0, internalbuffer.Length);


                        //this condition which checks - buffer has avaliable place for writing or not
                        while (!buffer.Write(internalbuffer, readbyte))
                        {
                            Thread.Sleep(50);
                        }
                        //this Sleep use only for beautifull visualizations of status 'progressbar'
                        Thread.Sleep(50);

                        //this condition check length of buffer and check read byte length
                        //if read byte length less  length of buffer - this is last bytes
                        //It's means that reading is over
                        if (readbyte < internalbuffer.Length)
                        {
                            MessageBox.Show("Reading complete.");
                            buffer.Finish();
                            LabelRead("Reading Complete");
                            break;
                        }
                    }

                }
            }
        }

    }
}
