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

    // https://en.wikipedia.org/wiki/Audio_Video_Interleave
    // https://stackoverflow.com/questions/60238492/screen-recording-as-video-audio-on-sharpavi-audio-not-recording
    // https://github.com/baSSiLL/SharpAvi/issues/33

    #region Include

    using System; //*** InvalidOperationException  ***//
    using SharpAvi; //*** KnownFourCCsCodex ***//

    #endregion

    /// <summary>
    /// <para>Contains functionality for recording a users screen in windows.</para>
    /// </summary>
    public static class VideoContext
    {
        #region Constructor

        /// <summary>
        /// <para>This is initialized automatically by the CLR before Main(string[] args)</para>
        /// </summary>
        static VideoContext()
        {
            _recording = false;
        }

        #endregion

        #region Field

        /// <summary>
        /// <para>Used to Configure the instances of VideoRecorder.</para>
        /// </summary>
        private static RecorderParameters _recorderParams;

        /// <summary>
        /// <para>Provides functionality for recording my screen!</para>
        /// </summary>
        private static VideoRecorder _videoRecorder;

        /// <summary>
        /// <para>A value indicating if RecordWindowsVideo(string filepath) has been called.</para>
        /// </summary>
        private static bool _recording;

        #endregion

        #region Method

        /// <summary>
        /// <para>Starts recording a windows video file.</para>
        /// </summary>
        /// <param name="filepath"></param>
        public static void RecordWindowsVideo(string filepath)
        {
            //-- Initializes a new instance of the InvalidOperationException class with a specified error message.--//
            if (_recording) throw new InvalidOperationException("Cannot Called Start () concurently twice");

            //--Using MotionJpeg as Avi encoder, output to 'out.avi' at 10 Frames per second, 70% quality--//



            //-- Initializes a new instance of the RecorderParams class. --//

            string filename = filepath;
            int frameRate = 10;
            var encoder = KnownFourCCs.Codecs.MotionJpeg;
            int quality = 70;

            //VideoRecordingParameters recorderParams = new VideoRecordingParameters(filename, frameRate, encoder, quality);

            //-- Initializes a new instance of the VideoRecordingParameters class. --//
            _recorderParams = new RecorderParameters(filename, frameRate, encoder, quality);

            //-- Provides functionality for recording my screen! --//
            _videoRecorder = new VideoRecorder(_recorderParams);

            //-- A value indicating that RecordWindowsVideo(string filepath) has been called. --//
            _recording = true;
        }

        /// <summary>
        /// <para>Stop recording the windows video file.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static void StopRecording()
        {
            //-- Initializes a new instance of the InvalidOperationException class with a specified error message.--//
            if (!_recording) throw new InvalidOperationException("Call StartRecord()");

            //-- Provides implementation for this.Dispose() --//
            _videoRecorder.Dispose();
        }

        #endregion


    }
}
