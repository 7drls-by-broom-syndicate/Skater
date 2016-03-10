using UnityEngine;
using System.Collections;

public static class Speed {

    public const int min = 0;
    public const int max = 12;
    public const int samecourse = +1;
    public const int turn45 = -3;
    public const int turn90 = -6;
    public const int turn135 = -9;
    public const int turn180 = -Speed.max;

    public static int[] deltasbyrelativeheading = { samecourse, turn45, turn90, turn135, turn180, turn135, turn90, turn45 };

}
