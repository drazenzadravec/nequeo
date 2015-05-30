using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Nequeo.IO.Audio.WavePlayer player = null;
        private void button1_Click(object sender, EventArgs e)
        {
            Nequeo.IO.Audio.Device device = Nequeo.IO.Audio.Devices.GetDevice(0);
            player = new Nequeo.IO.Audio.WavePlayer(device);

            //player.Open("http://video.ch9.ms/ch9/ea15/41fd375a-c978-4322-9f0d-150d48f0ea15/GN34Superbowl2.mp3");
            //player.Open(@"C:\Users\PC\Music\Ludwig Van Beethoven\Symphony No  6 In F Major, Op  68  Pastoral Symphony  Recollections of Country Life  - Single\01 Symphony No  6 In F Major, Op  68  Pastoral Symphony  Recollections of Country Life.mp3");
            //player.Open(@"C:\Temp\Capture\data.wav");

            System.IO.Stream gg = new System.IO.FileStream(@"C:\Temp\Capture\tt_0.wav", System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //System.IO.Stream gg = new System.IO.FileStream(@"C:\Users\PC\Music\Ludwig Van Beethoven\Symphony No  6 In F Major, Op  68  Pastoral Symphony  Recollections of Country Life  - Single\01 Symphony No  6 In F Major, Op  68  Pastoral Symphony  Recollections of Country Life.mp3", System.IO.FileMode.Open, System.IO.FileAccess.Read);
            player.Open(gg, Nequeo.IO.Audio.AudioFormat.Wav);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            player.Volume = 1.0F;
            player.Play();
        }
    }
}
