public class BeingInfo
{
      private static int _currentId;
      public BeingInfo(string name)
      {
            Name = name;
            Id = _currentId++;
      }

      public string Name { get; }
      public int Id { get; private set; }
}