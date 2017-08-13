using System;

[Serializable]
public struct Coord
{

    public int x;
    public int y;

    public Coord(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
    }

    public static bool operator ==(Coord c1, Coord c2)
    {
        return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator !=(Coord c1, Coord c2)
    {
        return !(c1 == c2);
    }

    public override string ToString()
    {
        return x.ToString() + "," + y.ToString();
    }

    public string ToString(string Seperator)
    {
        return x.ToString() + Seperator + y.ToString();
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Coord obj)
    {
        return this == obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
