using Project_pac.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_pac
{
    public partial class GameLoad : Form
    {


        // StartPostion 을 윈폼에서 CenterScreen 으로 설정해서 화면 정중앙에 프로그램이 시작됨. << 참고
        public GameLoad()
        {
            InitializeComponent();
        }

        private void GameLoad_Load(object sender, EventArgs e)
        {
            MessageBox.Show("made by. 윈도우 프로그래밍 8조");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.FormClosed += (s, args) => this.Show(); // Form1이 닫힐 때 GameLoad를 다시 표시
            form1.Show();
            this.Hide(); // 일시적으로 숨기는 방안 채택, 왜인지 모르겠으나 Close를 사용하면 둘 다 닫히는 버그 존재.
                         // 일단 버그 우회하는 방법으로 선택.
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
