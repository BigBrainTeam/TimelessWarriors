using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class for Character specific scripts.
/// </summary>
public class Speciality : MonoBehaviour {

    [SerializeField]
    GameObject UI;
    protected GameObject instanceUI;
    protected Character user;

    void Start () {
        initComponents();
	}

    virtual protected void initComponents() {
        user = this.GetComponent<Character>();
    }
    virtual public void onBlock() { }
    virtual public void onJump() { }
    virtual public void resetJump(int jumps) { }
    virtual public void ultimateUpdate() { }
    virtual public void startUltimate() {
        user.HurtBox.enabled = false;
    }
    virtual public void endUltimate() {
        user.HurtBox.enabled = true;
    }

    virtual public void instantiateSpecialityUI(Transform parent)
    {
        instanceUI = Instantiate(UI, parent, false);
    }
}
