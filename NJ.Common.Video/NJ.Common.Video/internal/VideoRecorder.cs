// https://github-wiki-see.page/m/baSSiLL/SharpAvi/wiki/Working-with-Audio-Streams
// https://renenyffenegger.ch/notes/development/graphic/video/cs-AVI-Writer/Mandelbrot

namespace NJ.Common.Video
{
    //--------------------------------------------------------------------//
    //                                                          C l a s s //
    // V i d e o R e c o r d e r                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // This program demonstrates how to .avi .mp4                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    #region Include

    //*** System.Reflection.MemberInfo ***//

    using System; //*** DateTime TimeSpan ManualResetEvent Reflection.MemberInfo ***//
    using System.Drawing; //*** Rectangle Size CopyPixelOpertation Bitmap Graphics Point ***//
    using System.Threading; //*** WaitHandle ManualResetEvent Thread ThreadState.Running***//
    using System.Threading.Tasks; //*** Task ***//
    using System.Drawing.Imaging; //*** ImageLockMode PixelFormat ***//
    using System.Runtime.InteropServices; //*** Marshall ***//

    using SharpAvi.Output; //*** AviWriter IAviVideoStream ***//

    #endregion

    /// <summary>
    /// <para>Provides functionality for recording my screen! This class cannot be inherited.</para>
    /// </summary>
    internal sealed class VideoRecorder : IDisposable
    {
        #region Constructor

        /// <summary>
        /// <para>Initializes a new instance of the Recorder class.</para>
        /// </summary>
        /// <param name="Params">Used to Configure the Recorder</param>
        internal VideoRecorder(RecorderParameters Params)
        {
            //-- Any description is better than no description. --//
            this.Params = Params;
            //-- Create AVI writer and specify FPS. --//
            writer = Params.CreateAviWriter();
            //-- Create video stream. --//
            videoStream = Params.CreateVideoStream(writer);
            //-- Audio stream of AVI file. --//
            audioStream = Params.CreateAudioStream(writer);
            //-- May be used by some players when displaying the list of available streams. --//
            audioStream.Name = "Sound";
            //-- Set only name. Other properties were when creating stream,  either explicitly by arguments or implicitly by the encoder used
            videoStream.Name = "Captura";
            //--Initializes a new instance of the Thread class.--//
            screenThread = new Thread(RecordScreen)
            {
                //-- Gets the name of the current member. --//
                Name = typeof(VideoRecorder).Name + ".RecordScreen",
                //-- Gets or sets a value indicating whether or not a thread is a background thread.--//
                IsBackground = true
            };
            //-- Initializes a new instance of the Thread class. --//
            audioThread = new Thread(RecordAudio)
            {
                //-- Gets the name of the current member. --//
                Name = typeof(VideoRecorder).Name + ".RecordAudio",
                //-- --//
                IsBackground = true
            };
            //-- Causes the operating system to change the state of the current instance to Running. --//
            screenThread.Start();
            //-- Causes the operating system to change the state of the current instance to System.Threading.ThreadState.Running. --//
            audioThread.Start();
        }
        #endregion

        #region Field

        /// <summary>
        /// <para>Used to write an AVI file.</para>
        /// </summary>
        AviWriter writer;

        /// <summary>
        /// <para>Used to Configure the Recorder</para>
        /// </summary>
        RecorderParameters Params;

        /// <summary>
        /// <para>Video stream of AVI file.</para>
        /// </summary>
        IAviVideoStream videoStream;

        /// <summary>
        /// <para>Audio stream of AVI file.</para>
        /// </summary>
        IAviAudioStream audioStream;

        /// <summary>
        /// <para>A thread for windows video.</para>
        /// </summary>
        Thread screenThread;

        /// <summary>
        /// <para>A thread for windows audio.</para>
        /// </summary>
        Thread audioThread;

        /// <summary>
        /// <para> Notifies one or more waiting threads that an event has occurred. This class cannot be inherited.</para>
        /// </summary>
        ManualResetEvent stopThread = new ManualResetEvent(false);

        /// <summary>
        /// <para>Notifies one or more waiting threads that an event has occurred. This class cannot be inherited.</para>
        /// </summary>
        ManualResetEvent stopThreadB = new ManualResetEvent(false);

        #endregion

        #region Method


