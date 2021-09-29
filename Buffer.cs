using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace TestSTC
{
    /// <sumary>
    /// This class work with buffer
    /// </sumary>
    class Buffer
    {
        public delegate void Complet(int value);
        public event Complet onChange;

        private Byte[] streamBuffer;
        int bufferLevel = 0;
        bool closeFlag = false;


        /// <summary>
        /// This is Constructor
        /// </summary>
        /// <param name="BufferLength">This is size of buffer</param>
        public Buffer(int BufferLength)
        {
            this.streamBuffer = new Byte[BufferLength];
        }

        /// <summary>
        /// This is method which reading to buffer
        /// </summary>
        /// <param name="size">This is size</param>
        public byte[] Read(int size)
        {

            byte[] outbuffer;

            lock (this)
            {
                // if size> bufferLevel that not avaliable place in buffer
                //bufferLevel is  current fulling status of buffer
                if (size > bufferLevel)
                {
                    size = bufferLevel;
                }
                outbuffer = CutBytesFromStart(size);
                ShiftBytesToLeft(size);
                onChange(Convert.ToInt32(bufferLevel / (streamBuffer.Length * 0.01)));

            }
            return outbuffer;
        }


        /// <summary>
        /// This is method which cut empty bytes from begining buffer 
        /// </summary>
        /// <param name="size">size of empty bytes</param>
        private byte[] CutBytesFromStart(int size)
        {
            byte[] outbuffer = new byte[size];
            Array.Copy(streamBuffer, 0, outbuffer, 0, size);
            return outbuffer;
        }


        /// <summary>
        /// This is method which move bytes from buffer on the left 
        /// </summary>
        /// <param name="distance">size of empty bytes</param>
        private void ShiftBytesToLeft(int distance)
        {
            Array.Copy(streamBuffer, distance, streamBuffer, 0, bufferLevel - distance);
            bufferLevel -= distance;
        }

        /// <summary>
        /// This is method which writing to buffer
        /// </summary>
        /// <param name="bytes">This is bytes from buffer</param>
        /// <param name="readbyte">this is size read bytes</param>
        public bool Write(byte[] bytes, int readbyte)
        {
            lock (this)
            {
                if (CanWrite(readbyte))
                {
                    Array.Copy(bytes, 0, streamBuffer, bufferLevel, readbyte);
                    bufferLevel += readbyte;
                    try
                    {
                        //this is status progressBar in percentage
                        onChange(Convert.ToInt32(bufferLevel / (streamBuffer.Length * 0.01)));
                    }
                    
                    catch (NullReferenceException)
                    {
                        return true;
                    }
                    return true;

                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// This is method which check bollean
        /// if CloseFlag = true that reading comlete
        /// </summary>
        public void Finish()
        {
            closeFlag = true;
        }
        /// <summary>
        /// This is method which check bollean
        /// if CloseFlag = true and buffer is empty that writing complete
        /// </summary>
        public bool Closed()
        {
            return closeFlag && bufferLevel == 0;
        }

        /// <summary>
        /// This is method which check bollean
        /// if bufferLevel - size > 0 that buffer has avaliable place
        /// </summary>
         private bool CanWrite(int size)
        {
            return streamBuffer.Length - bufferLevel >= size;
        }




    }
}
