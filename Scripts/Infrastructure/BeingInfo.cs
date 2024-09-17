using System;

public class BeingInfo
{
      #region Public properties
      public int Id { get; private set; }
      public event Action<string> NameChangedEvent;
      #endregion Public properties

      #region Private fields
      private static int currentId;
      private string name;
      #endregion Private fields

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
}