        // public override void Dispose() ?
        /// <summary>
        /// <para>Provides implementation for this.Dispose()</para>
        /// </summary>
        public void Dispose()
        {
            //-- Sets the state of the event to signaled, allowing one or more waiting threads to proceed. --//
            stopThread.Set();
            //-- Blocks the calling thread until a thread terminates, while continuing to perform standard COM and SendMessage pumping. --//
            screenThread.Join();
            //-- Close writer: the remaining data is written to a file and file is closed
            writer.Close();
            //-- Releases all resources used by the current instance of the WaitHandle class. --//
            stopThread.Dispose();
        }

        /// <summary>
        /// <para>Any description is better than no description.</para>
        /// </summary>
        void RecordAudio()
        {
            //-- Sets the state of the event to signaled, allowing one or more waiting threads to proceed. --//
            stopThreadB.Set();
            //--  Blocks the calling thread until a thread terminates, while continuing to perform standard COM and SendMessage pumping. --//
            audioThread.Join();
        }

        /// <summary>
        /// <para>Starts the recording of a screen.</para>
        /// </summary>
        void RecordScreen()
        {
            //-- Returns a System.TimeSpan that represents a specified number of seconds, where the specification is accurate to the nearest millisecond. --//
            var frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
            //-- --//
            var buffer = new byte[Params.Width * Params.Height * 4];
            //-- Represents an asynchronous operation. --//
            Task videoWriteTask = null;
            //-- Represents the zero System.TimeSpan value. This field is read-only. --//
            var timeTillNextFrame = TimeSpan.Zero;
            //-- Blocks the current thread until the current instance receives a signal, using a TimeSpan to specify the time interval. --//
            while (!stopThread.WaitOne(timeTillNextFrame))
            {
                //-- Gets a DateTime object that is set to the current date and time on this computer, expressed as the local time. --//
                var timestamp = DateTime.Now;

                //-- Take a screenshot. --//
                Screenshot(buffer);

                //--Wait for the previous frame is written--//
                videoWriteTask?.Wait();

                //--Start asynchronous (encoding and) writing of the new frame--//
                videoWriteTask = videoStream.WriteFrameAsync(true, buffer, 0, buffer.Length);

                timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                if (timeTillNextFrame < TimeSpan.Zero) timeTillNextFrame = TimeSpan.Zero;
            }

            //--Wait for the last frame is written--//
            //--Waits for the System.Threading.Tasks.Task to complete execution.--//
            videoWriteTask?.Wait();
        }

        /// <summary>
        /// <para>Take a screenshot.</para>
        /// </summary>
        /// <param name="Buffer"> Represents an array of 8-bit unsigned integers.</param>
        public void Screenshot(byte[] Buffer)
        {
            //-- Encapsulates a GDI+ bitmap, which consists of the pixel data for a graphics image and its attributes. --//
            //-- A Bitmap is an object used to work with images defined by pixel data.  --//
            using (Bitmap bitmap = new Bitmap(Params.Width, Params.Height))
            {
                //--  Creates a new System.Drawing.Graphics from the specified System.Drawing.Image. --//
                //-- Encapsulates a GDI+ drawing surface. This class cannot be inherited. --//
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {

                    var upperLeftSource = Point.Empty;
                    var upperLeftDestination = Point.Empty;
                    var blockRegionSize = new Size(Params.Width, Params.Height);
                    var copyPixelOperation = CopyPixelOperation.SourceCopy;

                    //-- Performs a bit-block transfer of color data, corresponding to a rectangle of pixels from the screen to the drawing surface of the Graphics.  --//
                    graphics.CopyFromScreen(upperLeftSource, upperLeftDestination, blockRegionSize, copyPixelOperation);

                    //-- Forces execution of all pending graphics operations and returns immediately without waiting for the operations to finish. --//
                    graphics.Flush();

                    //-- Initializes a new instance of the System.Drawing.Rectangle class with the specified location and size.--//
                    int x = 0;
                    int y = 0;
                    int width = Params.Width;
                    int height = Params.Height;
                    var rectangle = new Rectangle(x, y, width, height);


                    var rect = rectangle;
                    var flags = ImageLockMode.ReadOnly;
                    var format = PixelFormat.Format32bppRgb;

                    //-- Locks a Bitmap into system memory. --//
                    BitmapData bits = bitmap.LockBits(rect, flags, format);




                    var source = bits.Scan0;
                    var destination = Buffer;
                    var startIndex = 0;
                    int length = Buffer.Length;

                    //-- Copies data from an unmanaged memory pointer to a managed 8-bit unsigned integer array.--//
                    Marshal.Copy(source, destination, startIndex, length);

                    //-- Unlocks this System.Drawing.Bitmap from system memory. --//
                    bitmap.UnlockBits(bits);
                }
            }
        }
        #endregion
    }
}
