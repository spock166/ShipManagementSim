using System;

public class BeingInfo
{
      private static int currentId;
      private string name;

      public BeingInfo(string name)
      {
            Name = name;
            Id = currentId++;
      }

      public string Name
      {
            get
            {
                  return name;
            }

            protected set
            {
                  name = value;
                  NameChangedEvent?.Invoke(value);
            }
      }
      public int Id { get; private set; }
      public event Action<string> NameChangedEvent;
}