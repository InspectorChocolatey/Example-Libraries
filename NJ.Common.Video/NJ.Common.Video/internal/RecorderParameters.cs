namespace NJ.Common.Video
{
    //--------------------------------------------------------------------//
    //                                                          C l a s s //
    // V i d e o C o n t e x t                                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // This program demonstrates how to .avi .mp4                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    #region Includes

    //*** System.Drawing.Rectangle ***//

    using SharpAvi;//*** FourCC ***//
    using SharpAvi.Codecs; //*** Mpeg4VideoEncoderVcm ***//
    using SharpAvi.Output; //*** AviWriter ***//

    using System.Windows.Forms; //*** Screen ***//

    #endregion

    /// <summary>
    /// <para>Used to Configure the instances of VideoRecorder. This class cannot be inherited.</para>
    /// </summary>
    internal sealed class RecorderParameters
    {
        #region Constructor

        /// <summary>
        /// <para>Initializes a new instance of the VideoRecordingParameters class.</para>
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="FrameRate"></param>
        /// <param name="Encoder">Represents four character code (FOURCC).</param>
        /// <param name="Quality"></param>
        public RecorderParameters(string filename, int FrameRate, FourCC Encoder, int Quality)
        {

            FileName = filename;

            FramesPerSecond = FrameRate;

            Codec = Encoder;

            this.Quality = Quality;

            //--Gets or sets the height of this Rectangle structure.--//
            Height = Screen.PrimaryScreen.Bounds.Height;

            //--Gets or sets the Width of this System.Drawing.Rectangle structure.--//
            Width = Screen.PrimaryScreen.Bounds.Width;
        }

        #endregion

        #region Field

        /// <summary>
        /// <para>The name of a file.</para>
        /// </summary>
        string FileName;

        /// <summary>
        /// <para>The number of frames per second</para>
        /// </summary>
        public int FramesPerSecond;

        /// <summary>
        /// <para></para>
        /// </summary>
        public int Quality;

        /// <summary>
        /// <para></para>
        /// </summary>
        FourCC Codec;

        /// <summary>
        /// <para>How high this is.</para>
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// <para>How wide this is.</para>
        /// </summary>
        public int Width { get; private set; }

        #endregion

        #region Method

        /// <summary>
        /// <para>Create AVI writer and specify FPS, EmitIndex.</para>
        /// <para>Referenced in line 43 of VideoRecorder.cs</para>
        /// </summary>
        /// <returns></returns>
        public AviWriter CreateAviWriter()
        {
            //-- Creates a new instance of AviWriter for writing to a file. --//
            return new AviWriter(FileName)
            {
                //-- The Frame rate.--//
                FramesPerSecond = FramesPerSecond,
                //-- Whether to emit index used in AVI v1 format. --//
                EmitIndex1 = true,

            };
        }

        /// <summary>
        /// <para>Provides Implementation of an IAviVideoStream.</para>
        /// <para>Referenced in line 45 of VideoRecorder.cs</para>
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public IAviVideoStream CreateVideoStream(AviWriter writer)
        {
            // Select encoder type based on FOURCC of codec
            if (Codec == KnownFourCCs.Codecs.Uncompressed)
            {
                return writer.AddUncompressedVideoStream(Width, Height);
            }
            else if (Codec == KnownFourCCs.Codecs.MotionJpeg)
            {
                return writer.AddMotionJpegVideoStream(Width, Height, Quality);
            }
            else
            {
                int width = Width;
                int height = Height;
                double fps = (double)writer.FramesPerSecond;

                //-- Adds new video stream with SharpAvi.Codecs.Mpeg4VideoEncoderVcm. --//
                return writer.AddMpeg4VideoStream(width, height, fps, quality: Quality, codec: Codec, forceSingleThreadedAccess: true);

                #region Crap

                //// Adds new video stream with SharpAvi.Codecs.Mpeg4VideoEncoderVcm.
                //return writer.AddMpeg4VideoStream(
                //    //Width,
                //    width,
                //    Height,
                //    (double)writer.FramesPerSecond,
                //    // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                //    // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                //    quality: Quality,
                //    codec: Codec,
                //    // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                //    // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                //    forceSingleThreadedAccess: true
                //);

                #endregion

            }
        }

        /// <summary>
        /// <para>Audio stream of AVI file.</para>
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public IAviAudioStream CreateAudioStream(AviWriter writer)
        {
            // Select encoder type based on FOURCC of codec
            if (Codec == KnownFourCCs.Codecs.Uncompressed)
            {
                int channelCount = 1;
                int samplesPerSecond = 44100;
                int bitsPerSample = 16;

                //-- Adds new audio stream. --//
                return writer.AddAudioStream(channelCount, samplesPerSecond, bitsPerSample);
            }
            else if (Codec == KnownFourCCs.Codecs.MotionJpeg)
            {
                //-- --//
                return writer.AddAudioStream(1, 44100, 16);
            }
            else
            {
                //-- Adds new video stream with SharpAvi.Codecs.Mpeg4VideoEncoderVcm. --//
                int channelCount = 1;
                int samplesPerSize = 44100;
                int bitsPerSample = 16;

                //-- --//
                return writer.AddAudioStream(channelCount, samplesPerSize, bitsPerSample);
            }
        }

        #endregion
    }
}
