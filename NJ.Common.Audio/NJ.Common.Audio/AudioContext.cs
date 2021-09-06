namespace NJ.Common.Audio
{
    //--------------------------------------------------------------------//
    //                                                          C l a s s //
    // A u d i o C o n t e x t                                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // http://cscore.codeplex.com/                                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    #region Include

    using System; //*** Environment InvalidOperationException ***//

    using CSCore.SoundIn; //*** WasapiLoopbackCapture ***//
    using CSCore.Codecs.WAV; //*** WaveWriter ***//

    #endregion

    /// <summary>
    /// <para>Contains functionality for recording windows audio.</para>
    /// </summary>
    public static class AudioContext
    {
        #region Constructor

        /// <summary>
        /// <para></para>
        /// </summary>
        static AudioContext()
        {
            _recording = false;
        }

        #endregion

        #region Field

        /// <summary>
        /// <para>Captures audio data from a audio device (through Wasapi Apis). To capture audio</para>
        /// <para>from an output device, use the WasapiLoopbackCapture class. Minimum</para>
        /// <para>supported OS: Windows Vista (see CSCore.SoundIn.WasapiCapture.IsSupportedOnCurrentPlatform</para>
        /// <para>property).</para>
        /// </summary>
        private static WasapiCapture _capture;

        /// <summary>
        /// <para>Encoder for wav files.</para>
        /// </summary>
        private static WaveWriter _waveWriter;

        /// <summary>
        /// <para>A value indicating if StartRecording has been called.</para>
        /// </summary>
        private static bool _recording;

        #endregion

        #region Method

        /// <summary>
        /// <para>Stop recording windows audio.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static void StopRecording()
        {
            //-- Initializes a new instance of the InvalidOperationException class. --//
            if (!_recording) throw new InvalidOperationException("please call StopRecording before calling this method.");

            //-- Stop recording. --//
            _capture.Stop();

            //-- Indicate that no recording is taking place. --//
            _recording = false;
            
            //-- Disposes the WaveWriter and writes down the wave header. --//
            _waveWriter.Dispose();
        }

        /// <summary>
        /// <para>Start recording windows audio output to the specified filepath.</para>
        /// </summary>
        /// <param name="filepath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void StartRecording(string filepath)
        {
            //-- Initializes a new instance of the System.InvalidOperationException class with a specified error message. --//
            if (_recording) throw new InvalidOperationException("RecordWindowsAudio called twice, please call StopRecording before calling this method.");

            //-- Initializes a new instance of the WasapiLoopbackCapture class. --//
            _capture = new WasapiLoopbackCapture();

            //-- Initializes WasapiCapture and prepares all resources for recording. Note that  properties like Device, etc. won't affect WasapiCapture after calling Initialize. --//
            _capture.Initialize();

            //-- Indicate that recording has begun. --//
            _recording = true;

            //-- Initializes a new instance of the WaveWriter class. --//
            _waveWriter = new WaveWriter(filepath, _capture.WaveFormat);

            //-- setup an eventhandler to receive the recorded data --//
            _capture.DataAvailable += (s, e) =>
            {
                //--save the recorded audio--//
                _waveWriter.Write(e.Data, e.Offset, e.ByteCount);
            };

            //-- Start recording --//
            _capture.Start();
        }

        #endregion
    }
}
