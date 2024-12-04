using System.Collections.Generic;
using System.Drawing;
using System;

public class Effect
{
    private Random random = new Random();
    public List<Point> Positions { get; private set; }
    public int Duration { get; private set; }
    private int elapsedTime;
    private List<PlusShape> plusShapes;

    public Effect(Point position, int duration)
    {
        Duration = duration;
        elapsedTime = 0;
        Positions = GenerateRandomPositions(position); // 팩맨 근처에 랜덤 포지션 생성
        plusShapes = new List<PlusShape>
        {
            new PlusShape(Positions[0], 2),  // 거의 원형
            new PlusShape(Positions[1], 10), // 살짝 작은 +
            new PlusShape(Positions[2], 1),  // 심심해서 작은거 하나 추가
            new PlusShape(Positions[3], 1)  // 조금 더 심심해서 원형 하나 추가
        };
    }

    private List<Point> GenerateRandomPositions(Point center)
    {
        List<Point> positions = new List<Point>();
        for (int i = 0; i < 4; i++) // 4개의 플러스 이펙트 생성
        {
            int offsetX = random.Next(-20, 20);
            int offsetY = random.Next(-20, 20);
            positions.Add(new Point(center.X + offsetX, center.Y + offsetY));
        }
        return positions;
    }

    public bool Update(int deltaTime)
    {
        elapsedTime += deltaTime;
        foreach (var plusShape in plusShapes)
        {
            plusShape.Update(deltaTime);
        }
        return elapsedTime >= Duration;
    }

    public void Draw(Graphics g)
    {
        foreach (var plusShape in plusShapes)
        {
            plusShape.Draw(g);
        }
    }

    private class PlusShape
    {
        public Point Position { get; private set; }
        public int Size { get; private set; }
        private int elapsedTime;
        private int duration;

        public PlusShape(Point position, int size)
        {
            Position = position;
            Size = size;
            duration = 500; // 이펙트 지속 시간
            elapsedTime = 0;
        }

        public void Update(int deltaTime)
        {
            elapsedTime += deltaTime;
        }

        public void Draw(Graphics g)
        {
            int alpha = 255 - (255 * elapsedTime / duration); // 점점 투명해지는 효과
            Color effectColor = Color.FromArgb(alpha, Color.Yellow);
            using (Brush brush = new SolidBrush(effectColor))
            {
                // 플러스 모양 그리기
                int halfSize = Size / 2;
                // 수직 선
                g.FillRectangle(brush, Position.X - 2, Position.Y - halfSize, 4, Size);
                // 수평 선
                g.FillRectangle(brush, Position.X - halfSize, Position.Y - 2, Size, 4);
            }
        }
    }
}


/*
 * 시간상 원형 이펙트까지는 제작 불가.
 * 너무 어렵기도 하고 퍼지는 원의 제작이 쉽지 않음.
public class CircleEffect
{
    public Point Position { get; private set; }
    public int Duration { get; private set; }

    public CircleEffect(Point position, int duration)
    {
        Position = position;
        Duration = duration;
        elapsedTime = 0;
    }
}

*/