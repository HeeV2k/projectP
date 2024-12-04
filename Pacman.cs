using System.Drawing;
using System;

public class Pacman
{
    private int x, y, direction;
    private const int size = 25;  // 팩맨 크기
                                  // 30으로 수정 << 초기 크기인 38은 너무 빡빡함. 한칸짜리 공간에 들어갈수가 없음.
                                  // 25로 수정 << 회의 결과 25 고정.

    private int pacmanSpeed = 3;

    private bool isMouthOpen;
    private int mouthAngleStart, mouthAngleSweep;
    private int mouthSpeed = 60; // 입의 속도 (주기적으로 업데이트)
                                 // ㄴ 입을 더 크게 벌리게 할 수 있는지?
                                 //    ㄴ 가능. 수정 했음. 추가적으로 수정할거면 아래 각도 수정 요청.

    public Pacman(int startX, int startY)
    {
        x = startX + 20;
        y = startY + 20;
        direction = 340; // 초기 방향: 오른쪽
        isMouthOpen = true;
        mouthAngleStart = 0;
        mouthAngleSweep = 90;  // 입을 여는 각도 초기화
    }

    public void SetDirection(int newDirection)
    {
        direction = newDirection;
        SetMouthDirection();  // 팩맨의 방향에 맞게 입 설정
    }

    public void Move()
    {
        // 팩맨 이동
        switch (direction)
        {
            case 0: // 오른쪽
                x += pacmanSpeed;
                break;
            case 1: // 아래
                y += pacmanSpeed;
                break;
            case 2: // 왼쪽
                x -= pacmanSpeed;
                break;
            case 3: // 위
                y -= pacmanSpeed;
                break;
        }
    }

    public void ToggleMouth()
    {
        // 입의 상태를 주기적으로 토글
        if (isMouthOpen)
        {
            mouthAngleSweep = Math.Min(mouthAngleSweep + mouthSpeed, 90);  // 입을 여는 동작
        }
        else
        {
            mouthAngleSweep = Math.Max(mouthAngleSweep - mouthSpeed, 0);  // 입을 닫는 동작
        }

        // 입이 완전히 열리거나 닫히면 상태 변경
        if (mouthAngleSweep == 90 || mouthAngleSweep == 0)
        {
            isMouthOpen = !isMouthOpen;
        }
    }

    private void SetMouthDirection()
    {
        // 팩맨의 진행 방향에 맞춰 입 모양을 설정
        // 현재 340도부터 시작하는게 오류가 아님. 팩맨이 입을 다 벌리면 각도를 따로 계산해야하는데 생각 이슈로 이렇게 대처.
        // ㄴ 낫배드.


        switch (direction)
        {
            case 0: mouthAngleStart = 340; break;  // 오른쪽
            case 1: mouthAngleStart = 70; break;  // 아래
            case 2: mouthAngleStart = 160; break;  // 왼쪽
            case 3: mouthAngleStart = 250; break;  // 위
        }
    }

    public void Draw(Graphics g)
    {
        g.FillEllipse(Brushes.Yellow, x - size / 2, y - size / 2, size, size);  // 팩맨 중심 맞추기

        g.FillPie(Brushes.Black, x - size / 2, y - size / 2, size, size, mouthAngleStart, mouthAngleSweep / 2);
        // 위에 340도부터 시작하는걸 그냥 90도 180도 고정하고, Fillpie 두개 사용해서 하나는 위로 열고 하나는 아래로 열고
        // 닫히면 합쳐지는 형태로 애니메이션 불가?
        // ㄴ 불가능, Black 애니메이션 두개를 써봤는데 이상하게 나옴. 기괴함.
        //    ㄴ ㅇㅋ.

    }

    public int GetDirection()
    {
        return direction;
    }

    public Rectangle GetBounds()
    {
        return new Rectangle(x - size / 2, y - size / 2, size, size);  // 크기 반영된 bounds
    }
}
