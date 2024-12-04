using System.Data;
using System.Drawing;

public class GameMap
{
    private int[,] map;
    private int cellSize = 40; // 각 셀의 크기

    public int MapWidth => map.GetLength(1);
    public int MapHeight => map.GetLength(0);
    public int CellSize => cellSize;

    public GameMap()
    {
        // 초기 맵 설정
        // 진짜 힘들었다. << King 20학번

        map = new int[24, 21]
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1},
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
            {1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        };
    }

    public void SetMap(int[,] newMap)
    {
        map = newMap;
    }


    public int GetMapValue(int row, int col)   // 2차원 배열로 설정된 맵의 행(row)과 열(column)<< 줄여서 col로 명칭, 을 반환함.
                                               // 해당 반환으로 Ghost 클래스에서 좌표 평면 위에 있는 유령이
                                               // 벽과 길을 2차원 배열로부터 인지 할 수 있게 됨.
                                               // 더 자세한 사항은 Ghost 클래스의 Ispath 확인 할 것.
                                               // 현재 테스트 중인 부분임.
    {
        if (row >= 0 && row < map.GetLength(0) && col >= 0 && col < map.GetLength(1))
        {
            return map[row, col];
        }
        return 1; // 기본값으로 벽(1)을 반환
    }


    public void DrawMap(Graphics g)
    {
        // 배경 검정색
        g.Clear(Color.Black);

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1) 
                {
                    g.FillRectangle(Brushes.Black, x * cellSize, y * cellSize, cellSize, cellSize); // 벽 내부 검정색

                    if (x == 0 || map[y, x - 1] != 1) // 왼쪽 벽 그리기
                    {
                        g.DrawLine(new Pen(Color.Blue, 3), x * cellSize, y * cellSize, x * cellSize, (y + 1) * cellSize);
                    }

                    if (y == 0 || map[y - 1, x] != 1) // 위쪽 벽 그리기
                    {
                        g.DrawLine(new Pen(Color.Blue, 3), x * cellSize, y * cellSize, (x + 1) * cellSize, y * cellSize);
                    }

                    if (x == map.GetLength(1) - 1 || map[y, x + 1] != 1) // 오른쪽 벽 그리기
                    {
                        g.DrawLine(new Pen(Color.Blue, 3), (x + 1) * cellSize, y * cellSize, (x + 1) * cellSize, (y + 1) * cellSize);
                    }

                    if (y == map.GetLength(0) - 1 || map[y + 1, x] != 1) // 아래쪽 벽 그리기
                    {
                        g.DrawLine(new Pen(Color.Blue, 3), x * cellSize, (y + 1) * cellSize, (x + 1) * cellSize, (y + 1) * cellSize);
                    }
                }
                else if (map[y, x] == 2)  // 아이템 그리기
                {
                    g.FillEllipse(Brushes.White, x * cellSize + 15, y * cellSize + 15, cellSize - 30, cellSize - 30);
                }
                else if (map[y, x] == 3)  // 유령을 통과할 수 있는 아이템 (3번)
                {
                    g.FillEllipse(Brushes.White, x * cellSize + 10, y * cellSize + 10, cellSize - 20, cellSize - 20);
                }
            }
        }
    }

public bool CanMove(Rectangle bounds, int direction, int speed)
    {
        // 팩맨의 새로운 위치를 계산
        Rectangle futureBounds = bounds;
        switch (direction)
        {
            case 0: // 오른쪽
                futureBounds.X += speed;
                break;
            case 1: // 아래
                futureBounds.Y += speed;
                break;
            case 2: // 왼쪽
                futureBounds.X -= speed;
                break;
            case 3: // 위
                futureBounds.Y -= speed;
                break;
        }

        // 팩맨 겉면이 벽과 충돌하는지 확인
        int left = futureBounds.Left / cellSize;
        int right = futureBounds.Right / cellSize;
        int top = futureBounds.Top / cellSize;
        int bottom = futureBounds.Bottom / cellSize;

        // 좌표가 배열 범위를 벗어나지 않도록 확인
        // 이거 수정 X << 테스트 용도긴 한데 나중에 맵 완성하면 지워도 무방함.
        // 캐릭터가 배열 밖에 있을때 강제종료되는거 방지용.
        if (left < 0 || right >= map.GetLength(1) || top < 0 || bottom >= map.GetLength(0))
        {
            return false; // 배열 범위를 벗어나면 이동 불가
        }

        // 팩맨 겉면 박스의 모든 모서리가 벽과 충돌하지 않는지 확인
        if (map[top, left] == 1 || map[top, right] == 1 || map[bottom, left] == 1 || map[bottom, right] == 1)
        {
            return false;
        }

        return true;
    }

    public bool EatItem(Rectangle bounds, out int itemType)
    {
        int mapX = bounds.X / cellSize;
        int mapY = bounds.Y / cellSize;

        // 좌표가 배열 범위를 벗어나지 않도록 확인
        // 이것도 마찬가지로 수정 X << 테스트 용도긴 한데 나중에 맵 완성하면 지워도 무방함.
        if (mapX < 0 || mapX >= map.GetLength(1) || mapY < 0 || mapY >= map.GetLength(0))
        {
            itemType = 0;
            return false;
        }

        itemType = map[mapY, mapX];
        if (itemType == 2 || itemType == 3)  // 아이템을 먹으면 점수 증가
        {
            map[mapY, mapX] = 0;  // 아이템 먹음
            return true;
        }
        return false;
    }

    public int GetItemCount()
    {
        int count = 0;
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 2 || map[y, x] == 3)
                {
                    count++;
                }
            }
        }
        return count;
    }
}
