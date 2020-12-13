using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OsuSongPathSearcher;
using Osu_BeatmapDataReader;
using System.IO;
using System.Threading;

namespace osu__beatmap_reader
{
    public partial class Form1 : Form
    {
        Osu_BeatmapDataReader.BeatmapReader beatmapReader = new BeatmapReader();
        OsuSongPathSearcher.Class1 ss = new OsuSongPathSearcher.Class1();
        string path, CurrentBeatmap, BeatmapFile, ApplicationPath;
        

       
        ListView lvB = new ListView();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            
            timer1.Start();
            ss.GetOsuSongPath();
            textBox4.Text = ss.OsuSongPath;
            path = ss.OsuSongPath;
            GetSongs();
            label7.Text = "Beatmaps: " + lvB.Items.Count;
            label8.Text = beatmapReader.DllInfo + Environment.NewLine + ss.DllInfo;
            
            ApplicationPath = "C:\\" + Environment.SpecialFolder.ProgramFiles + "\\osu!BeatmapReader";
          
           
            Directory.CreateDirectory(ApplicationPath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadBeatmap();
        }

        private void GetSongs()
        {
            label6.Visible = false;
            listBox2.Items.Clear();
           
            lvB.Items.Clear();
           

            try
            {
                string[] filelist;

                filelist = Directory.GetDirectories(path);

                foreach (string s in filelist)
                {
                    string[] TempPath;
                 
                    TempPath = Directory.GetFiles(s);
                    listBox2.Items.Add(s.Replace(path + "\\","")); ;
                    label2.Text = "Beatmap Folders: " + listBox2.Items.Count;
                    foreach (String a in TempPath)
                    {
                        if (a.Contains(".osu"))
                        {
                            lvB.Items.Add(a);
                            
                        }
                    }

                }
            }
            catch { label6.Text = "Error: Invailid Path" + Environment.NewLine + "(" + path + ")"; label6.Visible = true; }
            label7.Text = "Beatmaps: " + lvB.Items.Count;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox4.Text = folderBrowserDialog1.SelectedPath;
            path = folderBrowserDialog1.SelectedPath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            GetSongs();
            textBox5.Text = "";
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();
                string Temp;
                string[] bm;
                Temp = listBox2.GetItemText(listBox2.SelectedItem);

                bm = Directory.GetFiles(path + "\\" + Temp);
                CurrentBeatmap = path + "\\" + Temp;
                foreach (string s in bm)
                {

                    if (s.Contains(".osu"))
                    {
                        beatmapReader.BeatmapPath = s;
                        beatmapReader.GetBeatmapData();
                        listBox1.Items.Add(beatmapReader.Version);

                    }
                }


            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] filelist1;
            filelist1 = Directory.GetFiles(CurrentBeatmap);
            try
            {
               
               
                foreach (string s in filelist1)
                    if (s.Contains(listBox1.GetItemText(listBox1.SelectedItem)))
                    {
                        BeatmapFile = s;
                      
                    }
              
            }

            catch { label2.Text = "Error: Invailid Path" + Environment.NewLine + "(" + path + ")"; }
           
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            Thread th = new Thread(Search);

            textBox5.Enabled = false;
                th.Start();

            
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            

            textBox3.Text = BeatmapFile;
           
        }

       

        

        private void button4_Click(object sender, EventArgs e)
        {
            path = textBox4.Text;
            MessageBox.Show("New osu!song path set to: " + Environment.NewLine + path + Environment.NewLine + Environment.NewLine + "Click 'Refresh List' to reload list with new path. ","Info",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

      

        private void Search()
        {
            

                try
                {
                listBox1.Items.Clear();
                string[] TempPath;
                lvB.Items.Clear();
                listBox2.Items.Clear();

                string[] filelist1;
                    filelist1 = Directory.GetDirectories(path);
                    foreach (string s in filelist1)
                        if (s.Contains(textBox5.Text))
                        {
                        listBox2.Items.Add(s.Replace(path + "\\", ""));

                           label2.Text = "Beatmap Folders: " + listBox2.Items.Count;
                           
                           TempPath = Directory.GetFiles(s);
                           foreach (String a in TempPath)
                           {


                            if (a.Contains(".osu"))
                            {
                                lvB.Items.Add(a);

                            }


                           }
                        }
                        else
                        {
                            if (listBox2.Items.Count == 0)
                            {
                                label2.Text = "Beatmap Folders: " + listBox2.Items.Count;
                            }
                        }
                }

                catch { label2.Text = "Error: Invailid Path" + Environment.NewLine + "(" + path + ")"; }
            label7.Text = "Beatmaps: " + lvB.Items.Count;
            textBox5.Enabled = true;
            textBox5.Focus();
        }

        public void ReadBeatmap()
        {
            
            try
            {

                dataGridView1.Rows.Clear();
                string Nr = "", X = "", Y = "", Time = "", Type = "", line;
                beatmapReader.BeatmapPath = textBox3.Text;
                beatmapReader.GetBeatmapData();
                textBox1.Text = beatmapReader.RAW;
                textBox2.Text = beatmapReader.RawClean;
                string[] SplitString;

                string temp = beatmapReader.Hitobjects;
                FileStream fs1 = new FileStream(ApplicationPath + "\\Temp_Hitobjects.txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs1);
                sw.Write(temp);
                fs1.Close();
                FileStream fs = new FileStream(ApplicationPath + "\\Temp_Hitobjects.txt", FileMode.Open);
                StreamReader sr = new StreamReader(fs);


                while (sr.Peek() != -1)
                {
                    line = sr.ReadLine();
                    SplitString = line.Split(';');
                    for (int i = 0; i < SplitString.Length; i++)
                    {
                        if (line.Contains("Sliders"))
                        {
                            if (i == 0) { Nr = SplitString[i].Replace("Nr.", ""); }
                            if (i == 1) { Type = SplitString[i]; dataGridView1.Rows.Add(Nr, "", "", "", Type); }
                        }
                        else
                        {

                            if (i == 0) { Nr = SplitString[i].Replace("Nr.", ""); }
                            if (i == 1) { X = SplitString[i]; }
                            if (i == 2) { Y = SplitString[i]; }
                            if (i == 3) { Time = SplitString[i]; }
                            if (i == 4) { Type = SplitString[i]; dataGridView1.Rows.Add(Nr, X, Y, Time, Type); }
                        }
                    }
                }
                sr.Close();
                
                
               
            }
            catch (Exception ex) { }
        }
    }
}

