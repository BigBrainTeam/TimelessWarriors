using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Hitter {
    void knockBackEntity(Entity target, float totalKnockBack);
    void onHit(float frames);
}
