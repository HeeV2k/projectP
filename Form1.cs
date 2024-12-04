using System;
using System.Drawing;
using System.Windows.Forms;

namespace Project_pac.UI
{
    public partial class Form1 : Form
    {
        private GameLogic gameLogic;
        private Timer gameTimer;     // 게임 루프를 위한 타이머
        private GameMap gameMap;     // 게임 맵

        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;  // 화면 깜빡임 방지 더블 버퍼 사용.

            // 게임 로직 및 맵 초기화
            gameLogic = new GameLogic();
            gameMap = new GameMap();

            // 폼 크기 설정 (맵 크기에 맞추기)
            // 폼 크기를 윈폼 자체에서 조정하기 힘든 부분이 있어서
            // gameMap에서 직접 따와서 MapWidth와 CellSize를 곱하여 가로 길이와 세로 길이를 도출해내고 적용하는 방법을 채택함.
            this.ClientSize = new Size(gameMap.MapWidth * gameMap.CellSize, gameMap.MapHeight * gameMap.CellSize);

            // 게임 타이머 설정 (초당 60번 업데이트)
            // 144로 설정해봤더니 게임 속도가 빨라짐.
            gameTimer = new Timer();
            gameTimer.Interval = 1000 / 60; // 60FPS
            gameTimer.Tick += (sender, e) =>
            {
                gameLogic.Update(); // 게임 상태 업데이트
                Invalidate();        // 화면 다시 그리기
            };
            gameTimer.Start();

            // 키 입력 처리
            this.KeyDown += (sender, e) => gameLogic.KeyDown(e.KeyCode);
            this.KeyUp += (sender, e) => gameLogic.KeyUp(e.KeyCode);
        }

        // 화면 그리기 (Paint 이벤트)
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            gameLogic.Draw(e.Graphics); // 게임 상태 그리기
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
