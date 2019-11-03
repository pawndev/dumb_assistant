using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using System.IO;
using System.Windows.Threading;
using System.Globalization;

namespace speech_to_binding
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        private Choices _recognizerChoices;
        private GrammarBuilder _recognizerBuilder;
        private Grammar _recognizerGrammar;
        private Choices _listenerChoices;
        private GrammarBuilder _listenerBuilder;
        private Grammar _listenerGrammar;
        public SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        public SpeechRecognitionEngine listener = new SpeechRecognitionEngine();
        public DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        public int recordTimeOut = 0;
        public DateTime now = DateTime.Now;
        private Random _rnd = new Random();
        public List<string> commandListItems = new List<string>();
        public List<GridLength> originalColumnSize = new List<GridLength>();

        public MainWindow()
        {
            /* using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {

                // Output information about all of the installed voices.   
                Console.WriteLine("Installed voices -");
                foreach (InstalledVoice voice in synth.GetInstalledVoices())
                {
                    VoiceInfo info = voice.VoiceInfo;
                    string AudioFormats = "";
                    foreach (SpeechAudioFormatInfo fmt in info.SupportedAudioFormats)
                    {
                        AudioFormats += String.Format("{0}\n",
                        fmt.EncodingFormat.ToString());
                    }

                    Console.WriteLine(" Name:          " + info.Name);
                    Console.WriteLine(" Culture:       " + info.Culture);
                    Console.WriteLine(" Age:           " + info.Age);
                    Console.WriteLine(" Gender:        " + info.Gender);
                    Console.WriteLine(" Description:   " + info.Description);
                    Console.WriteLine(" ID:            " + info.Id);
                    Console.WriteLine(" Enabled:       " + voice.Enabled);
                    if (info.SupportedAudioFormats.Count != 0)
                    {
                        Console.WriteLine(" Audio formats: " + AudioFormats);
                    }
                    else
                    {
                        Console.WriteLine(" No supported audio formats found");
                    }

                    string AdditionalInfo = "";
                    foreach (string key in info.AdditionalInfo.Keys)
                    {
                        AdditionalInfo += String.Format("  {0}: {1}\n", key, info.AdditionalInfo[key]);
                    }

                    Console.WriteLine(" Additional Info - " + AdditionalInfo);
                    Console.WriteLine();
                }
            } */

            InitializeComponent();

            string[] commands = File.ReadAllLines(@"commands.txt");
            foreach (string command in commands)
            {
                Console.WriteLine(command);
                //this.commandListItems.Add(command);
                this.commandList.Items.Add(command);
            }

            this.dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
            this.dispatcherTimer.Start();

            this._recognizerChoices = new Choices(File.ReadAllLines(@"commands.txt"));
            this._recognizerBuilder = new GrammarBuilder(this._recognizerChoices);
            this._recognizerGrammar = new Grammar(this._recognizerBuilder);

            this._listenerChoices = new Choices(File.ReadAllLines(@"commands.txt"));
            this._listenerBuilder = new GrammarBuilder(this._listenerChoices);
            this._listenerGrammar = new Grammar(this._listenerBuilder);

            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammarAsync(this._recognizerGrammar);
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Default_SpeechRecognized);
            _recognizer.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(_recognizer_SpeechRecognized);
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);

            listener.SetInputToDefaultAudioDevice();
            listener.LoadGrammarAsync(this._listenerGrammar);
            listener.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(listener_SpeechRecognized);

            synthesizer.SetOutputToDefaultAudioDevice();

            synthesizer.SelectVoice("Microsoft Zira Desktop");

            foreach (ColumnDefinition col in this.mainGrid.ColumnDefinitions)
            {
                this.originalColumnSize.Add(col.Width);
            }

            // or this
            // synthesizer.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.NotSet, 0, CultureInfo.GetCultureInfo("en-us"));
        }

        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (this.recordTimeOut == 10)
            {
                this._recognizer.RecognizeAsyncCancel();
            } 
            else if (this.recordTimeOut == 11)
            {
                this.dispatcherTimer.Stop();
                this.listener.RecognizeAsync(RecognizeMode.Multiple);
                this.recordTimeOut = 0;
            }
        }

        public void listener_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string sentence = e.Result.Text;

            if (sentence == "Wake up")
            {
                this.listener.RecognizeAsyncCancel();
                this.synthesizer.SpeakAsync("I am here");
                this._recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        public void _recognizer_SpeechRecognized(object sender, SpeechDetectedEventArgs e)
        {
            this.recordTimeOut = 0;
        }

        private void Default_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string sentence = e.Result.Text;
            this.lastCommand.Text = sentence;
            int rnd;

            if (sentence == "Hello")
            {
                synthesizer.SpeakAsync("Hi");
            }

            if (sentence == "What time is it")
            {
                synthesizer.SpeakAsync(DateTime.Now.ToString("h mm tt"));
            }

            if (sentence == "Test")
            {
                synthesizer.SpeakAsync("She had said I’m tired of begging God to overthrow my son, because all this business of living in the presidential palace is like having the lights on all the time, sir, and she had said it with the same naturalness with which on one national holiday she had made her way through the guard of honor with a basket of empty bottles and reached the presidential limousine that was leading the parade of celebration in an uproar of ovations and martial music and storms of flowers and she shoved the basket through the window and shouted to her son that since you’ll be passing right by take advantage and return these bottles to the store on the corner, poor mother.");
            }

            if (sentence == "Stop talking")
            {
                this.synthesizer.SpeakAsyncCancelAll();
                rnd = this._rnd.Next(1, 2);

                if (rnd == 1)
                {
                    this.synthesizer.SpeakAsync("Sure");
                }

                if (rnd == 2)
                {
                    this.synthesizer.SpeakAsync("Okay");
                }
            }

            if (sentence == "Stop listening")
            {
                this.synthesizer.SpeakAsync("Call me when you want");
                this._recognizer.RecognizeAsyncCancel();
                this.listener.RecognizeAsync(RecognizeMode.Multiple);
            }

            if (sentence == "Show Commands")
            {
                if (this.mainGrid.ColumnDefinitions[0].Width == this.originalColumnSize[0])
                {
                    this.synthesizer.SpeakAsync("Already on screen");
                }

                for (int index = 0; index < this.originalColumnSize.Count; index += 1)
                {
                    this.mainGrid.ColumnDefinitions[index].Width = this.originalColumnSize[index];
                }
            }

            if (sentence == "Unshow Commands")
            {
                if (this.mainGrid.ColumnDefinitions[0].Width != this.originalColumnSize[0])
                {
                    this.synthesizer.SpeakAsync("Already done");
                }

                this.mainGrid.ColumnDefinitions[0].Width = new GridLength(this.Width - 5, GridUnitType.Star);
                this.mainGrid.ColumnDefinitions[1].Width = new GridLength(5, GridUnitType.Pixel);
                this.mainGrid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);
            }

            if (sentence == "Kill yourself")
            {
                this.synthesizer.Speak("Good bye");
                Application.Current.Shutdown();
            }
        }
    }
}
