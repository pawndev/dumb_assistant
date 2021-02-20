using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using speech_to_binding.Models;

namespace speech_to_binding.Manager
{
    public class Alice
    {
        public const string ZIRA_VOICE = "Microsoft Zira Desktop";

        private string _voice = ZIRA_VOICE;

        private ConfigurationManager _configManager;
        private ContextManager context;

        private SpeechRecognitionEngine _aliceRecognizer = new SpeechRecognitionEngine();
        private SpeechRecognitionEngine _aliceListener = new SpeechRecognitionEngine();

        private SpeechSynthesizer _aliceSynthesizer = new SpeechSynthesizer();

        private Choices _recognizerChoices = new Choices();
        private GrammarBuilder _recognizerGrammarBuilder;
        private Grammar _recognizerGrammar;

        private Choices _listenerChoices = new Choices();
        private GrammarBuilder _listenerGrammarBuilder;
        private Grammar _listenerGrammar;

        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private int recordTimeOut = 0;
        private DateTime now = DateTime.Now;
        private Random _rnd = new Random();

        public string Voice
        {
            get
            {
                return this._voice;
            }
            set
            {
                this._voice = value;
                this._aliceSynthesizer.SelectVoice(value);
            }
        }

        public Alice(string[] phrases) : this(phrases, null, ZIRA_VOICE) { }
        public Alice(string[] phrases, ContextManager contextManager) : this(phrases, contextManager, ZIRA_VOICE) { }

        public Alice(string[] phrases, ContextManager contextManager, string voice)
        {
            this.context = contextManager;
            this._dispatcherTimer.Tick += new EventHandler(_dispatcherTimer_Tick);
            this._dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
            this._dispatcherTimer.Start();

            this.Voice = voice;

            this._listenerChoices.Add(phrases);
            this._listenerGrammarBuilder = new GrammarBuilder(this._listenerChoices);
            this._listenerGrammar = new Grammar(this._listenerGrammarBuilder);

            this._recognizerChoices.Add(phrases);
            this._recognizerGrammarBuilder = new GrammarBuilder(this._recognizerChoices);
            this._recognizerGrammar = new Grammar(this._recognizerGrammarBuilder);

            this._aliceRecognizer.SetInputToDefaultAudioDevice();
            this._aliceRecognizer.LoadGrammarAsync(this._recognizerGrammar);
            this._aliceRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            this._aliceRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Default_SpeechRecognized);
            this._aliceRecognizer.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(_aliceRecognizer_SpeechRecognized);

            this._aliceListener.SetInputToDefaultAudioDevice();
            this._aliceListener.LoadGrammarAsync(this._listenerGrammar);
            this._aliceListener.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_aliceListener_SpeechRecognized);

            this._aliceSynthesizer.SetOutputToDefaultAudioDevice();
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (this.recordTimeOut == 10)
            {
                this._aliceRecognizer.RecognizeAsyncCancel();
            }
            else if (this.recordTimeOut == 11)
            {
                this._dispatcherTimer.Stop();
                this._aliceListener.RecognizeAsync(RecognizeMode.Multiple);
                this.recordTimeOut = 0;
            }
        }

        private void _aliceListener_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string sentence = e.Result.Text;

            if (sentence == "Wake up")
            {
                this._aliceListener.RecognizeAsyncCancel();
                this._aliceSynthesizer.SpeakAsync("I am here");
                this._aliceRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void Default_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string sentence = e.Result.Text;
            this.context.windowManager.LastCommand = sentence;

            int rnd;

            if (sentence == "Hello")
            {
                this._aliceSynthesizer.SpeakAsync("Hi");
            }

            if (sentence == "What time is it")
            {
                this._aliceSynthesizer.SpeakAsync(DateTime.Now.ToString("h mm tt"));
            }

            //if (sentence == "Test")
            //{
            //    synthesizer.SpeakAsync("She had said I’m tired of begging God to overthrow my son, because all this business of living in the presidential palace is like having the lights on all the time, sir, and she had said it with the same naturalness with which on one national holiday she had made her way through the guard of honor with a basket of empty bottles and reached the presidential limousine that was leading the parade of celebration in an uproar of ovations and martial music and storms of flowers and she shoved the basket through the window and shouted to her son that since you’ll be passing right by take advantage and return these bottles to the store on the corner, poor mother.");
            //}

            if (sentence == "Stop talking")
            {
                this._aliceSynthesizer.SpeakAsyncCancelAll();
                rnd = this._rnd.Next(1, 2);

                if (rnd == 1)
                {
                    this._aliceSynthesizer.SpeakAsync("Sure");
                }

                if (rnd == 2)
                {
                    this._aliceSynthesizer.SpeakAsync("Okay");
                }
            }

            if (sentence == "Stop listening")
            {
                this._aliceSynthesizer.SpeakAsync("Call me when you want");
                this._aliceRecognizer.RecognizeAsyncCancel();
                this._aliceListener.RecognizeAsync(RecognizeMode.Multiple);
            }

            if (sentence == "Show Commands")
            {
                WindowAction actionResult = this.context.windowManager.ShowCommands();
                if (actionResult == WindowAction.ALREADY_DONE)
                {
                    this._aliceSynthesizer.SpeakAsync("Already done");
                }
                else
                {
                    this._aliceSynthesizer.SpeakAsync("Sure");
                }
            }

            if (sentence == "Unshow Commands")
            {
                WindowAction actionResult = this.context.windowManager.UnshowCommands();
                if (actionResult == WindowAction.ALREADY_DONE)
                {
                    this._aliceSynthesizer.SpeakAsync("Already done");
                }
                else
                {
                    this._aliceSynthesizer.SpeakAsync("Sure");
                }
            }

            if (sentence == "Kill yourself")
            {
                this._aliceSynthesizer.Speak("Good bye");
                this.context.windowManager.Kill();
            }

            if (sentence == "Minimize window")
            {
                this._aliceSynthesizer.SpeakAsync("Minimized");
                this.context.windowManager.Minimize();
            }

            if (sentence == "Maximize window")
            {
                this._aliceSynthesizer.SpeakAsync("Maximized");
                this.context.windowManager.Maximize();
            }

            if (sentence == "Normal window")
            {
                this._aliceSynthesizer.SpeakAsync("Normal");
                this.context.windowManager.Normal();
            }

            ConfigurationMatch match = this.context.configManager.GetAllMatch().Find(x => x.phrase.Contains(sentence));

            if (match != null)
            {
                foreach (ConfigurationMatchAction action in match.actions)
                {
                    Console.WriteLine(action);
                    switch (action.type)
                    {
                        case ActionType.SPEAK:
                            this._aliceSynthesizer.SpeakAsync(action.message);
                            break;
                        case ActionType.CALL_METHOD:
                            Type thisType = this._aliceSynthesizer.GetType();
                            MethodInfo theMethod = thisType.GetMethod(action.function);
                            theMethod.Invoke(this._aliceSynthesizer, null);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("sentence :");
                Console.WriteLine(sentence);
            }
        }

        private void _aliceRecognizer_SpeechRecognized(object sender, SpeechDetectedEventArgs e)
        {
            this.recordTimeOut = 0;
        }
    }
}
