using System;
using System.Collections;
using System.Collections.Generic;

namespace LD38Runner {
  public static class Utils {
    private static Random rng = new Random();

    public static T Sample<T>(this IList<T> list) {
      return list[rng.Next(list.Count)];
    }
  }
}
