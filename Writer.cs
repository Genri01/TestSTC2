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
    /// This class describe record to destanation file
    /// </sumary>
    class Writer
    {
        private Thread ThreadWrite;
        private string destFilePath;
        private Buffer buffer;
        private int sizeBuffer;
        private bool active = true;

        public delegate void SetLabelWrite(string str);
        public event SetLabelWrite LabelWrite;
        /// <summary>
        /// This is Constructor
        /// </summary>
        /// <param name="DestFilePath">Path of destanation file</param>
        /// <param name="buffer">Variable of class Buffer</param>
        /// <param name="sizeBuffer">value is size Buffer from TextBox</param>
        public Writer(string DestFilePath, Buffer buffer, int sizeBuffer)
        {
            this.destFilePath = DestFilePath;
            this.buffer = buffer;
            this.sizeBuffer = sizeBuffer;
        }

        /// <summary>
        /// This method start writer thread
        /// </summary>
        public void Start()
        {
            ThreadWrite = new Thread(Run);
            ThreadWrite.Start();
        }
        /// <summary>
        /// This method stops writer thread
        /// </summary>
        public void pause()
        {
            active = false;
        }

        /// <summary>
        /// This method resume writer thread after stopping
        /// </summary>
        public void resume()
        {
            active = true;
        }
        /// <summary>
        /// This method describe work writer thread
        /// </summary>
        public void Run()
        {
            using (FileStream destStream = new FileStream(destFilePath, FileMode.Create, FileAccess.Write))
            {
                while (true)
                {
                    //This is boolean, which reports that writing not stopped
                    if (active)
                    {
                        byte[] bytes = buffer.Read(sizeBuffer);
                        destStream.Write(bytes, 0, bytes.Length);

                        //if buffer Closed = true, that writing comlite. More in method buffer.Closed
                        if (buffer.Closed())
                        {
                            MessageBox.Show("Writing complete. File copied successfully.");
                            LabelWrite("Writing complete");
                            break;
                        }
                        //this Sleep use only for beautifull visualizations of status 'progressbar'
                        Thread.Sleep(50);

                    }
                }
            }
        }
    }
}
