using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class GameLogic
{
    private Pacman pacman;
    private GameMap gameMap;
    private int score;
    private HashSet<Keys> keysPressed = new HashSet<Keys>();
    private bool stageCleared = false; // 스테이지 클리어 상태를 추적하는 변수
    private List<Ghost> ghosts; // 유령 목록
    private Timer ghostTimer; // 유령 움직임 타이머
    private bool ghostsFrozen = true; // 유령이 멈춰있는 상태
    private bool gameOver = false; // 게임 종료 상태를 추적하는 변수
    private bool gamePaused = false; // 게임 일시정지 상태를 추적하는 변수
    private bool canPassGhosts = false; // 유령을 통과할 수 있는 상태를 추적하는 변수
    private Timer passGhostsTimer; // 유령 통과 상태 타이머
    private int changedGhostSpeed = 5;
    private int defaultGhostSpeed = 3;
    private List<Effect> effects; // 점수 아이템 먹을 때 이펙트
    // private List<CircleEffect> circleEffects;
    private Timer effectTimer; // 이펙트 타이머

    public GameLogic()
    {
        pacman = new Pacman(400, 300); // 팩맨 초기 위치 << 나중에 조정 필요. 보통 어디서 나타나는지 조사 요청.
        gameMap = new GameMap();
        score = 0;
        stageCleared = false; // 초기화
        gameOver = false; // 초기화
        gamePaused = false; // 초기화
        canPassGhosts = false; // 초기화

        // 유령 초기화
        ghosts = new List<Ghost>
        {
            new Ghost(220, 200, defaultGhostSpeed),
            new Ghost(220, 200, defaultGhostSpeed),
            new Ghost(620, 400, defaultGhostSpeed),
            new Ghost(620, 400, defaultGhostSpeed)
        };

        // 유령 움직임 타이머 초기화
        ghostTimer = new Timer();
        ghostTimer.Interval = 3000; // 3초 동안 멈춤 << 시작하면 창이 작업표시줄 아래로 내려가서 조정하는 경우 있음. 5초로 늘리기 권장.
        ghostTimer.Tick += (sender, e) =>
        {
            ghostsFrozen = false; 
            ghostTimer.Stop(); // 타이머 멈춤
        };
        ghostTimer.Start();

        // 유령 통과 상태 타이머 초기화
        passGhostsTimer = new Timer();
        passGhostsTimer.Interval = 5000; // 5초 동안 유령 통과 가능 *5초에서 수정 금지. 5초가 적당함.
        passGhostsTimer.Tick += (sender, e) =>
        {
            canPassGhosts = false; // 유령 통과 상태 종료
            foreach (var ghost in ghosts)
            {
                ghost.SetPassable(false); // 유령 통과 가능 상태 해제
            }
            passGhostsTimer.Stop(); // 타이머 멈춤
        };

        // 플러스 이펙트 초기화
        effects = new List<Effect>();

        // 원형 이펙트 초기화 ( 비활성화 )
        // circleEffects = new List<CircleEffect>();

        effectTimer = new Timer();
        effectTimer.Interval = 16; // 약 60FPS
        effectTimer.Tick += (sender, e) => UpdateEffects(16);
        effectTimer.Start();
    }
    private void UpdateEffects(int deltaTime)
    {
        effects.RemoveAll(effect => effect.Update(deltaTime));
    }

    public void KeyDown(Keys key)
    {
        if (!gamePaused)
        {
            keysPressed.Add(key);
        }
    }

    public void KeyUp(Keys key)
    {
        if (!gamePaused)
        {
            keysPressed.Remove(key);
        }
    }

    public void Update()
    {
        if (gameOver || gamePaused) return; // 게임이 종료되거나 일시정지된 상태에서는 화면 업데이트를 중단
                                            // 간단하게 퍼즈 상태임. 이거 없으면 게임 종료 후 유령과 충돌 할 가능성 있음.
                                            // Update 함수 수정 금지. 최종 완성된 상태.

        // 키 상태에 따른 이동 처리
        if (keysPressed.Contains(Keys.Right)) pacman.SetDirection(0);
        if (keysPressed.Contains(Keys.Down)) pacman.SetDirection(1);
        if (keysPressed.Contains(Keys.Left)) pacman.SetDirection(2);
        if (keysPressed.Contains(Keys.Up)) pacman.SetDirection(3);

        // 팩맨의 이동 방향을 가져옴
        int direction = pacman.GetDirection();

        // 벽을 통과하지 않도록 이동 전에 체크
        if (gameMap.CanMove(pacman.GetBounds(), direction, 5))
        {
            pacman.Move(); // 이동이 가능하면 이동
        }

        // 아이템을 먹었는지 확인하고, 확인 되면 이벤트 발생.
        if (gameMap.EatItem(pacman.GetBounds(), out int itemType))
        {
            score += 1;  // 아이템을 먹으면 점수 증가
            if (itemType == 2) // 2번 아이템을 먹었을 경우
            {
                Point position = new Point(pacman.GetBounds().X, pacman.GetBounds().Y);
                effects.Add(new Effect(position, 500)); // 플러스 이펙트 추가 (500ms 지속)
            }
            else if (itemType == 3) // 3번 아이템을 먹었을 경우
            {
                /* 3번 아이템 이펙트 부분 테스트 종료. ( 시간 내 추가 불가능. )
                 * Point position = new Point(pacman.GetBounds().X, pacman.GetBounds().Y);
                circleEffects.Add(new CircleEffect(position
                */

                canPassGhosts = true; // 유령 통과 상태로 설정
                foreach (var ghost in ghosts)
                {
                    ghost.SetPassable(true); // 유령을 통과 가능 상태로 설정
                }
                passGhostsTimer.Start(); // 타이머 시작
            }
        }

        // 유령 이동 처리 (3초 후에만 이동)
        if (!ghostsFrozen)
        {
            foreach (var ghost in ghosts)
            {
                ghost.Move(gameMap);
            }
        }

        // 유령과 충돌 체크
        if (!canPassGhosts)
        {
            foreach (var ghost in ghosts)
            {
                if (pacman.GetBounds().IntersectsWith(ghost.GetBounds()))
                {
                    GameOver();
                    return;
                }
            }
        }

        // 남은 아이템 수를 확인하여 다음 스테이지로 이동
        if (gameMap.GetItemCount() == 0 && !stageCleared)
        {
            stageCleared = true; // 스테이지 클리어 상태로 설정
            gamePaused = true; // 게임 일시정지
            ShowClearMessage(); // 클리어 메시지 표시
        }

        // 팩맨 입 모양 업데이트
        pacman.ToggleMouth();  // 입 모양을 주기적으로 업데이트
    }

    public void Draw(Graphics g)
    {
        gameMap.DrawMap(g);
        pacman.Draw(g);

        // 유령 그리기
        foreach (var ghost in ghosts)
        {
            ghost.Draw(g);
        }
        // 이펙트 그리기
        foreach (var effect in effects)
        {
            effect.Draw(g);
        }
        /* // 원형 이펙트 그리기
        foreach (var circleEffect in circleEffects)
        {
            circleEffect.Draw(g);
        }
        */

        g.DrawString($"Score: {score}", new Font("Arial", 16), Brushes.White, 10, 10);
    }

    private void ShowClearMessage()
    {
        // 클리어 메시지 박스 표시
        var result = MessageBox.Show("스테이지를 클리어했습니다! 계속 진행하시겠습니까?", "스테이지 클리어", MessageBoxButtons.OK);

        // "계속 진행" 버튼을 클릭하면 다음 스테이지로 이동
        if (result == DialogResult.OK)
        {
            NextStage();
        }
    }

    private void NextStage()
    {
        int[,] newMap = new int[24, 21]
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 3, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 1},
            {1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1},
            {1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1},
            {1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1},
            {1, 2, 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1},
            {1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1},
            {1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 2, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 2, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 2, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 2, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1},
            {1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1},
            {1, 2, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 2, 1},
            {1, 2, 2, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 2, 2, 1},
            {1, 1, 2, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 2, 1, 1},
            {1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1},
            {1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1},
            {1, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        };


        // 다음 스테이지로 이동
        gameMap.SetMap(newMap); // 새로운 맵 생성
        pacman = new Pacman(400, 300); // 팩맨 초기 위치로 이동 << 이거 위치도 다음 맵을 보고 수정해야함.
        score = 0; // 점수 초기화 (원하는 경우 기존 점수를 유지할 수 있음)
        stageCleared = false; // 스테이지 클리어 상태 초기화
        gameOver = false; // 게임 종료 상태 초기화
        gamePaused = false; // 게임 일시정지 상태 초기화
        canPassGhosts = false; // 유령 통과 상태 초기화



        // 유령 초기화 *(게임 난이도 만들면 연동 필요.)
        // 현재 방식은 맵에 좌표를 찍어서 유령을 스폰시키는 구조인데,
        // 유령을 맵 내의 아이템 2번 위치에 유령이 랜덤으로 스폰되게 할 수 있는지?
        // ㄴ 불가능한건 아니지만 2번 위치에 유령을 스폰시키면,
        //    예외처리 하지 않는 이상 게임 시작 시 유령과 팩맨이 바로 겹칠 아주 작은 확률 존재.
        //    ㄴ ㅇㅋ 패스.
        ghosts = new List<Ghost>
        {
            new Ghost(220, 200, changedGhostSpeed),
            new Ghost(220, 200, changedGhostSpeed),
            new Ghost(620, 400, changedGhostSpeed),
            new Ghost(620, 400, changedGhostSpeed)
        };

        // 유령 움직임 타이머 초기화 << 이거 없으면 유령이 다음 스테이지 시작시 안 멈춤.
        // 필요없는 코드 제거 할 때 수정 X
        ghostsFrozen = true;
        ghostTimer.Start();
    }

    private void GameOver()
    {
        gameOver = true; // 게임 종료 상태로 설정 (게임 오버 메세지 중복 방지용) 마찬가지로 제거 금지.
        MessageBox.Show("게임 오버! 유령에 잡혔습니다.", "게임 오버", MessageBoxButtons.OK);
        Application.Exit(); // 게임 종료
    }
}
