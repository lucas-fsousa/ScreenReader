namespace PublicUtility.ScreenReader.Structs {
  public readonly record struct PixelColor(byte Alpha, byte Red, byte Green, byte Blue);

  public readonly record struct ScreenSize(int Width, int Height);

  public readonly record struct PointIntoScreen(int X, int Y);

  public readonly record struct BoxOfScreen(ScreenSize Size, PointIntoScreen Point) {
    public readonly bool Filled = (Size.Width > 0 && Size.Height > 0) & (Point.Y > 0 && Point.X > 0);

    public PointIntoScreen GetCenterBox() =>  new(Point.X + (Size.Width / 2), Point.Y + (Size.Height / 2));
    public PointIntoScreen GetEndBox() => new(Point.X + Size.Width, Point.Y + Size.Height);
    public PointIntoScreen GetStartBox() => new(Point.X, Point.Y);
  }
  
}
