using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Runtime.Remoting.Channels;


namespace CASSIE_Maker
{
    public class Phoneme
    {
        public string name;
        public SoundPlayer soundPlayer;
        public string location;
        public TimeSpan duration;

        public Phoneme(string name, string location)
        {
            this.name = name;
            this.location = location;

            this.soundPlayer = new SoundPlayer();
            soundPlayer.SoundLocation = this.location;

            WaveFileReader waveFileReader = new WaveFileReader(location);
            this.duration = waveFileReader.TotalTime;
            waveFileReader.Dispose();
        }

        public void Play()
        {
            this.soundPlayer.Play();
        }
    }
    public partial class Form1 : Form
    {
        Dictionary<string, Phoneme> phonemesSounds = new Dictionary<string, Phoneme>();
        Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            load_phonemes_sounds();

            richTextBox1.Text = "The quick brown fox jumps over the lazy dog";

            // Read file with all words
            string[] dict = File.ReadAllText(@"C:\Users\Altin\OneDrive\Pulpit\cmu_dict.txt").Split('\n');

            string word;
            string[] phonemes;

            // Add all words with pronunciations to dictionary
            foreach (string line in dict)
            {
                // Try beacause of 5 the same keys of items
                try
                {
                    // Get word and phonemes from line and add them
                    word = line.Trim().Split(null, 2)[0].Trim();
                    phonemes = line.Trim().Split(null, 2)[1].Split(null);
                    phonemes = phonemes.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                    dictionary.Add(word,phonemes);
                }
                catch (ArgumentException er)
                {
                    Console.WriteLine(er.Message);
                }
            }
            
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string text = richTextBox1.Text.Trim().Replace('\n',' ');
            string[] meaningParts = text.Split(' ');
            meaningParts = meaningParts.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            Task playSentence = Task.Factory.StartNew(() =>
            {
                foreach (string part in meaningParts)
                {
                    if (part.Trim().Length <= 0)
                        continue;
                    word_player(part.ToUpper());
                    Thread.Sleep(100);
                }
            });
            
            
        }


        private void word_player(string word)
        {
            string[] phonemesNames;
            if (!dictionary.TryGetValue(word, out phonemesNames))
                return;

            for (int i = 0; i < phonemesNames.Length; i++)
            {
                phonemesSounds[phonemesNames[i]].Play();
                Thread.Sleep((int) Math.Round(phonemesSounds[phonemesNames[i]].duration.Milliseconds / 1.5));
            } 
        }

        private void load_phonemes_sounds()
        {
            string[] phonemesNames = new string[]
            {
                "AA",
                "AE",
                "AH",
                "AO",
                "AW",
                "AY",
                "B",
                "CH",
                "D",
                "DH",
                "EH",
                "ER",
                "EY",
                "F",
                "G",
                "HH",
                "IH",
                "IY",
                "JH",
                "K",
                "L",
                "M",
                "N",
                "NG",
                "OW",
                "OY",
                "P",
                "R",
                "S",
                "SH",
                "T",
                "TH",
                "UH",
                "UW",
                "V",
                "W",
                "Y",
                "Z",
                "ZH"
            };

            foreach(string phonemeName in phonemesNames)
            {
                string location = @"D:\\!Downloads\\SCP_SL-20221206T161301Z-001\\Words_Numbers_Letters\\Phonemes_CMU\\" + phonemeName + ".wav";

                phonemesSounds.Add(phonemeName, new Phoneme(phonemeName, location));
            }
        }
    }
}
