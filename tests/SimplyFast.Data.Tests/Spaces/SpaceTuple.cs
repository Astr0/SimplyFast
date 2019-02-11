
namespace SimplyFast.Data.Tests.Spaces
{
    public class SpaceTuple
    {
        public SpaceTuple(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        private bool Equals(SpaceTuple other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SpaceTuple) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X*397) ^ Y;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}