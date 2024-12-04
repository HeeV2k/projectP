using System;
using System.Drawing;
using System.Windows.Forms;

public class Ghost
{
    private int x, y, direction;
    // 유령 크기 ( 팩맨과 동일한 25, 벽을 통과하기 좋은 크기 + 알고리즘을 짜기 용이함,
    // 너무 작을 경우 벽에 부딪히는 빈도수가 짧은 시간내에 데이터가 쌓이지 않음 )
    private int ghostSpeed; // 유령 이동 속도 ( 팩맨과 동일하게 설정함. ) ( 5로 설정한 난이도 높은 버전 요청 )
    private const int size = 25;
    private static Random random = new Random();
    private Timer directionChangeTimer;
    private bool isPassable = false; // 유령 통과 가능한 상태 bool 함수
    private Timer passableTimer; // 유령 통과 가능 상태 타이머
    // private int followProbability = 20; // 팩맨을 추적할 확률 << 높을수록 수직, 수평 이동 하려고 해서 벽에 충돌함.
                                        // 20으로 놔둘것.
    private int lastCollisionDirection = -1; // 마지막으로 충돌한 방향
    private int distanceSinceLastCollision = 0; // 충돌 후 이동한 거리

    public Ghost(int startX, int startY, int ghostSpeed)            // X, Y와 ghostSpeed를 넣어 Logic에서 관리 가능하게 수정.
    {
        x = startX;
        y = startY;
        direction = random.Next(0, 4); // 유령이 랜덤한 방향을 바라본채 시작하게 함.
        this.ghostSpeed = ghostSpeed;

        // 방향 변경 타이머 설정
        directionChangeTimer = new Timer();
        directionChangeTimer.Interval = 5000; // 5초마다 방향 변경 ( 최적임. 수정하지 말것 )
        directionChangeTimer.Tick += (sender, e) => ChangeDirection();
        directionChangeTimer.Start();

        // 유령 통과 가능 상태 타이머 설정
        passableTimer = new Timer();
        passableTimer.Interval = 5000; // 5초 동안 통과 가능 ( 3초는 너무 짧은 경향 있음 )
        passableTimer.Tick += (sender, e) =>
        {
            isPassable = false; // 통과 가능 상태 종료
            passableTimer.Stop(); // 타이머 멈춤
        };
    }

    public void SetGhostSpeedP(int ghostSpeed)
    { this.ghostSpeed = ghostSpeed; }

    private void ChangeDirection()
    {
        direction = random.Next(0, 4); // 랜덤하게 새로운 방향으로 변경 ( 따로 사용하는 함수임. 아래에서 사용 )
    }

    private void ChooseNewDirection(GameMap gameMap)
    {
        // 벽에 부딪힌 경우에 새로운 방향을 선택하게 하는 알고리즘
        // 그냥 랜덤으로 만들어 놓을 경우 너무 벽에 많이 부딪히는 문제 발생.
        // 해당 문제를 해결하기 위해 유령이 부딪힌 벽 방향을 일정 거리 이상 움직이기 전까지 제외하는 방식 선택.
        for (int i = 0; i < 4; i++)
        {
            int newDirection = random.Next(0, 4);
            if (newDirection == lastCollisionDirection) continue; // 충돌 방향 제외
            int futureX = x, futureY = y;
            switch (newDirection)
            {
                case 0: futureX += ghostSpeed; break; // 오른쪽
                case 1: futureY += ghostSpeed; break; // 아래
                case 2: futureX -= ghostSpeed; break; // 왼쪽
                case 3: futureY -= ghostSpeed; break; // 위
            }
            Rectangle futureBounds = new Rectangle(futureX - size / 2, futureY - size / 2, size, size);
            if (gameMap.CanMove(futureBounds, newDirection, 5))
            {
                direction = newDirection;
                return;
            }
        }
    }

    public void Move(GameMap gameMap)
    {
        // 유령 이동
        int futureX = x, futureY = y;

        // 랜덤하게 이동
        switch (direction)
        {
            case 0: futureX += ghostSpeed; break; // 오른쪽
            case 1: futureY += ghostSpeed; break; // 아래
            case 2: futureX -= ghostSpeed; break; // 왼쪽
            case 3: futureY -= ghostSpeed; break; // 위
        }

        Rectangle futureBounds = new Rectangle(futureX - size / 2, futureY - size / 2, size, size);
        if (gameMap.CanMove(futureBounds, direction, ghostSpeed))
        {
            x = futureX;
            y = futureY;
            distanceSinceLastCollision += ghostSpeed; // 충돌 후 이동 거리 증가
            if (distanceSinceLastCollision >= 80)
            {
                lastCollisionDirection = -1; // 일정 거리 이동 후 충돌 방향 초기화
            }
        }
        else
        {
            lastCollisionDirection = direction; // 충돌 방향 저장
            distanceSinceLastCollision = 0; // 이동 거리 초기화
            ChooseNewDirection(gameMap); // 새로운 방향 선택
        }
    }



    public void Draw(Graphics g)
    {
        // 유령 색상 설정 ( 이거 초록보다 주황색이 나을듯. 초록색 병 걸린거 같음 ) << 확인
        Brush ghostBrush = isPassable ? Brushes.Orange : Brushes.Red;
        g.FillEllipse(ghostBrush, x - size / 2, y - size / 2, size, size); // 유령 그리기
    }

    public void SetPassable(bool passable)
    {
        isPassable = passable;
        if (isPassable)
        {
            passableTimer.Start(); // 통과 가능 상태 타이머 시작
        }
        else
        {
            passableTimer.Stop(); // 통과 가능 상태 타이머 멈춤
        }
    }

    public Rectangle GetBounds() // << 이거 왜 있는 기능인지? << 게임 내 개체의 경계를 반환하는 메서드임. 수정X
                                 // ( 팩맨과 타 엔티티 충돌에 주로 사용 )
                                 // << 근데 사각형을 기준으로 계산하는거라 원형 기준 계산은 없나 생각해볼만함.
                                 // 수정할거면 예시 코드 Rectangle( X, Y는 개체의 중심 좌표, size는 개체의 크기 )
    {
        return new Rectangle(x - size / 2, y - size / 2, size, size);
    }
}

/*         아래 로직은 현재 짜여진 로직과 충돌함.
 *         고스트가 팩맨을 추적하는 간단한 알고리즘.
 *         if (random.Next(100) < followProbability)
        {
            // 팩맨을 추적하는 로직 ( 단순화 )
            if (Math.Abs(pacmanX - x) > Math.Abs(pacmanY - y))
            {
                // 팩맨이 가로 방향으로 더 멀다면 유령도 가로 방향으로 이동
                futureX += (pacmanX > x) ? speed : -speed;
            }
            else
            {
                // 팩맨이 세로 방향으로 더 멀다면 유령도 세로 방향으로 이동
                futureY += (pacmanY > y) ? speed : -speed;
            }
        }
        else
        {
            // 랜덤하게 이동
            switch (direction)
            {
                case 0: futureX += speed; break; // 오른쪽
                case 1: futureY += speed; break; // 아래
                case 2: futureX -= speed; break; // 왼쪽
                case 3: futureY -= speed; break; // 위
            }
        }
        

        Rectangle futureBounds = new Rectangle(futureX - size / 2, futureY - size / 2, size, size);
*/