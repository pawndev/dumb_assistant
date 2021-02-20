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
using System.Windows.Forms;
using System.Windows.Threading;
using System.Globalization;
using System.Drawing;
using speech_to_binding.Services;
using speech_to_binding.Models;
using speech_to_binding.Manager;
using System.Reflection;
using speech_to_binding.Manager.UtilDebug;

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

        private NotifyIcon notifyIcon;

        private ConfigurationInterpreter _defaultConfigurationInterpreter;
        private ConfigurationInterpreter _sampleConfigurationInterpreter;
        private ConfigurationManager _configurationManager;

        public SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        public SpeechRecognitionEngine listener = new SpeechRecognitionEngine();

        public DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public int recordTimeOut = 0;
        public DateTime now = DateTime.Now;
        private Random _rnd = new Random();

        public List<string> commandListItems = new List<string>();
        public List<GridLength> originalColumnSize = new List<GridLength>();

        public Uri plainIconUri = new Uri("pack://application:,,,/speech_to_binding;component/Resources/baseCircle.ico", UriKind.Absolute);
        public Uri baseIconUri = new Uri("pack://application:,,,/speech_to_binding;component/Resources/plainCircle.ico", UriKind.Absolute);
        public ImageSource plainIcon = new BitmapImage(new Uri("pack://application:,,,/speech_to_binding;component/Resources/baseCircle.ico", UriKind.Absolute));
        public ImageSource baseIcon  = new BitmapImage(new Uri("pack://application:,,,/speech_to_binding;component/Resources/plainCircle.ico", UriKind.Absolute));

        public MainWindow()
        {
            InitializeComponent();

            this._configurationManager = new ConfigurationManager();

            byte[] bufferDefaultConfig = Properties.Resources.default_command;
            string defaultConfigStr = Encoding.UTF8.GetString(bufferDefaultConfig, 0, bufferDefaultConfig.Length);
            this._defaultConfigurationInterpreter = new ConfigurationInterpreter(defaultConfigStr);
            Configuration defaultConfig = this._defaultConfigurationInterpreter.GetConfiguration();
            this._configurationManager.Add("default", defaultConfig);

            byte[] bufferSampleConfig = Properties.Resources.sample_config;
            string sampleConfigStr = Encoding.UTF8.GetString(bufferSampleConfig, 0, bufferSampleConfig.Length);
            this._sampleConfigurationInterpreter = new ConfigurationInterpreter(sampleConfigStr);
            Configuration userConfig = this._sampleConfigurationInterpreter.GetConfiguration();
            this._configurationManager.Add("user", userConfig);

            string[] phrases = this._configurationManager.GetAllMatch().Select(x => x.phrase).ToArray();
            string[] commands = File.ReadAllLines(@"commands.txt");
            string[] allPhrases = phrases.Concat(commands).ToArray();

            foreach (string command in allPhrases)
            {
                this.commandList.Items.Add(command);
            }

            WindowManager windowManager = new WindowManager(this);
            ContextManager context = new ContextManager(windowManager, this._configurationManager);
            Alice alice = new Alice(allPhrases, context);
            //alice.
            
            //this.notifyIcon = new NotifyIcon();
            //this.notifyIcon.Icon = Properties.Resources.icon; // new Icon(System.Windows.Application.GetResourceStream(this.baseIconUri).Stream);
            //this.notifyIcon.MouseDoubleClick +=
            //    new System.Windows.Forms.MouseEventHandler
            //        (NotifyIcon_MouseDoubleClick);
        }

        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                this.notifyIcon.BalloonTipTitle = "Minimize Sucessful";
                this.notifyIcon.BalloonTipText = "Minimized the app ";
                this.notifyIcon.ShowBalloonTip(400);
                this.notifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.notifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }
    }
}
