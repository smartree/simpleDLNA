using System;
using System.Collections.Generic;

namespace NMaier.SimpleDlna.Utilities
{
  public abstract class Repository<TInterface>
    where TInterface : class, IRepositoryItem
  {
    private static readonly Dictionary<string, TInterface> items = BuildRepository();


    private static Dictionary<string, TInterface> BuildRepository()
    {
      var items = new Dictionary<string, TInterface>();
      var type = typeof(TInterface).Name;
      var a = typeof(TInterface).Assembly;
      foreach (Type t in a.GetTypes()) {
        if (t.GetInterface(type) == null) {
          continue;
        }
        var ctor = t.GetConstructor(new Type[] { });
        if (ctor == null) {
          continue;
        }
        try {
          var item = ctor.Invoke(new object[] { }) as TInterface;
          if (item == null) {
            continue;
          }
          items.Add(item.Name, item);
        }
        catch (Exception) {
          continue;
        }
      }
      return items;
    }


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    public static IDictionary<string, string> ListItems()
    {
      var rv = new Dictionary<string, string>();
      foreach (var v in items.Values) {
        rv.Add(v.Name, v.Description);
      }
      return rv;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    public static TInterface Lookup(string name)
    {
      if (string.IsNullOrWhiteSpace(name)) {
        throw new ArgumentException("Invalid repository name", "name");
      }
      name = name.ToLower().Trim();
      var result = (TInterface)null;
      if (!items.TryGetValue(name, out result)) {
        throw new RepositoryLookupException(name);
      }
      return result;
    }
  }
}